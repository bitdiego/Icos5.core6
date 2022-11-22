using Icos5.core6.DbManager;
using Icos5.core6.Models.File;
using IcosClassLibrary.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Policy;

namespace Icos5.core6.Services.FileServices
{
    public class EFFileService : IFIleService
    {
        private readonly IcosDbContext _context;
        public EFFileService(IcosDbContext context)
        {
            _context = context;
        }
        public IEnumerable<FileForms> GetGrpFileList(int siteId, int logger_id, int file_id, string type)
        {
            var fileInfoList = _context.GRP_FILE.Where(ff => ff.SiteId == siteId && ff.FILE_TYPE.ToLower() == type.ToLower() && ff.FILE_LOGGER_ID == logger_id
                                                        && ff.FILE_ID == file_id && ff.DataStatus == 0).OrderByDescending(ff => ff.FILE_DATE).ToList();
            var ffList = Mappers.Mappers.GrpFileToFileForms(fileInfoList);
            return ffList;
        }

        public IEnumerable<FileFormsExt> GetGrpFileListExt(int siteId, int logger_id, int file_id, string type)
        {
            var fileInfoList = _context.GRP_FILE.Where(ff => ff.SiteId == siteId && ff.FILE_TYPE.ToLower() == type.ToLower() && ff.FILE_LOGGER_ID == logger_id
                                                        && ff.FILE_ID == file_id && ff.DataStatus == 0).OrderByDescending(ff => ff.FILE_DATE).ToList();
            var ffList = Mappers.Mappers.GrpFileToFileFormsNew(fileInfoList);
            return ffList;
        }

        public string GetCompressed(int siteId, int logger_id, int file_id, string type)
        {
            string result = "0";
            var fileInfo = _context.GRP_FILE.Where(ff => ff.SiteId == siteId && ff.FILE_TYPE.ToLower() == type.ToLower() && ff.FILE_LOGGER_ID == logger_id
                                                        && ff.FILE_ID == file_id && ff.DataStatus == 0).OrderByDescending(ff => ff.FILE_DATE).FirstOrDefault();

            if (fileInfo != null)
            {
                if (!String.IsNullOrEmpty(fileInfo.FILE_COMPRESS))
                {
                    result = "1";
                }
            }

            return result;
        }

        public IEnumerable<Epox> GetEpoch(int siteId, int logger_id, int file_id, string type)
        {
            List<Epox> epoxList = new List<Epox>();
            //maybe need top 1?
            var epoxInfo = _context.GRP_FILE.Where(ff => ff.SiteId == siteId && ff.FILE_TYPE.ToLower() == type.ToLower() && ff.FILE_LOGGER_ID == logger_id
                                                   && ff.FILE_ID == file_id && ff.DataStatus == 0).OrderByDescending(ff => ff.FILE_DATE).ToList();

            if (epoxInfo != null)
            {
                foreach (var info in epoxInfo)
                {
                    Epox epox = new Epox();
                    epox.FileEpoch = info.FILE_EPOCH.Substring(0, "199001010000".Length);
                    epoxList.Add(epox);
                }
            }
            return epoxList;
        }

        public IEnumerable<MissTimeBin> GetMissing(int siteId, int logger_id, int file_id, string type)
        {
            List<MissTimeBin> mtb = new List<MissTimeBin>();

            var missingInfo = _context.GRP_FILE.Where(ff => ff.SiteId == siteId && ff.FILE_TYPE.ToLower() == type.ToLower() && ff.FILE_LOGGER_ID == logger_id
                                                   && ff.FILE_ID == file_id && ff.DataStatus == 0).OrderByDescending(ff => ff.FILE_DATE).FirstOrDefault();
            //.ToList();
            if (missingInfo != null)
            {
                MissTimeBin mb = new MissTimeBin();
                mb.MissingValue = missingInfo.FILE_MISSING_VALUE;
                mtb.Add(mb);
            }
            return mtb;
        }
        public IEnumerable<MissTimeBin> GetTimestamp(int siteId, int logger_id, int file_id, string type)
        {
            List<MissTimeBin> mtb = new List<MissTimeBin>();

            var missingInfo = _context.GRP_FILE.Where(ff => ff.SiteId == siteId && ff.FILE_TYPE.ToLower() == type.ToLower() && ff.FILE_LOGGER_ID == logger_id
                                                   && ff.FILE_ID == file_id && ff.DataStatus == 0).OrderByDescending(ff => ff.FILE_DATE).FirstOrDefault();
            //.ToList();
            if (missingInfo != null)
            {
                MissTimeBin mb = new MissTimeBin();
                mb.MissingValue = missingInfo.FILE_MISSING_VALUE;
                mtb.Add(mb);
            }
            return mtb;
        }

       public GRP_FILE? GetGrpFile(int siteId, int logger_id, int file_id, string type)
       {
            var fileInfo = _context.GRP_FILE.Where(ff => ff.SiteId == siteId && ff.FILE_TYPE.ToLower() == type.ToLower() && ff.FILE_LOGGER_ID == logger_id
                                                        && ff.FILE_ID == file_id && ff.DataStatus == 0).OrderByDescending(ff => ff.FILE_DATE).FirstOrDefault();
            return fileInfo;
       }
        public IEnumerable<LoggerFile> GetExpectedFiles(int siteId, string type, string timestamp)
        {
            List<string> loggerIdList = new List<string>();
            List<string> fileIdList = new List<string>();
            List<LoggerFile> lfList = new List<LoggerFile>(); //rename in lfList or whatever
            switch (type.ToLower())
            {
                case "ec":
                    GetEcExpectedFiles(siteId, timestamp, lfList);
                    break;
                case "bm":
                    GetBMExpectedFiles(siteId, timestamp, lfList);
                    break;
                case "st":
                case "sto":
                    GetStoExpectedFiles(siteId, timestamp, lfList);
                    break;
                default:
                    break;
            }
            if (String.Compare(type, "EC", true) == 0)
            {

            }
            if (String.Compare(type, "EC", true) == 0)
            {

            }
            return lfList;
        }

        private void GetStoExpectedFiles(int siteId, string timestamp, List<LoggerFile> lfList)
        {
            //if STO_CONFIG is sequential or simultaneous, get only the latest L,F couple (only one L F is valid)
            //else, get all the L.F pairs
            string stoConfig = "";
            bool isSeqOrSim = false;
            var stoCazzo = _context.StoOperationsByDate.Where(st => st.SiteId == siteId && st.STO_DATE.CompareTo(timestamp) <= 0 &&
                                                        ((st.STO_TYPE.ToLower() == "ga installation" || st.STO_TYPE.ToLower() == "variable map" || st.STO_TYPE.ToLower() == "ga removal")))
                                                        .OrderByDescending(st => st.STO_DATE).ThenByDescending(st => st.STO_TYPE).ThenBy(st => st.STO_GA_MODEL).ThenBy(st => st.STO_GA_SN).ToList();

            if (stoCazzo.Count > 0)
            {
                foreach(var stoca in stoCazzo)
                {
                    string lastOp = stoca.STO_TYPE.ToLower();
                    int? loggerId;
                    int? fileId;
                    string ts = "";
                    switch (lastOp)
                    {
                        case "removal":
                        case "ga removal":
                            //go = false;
                            break;
                        case "installation":
                            loggerId = stoca.STO_LOGGER;
                            fileId = stoca.STO_FILE;
                            ts = stoca.STO_DATE;
                            //add loggerid and fileid to list, if not already present
                            if (loggerId !=null && fileId!=null)
                            {
                                //add loggerid and fileid to list, if not already present
                                AddLoggerFileItem((int)loggerId, (int)fileId, lfList);
                            }
                            break;
                        case "ga installation":
                        case "variable map":
                            loggerId = stoca.STO_LOGGER;
                            fileId = stoca.STO_FILE;
                            ts = stoca.STO_DATE;
                            //add loggerid and fileid to list, if not already present
                            if (loggerId != null && fileId != null)
                            {
                                //add loggerid and fileid to list, if not already present
                                AddLoggerFileItem((int)loggerId, (int)fileId, lfList);
                            }
                            break;
                        case "maintenance":
                            loggerId = stoca.STO_LOGGER;
                            fileId = stoca.STO_FILE;
                            ts = stoca.STO_DATE;
                            if (loggerId != null && fileId != null)
                            {
                                //add loggerid and fileid to list, if not already present
                                AddLoggerFileItem((int)loggerId, (int)fileId, lfList);
                            }
                            break;


                    }

                    var stx = _context.GRP_STO.Where(st=>st.DataStatus == 0 && st.SiteId == siteId && st.STO_GA_MODEL == stoca.STO_GA_MODEL && st.STO_GA_SN == stoca.STO_GA_SN
                                                    && st.STO_DATE.CompareTo(timestamp)<=0).OrderByDescending(st=>st.STO_DATE).FirstOrDefault();
                    int? profId = stx.STO_GA_PROF_ID;
                    stx = _context.GRP_STO.Where(st => st.DataStatus == 0 && st.SiteId == siteId && !String.IsNullOrEmpty( st.STO_CONFIG) && st.STO_PROF_ID == profId
                                                    && st.STO_DATE.CompareTo(timestamp) <= 0).OrderBy(st => st.STO_DATE).FirstOrDefault();
                    stoConfig = stx.STO_CONFIG;
                    if (String.Compare(stoConfig, "sequential", true) == 0 || String.Compare(stoConfig, "simultaneous", true) == 0)
                    {
                        isSeqOrSim = true;
                    }

                    if (isSeqOrSim) break;
                }
            }
        }

        private void GetBMExpectedFiles(int siteId, string timestamp, List<LoggerFile> lfList)
        {
            var bmInstList = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == siteId).Distinct().OrderBy(bm => bm.BM_MODEL).ThenBy(bm => bm.BM_SN).ToList();
            if (bmInstList.Count > 0)
            {
                foreach(var bmInst in bmInstList)
                {
                    var innerBmList = _context.BmOperationsByDate.Where(bm=>bm.SiteId==siteId && bm.BM_MODEL== bmInst.BM_MODEL && bm.BM_SN == bmInst.BM_SN 
                                    && bm.BM_DATE.CompareTo(timestamp)<=0 &&
                                    (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "maintenance" || bm.BM_TYPE.ToLower() == "removal" || bm.BM_TYPE.ToLower() == "variable map"))
                        .OrderByDescending(bm => bm.BM_DATE).ThenBy(bm => bm.BM_MODEL).ThenBy(bm => bm.BM_SN).ToList();
                    bool go = true;
                    string preModel = "", preSn = "";
                    foreach (var inner in innerBmList)
                    {
                        string lastOp = inner.BM_TYPE.ToLower();
                        int? loggerId;
                        int? fileId;
                        string ts = "";
                        switch (lastOp)
                        {
                            case "removal":
                                go = false;
                                break;
                            case "installation":
                            case "maintenance":
                            case "variable map":
                                preModel = bmInst.BM_MODEL;
                                preSn = bmInst.BM_SN;
                                loggerId = inner.BM_LOGGER;
                                fileId = inner.BM_FILE;

                                ts = inner.BM_DATE;
                                //add loggerid and fileid to list, if not already present
                                if (loggerId!=null && fileId!=null)
                                {
                                    //add loggerid and fileid to list, if not already present
                                    AddLoggerFileItem((int)loggerId, (int)fileId, lfList);
                                    go = false;
                                }
                                break;
                        }
                        if (!go) break;
                    }
                }
            }
        }

        private void GetEcExpectedFiles(int siteId, string timestamp, List<LoggerFile> temp)
        {
            bool isPump = false;
            var ecInstList = _context.GRP_EC.Where(ec => ec.DataStatus == 0 && ec.SiteId == siteId).Distinct().OrderBy(ec => ec.EC_MODEL).ThenBy(ec => ec.EC_SN).ToList();
            if (ecInstList.Count > 0)
            {
                foreach(var ecInst in ecInstList)
                {
                    isPump = (String.Compare(ecInst.EC_MODEL, "GA_PUMP-other", true) == 0);
                    var xcInstList = _context.EcOperationsByDate.Where(ec => ec.SiteId == siteId && ec.EC_MODEL == ecInst.EC_MODEL && ec.EC_SN == ecInst.EC_SN &&
                                    ec.EC_DATE.CompareTo(timestamp) <= 0 &&
                                    (ec.EC_TYPE.ToLower() == "installation" || ec.EC_TYPE.ToLower() == "maintenance" || ec.EC_TYPE.ToLower() == "removal"))
                                                                .OrderByDescending(ec => ec.EC_DATE).ThenBy(ec => ec.EC_MODEL).ThenBy(ec => ec.EC_SN).ToList();
                    if (xcInstList.Count > 0)
                    {
                        bool go = true;
                        foreach(var inner in xcInstList)
                        {
                            string lastOp = inner.EC_TYPE.ToLower();
                            int? _logId;
                            int? _fileId;
                            string loggerId = "", fileId = "", ts = "";
                            switch (lastOp)
                            {
                                case "removal":
                                    go = false;
                                    break;
                                case "installation":
                                case "maintenance":
                                    _logId = inner.EC_LOGGER;
                                    _fileId = inner.EC_FILE;
                                    //Added TS
                                    ts = inner.EC_DATE;
                                    //add loggerid and fileid to list, if not already present
                                    if (_logId!=null && _fileId!=null)
                                    {
                                        //add loggerid and fileid to list, if not already present
                                        AddLoggerFileItem((int)_logId, (int)_fileId,  temp);
                                        go = false;
                                    }
                                    break;
                                    /* case "maintenance":
                                         loggerId = innerRd["LOGGER"].ToString();
                                         fileId = innerRd["FILEID"].ToString();
                                         ts = innerRd["LastUpdateDate"].ToString();
                                         if (!String.IsNullOrEmpty(loggerId) && !String.IsNullOrEmpty(fileId))
                                         {
                                             //add loggerid and fileid to list, if not already present
                                             //AddLoggerFileItem(loggerId, fileId, temp);
                                             AddLoggerFileItem(loggerId, fileId, temp);
                                             go = false;
                                         }
                                         break;*/
                            }
                            if (!go) break;
                        }
                    }
                }
            }
        }

        private void AddLoggerFileItem(int loggerId, int fileId, List<LoggerFile> lf)
        {
            if (lf.Count == 0)
            {
                lf.Add(new LoggerFile(loggerId, fileId));
            }
            else
            {
                foreach (var item in lf)
                {
                    if ((item.LoggerId == loggerId) && (item.FileId == fileId))
                    {
                        return;
                    }
                }
                lf.Add(new LoggerFile(loggerId, fileId));
            }
        }
    }
}
