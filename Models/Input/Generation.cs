using System.Collections.Generic;
using System.Xml.Serialization;
namespace Models
{

    public class Generation
    {
        public Generation()
        {
            day = new List<Day>();
        }
        
        [XmlElement("Day")]
        public List<Day> day { get; set; }
    }
}