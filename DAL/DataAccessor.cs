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
    public class DataAccessor : IDataAccessor
    {
        public GenerationReport ReadInputFile(string fullPath)
        {
            try
            {
                var doc = XDocument.Load(fullPath);
                IEnumerable<XElement> generatedReport = doc.Elements();
                var inputGenerationReport = generatedReport.FirstOrDefault().Deserialize<GenerationReport>();
                return inputGenerationReport;
            }
            catch
            {
                throw;
            }

        }

        public Factors FetchFactors()
        {
            try
            {
                string referenceFilePath = ConfigurationManager.AppSettings["ReferenceFilePath"];
                XDocument referenceDocument = XDocument.Load(referenceFilePath);
                IEnumerable<XElement> referenceData = referenceDocument.Elements();
                var referenceDeseriaizedData = referenceData.FirstOrDefault().Deserialize<ReferenceData>();
                return referenceDeseriaizedData.factors;
            }
            catch
            {
                throw;
            }
        }

        public void GenerateOutputFile(GenerationOutput finalGenerationOutput)
        {
            try
            {
                var outputFilePath = ConfigurationManager.AppSettings["OutputFilePath"];
                var writer = new XmlSerializer(typeof(GenerationOutput));

                using (var file = System.IO.File.Create(outputFilePath))
                {
                    writer.Serialize(file, finalGenerationOutput);
                }
               
                //var doc = new XDocument();
                //using (var writer = doc.CreateWriter())
                //{
                //    xmlSerializer.Serialize(writer, finalGenerationOutput);
                //}
                //using (var writer = new StreamWriter(outputFilePath))
                //{
                //    xmlSerializer.Serialize(writer, finalGenerationOutput);
                //}
            }
            catch
            {
                throw;
            }
        }

        public GenerationOutput ReadOutputFile()
        {
            try
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
            catch
            {
                throw;
            }
        }
    }
}
