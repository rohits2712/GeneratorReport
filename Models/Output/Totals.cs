using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    public class Totals
    {
        [XmlElement("Generator")]
        public List<GeneratorOutput> GeneratorOutput { get; set; }

        public Totals()
        {
            GeneratorOutput = new List<GeneratorOutput>();
        }

    }
}