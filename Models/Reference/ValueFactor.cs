using System.Xml.Serialization;

namespace Models
{
    public class ValueFactor
    {
        [XmlElement("High")]
        public float High { get; set; }
        [XmlElement("Medium")]
        public float Medium { get; set; }
        [XmlElement("Low")]
        public float Low { get; set; }
    }
}