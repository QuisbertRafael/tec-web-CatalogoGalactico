using CatalogoGalactico.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Data;

public class CatalogoDbContext : DbContext
{
    public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options) : base(options)
    {
    }

    public DbSet<Personaje> Personajes => Set<Personaje>();
    public DbSet<CardPersonaje> Cartas => Set<CardPersonaje>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<EventoParticipante> EventoParticipantes => Set<EventoParticipante>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Personaje>(e =>
        {
            e.Property(p => p.Nombre).HasMaxLength(80).IsRequired();
            e.Property(p => p.Especie).HasMaxLength(50).IsRequired();
            e.Property(p => p.Afiliacion).HasMaxLength(80).IsRequired();
            e.Property(p => p.Image).HasMaxLength(500).IsRequired();
            // Los enums se guardan como texto ("Rebelde", "Vivo") para que la tabla sea legible.
            e.Property(p => p.Faccion).HasConversion<string>().HasMaxLength(20);
            e.Property(p => p.Estado).HasConversion<string>().HasMaxLength(20);
        });

        mb.Entity<CardPersonaje>(e =>
        {
            e.Property(c => c.HabilidadEspecial).HasMaxLength(100).IsRequired();
            e.Property(c => c.Arma).HasMaxLength(100).IsRequired();
            e.Property(c => c.ImagenUrl).HasMaxLength(500).IsRequired();

            // Relación 1:1: EF crea un índice único sobre PersonajeId, así que la base
            // también impide dos cartas para el mismo personaje.
            e.HasOne(c => c.Personaje)
                .WithOne(p => p.Card)
                .HasForeignKey<CardPersonaje>(c => c.PersonajeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Evento>(e =>
        {
            e.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            e.Property(x => x.Fecha).HasMaxLength(30).IsRequired();
            e.Property(x => x.Ubicacion).HasMaxLength(120).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
            e.Property(x => x.Ganador).HasMaxLength(20);
            e.HasIndex(x => x.Anio);
        });

        mb.Entity<EventoParticipante>(e =>
        {
            e.HasKey(x => new { x.EventoId, x.PersonajeId });

            // Evento 1:N EventoParticipante (si se borra el evento, se borran sus participaciones).
            e.HasOne(x => x.Evento)
                .WithMany(ev => ev.Participantes)
                .HasForeignKey(x => x.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Personaje 1:N EventoParticipante (no se puede borrar un personaje con participaciones).
            e.HasOne(x => x.Personaje)
                .WithMany(p => p.Participaciones)
                .HasForeignKey(x => x.PersonajeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Datos iniciales (se insertan con la migración).
        mb.Entity<Personaje>().HasData(SemillaDatos.Personajes);
        mb.Entity<CardPersonaje>().HasData(SemillaDatos.Cartas);
        mb.Entity<Evento>().HasData(SemillaDatos.Eventos);
        mb.Entity<EventoParticipante>().HasData(SemillaDatos.Participaciones);
    }
}
