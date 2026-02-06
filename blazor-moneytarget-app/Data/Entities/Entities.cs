using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyTarget.Data.Entities;

/// <summary>
/// Entità Utente - Rappresenta un utente dell'applicazione
/// </summary>
[Table("Utenti")]
public class Utente
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Il nome è obbligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Il nome deve essere tra 2 e 100 caratteri")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il cognome è obbligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Il cognome deve essere tra 2 e 100 caratteri")]
    public string Cognome { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email è obbligatoria")]
    [EmailAddress(ErrorMessage = "Formato email non valido")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(3)]
    public string Valuta { get; set; } = "EUR";

    [StringLength(5)]
    public string Lingua { get; set; } = "it";

    [StringLength(20)]
    public string Tema { get; set; } = "dark";

    public bool TwoFactorEnabled { get; set; } = false;

    public DateTime DataRegistrazione { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoAccesso { get; set; }

    public bool IsAttivo { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<Categoria> Categorie { get; set; } = new List<Categoria>();
    public virtual ICollection<Transazione> Transazioni { get; set; } = new List<Transazione>();
    public virtual ICollection<Budget> Budget { get; set; } = new List<Budget>();
    public virtual ICollection<Asset> Asset { get; set; } = new List<Asset>();
    public virtual ImpostazioniUtente? Impostazioni { get; set; }

    // Computed Properties
    [NotMapped]
    public string NomeCompleto => $"{Nome} {Cognome}";
}

/// <summary>
/// Tipo di transazione/categoria
/// </summary>
public enum TipoTransazione : byte
{
    Entrata = 0,
    Uscita = 1
}

/// <summary>
/// Entità Categoria - Raggruppa le transazioni per tipo
/// </summary>
[Table("Categorie")]
public class Categoria
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UtenteId { get; set; }

    [Required(ErrorMessage = "Il nome della categoria è obbligatorio")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Il nome deve essere tra 2 e 50 caratteri")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public TipoTransazione Tipo { get; set; }

    [StringLength(10)]
    public string Icona { get; set; } = "📦";

    [Required]
    [StringLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Formato colore non valido (es: #FF5733)")]
    public string Colore { get; set; } = "#A9A9A9";

    [StringLength(200)]
    public string? Descrizione { get; set; }

    public bool IsAttiva { get; set; } = true;

    public bool IsPredefinita { get; set; } = false;

    public int Ordine { get; set; } = 0;

    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    public DateTime? DataModifica { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(UtenteId))]
    public virtual Utente? Utente { get; set; }

    public virtual ICollection<Transazione> Transazioni { get; set; } = new List<Transazione>();
    public virtual ICollection<Budget> Budget { get; set; } = new List<Budget>();
}

/// <summary>
/// Entità Transazione - Rappresenta un movimento finanziario
/// </summary>
[Table("Transazioni")]
public class Transazione
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UtenteId { get; set; }

    [Required(ErrorMessage = "La categoria è obbligatoria")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "La data è obbligatoria")]
    [DataType(DataType.Date)]
    public DateTime Data { get; set; }

    [Required(ErrorMessage = "La descrizione è obbligatoria")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "La descrizione deve essere tra 3 e 200 caratteri")]
    public string Descrizione { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'importo è obbligatorio")]
    [Range(0.01, 999999999.99, ErrorMessage = "L'importo deve essere maggiore di 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Importo { get; set; }

    [Required]
    public TipoTransazione Tipo { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    public bool IsRicorrente { get; set; } = false;

    public int? RicorrenzaId { get; set; }

    [StringLength(500)]
    public string? Allegato { get; set; }

    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    public DateTime? DataModifica { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(UtenteId))]
    public virtual Utente? Utente { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public virtual Categoria? Categoria { get; set; }
}

/// <summary>
/// Entità Budget - Limite di spesa mensile per categoria
/// </summary>
[Table("Budget")]
public class Budget
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UtenteId { get; set; }

    [Required(ErrorMessage = "La categoria è obbligatoria")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "L'importo del budget è obbligatorio")]
    [Range(1, 999999999.99, ErrorMessage = "L'importo deve essere maggiore di 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ImportoMensile { get; set; }

    [Required]
    [Range(2020, 2100, ErrorMessage = "Anno non valido")]
    public int Anno { get; set; }

    [Required]
    [Range(0, 12, ErrorMessage = "Mese non valido (0 = tutti i mesi, 1-12 = mese specifico)")]
    public byte Mese { get; set; }

    [StringLength(200)]
    public string? Note { get; set; }

    public bool IsAttivo { get; set; } = true;

    [Range(50, 100, ErrorMessage = "La soglia deve essere tra 50% e 100%")]
    public byte NotificaSoglia { get; set; } = 80;

    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    public DateTime? DataModifica { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(UtenteId))]
    public virtual Utente? Utente { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public virtual Categoria? Categoria { get; set; }

    // Computed Properties
    [NotMapped]
    public string PeriodoDescrizione => Mese == 0 
        ? $"Anno {Anno}" 
        : $"{new DateTime(Anno, Mese, 1):MMMM yyyy}";
}

/// <summary>
/// Tipo di Asset patrimoniale
/// </summary>
public enum TipoAsset
{
    ContoCorrente,
    Risparmio,
    Investimento,
    Immobile,
    Veicolo,
    Crypto,
    Altro
}

/// <summary>
/// Entità Asset - Rappresenta un bene patrimoniale
/// </summary>
[Table("Asset")]
public class Asset
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UtenteId { get; set; }

    [Required(ErrorMessage = "Il nome dell'asset è obbligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Il nome deve essere tra 2 e 100 caratteri")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } = nameof(TipoAsset.Altro);

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValoreIniziale { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValoreAttuale { get; set; }

    [StringLength(3)]
    public string Valuta { get; set; } = "EUR";

    [StringLength(100)]
    public string? Istituto { get; set; }

    [StringLength(50)]
    public string? NumeroRiferimento { get; set; }

    [StringLength(10)]
    public string Icona { get; set; } = "💰";

    [StringLength(7)]
    public string Colore { get; set; } = "#00D4AA";

    [StringLength(500)]
    public string? Note { get; set; }

    public bool IsAttivo { get; set; } = true;

    [DataType(DataType.Date)]
    public DateTime? DataAcquisto { get; set; }

    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    public DateTime? DataModifica { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(UtenteId))]
    public virtual Utente? Utente { get; set; }

    public virtual ICollection<StoricoAsset> Storico { get; set; } = new List<StoricoAsset>();

    // Computed Properties
    [NotMapped]
    public decimal Variazione => ValoreAttuale - ValoreIniziale;

    [NotMapped]
    public decimal VariazionePercentuale => ValoreIniziale != 0 
        ? (Variazione / ValoreIniziale) * 100 
        : 0;
}

/// <summary>
/// Entità StoricoAsset - Traccia le variazioni di valore nel tempo
/// </summary>
[Table("StoricoAsset")]
public class StoricoAsset
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int AssetId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Valore { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Data { get; set; }

    [StringLength(200)]
    public string? Note { get; set; }

    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey(nameof(AssetId))]
    public virtual Asset? Asset { get; set; }
}

/// <summary>
/// Entità ImpostazioniUtente - Preferenze personalizzate
/// </summary>
[Table("ImpostazioniUtente")]
public class ImpostazioniUtente
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UtenteId { get; set; }

    public bool NotificheBudget { get; set; } = true;

    [Range(50, 100)]
    public byte SogliaBudget { get; set; } = 80;

    public bool ReportSettimanale { get; set; } = false;

    public bool ReportMensile { get; set; } = true;

    public bool NotifichePush { get; set; } = true;

    [StringLength(20)]
    public string FormatoData { get; set; } = "dd/MM/yyyy";

    [Range(0, 1)]
    public byte PrimoGiornoSettimana { get; set; } = 1; // 0=Dom, 1=Lun

    public bool BackupAutomatico { get; set; } = true;

    // Navigation Properties
    [ForeignKey(nameof(UtenteId))]
    public virtual Utente? Utente { get; set; }
}
