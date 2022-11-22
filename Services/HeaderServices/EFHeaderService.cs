using Icos5.core6.DbManager;
using IcosClassLibrary.Models;

namespace Icos5.core6.Services.HeaderServices
{
    public class EFHeaderService : IHeaderService
    {
        private readonly IcosDbContext _context;

        public EFHeaderService(IcosDbContext ctx)
        {
            _context = ctx;
        }
        public SubmittedFile? GetSubmittedFile(int siteId, int fileTypeId, string findStr1, string findStr2)
        {
            var submittedFile = _context.SubmittedFiles.Where(sf => sf.Site.Id == siteId && sf.FileTypeId == fileTypeId
                                                              && sf.Name.IndexOf(findStr1) >= 0 && sf.Name.IndexOf(findStr2) >= 0 && sf.Status.ToLower() == "uploaded")
                                                            .FirstOrDefault();
            return submittedFile;
        }
        public SubmittedFile? GetSubmittedFileTimeStamp(int siteId, int fileTypeId, string findStr1, string findStr2, string timeStamp)
        {
            var submittedFile = _context.SubmittedFiles.Where(sf => sf.Site.Id == siteId && sf.FileTypeId == fileTypeId
                                                              && sf.Name.IndexOf(findStr1) >= 0 && sf.Name.IndexOf(findStr2) >= 0 && sf.Status.ToLower() == "uploaded"
                                                              && sf.Name.Substring(16, 12).CompareTo(timeStamp) <= 0)
                                                            .FirstOrDefault();
            return submittedFile;
        }
        public IEnumerable<GRP_FILE> GetGrpFileList(int siteId, int logger_id, int file_id, string type)
        {
            var fileHeadInfo = _context.GRP_FILE.Where(ff => ff.SiteId == siteId && ff.FILE_TYPE.ToLower() == type.ToLower() &&
                                                        ff.FILE_LOGGER_ID == logger_id && ff.FILE_ID == file_id && ff.DataStatus == 0)
                                                .OrderByDescending(ff => ff.FILE_DATE).ToList();
            return fileHeadInfo;
        }
    }
}
