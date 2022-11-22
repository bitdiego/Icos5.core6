using Icos5.core6.Models.Sensors;
using IcosClassLibrary.Models;

namespace Icos5.core6.Services.SensorServices
{
    public interface ISensorService
    {
        IEnumerable<Frequency> GetFrequencyList(int siteId, int logger_id, int file_id, string type);
        IEnumerable<GRP_BM> GetBmConfig(int siteId);
        IEnumerable<GRP_STO> GetStoConfig(int siteId);
        IEnumerable<string> GetVarsList(int siteId, int logger_id, int file_id, string type, IUtilityServices _service);
        IEnumerable<string> GetVarsListInDay(int siteId, int logger_id, int file_id, string timeStamp, string type, IUtilityServices _service);
    }
}
