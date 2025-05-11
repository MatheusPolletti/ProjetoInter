using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoInter.Models;

[Table("usuario")]
public class Usuario
{
    [Key]
    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [Column("nome")]
    public string Nome { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("data_nascimento")]
    public DateTime? DataNascimento { get; set; }
}