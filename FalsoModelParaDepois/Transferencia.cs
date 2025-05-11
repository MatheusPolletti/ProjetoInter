using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProjetoInter.Models;

public class Transferencia
{
    [Key]
    public int TransferenciaId { get; set; }
    public Animal Animal { get; set; }
    
    // Serve para não dar defeito do Entity framework estar ciclico e não permitir duas Foreign Key
    [DeleteBehavior(DeleteBehavior.NoAction)]
    // Serve para não dar defeito do Entity framework estar ciclico e não permitir duas Foreign Key
    public Instituicao InstituicaoOrigem { get; set; }
    
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Instituicao InstituicaoDestino { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
}