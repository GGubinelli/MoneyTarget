using Microsoft.EntityFrameworkCore;
using MoneyTarget.Data.Entities;

namespace MoneyTarget.Data;

/// <summary>
/// DbContext per l'applicazione MoneyTarget
/// Configurato per SQL Server
/// </summary>
public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    // DbSets per le entità
    public DbSet<Utente> Utenti => Set<Utente>();
    public DbSet<Categoria> Categorie => Set<Categoria>();
    public DbSet<Transazione> Transazioni => Set<Transazione>();
    public DbSet<Budget> Budget => Set<Budget>();
    public DbSet<Asset> Asset => Set<Asset>();
    public DbSet<StoricoAsset> StoricoAsset => Set<StoricoAsset>();
    public DbSet<ImpostazioniUtente> ImpostazioniUtente => Set<ImpostazioniUtente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================================
        // Configurazione Utente
        // ============================================
        modelBuilder.Entity<Utente>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.DataRegistrazione)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // ============================================
        // Configurazione Categoria
        // ============================================
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasIndex(e => new { e.UtenteId, e.Nome }).IsUnique();
            
            entity.Property(e => e.DataCreazione)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(c => c.Utente)
                .WithMany(u => u.Categorie)
                .HasForeignKey(c => c.UtenteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // Configurazione Transazione
        // ============================================
        modelBuilder.Entity<Transazione>(entity =>
        {
            entity.HasIndex(e => e.UtenteId);
            entity.HasIndex(e => e.CategoriaId);
            entity.HasIndex(e => e.Data);
            entity.HasIndex(e => new { e.UtenteId, e.Data });

            entity.Property(e => e.Importo)
                .HasPrecision(18, 2);

            entity.Property(e => e.DataCreazione)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(t => t.Utente)
                .WithMany(u => u.Transazioni)
                .HasForeignKey(t => t.UtenteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.Categoria)
                .WithMany(c => c.Transazioni)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.NoAction); // Evita cascade multiplo
        });

        // ============================================
        // Configurazione Budget
        // ============================================
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasIndex(e => new { e.UtenteId, e.CategoriaId, e.Anno, e.Mese }).IsUnique();

            entity.Property(e => e.ImportoMensile)
                .HasPrecision(18, 2);

            entity.Property(e => e.DataCreazione)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(b => b.Utente)
                .WithMany(u => u.Budget)
                .HasForeignKey(b => b.UtenteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Categoria)
                .WithMany(c => c.Budget)
                .HasForeignKey(b => b.CategoriaId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ============================================
        // Configurazione Asset
        // ============================================
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasIndex(e => e.UtenteId);
            entity.HasIndex(e => e.Tipo);

            entity.Property(e => e.ValoreIniziale)
                .HasPrecision(18, 2);

            entity.Property(e => e.ValoreAttuale)
                .HasPrecision(18, 2);

            entity.Property(e => e.DataCreazione)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(a => a.Utente)
                .WithMany(u => u.Asset)
                .HasForeignKey(a => a.UtenteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // Configurazione StoricoAsset
        // ============================================
        modelBuilder.Entity<StoricoAsset>(entity =>
        {
            entity.HasIndex(e => e.AssetId);
            entity.HasIndex(e => e.Data);

            entity.Property(e => e.Valore)
                .HasPrecision(18, 2);

            entity.Property(e => e.DataCreazione)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(s => s.Asset)
                .WithMany(a => a.Storico)
                .HasForeignKey(s => s.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // Configurazione ImpostazioniUtente
        // ============================================
        modelBuilder.Entity<ImpostazioniUtente>(entity =>
        {
            entity.HasIndex(e => e.UtenteId).IsUnique();

            entity.HasOne(i => i.Utente)
                .WithOne(u => u.Impostazioni)
                .HasForeignKey<ImpostazioniUtente>(i => i.UtenteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Override SaveChanges per aggiornare automaticamente DataModifica
    /// </summary>
    public override int SaveChanges()
    {
        UpdateModificationTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync per aggiornare automaticamente DataModifica
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateModificationTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateModificationTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Aggiorna DataModifica per le entità che lo supportano
            var dataModificaProperty = entry.Properties
                .FirstOrDefault(p => p.Metadata.Name == "DataModifica");
            
            if (dataModificaProperty != null)
            {
                dataModificaProperty.CurrentValue = DateTime.UtcNow;
            }
        }
    }
}

/// <summary>
/// Estensioni per la configurazione del DbContext
/// </summary>
public static class FinanceDbContextExtensions
{
    /// <summary>
    /// Aggiunge il DbContext configurato per SQL Server
    /// </summary>
    public static IServiceCollection AddFinanceDbContext(
        this IServiceCollection services, 
        string connectionString)
    {
        services.AddDbContext<FinanceDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));

        return services;
    }
}
