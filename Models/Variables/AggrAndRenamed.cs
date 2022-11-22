namespace Icos5.core6.Models.Variables
{
    public class AggrAndRenamed
    {
        public int Id { get; set; }
        public string var_name { get; set; } = "";
        public string vars_input { get; set; } = "";
        public string var_output { get; set; } = "";
        public bool in_use { get; set; }
        public string date_from { get; set; } = "";
        public string? date_to { get; set; } = "";
        public string site { get; set; } = "";
        public string original_name { get; set; } = "";
        public string new_name { get; set; } = "";
        public bool vr_in_use { get; set; }
        public string vr_date_from { get; set; } = "";
        public string? vr_date_to { get; set; } = "";

    }
}