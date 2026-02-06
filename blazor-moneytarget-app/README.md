# 💰 MoneyTarget - Blazor Web App

Applicazione Web responsive per la gestione delle finanze personali, sviluppata in **C# / Blazor**.

## 📁 Struttura del Progetto

```
blazor-moneytarget-app/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor       # Layout principale con sidebar
│   │   └── NavMenu.razor          # Menu di navigazione laterale
│   └── Pages/
│       ├── Dashboard.razor        # Homepage con KPI e grafici
│       ├── NuovaTransazione.razor # Form inserimento transazioni
│       ├── Transazioni.razor      # Lista completa transazioni
│       ├── Budget.razor           # Gestione budget per categoria
│       ├── Report.razor           # Report e grafici analitici
│       ├── Categorie.razor        # CRUD categorie
│       └── Impostazioni.razor     # Impostazioni utente
├── Data/
│   ├── Entities/
│   │   └── Entities.cs            # Modelli Entity Framework
│   ├── DTOs/
│   │   └── DTOs.cs                # Data Transfer Objects
│   ├── Repositories/
│   │   └── FinanceRepository.cs   # Repository pattern
│   └── FinanceDbContext.cs        # DbContext EF Core
├── Database/
│   ├── 001_CreateTables.sql       # Script creazione tabelle
│   └── 002_StoredProcedures.sql   # Stored Procedures
├── Models/
│   └── Transaction.cs             # Modelli dati (Mock)
├── Services/
│   ├── IFinanceService.cs         # Interfaccia servizio
│   └── MockFinanceService.cs      # Implementazione Mock
├── wwwroot/
│   └── css/
│       ├── app.css                # Stili Dark Theme
│       └── pages.css              # Stili pagine aggiuntive
├── App.razor                      # Componente root HTML
├── Routes.razor                   # Configurazione routing
├── _Imports.razor                 # Import globali
├── Program.cs                     # Entry point applicazione
├── appsettings.json              # Configurazione
└── MoneyTarget.csproj            # File progetto .NET 8
```

## 🚀 Setup in Visual Studio

### Prerequisiti
- Visual Studio 2022 (17.8+)
- .NET 8 SDK
- SQL Server (opzionale, per persistenza dati reale)

### Passaggi

1. **Apri Visual Studio** e crea un nuovo progetto:
   - Seleziona "Blazor Web App"
   - Nome: `MoneyTarget`
   - Framework: `.NET 8`
   - Render mode: `Interactive Server`

2. **Copia i file** dalla cartella `blazor-moneytarget-app/` nel progetto

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

### Lista Transazioni
- Visualizzazione completa con filtri
- Ordinamento per data/importo
- Ricerca testuale

### Budget
- Gestione budget mensili per categoria
- Progress bar con stati (OK/Attenzione/Superato)
- Modal per aggiunta/modifica

### Report
- Grafici andamento entrate/uscite
- Risparmio cumulativo
- Analisi spese per categoria
- Insights automatici

### Categorie
- CRUD completo
- Icon picker e color picker
- Filtri per tipo (Entrata/Uscita)

### Impostazioni
- Profilo utente
- Preferenze (tema, lingua, valuta)
- Notifiche e sicurezza

## 🎨 Design

- **Tema**: Dark Mode professionale (stile Bloomberg/Trading)
- **Colori**: 
  - Background: `#0d1117`, `#161b22`
  - Accent verde: `#00D4AA` (entrate)
  - Accent rosso: `#f85149` (uscite)
- **Font**: Inter (UI), JetBrains Mono (numeri)
- **Responsive**: Sidebar → Hamburger menu su mobile

## 🔧 Configurazione SQL Server

### 1. Esegui gli script SQL
```sql
-- Esegui in ordine:
-- Database/001_CreateTables.sql
-- Database/002_StoredProcedures.sql
```

### 2. Configura la connection string
In `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TUO_SERVER;Database=MoneyTargetDB;..."
  }
}
```

### 3. Abilita Entity Framework
In `Program.cs`, decommenta la sezione SQL Server.

## 📊 Stored Procedures

| Procedura | Descrizione |
|-----------|-------------|
| `sp_GetBudgetUtilization` | Confronto spese vs budget con % utilizzo |
| `sp_GetDashboardSummary` | KPI per la dashboard |
| `sp_GetAndamentoSaldo` | Andamento saldo ultimi N mesi |
| `sp_GetSpesePorCategoria` | Ripartizione spese per grafico a torta |

---

Sviluppato come applicazione completa per la gestione finanze personali in Blazor .NET 8.
