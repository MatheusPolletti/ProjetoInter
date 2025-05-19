using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("funcionario")]
public class Funcionario
{
    [Key]
    [Column("funcionarioid")]
    public int FuncionarioId { get; set; }

    [Column("status")]
    public int StatusFuncionarioId { get; set; }  // FK para StatusFuncionario

    [Column("nome")]
    public string Nome { get; set; }

    [Column("cpf")]
    public string Cpf { get; set; }

    [Column("cargo")]
    public string Cargo { get; set; }

    [Column("telefone")]
    public string? Telefone { get; set; }

    // Navegação
    [ForeignKey("StatusFuncionarioId")]
    public StatusFuncionario StatusFuncionario { get; set; }
}