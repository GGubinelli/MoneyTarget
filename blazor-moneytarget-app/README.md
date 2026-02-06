# 💰 Finance Manager - Blazor Web App

Applicazione Web responsive per la gestione delle finanze personali, sviluppata in **C# / Blazor**.

## 📁 Struttura del Progetto

```
blazor-finance-app/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor       # Layout principale con sidebar
│   │   └── NavMenu.razor          # Menu di navigazione laterale
│   └── Pages/
│       ├── Dashboard.razor        # Homepage con KPI e grafici
│       ├── NuovaTransazione.razor # Form inserimento transazioni
│       └── Transazioni.razor      # Lista completa transazioni
├── Models/
│   └── Transaction.cs             # Modelli dati (Transaction, KPI, etc.)
├── Services/
│   ├── IFinanceService.cs         # Interfaccia servizio
│   └── MockFinanceService.cs      # Implementazione Mock (dati finti)
├── wwwroot/
│   └── css/
│       └── app.css                # Stili Dark Theme (Bloomberg style)
├── App.razor                      # Componente root HTML
├── Routes.razor                   # Configurazione routing
├── _Imports.razor                 # Import globali
├── Program.cs                     # Entry point applicazione
└── FinanceApp.csproj             # File progetto .NET 8
```

## 🚀 Setup in Visual Studio

### Prerequisiti
- Visual Studio 2022 (17.8+)
- .NET 8 SDK
- SQL Server (opzionale, per persistenza dati reale)

### Passaggi

1. **Apri Visual Studio** e crea un nuovo progetto:
   - Seleziona "Blazor Web App"
   - Nome: `FinanceApp`
   - Framework: `.NET 8`
   - Render mode: `Interactive Server`

2. **Copia i file** dalla cartella `blazor-finance-app/` nel progetto

3. **Esegui l'applicazione**:
   ```bash
   dotnet run
   ```

4. Apri il browser su `https://localhost:5001`

## ✨ Funzionalità

### Dashboard (Home)
- **KPI Cards**: Patrimonio totale, Entrate/Uscite mese, Risparmio netto
- **Grafico a Torta**: Ripartizione spese per categoria
- **Grafico a Linee**: Andamento saldo ultimi 12 mesi
- **Lista Transazioni Recenti**: Ultime 10 operazioni

### Nuova Transazione
- Form con validazione (`EditForm` + `DataAnnotationsValidator`)
- Selezione tipo (Entrata/Uscita)
- Input importo, data, categoria, descrizione
- Riepilogo in tempo reale
- Toast di conferma salvataggio

### Lista Transazioni
- Visualizzazione completa con filtri
- Ordinamento per data/importo
- Ricerca testuale
- Statistiche rapide (totale entrate/uscite)

## 🎨 Design

- **Tema**: Dark Mode professionale (stile Bloomberg/Trading)
- **Colori**: 
  - Background: `#0d1117`, `#161b22`
  - Accent verde: `#00D4AA` (entrate)
  - Accent rosso: `#f85149` (uscite)
- **Font**: Inter (UI), JetBrains Mono (numeri)
- **Responsive**: Sidebar → Hamburger menu su mobile

## 🔧 Personalizzazione per SQL Server

Per collegare a SQL Server reale, sostituisci `MockFinanceService` con una nuova implementazione:

```csharp
// Services/SqlFinanceService.cs
public class SqlFinanceService : IFinanceService
{
    private readonly string _connectionString;
    
    public SqlFinanceService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }
    
    public async Task<List<Transaction>> GetTransazioniRecentiAsync(int count = 10)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "SELECT TOP(@Count) * FROM Transactions ORDER BY Data DESC";
        return (await connection.QueryAsync<Transaction>(query, new { Count = count })).ToList();
    }
    
    // ... altri metodi
}
```

E in `Program.cs`:
```csharp
// Sostituisci:
// builder.Services.AddScoped<IFinanceService, MockFinanceService>();
// Con:
builder.Services.AddScoped<IFinanceService, SqlFinanceService>();
```

## 📊 Schema Database SQL Server (suggerito)

```sql
CREATE TABLE Transactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Data DATETIME2 NOT NULL,
    Descrizione NVARCHAR(200) NOT NULL,
    Categoria NVARCHAR(50) NOT NULL,
    Importo DECIMAL(18,2) NOT NULL,
    Tipo INT NOT NULL -- 0 = Entrata, 1 = Uscita
);

CREATE TABLE Categorie (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(50) NOT NULL
);

-- Indici per performance
CREATE INDEX IX_Transactions_Data ON Transactions(Data DESC);
CREATE INDEX IX_Transactions_Categoria ON Transactions(Categoria);
```

## 📱 Screenshot Attesi

### Desktop
- Sidebar di navigazione a sinistra (260px)
- Area contenuto centrale con grafici e tabelle
- KPI cards in griglia 4 colonne

### Mobile (< 768px)
- Sidebar nascosta (hamburger menu)
- KPI cards in singola colonna
- Grafici a larghezza piena

## 🔐 Valuta

L'applicazione usa **Euro (€)** come valuta predefinita. Per cambiarla, modifica il metodo `FormatCurrency` nei componenti Razor:

```csharp
private string FormatCurrency(decimal amount)
{
    return amount.ToString("C", new CultureInfo("it-IT")); // Euro
    // return amount.ToString("C", new CultureInfo("en-US")); // Dollaro
}
```

---

Sviluppato come demo per la gestione finanze personali in Blazor .NET 8.
