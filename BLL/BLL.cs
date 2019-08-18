
using Models;
using System.Collections.Generic;
using System.Linq;
using DAL;
using System;

namespace BLL
{
    public class BLL : IBLL
    {

        private static List<GeneratorOutput> lsGenOutput = new List<GeneratorOutput>();
        private static List<DayOutput> lsMaxGenOutput = new List<DayOutput>();
        private static GenerationOutput existingGenOutput = new GenerationOutput();

        public GenerationReport fetchInput(string fullPath)
        {
            try
            {
                DAL.IDAL dataAccessLayer = new DAL.DAL();
                //Get Generation Report
                GenerationReport generationReport = dataAccessLayer.fetchInput(fullPath);
                return generationReport;
            }
            catch
            {
                throw;
            }
        }

        public void generateOutput(GenerationReport reportGenerated)
        {
            try
            {
                FetchExistingGenerationOutput();
                DAL.IDAL dataAccessLayer = new DAL.DAL();
                //Get Generation Report
                GenerationOutput finalOutput = new GenerationOutput()
                {
                    totals = TotalGenerationValues(reportGenerated),
                    maxEmissionGenerators = MaxEmissionGenerators(reportGenerated),
                    actualHeatRates = GetActualHeatRates(reportGenerated.coal)
                };
                dataAccessLayer.GenerateOutputFile(finalOutput);
            }
            catch
            {
                throw;
            }


        }

        #region Output file generation methods
        private static Totals TotalGenerationValues(GenerationReport reportGenerated)
        {
            //Calculate Total generation value 
            FetchExistingGenerationOutput();

            foreach (var item in reportGenerated.wind.windGenerator)
            {
                GeneratorOutput windGen = new GeneratorOutput();
                windGen.Name = item.name;
                windGen.Total = item.generation.day.Sum(x => x.energy * x.price * GetGeneratorFactorMapping(item.name).valueFactor);
                SetGeneratorinGenerationOutput(windGen);
            }
            foreach (var item in reportGenerated.gas.gasGenerator)
            {
                GeneratorOutput gasGen = new GeneratorOutput();
                gasGen.Name = item.name;
                gasGen.Total = item.generation.day.Sum(x => x.energy * x.price * GetGeneratorFactorMapping(item.name).valueFactor);
                SetGeneratorinGenerationOutput(gasGen);
            }

            foreach (var item in reportGenerated.coal.coalGenerator)
            {
                GeneratorOutput coalGen = new GeneratorOutput();
                coalGen.Name = item.name;
                coalGen.Total = item.generation.day.Sum(x => x.energy * x.price * GetGeneratorFactorMapping(item.name).valueFactor);
                SetGeneratorinGenerationOutput(coalGen);
            }
            var totals = new Totals();
            //MergeGenerationOutput(totals);
            return new Totals() { GeneratorOutput = lsGenOutput };
            //Calculate Total generation value end 
        }

        private static void SetGeneratorinGenerationOutput(GeneratorOutput genOutput)
        {
            if (lsGenOutput.Exists(x => x.Name.ToLower() == genOutput.Name.ToLower()))
            {
                int index = lsGenOutput.FindIndex(generator => generator.Name.ToLower() == genOutput.Name.ToLower());
                var updatedValue = new GeneratorOutput();
                updatedValue.Name = genOutput.Name;
                updatedValue.Total = lsGenOutput[index].Total + genOutput.Total;
                lsGenOutput[index] = updatedValue;
            }
            else
            {
                lsGenOutput.Add(genOutput);
            }
        }

        private static MaxEmissionGenerators MaxEmissionGenerators(GenerationReport reportGenerated)
        {
            //Highest Daily emission for each day along with emission value
            //Considering only Gas and coal generators as per wind generators do not have emission

            var maxEmissionGenerator = new MaxEmissionGenerators();

            foreach (var generator in reportGenerated.coal.coalGenerator)
            {
                foreach (var day in generator.generation.day)
                {
                    DayOutput d = new DayOutput();
                    d.Name = generator.name;
                    d.Date = day.date;
                    d.Emission = TotalDailyEmissions(day.energy, generator.EmissionsRating, GetGeneratorFactorMapping(generator.name).emissionsFactor);
                    SetMaxEmission(d);
                }

            }

            foreach (var generator in reportGenerated.gas.gasGenerator)
            {
                foreach (var day in generator.generation.day)
                {
                    DayOutput d = new DayOutput();
                    d.Name = generator.name;
                    d.Date = day.date;
                    d.Emission = TotalDailyEmissions(day.energy, generator.emissionsRating, GetGeneratorFactorMapping(generator.name).emissionsFactor);
                    SetMaxEmission(d);
                }

            }

            //Here before grouping we need to add the existing emissions

            var dayOutput = lsMaxGenOutput.GroupBy(x => x.Date)
                                        .OrderBy(g => g.Key);


            var lsfinalDayOutput = new List<DayOutput>();
            foreach (var group in dayOutput)
            {
                foreach (var dayEmission in group.OrderByDescending(c => c.Emission).Take(1))
                {
                    //Console.WriteLine($"On date - {group.Key}, max Emission was from {dayEmission.Name} with a value of {dayEmission.Emission}");
                    lsfinalDayOutput.Add(dayEmission);
                }
            }
            maxEmissionGenerator.Days = lsfinalDayOutput;

            return maxEmissionGenerator;
            //Highest Daily emission for each day along with emission value end
        }

        private static void SetMaxEmission(DayOutput d)
        {
            //adding emission to the existing record in case name is there and date is also same as of record, hence cumulative emission
            if (lsMaxGenOutput.Exists(x => x.Name.ToLower() == d.Name.ToLower() && x.Date.Date == d.Date.Date))
            {
                int index = lsMaxGenOutput.FindIndex(generator => generator.Name.ToLower() == d.Name.ToLower());
                var updatedValue = new DayOutput();
                updatedValue.Date = d.Date;
                updatedValue.Name = d.Name;
                updatedValue.Emission = lsMaxGenOutput[index].Emission + d.Emission; //Adding to existing emission value
                lsMaxGenOutput[index] = updatedValue;
            }
            else if (lsMaxGenOutput.Exists(x => x.Name.ToLower() == d.Name.ToLower() && x.Date.Date != d.Date.Date))
            {//same generator but a new date - emission needs to be added for the generator for the day
                lsMaxGenOutput.Add(d);
            }
            else
            {//new generator or date details
                lsMaxGenOutput.Add(d);
            }
        }

        private ActualHeatRates GetActualHeatRates(Coal coal)
        {
            List<ActualHeatRate> lsActualHeatRates = new List<ActualHeatRate>();

            foreach (var generator in coal.coalGenerator)
            {
                ActualHeatRate actualHeatRate = new ActualHeatRate();
                actualHeatRate.HeatRate = generator.TotalHeatInput / generator.ActualNetGeneration;
                actualHeatRate.Name = generator.name;
                lsActualHeatRates.Add(actualHeatRate);
            }
            return new ActualHeatRates() { actualHeatRate = lsActualHeatRates };
        }

        private static void FetchExistingGenerationOutput()
        {
            DAL.IDAL dataAccessLayer = new DAL.DAL();
            existingGenOutput = dataAccessLayer.ReadOutputFile();
            //assign existing value in list
            lsGenOutput = existingGenOutput.totals.GeneratorOutput;
            lsMaxGenOutput = existingGenOutput.maxEmissionGenerators.Days;
        }

        #endregion

        #region Calculation Helpers
        public static Factors GetFactors()
        {
            try
            {
                DAL.IDAL dataAccessLayer = new DAL.DAL();
                //Get Generation Report
                Factors factors = dataAccessLayer.fetchFactors();
                return factors;
            }
            catch
            {
                throw;
            }

        }
        private static GeneratorFactorMapping GetGeneratorFactorMapping(string Name)
        {
            GeneratorFactorMapping genFactorMapping = new GeneratorFactorMapping();
            genFactorMapping.generatorName = Name;
            Factors factors = GetFactors();
            Name = Name.ToLower();
            if (Name.Contains("wind") && Name.Contains("offshore"))
            {
                genFactorMapping.valueFactor = factors.valueFactor.Low;
                genFactorMapping.emissionsFactor = null;
            }
            else if (Name.Contains("wind") && Name.Contains("onshore"))
            {
                genFactorMapping.valueFactor = factors.valueFactor.High;
                genFactorMapping.emissionsFactor = null;
            }
            else if (Name.Contains("gas"))
            {
                genFactorMapping.valueFactor = factors.valueFactor.Medium;
                genFactorMapping.emissionsFactor = factors.emissionsFactor.Medium;
            }
            else if (Name.Contains("coal"))
            {
                genFactorMapping.valueFactor = factors.valueFactor.Medium;
                genFactorMapping.emissionsFactor = factors.valueFactor.High;
            }
            return genFactorMapping;
        }
        private static double TotalDailyGenerationValue(float Energy, float Price, float valueFactor)
        {
            return Energy * Price * valueFactor;
        }
        private static float? TotalDailyEmissions(float Energy, float EmissionRating, float? EmissionsFactor)
        {
            if (EmissionsFactor == null)
            {
                return 0;
            }
            return Energy * EmissionRating * EmissionsFactor;
        }

        #endregion
    }
}
