using IcosClassLibrary.Models;

namespace Icos5.core6.Services.KPlotServices
{
    public interface IKPlotService
    {
        IEnumerable<SubmittedFile> GetSubmittedFilesByType(int siteId, string ext);
    }
}
