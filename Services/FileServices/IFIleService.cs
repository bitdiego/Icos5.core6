using Icos5.core6.Models.File;
using IcosClassLibrary.Models;

namespace Icos5.core6.Services.FileServices
{
    public interface IFIleService
    {
        IEnumerable<FileForms> GetGrpFileList(int siteId, int logger_id, int file_id, string type);
        IEnumerable<FileFormsExt> GetGrpFileListExt(int siteId, int logger_id, int file_id, string type);
        string GetCompressed(int siteId, int logger_id, int file_id, string type);
        IEnumerable<Epox> GetEpoch(int siteId, int logger_id, int file_id, string type);
        IEnumerable<MissTimeBin> GetMissing(int siteId, int logger_id, int file_id, string type);
        IEnumerable<MissTimeBin> GetTimestamp(int siteId, int logger_id, int file_id, string type);
        GRP_FILE? GetGrpFile(int siteId, int logger_id, int file_id, string type);
        IEnumerable<LoggerFile> GetExpectedFiles(int siteId, string type, string timestamp);
    }
}
