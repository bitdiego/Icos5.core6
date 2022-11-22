using Icos5.core6.DbManager;
using Icos5.core6.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using Microsoft.Data.SqlClient;
using System.Data;
using Icos5.core6.Services.KPlotServices;

namespace Icos5.core6.Controllers.KPlot
{
    public class KPlotController : Controller
    {
        private IcosDbContext _context;
        private IUtilityServices _service;
        private IKPlotService _kService;
        //Mayb to move in config json file???
        const string HEADER_PLOT = "FEAT\tLAT\tLON\tPLOT_EASTWARD_DIST\tPLOT_NORTHWARD_DIST\tPLOT_DISTANCE_POLAR\tPLOT_ANGLE_POLAR\tPLOT_RECT_X\tPLOT_RECT_Y\tPLOT_RECT_DIR\tPLOT_DATE\tPLOT_COMMENT";
        const string PATH = @"E:\sa\icosapi\icosapi\jack\"; //@"D:\Diego\ICOS\jack\";
        const string ICOS_PATH = @"F:\icos\";  //@"D:\Diego\ICOS\";

        public int Type { get; set; }

        public KPlotController(IcosDbContext context, IUtilityServices service, IKPlotService kService)
        {
            _context = context;
            _service = service;
            _kService = kService;
        }
        [HttpGet]
        public IActionResult Index(string site, string type)
        {
            int siteId = _service.GetSiteIdByCode(site);
            string realSiteCode = "";
            if (siteId <= 0)
            {
                ViewBag.Message = "Wrong site name";
                return View();
            }
            else
            {
                realSiteCode = _service.GetRealSiteCode(site);
            }

            FileContentResult fileContent = Download(realSiteCode, siteId, type);
            if (fileContent.FileContents.Length == 0)
            {
                ViewBag.Message = "Error: could not download file";
                return View();
            }
            else
            {
                //x.ExecuteResult(this.ControllerContext);
            }
            ViewBag.Message = "File download success";
            //return View();
            return fileContent;
        }

        [HttpGet]
        public FileContentResult Download(string site, int siteId, string type) //FileStreamResult
        {
            string fName = "", fullName = "";
            int flag = 0;
            switch (type.ToLower())
            {
                case "plot":
                    fName = site + "_PLOT.txt";
                    fullName = PATH + fName;
                    WritePlotFile(siteId, fullName);
                    flag = 0;
                    break;
                case "kml":
                    fName = site + "_KML.zip";
                    fullName = PATH + fName;
                    int zip = WriteZipFile(site, siteId, fullName);
                    if (zip > 0) return null;
                    flag = 1;
                    break;
                case "shape":
                case "zip":
                    fullName = AttachZip(site, siteId);
                    int slash = fullName.LastIndexOf("\\");
                    if (slash > 0)
                    {
                        fName = fullName.Substring(slash + 1);
                        flag = 1;
                    }
                    else
                    {
                        byte[] fileBytes = new byte[0];
                        return File(fileBytes, "text/plain", fName);
                    }
                    break;
                default:
                    break;
            }

            if (flag == 0)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullName);
                return File(fileBytes, "text/plain", fName);
               
            }
            if (flag == 1)
            {
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fullName);
                    return File(fileBytes, "application/x-zip-compressed", fName); // the constructor will fire Dispose() when done
                }
            }
            return null;

        }

        private void WritePlotFile(int siteId, string fullName)
        {
            StreamWriter wr = null;
            wr = new StreamWriter(fullName, false);
            wr.WriteLine("ID\tFEAT\tLAT\tLON\tPLOT_EASTWARD_DIST\tPLOT_NORTHWARD_DIST\tPLOT_DISTANCE_POLAR\tPLOT_ANGLE_POLAR\tPLOT_RECT_X\tPLOT_RECT_Y\tPLOT_RECT_DIR\tPLOT_NORTHREF\tPLOT_DATE\tPLOT_COMMENT"); // e PLOT_NORTHWARD_DIST
           // List<string> records = new List<string>();

            var coordinates = _context.GRP_LOCATION.Where(loc => loc.DataStatus == 0 && loc.SiteId == siteId).FirstOrDefault();

            string l0 = coordinates.Id.ToString();
            string l1 = coordinates.LOCATION_LAT.ToString();
            string l2 = coordinates.LOCATION_LONG.ToString();

            string line = 0 + "\t" + "TOWER" + "\t" + l1 + "\t" + l2 + "\t\t\t\t\t\t\t\t\t\t";
            wr.WriteLine(line);
           // records.Add(l0 + "\t" + "TOWER" + "\t" + l1 + "\t" + l2 + "\t\t\t\t\t\t\t\t\t\t");

            var plots = _context.GRP_PLOT.Where(plot=>plot.DataStatus==0 && plot.SiteId==siteId).OrderBy(plot=>plot.PLOT_ID).ToList();

            foreach(var plot in plots)
            {
                line = plot.Id.ToString() + "\t" + plot.PLOT_ID.ToString() + "\t" + plot.PLOT_LOCATION_LAT.ToString() + "\t" + plot.PLOT_LOCATION_LONG.ToString() + "\t" +
                            plot.PLOT_EASTWARD_DIST.ToString() + "\t" + plot.PLOT_NORTHWARD_DIST.ToString() + "\t"
                            + plot.PLOT_DISTANCE_POLAR.ToString() + "\t" + plot.PLOT_ANGLE_POLAR.ToString() +
                            "\t" + plot.PLOT_RECT_X.ToString() + "\t" + plot.PLOT_RECT_Y.ToString() + "\t" +
                            plot.PLOT_RECT_DIR.ToString() + "\t" + plot.PLOT_NORTHREF + "\t" + plot.PLOT_DATE + "\t" + plot.PLOT_COMMENT;
                wr.WriteLine(line);
                /*records.Add(plot.Id.ToString() + "\t" + plot.PLOT_ID.ToString() + "\t" + plot.PLOT_LOCATION_LAT.ToString() + "\t" + plot.PLOT_LOCATION_LONG.ToString() + "\t" +
                            plot.PLOT_EASTWARD_DIST.ToString() + "\t" + plot.PLOT_NORTHWARD_DIST.ToString() + "\t"
                            + plot.PLOT_DISTANCE_POLAR.ToString() + "\t" + plot.PLOT_ANGLE_POLAR.ToString() +
                            "\t" + plot.PLOT_RECT_X.ToString() + "\t" + plot.PLOT_RECT_Y.ToString() + "\t" +
                            plot.PLOT_RECT_DIR.ToString() + "\t" + plot.PLOT_NORTHREF + "\t" + plot.PLOT_DATE+ "\t" + plot.PLOT_COMMENT);*/
            }
           
            /*for (int i = 0; i < records.Count; i++)
            {
                wr.WriteLine(records[i]);
            }*/
            wr.Close();
            wr.Dispose();
        }

        private int WriteZipFile(string siteCode, int siteId, string fullName)
        {

            int res = 0;
            string destDirectory = PATH + siteCode;
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(destDirectory);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            List<string> filesFound = new List<string>();
            var files = _kService.GetSubmittedFilesByType(siteId, ".kml").ToList();

            if (files!=null && files.Count > 0)
            {
                foreach(var file in files)
                {
                    if (!filesFound.Contains(file.OriginalName))
                    {
                        filesFound.Add(file.OriginalName);
                        string pp = "";
                        if (file.OriginalName.EndsWith(".zip"))
                        {
                            pp = ICOS_PATH + siteCode + "\\" + file.OriginalName.Substring(0, file.OriginalName.Length - 4) + "\\" + file.Name;
                        }
                        else
                        {
                            pp = ICOS_PATH + siteCode + "\\" + file.Name;
                        }
                        FileInfo fi = new FileInfo(pp);
                        fi.CopyTo(destDirectory + "\\" + file.Name);
                    }
                }
            }

            else
            {
                return 1;
            }
            res = ZipFolder(fullName, destDirectory);
            return res;
        }

        private static int ZipFolder(string fullName, string destDirectory)
        {
            try
            {
                using var archive = ZipFile.Open(fullName, ZipArchiveMode.Create);
                string[] filenames = Directory.GetFiles(destDirectory);
                foreach (var ff in filenames)
                {
                    var entry = archive.CreateEntryFromFile(ff, Path.GetFileName(ff), CompressionLevel.Optimal);
                }
            }
            catch (Exception ex)
            {
                // No need to rethrow the exception as for our purposes its handled.
                return 2;
            }
            return 0;
        }

        private string AttachZip(string site, int siteId)
        {
            string fn = "";
            string destDirectory = PATH + site;
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(destDirectory);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            
            List<string> filesFound = new List<string>();
            var files = _kService.GetSubmittedFilesByType(siteId, ".zip").ToList();
            if (files!=null && files.Count > 0)
            {
                foreach(var ff in files)
                {
                    if (!filesFound.Contains(ff.OriginalName))
                    {
                        filesFound.Add(ff.OriginalName);
                        string pp = ICOS_PATH + site + "\\" + ff.OriginalName.Substring(0, ff.OriginalName.Length - 4) + "\\" + ff.Name;
                        FileInfo fi = new FileInfo(pp);
                        fi.CopyTo(destDirectory + "\\" + ff.Name);
                        fn = destDirectory + "\\" + ff.Name;
                    }
                }
            }
            else
            {
                return "";
            }
            
            return fn;
        }
    }
}
