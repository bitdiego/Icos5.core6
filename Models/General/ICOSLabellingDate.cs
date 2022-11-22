using System.ComponentModel.DataAnnotations;

namespace Icos5.core6.Models.General
{
    public class ICOSLabellingDate
    {
        [Key]
        public string Site { get; set; } = "";
        public string LabelDate { get; set; } = "";
    }
}
