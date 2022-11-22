using Icos5.core6.DbManager;
using Icos5.core6.Models.General;
using IcosClassLibrary.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;

namespace Icos5.core6.Services
{
    public class UtilityServices : IUtilityServices
    {
        private readonly IcosDbContext _context;

        public UtilityServices(IcosDbContext ctx)
        {
            _context = ctx;
        }
        public int GetSiteIdByCode(string site)
        {
            int id = 0;
            var st = _context.Sites.FirstOrDefault(s => s.SiteCode.ToLower() == site.ToLower());
            if (st != null)
            {
                id = st.Id;
            }
            return id;
        }

        public IEnumerable<string> IsIcosVar(string type, string shortname)
        {
            int cv_index = 0;
            if (type.ToLower() == "bm")
            {
                cv_index = 73;
            }
            else if (type.ToLower() == "st")
            {
                cv_index = 77;
            }
            List<string> icv = new List<string>();
            var xlist = _context.BADMList.Where(x=>x.cv_index==cv_index && shortname.ToLower() == x.shortname.ToLower()).Count();
            /*foreach (var x in xlist)
            {

            }*/
            icv.Add(xlist.ToString());
            return icv;
        }

        public IEnumerable<OutOfRange> GetoutOfRange()
        {
            var oor = _context.OutOfRange.OrderBy(oor => oor.Name);
            return oor;
        }

        public string GetSiteCoordinates(int siteId)
        {
            string coo = "";
            var location = _context.GRP_LOCATION.Where(l => l.SiteId == siteId && l.DataStatus == 0).FirstOrDefault();
            if (location != null)
            {
                coo = location.LOCATION_LAT + "," + location.LOCATION_LONG;
            }
            return coo;
        }
        public string GetWetland(int siteId)
        {
            string resp = "";
            var igbp = _context.GRP_IGBP.Where(u => u.DataStatus == 0 && u.SiteId == siteId).OrderByDescending(u => u.Id).FirstOrDefault();
            if (igbp != null)
            {
                resp = String.Compare(igbp.IGBP, "WET", true) == 0 ? "1" : "0";
            }
            return resp;
        }

        public string GetRealSiteCode(string site)
        {
            var sc = _context.Sites.FirstOrDefault(x => x.SiteCode.ToLower() == site.ToLower());
            if (sc != null)
            {
                return sc.SiteCode;
            }
            return "";
        }

        public string GetIcosLabellingDate(string site)
        {
            string labDate = "";
            var labSite = _context.ICOSLabellingDate.Where(st => st.Site.ToLower() == site.ToLower()).FirstOrDefault();
            if (labSite != null)
            {
                labDate=labSite.LabelDate;
            }
            return labDate;
        }

        public bool IsEcSensorInstalled(string model, string sn, int siteId)
        {
            bool res = false;
            var ecRes = _context.GRP_EC.Where(ec => ec.SiteId == siteId && ec.DataStatus == 0 && ec.EC_MODEL == model && ec.EC_SN == sn &&
                                            (ec.EC_TYPE.ToLower() == "installation" || ec.EC_TYPE.ToLower() == "removal"))
                                        .OrderByDescending(ec => ec.EC_DATE).FirstOrDefault();


            if (ecRes != null)
            {
                res = (String.Compare(ecRes.EC_TYPE, "installation", true) == 0);
            }
            return res;
        }
        public bool IsMeteoSensorInstalled(string model, string sn, int siteId)
        {
            bool res = false;
            var bmRes = _context.GRP_BM.Where(bm => bm.SiteId == siteId && bm.DataStatus == 0 && bm.BM_MODEL == model && bm.BM_SN == sn &&
                                            (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "removal"))
                                        .OrderByDescending(bm => bm.BM_DATE).FirstOrDefault();
            
            
            if (bmRes != null)
            {
                res = (String.Compare(bmRes.BM_TYPE, "installation", true) == 0);
            }
            return res;
        }
        public bool IsStoSensorInstalled(string model, string sn, int siteId)
        {
            bool res = false;
            var stRes = _context.GRP_STO.Where(st => st.SiteId == siteId && st.DataStatus == 0 && st.STO_GA_MODEL == model && st.STO_GA_SN == sn &&
                                            (st.STO_TYPE.ToLower() == "ga installation" || st.STO_TYPE.ToLower() == "ga removal"))
                                        .OrderByDescending(st => st.STO_DATE).FirstOrDefault();


            if (stRes != null)
            {
                res = (String.Compare(stRes.STO_TYPE, "ga installation", true) == 0);
            }
            return res;
        }

        public GRP_BM GetLastMeteoSensorOperationByVarAndDate(string model, string sn, string variable, string timestamp, int siteId)
        {
            GRP_BM bm = _context.GRP_BM.Where(bm => bm.SiteId == siteId && bm.DataStatus == 0 && bm.BM_MODEL == model && bm.BM_SN == sn &&
                                            (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "removal" || bm.BM_TYPE.ToLower() == "variable map") && 
                                            (bm.BM_VARIABLE_H_V_R == variable || bm.BM_VARIABLE_H_V_R == null) && bm.BM_DATE.CompareTo(timestamp)<=0)
                                        .OrderByDescending(bm => bm.BM_DATE).OrderByDescending(bm=>bm.Id).FirstOrDefault();
            
            return bm;
        }
        public GRP_STO GetLastStoSensorOperationByVarAndDate(string model, string sn, string variable, string timestamp, int siteId)
        {
            GRP_STO st = _context.GRP_STO.Where(st => st.SiteId == siteId && st.DataStatus == 0 && st.STO_GA_MODEL == model && st.STO_GA_SN == sn &&
                                            (st.STO_TYPE.ToLower() == "ga installation" || st.STO_TYPE.ToLower() == "ga removal" || st.STO_TYPE.ToLower() == "variable map") &&
                                            (st.STO_GA_VARIABLE == variable || st.STO_GA_VARIABLE == null) && st.STO_DATE.CompareTo(timestamp) <= 0)
                                        .OrderByDescending(st => st.STO_DATE).OrderByDescending(st => st.Id).FirstOrDefault();

            return st;
        }
    }
}
