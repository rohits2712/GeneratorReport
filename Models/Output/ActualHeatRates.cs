using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    public class ActualHeatRates
    {
        [XmlElement("ActualHeatRate")]
        public List<ActualHeatRate> actualHeatRate { get; set; }

    }
    public class ActualHeatRate
    {
        public string Name { get; set; }
        public float HeatRate { get; set; }

    }

}