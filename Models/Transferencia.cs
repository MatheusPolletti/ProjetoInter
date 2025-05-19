using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("transferencia")]
public class Transferencia
{
    [Key]
    [Column("transferenciaid")]
    public int TransferenciaId { get; set; }

    [Column("animal")]
    public int AnimalId { get; set; }

    [Column("instituicaoorigem")]
    public int InstituicaoOrigemId { get; set; }

    [Column("instituicaodestino")]
    public int InstituicaoDestinoId { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    [Column("datasaida")]
    public DateTime? DataSaida { get; set; }

    [Column("dataentrada")]
    public DateTime? DataEntrada { get; set; }

    // Navegação
    [ForeignKey("AnimalId")]
    public Animal Animal { get; set; }

    [ForeignKey("InstituicaoOrigemId")]
    public Instituicao InstituicaoOrigem { get; set; }

    [ForeignKey("InstituicaoDestinoId")]
    public Instituicao InstituicaoDestino { get; set; }
}