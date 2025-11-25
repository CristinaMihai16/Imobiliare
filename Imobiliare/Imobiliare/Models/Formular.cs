using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imobiliare.Models
{
    public class Formular
    {
        [Key]
        public int ID_Formular { get; set; }
        public string Nume { get; set; }
        public string Email { get; set; }
        public string Subiect { get; set; }
        public DateTime Data_trimitere { get; set; }


        public int IdUtilizator { get; set; }

        [ForeignKey("IdUtilizator")]
        public Utilizator? Utilizator { get; set; }

    }
}
