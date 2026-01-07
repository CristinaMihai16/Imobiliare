using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Imobiliare.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace Imobiliare.ViewComponents
{
    public class MesajeNecititeViewComponent : ViewComponent
    {
        private readonly ImobiliareContext _context;

        public MesajeNecititeViewComponent(ImobiliareContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View(0);
            }

            var userIdString = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return View(0);

            int currentUserId = int.Parse(userIdString);

            var unreadCount = await _context.Mesaje
                .Include(m => m.Conversatie)
                .Where(m => m.Status == "Necitit"
                            && m.ID_Utilizator_expeditor != currentUserId
                            && (m.Conversatie.ID_Utilizator_client == currentUserId || m.Conversatie.ID_Utilizator_proprietar == currentUserId))
                .CountAsync();

            return View(unreadCount);
        }
    }
}