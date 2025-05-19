using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("animalespecie")]
public class AnimalEspecie
{
    [Key]
    [Column("animalespecieid")]
    public int AnimalEspecieId { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; }
}