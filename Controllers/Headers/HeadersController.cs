using Icos5.core6.DbManager;
using Icos5.core6.Models.Headers;
using Icos5.core6.Models.StringOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Icos5.core6.Utils;
using Microsoft.Data.SqlClient;
using Icos5.core6.Services;
using Icos5.core6.Services.HeaderServices;
using IcosClassLibrary.Models;

namespace Icos5.core6.Controllers.Headers
{
    public class HeadersController : Controller
    {
        private IcosDbContext _context;
        private IUtilityServices _service;
        private IHeaderService _headerService;
        private string _siteCode = "";
        private string pathToFiles = "";

        public HeadersController(IcosDbContext context, IUtilityServices service, IHeaderService headerService)
        {
            _context = context;
            _service = service;
            _headerService = headerService;
            pathToFiles = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FilesPath")["LocalPath"];
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetHeaderContent(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            var _sCode = _service.GetRealSiteCode(site);
            if (String.IsNullOrEmpty(_sCode))
            {
                return View();
            }
            string subFileName1, subFileName2;
            subFileName1 = _sCode + "_" + type + "HEADER"; //HEADER_[0-9]
            subFileName2 = "L" + Utilities.LeadingZero(logger_id);   //((logger_id < 10) ? "0" + logger_id.ToString() : logger_id.ToString()); //%L02_F01.csv
            subFileName2 += "_F" + Utilities.LeadingZero(file_id);// ((file_id < 10) ? "0" + file_id.ToString() : file_id.ToString());
            subFileName2 += ".csv";

            var submittedFile = _headerService.GetSubmittedFile(id, 18, subFileName1, subFileName2);
            if(submittedFile == null)
            {
                return View();
            }
            string fileToread = submittedFile.Name;
            List<HeaderContent> content = new List<HeaderContent>();
            HeaderContent hc = new HeaderContent();
            hc.FileName = fileToread;
            using (StreamReader reader= new System.IO.StreamReader(pathToFiles  +_sCode + "\\"+ fileToread))
            {
                string cont = reader.ReadToEnd();
                hc.SContent += cont;
                content.Add(hc);
            }
            
            MyStringify<HeaderContent> stfy = new MyStringify<HeaderContent>(content);
            stfy.MakeStringNewLine();
            return View(stfy);
        }

        public ActionResult GetHeaderContentTimeStamp(string site, int logger_id, int file_id, string type, string timeStamp)
        {
            int id = _service.GetSiteIdByCode(site);
            var _sCode = _service.GetRealSiteCode(site);
            if (timeStamp.Length == 8) timeStamp += "2359";
           
            string subFileName1, subFileName2;
            subFileName1 = site + "_" + type + "HEADER";
            subFileName2 = "L" + Utilities.LeadingZero(logger_id);
            subFileName2 += "_F" + Utilities.LeadingZero(file_id);
            subFileName2 += ".csv";

            var submittedFile = _headerService.GetSubmittedFileTimeStamp(id, 18, subFileName1, subFileName2, timeStamp);
                
            if (submittedFile == null)
            {
                return View();
            }
            string fileToread = submittedFile.Name;
            List<HeaderContent> content = new List<HeaderContent>();
            HeaderContent hc = new HeaderContent();
            hc.FileName = fileToread;
            using (StreamReader reader = new System.IO.StreamReader(pathToFiles + _sCode + "\\" + fileToread))
            {
                string cont = reader.ReadToEnd();
                hc.SContent += cont;
                content.Add(hc);
            }

            MyStringify<HeaderContent> stfy = new MyStringify<HeaderContent>(content);
            stfy.MakeStringNewLine();
            return View(stfy);

        }

        public IActionResult GetHeaderinfo(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            var _sCode = _service.GetRealSiteCode(site);
            List<HeaderInfo> hiList = new List<HeaderInfo>();

            var fileHeadInfo = _context.GRP_FILE.Where(ff => ff.SiteId == id && ff.FILE_TYPE.ToLower() == type.ToLower() &&
                                                        ff.FILE_LOGGER_ID == logger_id && ff.FILE_ID == file_id && ff.DataStatus == 0).OrderByDescending(ff => ff.FILE_DATE).ToList();

            foreach(var ff in fileHeadInfo)
            {
                HeaderInfo hinfo = new HeaderInfo();
                if (ff.FILE_HEAD_VARS != null)
                {
                    hinfo.FileHeadVars = ff.FILE_HEAD_VARS;
                }
                else
                {
                    hinfo.FileHeadVars = 0;
                }
                if (ff.FILE_HEAD_NUM != null)
                {
                    hinfo.FileHeadNum = ff.FILE_HEAD_NUM;
                }
                else
                {
                    hinfo.FileHeadNum = 0;
                }

                hiList.Add(hinfo);
            }

            MyStringify<HeaderInfo> stfy = new MyStringify<HeaderInfo>(hiList);
            stfy.MakeString();
            return View(stfy);
        }

        public IActionResult GetHeaderinfoTimeStamp(string site, int logger_id, int file_id, string type, string timeStamp)
        {
            int id = _service.GetSiteIdByCode(site);
            var _sCode = _service.GetRealSiteCode(site);
            if (timeStamp.Length == 8) timeStamp += "2359";
            List<HeaderInfo> hiList = new List<HeaderInfo>();

            var fileHeadInfo = _context.GRP_FILE.Where(ff => ff.SiteId == id && ff.FILE_TYPE.ToLower() == type.ToLower() &&
                                                        ff.FILE_LOGGER_ID == logger_id && ff.FILE_ID == file_id
                                                        && ff.FILE_DATE.CompareTo(timeStamp)<=0 && ff.DataStatus == 0).OrderByDescending(ff => ff.FILE_DATE).ToList();



            foreach (var ff in fileHeadInfo)
            {
                HeaderInfo hinfo = new HeaderInfo();
                if (ff.FILE_HEAD_VARS != null)
                {
                    hinfo.FileHeadVars = ff.FILE_HEAD_VARS;
                }
                else
                {
                    hinfo.FileHeadVars = 0;
                }
                if (ff.FILE_HEAD_NUM != null)
                {
                    hinfo.FileHeadNum = ff.FILE_HEAD_NUM;
                }
                else
                {
                    hinfo.FileHeadNum = 0;
                }

                hiList.Add(hinfo);
            }

            MyStringify<HeaderInfo> stfy = new MyStringify<HeaderInfo>(hiList);
            stfy.MakeString();
            return View(stfy);
        }
    }
}
