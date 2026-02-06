using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using FinanceApp.Data.Entities;
using FinanceApp.Data.DTOs;

namespace FinanceApp.Data.Repositories;

/// <summary>
/// Repository per l'accesso ai dati finanziari con SQL Server
/// </summary>
public interface IFinanceRepository
{
    // Transazioni
    Task<List<Transazione>> GetTransazioniAsync(int utenteId, int? anno = null, int? mese = null, int? limit = null);
    Task<Transazione?> GetTransazioneByIdAsync(int id);
    Task<Transazione> AddTransazioneAsync(Transazione transazione);
    Task<Transazione> UpdateTransazioneAsync(Transazione transazione);
    Task DeleteTransazioneAsync(int id);

    // Categorie
    Task<List<Categoria>> GetCategorieAsync(int utenteId, TipoTransazione? tipo = null);
    Task<Categoria?> GetCategoriaByIdAsync(int id);
    Task<Categoria> AddCategoriaAsync(Categoria categoria);
    Task<Categoria> UpdateCategoriaAsync(Categoria categoria);
    Task DeleteCategoriaAsync(int id);

    // Budget
    Task<List<Budget>> GetBudgetAsync(int utenteId, int? anno = null, int? mese = null);
    Task<Budget?> GetBudgetByIdAsync(int id);
    Task<Budget> AddBudgetAsync(Budget budget);
    Task<Budget> UpdateBudgetAsync(Budget budget);
    Task DeleteBudgetAsync(int id);

    // Asset
    Task<List<Asset>> GetAssetAsync(int utenteId);
    Task<Asset?> GetAssetByIdAsync(int id);
    Task<Asset> AddAssetAsync(Asset asset);
    Task<Asset> UpdateAssetAsync(Asset asset);
    Task DeleteAssetAsync(int id);

    // Stored Procedures
    Task<List<BudgetUtilizationResult>> GetBudgetUtilizationAsync(int utenteId, int? anno = null, int? mese = null);
    Task<DashboardSummaryResult?> GetDashboardSummaryAsync(int utenteId);
    Task<List<AndamentoSaldoResult>> GetAndamentoSaldoAsync(int utenteId, int numeroMesi = 12);
    Task<List<SpesaCategoriaResult>> GetSpeseCategoriaAsync(int utenteId, int? anno = null, int? mese = null);
}

/// <summary>
/// Implementazione del repository con Entity Framework Core e SQL Server
/// </summary>
public class FinanceRepository : IFinanceRepository
{
    private readonly FinanceDbContext _context;

    public FinanceRepository(FinanceDbContext context)
    {
        _context = context;
    }

    #region Transazioni

    public async Task<List<Transazione>> GetTransazioniAsync(int utenteId, int? anno = null, int? mese = null, int? limit = null)
    {
        var query = _context.Transazioni
            .Include(t => t.Categoria)
            .Where(t => t.UtenteId == utenteId);

        if (anno.HasValue)
            query = query.Where(t => t.Data.Year == anno.Value);

        if (mese.HasValue)
            query = query.Where(t => t.Data.Month == mese.Value);

        query = query.OrderByDescending(t => t.Data).ThenByDescending(t => t.Id);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.ToListAsync();
    }

    public async Task<Transazione?> GetTransazioneByIdAsync(int id)
    {
        return await _context.Transazioni
            .Include(t => t.Categoria)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transazione> AddTransazioneAsync(Transazione transazione)
    {
        _context.Transazioni.Add(transazione);
        await _context.SaveChangesAsync();
        return transazione;
    }

    public async Task<Transazione> UpdateTransazioneAsync(Transazione transazione)
    {
        _context.Transazioni.Update(transazione);
        await _context.SaveChangesAsync();
        return transazione;
    }

    public async Task DeleteTransazioneAsync(int id)
    {
        var transazione = await _context.Transazioni.FindAsync(id);
        if (transazione != null)
        {
            _context.Transazioni.Remove(transazione);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Categorie

    public async Task<List<Categoria>> GetCategorieAsync(int utenteId, TipoTransazione? tipo = null)
    {
        var query = _context.Categorie
            .Where(c => c.UtenteId == utenteId && c.IsAttiva);

        if (tipo.HasValue)
            query = query.Where(c => c.Tipo == tipo.Value);

        return await query.OrderBy(c => c.Ordine).ThenBy(c => c.Nome).ToListAsync();
    }

    public async Task<Categoria?> GetCategoriaByIdAsync(int id)
    {
        return await _context.Categorie.FindAsync(id);
    }

    public async Task<Categoria> AddCategoriaAsync(Categoria categoria)
    {
        _context.Categorie.Add(categoria);
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task<Categoria> UpdateCategoriaAsync(Categoria categoria)
    {
        _context.Categorie.Update(categoria);
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task DeleteCategoriaAsync(int id)
    {
        var categoria = await _context.Categorie.FindAsync(id);
        if (categoria != null)
        {
            // Soft delete - disattiva invece di eliminare
            categoria.IsAttiva = false;
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Budget

    public async Task<List<Budget>> GetBudgetAsync(int utenteId, int? anno = null, int? mese = null)
    {
        var query = _context.Budget
            .Include(b => b.Categoria)
            .Where(b => b.UtenteId == utenteId && b.IsAttivo);

        if (anno.HasValue)
            query = query.Where(b => b.Anno == anno.Value);

        if (mese.HasValue)
            query = query.Where(b => b.Mese == mese.Value || b.Mese == 0);

        return await query.OrderBy(b => b.Categoria!.Nome).ToListAsync();
    }

    public async Task<Budget?> GetBudgetByIdAsync(int id)
    {
        return await _context.Budget
            .Include(b => b.Categoria)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Budget> AddBudgetAsync(Budget budget)
    {
        _context.Budget.Add(budget);
        await _context.SaveChangesAsync();
        return budget;
    }

    public async Task<Budget> UpdateBudgetAsync(Budget budget)
    {
        _context.Budget.Update(budget);
        await _context.SaveChangesAsync();
        return budget;
    }

    public async Task DeleteBudgetAsync(int id)
    {
        var budget = await _context.Budget.FindAsync(id);
        if (budget != null)
        {
            budget.IsAttivo = false;
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Asset

    public async Task<List<Asset>> GetAssetAsync(int utenteId)
    {
        return await _context.Asset
            .Where(a => a.UtenteId == utenteId && a.IsAttivo)
            .OrderBy(a => a.Tipo)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Asset?> GetAssetByIdAsync(int id)
    {
        return await _context.Asset
            .Include(a => a.Storico.OrderByDescending(s => s.Data).Take(12))
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Asset> AddAssetAsync(Asset asset)
    {
        _context.Asset.Add(asset);
        await _context.SaveChangesAsync();
        
        // Aggiungi primo record storico
        _context.StoricoAsset.Add(new StoricoAsset
        {
            AssetId = asset.Id,
            Valore = asset.ValoreIniziale,
            Data = asset.DataAcquisto ?? DateTime.Today,
            Note = "Valore iniziale"
        });
        await _context.SaveChangesAsync();
        
        return asset;
    }

    public async Task<Asset> UpdateAssetAsync(Asset asset)
    {
        var existingAsset = await _context.Asset.FindAsync(asset.Id);
        if (existingAsset != null && existingAsset.ValoreAttuale != asset.ValoreAttuale)
        {
            // Registra variazione nello storico
            _context.StoricoAsset.Add(new StoricoAsset
            {
                AssetId = asset.Id,
                Valore = asset.ValoreAttuale,
                Data = DateTime.Today,
                Note = "Aggiornamento valore"
            });
        }

        _context.Asset.Update(asset);
        await _context.SaveChangesAsync();
        return asset;
    }

    public async Task DeleteAssetAsync(int id)
    {
        var asset = await _context.Asset.FindAsync(id);
        if (asset != null)
        {
            asset.IsAttivo = false;
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Stored Procedures

    public async Task<List<BudgetUtilizationResult>> GetBudgetUtilizationAsync(int utenteId, int? anno = null, int? mese = null)
    {
        var parameters = new[]
        {
            new SqlParameter("@UtenteId", utenteId),
            new SqlParameter("@Anno", (object?)anno ?? DBNull.Value),
            new SqlParameter("@Mese", (object?)mese ?? DBNull.Value)
        };

        return await _context.Database
            .SqlQueryRaw<BudgetUtilizationResult>(
                "EXEC sp_GetBudgetUtilization @UtenteId, @Anno, @Mese", 
                parameters)
            .ToListAsync();
    }

    public async Task<DashboardSummaryResult?> GetDashboardSummaryAsync(int utenteId)
    {
        var parameter = new SqlParameter("@UtenteId", utenteId);

        var results = await _context.Database
            .SqlQueryRaw<DashboardSummaryResult>(
                "EXEC sp_GetDashboardSummary @UtenteId", 
                parameter)
            .ToListAsync();

        return results.FirstOrDefault();
    }

    public async Task<List<AndamentoSaldoResult>> GetAndamentoSaldoAsync(int utenteId, int numeroMesi = 12)
    {
        var parameters = new[]
        {
            new SqlParameter("@UtenteId", utenteId),
            new SqlParameter("@NumeroMesi", numeroMesi)
        };

        return await _context.Database
            .SqlQueryRaw<AndamentoSaldoResult>(
                "EXEC sp_GetAndamentoSaldo @UtenteId, @NumeroMesi", 
                parameters)
            .ToListAsync();
    }

    public async Task<List<SpesaCategoriaResult>> GetSpeseCategoriaAsync(int utenteId, int? anno = null, int? mese = null)
    {
        var parameters = new[]
        {
            new SqlParameter("@UtenteId", utenteId),
            new SqlParameter("@Anno", (object?)anno ?? DBNull.Value),
            new SqlParameter("@Mese", (object?)mese ?? DBNull.Value)
        };

        return await _context.Database
            .SqlQueryRaw<SpesaCategoriaResult>(
                "EXEC sp_GetSpesePorCategoria @UtenteId, @Anno, @Mese", 
                parameters)
            .ToListAsync();
    }

    #endregion
}
