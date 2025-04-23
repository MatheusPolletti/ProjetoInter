namespace ProjetoInter.Models;

public class HistoricoSaude
{
    public int HistoricoSaudeId { get; set; }
    public Animal Animal { get; set; }
    public string TipoEvento { get; set; }
    public DateTime Data { get; set; }
    public string EstadoSaude { get; set; }
    public string? Observacoes { get; set; }
}