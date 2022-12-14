use icosbis

select * from GRP_FILE where DataStatus=0 and SiteId!=70 order by siteId


select * from Sites where id=9

select * from GRP_INST where DataStatus=0 order by Id desc

SELECT  id_datastorage,groupID, siteID,[MODEL],  [SN],  [TYPE],LOGGER, FILEID, MAX(DATE) AS LastUpdateDate
FROM
(
SELECT id_datastorage,groupID, siteID, qual0 AS [MODEL], qual1 AS [SN], qual2 AS [TYPE],qual21 AS LOGGER ,qual22 AS FILEID, qual23 AS [DATE]
FROM DataStorageLive where groupID=2003 
UNION
SELECT id_datastorage, groupID, siteID, qual0 AS [MODEL], qual1 AS [SN], qual2 AS [TYPE],qual21 AS LOGGER ,qual22 AS FILEID, qual24 AS [DATE]
FROM DataStorageLive where groupID=2003 
) ud where ud.siteID=22 
--AND MODEL = '" + model + "' AND SN='" + sn + "'
AND [TYPE] IN ('installation','Maintenance','removal') 
GROUP BY id_datastorage,groupID, siteID,[MODEL],  [SN],  [TYPE],LOGGER, FILEID HAVING MAX(DATE)<='20230101' ORDER by  LastUpdateDate desc, MODEL, SN



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
--HAVING MAX(XDATE)<='20230101' ORDER by  XDATE desc, EC_MODEL, EC_SN


select * from EcOperationsByDate where siteId=22 and XDAte<'20230101'
AND [EC_TYPE] IN ('installation','Maintenance','removal') 
order by XDATE desc


select * from BmOperationsByDate

/*****************************/
select Id, SiteID,DataStatus, [STO_CONFIG], [STO_TYPE], [STO_PROF_ID], [STO_PROF_LEVEL],
[STO_PROF_HEIGHT],[STO_PROF_EASTWARD_DIST],[STO_PROF_NORTHWARD_DIST],[STO_PROF_HORIZ_S_POINTS], [STO_PROF_BUFFER_VOL],
 [STO_PROF_BUFFER_FLOWRATE], [STO_PROF_TUBE_LENGTH],[STO_PROF_TUBE_DIAM], [STO_PROF_TUBE_MAT], [STO_PROF_TUBE_THERM],
 [STO_PROF_SAMPLING_TIME],[STO_GA_MODEL], [STO_GA_SN], [STO_GA_VARIABLE], [STO_GA_PROF_ID],[STO_GA_FLOW_RATE],
[STO_GA_SAMPLING_INT],[STO_GA_AZIM_MOUNT],[STO_GA_VERT_MOUNT],[STO_GA_TUBE_LENGTH],[STO_GA_TUBE_DIAM],
 [STO_GA_TUBE_MAT], [STO_GA_TUBE_THERM], [STO_GA_CAL_VARIABLE], [STO_GA_CAL_VALUE],[STO_GA_CAL_REF],
 [STO_GA_CAL_TA], [STO_LOGGER], [STO_FILE], XDATE,[STO_DATE_END],
[STO_DATE_UNC], [STO_COMMENT] FROM 
(select Id, SiteID,DataStatus, [STO_CONFIG], [STO_TYPE], [STO_PROF_ID], [STO_PROF_LEVEL],
[STO_PROF_HEIGHT],[STO_PROF_EASTWARD_DIST],[STO_PROF_NORTHWARD_DIST],[STO_PROF_HORIZ_S_POINTS], [STO_PROF_BUFFER_VOL],
 [STO_PROF_BUFFER_FLOWRATE], [STO_PROF_TUBE_LENGTH],[STO_PROF_TUBE_DIAM], [STO_PROF_TUBE_MAT], [STO_PROF_TUBE_THERM],
 [STO_PROF_SAMPLING_TIME],[STO_GA_MODEL], [STO_GA_SN], [STO_GA_VARIABLE], [STO_GA_PROF_ID],[STO_GA_FLOW_RATE],
[STO_GA_SAMPLING_INT],[STO_GA_AZIM_MOUNT],[STO_GA_VERT_MOUNT],[STO_GA_TUBE_LENGTH],[STO_GA_TUBE_DIAM],
 [STO_GA_TUBE_MAT], [STO_GA_TUBE_THERM], [STO_GA_CAL_VARIABLE], [STO_GA_CAL_VALUE],[STO_GA_CAL_REF],
 [STO_GA_CAL_TA], [STO_LOGGER], [STO_FILE],[STO_DATE] AS XDATE,[STO_DATE_END],
[STO_DATE_UNC], [STO_COMMENT]
from GRP_STO where  Datastatus=0 AND STO_DATE is not null
UNION
select Id, SiteID,DataStatus, [STO_CONFIG], [STO_TYPE], [STO_PROF_ID], [STO_PROF_LEVEL],
[STO_PROF_HEIGHT],[STO_PROF_EASTWARD_DIST],[STO_PROF_NORTHWARD_DIST],[STO_PROF_HORIZ_S_POINTS], [STO_PROF_BUFFER_VOL],
 [STO_PROF_BUFFER_FLOWRATE], [STO_PROF_TUBE_LENGTH],[STO_PROF_TUBE_DIAM], [STO_PROF_TUBE_MAT], [STO_PROF_TUBE_THERM],
 [STO_PROF_SAMPLING_TIME],[STO_GA_MODEL], [STO_GA_SN], [STO_GA_VARIABLE], [STO_GA_PROF_ID],[STO_GA_FLOW_RATE],
[STO_GA_SAMPLING_INT],[STO_GA_AZIM_MOUNT],[STO_GA_VERT_MOUNT],[STO_GA_TUBE_LENGTH],[STO_GA_TUBE_DIAM],
 [STO_GA_TUBE_MAT], [STO_GA_TUBE_THERM], [STO_GA_CAL_VARIABLE], [STO_GA_CAL_VALUE],[STO_GA_CAL_REF],
 [STO_GA_CAL_TA], [STO_LOGGER], [STO_FILE],[STO_DATE_START] AS XDATE,[STO_DATE_END],
[STO_DATE_UNC], [STO_COMMENT]
from GRP_STO where  Datastatus=0 AND STO_DATE_START is not null) as T
GROUP By Id, SiteID,DataStatus, [STO_CONFIG], [STO_TYPE], [STO_PROF_ID], [STO_PROF_LEVEL],
[STO_PROF_HEIGHT],[STO_PROF_EASTWARD_DIST],[STO_PROF_NORTHWARD_DIST],[STO_PROF_HORIZ_S_POINTS], [STO_PROF_BUFFER_VOL],
 [STO_PROF_BUFFER_FLOWRATE], [STO_PROF_TUBE_LENGTH],[STO_PROF_TUBE_DIAM], [STO_PROF_TUBE_MAT], [STO_PROF_TUBE_THERM],
 [STO_PROF_SAMPLING_TIME],[STO_GA_MODEL], [STO_GA_SN], [STO_GA_VARIABLE], [STO_GA_PROF_ID],[STO_GA_FLOW_RATE],
[STO_GA_SAMPLING_INT],[STO_GA_AZIM_MOUNT],[STO_GA_VERT_MOUNT],[STO_GA_TUBE_LENGTH],[STO_GA_TUBE_DIAM],
 [STO_GA_TUBE_MAT], [STO_GA_TUBE_THERM], [STO_GA_CAL_VARIABLE], [STO_GA_CAL_VALUE],[STO_GA_CAL_REF],
 [STO_GA_CAL_TA], [STO_LOGGER], [STO_FILE], XDATE,[STO_DATE_END],
[STO_DATE_UNC], [STO_COMMENT]


select * from StoOperationsByDate where XDATE is null