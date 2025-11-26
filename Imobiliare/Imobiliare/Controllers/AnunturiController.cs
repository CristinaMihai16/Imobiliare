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
    public class AnunturiController : Controller
    {
        private readonly ImobiliareContext _context;

        public AnunturiController(ImobiliareContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var anunturi = await _context.Anunturi
                .OrderByDescending(a => a.Data_publicare)
                .ToListAsync();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<int?> favoriteIds = new List<int?>();

            if (userIdString != null)
            {
                int userId = int.Parse(userIdString);

                favoriteIds = await _context.Favorite
                    .Where(f => f.ID_Utilizator == userId)
                    .Select(f => f.ID_Anunt)
                    .ToListAsync();
            }

            ViewBag.FavoriteIds = favoriteIds;

            return View(anunturi);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anunturi = await _context.Anunturi
                .Include(a => a.Utilizator)
                .FirstOrDefaultAsync(m => m.ID_Anunt == id);
            if (anunturi == null)
            {
                return NotFound();
            }

            return View(anunturi);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Anunturi model, IFormFile ImagineFile)
        {
            ModelState.Remove(nameof(model.ID_Utilizator));
            ModelState.Remove(nameof(model.Utilizator));
            ModelState.Remove(nameof(model.Imagine_anunt));
            ModelState.Remove(nameof(model.Conversatie));

            
            ModelState.Remove("ImagineFile");  

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ID_Utilizator = int.Parse(userIdString);

            
            if (ImagineFile != null && ImagineFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ImagineFile.CopyToAsync(memoryStream);
                    model.Imagine_anunt = memoryStream.ToArray();
                }
            }
            else
            {
                model.Imagine_anunt = null;
            }

            model.Data_publicare = DateTime.UtcNow;

            if (!ModelState.IsValid)
                return View(model);

            _context.Add(model);
            await _context.SaveChangesAsync();

            TempData["succes"] = "Anunț adăugat cu succes!";
            return RedirectToAction(nameof(Index));
        }


        // GET: Anunturi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anunturi = await _context.Anunturi.FindAsync(id);
            if (anunturi == null)
            {
                return NotFound();
            }
            return View(anunturi);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Anunturi model, IFormFile ImagineFile)
        {
            if (id != model.ID_Anunt)
                return NotFound();

            var anuntExistent = await _context.Anunturi
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ID_Anunt == id);

            if (anuntExistent == null)
                return NotFound();

            ModelState.Remove(nameof(model.Imagine_anunt));
            ModelState.Remove("ImagineFile");   
            ModelState.Remove(nameof(model.Data_publicare));
            ModelState.Remove(nameof(model.ID_Utilizator));

            if (!ModelState.IsValid)
            {
                model.Imagine_anunt = anuntExistent.Imagine_anunt;
                return View(model);
            }

            try
            {
                if (ImagineFile != null && ImagineFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await ImagineFile.CopyToAsync(ms);
                    model.Imagine_anunt = ms.ToArray();
                }
                else
                {
                    model.Imagine_anunt = anuntExistent.Imagine_anunt;
                }

                model.Data_publicare = anuntExistent.Data_publicare;
                model.ID_Utilizator = anuntExistent.ID_Utilizator;

                _context.Update(model);
                await _context.SaveChangesAsync();

                TempData["succes"] = "Anunțul a fost modificat!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["eroare"] = ex.Message;
                model.Imagine_anunt = anuntExistent.Imagine_anunt;
                return View(model);
            }
        }



        // GET: Anunturi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anunturi = await _context.Anunturi
                .FirstOrDefaultAsync(m => m.ID_Anunt == id);
            if (anunturi == null)
            {
                return NotFound();
            }

            return View(anunturi);
        }

        // POST: Anunturi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anunturi = await _context.Anunturi.FindAsync(id);
            if (anunturi != null)
            {
                _context.Anunturi.Remove(anunturi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnunturiExists(int id)
        {
            return _context.Anunturi.Any(e => e.ID_Anunt == id);
        }
        [Authorize]
        public async Task<IActionResult> AnunturileMele()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdString);

            var anunturileMele = await _context.Anunturi
                .Where(a => a.ID_Utilizator == userId)
                .OrderByDescending(a => a.Data_publicare)
                .ToListAsync();

            return View(anunturileMele);
        }
    }
}
