using Icos5.core6.DbManager;
using Icos5.core6.Services.SensorServices;
using Icos5.core6.Services;
using Microsoft.AspNetCore.Mvc;
using Icos5.core6.Models.Profile;
using Icos5.core6.Models.StringOptions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using Icos5.core6.Models.Sensors;
using Icos5.core6.Services.ProfileServices;

namespace Icos5.core6.Controllers.Profile
{
    public class ProfileController : Controller
    {
        private IUtilityServices _service;
        private IProfileService _profService;

        public ProfileController(IUtilityServices service, IProfileService profService)
        {
            _service = service;
            _profService = profService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult BMProfile(string site)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();

            var bmList = _profService.GetBmProfileList(id, site).ToList();
            MyStringify<VarProfileExt> stfy = new MyStringify<VarProfileExt>(bmList);
            stfy.MakeString();
            return View(stfy);
        }

        public ActionResult STProfile(string site)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();

            List<string> resp = _profService.GetStProfileList(id).ToList();
            MyStringify<string> stfy = new MyStringify<string>(resp);
            stfy.MakeByStringNewLine();
            return View(stfy);
        }
    }
}
