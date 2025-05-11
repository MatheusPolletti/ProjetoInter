using ProjetoInter.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjetoInter.Data;

public class DbZoologico : DbContext
{
    public DbZoologico(DbContextOptions options) : base(options) {}
    
    public DbSet<Usuario> Usuarios { get; set; }
    // public DbSet<Setor> Setores { get; set; }
    // public DbSet<Animal> Animais { get; set; }
    // public DbSet<Instituicao> Instituicoes { get; set; }
    // public DbSet<Transferencia> Transferencias { get; set; }
    // public DbSet<Funcionario> Funcionarios { get; set; }
    // public DbSet<Procedimento> Procedimentos { get; set; }
    // public DbSet<AtendimentoVeterinario> AtendimentosVeterinarios { get; set; }
    // public DbSet<HistoricoSaude> HistoricosSaude { get; set; }
}