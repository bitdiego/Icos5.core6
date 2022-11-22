namespace Icos5.core6.Models.File
{
    public class MissTimeBin
    {
        public MissTimeBin()
        {

        }
        public MissTimeBin(string miss, string ts, string ff)
        {
            MissingValue = miss;
            TimeStamp = ts;
            FileFormat = ff;
        }
        public string MissingValue { get; set; } = "";
        public string TimeStamp { get; set; } = "";
        public string FileFormat { get; set; } = "";
    }
}
