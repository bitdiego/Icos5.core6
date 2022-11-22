using Icos5.core6.Services;
using Icos5.core6.Services.Ec;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Icos5.core6.Controllers.Ec
{
    public class EcController : Controller
    {
        private readonly IEcServices _ecService;
        private readonly IUtilityServices _service;

        public EcController(IEcServices ecService, IUtilityServices service)
        {
            _ecService = ecService;
            _service = service;
        }

        public IActionResult GetECOps(string site, int logger_id, int file_id, string timestamp)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View("");
            string ops = _ecService.GetECOperation(id, logger_id, file_id, timestamp);
            ViewBag.EcOps = ops;
            return View();
        }

        public IActionResult GetSaGillAlign(string site, int logger_id, int file_id, string timestamp)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View("");
            string saGillAlign = _ecService.GetSaGillAlign(id, logger_id, file_id, timestamp);
            ViewBag.SAGillAlign = saGillAlign;
            return View();
        }

        public IActionResult GetGaFlowRate(string site, int logger_id, int file_id, string timestamp)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View("");
            string gaFlowRate = _ecService.GetGaFlowRate(id, logger_id, file_id, timestamp);
            
            ViewBag.GaFlowRate = gaFlowRate;
            return View();
        }

        public IActionResult GetFlag(string site, string type, int logger_id, int file_id, string timestamp)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View("");
            if (timestamp.Length == 8) timestamp += "2359";
            string flag = _ecService.GetFlag(id, type, logger_id, file_id, timestamp);
            ViewBag.Flag = flag;
            return View();
        }
    }
}
