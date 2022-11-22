using Icos5.core6.DbManager;
using Icos5.core6.Models.Profile;
using Icos5.core6.Models.Sensors;
using IcosClassLibrary.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Security.Policy;
using System.Text;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Icos5.core6.Services.ProfileServices
{
    public class EFProfileService : IProfileService
    {
        private readonly IcosDbContext _context;
        private readonly IUtilityServices _utilityServices;
        public EFProfileService(IcosDbContext context, IUtilityServices utilityServices)
        {
            _context = context;
            _utilityServices = utilityServices;
        }
        public string GetSiteCoordinates(int siteId)
        {
            string coo = "";
            var location = _context.GRP_LOCATION.Where(l => l.SiteId == siteId && l.DataStatus == 0).FirstOrDefault();
            if (location != null)
            {
                coo = location.LOCATION_LAT + "," + location.LOCATION_LONG;
            }
            return coo;
        }

        public IEnumerable<STProfile> GetStoProfileList(int siteId)
        {
            List<STProfile> stProfile = new List<STProfile>();

            return stProfile;
        }
        public IEnumerable<VarProfileExt> GetBmProfileList(int siteId, string site)
        {
            List<VarProfileExt> bmList = new List<VarProfileExt>();
            List<string> taProfile;
            List<string> rhProfile;
            List<string> paProfile;
            List<VarProfileExt> TaList = new List<VarProfileExt>();
            List<VarProfileExt> RhList = new List<VarProfileExt>();
            List<VarProfileExt> PaList = new List<VarProfileExt>();
            string labelDate = _utilityServices.GetIcosLabellingDate(site);
            //1. get Ta, PA, RH from AggAndrenamed view. select name, new name, original name and date from / to
            //   For each variable: 
            //2. get First Date Before Label
            //3. get sensor model and sn and height

            var profs = _context.AggrAndRenamed.Where(agg => agg.site == site && (agg.var_name.StartsWith("TA") || agg.var_name.StartsWith("RH") || agg.var_name.StartsWith("PA")));
            if (profs.Any())
            {
                foreach(var pro in profs)
                {
                    taProfile = new List<string>();
                    rhProfile = new List<string>();
                    paProfile = new List<string>();
                    bool bNext = false;
                    string var_name = pro.var_name;
                    //string name = pro.var_name;
                    string newName = pro.new_name;
                    string orName = pro.original_name;
                    string varOut = pro.var_output;
                    //string _dateFrom = Convert.ToDateTime(reader["date_from"].ToString()).ToString("yyyyMMddHHmm");
                    string _dateFrom = pro.date_from;
                    string? _dateTo = pro.date_to;
                    //string _dateStart = Convert.ToDateTime(reader["date_start"].ToString()).ToString("yyyyMMddHHmm");
                    //string _dateStart = reader["VRFROM"].ToString();
                    //string _dateEnd = reader["VRTO"].ToString();
                    if (!String.IsNullOrEmpty(_dateTo))
                    {
                        //_dateTo = Convert.ToDateTime(reader["date_to"].ToString()).ToString("yyyyMMddHHmm");
                    }
                    if (!String.IsNullOrEmpty(_dateTo))
                    {
                        //_dateEnd = Convert.ToDateTime(reader["date_end"].ToString()).ToString("yyyyMMddHHmm");
                        bNext = true;
                    }
                    else
                    {
                        bNext = false;
                    }

                    string instName = "", instSn = "", installDate = "";
                    string dtf = labelDate;
                    if (bNext)
                    {
                       // dtf = _dateStart;
                    }
                    GetFirstDateBeforeLabelByVar(siteId, orName, dtf, ref instName, ref instSn, ref installDate);

                    decimal? _height = GetSensorHeight(siteId, orName, _dateFrom, _dateTo, instName, instSn); //(String.Compare(installDate, labelDate)<0)?installDate:labelDate
                    if (_height == null)
                    {
                        _height = GetSensorHeight(siteId, orName, installDate, _dateTo, instName, instSn);
                    }
                    if (_height == null)
                    {
                        string prefix = (var_name.StartsWith("TA")) ? "RH" : "TA";
                        _height = FindHeightByOtherVariable(siteId, prefix, installDate, _dateTo, instName, instSn);
                        if (_height == null)
                        {
                            _height = FindHeightByOtherVariable(siteId, prefix, installDate, _dateTo, instName, instSn);
                        }
                    }

                    VarProfileExt vpMin = new VarProfileExt()
                    {
                        VarName = var_name,
                        VarOriginalName = orName,
                        VarRenamed = newName,
                        VarOutput = varOut,
                        Height = _height,
                        DateStart = _dateFrom,// _dateStart,
                        DateEnd = _dateTo// _dateEnd
                    };

                    bmList.Add(vpMin);
                }
            }
           
            return bmList;
        }
        public IEnumerable<string> GetStProfileList(int siteId)
        {
            List<STInstrument> stInstList = new List<STInstrument>();
            List<GRP_STO> stProfile = new List<GRP_STO>();
            stProfile = _context.GRP_STO.Where(st => st.SiteId == siteId && st.DataStatus == 0 &&
                                               (st.STO_TYPE.ToLower() == "level installation" || st.STO_TYPE.ToLower() == "level removal"))
                                        .OrderBy(st => st.STO_DATE).ToList();
            List<int> profilesId = new List<int>();
            profilesId = stProfile.Select(stProfile => stProfile.Id).Distinct().ToList();
            
            var xList = _context.GRP_STO.Where(st => st.SiteId == siteId && st.DataStatus == 0 &&
                                               (st.STO_TYPE.ToLower() == "ga installation" || st.STO_TYPE.ToLower() == "ga removal" || st.STO_TYPE.ToLower() == "variable map"))
                                        .OrderBy(st => st.STO_DATE).ThenBy(p => (p.STO_TYPE.ToLower() == "ga installation") ? "1" : (p.STO_TYPE.ToLower() == "variable map")?"2":"3").ToList();

            foreach(var item in xList)
            {
                if (String.Compare(item.STO_TYPE, "ga installation", true) == 0)
                {
                    STInstrument stInst = new STInstrument
                    {
                        Model = item.STO_GA_MODEL,
                        SerialNumber = item.STO_GA_SN,
                        Type = item.STO_TYPE,
                        StoGaProfileId = (int)item.STO_GA_PROF_ID, //nullable
                        // SamplingInt = float.Parse(sampInt),
                        // FlowRate = float.Parse(flowRate),
                        TubeLength = (decimal)item.STO_GA_TUBE_LENGTH,//nullable
                        TubeDiam =  (decimal)item.STO_GA_TUBE_DIAM,////nullable
                        GaVariable = item.STO_GA_VARIABLE,
                        Date = item.STO_DATE
                    };
                    if (item.STO_GA_FLOW_RATE != null)
                    {
                        stInst.FlowRate = (decimal)item.STO_GA_FLOW_RATE; //nullable
                    }
                    else
                    {
                        stInst.FlowRate = -9999;
                    }
                    if (item.STO_GA_SAMPLING_INT != null)
                    {
                        stInst.SamplingInt = (decimal)item.STO_GA_SAMPLING_INT; //nullable
                    }
                    else
                    {
                        stInst.FlowRate = 1;
                    }
                    stInstList.Add(stInst);
                }
                else
                {
                    STInstrument stInst = new STInstrument
                    {
                        Model = item.STO_GA_MODEL,
                        SerialNumber = item.STO_GA_SN,
                        Type = item.STO_TYPE,
                        StoGaProfileId = stInstList[stInstList.Count - 1].StoGaProfileId,
                        SamplingInt = stInstList[stInstList.Count - 1].SamplingInt,//float.Parse(sampInt),
                        FlowRate = stInstList[stInstList.Count - 1].FlowRate, //float.Parse(flowRate),
                        TubeLength = stInstList[stInstList.Count - 1].TubeLength, // float.Parse(tubeLen),
                        TubeDiam = stInstList[stInstList.Count - 1].TubeDiam,
                        GaVariable = item.STO_GA_VARIABLE,
                        Date = item.STO_DATE
                    };
                    stInstList.Add(stInst);
                    
                }
            }
            
            if (profilesId.Count > 0)
            { 
                foreach(var prof in profilesId)
                {

                }
            }

            List<string> resp = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (var profile in stProfile)
            {
                sb.Append(profile.STO_CONFIG);
                sb.Append(",");
                sb.Append(profile.STO_PROF_ID.ToString());
                sb.Append(",");
                sb.Append(profile.STO_PROF_LEVEL.ToString());
                sb.Append(",");
                sb.Append(profile.STO_PROF_HEIGHT.ToString());
                sb.Append(",");
                sb.Append(profile.STO_PROF_EASTWARD_DIST);
                sb.Append(",");
                sb.Append(profile.STO_PROF_NORTHWARD_DIST);
                sb.Append(",");
                sb.Append(profile.STO_PROF_HORIZ_S_POINTS);
                sb.Append(",");
                sb.Append(profile.STO_PROF_BUFFER_VOL);
                sb.Append(",");
                sb.Append(profile.STO_PROF_BUFFER_FLOWRATE);
                sb.Append(",");
                sb.Append(profile.STO_PROF_TUBE_LENGTH);
                sb.Append(",");
                sb.Append(profile.STO_PROF_TUBE_DIAM);
                sb.Append(",");
                sb.Append(profile.STO_PROF_SAMPLING_TIME);
                for (int i = 0; i < 3; ++i) sb.Append(",");
                sb.Append(profile.STO_TYPE);
                for (int i = 0; i < 7; ++i) sb.Append(",");
                sb.Append(profile.STO_DATE_START);
                sb.Append(",");
                sb.Append(profile.STO_DATE_END);
                //sb.Append("\r\n");
                resp.Add(sb.ToString());
                sb.Clear();
            }

            foreach (var inst in stInstList)
            {
                for (int i = 0; i < 12; ++i) sb.Append(",");
                sb.Append(inst.Model);
                sb.Append(",");
                sb.Append(inst.SerialNumber);
                sb.Append(",");
                sb.Append(inst.Type);
                sb.Append(",");
                sb.Append(inst.StoGaProfileId);
                sb.Append(",");
                sb.Append(inst.GaVariable);
                sb.Append(",");
                sb.Append(inst.FlowRate);
                sb.Append(",");
                sb.Append(inst.SamplingInt);
                sb.Append(",");
                sb.Append(inst.TubeLength);
                sb.Append(",");
                sb.Append(inst.TubeDiam);
                sb.Append(",");
                sb.Append(inst.Date);
                sb.Append(",");
                //sb.Append(",");
                //sb.Append("\r\n");
                resp.Add(sb.ToString());
                sb.Clear();
            }

            return resp;
        }

        private void GetFirstDateBeforeLabelByVar(int siteId, string varName, string lDate, ref string instName, ref string instSn, ref string installDate)
        {
            string tt = "";
            var _lastOp = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == siteId && bm.BM_VARIABLE_H_V_R == varName &&
                                                (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "variable map") && bm.BM_DATE.CompareTo(lDate) <= 0)
                                         .OrderByDescending(bm => bm.BM_DATE).FirstOrDefault();
            if (_lastOp != null)
            {
                instName = _lastOp.BM_MODEL;
                instSn= _lastOp.BM_SN;
                installDate= _lastOp.BM_DATE;
            }
            else
            {
                _lastOp = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == siteId && bm.BM_VARIABLE_H_V_R == varName &&
                                                (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "variable map") && bm.BM_DATE.CompareTo(lDate) >= 0)
                                         .OrderBy(bm => bm.BM_DATE).FirstOrDefault();
                if (_lastOp != null)
                {
                    instName = _lastOp.BM_MODEL;
                    instSn = _lastOp.BM_SN;
                    installDate = _lastOp.BM_DATE;
                }
            }

            if (String.Compare(_lastOp.BM_TYPE, "variable map", true) == 0)
            {
                GetInstallDate(siteId, lDate, instName, instSn, ref installDate);
            }
        }

        private void GetInstallDate(int siteId, string lDate, string instName, string instSn, ref string installDate)
        {
            var _instOp = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == siteId && bm.BM_MODEL == instName && bm.BM_SN == instSn &&
                                               (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "removal") && bm.BM_DATE.CompareTo(lDate) <= 0)
                                        .OrderByDescending(bm => bm.BM_DATE).FirstOrDefault();

            if (_instOp != null)
            {
                if (String.Compare(_instOp.BM_TYPE, "removal", true) == 0)
                {
                //    continue;
                }
                installDate = _instOp.BM_DATE;
            
            }
        }

        private decimal? GetSensorHeight(int _siteId, string orName, string _dateStart, string _dateEnd, string instName, string instSn)
        {
            decimal? hh = null;
            var record = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == _siteId && bm.BM_MODEL == instName && bm.BM_SN == instSn &&
                                               (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "variable map") && bm.BM_VARIABLE_H_V_R == orName
                                               && bm.BM_DATE.CompareTo(_dateStart) <= 0)
                                        .OrderByDescending(bm => bm.BM_DATE).ThenByDescending(bm=>bm.BM_HEIGHT).FirstOrDefault();

            if (record != null)
            {
                hh = record.BM_HEIGHT;
                if (!String.IsNullOrEmpty(_dateEnd))
                {
                    if(String.Compare(_dateEnd, record.BM_DATE) < 0)
                    {
                        hh = null;
                    }
                }
            }

            return hh;
        }

        private decimal? FindHeightByOtherVariable(int _siteId, string prefix, string _dateStart, string _dateEnd, string instName, string instSn)
        {
            decimal? hh = null;
            var record = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == _siteId && bm.BM_MODEL == instName && bm.BM_SN == instSn &&
                                               (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "variable map") && bm.BM_VARIABLE_H_V_R.IndexOf(prefix)>=0
                                               && bm.BM_DATE.CompareTo(_dateStart) <= 0)
                                        .OrderBy(bm => bm.BM_DATE).ThenBy(bm => bm.BM_HEIGHT).FirstOrDefault();

            if (record != null)
            {
                hh = record.BM_HEIGHT;
                if (!String.IsNullOrEmpty(_dateEnd))
                {
                    if (String.Compare(_dateEnd, record.BM_DATE) < 0)
                    {
                        hh = null;
                    }
                }
            }
            
            return hh;
        }
    }
}
/*
*  instCommand.CommandText = @"select qual1 as [STO_TYPE],qual15 as [STO_GA_MODEL],qual16 as [STO_GA_SN],
qual17 as [STO_GA_VARIABLE],qual18 as [STO_GA_PROF_ID],qual19 as [STO_GA_FLOW_RATE]
,qual20 as [STO_GA_SAMPLING_INT],qual23 as [STO_GA_TUBE_LENGTH],qual24 as [STO_GA_TUBE_DIAM],
qual33 as [STO_DATE] FROM DataStorageLive where  groupID=2006  and siteId=@sid and qual1 in ('ga installation', 'variable map', 'ga removal')";
if (k < profilesId.Count - 1)
{
instCommand.CommandText += " and (qual33>='" + stProfile.Where(lev => lev.ProfileId == profile).Min(d => d.DateFrom) + "' and qual33<='" + stProfile.Where(lev => lev.ProfileId == profile).Min(d => d.DateFrom) + "'  ) ";
}
instCommand.CommandText += @" ORDER BY STO_DATE ASC,
CASE WHEN qual1 = 'ga installation' THEN '1'
WHEN qual1 = 'variable map' THEN '2'
ELSE qual1 END ASC";*/