using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Models
{
    public class GenerationOutput
    {
        [XmlElement("Totals")]
        public Totals totals { get; set; }
        [XmlElement("MaxEmissionGenerators")]

        public MaxEmissionGenerators maxEmissionGenerators { get; set; }
        [XmlElement("ActualHeatRates")]

        public ActualHeatRates actualHeatRates { get; set; }
        
    }
}
