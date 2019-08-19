
using Models;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer;
using System;

namespace BusinessLayer
{
    public class ReportProcessor : IReportProcessor
    {
        private readonly IDataAccessor _dataAccessor;
        private List<GeneratorOutput> _lsFinalGenOutput;
        private List<DayOutput> _lsFinalMaxGenOutput;
        private GenerationOutput _existingGenOutput;

        public ReportProcessor()
        {
            _dataAccessor = new DataAccessor();
            _lsFinalGenOutput = new List<GeneratorOutput>();
            _lsFinalMaxGenOutput = new List<DayOutput>();
            _existingGenOutput = new GenerationOutput();
        }


        public GenerationReport ReadInputFile(string fullPath)
        {
            //Get Generation Report
            GenerationReport generationReport = _dataAccessor.ReadInputFile(fullPath);
            return generationReport;
        }

        public void GenerateOutputFile(GenerationReport incomingGeneratedReport)
        {
            FetchExistingGenerationOutput();
            //Get Generation Report
            var finalOutput = new GenerationOutput()
            {
                totals = TotalGeneratorsOutputPerDay(incomingGeneratedReport),
                maxEmissionGenerators = MaxEmissionGeneratorPerDay(incomingGeneratedReport),
                actualHeatRates = GetActualHeatRates(incomingGeneratedReport.coal)
            };
            _dataAccessor.GenerateOutputFile(finalOutput);
        }

        #region Output file generation methods
        private Totals TotalGeneratorsOutputPerDay(GenerationReport reportGenerated)
        {
            foreach (var item in reportGenerated.wind.windGenerator)
            {
                var windGen = new GeneratorOutput
                {
                    Name = item.name,
                    Total = item.generation.day.Sum(x => x.energy * x.price * GetGeneratorFactorMapping(item.name).valueFactor)
                };
                SetGeneratorInOutputGenerationList(windGen);
            }
            foreach (var item in reportGenerated.gas.gasGenerator)
            {
                var gasGen = new GeneratorOutput
                {
                    Name = item.name,
                    Total = item.generation.day.Sum(x => x.energy * x.price * GetGeneratorFactorMapping(item.name).valueFactor)
                };
                SetGeneratorInOutputGenerationList(gasGen);
            }

            foreach (var item in reportGenerated.coal.coalGenerator)
            {
                var coalGen = new GeneratorOutput
                {
                    Name = item.name,
                    Total = item.generation.day.Sum(x => x.energy * x.price * GetGeneratorFactorMapping(item.name).valueFactor)
                };
                SetGeneratorInOutputGenerationList(coalGen);
            }
            var totals = new Totals();
            return new Totals() { GeneratorOutput = _lsFinalGenOutput };
        }

        private void SetGeneratorInOutputGenerationList(GeneratorOutput incomingGeneratorOutput)
        {
            if (_lsFinalGenOutput.Any(x => x.Name.Equals(incomingGeneratorOutput.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                var index = _lsFinalGenOutput.FindIndex(generator => string.Equals(generator.Name, incomingGeneratorOutput.Name, StringComparison.CurrentCultureIgnoreCase));
                var updatedValue = new GeneratorOutput
                {
                    Name = incomingGeneratorOutput.Name,
                    Total = _lsFinalGenOutput[index].Total + incomingGeneratorOutput.Total
                };
                _lsFinalGenOutput[index] = updatedValue;
            }
            else
            {
                _lsFinalGenOutput.Add(incomingGeneratorOutput);
            }
        }
        /// <summary>
        /// Method to find out max emission each day and the generator name.
        /// Only Gas and coal generators as per wind generators do not have emission
        /// </summary>
        /// <param name="incomingGeneratedReport"></param>
        /// <returns></returns>
        private MaxEmissionGenerators MaxEmissionGeneratorPerDay(GenerationReport incomingGeneratedReport)
        {
            var maxEmissionGenerator = new MaxEmissionGenerators();
            foreach (var generator in incomingGeneratedReport.coal.coalGenerator)
            {
                foreach (var day in generator.generation.day)
                {
                    var coalDayOutput = new DayOutput
                    {
                        Name = generator.name,
                        Date = day.date,
                        Emission = TotalDailyEmissions(day.energy, generator.EmissionsRating,
                            GetGeneratorFactorMapping(generator.name).emissionsFactor)
                    };
                    SetMaxEmission(coalDayOutput);
                }

            }

            foreach (var generator in incomingGeneratedReport.gas.gasGenerator)
            {
                foreach (var day in generator.generation.day)
                {
                    var gasdayOutput = new DayOutput
                    {
                        Name = generator.name,
                        Date = day.date,
                        Emission = TotalDailyEmissions(day.energy, generator.emissionsRating,
                            GetGeneratorFactorMapping(generator.name).emissionsFactor)
                    };
                    SetMaxEmission(gasdayOutput);
                }

            }
            var dayOutput = _lsFinalMaxGenOutput.GroupBy(x => x.Date)
                                        .OrderBy(g => g.Key);

            var lsfinalDayOutput = dayOutput.SelectMany(@group => @group.OrderByDescending(c => c.Emission).Take(1)).ToList();
            maxEmissionGenerator.Days = lsfinalDayOutput;

            return maxEmissionGenerator;
        }

        /// <summary>
        /// SetEmission - Add emission for each generator in existing list 
        /// If name and date is a match, this means cumulative emission and emission for generator needs to be updated in final list.
        /// if name is same but date different, then new entry for the generator
        /// else it is a new entry
        /// </summary>
        /// <param name="incomingDayOutput"></param>
        private void SetMaxEmission(DayOutput incomingDayOutput)
        {            
            if (_lsFinalMaxGenOutput.Exists(x =>
                string.Equals(x.Name.ToLower(), incomingDayOutput.Name.ToLower(), StringComparison.CurrentCultureIgnoreCase) && x.Date.Date == incomingDayOutput.Date.Date))
            {
                var index = _lsFinalMaxGenOutput.FindIndex(generator => string.Equals(generator.Name.ToLower(),
                    incomingDayOutput.Name.ToLower(), StringComparison.CurrentCultureIgnoreCase));
                var updatedValue = new DayOutput
                {
                    Date = incomingDayOutput.Date,
                    Name = incomingDayOutput.Name,
                    Emission = _lsFinalMaxGenOutput[index].Emission + incomingDayOutput.Emission
                };
                _lsFinalMaxGenOutput[index] = updatedValue;
            }
            else if (_lsFinalMaxGenOutput.Exists(x => String.Equals(x.Name, incomingDayOutput.Name, StringComparison.CurrentCultureIgnoreCase) && x.Date.Date != incomingDayOutput.Date.Date))
            {
                _lsFinalMaxGenOutput.Add(incomingDayOutput);
            }
            else
            {
                _lsFinalMaxGenOutput.Add(incomingDayOutput);
            }
        }

        private ActualHeatRates GetActualHeatRates(Coal coal)
        {
            var lsActualHeatRates = new List<ActualHeatRate>();

            foreach (var generator in coal.coalGenerator)
            {
                var actualHeatRate = new ActualHeatRate
                {
                    HeatRate = generator.TotalHeatInput / generator.ActualNetGeneration, Name = generator.name
                };
                lsActualHeatRates.Add(actualHeatRate);
            }
            return new ActualHeatRates() { actualHeatRate = lsActualHeatRates };
        }

        /// <summary>
        /// Read from existing output file and prepare model to merge any changes
        /// </summary>
        private void FetchExistingGenerationOutput()
        {
            var existingFileGenOutput = _dataAccessor.ReadOutputFile();
            if (existingFileGenOutput != null)
            {
                _lsFinalGenOutput = existingFileGenOutput.totals.GeneratorOutput;
                _lsFinalMaxGenOutput = existingFileGenOutput.maxEmissionGenerators.Days;
            }
        }

        #endregion

        #region Calculation Helpers
        public static Factors GetFactors()
        {
            IDataAccessor dataAccessLayer = new DataAccessor();
            //Get Generation Report
            var factors = dataAccessLayer.FetchFactors();
            return factors;
        }
        private static GeneratorFactorMapping GetGeneratorFactorMapping(string name)
        {
            var genFactorMapping = new GeneratorFactorMapping {generatorName = name};
            var factors = GetFactors();
            name = name.ToLower();
            if (name.Contains("wind") && name.Contains("offshore"))
            {
                genFactorMapping.valueFactor = factors.valueFactor.Low;
                genFactorMapping.emissionsFactor = null;
            }
            else if (name.Contains("wind") && name.Contains("onshore"))
            {
                genFactorMapping.valueFactor = factors.valueFactor.High;
                genFactorMapping.emissionsFactor = null;
            }
            else if (name.Contains("gas"))
            {
                genFactorMapping.valueFactor = factors.valueFactor.Medium;
                genFactorMapping.emissionsFactor = factors.emissionsFactor.Medium;
            }
            else if (name.Contains("coal"))
            {
                genFactorMapping.valueFactor = factors.valueFactor.Medium;
                genFactorMapping.emissionsFactor = factors.valueFactor.High;
            }
            return genFactorMapping;
        }

        private static float? TotalDailyEmissions(float energy, float emissionRating, float? emissionsFactor)
        {
            if (emissionsFactor == null)
            {
                return 0;
            }
            return energy * emissionRating * emissionsFactor;
        }

        #endregion
    }
}
