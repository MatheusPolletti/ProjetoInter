using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("atendimentoveterinario")]
public class AtendimentoVeterinario
{
    [Key]
    [Column("atendimentoveterinarioid")]
    public int AtendimentoVeterinarioId { get; set; }

    [Column("animal")]
    public int AnimalId { get; set; }  // FK correta: nome consistente

    [Column("funcionariosolicitante")]
    public int FuncionarioSolicitanteId { get; set; }  // FK correta

    [Column("funcionarioveterinario")]
    public int FuncionarioVeterinarioId { get; set; }  // FK correta

    [Column("descricao")]
    public string Descricao { get; set; }

    [Column("resultado")]
    public string? Resultado { get; set; }

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    [Column("data")]
    public DateOnly? Data { get; set; }

    // Navegações

    [ForeignKey("AnimalId")]
    public Animal Animal { get; set; }

    [ForeignKey("FuncionarioSolicitanteId")]
    public Funcionario FuncionarioSolicitante { get; set; }

    [ForeignKey("FuncionarioVeterinarioId")]
    public Funcionario FuncionarioVeterinario { get; set; }
}