using Icos5.core6.DbManager;
using IcosClassLibrary.Models;

namespace Icos5.core6.Services.KPlotServices
{
    public class EFKPlotService : IKPlotService
    {
        private readonly IcosDbContext _context;

        public EFKPlotService(IcosDbContext context)
        {
            _context = context;
        }
        public IEnumerable<SubmittedFile> GetSubmittedFilesByType(int siteId, string ext)
        {
            var files = _context.SubmittedFiles.Where(ff => ff.SiteId == siteId && (ff.FileTypeId == 13 || ff.FileTypeId == 14)
                                                      && ff.Status.ToLower().CompareTo("discarded") != 0 && ff.Name.IndexOf(ext) >= 0)
                                                .OrderByDescending(f=>f.Date).ToList();
            return files;
        }
    }
}
