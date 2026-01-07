using Imobiliare.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ImaginiAnunt
{
    [Key]
    public int ID_ImaginiAnunt { get; set; }

    public byte[]? Imagine { get; set; }

    
    public int ID_Anunt { get; set; }
    public Anunturi? Anunt { get; set; }
}
