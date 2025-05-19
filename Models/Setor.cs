using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("setor")]
public class Setor
{
    [Key]
    [Column("setorid")]
    public int SetorId { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    [Column("instituicaopertence")]
    public int InstituicaoPertenceId { get; set; }

    [Column("nome")]
    public string Nome { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; }

    // Navegação
    [ForeignKey("InstituicaoPertenceId")]
    public Instituicao InstituicaoPertence { get; set; }
}