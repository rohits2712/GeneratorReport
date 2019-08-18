using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IDataAccessor
    {
        GenerationReport ReadInputFile(string fullPath);
        Factors FetchFactors();
        void GenerateOutputFile(GenerationOutput finalGenerationOutput);
        GenerationOutput ReadOutputFile();

    }
}
