using IcosClassLibrary.Models;

namespace Icos5.core6.Services.HeaderServices
{
    public interface IHeaderService
    {
        SubmittedFile? GetSubmittedFile(int siteId, int fileTypeId, string findStr1, string findStr2);
        SubmittedFile? GetSubmittedFileTimeStamp(int siteId, int fileTypeId, string findStr1, string findStr2, string timeStamp);
        IEnumerable<GRP_FILE> GetGrpFileList(int siteId, int logger_id, int file_id, string type);
    }
}
