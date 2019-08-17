using HelpersLibrary;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace DAL
{
    public class DAL
    {
        public GenerationReport fetchInput()
        {
            try
            {
                string inputPath = ConfigurationManager.AppSettings["InputFilePath"];
                XDocument doc = XDocument.Load(inputPath);
                IEnumerable<XElement> GeneratedReport = doc.Elements();
                GenerationReport inputGenerationReport = GeneratedReport.FirstOrDefault().Deserialize<GenerationReport>();
                return inputGenerationReport;
            }
            catch
            {
                throw;
            }

        }

        public Factors fetchFactors()
        {
            try
            {
                string referenceFilePath = ConfigurationManager.AppSettings["ReferenceFilePath"];
                XDocument refdoc = XDocument.Load(referenceFilePath);
                IEnumerable<XElement> referenceData = refdoc.Elements();
                ReferenceData referenceDeseriaizedData = referenceData.FirstOrDefault().Deserialize<ReferenceData>();
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
                var OutputFilePath = ConfigurationManager.AppSettings["OutputFilePath"];
                var xmlSerializer = new XmlSerializer(typeof(GenerationOutput));
                using (var writer = new StreamWriter(OutputFilePath))
                {
                    xmlSerializer.Serialize(writer, finalGenerationOutput);
                }
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
                var OutputFilePath = ConfigurationManager.AppSettings["OutputFilePath"];
                XDocument existingOutputFile = XDocument.Load(OutputFilePath);
                IEnumerable<XElement> existingOutputData = existingOutputFile.Elements();
                GenerationOutput existingGenerationOutput = existingOutputData.FirstOrDefault().Deserialize<GenerationOutput>();
                return existingGenerationOutput;
            }
            catch
            {
                throw;
            }
        }
    }
}
