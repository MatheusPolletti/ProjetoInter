using Microsoft.EntityFrameworkCore;

namespace ProjetoInter.Models;

public class AtendimentoVeterinario
{
    public int AtendimentoVeterinarioId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Animal Animal { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Funcionario Funcionario { get; set; }
    public DateTime Data { get; set; }
    public string Descricao { get; set; }
    public string Resultado { get; set; }
    public string? Observacoes { get; set; }
}