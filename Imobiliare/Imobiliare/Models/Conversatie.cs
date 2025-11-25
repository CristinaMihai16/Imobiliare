using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imobiliare.Models
{
    public class Conversatie
    {
        [Key]
        public int ID_Conversatie { get; set; }
        public int ID_Anunt{ get; set; }
        public Anunturi? Anunturi { get; set; }
      
        public int ID_Utilizator_proprietar { get; set; }
        [ForeignKey("ID_Utilizator_proprietar")]
        public Utilizator? Utilizator_proprietar { get; set; }
        public int ID_Utilizator_client { get; set; }

        [ForeignKey("ID_Utilizator_client")]
        public Utilizator? Utilizator_client { get; set; }


    }
}
