using Icos5.core6.DbManager;
using Icos5.core6.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Icos5.core6.Controllers
{
    public class Variables : Controller
    {
        private IcosDbContext _context;
        private IUtilityServices _service;
        private string _siteCode = "";
        private string pathToFiles = "";
        public Variables(IcosDbContext context, IUtilityServices services)
        {
            _context = context;
            _service = services;
        }

        public IActionResult GetVarsRenamedExt(string site, string timestamp)
        {

            int _siteId = _service.GetSiteIdByCode(site);
            if (_siteId == 0 || timestamp.Length != 8)
            {
                return View("Wrong site input or wrong date format");
            }
            string siteCode = _service.GetRealSiteCode(site);
            if (String.IsNullOrEmpty(siteCode))
            {
                return View();
            }
            string resp = "";
            DateTime _dtTimestamp = new DateTime(int.Parse(timestamp.Substring(0, 4)),
                                            int.Parse(timestamp.Substring(4, 2)),
                                            int.Parse(timestamp.Substring(6, 2)));
            DateTime? dateStart = null, dateEnd = null;

            /*
            cmd.CommandText = @"SELECT [original_name]  ,[new_name],[date_from],[date_to] FROM [ICOS].[dbo].[Var_Renamed] WHERE site='" + site + "' and in_use=1 order by original_name";
            rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                while (rd.Read())
                {
                    dateStart = null;
                    dateEnd = null;
                    string vv = rd["original_name"].ToString();
                    string timeStampReloaded = timestamp;

                    string _ds = rd["date_from"].ToString();
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
                    string _de = rd["date_to"].ToString();
                    if (String.IsNullOrEmpty(_de))
                    {
                        _de = "-9999";
                    }
                    else
                    {
                        // dateEnd = Convert.ToDateTime(_de);

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
                                resp += rd["original_name"].ToString() + "," + rd["new_name"].ToString() + "\r\n";
                            }
                        }
                        else
                        {
                            if ((String.Compare(_ds, timestamp) <= 0 || String.Compare(_ds, timeStampReloaded) <= 0))
                            {
                                resp += rd["original_name"].ToString() + "," + rd["new_name"].ToString() + "\r\n";
                            }
                        }

                    }

                }
                if (!String.IsNullOrEmpty(resp))
                {
                    resp = resp.Substring(0, resp.Length - 1);
                }
            }
            conn.Close();
            ViewBag.Resp = resp;
            scriviLog("getVarsRenamedExt stop " + DateTime.Now.ToString());*/
            return View();
        }

        public IActionResult GetVarsRenamedInRange(string site, string startYear, string endYear)
        {
            int _siteId = _service.GetSiteIdByCode(site);
            if (_siteId == 0)
            {
                return View("Wrong site input or wrong date format");
            }
            string siteCode = _service.GetRealSiteCode(site);
            if (String.IsNullOrEmpty(siteCode))
            {
                return View();
            }
            string resp = "";

            if (_siteId == 0 || startYear.Length != 4 || endYear.Length != 4)
            {
                return View("Wrong site input or wrong dates format");
            }
            string isoDateStart = startYear.Substring(0, 4) + "0101";
            string isoDateEnd = endYear.Substring(0, 4) + "1231";
            DateTime _dtTimeSart = new DateTime(int.Parse(startYear.Substring(0, 4)), 1, 1);
            DateTime _dtTimeEnd = new DateTime(int.Parse(endYear.Substring(0, 4)), 12, 31);
            DateTime? dateStart = null, dateEnd = null;
            List<string> varsList = new List<string>();

            /*            string query = @"select [original_name]  ,[new_name], [date_from],[date_to] FROM
            (SELECT [original_name]  ,[new_name], [date_from],[date_to] FROM [ICOS].[dbo].[Var_Renamed] WHERE site='xxx'  and in_use=1
            UNION
            select Vars_Output as [original_name],Vars_Output as new_name,[date_from],[date_to] from CalcAggr where Site='xxx'  and in_use=1) as T
            order by new_name ";
                        cmd.CommandText = query.Replace("xxx", site);
                        cmd.CommandTimeout = 0; SqlDataReader rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                dateStart = null;
                                dateEnd = null;
                                string vv = rd["original_name"].ToString();


                                string _ds = rd["date_from"].ToString();
                                if (String.IsNullOrEmpty(_ds))
                                {
                                    _ds = "-9999";
                                }
                                else
                                {
                                    dateStart = DateTime.Now;//Convert.ToDateTime(_ds);
                                }
                                string _de = rd["date_to"].ToString();
                                if (String.IsNullOrEmpty(_de))
                                {
                                    _de = DateTime.Now.ToString("yyyyMMddHHmm");// "-9999";
                                    dateEnd = DateTime.Now;
                                }
                                else
                                {
                                    dateEnd = DateTime.Now; //Convert.ToDateTime(_de);
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
                                        resp += rd["original_name"].ToString() + "," + rd["new_name"].ToString() + "\r\n";
                                        if (!varsList.Contains(rd["new_name"].ToString()))
                                        {
                                            varsList.Add(rd["new_name"].ToString());
                                        }
                                    }

                                }

                            }
                            if (!String.IsNullOrEmpty(resp))
                            {
                                resp = resp.Substring(0, resp.Length - 1);
                            }
                        }
                        conn.Close();
                        ViewBag.Resp = String.Join("\r\n", varsList);//resp; .ToArray()*/
            return View();
        }

        public IActionResult GetVarsAggregated(string site, string startYear, string endYear)
        {
            int _siteId = _service.GetSiteIdByCode(site);
            if (_siteId == 0)
            {
                return View("Wrong site input or wrong date format");
            }
            string siteCode = _service.GetRealSiteCode(site);
            if (String.IsNullOrEmpty(siteCode))
            {
                return View();
            }
            string resp = "";

            if (_siteId == 0 || startYear.Length != 4 || endYear.Length != 4)
            {
                return View("Wrong site input or wrong dates format");
            }
            string isoDateStart = startYear.Substring(0, 4) + "01010000";
            string isoDateEnd = endYear.Substring(0, 4) + "12312359";
            DateTime _dtTimeSart = new DateTime(int.Parse(startYear.Substring(0, 4)), 1, 1);
            DateTime _dtTimeEnd = new DateTime(int.Parse(endYear.Substring(0, 4)), 12, 31);
            DateTime? dateStart = null, dateEnd = null;
            List<string> varsList = new List<string>();

            string isoTimeStart = startYear.Substring(0, 4) + "01010000";
            string isoTimeEnd = endYear.Substring(0, 4) + "12312359";

            string isodateStart = "", isodateEnd = "";

            /*SqlConnection conn = new SqlConnection(Utils.CONN_STRING);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            cmd.CommandText = @"SELECT [var_name]  ,[vars_input]  ,[var_output] , [in_use], date_from, date_to  FROM [ICOS].[dbo].[AggregationRules2021] WHERE site='" + site + "' and in_use=1 order by var_output";
            cmd.CommandTimeout = 0; SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                while (rd.Read())
                {
                    dateStart = null;
                    dateEnd = null;
                    isodateStart = "";
                    isodateEnd = "";

                    string _ds = rd["date_from"].ToString();
                    if (String.IsNullOrEmpty(_ds))
                    {
                        _ds = "-9999";
                    }
                    else
                    {
                        //dateStart = Convert.ToDateTime(_ds);
                    }
                    string _de = rd["date_to"].ToString();
                    if (String.IsNullOrEmpty(_de))
                    {
                        _de = "-9999";
                        dateEnd = DateTime.Now;
                        _de = DateTime.Now.ToString("yyyyMMddHHmm");
                    }
                    else
                    {
                        //dateEnd = Convert.ToDateTime(_de);
                    }
                    if (dateStart == null && dateEnd == null)
                    {

                    }
                    else
                    {
                        // if (_dtTimeEnd < dateStart) continue;
                        if (String.Compare(_ds, isoTimeEnd) > 0 || String.Compare(_de, isoTimeStart) < 0)
                        {
                            continue;
                        }

                        
                        else
                        {
                            //resp += rd["original_name"].ToString() + "," + rd["new_name"].ToString() + "\r\n";
                            if (!varsList.Contains(rd["var_output"].ToString()))
                            {
                                varsList.Add(rd["var_output"].ToString());
                            }
                        }


                    }
                }
                rd.Close();
                //resp = resp.Substring(0, resp.Length - 1);
            }
            else
            {
                rd.Close();
                conn.Close();
                return View();
            }
            conn.Close();
            ViewBag.Resp = String.Join("\r\n", varsList);*/
            return View();
        }

        public IActionResult GetAlb(string site, string startDate)
        {
            int _siteId = _service.GetSiteIdByCode(site);

            if (_siteId == 0 || startDate.Length != 8)
            {
                return View("Wrong site input or wrong dates format");
            }
            DateTime _dtTimeSart = new DateTime(int.Parse(startDate.Substring(0, 4)),
                                                int.Parse(startDate.Substring(4, 2)),
                                                int.Parse(startDate.Substring(6, 2)));
            DateTime? dateStart = null, dateEnd = null;
            List<string> varsList = new List<string>();
            /*SqlConnection conn = new SqlConnection(Utils.CONN_STRING);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT [Vars_Input],[Vars_Output],[date_from] ,[date_to] FROM [ICOS].[dbo].[CalcAggr] where in_use=1 And vars_output like 'ALB%' And site='" + site + "'";
            cmd.CommandTimeout = 0; SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                while (rd.Read())
                {
                    dateStart = null;
                    dateEnd = null;

                    string _ds = rd["date_from"].ToString();
                    if (String.IsNullOrEmpty(_ds))
                    {
                        _ds = "-9999";

                    }
                    else
                    {
                        //dateStart = Convert.ToDateTime(_ds);
                    }
                    string _de = rd["date_to"].ToString();
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
                        sb.Append("'" + rd["Vars_input"].ToString() + "'");
                        sb.Append(",");
                        sb.Append(rd["Vars_output"].ToString());
                        sb.Append(",");
                        sb.Append(rd["date_from"].ToString());
                        sb.Append(",");
                        sb.Append((String.IsNullOrEmpty(_de)) ? "-9999" : rd["date_to"].ToString());
                        varsList.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                rd.Close();
            }
            else
            {
                rd.Close();
                conn.Close();
                return View();
            }
            conn.Close();
            ViewBag.Resp = String.Join("\r\n", varsList);*/
            return View();
        }

        public IActionResult GetNetrad(string site, string startDate)
        {
            int _siteId = _service.GetSiteIdByCode(site);

            if (_siteId == 0 || startDate.Length != 8)
            {
                return View("Wrong site input or wrong dates format");
            }
            DateTime _dtTimeSart = new DateTime(int.Parse(startDate.Substring(0, 4)),
                                                int.Parse(startDate.Substring(4, 2)),
                                                int.Parse(startDate.Substring(6, 2)));
            DateTime? dateStart = null, dateEnd = null;
            List<string> varsList = new List<string>();
            /*SqlConnection conn = new SqlConnection(Utils.CONN_STRING);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT [Vars_Input],[Vars_Output],[date_from] ,[date_to] FROM [ICOS].[dbo].[CalcAggr] where in_use=1 And vars_output like 'NETRAD%' And site='" + site + "'";
            cmd.CommandTimeout = 0; SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                while (rd.Read())
                {
                    dateStart = null;
                    dateEnd = null;

                    string _ds = rd["date_from"].ToString();
                    if (String.IsNullOrEmpty(_ds))
                    {
                        _ds = "-9999";

                    }
                    else
                    {
                        //dateStart = Convert.ToDateTime(_ds);
                    }
                    string _de = rd["date_to"].ToString();
                    if (String.IsNullOrEmpty(_de))
                    {
                        _de = "";
                        dateEnd = DateTime.Now;
                    }
                    else
                    {
                        //dateEnd = Convert.ToDateTime(_de);
                        //_de = dateEnd.ToString("yyyyMMddHHmm");
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
                        sb.Append("'" + rd["Vars_input"].ToString() + "'");
                        sb.Append(",");
                        sb.Append(rd["Vars_output"].ToString());
                        sb.Append(",");
                        sb.Append(rd["date_from"].ToString());
                        sb.Append(",");
                        sb.Append((String.IsNullOrEmpty(_de)) ? "-9999" : rd["date_to"].ToString());
                        varsList.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                rd.Close();
                //resp = resp.Substring(0, resp.Length - 1);
            }
            else
            {
                rd.Close();
                conn.Close();
                return View();
            }
            conn.Close();
            ViewBag.Resp = String.Join("\r\n", varsList);*/
            return View();
        }

        public IActionResult GetVpd(string site, string startDate)
        {
            int _siteId = _service.GetSiteIdByCode(site);

            if (_siteId == 0 || startDate.Length != 8)
            {
                return View("Wrong site input or wrong dates format");
            }
            DateTime _dtTimeSart = new DateTime(int.Parse(startDate.Substring(0, 4)),
                                                int.Parse(startDate.Substring(4, 2)),
                                                int.Parse(startDate.Substring(6, 2)));
            DateTime? dateStart = null, dateEnd = null;
            List<string> varsList = new List<string>();
            /*SqlConnection conn = new SqlConnection(Utils.CONN_STRING);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT [Vars_Input],[Vars_Output],[date_from] ,[date_to] FROM [ICOS].[dbo].[CalcAggr] where in_use=1 And vars_output like 'VPD%' And site='" + site + "'";
            cmd.CommandTimeout = 0; SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                while (rd.Read())
                {
                    dateStart = null;
                    dateEnd = null;

                    string _ds = rd["date_from"].ToString();
                    if (String.IsNullOrEmpty(_ds))
                    {
                        _ds = "-9999";

                    }
                    else
                    {
                        //dateStart = Convert.ToDateTime(_ds);
                    }
                    string _de = rd["date_to"].ToString();
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
                        sb.Append("'" + rd["Vars_input"].ToString() + "'");
                        sb.Append(",");
                        sb.Append(rd["Vars_output"].ToString());
                        sb.Append(",");
                        sb.Append(rd["date_from"].ToString());
                        sb.Append(",");
                        sb.Append((String.IsNullOrEmpty(_de)) ? "-9999" : rd["date_to"].ToString());
                        varsList.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                rd.Close();
                //resp = resp.Substring(0, resp.Length - 1);
            }
            else
            {
                rd.Close();
                conn.Close();
                return View();
            }
            conn.Close();
            ViewBag.Resp = String.Join("\r\n", varsList);*/
            return View();
        }

        public IActionResult GetGTSWC(string site, string startDate)
        {
            int _siteId = _service.GetSiteIdByCode(site);

            if (_siteId == 0 || startDate.Length != 8)
            {
                return View("Wrong site input or wrong dates format");
            }
            DateTime _dtTimeSart = new DateTime(int.Parse(startDate.Substring(0, 4)),
                                                int.Parse(startDate.Substring(4, 2)),
                                                int.Parse(startDate.Substring(6, 2)));
            DateTime? dateStart = null, dateEnd = null;
            List<string> varsList = new List<string>();
            /*SqlConnection conn = new SqlConnection(Utils.CONN_STRING);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT [Vars_Input],[Vars_Output],[date_from] ,[date_to] FROM [ICOS].[dbo].[CalcAggr] where in_use=1 And vars_output like 'SG%' And site='" + site + "'";
            cmd.CommandTimeout = 0; SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                while (rd.Read())
                {
                    dateStart = null;
                    dateEnd = null;

                    string _ds = rd["date_from"].ToString();
                    if (String.IsNullOrEmpty(_ds))
                    {
                        _ds = "-9999";

                    }
                    else
                    {
                        //dateStart = Convert.ToDateTime(_ds);
                    }
                    string _de = rd["date_to"].ToString();
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
                        sb.Append("'" + rd["Vars_input"].ToString() + "'");
                        sb.Append(",");
                        sb.Append(rd["Vars_output"].ToString());
                        sb.Append(",");
                        sb.Append(rd["date_from"].ToString());
                        sb.Append(",");
                        sb.Append((String.IsNullOrEmpty(_de)) ? "-9999" : rd["date_to"].ToString());
                        varsList.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                rd.Close();
                //resp = resp.Substring(0, resp.Length - 1);
            }
            else
            {
                rd.Close();
                conn.Close();
                return View();
            }
            conn.Close();
            ViewBag.Resp = String.Join("\r\n", varsList);
*/
            return View();
        }

        [HttpGet("getdatarecords")]
        public FileContentResult GetDataRecords(string site, string varName) //FileStreamResult
        {
            throw new NotImplementedException();
        }
    }
}
