using Icos5.core6.DbManager;
using Icos5.core6.Models.Sensors;
using IcosClassLibrary.Models;

namespace Icos5.core6.Services.SensorServices
{
    public class EFSensorService : ISensorService
    {
        private readonly IcosDbContext _context;

        public EFSensorService(IcosDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Frequency> GetFrequencyList(int siteId, int logger_id, int file_id, string type)
        {
            List<Frequency> efList = new List<Frequency>();
            switch (type.ToLower())
            {
                case "bm":
                    efList = _context.GRP_BM.Where(bm => bm.SiteId == siteId && bm.DataStatus == 0 && bm.BM_LOGGER == logger_id && bm.BM_FILE == file_id
                                                   && bm.BM_SAMPLING_INT != null && (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "variable map"))
                                                   .OrderBy(bm => bm.BM_VARIABLE_H_V_R).OrderByDescending(bm => bm.BM_DATE).OrderByDescending(bm => bm.BM_DATE_START)
                                                   .Select(xm => new Frequency { Name = xm.BM_VARIABLE_H_V_R, SamplingInt = xm.BM_SAMPLING_INT })
                                                   .ToList();
                    break;
                case "st":///TO UPDATE!!!!!
                    efList = _context.GRP_STO.Where(st => st.SiteId == siteId && st.DataStatus == 0 && st.STO_LOGGER == logger_id && st.STO_FILE == file_id
                                                   && st.STO_GA_SAMPLING_INT != null && (st.STO_TYPE.ToLower() == "ga installation" || st.STO_TYPE.ToLower() == "variable map"))
                                                   .OrderBy(st => st.STO_GA_VARIABLE).OrderByDescending(st => st.STO_DATE).OrderByDescending(st => st.STO_DATE_START)
                                                   .Select(xm => new Frequency { Name = xm.STO_GA_VARIABLE, SamplingInt = xm.STO_GA_SAMPLING_INT }).ToList();
                    break;
                case "ec":
                    efList = _context.GRP_EC.Where(ec => ec.SiteId == siteId && ec.DataStatus == 0 && ec.EC_LOGGER == logger_id && ec.EC_FILE == file_id
                                                   && ec.EC_SAMPLING_INT != null && (ec.EC_TYPE.ToLower() == "installation" || ec.EC_TYPE.ToLower() == "maintenance"))
                                                   .OrderBy(ec => ec.EC_MODEL).OrderByDescending(ec => ec.EC_DATE).OrderByDescending(ec => ec.EC_DATE_START)
                                                   .Select(xm => new Frequency { Name = xm.EC_MODEL, SamplingInt = xm.EC_SAMPLING_INT }).ToList();
                    break;
            }
            return efList;
        }

        public IEnumerable<GRP_BM> GetBmConfig(int siteId)
        {
            var results = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == siteId && (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "variable map")
                                                && (bm.BM_VARIABLE_H_V_R.StartsWith("TA") || bm.BM_VARIABLE_H_V_R.StartsWith("RH") || bm.BM_VARIABLE_H_V_R.StartsWith("PA")))
                                                .OrderBy(bm => bm.BM_VARIABLE_H_V_R).ToList();
            return results;
        }

        public IEnumerable<GRP_STO> GetStoConfig(int siteId)
        {
            var stoList = _context.GRP_STO.Where(st => st.DataStatus == 0 && st.SiteId == siteId).ToList();
            return stoList;
        }

        public IEnumerable<string> GetVarsList(int id, int logger_id, int file_id, string type, IUtilityServices _service)
        {
            List<string> vars = new List<string>();
            switch (type.ToLower())
            {
                case "bm":
                    var bmList = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == id && bm.BM_FILE == file_id && bm.BM_LOGGER == logger_id
                                                 && bm.BM_VARIABLE_H_V_R != null && (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "variable map"))
                                                .OrderBy(bm => bm.BM_VARIABLE_H_V_R).ToList();
                    foreach (var bm in bmList)
                    {
                        if (_service.IsMeteoSensorInstalled(bm.BM_MODEL, bm.BM_SN, id))
                        {
                            vars.Add(bm.BM_VARIABLE_H_V_R);
                        }
                    }
                    break;
                case "st":
                    var stList = _context.GRP_STO.Where(st => st.DataStatus == 0 && st.SiteId == id && st.STO_FILE == file_id && st.STO_LOGGER == logger_id
                                                 && st.STO_GA_VARIABLE != null).OrderBy(st => st.STO_GA_VARIABLE).ToList();
                    foreach (var st in stList)
                    {
                        if (_service.IsStoSensorInstalled(st.STO_GA_MODEL, st.STO_GA_SN, id))
                        {
                            vars.Add(st.STO_GA_VARIABLE);
                        }
                    }
                    break;
                case "ec":

                    break;
            }

            return vars;
        }
       /* public IEnumerable<GRP_STO> GetStoVarsList(int siteId, int logger_id, int file_id)
        {
            throw new NotFiniteNumberException();
        }
       */
        public IEnumerable<string> GetVarsListInDay(int siteId, int logger_id, int file_id, string timeStamp, string type, IUtilityServices _service)
        {
            List<string> vars = new List<string>();

            switch (type.ToLower())
            {
                case "bm":
                    var bmList = _context.GRP_BM.Where(bm => bm.DataStatus == 0 && bm.SiteId == siteId && bm.BM_FILE == file_id && bm.BM_LOGGER == logger_id
                                                 && bm.BM_VARIABLE_H_V_R != null && (bm.BM_TYPE.ToLower() == "installation" || bm.BM_TYPE.ToLower() == "variable map")
                                                 && bm.BM_DATE.CompareTo(timeStamp) <= 0).Distinct().OrderBy(bm => bm.BM_DATE).ToList();
                    foreach (var bm in bmList)
                    {
                        var _xbm = _service.GetLastMeteoSensorOperationByVarAndDate(bm.BM_MODEL, bm.BM_SN, bm.BM_VARIABLE_H_V_R, timeStamp, siteId);
                        if (String.Compare(_xbm.BM_TYPE, "installation", true) == 0 || String.Compare(_xbm.BM_TYPE, "variable map", true) == 0)
                        {
                            if (_xbm.BM_LOGGER != null && _xbm.BM_FILE != null)
                            {
                                if (_xbm.BM_LOGGER != logger_id || _xbm.BM_FILE != file_id)
                                {
                                    continue;
                                }
                                if (String.Compare(_xbm.BM_VARIABLE_H_V_R, bm.BM_VARIABLE_H_V_R, true) != 0)
                                {
                                    continue;
                                }
                                if (!vars.Contains(bm.BM_VARIABLE_H_V_R))
                                {
                                    vars.Add(bm.BM_VARIABLE_H_V_R);
                                }
                            }
                            else
                            {
                                if (!vars.Contains(bm.BM_VARIABLE_H_V_R))
                                {
                                    vars.Add(bm.BM_VARIABLE_H_V_R);
                                }
                            }
                        }
                    }

                    break;
                case "st":

                    var stList = _context.GRP_STO.Where(st => st.DataStatus == 0 && st.SiteId == siteId && st.STO_FILE == file_id && st.STO_LOGGER == logger_id
                                                 && st.STO_GA_VARIABLE != null && (st.STO_TYPE.ToLower() == "ga installation" || st.STO_TYPE.ToLower() == "variable map")
                                                 && st.STO_DATE.CompareTo(timeStamp) <= 0).Distinct().OrderBy(st => st.STO_DATE).ToList();
                    foreach (var st in stList)
                    {
                        var _xst = _service.GetLastStoSensorOperationByVarAndDate(st.STO_GA_MODEL, st.STO_GA_SN, st.STO_GA_VARIABLE, timeStamp, siteId);
                        if (_xst.STO_LOGGER != null && _xst.STO_FILE != null)
                        {
                            if (_xst.STO_LOGGER != logger_id || _xst.STO_FILE != file_id)
                            {
                                continue;
                            }
                            if (String.Compare(_xst.STO_GA_VARIABLE, st.STO_GA_VARIABLE, true) != 0)
                            {
                                continue;
                            }
                            if (!vars.Contains(st.STO_GA_VARIABLE))
                            {
                                vars.Add(st.STO_GA_VARIABLE);
                            }
                        }
                        else
                        {
                            if (!vars.Contains(st.STO_GA_VARIABLE))
                            {
                                vars.Add(st.STO_GA_VARIABLE);
                            }
                        }
                    }
                    break;
            }
            return vars;
        }
    
    }
}
