using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("procedimento")]
public class Procedimento
{
    [Key]
    [Column("procedimentoid")]
    public int ProcedimentoId { get; set; }

    [Column("animal")]
    public int AnimalId { get; set; }

    [Column("funcionariotarefa")]
    public int FuncionarioTarefaId { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; }

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    [Column("dataprocedimento")]
    public DateOnly DataProcedimento { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    // Navegação
    [ForeignKey("AnimalId")]
    public Animal Animal { get; set; }

    [ForeignKey("FuncionarioTarefaId")]
    public Funcionario FuncionarioTarefa { get; set; }
}