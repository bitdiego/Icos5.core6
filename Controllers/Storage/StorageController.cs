using Icos5.core6.Models.StringOptions;
using Icos5.core6.Services;
using Icos5.core6.Services.StorageServices;
using Microsoft.AspNetCore.Mvc;

namespace Icos5.core6.Controllers.Storage
{
    public class StorageController : Controller
    {
        private readonly IStorageService _stoSrevice;
        private IUtilityServices _service;
        public StorageController(IStorageService stoService, IUtilityServices service)
        {
            _stoSrevice = stoService;
            _service = service;
        }
        public IActionResult GetStorageOps(string site, int logger_id, int file_id, string variable, string timestamp)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
            string ops = _stoSrevice.GetStorageOps(id, logger_id, file_id, variable, timestamp);
            ViewBag.StorageOps = ops;
            return View();
        }

        public IActionResult GetStorageOpsByLevel(string site, int logger_id, int file_id, int stoProfLevel, string timestamp)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
            string ops = _stoSrevice.GetStorageOpsByLevel(id, logger_id, file_id, stoProfLevel, timestamp);
            ViewBag.StorageOps = ops;
            return View();
        }

        public IActionResult GetStorageConfigProfLevel(string site, string date)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
            var stoList = _stoSrevice.GetStorageConfigProfLevel(id, date).ToList();
            MyStringify<string> stfy = new MyStringify<string>(stoList);
            stfy.MakeByStringNewLine();
            return View(stfy);
        }

        public IActionResult GetOpenCloseSTModel(string site, int logger_id, int file_id, string variable, string timestamp)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
            string s = _stoSrevice.GetOpenCloseSTModel(id, logger_id, file_id, variable, timestamp);
            return View();
        }
    }
}
