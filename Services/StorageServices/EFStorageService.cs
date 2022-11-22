using Icos5.core6.DbManager;
using Icos5.core6.Models.Sensors;
using IcosClassLibrary.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Text;

namespace Icos5.core6.Services.StorageServices
{
    public class EFStorageService : IStorageService
    {
        private readonly IcosDbContext _context;
        private string[] ops { get;  set; } = { "Disturbance", "Field calibration", "Field calibration check", "Field cleaning", "Maintenance", "Parts substitution" };
        private string[] msgs { get; set; } = { "0", "0", "0", "0", "0", "0" };

        public EFStorageService(IcosDbContext context)
        {
            _context = context;
        }
        public IEnumerable<string> GetStorageConfigProfLevel(int siteId, string date)
        {
            List<string> stoList = new List<string>();
            string results = "";
            var stProfiles = _context.GRP_STO.Where(st => st.DataStatus == 0 && st.SiteId == siteId && st.STO_DATE.CompareTo(date) <= 0
                                                    && !String.IsNullOrEmpty(st.STO_CONFIG) && st.STO_TYPE.ToLower() == "level installation").ToList();
            if (stProfiles != null)
            {
                bool go = true;
                int nLevel = 0;
                foreach (var stProfile in stProfiles)
                {
                    if (go)
                    {
                        switch (stProfile.STO_CONFIG.ToLower())
                        {
                            case "simultaneous":
                                results = stProfile.STO_CONFIG + ",0";
                                go = false;
                                break;
                            case "sequential":
                            case "separate":
                                var stResult = _context.GRP_STO.Where(st => st.DataStatus == 0 && st.SiteId == siteId && st.STO_PROF_ID == stProfile.STO_PROF_ID &&
                                                                      st.STO_PROF_LEVEL == stProfile.STO_PROF_LEVEL && st.STO_DATE.CompareTo(date) <= 0
                                                                      && (st.STO_TYPE.ToLower() == "level installation" || st.STO_TYPE.ToLower() == "level removal"))
                                                                .OrderByDescending(st => st.STO_DATE).FirstOrDefault();
                                if(stResult != null)
                                {
                                    if (String.Compare(stResult.STO_TYPE, "level installation") == 0)
                                    {
                                        ++nLevel;
                                        results = stProfile.STO_CONFIG + "," + nLevel;
                                    }
                                }
                                
                                break;
                        }
                    }
                }
            }

            stoList.Add(results);
            return stoList;
        }

        public string GetStorageOpsByLevel(int siteId, int logger_id, int file_id, int stoProfLevel, string timestamp)
        {
            DateTime _dtTimeStamp = new DateTime(int.Parse(timestamp.Substring(0, 4)),
                                            int.Parse(timestamp.Substring(4, 2)),
                                            int.Parse(timestamp.Substring(6, 2)),
                                            0, 0, 0);

            DateTime _start = _dtTimeStamp.AddMinutes(0);
            DateTime _end = _dtTimeStamp.AddDays(1);

            string _startISO = _start.ToString("yyyyMMddHHmm");
            string _endISO = _end.ToString("yyyyMMddHHmm");

            StringBuilder sb = new StringBuilder();
            bool isDist = false;
            SqlDataReader _rd = null;

            var stList = _context.StoOperationsByDate.Where(st => st.SiteId == siteId && st.STO_LOGGER == logger_id && st.STO_FILE == file_id &&
                                                            (st.STO_TYPE.ToLower() == "ga installation" || st.STO_TYPE.ToLower() == "variable map") && st.STO_DATE.CompareTo(timestamp) < 0)
                                                     .OrderByDescending(st => st.STO_DATE).ToList();
            foreach(var stl in stList)
            {
                string stInst = stl.STO_GA_MODEL;
                string stSn = stl.STO_GA_SN;
                string stDate = stl.STO_DATE;
                string _bmStartDate = "", _bmEndDate = "";
                int? stoGaProfId = stl.STO_GA_PROF_ID;

                bool isGa = true;// IsGaInstalled(bmInst, bmSn, _siteId, timestamp, conn);
                if (!isGa) continue;

                var inStlist = _context.GRP_STO.Where(st => st.DataStatus == 0 && st.SiteId == siteId && (st.STO_TYPE.ToLower() == "Level installation" || st.STO_TYPE.ToLower() == "Level removal")
                                                     && st.STO_DATE.CompareTo(timestamp) < 0).ToList();
                foreach(var _sl in inStlist)
                {
                    string _type = _sl.STO_TYPE;
                    if (String.Compare(_type, "level removal", true) == 0)
                    {
                        continue;
                    }

                    var stXX = _context.StoOperationsByDate.Where(st=>st.SiteId == siteId && st.STO_GA_MODEL == stInst && st.STO_GA_SN==stSn &&
                              (st.STO_TYPE.ToLower() == "disturbance" || st.STO_TYPE.ToLower() == "maintenance" || st.STO_TYPE.ToLower() == "parts substitution" || st.STO_TYPE.ToLower().IndexOf("field") >= 0) && 
                              st.STO_GA_PROF_ID == stoGaProfId && st.STO_DATE.CompareTo(timestamp) < 0)
                                                     .OrderByDescending(st => st.STO_DATE).ToList();
                    foreach(var x in stXX)
                    {
                        _bmStartDate = x.STO_DATE_START;
                        _bmEndDate = x.STO_DATE_END;
                        string innertype = x.STO_TYPE;
                        if (String.IsNullOrEmpty(_bmEndDate))
                        {
                            _bmEndDate = _bmStartDate.Substring(0, 8);// +"2359";
                        }
                        else if (_bmEndDate.Length == 8)
                        {

                        }

                        if (String.Compare(_startISO, _bmStartDate) > 0) _bmStartDate = _startISO;
                        if (String.Compare(_endISO, _bmEndDate) < 0) _bmEndDate = _endISO;
                        sb.Append(_bmStartDate + "," + _bmEndDate + Environment.NewLine);
                        isDist = true;
                        int index = ops.ToList().FindIndex(item => String.Compare(item, innertype, true) == 0);
                        msgs[index] = /*_type + "," +*/ _bmStartDate + "," + _bmEndDate;
                    }

                }

                if (isDist) break;
            }

            return string.Join("\r\n", msgs);
        }
        public string GetStorageOps(int siteId, int logger_id, int file_id, string variable, string timestamp)
        {
            DateTime _dtTimeStamp = new DateTime(int.Parse(timestamp.Substring(0, 4)),
                                            int.Parse(timestamp.Substring(4, 2)),
                                            int.Parse(timestamp.Substring(6, 2)),
                                            0, 0, 0);

            DateTime _start = _dtTimeStamp.AddMinutes(0);
            DateTime _end = _dtTimeStamp.AddDays(1);

            string _startISO = _start.ToString("yyyyMMddHHmm");
            string _endISO = _end.ToString("yyyyMMddHHmm");

            List<Instrument> instList = new List<Instrument>();
            StringBuilder sb = new StringBuilder();
            bool isDist = false;
            
            var stList = _context.StoOperationsByDate.Where(st=>st.SiteId==siteId && st.STO_GA_VARIABLE==variable && st.STO_LOGGER==logger_id && st.STO_FILE == file_id &&
                                                            (st.STO_TYPE.ToLower()== "ga installation" || st.STO_TYPE.ToLower() == "variable map")&&st.STO_DATE.CompareTo(timestamp)<0)
                                                     .OrderByDescending(st=>st.STO_DATE).ToList();      


            foreach (var stx in stList)
            {
                if (!isDist)
                {
                    string bmInst = stx.STO_GA_MODEL;
                    string bmSn = stx.STO_GA_SN;
                    if (instList.Any(item => item.Model == bmInst && item.SerialNumber == bmSn)) continue;
                    string bmDate = stx.STO_DATE;
                    string bmMaxDate = "";// XDATE
                    string _bmStartDate = "", _bmEndDate = "";
                    var inSto = _context.StoOperationsByDate.Where(st => st.SiteId == siteId && st.STO_GA_MODEL == stx.STO_GA_MODEL && st.STO_GA_SN == stx.STO_GA_SN &&
                                                                   ((st.STO_DATE_START.Substring(0, 8).CompareTo(timestamp) <= 0 && st.STO_DATE_END.Substring(0, 8).CompareTo(timestamp) >= 0)
                                                                    || st.STO_DATE_START.Substring(0, 8).CompareTo(timestamp) == 0) &&
                                                            (st.STO_TYPE.ToLower() == "disturbance" || st.STO_TYPE.ToLower() == "maintenance" || st.STO_TYPE.ToLower() == "parts substitution" || st.STO_TYPE.ToLower().IndexOf("field") >= 0) &&
                                                            (st.STO_GA_VARIABLE.CompareTo(variable) == 0 || String.IsNullOrEmpty(st.STO_GA_VARIABLE))).OrderBy(st => st.STO_DATE_START).ToList();
                    foreach(var x in inSto)
                    {
                        string _type = x.STO_TYPE;
                        _bmStartDate = x.STO_DATE_START;
                        _bmEndDate = x.STO_DATE_END;
                        if (String.IsNullOrEmpty(_bmEndDate))
                        {
                            _bmEndDate = _bmStartDate.Substring(0, 8);// +"2359";
                        }
                        else if (_bmEndDate.Length == 8)
                        {
                            // _bmEndDate += "2359";
                        }

                        if (String.Compare(_startISO, _bmStartDate) > 0) _bmStartDate = _startISO;
                        if (String.Compare(_endISO, _bmEndDate) < 0) _bmEndDate = _endISO;

                        isDist = true;
                        int index = ops.ToList().FindIndex(item => String.Compare(item, _type, true) == 0);
                        msgs[index] = _bmStartDate + "," + _bmEndDate;
                    }
                }
            }
           
            return string.Join("\r\n", msgs);
        }

        public string GetOpenCloseSTModel(int siteId, int logger_id, int file_id, string variable, string timestamp)
        {
            string msg = "";
            DateTime _dtTimeStamp = new DateTime(int.Parse(timestamp.Substring(0, 4)),
                                            int.Parse(timestamp.Substring(4, 2)),
                                            int.Parse(timestamp.Substring(6, 2)),
                                            0, 0, 0);

            DateTime _start = _dtTimeStamp.AddMinutes(0);
            DateTime _end = _dtTimeStamp.AddDays(1);

            string _startISO = _start.ToString("yyyyMMddHHmm");
            string _endISO = _end.ToString("yyyyMMddHHmm");

            string openClose = "";
            var storageList = _context.StoOperationsByDate.Where(st => st.SiteId == siteId && st.STO_GA_VARIABLE == variable && st.STO_LOGGER == logger_id && st.STO_FILE == file_id &&
                                                            (st.STO_TYPE.ToLower() == "ga installation" || st.STO_TYPE.ToLower() == "variable map") && st.STO_DATE.CompareTo(timestamp) < 0)
                                                           .OrderByDescending(st => st.STO_DATE).ToList();
            //insert TOP 1 CLAUSE????

            if (storageList != null)
            {
                foreach(var storage in storageList)
                {
                    string stInst = storage.STO_GA_MODEL;
                    string stSn = storage.STO_GA_SN;
                    string stDate = storage.STO_DATE;
                    string stMaxDate = stDate;

                    var stoInst = _context.GRP_STO.Where(xs=>xs.DataStatus==0 && xs.SiteId==siteId && xs.STO_GA_MODEL == stInst && xs.STO_GA_SN == stSn &&
                                                        (xs.STO_TYPE.ToLower() == "ga installation" || xs.STO_TYPE.ToLower() == "ga removal" || xs.STO_TYPE.ToLower() == "variable map") 
                                                        && xs.STO_DATE.CompareTo(timestamp)<=0).ToList();
                    foreach(var inst in stoInst)
                    {
                        if (String.Compare(inst.STO_TYPE, "ga removal", true) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            openClose = stInst;
                            break;
                        }
                    }
                }
                msg = (openClose.IndexOf("GA_OP") >= 0) ? "OPEN" : "CLOSE";
            }

            return msg;
        }
    }
}
