using System.Diagnostics;
using Imobiliare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Imobiliare.Data;
using System.Security.Claims; // NECESAR pentru ID utilizator

namespace Imobiliare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ImobiliareContext _context;

        public HomeController(ILogger<HomeController> logger, ImobiliareContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Luăm cele mai noi 3 anunțuri
            var anunturiRecente = await _context.Anunturi
                .OrderByDescending(a => a.Data_publicare)
                .Take(3)
                .ToListAsync();

            // 2. Obținem ID-ul utilizatorului logat (dacă există)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<int?> favoriteIds = new List<int?>();


            // 3. Dacă utilizatorul este logat, extragem favoritele lui
            if (userIdString != null)
            {
                int userId = int.Parse(userIdString);

                favoriteIds = await _context.Favorite
                    .Where(f => f.ID_Utilizator == userId)
                    .Select(f => f.ID_Anunt)
                    .ToListAsync();
            }

            // 4. Trimitem ID-urile favorite către View (pentru inimioare colorate)
            ViewBag.FavoriteIds = favoriteIds;

            // 5. Întoarcem view-ul cu anunțurile
            return View(anunturiRecente);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
