using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Imobiliare.Data;
using Imobiliare.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Imobiliare.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ImobiliareContext _context;

        public ChatController(ImobiliareContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? idConversatieActiva)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");
            int currentUserId = int.Parse(userIdString);


            if (idConversatieActiva.HasValue)
            {

                var mesajeDeMarcat = await _context.Mesaje
                    .Where(m => m.ID_Conversatie == idConversatieActiva.Value
                                && m.Status == "Necitit"
                                && m.ID_Utilizator_expeditor != currentUserId)
                    .ToListAsync();

                if (mesajeDeMarcat.Any())
                {
                    foreach (var mesaj in mesajeDeMarcat)
                    {
                        mesaj.Status = "Citit";
                    }
                    await _context.SaveChangesAsync();
                }
            }
           
            var conversatii = await _context.Conversatii
                .Include(c => c.Utilizator_client)
                .Include(c => c.Utilizator_proprietar)
                .Include(c => c.Anunturi)
                .Include(c => c.ListaMesaje)
                .Where(c => c.ID_Utilizator_client == currentUserId || c.ID_Utilizator_proprietar == currentUserId)
                .ToListAsync();

            conversatii = conversatii
                .OrderByDescending(c => c.ListaMesaje != null && c.ListaMesaje.Any()
                    ? c.ListaMesaje.Max(m => m.Data)
                    : DateTime.MinValue)
                .ToList();

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.ActiveConversationId = idConversatieActiva;

            return View(conversatii);
        }


        public async Task<IActionResult> StartChat(int idAnunt)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId = int.Parse(userIdString);

            var anunt = await _context.Anunturi.FindAsync(idAnunt);
            if (anunt == null) return NotFound();

            if (anunt.ID_Utilizator == currentUserId)
                return RedirectToAction("Index");

            var conversatieExistenta = await _context.Conversatii
                .FirstOrDefaultAsync(c => c.ID_Anunt == idAnunt &&
                                          c.ID_Utilizator_client == currentUserId &&
                                          c.ID_Utilizator_proprietar == anunt.ID_Utilizator);

            if (conversatieExistenta != null)
            {
                return RedirectToAction("Index", new { idConversatieActiva = conversatieExistenta.ID_Conversatie });
            }

            var nouaConversatie = new Conversatie
            {
                ID_Anunt = idAnunt,
                ID_Utilizator_client = currentUserId, 
                ID_Utilizator_proprietar = anunt.ID_Utilizator 
            };

            _context.Conversatii.Add(nouaConversatie);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { idConversatieActiva = nouaConversatie.ID_Conversatie });
        }

        [HttpPost]
        public async Task<IActionResult> TrimiteMesaj(int idConversatie, string textMesaj)
        {
            if (string.IsNullOrWhiteSpace(textMesaj))
                return RedirectToAction("Index", new { idConversatieActiva = idConversatie });

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId = int.Parse(userIdString);

            var mesaj = new Mesaje
            {
                ID_Conversatie = idConversatie,
                ID_Utilizator_expeditor = currentUserId,
                Text = textMesaj,
                Status = "Necitit",

                
                Data = DateTime.UtcNow.Date,
                Ora = DateTime.UtcNow
            };

            _context.Mesaje.Add(mesaj);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { idConversatieActiva = idConversatie });
        }
    }
}