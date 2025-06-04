using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("instituicao")]
public class Instituicao
{
    [Key]
    [Column("instituicaoid")]
    public int InstituicaoId { get; set; }

    [Column("nome")]
    public string Nome { get; set; }

    [Column("endereco")]
    public string Endereco { get; set; }

    [Column("contato")]
    public string? Contato { get; set; }

    [Column("imagemurl")]
    public string? ImagemUrl { get; set; }
}