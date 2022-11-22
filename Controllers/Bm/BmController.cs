using Icos5.core6.Services.Ec;
using Icos5.core6.Services;
using Microsoft.AspNetCore.Mvc;
using Icos5.core6.Services.BmServices;
using Icos5.core6.Models.StringOptions;
using Microsoft.Data.SqlClient;

namespace Icos5.core6.Controllers.Bm
{
    public class BmController : Controller
    {
        private readonly IBmService _bmService;
        private readonly IUtilityServices _service;

        public BmController(IBmService bmService, IUtilityServices service)
        {
            _bmService = bmService;
            _service = service;
        }

        public IActionResult IsBMVarMapped(string site, int logger_id, int file_id, string variable)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View("");
            List<string> bmv = _bmService.BMVarMapped(id, logger_id, file_id, variable).ToList();
            MyStringify<string> stfy = new MyStringify<string>(bmv);
            stfy.MakeByStringNewLine();
            return View(stfy);
        }

        public ActionResult GetBMOps(string site, int logger_id, int file_id, string variable, string timestamp)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0 || timestamp.Length != 8) return View("");
            
            string ops = _bmService.GetBMOperation(id, logger_id, file_id, variable, timestamp);
            ViewBag.BMOps = ops;
            return View();
        }
    }
}
