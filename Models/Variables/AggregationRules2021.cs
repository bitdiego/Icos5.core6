using System.ComponentModel.DataAnnotations;

namespace Icos5.core6.Models.Variables
{
    public class AggregationRules2021
    {
        [Key]
		public int id_AggregationRules { get; set; }
		public string site { get; set; } = "";
        public string var_name { get; set; } = "";
        public string vars_input { get; set; } = "";
        public string var_output { get; set; } = "";
		public bool in_use { get; set; }
        public string? date_from { get; set; } = "";
        public string? date_to { get; set; } = "";
        public string? notes { get; set; } = "";
        public int? UserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public int? ResetUserId { get; set; }
        public DateTime? ResetDate { get; set; }
    }
}