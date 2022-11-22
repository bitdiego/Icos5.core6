using Icos5.core6.DbManager;
using Icos5.core6.Models.File;
using Icos5.core6.Models.StringOptions;
using Icos5.core6.Services;
using Icos5.core6.Services.FileServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Drawing;

namespace Icos5.core6.Controllers.File
{
    public class FileController : Controller
    {
        private IcosDbContext _context;
        private IUtilityServices _service;
        private IFIleService _fileService;
        private string pathToFiles = "";
        public FileController(IcosDbContext context, IUtilityServices service, IFIleService fIleService)
        {
            _context = context;
            _service = service;
            _fileService = fIleService;
            pathToFiles = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FilesPath")["LocalPath"];
        }

        public IActionResult GetFormatExt(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            //needs only top 1???
            List<FileForms> ffList = new List<FileForms>();
            ffList = _fileService.GetGrpFileList(id, logger_id, file_id, type).ToList();

            MyStringify<FileForms> stfy = new MyStringify<FileForms>(ffList);
            stfy.MakeString();
            return View(stfy);
        }

        public IActionResult GetFormatExtNew(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            List<FileFormsExt> ffList = new List<FileFormsExt>();

            //needs only top 1???
            ffList = _fileService.GetGrpFileListExt(id, logger_id, file_id, type).ToList();
            
            MyStringify<FileFormsExt> stfy = new MyStringify<FileFormsExt>(ffList);
            stfy.MakeString();
            return View(stfy);
        }

        public IActionResult GetCompressed(string site, int logger_id, int file_id, string type)
        {
            string result = "0";
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
            result = _fileService.GetCompressed(id, logger_id, file_id, type);
            ViewBag.Result = result;
            return View();
        }

        public IActionResult GetEpoch(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id <= 0) return View();
            var epoxList = _fileService.GetEpoch(id, logger_id, file_id, type).ToList();
            
            MyStringify<Epox> stfy = new MyStringify<Epox>(epoxList);
            stfy.MakeString();
            return View(stfy);
        }

        public IActionResult GetMissing(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id == 0)
            {
                return View();
            }

            List<MissTimeBin> mtb = _fileService.GetMissing(id, logger_id, file_id, type).ToList();

            MyStringify<MissTimeBin> stfy = new MyStringify<MissTimeBin>(mtb);
            stfy.MakeString("MissingValue");
            return View(stfy);
        }


        public IActionResult GetTimeStamp(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id == 0)
            {
                return View();
            }
            List<MissTimeBin> mtb = _fileService.GetMissing(id, logger_id, file_id, type).ToList();

            MyStringify<MissTimeBin> stfy = new MyStringify<MissTimeBin>(mtb);
            stfy.MakeString("TimeStamp");
            return View(stfy);
        }

        public IActionResult GetFileFormat(string site, int logger_id, int file_id, string type)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id == 0)
            {
                return View();
            }
            List<MissTimeBin> mtb = _fileService.GetMissing(id, logger_id, file_id, type).ToList();
            
            MyStringify<MissTimeBin> stfy = new MyStringify<MissTimeBin>(mtb);
            stfy.MakeString("FileFormat");
            return View(stfy);
        }

        public IActionResult GetExpectedFiles(string site, string type, string timestamp)
        {
            int id = _service.GetSiteIdByCode(site);
            if (id == 0)
            {
                return View();
            }
            if (timestamp.Length == 8)
            {
                timestamp += "2359";
            }
            var lfList = _fileService.GetExpectedFiles(id, type, timestamp).ToList();
            MyStringify<LoggerFile> stfy = new MyStringify<LoggerFile>(lfList);
            stfy.MakeString();
            return View("getExpectedFiles", stfy);
        }

    }
}
