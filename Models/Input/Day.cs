using System;
using System.Xml.Serialization;

namespace Models
{
    
    public class Day
    {
        [XmlElement("Date")]
        public DateTime date { get; set; }

        [XmlElement("Energy")]
        public float energy { get; set; }

        [XmlElement("Price")]
        public float price { get; set; }
    }
}