using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    public class Wind
    {
        [XmlElement("WindGenerator")]
        public List<WindGenerator> windGenerator { get; set; }
    }
}