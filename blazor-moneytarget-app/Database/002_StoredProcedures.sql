-- ============================================
-- STORED PROCEDURE: Confronto Spese vs Budget
-- Raggruppa le spese per categoria nell'ultimo mese
-- e le confronta con il budget, restituendo la % di utilizzo
-- ============================================

USE MoneyTargetDB;
GO

IF OBJECT_ID('dbo.sp_GetBudgetUtilization', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetBudgetUtilization;
GO

CREATE PROCEDURE dbo.sp_GetBudgetUtilization
    @UtenteId INT,
    @Anno INT = NULL,
    @Mese INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Se non specificati, usa mese/anno corrente
    IF @Anno IS NULL SET @Anno = YEAR(GETDATE());
    IF @Mese IS NULL SET @Mese = MONTH(GETDATE());
    
    -- CTE per calcolare le spese del mese per categoria
    ;WITH SpeseMese AS (
        SELECT 
            t.CategoriaId,
            SUM(t.Importo) AS TotaleSpeso
        FROM dbo.Transazioni t
        WHERE t.UtenteId = @UtenteId
          AND t.Tipo = 1  -- Solo uscite
          AND YEAR(t.Data) = @Anno
          AND MONTH(t.Data) = @Mese
        GROUP BY t.CategoriaId
    ),
    -- CTE per il budget (considera sia budget specifico del mese che annuale)
    BudgetMese AS (
        SELECT 
            b.CategoriaId,
            b.ImportoMensile,
            b.NotificaSoglia
        FROM dbo.Budget b
        WHERE b.UtenteId = @UtenteId
          AND b.IsAttivo = 1
          AND b.Anno = @Anno
          AND (b.Mese = @Mese OR b.Mese = 0) -- Mese specifico o budget annuale
    )
    SELECT 
        c.Id AS CategoriaId,
        c.Nome AS Categoria,
        c.Icona,
        c.Colore,
        ISNULL(bm.ImportoMensile, 0) AS BudgetMensile,
        ISNULL(sm.TotaleSpeso, 0) AS SpesaAttuale,
        -- Calcolo rimanente (può essere negativo se superato)
        ISNULL(bm.ImportoMensile, 0) - ISNULL(sm.TotaleSpeso, 0) AS Rimanente,
        -- Percentuale di utilizzo
        CASE 
            WHEN ISNULL(bm.ImportoMensile, 0) = 0 THEN 0
            ELSE CAST(ROUND((ISNULL(sm.TotaleSpeso, 0) / bm.ImportoMensile) * 100, 2) AS DECIMAL(5,2))
        END AS PercentualeUtilizzo,
        -- Soglia notifica
        ISNULL(bm.NotificaSoglia, 80) AS SogliaNotifica,
        -- Stato: 'OK', 'Attenzione', 'Superato', 'NoBudget'
        CASE 
            WHEN bm.ImportoMensile IS NULL OR bm.ImportoMensile = 0 THEN 'NoBudget'
            WHEN (sm.TotaleSpeso / bm.ImportoMensile) * 100 >= 100 THEN 'Superato'
            WHEN (sm.TotaleSpeso / bm.ImportoMensile) * 100 >= ISNULL(bm.NotificaSoglia, 80) THEN 'Attenzione'
            ELSE 'OK'
        END AS Stato,
        @Anno AS Anno,
        @Mese AS Mese
    FROM dbo.Categorie c
    LEFT JOIN BudgetMese bm ON c.Id = bm.CategoriaId
    LEFT JOIN SpeseMese sm ON c.Id = sm.CategoriaId
    WHERE c.UtenteId = @UtenteId
      AND c.Tipo = 1  -- Solo categorie di uscita
      AND c.IsAttiva = 1
    ORDER BY 
        CASE 
            WHEN bm.ImportoMensile IS NULL THEN 2
            WHEN (ISNULL(sm.TotaleSpeso, 0) / NULLIF(bm.ImportoMensile, 0)) * 100 >= 100 THEN 0
            ELSE 1
        END,
        PercentualeUtilizzo DESC;
END
GO

-- ============================================
-- STORED PROCEDURE: Riepilogo Dashboard
-- ============================================
IF OBJECT_ID('dbo.sp_GetDashboardSummary', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetDashboardSummary;
GO

CREATE PROCEDURE dbo.sp_GetDashboardSummary
    @UtenteId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @MeseCorrente INT = MONTH(GETDATE());
    DECLARE @AnnoCorrente INT = YEAR(GETDATE());
    DECLARE @MesePrecedente INT = CASE WHEN @MeseCorrente = 1 THEN 12 ELSE @MeseCorrente - 1 END;
    DECLARE @AnnoPrecedente INT = CASE WHEN @MeseCorrente = 1 THEN @AnnoCorrente - 1 ELSE @AnnoCorrente END;
    
    -- Calcolo totali mese corrente
    DECLARE @EntrateMese DECIMAL(18,2), @UsciteMese DECIMAL(18,2);
    DECLARE @EntrateMesePrecedente DECIMAL(18,2), @UsciteMesePrecedente DECIMAL(18,2);
    
    SELECT @EntrateMese = ISNULL(SUM(Importo), 0)
    FROM dbo.Transazioni
    WHERE UtenteId = @UtenteId AND Tipo = 0 
      AND YEAR(Data) = @AnnoCorrente AND MONTH(Data) = @MeseCorrente;
    
    SELECT @UsciteMese = ISNULL(SUM(Importo), 0)
    FROM dbo.Transazioni
    WHERE UtenteId = @UtenteId AND Tipo = 1 
      AND YEAR(Data) = @AnnoCorrente AND MONTH(Data) = @MeseCorrente;
    
    SELECT @EntrateMesePrecedente = ISNULL(SUM(Importo), 0)
    FROM dbo.Transazioni
    WHERE UtenteId = @UtenteId AND Tipo = 0 
      AND YEAR(Data) = @AnnoPrecedente AND MONTH(Data) = @MesePrecedente;
    
    SELECT @UsciteMesePrecedente = ISNULL(SUM(Importo), 0)
    FROM dbo.Transazioni
    WHERE UtenteId = @UtenteId AND Tipo = 1 
      AND YEAR(Data) = @AnnoPrecedente AND MONTH(Data) = @MesePrecedente;
    
    -- Calcolo patrimonio totale (Asset attivi)
    DECLARE @TotalePatrimonio DECIMAL(18,2);
    SELECT @TotalePatrimonio = ISNULL(SUM(ValoreAttuale), 0)
    FROM dbo.Asset
    WHERE UtenteId = @UtenteId AND IsAttivo = 1;
    
    -- Output
    SELECT 
        @TotalePatrimonio AS TotalePatrimonio,
        @EntrateMese AS EntrateMese,
        @UsciteMese AS UsciteMese,
        (@EntrateMese - @UsciteMese) AS RisparmioNetto,
        -- Variazioni percentuali
        CASE 
            WHEN @EntrateMesePrecedente = 0 THEN 0
            ELSE CAST(ROUND(((@EntrateMese - @EntrateMesePrecedente) / @EntrateMesePrecedente) * 100, 1) AS DECIMAL(5,1))
        END AS VariazioneEntrate,
        CASE 
            WHEN @UsciteMesePrecedente = 0 THEN 0
            ELSE CAST(ROUND(((@UsciteMese - @UsciteMesePrecedente) / @UsciteMesePrecedente) * 100, 1) AS DECIMAL(5,1))
        END AS VariazioneUscite,
        @MeseCorrente AS MeseCorrente,
        @AnnoCorrente AS AnnoCorrente;
END
GO

-- ============================================
-- STORED PROCEDURE: Andamento Saldo Mensile
-- ============================================
IF OBJECT_ID('dbo.sp_GetAndamentoSaldo', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetAndamentoSaldo;
GO

CREATE PROCEDURE dbo.sp_GetAndamentoSaldo
    @UtenteId INT,
    @NumeroMesi INT = 12
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calcola il patrimonio iniziale (somma asset)
    DECLARE @PatrimonioIniziale DECIMAL(18,2);
    SELECT @PatrimonioIniziale = ISNULL(SUM(ValoreIniziale), 0)
    FROM dbo.Asset
    WHERE UtenteId = @UtenteId;
    
    ;WITH Mesi AS (
        SELECT 
            DATEADD(MONTH, -n, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS PrimoGiornoMese
        FROM (
            SELECT TOP (@NumeroMesi) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
            FROM sys.objects
        ) nums
    ),
    TransazioniMensili AS (
        SELECT 
            DATEFROMPARTS(YEAR(Data), MONTH(Data), 1) AS Mese,
            SUM(CASE WHEN Tipo = 0 THEN Importo ELSE 0 END) AS Entrate,
            SUM(CASE WHEN Tipo = 1 THEN Importo ELSE 0 END) AS Uscite
        FROM dbo.Transazioni
        WHERE UtenteId = @UtenteId
          AND Data >= DATEADD(MONTH, -@NumeroMesi, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        GROUP BY DATEFROMPARTS(YEAR(Data), MONTH(Data), 1)
    )
    SELECT 
        FORMAT(m.PrimoGiornoMese, 'MMM', 'it-IT') AS Mese,
        YEAR(m.PrimoGiornoMese) AS Anno,
        ISNULL(tm.Entrate, 0) AS Entrate,
        ISNULL(tm.Uscite, 0) AS Uscite,
        @PatrimonioIniziale + (
            SELECT ISNULL(SUM(
                CASE WHEN t2.Tipo = 0 THEN t2.Importo ELSE -t2.Importo END
            ), 0)
            FROM dbo.Transazioni t2
            WHERE t2.UtenteId = @UtenteId
              AND t2.Data <= EOMONTH(m.PrimoGiornoMese)
        ) AS SaldoCumulativo
    FROM Mesi m
    LEFT JOIN TransazioniMensili tm ON m.PrimoGiornoMese = tm.Mese
    ORDER BY m.PrimoGiornoMese;
END
GO

-- ============================================
-- STORED PROCEDURE: Spese per Categoria (Torta)
-- ============================================
IF OBJECT_ID('dbo.sp_GetSpesePorCategoria', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetSpesePorCategoria;
GO

CREATE PROCEDURE dbo.sp_GetSpesePorCategoria
    @UtenteId INT,
    @Anno INT = NULL,
    @Mese INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Anno IS NULL SET @Anno = YEAR(GETDATE());
    IF @Mese IS NULL SET @Mese = MONTH(GETDATE());
    
    DECLARE @TotaleSpese DECIMAL(18,2);
    
    SELECT @TotaleSpese = ISNULL(SUM(Importo), 0)
    FROM dbo.Transazioni
    WHERE UtenteId = @UtenteId AND Tipo = 1
      AND YEAR(Data) = @Anno AND MONTH(Data) = @Mese;
    
    SELECT 
        c.Nome AS Categoria,
        c.Icona,
        c.Colore,
        ISNULL(SUM(t.Importo), 0) AS Importo,
        CASE 
            WHEN @TotaleSpese = 0 THEN 0
            ELSE CAST(ROUND((ISNULL(SUM(t.Importo), 0) / @TotaleSpese) * 100, 2) AS DECIMAL(5,2))
        END AS Percentuale
    FROM dbo.Categorie c
    LEFT JOIN dbo.Transazioni t ON c.Id = t.CategoriaId
        AND YEAR(t.Data) = @Anno AND MONTH(t.Data) = @Mese
    WHERE c.UtenteId = @UtenteId
      AND c.Tipo = 1
      AND c.IsAttiva = 1
    GROUP BY c.Id, c.Nome, c.Icona, c.Colore
    HAVING ISNULL(SUM(t.Importo), 0) > 0
    ORDER BY Importo DESC;
END
GO

PRINT 'Stored Procedures create con successo!';
PRINT 'Procedure disponibili:';
PRINT '  - sp_GetBudgetUtilization: Confronto spese vs budget con % utilizzo';
PRINT '  - sp_GetDashboardSummary: KPI per la dashboard';
PRINT '  - sp_GetAndamentoSaldo: Andamento saldo ultimi N mesi';
PRINT '  - sp_GetSpesePorCategoria: Ripartizione spese per grafico a torta';
GO
