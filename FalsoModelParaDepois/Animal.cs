namespace ProjetoInter.Models;

public class Animal
{
    public int AnimalId { get; set; }
    // O Setor Setor define que é uma chave estrangeira para o id do Setor
    public Setor Setor { get; set; }
    public string Nome { get; set; }
    public string Especie { get; set; }
    // O ? defini que pode ser nulo a variável
    public DateOnly? DataNascimento { get; set; }
    public DateOnly? DataFalecimento { get; set; }
    public bool Status { get; set; }
    public string? ImagemUrl { get; set; }
}