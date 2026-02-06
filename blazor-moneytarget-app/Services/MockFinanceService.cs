using MoneyTarget.Models;

namespace MoneyTarget.Services;

/// <summary>
/// Servizio Mock per la gestione delle finanze personali
/// Simula un database SQL Server con dati in memoria
/// </summary>
public class MockFinanceService : IFinanceService
{
    private readonly List<Transaction> _transazioni;
    private readonly List<string> _categorie;

    public MockFinanceService()
    {
        _categorie = new List<string>
        {
            "Stipendio",
            "Affitto",
            "Bollette",
            "Spesa Alimentare",
            "Trasporti",
            "Intrattenimento",
            "Salute",
            "Abbigliamento",
            "Ristoranti",
            "Investimenti",
            "Freelance",
            "Altro"
        };

        _transazioni = GeneraTransazioniMock();
    }

    private List<Transaction> GeneraTransazioniMock()
    {
        var transazioni = new List<Transaction>
        {
            // Gennaio 2024
            new() { Id = 1, Data = new DateTime(2024, 1, 5), Descrizione = "Stipendio Gennaio", Categoria = "Stipendio", Importo = 2800.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 2, Data = new DateTime(2024, 1, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 3, Data = new DateTime(2024, 1, 10), Descrizione = "Bolletta Luce", Categoria = "Bollette", Importo = 95.50m, Tipo = TipoTransazione.Uscita },
            new() { Id = 4, Data = new DateTime(2024, 1, 12), Descrizione = "Supermercato Esselunga", Categoria = "Spesa Alimentare", Importo = 187.30m, Tipo = TipoTransazione.Uscita },
            new() { Id = 5, Data = new DateTime(2024, 1, 15), Descrizione = "Abbonamento Metro", Categoria = "Trasporti", Importo = 39.00m, Tipo = TipoTransazione.Uscita },
            
            // Febbraio 2024
            new() { Id = 6, Data = new DateTime(2024, 2, 5), Descrizione = "Stipendio Febbraio", Categoria = "Stipendio", Importo = 2800.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 7, Data = new DateTime(2024, 2, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 8, Data = new DateTime(2024, 2, 14), Descrizione = "Cena San Valentino", Categoria = "Ristoranti", Importo = 125.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 9, Data = new DateTime(2024, 2, 18), Descrizione = "Bolletta Gas", Categoria = "Bollette", Importo = 78.20m, Tipo = TipoTransazione.Uscita },
            new() { Id = 10, Data = new DateTime(2024, 2, 20), Descrizione = "Progetto Freelance Web", Categoria = "Freelance", Importo = 650.00m, Tipo = TipoTransazione.Entrata },
            
            // Marzo 2024
            new() { Id = 11, Data = new DateTime(2024, 3, 5), Descrizione = "Stipendio Marzo", Categoria = "Stipendio", Importo = 2800.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 12, Data = new DateTime(2024, 3, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 13, Data = new DateTime(2024, 3, 10), Descrizione = "Visita Medica", Categoria = "Salute", Importo = 120.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 14, Data = new DateTime(2024, 3, 15), Descrizione = "Spesa Settimanale", Categoria = "Spesa Alimentare", Importo = 165.80m, Tipo = TipoTransazione.Uscita },
            new() { Id = 15, Data = new DateTime(2024, 3, 22), Descrizione = "Cinema e Pizza", Categoria = "Intrattenimento", Importo = 45.00m, Tipo = TipoTransazione.Uscita },
            
            // Aprile 2024
            new() { Id = 16, Data = new DateTime(2024, 4, 5), Descrizione = "Stipendio Aprile", Categoria = "Stipendio", Importo = 2850.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 17, Data = new DateTime(2024, 4, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 18, Data = new DateTime(2024, 4, 12), Descrizione = "Acquisto Vestiti", Categoria = "Abbigliamento", Importo = 215.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 19, Data = new DateTime(2024, 4, 18), Descrizione = "Bollette Varie", Categoria = "Bollette", Importo = 142.30m, Tipo = TipoTransazione.Uscita },
            new() { Id = 20, Data = new DateTime(2024, 4, 25), Descrizione = "Bonus Performance", Categoria = "Stipendio", Importo = 500.00m, Tipo = TipoTransazione.Entrata },
            
            // Maggio 2024
            new() { Id = 21, Data = new DateTime(2024, 5, 5), Descrizione = "Stipendio Maggio", Categoria = "Stipendio", Importo = 2850.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 22, Data = new DateTime(2024, 5, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 23, Data = new DateTime(2024, 5, 10), Descrizione = "Investimento ETF", Categoria = "Investimenti", Importo = 300.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 24, Data = new DateTime(2024, 5, 15), Descrizione = "Tagliando Auto", Categoria = "Trasporti", Importo = 280.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 25, Data = new DateTime(2024, 5, 20), Descrizione = "Spesa Alimentare", Categoria = "Spesa Alimentare", Importo = 198.50m, Tipo = TipoTransazione.Uscita },
            
            // Giugno 2024
            new() { Id = 26, Data = new DateTime(2024, 6, 5), Descrizione = "Stipendio Giugno", Categoria = "Stipendio", Importo = 2850.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 27, Data = new DateTime(2024, 6, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 28, Data = new DateTime(2024, 6, 12), Descrizione = "Abbonamento Palestra", Categoria = "Salute", Importo = 45.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 29, Data = new DateTime(2024, 6, 18), Descrizione = "Cena Compleanno", Categoria = "Ristoranti", Importo = 89.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 30, Data = new DateTime(2024, 6, 25), Descrizione = "Freelance Consulenza", Categoria = "Freelance", Importo = 400.00m, Tipo = TipoTransazione.Entrata },
            
            // Luglio 2024
            new() { Id = 31, Data = new DateTime(2024, 7, 5), Descrizione = "Stipendio Luglio", Categoria = "Stipendio", Importo = 2850.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 32, Data = new DateTime(2024, 7, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 33, Data = new DateTime(2024, 7, 15), Descrizione = "Vacanza Mare", Categoria = "Intrattenimento", Importo = 650.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 34, Data = new DateTime(2024, 7, 20), Descrizione = "Bolletta Condizionatore", Categoria = "Bollette", Importo = 185.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 35, Data = new DateTime(2024, 7, 28), Descrizione = "Quattordicesima", Categoria = "Stipendio", Importo = 2850.00m, Tipo = TipoTransazione.Entrata },
            
            // Agosto 2024
            new() { Id = 36, Data = new DateTime(2024, 8, 5), Descrizione = "Stipendio Agosto", Categoria = "Stipendio", Importo = 2850.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 37, Data = new DateTime(2024, 8, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 38, Data = new DateTime(2024, 8, 12), Descrizione = "Spesa Ferragosto", Categoria = "Spesa Alimentare", Importo = 245.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 39, Data = new DateTime(2024, 8, 20), Descrizione = "Ristorante Montagna", Categoria = "Ristoranti", Importo = 78.50m, Tipo = TipoTransazione.Uscita },
            
            // Settembre 2024
            new() { Id = 40, Data = new DateTime(2024, 9, 5), Descrizione = "Stipendio Settembre", Categoria = "Stipendio", Importo = 2900.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 41, Data = new DateTime(2024, 9, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 42, Data = new DateTime(2024, 9, 10), Descrizione = "Corso Formazione", Categoria = "Altro", Importo = 199.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 43, Data = new DateTime(2024, 9, 15), Descrizione = "Investimento Azioni", Categoria = "Investimenti", Importo = 500.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 44, Data = new DateTime(2024, 9, 22), Descrizione = "Freelance App Mobile", Categoria = "Freelance", Importo = 800.00m, Tipo = TipoTransazione.Entrata },
            
            // Ottobre 2024
            new() { Id = 45, Data = new DateTime(2024, 10, 5), Descrizione = "Stipendio Ottobre", Categoria = "Stipendio", Importo = 2900.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 46, Data = new DateTime(2024, 10, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 47, Data = new DateTime(2024, 10, 12), Descrizione = "Bolletta Riscaldamento", Categoria = "Bollette", Importo = 125.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 48, Data = new DateTime(2024, 10, 18), Descrizione = "Acquisto Giacca", Categoria = "Abbigliamento", Importo = 189.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 49, Data = new DateTime(2024, 10, 25), Descrizione = "Spesa Settimanale", Categoria = "Spesa Alimentare", Importo = 175.30m, Tipo = TipoTransazione.Uscita },
            
            // Novembre 2024
            new() { Id = 50, Data = new DateTime(2024, 11, 5), Descrizione = "Stipendio Novembre", Categoria = "Stipendio", Importo = 2900.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 51, Data = new DateTime(2024, 11, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 52, Data = new DateTime(2024, 11, 11), Descrizione = "Black Friday Tech", Categoria = "Altro", Importo = 349.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 53, Data = new DateTime(2024, 11, 15), Descrizione = "Cena Aziendale", Categoria = "Ristoranti", Importo = 0.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 54, Data = new DateTime(2024, 11, 20), Descrizione = "Dividendi ETF", Categoria = "Investimenti", Importo = 45.00m, Tipo = TipoTransazione.Entrata },
            
            // Dicembre 2024
            new() { Id = 55, Data = new DateTime(2024, 12, 5), Descrizione = "Stipendio Dicembre", Categoria = "Stipendio", Importo = 2900.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 56, Data = new DateTime(2024, 12, 7), Descrizione = "Affitto Appartamento", Categoria = "Affitto", Importo = 850.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 57, Data = new DateTime(2024, 12, 10), Descrizione = "Regali Natale", Categoria = "Altro", Importo = 385.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 58, Data = new DateTime(2024, 12, 15), Descrizione = "Spesa Natalizia", Categoria = "Spesa Alimentare", Importo = 298.50m, Tipo = TipoTransazione.Uscita },
            new() { Id = 59, Data = new DateTime(2024, 12, 20), Descrizione = "Tredicesima", Categoria = "Stipendio", Importo = 2900.00m, Tipo = TipoTransazione.Entrata },
            new() { Id = 60, Data = new DateTime(2024, 12, 28), Descrizione = "Cenone Capodanno", Categoria = "Ristoranti", Importo = 150.00m, Tipo = TipoTransazione.Uscita },
            new() { Id = 61, Data = new DateTime(2024, 12, 30), Descrizione = "Bollette Fine Anno", Categoria = "Bollette", Importo = 165.80m, Tipo = TipoTransazione.Uscita }
        };

        return transazioni;
    }

    public Task<DashboardKPI> GetDashboardKPIAsync()
    {
        var oggi = DateTime.Now;
        var meseCorrente = oggi.Month;
        var annoCorrente = oggi.Year;

        // Simuliamo dicembre 2024 per avere dati
        meseCorrente = 12;
        annoCorrente = 2024;

        var transazioniMese = _transazioni
            .Where(t => t.Data.Month == meseCorrente && t.Data.Year == annoCorrente)
            .ToList();

        var entrateMese = transazioniMese
            .Where(t => t.Tipo == TipoTransazione.Entrata)
            .Sum(t => t.Importo);

        var usciteMese = transazioniMese
            .Where(t => t.Tipo == TipoTransazione.Uscita)
            .Sum(t => t.Importo);

        // Calcolo patrimonio totale (somma di tutte le entrate - uscite storiche)
        var totaleEntrate = _transazioni
            .Where(t => t.Tipo == TipoTransazione.Entrata)
            .Sum(t => t.Importo);

        var totaleUscite = _transazioni
            .Where(t => t.Tipo == TipoTransazione.Uscita)
            .Sum(t => t.Importo);

        // Patrimonio iniziale simulato + entrate - uscite
        var patrimonioIniziale = 15000m;
        var totalePatrimonio = patrimonioIniziale + totaleEntrate - totaleUscite;

        // Calcolo variazioni rispetto al mese precedente
        var mesePrecedente = meseCorrente == 1 ? 12 : meseCorrente - 1;
        var annoPrecedente = meseCorrente == 1 ? annoCorrente - 1 : annoCorrente;

        var transazioniMesePrecedente = _transazioni
            .Where(t => t.Data.Month == mesePrecedente && t.Data.Year == annoPrecedente)
            .ToList();

        var entrateMesePrecedente = transazioniMesePrecedente
            .Where(t => t.Tipo == TipoTransazione.Entrata)
            .Sum(t => t.Importo);

        var usciteMesePrecedente = transazioniMesePrecedente
            .Where(t => t.Tipo == TipoTransazione.Uscita)
            .Sum(t => t.Importo);

        var kpi = new DashboardKPI
        {
            TotalePatrimonio = totalePatrimonio,
            EntrateMese = entrateMese,
            UsciteMese = usciteMese,
            RisparmioNetto = entrateMese - usciteMese,
            VariazionePatrimonio = 8.5m, // Simulato
            VariazioneEntrate = entrateMesePrecedente > 0 
                ? ((entrateMese - entrateMesePrecedente) / entrateMesePrecedente) * 100 
                : 0,
            VariazioneUscite = usciteMesePrecedente > 0 
                ? ((usciteMese - usciteMesePrecedente) / usciteMesePrecedente) * 100 
                : 0
        };

        return Task.FromResult(kpi);
    }

    public Task<List<Transaction>> GetTransazioniRecentiAsync(int count = 10)
    {
        var transazioniRecenti = _transazioni
            .OrderByDescending(t => t.Data)
            .Take(count)
            .ToList();

        return Task.FromResult(transazioniRecenti);
    }

    public Task<List<SpesaCategoria>> GetSpeseCategoriaAsync()
    {
        // Colori professionali stile Bloomberg/Trading
        var coloriCategorie = new Dictionary<string, string>
        {
            { "Affitto", "#FF6B6B" },
            { "Bollette", "#4ECDC4" },
            { "Spesa Alimentare", "#45B7D1" },
            { "Trasporti", "#96CEB4" },
            { "Intrattenimento", "#FFEAA7" },
            { "Salute", "#DDA0DD" },
            { "Abbigliamento", "#98D8C8" },
            { "Ristoranti", "#F7DC6F" },
            { "Investimenti", "#BB8FCE" },
            { "Altro", "#85C1E9" }
        };

        var spesePerCategoria = _transazioni
            .Where(t => t.Tipo == TipoTransazione.Uscita && t.Importo > 0)
            .GroupBy(t => t.Categoria)
            .Select(g => new SpesaCategoria
            {
                Categoria = g.Key,
                Importo = g.Sum(t => t.Importo),
                Colore = coloriCategorie.GetValueOrDefault(g.Key, "#A9A9A9")
            })
            .OrderByDescending(s => s.Importo)
            .ToList();

        var totaleSpese = spesePerCategoria.Sum(s => s.Importo);

        foreach (var spesa in spesePerCategoria)
        {
            spesa.Percentuale = totaleSpese > 0 
                ? (double)(spesa.Importo / totaleSpese) * 100 
                : 0;
        }

        return Task.FromResult(spesePerCategoria);
    }

    public Task<List<SaldoMensile>> GetAndamentoSaldoAsync()
    {
        var mesi = new[] { "Gen", "Feb", "Mar", "Apr", "Mag", "Giu", "Lug", "Ago", "Set", "Ott", "Nov", "Dic" };
        var saldoIniziale = 15000m;
        var andamento = new List<SaldoMensile>();

        for (int mese = 1; mese <= 12; mese++)
        {
            var transazioniFinoAlMese = _transazioni
                .Where(t => t.Data.Year == 2024 && t.Data.Month <= mese)
                .ToList();

            var entrate = transazioniFinoAlMese
                .Where(t => t.Tipo == TipoTransazione.Entrata)
                .Sum(t => t.Importo);

            var uscite = transazioniFinoAlMese
                .Where(t => t.Tipo == TipoTransazione.Uscita)
                .Sum(t => t.Importo);

            andamento.Add(new SaldoMensile
            {
                Mese = mesi[mese - 1],
                Saldo = saldoIniziale + entrate - uscite
            });
        }

        return Task.FromResult(andamento);
    }

    public Task<List<string>> GetCategorieAsync()
    {
        return Task.FromResult(_categorie);
    }

    public Task AddTransazioneAsync(Transaction transazione)
    {
        transazione.Id = _transazioni.Max(t => t.Id) + 1;
        _transazioni.Add(transazione);
        return Task.CompletedTask;
    }
}
