using FinanceApp.Models;

namespace FinanceApp.Services;

/// <summary>
/// Interfaccia per il servizio di gestione finanze
/// </summary>
public interface IFinanceService
{
    Task<DashboardKPI> GetDashboardKPIAsync();
    Task<List<Transaction>> GetTransazioniRecentiAsync(int count = 10);
    Task<List<SpesaCategoria>> GetSpeseCategoriaAsync();
    Task<List<SaldoMensile>> GetAndamentoSaldoAsync();
    Task<List<string>> GetCategorieAsync();
    Task AddTransazioneAsync(Transaction transazione);
}
