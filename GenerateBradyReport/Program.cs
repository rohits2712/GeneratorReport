using Models;
using System;
using BLL;
using System.IO;
using System.Configuration;

namespace GenerateBradyReport
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string observeDirectoryPath = ConfigurationManager.AppSettings["observeDirectoryPath"];
                FileSystemWatcher watcher = new FileSystemWatcher(observeDirectoryPath);
                watcher.EnableRaisingEvents = true;
                watcher.Created += Watcher_Created;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.Filter = "*.xml";
 

            }
            catch (Exception ex) {
                Console.WriteLine($"Exception occurred - Message - {ex.Message}");
                Console.WriteLine($"Exception occurred - Stacktrace - {ex.StackTrace}");
            }

            Console.ReadLine();
        }
        
        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {                
                Console.WriteLine($"File {e.Name} created at {DateTime.Now}");
                
                BLL.IBLL businessLogicLayer = new BLL.BLL();

                //Get Generation Report
                GenerationReport generationReport = businessLogicLayer.fetchInput(e.FullPath);

                if (generationReport != null)
                {
                    /* Generate single Output Report - 
                    Please note if multiple files are placed one after another with same name - GenerationReport.xml.
                    The ouput file gets overwritten*/
                    businessLogicLayer.generateOutput(generationReport);
                }
                else
                {
                    Console.WriteLine($"Invalid Report Input");
                }
            }
            catch {
                throw;
            }
        }





    }

}
