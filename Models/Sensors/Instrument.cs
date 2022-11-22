namespace Icos5.core6.Models.Sensors
{
    public class Instrument
    {
        public string Model { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Type { get; set; } = "";
        public decimal Height { get; set; }
        public decimal EastwardDist { get; set; }
        public decimal NorthwardDist { get; set; }
        public decimal SamplingInt { get; set; }
        public int LoggerID { get; set; }
        public int FileID { get; set; }
        public string Date { get; set; } = "";
        public string DateStart { get; set; } = "";
        public string DateEnd { get; set; } = "";
        public decimal DateUnc { get; set; }
        public string Comment { get; set; } = "";
    }
}
