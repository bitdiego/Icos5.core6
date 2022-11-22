using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icos5.core6.Models.Variables
{
    public class Var_Renamed
    {
        [Key]
        public int id_var_renamed { get; set; }
        public string site { get; set; } = "";
        public string original_name { get; set; } = "";
        public string new_name { get; set; } = "";
        public bool in_use { get; set; }
        public string? date_from { get; set; } = "";
        public string? date_to { get; set; } = "";
        public string? notes { get; set; } = "";
        public int? UserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public int? ResetUserId { get; set; }
        public DateTime? ResetDate { get; set; }
    }

    /***********************/
    public class BaseVarRenamed
    {
        [Key]
        public int Id { get; set; }
        public string site { get; set; } = "";
        public string original_name { get; set; } = "";
        public string new_name { get; set; } = "";
        public string? date_from { get; set; } = "";
        public string? date_to { get; set; } = "";
    }


}
