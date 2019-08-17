using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    public class Gas
    {
        [XmlElement("GasGenerator")]
        public List<GasGenerator> gasGenerator { get; set; }
    }
}