namespace ProjetoInter.Models;

public class Instituicao
{
    public int InstituicaoId { get; set; }
    public string Nome { get; set; }
    public string Endereco { get; set; }
    public string? Contato { get; set; }
    public bool Status { get; set; }
}