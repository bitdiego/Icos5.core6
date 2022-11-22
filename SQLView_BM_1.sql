/*
Script di distribuzione per icosbis

Questo codice è stato generato da uno strumento.
Le modifiche apportate al file possono causare un comportamento non corretto e verranno perse se
il codice viene rigenerato.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "icosbis"
:setvar DefaultFilePrefix "icosbis"
:setvar DefaultDataPath ""
:setvar DefaultLogPath ""

GO
:on error exit
GO
/*
Rileva la modalità SQLCMD e disabilita l'esecuzione dello script se la modalità SQLCMD non è supportata.
Per abilitare nuovamente lo script dopo aver abilitato la modalità SQLCMD, eseguire l'istruzione seguente:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'Per la corretta esecuzione dello script è necessario abilitare la modalità SQLCMD.';
        SET NOEXEC ON;
    END


GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
PRINT N'Creazione di Vista [dbo].[BmOperationsByDate]...';


GO
CREATE VIEW [dbo].BmOperationsByDate
	AS SELECT  Id, siteID, insertDate, dataStatus, [BM_MODEL], [BM_SN], [BM_TYPE], [BM_VARIABLE_H_V_R],
[BM_HEIGHT],[BM_EASTWARD_DIST],[BM_NORTHWARD_DIST],[BM_SAMPLING_INT],[BM_INST_HEAT], [BM_INST_SHIELDING],
[BM_INST_ASPIRATION],[BM_LOGGER],[BM_FILE], XDATE, [BM_DATE_END],[BM_DATE_UNC],
 [BM_COMMENT] FROM
(select Id, siteID, insertDate, dataStatus, [BM_MODEL], [BM_SN], [BM_TYPE], [BM_VARIABLE_H_V_R],
[BM_HEIGHT],[BM_EASTWARD_DIST],[BM_NORTHWARD_DIST],[BM_SAMPLING_INT],[BM_INST_HEAT], [BM_INST_SHIELDING],
[BM_INST_ASPIRATION],[BM_LOGGER],[BM_FILE],[BM_DATE] as XDATE, [BM_DATE_START],[BM_DATE_END],[BM_DATE_UNC],
 [BM_COMMENT], [BM_NORTHREF],[BM_LOCATION_CALC_LAT],[BM_LOCATION_CALC_LONG]
 from GRP_BM where datastatus=0 and BM_DATE is not null
 UNION
 select Id, siteID, insertDate, dataStatus, [BM_MODEL], [BM_SN], [BM_TYPE], [BM_VARIABLE_H_V_R],
[BM_HEIGHT],[BM_EASTWARD_DIST],[BM_NORTHWARD_DIST],[BM_SAMPLING_INT],[BM_INST_HEAT], [BM_INST_SHIELDING],
[BM_INST_ASPIRATION],[BM_LOGGER],[BM_FILE],[BM_DATE] , [BM_DATE_START] as XDATE,[BM_DATE_END],[BM_DATE_UNC],
 [BM_COMMENT], [BM_NORTHREF],[BM_LOCATION_CALC_LAT],[BM_LOCATION_CALC_LONG]
 from GRP_BM where datastatus=0 and BM_DATE_START is not null) as T 
 GROUP BY Id, siteID, insertDate, dataStatus, [BM_MODEL], [BM_SN], [BM_TYPE], [BM_VARIABLE_H_V_R],
[BM_HEIGHT],[BM_EASTWARD_DIST],[BM_NORTHWARD_DIST],[BM_SAMPLING_INT],[BM_INST_HEAT], [BM_INST_SHIELDING],
[BM_INST_ASPIRATION],[BM_LOGGER],[BM_FILE], XDATE,[BM_DATE_END],[BM_DATE_UNC],
 [BM_COMMENT], [BM_NORTHREF],[BM_LOCATION_CALC_LAT],[BM_LOCATION_CALC_LONG], XDATE
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF OBJECT_ID(N'tempdb..#tmpErrors') IS NULL
    CREATE TABLE [#tmpErrors] (
        Error INT
    );

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT N'Aggiornamento della parte del database sottoposta a transazione completato.'
COMMIT TRANSACTION
END
ELSE PRINT N'Impossibile aggiornare la parte del database sottoposta a transazione.'
GO
IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
GO
PRINT N'Aggiornamento completato.';


GO
