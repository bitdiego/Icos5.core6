using Icos5.core6.DbManager;
using Icos5.core6.Models.Variables;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Icos5.core6.Services.VariablesServices
{
    public class VariableService : IVariableService
    {
        private readonly IcosDbContext _context;
        public VariableService(IcosDbContext context)
        {
            _context = context;
        }

        public string GetAggregatedVarList(string site)
        {
            StringBuilder sb=new StringBuilder();
            var aggVars = _context.AggregationRules2021.Where(vv=>vv.in_use && vv.site.ToLower() == site.ToLower()).ToList();
            foreach(var aggVar in aggVars)
            {
                var xvar = aggVar.vars_input.Replace(" ", "");
                string[] xvar_ar = xvar.Split(',');
                sb.Append(aggVar.var_name + ",'" + xvar + "'," + aggVar.var_output + ",");
                sb.Append(aggVar.date_from + ",");
                sb.Append(String.IsNullOrEmpty(aggVar.date_to)?"-9999" : aggVar.date_to + ",");
                sb.Append("\n");
            }
            sb.Remove(sb.Length - 1,1);
            return sb.ToString();
        }

        public string GetAggregatedVarListByName(string site, string variable, string startDate)
        {
            List<string> varsList = new List<string>();
            DateTime? dateStart = null, dateEnd = null;
            var list = _context.CalcAggr.Where(ca => ca.Site.ToLower() == site.ToLower() && ca.in_use && ca.Vars_Output.ToLower().IndexOf(variable.ToLower()) >= 0);
            if (list != null)
            {
                foreach(var ca in list)
                {
                    dateStart = null;
                    dateEnd = null;
                    string _ds = ca.date_from;
                    if (String.IsNullOrEmpty(_ds))
                    {
                        _ds = "-9999";

                    }
                    else
                    {
                        //dateStart = Convert.ToDateTime(_ds);
                    }
                    string _de = ca.date_to;
                    if (String.IsNullOrEmpty(_de))
                    {
                        _de = "";
                        dateEnd = DateTime.Now;
                    }
                    else
                    {
                        
                    }
                    if (dateStart == null && dateEnd == null)
                    {

                    }
                    else
                    {
                        // if (_dtTimeEnd < dateStart) continue;
                        if (!String.IsNullOrEmpty(_de) && String.Compare(startDate, _de) > 0) continue;
                        // if (_dtTimeSart > dateEnd) continue;
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append("'" + ca.Vars_Input + "'");
                        sb.Append(",");
                        sb.Append(ca.Vars_Output);
                        sb.Append(",");
                        sb.Append(ca.date_from);
                        sb.Append(",");
                        sb.Append((String.IsNullOrEmpty(_de)) ? "-9999" : ca.date_to);
                        varsList.Add(sb.ToString());
                        sb.Clear();
                    }
                }
            }
            return String.Join("\r\n", varsList);
        }
        public string GetVarsRenamedList(string site, string timestamp)
        {
            string resp = "";
            DateTime? dateStart = null, dateEnd = null;
            var varRenamed = _context.Var_Renamed.Where(vr => vr.site.ToLower() == site.ToLower() && vr.in_use == true).OrderBy(vr => vr.original_name).ToList();
            if (varRenamed != null)
            {
                foreach (var vr in varRenamed)
                {
                    dateStart = null;
                    dateEnd = null;
                    string vv = vr.original_name;
                    string timeStampReloaded = timestamp;

                    string _ds = vr.date_from;
                    if (String.IsNullOrEmpty(_ds))
                    {
                        _ds = "-9999";

                    }
                    else
                    {
                        //dateStart = Convert.ToDateTime(_ds);
                        dateStart = new DateTime(int.Parse(_ds.Substring(0, 4)),
                                            int.Parse(_ds.Substring(4, 2)),
                                            int.Parse(_ds.Substring(6, 2)));
                    }
                    string _de = vr.date_to;
                    if (String.IsNullOrEmpty(_de))
                    {
                        _de = "-9999";
                    }

                    if (_ds.Length > timestamp.Length)
                    {
                        while (timeStampReloaded.Length < _ds.Length)
                        {
                            timeStampReloaded += "0";
                        }
                    }

                    if (dateStart == null && dateEnd == null)
                    {

                    }
                    else
                    {
                        if (dateEnd != null || _de != "-9999")
                        {
                            if ((String.Compare(_ds, timestamp) <= 0 || String.Compare(_ds, timeStampReloaded) <= 0) && String.Compare(timestamp, _de) < 0)
                            {
                                resp += vr.original_name + "," + vr.new_name + "\r\n";
                            }
                        }
                        else
                        {
                            if ((String.Compare(_ds, timestamp) <= 0 || String.Compare(_ds, timeStampReloaded) <= 0))
                            {
                                resp += vr.original_name + "," + vr.new_name + "\r\n";
                            }
                        }

                    }
                }
            }

            return resp;
        }
        public string GetVarsRenamedAggregatedInRange(string site, string startYear, string endYear)
        {
            string resp = "";
            string isoDateStart = startYear.Substring(0, 4) + "0101";
            string isoDateEnd = endYear.Substring(0, 4) + "1231";
            DateTime _dtTimeSart = new DateTime(int.Parse(startYear.Substring(0, 4)), 1, 1);
            DateTime _dtTimeEnd = new DateTime(int.Parse(endYear.Substring(0, 4)), 12, 31);
            DateTime? dateStart = null, dateEnd = null;
            List<string> varsList = new List<string>();

            List<BaseVarRenamed> bv = new List<BaseVarRenamed>();
            bv=_context.CalcAndRenamed.Where(vr => vr.site.ToLower() == site.ToLower()).OrderBy(vr => vr.new_name).ToList();

            foreach (var v in bv)
            {
                dateStart = null;
                dateEnd = null;
                string vv = v.original_name;
                string _ds = v.date_from;
                if (String.IsNullOrEmpty(_ds))
                {
                    _ds = "-9999";
                }
                else
                {
                    dateStart = DateTime.Now;
                }
                string _de = v.date_to;
                if (String.IsNullOrEmpty(_de))
                {
                    _de = DateTime.Now.ToString("yyyyMMddHHmm");
                    dateEnd = DateTime.Now;
                }
                else
                {
                    dateEnd = DateTime.Now; 
                }
                if (dateStart == null && dateEnd == null)
                {

                }
                else
                {
                    if (String.Compare(_ds, isoDateEnd) > 0 || String.Compare(_de, isoDateStart) < 0)
                    {
                        continue;
                    }
                   
                    else
                    {
                        resp += v.original_name + "," + v.new_name + "\r\n";
                        if (!varsList.Contains(v.original_name))
                        {
                            varsList.Add(v.original_name);
                        }
                    }

                }
            }
            return String.Join("\r\n", varsList);
        }
    }
}
