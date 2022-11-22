namespace Icos5.core6.Models.File
{
    public class LoggerFile
    {
        public LoggerFile(int l, int f)
        {
            LoggerId = l;
            FileId = f;
        }
        public int LoggerId { get; set; }
        public int FileId { get; set; }
        public string Suffix { get; set; } = "";
    }
}
