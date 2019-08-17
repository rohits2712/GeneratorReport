using System.Xml.Serialization;

namespace Models
{
    public class Generator
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Total")]
        public float Total { get; set; }
    }
}
