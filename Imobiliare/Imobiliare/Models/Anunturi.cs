using System.ComponentModel.DataAnnotations;

namespace Imobiliare.Models
{
    public class Anunturi
    {
        [Key]
        public int ID_Anunt { get; set; }
        public string Denumire { get; set; }
        public string? Descriere { get; set; }
        public decimal Pret { get; set; }
        public byte[]? Imagine_anunt { get; set; }
        public string? Locatie { get; set; }
        [Required]
        public string Oras { get; set; }
        [Required]
        public string Tranzactie { get; set; }
        [Required]
        public string TipProprietate { get; set; } 
        public DateTime Data_publicare { get; set; }
        public int ID_Utilizator { get; set; }
        public Utilizator? Utilizator { get; set; }
        public ICollection<Conversatie>? Conversatie { get; set; }
        public ICollection<ImaginiAnunt>? Imagini { get; set; }

    }
}
