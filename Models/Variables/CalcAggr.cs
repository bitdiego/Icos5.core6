using System.ComponentModel.DataAnnotations;

namespace Icos5.core6.Models.Variables
{
    public class CalcAggr
    {
        [Key]
        public int Id { get; set; }
        public string Site { get; set; } = "";
        public string Vars_Input { get; set; } = "";
        public string Vars_Output { get; set; } = "";
        public bool in_use { get; set; }
        public string date_from { get; set; } = "";
        public string? date_to { get; set; } = "";
        public string? notes { get; set; } = "";
        public int? UserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public int? ResetUserId { get; set; }
        public DateTime? ResetDate { get; set; }
    }
}