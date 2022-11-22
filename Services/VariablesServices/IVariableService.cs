namespace Icos5.core6.Services.VariablesServices
{
    public interface IVariableService
    {
        string GetAggregatedVarList(string site);
        string GetAggregatedVarListByName(string site, string variable, string startDate);
        string GetVarsRenamedList(string site, string timestamp);
        string GetVarsRenamedAggregatedInRange(string site, string startYear, string endYear);
    }
}
