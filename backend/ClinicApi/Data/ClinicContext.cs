using ClinicApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Data;

public class ClinicContext : DbContext
{
    public ClinicContext(DbContextOptions<ClinicContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Consulta> Consultas => Set<Consulta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.Property(e => e.Correo).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.NombreCompleto).IsRequired();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(e => e.Medico)
                  .WithMany(m => m.Usuarios)
                  .HasForeignKey(e => e.MedicoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.ToTable("Medicos");
            entity.Property(e => e.PrimerNombre).IsRequired();
            entity.Property(e => e.ApellidoPaterno).IsRequired();
            entity.Property(e => e.ApellidoMaterno).IsRequired();
            entity.Property(e => e.Cedula).IsRequired();
            entity.Property(e => e.Telefono).IsRequired();
            entity.Property(e => e.Especialidad).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.ToTable("Pacientes");
            entity.Property(e => e.PrimerNombre).IsRequired();
            entity.Property(e => e.ApellidoPaterno).IsRequired();
            entity.Property(e => e.ApellidoMaterno).IsRequired();
            entity.Property(e => e.Telefono).IsRequired();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.ToTable("Consultas");
            entity.Property(e => e.Sintomas).IsRequired();
            entity.Property(e => e.Recomendaciones).IsRequired();
            entity.Property(e => e.Diagnostico).IsRequired();
            entity.HasOne(e => e.Medico)
                  .WithMany(m => m.Consultas)
                  .HasForeignKey(e => e.MedicoId);
            entity.HasOne(e => e.Paciente)
                  .WithMany(p => p.Consultas)
                  .HasForeignKey(e => e.PacienteId);
        });
    }
}
