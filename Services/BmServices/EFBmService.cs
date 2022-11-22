using Icos5.core6.DbManager;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Icos5.core6.Services.BmServices
{
    public class EFBmService : IBmService
    {
        private readonly IcosDbContext _context;
        public EFBmService(IcosDbContext context)
        {
            _context = context;
        }

        //DIEGO:: may be querie must search only the TOP 1 record???
        public string GetBMOperation(int siteId, int logger_id, int file_id, string variable, string timestamp)
        {
            string[] msgs = { "0", "0", "0", "0", "0", "0" };
            string[] ops = { "Disturbance", "Field calibration", "Field calibration check", "Field cleaning", "Maintenance", "Parts substitution" };
            StringBuilder sb = new StringBuilder();

            DateTime _dtTimeStamp = new DateTime(int.Parse(timestamp.Substring(0, 4)),
                                        int.Parse(timestamp.Substring(4, 2)),
                                        int.Parse(timestamp.Substring(6, 2)),
                                        0, 0, 0);

            DateTime _start = _dtTimeStamp.AddMinutes(0);
            DateTime _end = _dtTimeStamp.AddDays(1);

            string _startISO = _start.ToString("yyyyMMddHHmm");
            string _endISO = _end.ToString("yyyyMMddHHmm");
            bool isDist = false;

            var bmOps = _context.BmOperationsByDate.Where(bm => bm.SiteId == siteId && (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "variable map") &&
                                                            bm.BM_VARIABLE_H_V_R == variable && bm.BM_LOGGER == logger_id && bm.BM_FILE == file_id && bm.BM_DATE.CompareTo(timestamp) < 0)
                                                    .OrderByDescending(bm => bm.BM_DATE).ToList();

            foreach(var bmOp in bmOps)
            {
                string _bmStartDate = "", _bmEndDate = "";
                var innerBmOps = _context.BmOperationsByDate.Where(bm => bm.SiteId == siteId && bm.BM_MODEL == bmOp.BM_MODEL && bm.BM_SN == bmOp.BM_SN &&
                                                                    (bm.BM_TYPE.ToLower() == "disturbance" || bm.BM_TYPE.ToLower() == "maintenance" || bm.BM_TYPE.ToLower() == "parts substitution") &&
                                                                    ((bm.BM_DATE_START.Substring(0,8).CompareTo(timestamp)<=0 && bm.BM_DATE_END.Substring(0, 8).CompareTo(timestamp) >= 0) || bm.BM_DATE_START.Substring(0, 8).CompareTo(timestamp) == 0))
                                                            .OrderByDescending(bm => bm.BM_DATE_START).ToList();
                if (innerBmOps.Count() > 0)
                {
                    foreach(var innerBmOp in innerBmOps)
                    {
                        string _type = innerBmOp.BM_TYPE;
                        _bmStartDate = innerBmOp.BM_DATE_START;
                        _bmEndDate = innerBmOp.BM_DATE_END;
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
                        sb.Append(_bmStartDate + "-" + _bmEndDate + Environment.NewLine);
                        int index = ops.ToList().FindIndex(item => String.Compare(item, _type, true) == 0);
                        msgs[index] = _bmStartDate + ((String.IsNullOrEmpty(_bmEndDate)) ? "" : ("-" + _bmEndDate));
                    }
                }
            }
           
            
            return string.Join("\r\n", msgs);
        }
        public IEnumerable<string> BMVarMapped(int siteId, int logger_id, int file_id, string variable)
        {
            List<string> bmv = new List<string>();
            string type = "BM";
            var bmList = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == siteId && bm.BM_FILE == file_id && bm.BM_LOGGER == logger_id &&
                                                bm.BM_VARIABLE_H_V_R.ToLower().IndexOf(variable.ToLower()) >= 0).ToList().Count();
            /*  switch (type.ToLower())
              {
                  case "bm":

                      cmd.CommandText = @" SELECT COUNT(qual3) AS CNT FROM DataStorageLive, ICOS_site WHERE site_code = '" + site + "' AND id_icos_site = siteID AND groupID = 2005 "
                                        + "   AND qual11 = '" + logger_id + "' AND qual12 = '" + file_id + "' AND qual3 IS NOT NULL AND qual3 LIKE '%" + variable + "%'";
                      break;
                  case "st":


                      break;
                  case "ec":

                      break;
              }*/
            bmv.Add(bmList.ToString());
            return bmv;
        }
    }
}
