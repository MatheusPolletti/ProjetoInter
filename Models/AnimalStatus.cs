using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("animalstatus")]
public class AnimalStatus
{
    [Key]
    [Column("animalstatusid")]
    public int AnimalStatusId { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; }
}