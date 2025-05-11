using Microsoft.EntityFrameworkCore;

namespace ProjetoInter.Models;

public class Funcionario
{
    public int FuncionarioId { get; set; }
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Setor Setor { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Cargo { get; set; }
    public string? Telefone { get; set; }
    public bool Status { get; set; }
}