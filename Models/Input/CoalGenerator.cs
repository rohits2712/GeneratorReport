using System.Xml.Serialization;

namespace Models
{
    public class CoalGenerator
    {
        [XmlElement("Name")]
        public string name { get; set; }
        [XmlElement("Generation")]
        public Generation generation { get; set; }
        public float TotalHeatInput { get; set; }
        public float ActualNetGeneration { get; set; }
        public float EmissionsRating { get; set; }

    }
}