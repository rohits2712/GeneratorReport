using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    public class MaxEmissionGenerators
    {[XmlElement("Day")]
        public List<DayOutput> Days { get; set; }
        public MaxEmissionGenerators()
        {
            Days = new List<DayOutput>();
        }
    }
}