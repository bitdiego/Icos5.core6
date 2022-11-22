namespace Icos5.core6.Models.Sensors
{
    public class STInstrument : Instrument
    {
        public decimal FlowRate { get; set; }
        public decimal TubeLength { get; set; }
        public decimal TubeDiam { get; set; }
        public string GaVariable { get; set; } = "";
        public int StoGaProfileId { get; set; }
    }
}
