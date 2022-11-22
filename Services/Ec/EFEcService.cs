using Icos5.core6.DbManager;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Icos5.core6.Services.Ec
{
    public class EFEcService : IEcServices
    {
        private readonly IcosDbContext _context;
        public EFEcService(IcosDbContext context)
        {
            _context = context;
        }
        public string GetECOperation(int siteId, int logger_id, int file_id, string timestamp)
        {
            string[] msgs = { "0", "0", "0", "0", "0", "0" };
            string[] ops = { "Disturbance", "Field calibration", "Field calibration check", "Field cleaning", "Maintenance", "Parts substitution" };
            StringBuilder sb = new StringBuilder();
            DateTime _dtTimeStamp = new DateTime(int.Parse(timestamp.Substring(0, 4)),
                                                int.Parse(timestamp.Substring(4, 2)),
                                                int.Parse(timestamp.Substring(6, 2)),
                                                int.Parse(timestamp.Substring(8, 2)),
                                                int.Parse(timestamp.Substring(10, 2)), 0);

            DateTime _start = _dtTimeStamp.AddMinutes(-35);
            DateTime _end = _dtTimeStamp.AddMinutes(5);

            string _startISO = _start.ToString("yyyyMMddHHmm");
            string _endISO = _end.ToString("yyyyMMddHHmm");
            bool isDist = false;

            var ecOps = _context.EcOperationsByDate.Where(ec => ec.SiteId == siteId && ec.EC_LOGGER == logger_id && ec.EC_FILE == file_id && ec.EC_DATE.CompareTo(_startISO) < 0 &&
                                                          (ec.EC_TYPE.ToLower()== "installation"|| ec.EC_TYPE.ToLower() == "maintenance") )
                                                    .OrderByDescending(ec => ec.EC_DATE).ToList();
            foreach (var ecOp in ecOps)
            {
                if (!isDist)
                {
                    string ecInst = ecOp.EC_MODEL;
                    string ecSn = ecOp.EC_SN;
                    string installDate = ecOp.EC_DATE;
                    string _ecStartDate = "", _ecEndDate = "";

                    var xOps = _context.EcOperationsByDate.Where(ec => ec.SiteId == siteId && ec.EC_MODEL == ecInst && ec.EC_SN == ecSn &&
                               (ec.EC_TYPE.ToLower() == "disturbance" || ec.EC_TYPE.ToLower() == "maintenance" || ec.EC_TYPE.ToLower() == "parts substitution" || ec.EC_TYPE.ToLower().IndexOf("field") >= 0)
                               && ((ec.EC_DATE.CompareTo(_startISO) >= 0 && ec.EC_DATE_START.CompareTo(_endISO) < 0) ||
                                    (ec.EC_DATE_END.CompareTo(_startISO) >= 0 && ec.EC_DATE_END.CompareTo(_endISO) < 0) ||
                                    (ec.EC_DATE.CompareTo(_startISO) < 0 && ec.EC_DATE_END.CompareTo(_endISO) > 0)))
                                    .OrderByDescending(ec => ec.EC_DATE).ToList();
                    if (xOps.Any())
                    {
                        foreach(var xOp in xOps)
                        {
                            string _type = xOp.EC_TYPE;
                            _ecStartDate = xOp.EC_DATE;
                            _ecEndDate = xOp.EC_DATE_END;
                            if (String.IsNullOrEmpty(_ecEndDate))
                            {
                                //add end_date checking resolution
                                //if res -> minutes, return (_ecStartDate - 5minutes, _ecStartDate + 5minutes) 202102041221 -> (202102041216 - 202102041226)
                                //if res -> hours, return (_ecStartDate, _ecStartDate + 59) 2021020412 -> (2021020412 - 2021020413)
                                //if res -> day, return (_ecStartDate0000, _ecStartDate2359) 20210204 -> (202102040000 - 202102042359)
                                _ecEndDate = _ecStartDate.Substring(0, 8);// +"2359";
                            }
                            else if (_ecEndDate.Length == 8)
                            {
                                // _bmEndDate += "2359";
                            }
                            if (String.Compare(_startISO, _ecStartDate) > 0) _ecStartDate = _startISO;
                            if (String.Compare(_endISO, _ecEndDate) < 0) _ecEndDate = _endISO;
                            sb.Append(_ecStartDate + "," + _ecEndDate + Environment.NewLine);
                            //isDist = true;
                            int index = ops.ToList().FindIndex(item => String.Compare(item, _type, true) == 0);
                            if (String.Compare(msgs[index], "0", true) == 0)
                            {
                                msgs[index] = _ecStartDate + "," + _ecEndDate + "," + ecInst.Substring(0, 2);
                            }
                            else
                            {
                                if (msgs[index].IndexOf(ecInst.Substring(0, 2)) < 0)
                                {
                                    msgs[index] += "," + ecInst.Substring(0, 2);
                                }
                            }
                        }
                    }
                }
            }
           
            return string.Join("\r\n", msgs);
        }
        public string GetSaGillAlign(int siteId, int logger_id, int file_id, string timestamp)
        {
            string saGillAlign = "";
            var ecInst = _context.GRP_EC.Where(ec => ec.DataStatus == 0 && ec.SiteId == siteId && ec.EC_LOGGER == logger_id && ec.EC_FILE == file_id &&
                                                ec.EC_MODEL.StartsWith("SA_")).OrderBy(ec => ec.EC_MODEL).ThenBy(ec => ec.EC_SN).ToList();
            foreach(var ec in ecInst)
            {
                string saModel = ec.EC_MODEL;
                string saSn = ec.EC_SN;
                var xEc = _context.EcOperationsByDate.Where(x => x.SiteId == siteId && x.EC_MODEL == saModel && x.EC_SN == saSn &&
                                                        (x.EC_TYPE.ToLower() == "installation" || x.EC_TYPE.ToLower() == "removal" || x.EC_TYPE.ToLower() == "maintenance")
                                                        && ec.EC_DATE.CompareTo(timestamp) < 0).OrderBy(x => x.EC_MODEL).ThenBy(x => x.EC_SN).ThenByDescending(x => x.EC_DATE).ToList();
                foreach(var xx in xEc)
                {
                    if (String.Compare(xx.EC_TYPE, "removal", true) == 0)
                    {
                        break;
                    }
                    else
                    {
                        saGillAlign = xx.EC_TYPE;
                    }
                }
                if (!String.IsNullOrEmpty(saGillAlign))
                {
                    break;
                }
            }
            
            return saGillAlign;
        }
        public string GetGaFlowRate(int siteId, int logger_id, int file_id, string timestamp)
        {
            string gaFlowRate = "";
            var ecInst = _context.GRP_EC.Where(ec => ec.DataStatus == 0 && ec.SiteId == siteId && ec.EC_LOGGER == logger_id && ec.EC_FILE == file_id &&
                                                ec.EC_MODEL.StartsWith("GA_") && ec.EC_DATE.CompareTo(timestamp)<0 &&
                                                (ec.EC_TYPE.ToLower() == "installation" || ec.EC_TYPE.ToLower() == "maintenance")).OrderBy(ec => ec.EC_MODEL).ThenBy(ec => ec.EC_SN).ThenByDescending(ec=>ec.EC_DATE).ToList();

            if (ecInst != null)
            {
                foreach(var xc in ecInst)
                {
                    var lastOp = _context.GRP_EC.Where(ec => ec.DataStatus == 0 && ec.SiteId == siteId && ec.EC_MODEL == xc.EC_MODEL && ec.EC_SN == xc.EC_SN &&
                                                    (ec.EC_TYPE.ToLower() == "installation" || ec.EC_TYPE.ToLower() == "removal") && ec.EC_DATE.CompareTo(timestamp) < 0)
                                                .OrderBy(ec => ec.EC_MODEL).ThenBy(ec => ec.EC_SN).ThenByDescending(ec => ec.EC_DATE).FirstOrDefault();
                    if (String.Compare(lastOp.EC_TYPE, "removal", true) == 0) continue;
                    lastOp = _context.GRP_EC.Where(ec => ec.DataStatus == 0 && ec.SiteId == siteId && ec.EC_MODEL == xc.EC_MODEL && ec.EC_SN == xc.EC_SN &&
                                                   (ec.EC_TYPE.ToLower() == "installation" || ec.EC_TYPE.ToLower() == "maintenance") && ec.EC_DATE.CompareTo(timestamp) < 0
                                                   && ec.EC_GA_FLOW_RATE!=null).OrderBy(ec => ec.EC_MODEL).ThenBy(ec => ec.EC_SN).ThenByDescending(ec => ec.EC_DATE).FirstOrDefault();
                    if (lastOp != null)
                    {
                        gaFlowRate = lastOp.EC_GA_FLOW_RATE.ToString();
                    }
                    else
                    {
                        if (xc.EC_MODEL.IndexOf("PUMP", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var ecStoCa=_context.GRP_EC.Where(ec => ec.DataStatus == 0 && ec.SiteId == siteId && ec.EC_HEIGHT == xc.EC_HEIGHT &&
                                                                ec.EC_NORTHWARD_DIST == xc.EC_NORTHWARD_DIST && ec.EC_EASTWARD_DIST == xc.EC_EASTWARD_DIST &&
                                                                ec.EC_GA_FLOW_RATE != null && ec.EC_DATE.CompareTo(timestamp) < 0)
                                                        .OrderBy(ec => ec.EC_MODEL).ThenBy(ec => ec.EC_SN).ThenByDescending(ec => ec.EC_DATE).FirstOrDefault();
                            if (ecStoCa != null)
                            {
                                gaFlowRate = ecStoCa.EC_GA_FLOW_RATE.ToString();
                            }
                            else
                            {
                                gaFlowRate = "";
                            }

                        }
                    }
                }
            }
            return gaFlowRate;
        }
        public string GetFlag(int siteId, string type, int logger_id, int file_id, string timestamp)
        {
            string flag = "";
            bool isPump = false;
            switch (type.ToLower())
            {
                case "ec":
                    var ecInst = _context.GRP_EC.Where(ec => ec.DataStatus == 0 && ec.SiteId == siteId && ec.EC_LOGGER == logger_id && ec.EC_FILE == file_id)
                                                .OrderBy(ec => ec.EC_MODEL).ThenBy(ec => ec.EC_SN).ToList();
                    if(ecInst.Count > 0)
                    {
                        foreach(var xc in ecInst)
                        {
                            isPump = isPump || (String.Compare(xc.EC_MODEL, "GA_PUMP-other", true) == 0);
                            
                            var innerEcList = _context.EcOperationsByDate.Where(ec=>ec.SiteId == siteId && ec.EC_MODEL == xc.EC_MODEL && ec.EC_SN == xc.EC_SN &&
                                                        (ec.EC_TYPE.ToLower() == "installation" || ec.EC_TYPE.ToLower() == "removal" || ec.EC_TYPE.ToLower() == "maintenance")
                                                         && ec.EC_DATE.CompareTo(timestamp)<=0).OrderByDescending(ec=>ec.EC_DATE).ThenBy(ec=>ec.EC_MODEL).ThenBy(ec=>ec.EC_SN);
                            bool go = true;
                            bool isRemoved = false;
                            foreach(var inner in innerEcList)
                            {
                                string lastOp = inner.EC_TYPE.ToLower();
                                switch (lastOp)
                                {
                                    case "removal":
                                        go = false;
                                        isRemoved = true;
                                        break;
                                    case "installation":
                                    case "maintenance":
                                        flag = (isPump) ? "p" : "m";
                                        go = false;
                                        break;
                                    default:
                                        break;
                                }
                                if (!go) break;
                            }

                            if (!isPump && !isRemoved)
                            {
                                innerEcList = _context.EcOperationsByDate.Where(ec => ec.SiteId == siteId && ec.EC_MODEL == xc.EC_MODEL && ec.EC_SN == xc.EC_SN &&
                                                        (ec.EC_TYPE.ToLower() == "installation" || ec.EC_TYPE.ToLower() == "removal" || ec.EC_TYPE.ToLower() == "maintenance")
                                                         && ec.EC_DATE.CompareTo(timestamp) <= 0).OrderByDescending(ec => ec.EC_DATE).ThenBy(ec => ec.EC_MODEL).ThenBy(ec => ec.EC_SN);

                                go = true;
                                foreach (var inner in innerEcList)
                                {
                                    string lastOp = inner.EC_TYPE.ToLower();
                                    switch (lastOp)
                                    {
                                        case "removal":
                                            go = false;
                                            break;
                                        case "installation":
                                        case "maintenance":
                                            flag = "mp";
                                            go = false;
                                            break;
                                        default:
                                            break;
                                    }
                                    if (!go) break;
                                }
                            }
                        }
                    }

                    break;
                case "bm":
                case "st":
                    break;
            }
            return flag;
        }
    }
}
