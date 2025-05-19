using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("statusfuncionario")]
public class StatusFuncionario
{
    [Key]
    [Column("statusfuncionarioid")]
    public int StatusFuncionarioId { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; }
}