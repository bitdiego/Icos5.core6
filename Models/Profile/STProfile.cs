namespace Icos5.core6.Models.Profile
{
    public class STProfile
    {
        #region Profile section
        public int ProfileId { get; set; }
        public int ProfileLevelId { get; set; }
        public decimal ProfileHeight { get; set; }
        public string ProfileConfiguration { get; set; }
        public decimal ProfileEastDist { get; set; }
        public decimal ProfileNorthDist { get; set; }
        public int ProfileHorizSPoints { get; set; }
        public decimal ProfileBufferVolume { get; set; }
        public decimal ProfileBufferFlowRate { get; set; }
        public decimal ProfileTubeLength { get; set; }
        public decimal ProfileTubeDiam { get; set; }
        public decimal ProfileSamplingTime { get; set; }
        public string ProfileType { get; set; }     //added on 23-12-2020
        #endregion

        #region GA_section
        public string StoGaModel { get; set; }
        public string StoGaSerialNumber { get; set; }
        public string StoGaVariable { get; set; }
        public int StoGaProfileId { get; set; }
        public decimal StoGaSamplInt { get; set; }
        public decimal StoGaFlowRate { get; set; }
        public decimal StoGaTubeLen { get; set; }
        public decimal StoGaTubeDiam { get; set; }

        /*public List<string> StoVariables { get; set; }
        public List<string> StoGaFlowRates { get; set; }
        public List<string> StoGaSamplInts { get; set; }
        public List<string> StoGaTubeLens { get; set; }
        public List<string> StoGaTubeDiams { get; set; }
         * */
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        #endregion
        public STProfile()
        {
            //ProfileLevels = new List<ProfileLevel>();
            /*StoVariables = new List<string>();
            StoGaFlowRates = new List<string>();
            StoGaSamplInts = new List<string>();
            StoGaTubeLens = new List<string>();
            StoGaTubeDiams = new List<string>();*/
        }
    }
}
