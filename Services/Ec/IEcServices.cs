namespace Icos5.core6.Services.Ec
{
    public interface IEcServices
    {
        string GetECOperation(int siteId, int logger_id, int file_id, string timestamp);
        string GetSaGillAlign(int siteId, int logger_id, int file_id, string timestamp);
        string GetGaFlowRate(int siteId, int logger_id, int file_id, string timestamp);
        string GetFlag(int siteId, string type, int logger_id, int file_id, string timestamp);
    }
}
