using Icos5.core6.DbManager;
using Icos5.core6.Models.General;
using Icos5.core6.Models.StringOptions;
using Icos5.core6.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Icos5.core6.Controllers.Utility
{
    public class UtilityController : Controller
    {
        private IcosDbContext _context;
        private IUtilityServices _service;
        private string _siteCode = "";
        private string pathToFiles = "";
        public UtilityController(IcosDbContext context, IUtilityServices service)
        {
            _context = context;
            _service = service;
        }
        public IActionResult GetOutofRange(string name)
        {
            List<OutOfRange> oorList = new List<OutOfRange>();
            var oorObj=_context.OutOfRange.Where(x=> x.Name == name).ToList();
            
            foreach(var _oor in oorObj)
            {
                OutOfRange oor = new OutOfRange();
                oor.Min = _oor.Min;
                oor.Max = _oor.Max;
                if (_oor.Step != null)
                {
                    oor.Step = -9999;
                }
                else
                {
                    oor.Step = _oor.Step;
                }
                oorList.Add(oor);
            }
            MyStringify<OutOfRange> stfy = new MyStringify<OutOfRange>(oorList);
            stfy.MakeString();
            return View(stfy);
        }

        public IActionResult GetOutofRangeAll()
        {
            var oorList = _service.GetoutOfRange().ToList();
            MyStringify<OutOfRange> stfy = new MyStringify<OutOfRange>(oorList);
            stfy.MakeString();
            return View(stfy);
        }

        public IActionResult GetSiteChars(string site, string timestamp)
        {
            string _lat = "", _lon = "", _utc = "-9999";
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0 || timestamp.Length != 8)
            {
                return View("Wrong site input or wrong dates format");
            }
            var location = _context.GRP_LOCATION.Where(ll => ll.DataStatus <= 2 && ll.SiteId == id && ll.LOCATION_DATE.Substring(0, 8).CompareTo(timestamp) <= 0)
                                                .OrderByDescending(ll => ll.LOCATION_DATE).OrderByDescending(ll => ll.Id).ToList();

            if(location!=null && location.Count > 0)
            {
                foreach (var loc in location)
                {
                    string _dd = "99990101";
                    int i = 0;
                    int _ds = loc.DataStatus;
                    string _date = loc.LOCATION_DATE;
                    if (String.Compare(_date, _dd) <= 0)
                    {
                        if (_ds == 0)
                        {
                            _lat = loc.LOCATION_LAT.ToString();
                            _lon = loc.LOCATION_LONG.ToString();
                            break;
                        }
                        if (i == 0)
                        {
                            _lat = loc.LOCATION_LAT.ToString();
                            _lon = loc.LOCATION_LONG.ToString();
                        }
                        ++i;
                    }
                }
            }
            else
            {
                var xloc = _context.GRP_LOCATION.Where(ll => ll.DataStatus == 0 && ll.SiteId == id)
                                                .OrderByDescending(ll => ll.LOCATION_DATE).OrderByDescending(ll => ll.Id).FirstOrDefault();
                _lat = xloc.LOCATION_LAT.ToString();
                _lon = xloc.LOCATION_LONG.ToString();
            }

            var utc = _context.GRP_UTC_OFFSET.Where(u => u.DataStatus <= 2 && u.SiteId == id && u.UTC_OFFSET_DATE_START.Substring(0, 8).CompareTo(timestamp) <= 0)
                                              .OrderByDescending(u => u.UTC_OFFSET_DATE_START).OrderByDescending(u => u.Id).ToList();
            if(utc!=null && utc.Count > 0)
            {
                string _dd = "99990101";
                int i = 0;
                foreach(var u in utc)
                {
                    int _ds = u.DataStatus;
                    string _date = u.UTC_OFFSET_DATE_START;
                    if (String.Compare(_date, _dd) <= 0)
                    {
                        if (_ds == 0)
                        {
                            _utc = u.UTC_OFFSET_DATE_START;
                            break;
                        }
                    }
                    if (i == 0)
                    {
                        _utc = u.UTC_OFFSET_DATE_START;
                    }
                    ++i;
                }
            }
            else
            {
                var obj = _context.GRP_UTC_OFFSET.Where(u => u.DataStatus == 0 && u.SiteId == id)
                                                 .OrderByDescending(u => u.UTC_OFFSET_DATE_START).OrderByDescending(u => u.Id).FirstOrDefault();
                if (obj != null)
                {
                    _utc = obj.UTC_OFFSET.ToString();
                }
            }
            string row = site + "," + _lat + "," + _lon + "," + _utc;
            MyStringify<string> stfy = new MyStringify<string>(new List<string> { row });
            stfy.MakeByString();
            return View(stfy);
        }

        public IActionResult GetWetland(string site)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
            var igbp = _context.GRP_IGBP.Where(u => u.DataStatus == 0 && u.SiteId == id).OrderByDescending(u=>u.Id).FirstOrDefault();
            string resp = _service.GetWetland(id);
            ViewBag.Resp = resp;
            return View();
        }

        public IActionResult IsIcosVar(string type, string shortname)
        {
            List<string> icv = _service.IsIcosVar(type, shortname).ToList();
            
            MyStringify<string> stfy = new MyStringify<string>(icv);
            stfy.MakeByStringNewLine();
            return View(stfy);
        }
    }
}
