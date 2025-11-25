using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Imobiliare.Models
{
    public class Favorite
    {
        [Key]
        public int ID_Favorite { get; set; }
        public int? ID_Utilizator { get; set; }
        public int? ID_Anunt { get; set; }
        public DateTime Data_adaugare { get; set; }

        public Utilizator? Utilizator { get; set; }
        [ForeignKey("ID_Anunt")]
        public Anunturi? Anunturi { get; set; }

    }
}
