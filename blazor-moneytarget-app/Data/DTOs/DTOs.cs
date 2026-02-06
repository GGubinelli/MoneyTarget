namespace FinanceApp.Data.DTOs;

/// <summary>
/// DTO per il risultato della Stored Procedure sp_GetBudgetUtilization
/// </summary>
public class BudgetUtilizationResult
{
    public int CategoriaId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Icona { get; set; } = "📦";
    public string Colore { get; set; } = "#A9A9A9";
    public decimal BudgetMensile { get; set; }
    public decimal SpesaAttuale { get; set; }
    public decimal Rimanente { get; set; }
    public decimal PercentualeUtilizzo { get; set; }
    public int SogliaNotifica { get; set; }
    public string Stato { get; set; } = "NoBudget"; // OK, Attenzione, Superato, NoBudget
    public int Anno { get; set; }
    public int Mese { get; set; }
}

/// <summary>
/// DTO per il risultato della Stored Procedure sp_GetDashboardSummary
/// </summary>
public class DashboardSummaryResult
{
    public decimal TotalePatrimonio { get; set; }
    public decimal EntrateMese { get; set; }
    public decimal UsciteMese { get; set; }
    public decimal RisparmioNetto { get; set; }
    public decimal VariazioneEntrate { get; set; }
    public decimal VariazioneUscite { get; set; }
    public int MeseCorrente { get; set; }
    public int AnnoCorrente { get; set; }
}

/// <summary>
/// DTO per il risultato della Stored Procedure sp_GetAndamentoSaldo
/// </summary>
public class AndamentoSaldoResult
{
    public string Mese { get; set; } = string.Empty;
    public int Anno { get; set; }
    public decimal Entrate { get; set; }
    public decimal Uscite { get; set; }
    public decimal SaldoCumulativo { get; set; }
}

/// <summary>
/// DTO per il risultato della Stored Procedure sp_GetSpesePorCategoria
/// </summary>
public class SpesaCategoriaResult
{
    public string Categoria { get; set; } = string.Empty;
    public string Icona { get; set; } = "📦";
    public string Colore { get; set; } = "#A9A9A9";
    public decimal Importo { get; set; }
    public decimal Percentuale { get; set; }
}

/// <summary>
/// DTO per la creazione/modifica di una transazione
/// </summary>
public class TransazioneDto
{
    public int? Id { get; set; }
    public DateTime Data { get; set; }
    public string Descrizione { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public decimal Importo { get; set; }
    public byte Tipo { get; set; } // 0 = Entrata, 1 = Uscita
    public string? Note { get; set; }
}

/// <summary>
/// DTO per la creazione/modifica di un budget
/// </summary>
public class BudgetDto
{
    public int? Id { get; set; }
    public int CategoriaId { get; set; }
    public decimal ImportoMensile { get; set; }
    public int Anno { get; set; }
    public byte Mese { get; set; }
    public string? Note { get; set; }
    public byte NotificaSoglia { get; set; } = 80;
}

/// <summary>
/// DTO per la creazione/modifica di una categoria
/// </summary>
public class CategoriaDto
{
    public int? Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public byte Tipo { get; set; }
    public string Icona { get; set; } = "📦";
    public string Colore { get; set; } = "#A9A9A9";
    public string? Descrizione { get; set; }
    public bool IsAttiva { get; set; } = true;
}

/// <summary>
/// DTO per la creazione/modifica di un asset
/// </summary>
public class AssetDto
{
    public int? Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Altro";
    public decimal ValoreIniziale { get; set; }
    public decimal ValoreAttuale { get; set; }
    public string Valuta { get; set; } = "EUR";
    public string? Istituto { get; set; }
    public string? NumeroRiferimento { get; set; }
    public string Icona { get; set; } = "💰";
    public string Colore { get; set; } = "#00D4AA";
    public string? Note { get; set; }
    public DateTime? DataAcquisto { get; set; }
}
