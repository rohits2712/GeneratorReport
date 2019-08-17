
using System.Xml.Serialization;

namespace Models
{
    public class WindGenerator
    {
        [XmlElement("Name")]
        public string name { get; set; }

        [XmlElement("Generation")]

        public Generation generation { get; set; }
        public string Location { get; set; }

        public WindGenerator(string name)
        {
            this.name = name;
            this.generation = new Generation();

        }
        public WindGenerator()
        {

        }
    }
}