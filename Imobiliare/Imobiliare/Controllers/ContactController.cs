using Imobiliare.Data;
using Imobiliare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // RĂMÂNE NECESAR PENTRU User.FindFirstValue

namespace Imobiliare.Controllers
{
    public class ContactController : Controller
    {
        private readonly ImobiliareContext _context;

        public ContactController(ImobiliareContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Formular formular)
        {
            if (!ModelState.IsValid)
                return View(formular);

            try
            {
                formular.Data_trimitere = DateTime.UtcNow;

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int userId = 0;

                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out userId))
                {
                    formular.IdUtilizator = userId;
                }
                else
                {
                    formular.IdUtilizator = 0;
                }

                _context.Formulare.Add(formular);
                _context.SaveChanges();

                TempData["succes"] = "Formularul a fost trimis!";
                return RedirectToAction("Confirmare");
            }
            catch (Exception ex)
            {
                TempData["eroare"] = "Eroare: " + ex.Message;
                return View(formular);
            }
        }

        public IActionResult Confirmare()
        {
            return View();
        }

    }
}