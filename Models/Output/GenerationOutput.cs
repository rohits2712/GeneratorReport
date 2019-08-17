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
        public Totals totals { get; set; }
        public MaxEmissionGenerators maxEmissionGenerators { get; set; }
        public ActualHeatRates actualHeatRates { get; set; }
        
    }
}
