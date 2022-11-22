using Icos5.core6.DbManager;
using Icos5.core6.Services.ProfileServices;
using Icos5.core6.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Icos5.core6.Services.VariablesServices;

namespace Icos5.core6.Controllers.Variables
{
    public class VariablesController : Controller
    {
        private IUtilityServices _service;
        private IVariableService _varService;

        public VariablesController(IUtilityServices service, IVariableService varService)
        {
            _service = service;
            _varService = varService;
        }

        public IActionResult GetVarsRenamedExt(string site, string timestamp)
        {
            string resp = _varService.GetVarsRenamedList(site, timestamp);
            ViewBag.Resp = resp;
            return View();
        }

        public IActionResult GetVarsInOutNew(string site)
        {
            var xs = _service.GetRealSiteCode(site);
            if (String.IsNullOrEmpty(xs))
            {
                return View();
            }

            string resp = _varService.GetAggregatedVarList(xs);
            ViewBag.Resp = resp;
            return View();
        }

        public ActionResult GetVarsRenamedInRange(string site, string startYear, string endYear)
        {
            string resp = "";
            int id = _service.GetSiteIdByCode(site);
            
            if (id <= 0)
            {
                ViewBag.Resp = "Wrong site input or wrong dates format";
                return View();
            }
            resp = _varService.GetVarsRenamedAggregatedInRange(site,startYear,endYear);
            ViewBag.Resp = resp;
            return View();
        }

        public IActionResult GetAlb(string site, string startDate)
        {
            string resp = "";
            int id = _service.GetSiteIdByCode(site);

            if (id <= 0)
            {
                ViewBag.Resp = "Wrong site input or wrong dates format";
                return View();
            }
            resp = _varService.GetAggregatedVarListByName(site, "ALB", startDate);
            ViewBag.Resp = resp;
            return View();
        }

        public IActionResult GetGTSWC(string site, string startDate)
        {
            string resp = "";
            int id = _service.GetSiteIdByCode(site);

            if (id <= 0)
            {
                ViewBag.Resp = "Wrong site input or wrong dates format";
                return View();
            }

            resp = _varService.GetAggregatedVarListByName(site, "SG", startDate);

            ViewBag.Resp = resp;
            return View();
        }

        public IActionResult GetNetrad(string site, string startDate)
        {
            string resp = "";
            int id = _service.GetSiteIdByCode(site);

            if (id <= 0)
            {
                ViewBag.Resp = "Wrong site input or wrong dates format";
                return View();
            }

            resp = _varService.GetAggregatedVarListByName(site, "NETRAD", startDate);
            
            ViewBag.Resp = resp;
            return View();
        }

        public ActionResult GetVpd(string site, string startDate)
        {
            string resp = "";
            int id = _service.GetSiteIdByCode(site);

            if (id <= 0)
            {
                ViewBag.Resp = "Wrong site input or wrong dates format";
                return View();
            }

            resp = _varService.GetAggregatedVarListByName(site, "VPD", startDate);
            ViewBag.Resp = resp;
            return View();
        }
    }
}
