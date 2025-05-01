namespace ProjetoInter.Models;

public class Procedimento
{
    public int ProcedimentoId { get; set; }
    public Animal Animal { get; set; }
    public Funcionario Funcionario { get; set; }
    public string Descricao { get; set; }
    public string Observacoes { get; set; }
    public DateTime DataProcedimento { get; set; }
}