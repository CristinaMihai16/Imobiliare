using System.ComponentModel.DataAnnotations;

namespace Imobiliare.Models
{
    public class Mesaje
    {
        [Key]
        public int ID_Mesaj { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }
        public DateTime Data{ get; set; }
        public DateTime Ora { get; set; }
        public int ID_Utilizator_expeditor { get; set; }
        public Utilizator? Utilizator_expeditor { get; set; }
        public int ID_Conversatie { get; set; }
        public Conversatie? Conversatie { get; set; }

    }
}
