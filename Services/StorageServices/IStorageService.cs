using System.Collections;

namespace Icos5.core6.Services.StorageServices
{
    public interface IStorageService
    {
        IEnumerable<string> GetStorageConfigProfLevel(int siteId, string date);
        string GetStorageOps(int siteId, int logger_id, int file_id, string variable, string timestamp);
        string GetStorageOpsByLevel(int siteId, int logger_id, int file_id, int stoProfLevel, string timestamp);
        string GetOpenCloseSTModel(int siteId, int logger_id, int file_id, string variable, string timestamp);
    }
}
