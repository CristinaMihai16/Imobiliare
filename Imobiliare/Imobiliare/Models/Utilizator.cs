using Microsoft.AspNetCore.Identity;

namespace Imobiliare.Models
{
    public class Utilizator : IdentityUser<int>
    {
        public string Tip_utilizator { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
        public byte[]? Imagine_profil { get; set; }
        public DateTime Data_creare { get; set; }

        public ICollection<Formular> Formular { get; set; }
        public ICollection<Mesaje> Mesaje { get; set; }
        public ICollection<Favorite> Favorite { get; set; }
        public ICollection<Conversatie> Conversatie { get; set; }
        public ICollection<Anunturi> Anunturi { get; set; }




    }
}
