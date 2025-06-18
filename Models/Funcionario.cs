using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("funcionario")]
public class Funcionario
{
    [Key]
    [Column("funcionarioid")]
    public int FuncionarioId { get; set; }

    [Column("auth_user_id")]
    public Guid? AuthUserId { get; set; }

    [Column("status")]
    public int StatusFuncionarioId { get; set; }

    [Column("nome")]
    public string Nome { get; set; }

    [Column("cpf")]
    public string Cpf { get; set; }

    [Column("cargo")]
    public string Cargo { get; set; }

    [Column("telefone")]
    public string? Telefone { get; set; }

    [Column("imagemurl")]
    public string? ImagemUrl { get; set; }

    [Column("isadmin")]
    public bool IsAdmin { get; set; }
    
    [Column("instituicaoid")]
    public int InstituicaoId { get; set; }

    // Relacionamentos
    [ForeignKey("StatusFuncionarioId")]
    public StatusFuncionario StatusFuncionario { get; set; }

    [ForeignKey("InstituicaoId")]
    public Instituicao Instituicao { get; set; }
}