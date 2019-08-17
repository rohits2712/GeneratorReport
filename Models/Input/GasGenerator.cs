using System.Xml.Serialization;

namespace Models
{
    public class GasGenerator
    {
        [XmlElement("Name")]

        public string name { get; set; }

        [XmlElement("Generation")]
        public Generation generation { get; set; }

        [XmlElement("EmissionsRating")]

        public float emissionsRating { get; set; }
    }
}