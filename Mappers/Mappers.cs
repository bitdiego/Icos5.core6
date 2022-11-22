using Icos5.core6.Models.File;
using IcosClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace Icos5.core6.Mappers
{
    public class Mappers
    {
        public static IEnumerable<FileForms> GrpFileToFileForms(IEnumerable<GRP_FILE> fileInfoList)
        {
            List<FileForms> ffList = new List<FileForms>();
            foreach (var info in fileInfoList)
            {
                FileForms ff = new FileForms();
                ff.FileFormat = info.FILE_FORMAT;
                ff.FileExtension = info.FILE_EXTENSION;
                ffList.Add(ff);
            }
            return ffList;
        }
        public static IEnumerable<FileFormsExt> GrpFileToFileFormsNew(IEnumerable<GRP_FILE> fileInfoList)
        {
            List<FileFormsExt> ffList = new List<FileFormsExt>();

            foreach (var info in fileInfoList)
            {
                FileFormsExt ff = new FileFormsExt();
                ff.FileFormat = info.FILE_FORMAT;
                ff.FileExtension = info.FILE_EXTENSION;
                ff.FileCompress = info.FILE_COMPRESS;
                ffList.Add(ff);
            }

            return ffList;
        }
    }
}
