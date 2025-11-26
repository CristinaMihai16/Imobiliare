using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Imobiliare.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Imobiliare.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Utilizator> _userManager;
        private readonly SignInManager<Utilizator> _signInManager;

        public IndexModel(
            UserManager<Utilizator> userManager,
            SignInManager<Utilizator> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }
        public string UserImage { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Nume")]
            [StringLength(100)]
            public string Nume { get; set; }

            [Display(Name = "Prenume")]
            [StringLength(100)]
            public string Prenume { get; set; }

            [Display(Name = "Adresă")]
            public string Adresa { get; set; }


            public string Telefon { get; set; }

            [Display(Name = "Schimbă poza")]
            public IFormFile? ImagineUpload { get; set; }


        }

        private async Task LoadAsync(Utilizator user)
        {
            Username = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            if (user.Imagine_profil != null && user.Imagine_profil.Length > 0)
            {
                UserImage = $"data:image/png;base64,{Convert.ToBase64String(user.Imagine_profil)}";
            }
            else
            {
                UserImage = "https://cdn.icon-icons.com/icons2/2506/PNG/512/user_icon_150670.png";
            }

            Input = new InputModel
            {
                Telefon =  user.Telefon, 
                Nume = user.Nume,
                Prenume = user.Prenume,
                Adresa = user.Adresa,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nu s-a putut încărca utilizatorul cu ID-ul '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nu s-a putut încărca utilizatorul cu ID-ul '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.Nume = Input.Nume;
            user.Prenume = Input.Prenume;
            user.Telefon = Input.Telefon;
            user.Adresa = Input.Adresa;

            if (Input.ImagineUpload != null)
            {
                using (var ms = new MemoryStream())
                {
                    await Input.ImagineUpload.CopyToAsync(ms);
                    user.Imagine_profil = ms.ToArray();
                }
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profilul a fost actualizat cu succes!";
            return RedirectToPage();

        }
    }
}