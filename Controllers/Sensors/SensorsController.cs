using Icos5.core6.DbManager;
using Icos5.core6.Models.Sensors;
using Icos5.core6.Models.StringOptions;
using Icos5.core6.Services;
using Icos5.core6.Services.SensorServices;
using IcosClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;

namespace Icos5.core6.Controllers.Sensors
{
    public class SensorsController : Controller
    {
        private IcosDbContext _context;
        private IUtilityServices _service;
        private ISensorService _sensorService;
        private string pathToFiles = "";

        public SensorsController(IcosDbContext context, IUtilityServices service, ISensorService sensorService)
        {
            _context = context;
            _service = service;
            _sensorService = sensorService;
            pathToFiles = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FilesPath")["LocalPath"];
        }

        public IActionResult GetFrequency(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
            List<Frequency> freqs = new List<Frequency>();
            List<Frequency> efList = _sensorService.GetFrequencyList(id, logger_id, file_id, type).ToList();
            
            string preVar = "", preSamplingInt = "";
            if (efList != null && efList.Count>0)
            {
                foreach (var freq in efList)
                {
                    string _si = freq.SamplingInt.ToString();
                    if (String.IsNullOrEmpty(preSamplingInt))
                    {
                        Frequency fq = new Frequency();
                        fq.Name = freq.Name;
                        fq.SamplingInt = freq.SamplingInt;
                        freqs.Add(fq);

                        preVar = freq.Name;
                        preSamplingInt = _si;
                    }
                    else
                    {
                        if (_si == preSamplingInt)
                        {
                            if (freq.Name != preVar)
                            {
                                Frequency fq = new Frequency();
                                fq.Name =freq.Name;
                                fq.SamplingInt = freq.SamplingInt;
                                freqs.Add(fq);
                                preVar = freq.Name;
                            }
                        }
                        else break;
                    }
                }
            }
            
            MyStringify<Frequency> stfy = new MyStringify<Frequency>(freqs);
            stfy.MakeString();
            return View(stfy);
        }

        public IActionResult GetMeteoConfig(string site)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
           
           // List<GRP_BM> bmList = new List<GRP_BM>();
            var results = _sensorService.GetBmConfig(id).ToList();

            MyStringify<GRP_BM> stfy = new MyStringify<GRP_BM>(results);
            stfy.MakeString();
            return View("getMeteoConfig", stfy);
        }

        public IActionResult GetStorageConfig(string site)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
            
            List<GRP_STO> stoList = new List<GRP_STO>();
            stoList = _sensorService.GetStoConfig(id).ToList();
            
            MyStringify<GRP_STO> stfy = new MyStringify<GRP_STO>(stoList);
            stfy.MakeString();

            return View("getStorageConfig", stfy);
        }

        public IActionResult GetVarList(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            if(id<=0) return View();

            List<string> vars = _sensorService.GetVarsList(id, logger_id, file_id, type, _service).ToList();
            
            MyStringify<string> stfy = new MyStringify<string>(vars);
            stfy.MakeByString();
            return View(stfy);
        }

        public IActionResult GetVarListInDay(string site, int logger_id, int file_id, string timestamp, string type)
        {
            if (String.Compare(type, "STO", true) != 0 && String.Compare(type, "ST", true) != 0)
            {
                return View("Wrong type: must be STO or ST");
            }
            if (timestamp.Length != 8)
            {
                return View("Wrong timestamp format: must be YYYYMMDD");
            }
            int id = _service.GetSiteIdByCode(site);
            if (id == 0)
            {
                return View("Wrong site name");
            }
            timestamp += "2359";
            List<string> vars = _sensorService.GetVarsListInDay(id, logger_id, file_id, timestamp, type, _service).ToList();

            MyStringify<string> stfy = new MyStringify<string>(vars);
            stfy.MakeByString();
            return View(stfy);
        }
    }
}
