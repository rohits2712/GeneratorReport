using HelpersLibrary;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace DataAccessLayer
{
    /// <summary>
    /// Generate object to be used by other layers. Layer to interact outside the project 
    /// </summary>
    public class DataAccessor : IDataAccessor
    {
        /// <summary>
        /// Throws exception in case of errors
        /// </summary>
        /// <param name="fullPath">The xml file which has been placed in Observe folder </param>
        /// <returns>If file is parseable to object returns object, otherwise raises exception handled in Main</returns>
        public GenerationReport ReadInputFile(string fullPath)
        {
            var doc = XDocument.Load(fullPath);
            IEnumerable<XElement> generatedReport = doc.Elements();
            var inputGenerationReport = generatedReport.FirstOrDefault().Deserialize<GenerationReport>();
            return inputGenerationReport;
        }

        public Factors FetchFactors()
        {
            string referenceFilePath = ConfigurationManager.AppSettings["ReferenceFilePath"];
            XDocument referenceDocument = XDocument.Load(referenceFilePath);
            IEnumerable<XElement> referenceData = referenceDocument.Elements();
            var referenceDeseriaizedData = referenceData.FirstOrDefault().Deserialize<ReferenceData>();
            return referenceDeseriaizedData.factors;
        }

        public void GenerateOutputFile(GenerationOutput finalGenerationOutput)
        {
            var outputFilePath = ConfigurationManager.AppSettings["OutputFilePath"];
            var writer = new XmlSerializer(typeof(GenerationOutput));

            using (var file = System.IO.File.Create(outputFilePath))
            {
                writer.Serialize(file, finalGenerationOutput);
            }
        }

        public GenerationOutput ReadOutputFile()
        {
            var outputFilePath = ConfigurationManager.AppSettings["OutputFilePath"];
            if (outputFilePath != null && File.Exists(outputFilePath))
            {
                var existingOutputFile = XDocument.Load(outputFilePath);
                IEnumerable<XElement> existingOutputData = existingOutputFile.Elements();
                var existingGenerationOutput = existingOutputData.FirstOrDefault().Deserialize<GenerationOutput>();
                return existingGenerationOutput;
            }
            return null;
        }
    }
}
