using ProjetoInter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProjetoInter.Data;

public class DbZoologico : DbContext
{
    public DbZoologico(DbContextOptions options) : base(options) { }

    public DbSet<Animal> Animais { get; set; }
    public DbSet<AnimalEspecie> AnimalEspecies { get; set; }
    public DbSet<AnimalStatus> AnimalStatuses { get; set; }
    public DbSet<AtendimentoVeterinario> AtendimentosVeterinarios { get; set; }
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<Instituicao> Instituicoes { get; set; }
    public DbSet<Procedimento> Procedimentos { get; set; }
    public DbSet<Setor> Setores { get; set; }
    public DbSet<StatusFuncionario> StatusFuncionarios { get; set; }
    public DbSet<Transferencia> Transferencias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly?, DateTime?>(
            dateOnly => dateOnly.HasValue ? dateOnly.Value.ToDateTime(TimeOnly.MinValue) : null,
            dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null);
            
        // Data animais    
        modelBuilder.Entity<Animal>().Property(a => a.DataNascimento).HasConversion(dateOnlyConverter);
        modelBuilder.Entity<Animal>().Property(a => a.DataFalecimento).HasConversion(dateOnlyConverter);

        // Data Atendimentos
        modelBuilder.Entity<AtendimentoVeterinario>().Property(a => a.Data).HasConversion(dateOnlyConverter);
    }
}