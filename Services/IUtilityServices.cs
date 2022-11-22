using Icos5.core6.Models.General;
using IcosClassLibrary.Models;

namespace Icos5.core6.Services
{
    public interface IUtilityServices
    {
        int GetSiteIdByCode(string site);
        string GetRealSiteCode(string site);
        string GetIcosLabellingDate(string site);
        IEnumerable<string> IsIcosVar(string type, string shortname);
        IEnumerable<OutOfRange> GetoutOfRange();
        string GetSiteCoordinates(int siteId);
        string GetWetland(int siteId);

        /// 
        bool IsEcSensorInstalled(string model, string sn, int siteId);
        bool IsMeteoSensorInstalled(string model, string sn, int siteId);
        GRP_BM GetLastMeteoSensorOperationByVarAndDate(string model, string sn, string variable, string timestamp, int siteId);
        GRP_STO GetLastStoSensorOperationByVarAndDate(string model, string sn, string variable, string timestamp, int siteId);
        bool IsStoSensorInstalled(string model, string sn, int siteId);
    }
}
