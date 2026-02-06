-- ============================================
-- MONEYTARGET - SQL SERVER DATABASE SCHEMA
-- Script T-SQL per la creazione delle tabelle
-- ============================================

USE master;
GO

-- Crea il database se non esiste
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MoneyTargetDB')
BEGIN
    CREATE DATABASE MoneyTargetDB;
END
GO

USE MoneyTargetDB;
GO

-- ============================================
-- TABELLA: Utenti
-- ============================================
IF OBJECT_ID('dbo.Utenti', 'U') IS NOT NULL
    DROP TABLE dbo.Utenti;
GO

CREATE TABLE dbo.Utenti (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Cognome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Valuta NVARCHAR(3) NOT NULL DEFAULT 'EUR',
    Lingua NVARCHAR(5) NOT NULL DEFAULT 'it',
    Tema NVARCHAR(20) NOT NULL DEFAULT 'dark',
    TwoFactorEnabled BIT NOT NULL DEFAULT 0,
    DataRegistrazione DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UltimoAccesso DATETIME2 NULL,
    IsAttivo BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT CK_Utenti_Email CHECK (Email LIKE '%_@_%._%')
);
GO

CREATE INDEX IX_Utenti_Email ON dbo.Utenti(Email);
GO

-- ============================================
-- TABELLA: Categorie
-- ============================================
IF OBJECT_ID('dbo.Categorie', 'U') IS NOT NULL
    DROP TABLE dbo.Categorie;
GO

CREATE TABLE dbo.Categorie (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtenteId INT NOT NULL,
    Nome NVARCHAR(50) NOT NULL,
    Tipo TINYINT NOT NULL, -- 0 = Entrata, 1 = Uscita
    Icona NVARCHAR(10) NOT NULL DEFAULT N'📦',
    Colore NVARCHAR(7) NOT NULL DEFAULT '#A9A9A9',
    Descrizione NVARCHAR(200) NULL,
    IsAttiva BIT NOT NULL DEFAULT 1,
    IsPredefinita BIT NOT NULL DEFAULT 0,
    Ordine INT NOT NULL DEFAULT 0,
    DataCreazione DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataModifica DATETIME2 NULL,
    
    CONSTRAINT FK_Categorie_Utenti FOREIGN KEY (UtenteId) 
        REFERENCES dbo.Utenti(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Categorie_Nome_Utente UNIQUE (UtenteId, Nome),
    CONSTRAINT CK_Categorie_Tipo CHECK (Tipo IN (0, 1)),
    CONSTRAINT CK_Categorie_Colore CHECK (Colore LIKE '#[0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f]')
);
GO

CREATE INDEX IX_Categorie_UtenteId ON dbo.Categorie(UtenteId);
CREATE INDEX IX_Categorie_Tipo ON dbo.Categorie(Tipo);
GO

-- ============================================
-- TABELLA: Transazioni
-- ============================================
IF OBJECT_ID('dbo.Transazioni', 'U') IS NOT NULL
    DROP TABLE dbo.Transazioni;
GO

CREATE TABLE dbo.Transazioni (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtenteId INT NOT NULL,
    CategoriaId INT NOT NULL,
    Data DATE NOT NULL,
    Descrizione NVARCHAR(200) NOT NULL,
    Importo DECIMAL(18,2) NOT NULL,
    Tipo TINYINT NOT NULL, -- 0 = Entrata, 1 = Uscita
    Note NVARCHAR(500) NULL,
    IsRicorrente BIT NOT NULL DEFAULT 0,
    RicorrenzaId INT NULL, -- Riferimento alla transazione ricorrente padre
    Allegato NVARCHAR(500) NULL, -- Path file allegato (ricevuta, fattura)
    DataCreazione DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataModifica DATETIME2 NULL,
    
    CONSTRAINT FK_Transazioni_Utenti FOREIGN KEY (UtenteId) 
        REFERENCES dbo.Utenti(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Transazioni_Categorie FOREIGN KEY (CategoriaId) 
        REFERENCES dbo.Categorie(Id),
    CONSTRAINT CK_Transazioni_Tipo CHECK (Tipo IN (0, 1)),
    CONSTRAINT CK_Transazioni_Importo CHECK (Importo >= 0)
);
GO

CREATE INDEX IX_Transazioni_UtenteId ON dbo.Transazioni(UtenteId);
CREATE INDEX IX_Transazioni_CategoriaId ON dbo.Transazioni(CategoriaId);
CREATE INDEX IX_Transazioni_Data ON dbo.Transazioni(Data DESC);
CREATE INDEX IX_Transazioni_Tipo ON dbo.Transazioni(Tipo);
CREATE INDEX IX_Transazioni_UtenteId_Data ON dbo.Transazioni(UtenteId, Data DESC);
GO

-- ============================================
-- TABELLA: Budget
-- ============================================
IF OBJECT_ID('dbo.Budget', 'U') IS NOT NULL
    DROP TABLE dbo.Budget;
GO

CREATE TABLE dbo.Budget (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtenteId INT NOT NULL,
    CategoriaId INT NOT NULL,
    ImportoMensile DECIMAL(18,2) NOT NULL,
    Anno INT NOT NULL,
    Mese TINYINT NOT NULL, -- 1-12, 0 = tutti i mesi (budget annuale)
    Note NVARCHAR(200) NULL,
    IsAttivo BIT NOT NULL DEFAULT 1,
    NotificaSoglia TINYINT NOT NULL DEFAULT 80, -- % per notifica
    DataCreazione DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataModifica DATETIME2 NULL,
    
    CONSTRAINT FK_Budget_Utenti FOREIGN KEY (UtenteId) 
        REFERENCES dbo.Utenti(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Budget_Categorie FOREIGN KEY (CategoriaId) 
        REFERENCES dbo.Categorie(Id),
    CONSTRAINT UQ_Budget_Utente_Categoria_Periodo UNIQUE (UtenteId, CategoriaId, Anno, Mese),
    CONSTRAINT CK_Budget_Importo CHECK (ImportoMensile > 0),
    CONSTRAINT CK_Budget_Mese CHECK (Mese BETWEEN 0 AND 12),
    CONSTRAINT CK_Budget_Soglia CHECK (NotificaSoglia BETWEEN 50 AND 100)
);
GO

CREATE INDEX IX_Budget_UtenteId ON dbo.Budget(UtenteId);
CREATE INDEX IX_Budget_CategoriaId ON dbo.Budget(CategoriaId);
CREATE INDEX IX_Budget_AnnoMese ON dbo.Budget(Anno, Mese);
GO

-- ============================================
-- TABELLA: Asset (Patrimonio)
-- ============================================
IF OBJECT_ID('dbo.Asset', 'U') IS NOT NULL
    DROP TABLE dbo.Asset;
GO

CREATE TABLE dbo.Asset (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtenteId INT NOT NULL,
    Nome NVARCHAR(100) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL, -- 'ContoCorrente', 'Risparmio', 'Investimento', 'Immobile', 'Veicolo', 'Crypto', 'Altro'
    ValoreIniziale DECIMAL(18,2) NOT NULL,
    ValoreAttuale DECIMAL(18,2) NOT NULL,
    Valuta NVARCHAR(3) NOT NULL DEFAULT 'EUR',
    Istituto NVARCHAR(100) NULL, -- Nome banca/broker
    NumeroRiferimento NVARCHAR(50) NULL, -- IBAN, numero conto, etc.
    Icona NVARCHAR(10) NOT NULL DEFAULT N'💰',
    Colore NVARCHAR(7) NOT NULL DEFAULT '#00D4AA',
    Note NVARCHAR(500) NULL,
    IsAttivo BIT NOT NULL DEFAULT 1,
    DataAcquisto DATE NULL,
    DataCreazione DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataModifica DATETIME2 NULL,
    
    CONSTRAINT FK_Asset_Utenti FOREIGN KEY (UtenteId) 
        REFERENCES dbo.Utenti(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Asset_Tipo CHECK (Tipo IN ('ContoCorrente', 'Risparmio', 'Investimento', 'Immobile', 'Veicolo', 'Crypto', 'Altro'))
);
GO

CREATE INDEX IX_Asset_UtenteId ON dbo.Asset(UtenteId);
CREATE INDEX IX_Asset_Tipo ON dbo.Asset(Tipo);
GO

-- ============================================
-- TABELLA: StoricoAsset (per tracciare variazioni)
-- ============================================
IF OBJECT_ID('dbo.StoricoAsset', 'U') IS NOT NULL
    DROP TABLE dbo.StoricoAsset;
GO

CREATE TABLE dbo.StoricoAsset (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AssetId INT NOT NULL,
    Valore DECIMAL(18,2) NOT NULL,
    Data DATE NOT NULL,
    Note NVARCHAR(200) NULL,
    DataCreazione DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_StoricoAsset_Asset FOREIGN KEY (AssetId) 
        REFERENCES dbo.Asset(Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_StoricoAsset_AssetId ON dbo.StoricoAsset(AssetId);
CREATE INDEX IX_StoricoAsset_Data ON dbo.StoricoAsset(Data DESC);
GO

-- ============================================
-- TABELLA: Impostazioni Utente
-- ============================================
IF OBJECT_ID('dbo.ImpostazioniUtente', 'U') IS NOT NULL
    DROP TABLE dbo.ImpostazioniUtente;
GO

CREATE TABLE dbo.ImpostazioniUtente (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtenteId INT NOT NULL UNIQUE,
    NotificheBudget BIT NOT NULL DEFAULT 1,
    SogliaBudget TINYINT NOT NULL DEFAULT 80,
    ReportSettimanale BIT NOT NULL DEFAULT 0,
    ReportMensile BIT NOT NULL DEFAULT 1,
    NotifichePush BIT NOT NULL DEFAULT 1,
    FormatoData NVARCHAR(20) NOT NULL DEFAULT 'dd/MM/yyyy',
    PrimoGiornoSettimana TINYINT NOT NULL DEFAULT 1, -- 0=Dom, 1=Lun
    BackupAutomatico BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_ImpostazioniUtente_Utenti FOREIGN KEY (UtenteId) 
        REFERENCES dbo.Utenti(Id) ON DELETE CASCADE
);
GO

-- ============================================
-- DATI INIZIALI: Categorie Predefinite
-- ============================================
-- Nota: Eseguire dopo aver creato almeno un utente

/*
-- Template per inserire categorie predefinite per un nuovo utente
DECLARE @UtenteId INT = 1; -- ID utente

INSERT INTO dbo.Categorie (UtenteId, Nome, Tipo, Icona, Colore, IsPredefinita, Ordine) VALUES
-- Entrate (Tipo = 0)
(@UtenteId, 'Stipendio', 0, N'💰', '#00D4AA', 1, 1),
(@UtenteId, 'Freelance', 0, N'💼', '#45B7D1', 1, 2),
(@UtenteId, 'Investimenti', 0, N'📈', '#BB8FCE', 1, 3),
(@UtenteId, 'Altro Entrata', 0, N'📥', '#96CEB4', 1, 4),
-- Uscite (Tipo = 1)
(@UtenteId, 'Affitto', 1, N'🏠', '#FF6B6B', 1, 10),
(@UtenteId, 'Bollette', 1, N'💡', '#4ECDC4', 1, 11),
(@UtenteId, 'Spesa Alimentare', 1, N'🛒', '#96CEB4', 1, 12),
(@UtenteId, 'Trasporti', 1, N'🚗', '#FFEAA7', 1, 13),
(@UtenteId, 'Intrattenimento', 1, N'🎬', '#F7DC6F', 1, 14),
(@UtenteId, 'Salute', 1, N'💊', '#DDA0DD', 1, 15),
(@UtenteId, 'Abbigliamento', 1, N'👔', '#98D8C8', 1, 16),
(@UtenteId, 'Ristoranti', 1, N'🍽️', '#85C1E9', 1, 17),
(@UtenteId, 'Altro Uscita', 1, N'📦', '#A9A9A9', 1, 99);
*/

PRINT 'Database MoneyTargetDB creato con successo!';
PRINT 'Tabelle create: Utenti, Categorie, Transazioni, Budget, Asset, StoricoAsset, ImpostazioniUtente';
GO
