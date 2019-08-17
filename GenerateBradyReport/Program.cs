using Models;
using System;
using BLL;

namespace GenerateBradyReport
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BLL.BLL businessLogicLayer = new BLL.BLL();
                //Get Generation Report
                GenerationReport generationReport = businessLogicLayer.fetchInput();

                if (generationReport != null)
                {
                    /* Generate single Output Report - 
                    Please note if multiple files are placed one after another with same name - GenerationReport.xml.
                    The ouput file gets overridden*/
                    businessLogicLayer.generateOutput(generationReport);
                }
                else {
                    Console.WriteLine($"Invalid Report Input");
                }                

            }
            catch (Exception ex) {
                Console.WriteLine($"Exception occurred - Message - {ex.Message}");
                Console.WriteLine($"Exception occurred - Stacktrace - {ex.StackTrace}");
            }

            Console.ReadLine();
        }

      

     

   

    }
    
}
