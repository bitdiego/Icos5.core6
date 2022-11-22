using System.ComponentModel.DataAnnotations;

namespace Icos5.core6.Models.General
{
    public class OutOfRange
    {
        [Key]
        public string Name { get; set; } = "";
        public double Min { get; set; }
        public double Max { get; set; }
        public double? Step { get; set; }
    }
}
