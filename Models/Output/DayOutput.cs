using System;
using System.Xml.Serialization;

namespace Models
{
    
    public class DayOutput
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Date")]
        public DateTime Date { get; set; }

        [XmlElement("Emission")]
        public float? Emission { get; set; }
    }
}