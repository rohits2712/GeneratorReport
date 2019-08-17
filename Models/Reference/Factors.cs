using System.Xml.Serialization;

namespace Models
{
    public class Factors
    {
        [XmlElement("ValueFactor")]
        public ValueFactor valueFactor { get; set; }
        [XmlElement("EmissionsFactor")]
        public EmissionsFactor emissionsFactor { get; set; }
    }
}