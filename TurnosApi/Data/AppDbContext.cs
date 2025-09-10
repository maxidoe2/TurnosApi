using Microsoft.EntityFrameworkCore;
using TurnosApi.Models;

namespace TurnosApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Medico>   Medicos   => Set<Medico>();
    public DbSet<Turno>    Turnos    => Set<Turno>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Unicidad
    modelBuilder.Entity<Paciente>()
        .HasIndex(p => p.DNI)
        .IsUnique();

    modelBuilder.Entity<Medico>()
        .HasIndex(m => m.Matricula)
        .IsUnique();

    // Relaciones (FKs)
    modelBuilder.Entity<Turno>()
        .HasOne(t => t.Paciente)
        .WithMany()
        .HasForeignKey(t => t.PacienteId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Turno>()
        .HasOne(t => t.Medico)
        .WithMany()
        .HasForeignKey(t => t.MedicoId)
        .OnDelete(DeleteBehavior.Restrict);

    // Seeds (valores ESTÁTICOS)
    modelBuilder.Entity<Paciente>().HasData(
        new Paciente { Id = 1, DNI = "30111222", Nombre = "Juan Perez", Email = "juan@demo.com" }
    );
    modelBuilder.Entity<Medico>().HasData(
        new Medico { Id = 1, Matricula = "M-123", Nombre = "Dra. Gómez", Especialidad = "Clínica" }
    );
    modelBuilder.Entity<Turno>().HasData(
        new Turno {
            Id = 1,
            PacienteId = 1,
            MedicoId   = 1,
            FechaHora  = new DateTime(2025, 10, 15, 15, 0, 0, DateTimeKind.Utc),
            Estado     = "pendiente"
        }
    );
}
}
