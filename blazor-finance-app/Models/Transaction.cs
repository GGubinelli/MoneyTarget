namespace FinanceApp.Models;

/// <summary>
/// Modello per le transazioni finanziarie
/// </summary>
public class Transaction
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public string Descrizione { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Importo { get; set; }
    public TipoTransazione Tipo { get; set; }
}

public enum TipoTransazione
{
    Entrata,
    Uscita
}

/// <summary>
/// Modello per i dati del grafico a torta
/// </summary>
public class SpesaCategoria
{
    public string Categoria { get; set; } = string.Empty;
    public decimal Importo { get; set; }
    public string Colore { get; set; } = string.Empty;
    public double Percentuale { get; set; }
}

/// <summary>
/// Modello per i dati del grafico a linee (andamento saldo)
/// </summary>
public class SaldoMensile
{
    public string Mese { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
}

/// <summary>
/// Modello per i KPI della dashboard
/// </summary>
public class DashboardKPI
{
    public decimal TotalePatrimonio { get; set; }
    public decimal EntrateMese { get; set; }
    public decimal UsciteMese { get; set; }
    public decimal RisparmioNetto { get; set; }
    public decimal VariazionePatrimonio { get; set; }
    public decimal VariazioneEntrate { get; set; }
    public decimal VariazioneUscite { get; set; }
}
