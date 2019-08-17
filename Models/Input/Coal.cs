using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    public class Coal
    {
        [XmlElement("CoalGenerator")]
        public List<CoalGenerator> coalGenerator { get; set; }
    }
}