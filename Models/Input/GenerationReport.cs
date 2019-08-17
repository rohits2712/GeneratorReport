
using System.Xml.Serialization;

namespace Models
{
    public class GenerationReport
    {
        [XmlElement("Wind")]
        public Wind wind { get; set; }

        [XmlElement("Gas")]

        public Gas gas { get; set; }

        [XmlElement("Coal")]

        public Coal coal { get; set; }
    }
}
