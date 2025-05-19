using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("animal")]
public class Animal
{
    [Key]
    [Column("animalid")]
    public int AnimalId { get; set; }

    [Column("setor")]
    public int SetorId { get; set; }

    [Column("status")]
    public int StatusId { get; set; }

    [Column("especie")]
    public int EspecieId { get; set; }

    [Column("nome")]
    public string Nome { get; set; }

    [Column("imagemurl")]
    public string? ImagemUrl { get; set; }

    [Column("sexo")]
    public string Sexo { get; set; }

    [Column("datanascimento")]
    public DateOnly? DataNascimento { get; set; }

    [Column("datafalecimento")]
    public DateOnly? DataFalecimento { get; set; }

    [Column("peso")]
    public decimal Peso { get; set; }

    // Declarando os Objetos para ter propriedade de navegação
    public AnimalStatus Status { get; set; }
    public AnimalEspecie Especie { get; set; }
    public Setor Setor { get; set; }
}