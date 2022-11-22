namespace Icos5.core6.Services.BmServices
{
    public interface IBmService
    {
        string GetBMOperation(int siteId, int logger_id, int file_id, string variable, string timestamp);
        IEnumerable<string> BMVarMapped(int siteId, int logger_id, int file_id, string variable);
    }
}
