using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Imobiliare.Data;
using Imobiliare.Models;
using System.Security.Claims;          
using Microsoft.AspNetCore.Authorization; 

namespace Imobiliare.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly ImobiliareContext _context;

        public FavoriteController(ImobiliareContext context)
        {
            _context = context;
        }

        // GET: Favorite 
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Anunturi"); 
            }

            var favoriteleMele = _context.Favorite
                .Include(f => f.Anunturi)
                .Where(f => f.ID_Utilizator == userId); 

            return View(await favoriteleMele.ToListAsync());
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite([FromForm] int anuntId, [FromForm] string action)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "Eroare: Utilizatorul nu este autentificat sau ID-ul este invalid." });
            }

            var existingFavorite = await _context.Favorite
                .FirstOrDefaultAsync(f => f.ID_Anunt == anuntId && f.ID_Utilizator == userId);

            if (action == "add")
            {
                if (existingFavorite == null)
                {
                    var newFavorite = new Favorite
                    {
                        ID_Anunt = anuntId,
                        ID_Utilizator = userId,
                        Data_adaugare = DateTime.UtcNow 
                    };
                    _context.Favorite.Add(newFavorite);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Adăugat la favorite." });
                }
                return Json(new { success = false, message = "Anunțul este deja în favorite." });
            }
            else if (action == "remove")
            {
                if (existingFavorite != null)
                {
                    _context.Favorite.Remove(existingFavorite);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Eliminat din favorite." });
                }
                return Json(new { success = false, message = "Anunțul nu se află în favorite." });
            }

            return Json(new { success = false, message = "Acțiune invalidă specificată." });
        }


        // GET: Favorite/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }

            var favorite = await _context.Favorite
                .Include(f => f.Anunturi)
                .Include(f => f.Utilizator)
                .FirstOrDefaultAsync(m => m.ID_Favorite == id);

            if (favorite == null) { return NotFound(); }

            if (favorite.ID_Utilizator.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid(); 
            }

            return View(favorite);
        }

        
        // GET: Favorite/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View();
        }

        // POST: Favorite/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID_Favorite,ID_Anunt,Data_adaugare")] Favorite favorite)
        {
            return View(favorite);
        }

        // GET: Favorite/Edit/5 
        public async Task<IActionResult> Edit(int? id)
        {
            return View();
        }

        // POST: Favorite/Edit/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID_Favorite,ID_Utilizator,ID_Anunt,Data_adaugare")] Favorite favorite)
        {
            return View(favorite);
        }

        // GET: Favorite/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return View(await _context.Favorite.FirstOrDefaultAsync(m => m.ID_Favorite == id));
        }

        // POST: Favorite/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var favorite = await _context.Favorite.FindAsync(id);
            if (favorite != null)
            {
                _context.Favorite.Remove(favorite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoriteExists(int id)
        {
            return _context.Favorite.Any(e => e.ID_Favorite == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var favs = _context.Favorite.Where(f => f.ID_Utilizator == userId);

            _context.Favorite.RemoveRange(favs);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}