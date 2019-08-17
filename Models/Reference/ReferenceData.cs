using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Models
{
   public class ReferenceData
    {
        [XmlElement("Factors")]
        public Factors factors { get; set; }
    }
}
