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
using System.IO;

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
            if (id == null) return NotFound();

            var anunt = await _context.Anunturi
                .Include(a => a.Utilizator)
                .Include(a => a.Imagini) 
                .FirstOrDefaultAsync(m => m.ID_Anunt == id);

            if (anunt == null) return NotFound();

            return View(anunt);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Anunturi model, List<IFormFile> ImaginiFiles)
        {
            ModelState.Remove(nameof(model.ID_Utilizator));
            ModelState.Remove(nameof(model.Utilizator));
            ModelState.Remove(nameof(model.Conversatie));
            ModelState.Remove(nameof(model.Imagini));

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString != null)
            {
                model.ID_Utilizator = int.Parse(userIdString);
            }
            model.Data_publicare = DateTime.UtcNow;

            if (ImaginiFiles != null && ImaginiFiles.Count > 0)
            {
                model.Imagini = new List<ImaginiAnunt>();

                foreach (var file in ImaginiFiles)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            var imagineNoua = new ImaginiAnunt
                            {
                                Imagine = ms.ToArray()
                            };
                            model.Imagini.Add(imagineNoua);
                        }
                    }
                }

                if (model.Imagini.Any())
                {
                    model.Imagine_anunt = model.Imagini.First().Imagine;
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Add(model);
            await _context.SaveChangesAsync();

            TempData["succes"] = "Anunț adăugat cu succes!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var anunt = await _context.Anunturi
                .Include(a => a.Imagini)
                .FirstOrDefaultAsync(m => m.ID_Anunt == id);

            if (anunt == null) return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || anunt.ID_Utilizator != int.Parse(userIdString))
            {
                TempData["eroare"] = "Nu ai dreptul să editezi acest anunț!";
                return RedirectToAction(nameof(Index));
            }
            

            return View(anunt);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Anunturi model, IFormFile? ImagineFile, List<IFormFile> ImaginiNoi)
        {
            if (id != model.ID_Anunt) return NotFound();

            ModelState.Remove(nameof(model.ID_Utilizator));
            ModelState.Remove(nameof(model.Utilizator));
            ModelState.Remove(nameof(model.Conversatie));
            ModelState.Remove(nameof(model.Imagini));
            ModelState.Remove("ImagineFile");
            ModelState.Remove("ImaginiNoi");
            ModelState.Remove(nameof(model.Data_publicare));
            ModelState.Remove(nameof(model.Imagine_anunt));

            var anuntDinDb = await _context.Anunturi
                .Include(a => a.Imagini)
                .FirstOrDefaultAsync(a => a.ID_Anunt == id);

            if (anuntDinDb == null) return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || anuntDinDb.ID_Utilizator != int.Parse(userIdString))
            {
                TempData["eroare"] = "Tentativă de editare neautorizată!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                anuntDinDb.Denumire = model.Denumire;
                anuntDinDb.Descriere = model.Descriere;
                anuntDinDb.Pret = model.Pret;
                anuntDinDb.Oras = model.Oras;
                anuntDinDb.Locatie = model.Locatie;
                anuntDinDb.Tranzactie = model.Tranzactie;
                anuntDinDb.TipProprietate = model.TipProprietate;

                if (ImagineFile != null && ImagineFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await ImagineFile.CopyToAsync(ms);
                    anuntDinDb.Imagine_anunt = ms.ToArray();
                }

                if (ImaginiNoi != null && ImaginiNoi.Count > 0)
                {
                    if (anuntDinDb.Imagini == null)
                    {
                        anuntDinDb.Imagini = new List<ImaginiAnunt>();
                    }

                    foreach (var file in ImaginiNoi)
                    {
                        if (file.Length > 0)
                        {
                            using var ms = new MemoryStream();
                            await file.CopyToAsync(ms);

                            var imgNoua = new ImaginiAnunt
                            {
                                Imagine = ms.ToArray()
                            };

                            anuntDinDb.Imagini.Add(imgNoua);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                TempData["succes"] = "Modificările au fost salvate!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                string mesaj = ex.Message;
                if (ex.InnerException != null) mesaj += " | " + ex.InnerException.Message;
                TempData["eroare"] = mesaj;

                model.Imagini = anuntDinDb.Imagini;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> StergeImagine(int id, int ID_Anunt)
        {
            var imagine = await _context.ImaginiAnunt
                .Include(i => i.Anunt) 
                .FirstOrDefaultAsync(i => i.ID_ImaginiAnunt == id);

            if (imagine != null)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString == null || imagine.Anunt.ID_Utilizator != int.Parse(userIdString))
                {
                    TempData["eroare"] = "Nu poți șterge imagini care nu îți aparțin!";
                    return RedirectToAction(nameof(Index));
                }

                _context.ImaginiAnunt.Remove(imagine);
                await _context.SaveChangesAsync();
                TempData["succes"] = "Imaginea a fost ștearsă!";
            }

            if (ID_Anunt > 0)
            {
                return RedirectToAction(nameof(Edit), new { id = ID_Anunt });
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var anunt = await _context.Anunturi
                .Include(a => a.Utilizator)
                .Include(a => a.Imagini)
                .FirstOrDefaultAsync(m => m.ID_Anunt == id);

            if (anunt == null) return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool isOwner = userIdString != null && anunt.ID_Utilizator == int.Parse(userIdString);

            bool isAdmin = User.IsInRole("Administrator");

            if (!isOwner && !isAdmin)
            {
                TempData["eroare"] = "Nu ai dreptul să ștergi acest anunț!";
                return RedirectToAction(nameof(Index));
            }

            return View(anunt);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anunt = await _context.Anunturi
                .Include(a => a.Imagini)
                .Include(a => a.Conversatie)
                .FirstOrDefaultAsync(m => m.ID_Anunt == id);

            if (anunt != null)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                bool isOwner = userIdString != null && anunt.ID_Utilizator == int.Parse(userIdString);
                bool isAdmin = User.IsInRole("Administrator");

                if (!isOwner && !isAdmin)
                {
                    TempData["eroare"] = "Acțiune neautorizată!";
                    return RedirectToAction(nameof(Index));
                }
               
                if (anunt.Imagini != null && anunt.Imagini.Any())
                {
                    _context.ImaginiAnunt.RemoveRange(anunt.Imagini);
                }

                if (anunt.Conversatie != null && anunt.Conversatie.Any())
                {
                    // _context.Conversatii.RemoveRange(anunt.Conversatie);
                }

                _context.Anunturi.Remove(anunt);
                await _context.SaveChangesAsync();

                TempData["succes"] = "Anunțul a fost șters definitiv.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> AnunturileMele()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdString);

            var anunturileMele = await _context.Anunturi
                .Where(a => a.ID_Utilizator == userId)
                .OrderByDescending(a => a.Data_publicare)
                .ToListAsync();

            return View(anunturileMele);
        }

        private bool AnunturiExists(int id)
        {
            return _context.Anunturi.Any(e => e.ID_Anunt == id);
        }
    }
}