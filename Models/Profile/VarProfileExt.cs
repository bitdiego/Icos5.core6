using System.ComponentModel.DataAnnotations;

namespace Icos5.core6.Models.Profile
{
    public class VarProfileExt
    {
        [Key]
        public int Id { get; set; }
        public string Site { get; set; } = "";
        public string InstrumentModel { get; set; } = "";
        public string InstrumentSn { get; set; } = "";
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public string VarName { get; set; } = "";//TA, RH, PA...
        public string VarOriginalName { get; set; } = ""; //TA_x_y_z...
        public string VarRenamed { get; set; } = ""; //TA_x1_y1_z1...
        public string VarOutput { get; set; } = "";//TA_x, PA_y, RH_z
        public decimal? Height { get; set; }
        public decimal? SamplingInt { get; set; }
        public string Date { get; set; } = "";
        public string DateStart { get; set; } = "";
        public string DateEnd { get; set; } = "";
    }
}
