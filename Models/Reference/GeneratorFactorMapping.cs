using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GeneratorFactorMapping
    {
        public string generatorName { get; set; }
        public float valueFactor { get; set; }
        public float? emissionsFactor { get; set; }
    }
}
