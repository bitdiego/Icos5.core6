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
PRINT N'Creazione di Vista [dbo].[EcOperationsByDate]...';


GO
CREATE VIEW [dbo].EcOperationsByDate
	AS 
	select Id, SiteId, EC_MODEL, EC_SN, EC_TYPE,EC_HEIGHT, EC_EASTWARD_DIST, EC_NORTHWARD_DIST, EC_SAMPLING_INT,
EC_SA_HEAT, EC_SA_OFFSET_N, EC_SA_WIND_FORMAT, EC_SA_GILL_ALIGN, EC_SA_GILL_PCI, 
EC_GA_FLOW_RATE, EC_GA_LICOR_FM_SN, EC_GA_LICOR_TP_SN, EC_GA_LICOR_AIU_SN, EC_GA_CAL_CO2_ZERO, EC_GA_CAL_CO2_OFFSET, EC_GA_CAL_CO2_REF,
EC_GA_CAL_H2O_ZERO, EC_GA_CAL_TA, EC_LOGGER, EC_FILE, EC_COMMENT, XDATE FROM
(select Id, SiteId, EC_MODEL, EC_SN, EC_TYPE,EC_HEIGHT, EC_EASTWARD_DIST, EC_NORTHWARD_DIST, EC_SAMPLING_INT,
EC_SA_HEAT, EC_SA_OFFSET_N, EC_SA_WIND_FORMAT, EC_SA_GILL_ALIGN, EC_SA_GILL_PCI, 
EC_GA_FLOW_RATE, EC_GA_LICOR_FM_SN, EC_GA_LICOR_TP_SN, EC_GA_LICOR_AIU_SN, EC_GA_CAL_CO2_ZERO, EC_GA_CAL_CO2_OFFSET, EC_GA_CAL_CO2_REF,
EC_GA_CAL_H2O_ZERO, EC_GA_CAL_TA, EC_LOGGER, EC_FILE, EC_COMMENT, EC_DATE as XDATE
from GRP_EC where DataStatus=0 AND EC_DATE is not null
Union 
select Id, SiteId, EC_MODEL, EC_SN, EC_TYPE,EC_HEIGHT, EC_EASTWARD_DIST, EC_NORTHWARD_DIST, EC_SAMPLING_INT,
EC_SA_HEAT, EC_SA_OFFSET_N, EC_SA_WIND_FORMAT, EC_SA_GILL_ALIGN, EC_SA_GILL_PCI, 
EC_GA_FLOW_RATE, EC_GA_LICOR_FM_SN, EC_GA_LICOR_TP_SN, EC_GA_LICOR_AIU_SN, EC_GA_CAL_CO2_ZERO, EC_GA_CAL_CO2_OFFSET, EC_GA_CAL_CO2_REF,
EC_GA_CAL_H2O_ZERO, EC_GA_CAL_TA, EC_LOGGER, EC_FILE, EC_COMMENT, EC_DATE_START as XDATE
from GRP_EC where DataStatus=0 AND EC_DATE_START is not null) AS ECT where ECT.SiteId>0
--AND [EC_TYPE] IN ('installation','Maintenance','removal') 
GROUP BY Id, SiteId, EC_MODEL, EC_SN, EC_TYPE,EC_HEIGHT, EC_EASTWARD_DIST, EC_NORTHWARD_DIST, EC_SAMPLING_INT,
EC_SA_HEAT, EC_SA_OFFSET_N, EC_SA_WIND_FORMAT, EC_SA_GILL_ALIGN, EC_SA_GILL_PCI, 
EC_GA_FLOW_RATE, EC_GA_LICOR_FM_SN, EC_GA_LICOR_TP_SN, EC_GA_LICOR_AIU_SN, EC_GA_CAL_CO2_ZERO, EC_GA_CAL_CO2_OFFSET, EC_GA_CAL_CO2_REF,
EC_GA_CAL_H2O_ZERO, EC_GA_CAL_TA, EC_LOGGER, EC_FILE, EC_COMMENT, XDATE
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
