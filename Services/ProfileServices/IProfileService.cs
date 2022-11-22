using Icos5.core6.Models.Profile;
using IcosClassLibrary.Models;

namespace Icos5.core6.Services.ProfileServices
{
    public interface IProfileService
    {
        string GetSiteCoordinates(int siteId);
        IEnumerable<STProfile> GetStoProfileList(int siteId);
        IEnumerable<VarProfileExt> GetBmProfileList(int siteId, string site);
        IEnumerable<string> GetStProfileList(int siteId);
    }
}
