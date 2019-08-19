using Models;
using System;
using System.IO;
using System.Configuration;
using BusinessLayer;

namespace GenerateBradyReport
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var observeDirectoryPath = ConfigurationManager.AppSettings["observeDirectoryPath"];
                var watcher = new FileSystemWatcher(observeDirectoryPath);
                watcher.EnableRaisingEvents = true;
                watcher.Created += Watcher_Created;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.Filter = "*.xml";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred - Message - {ex.Message}");
                Console.WriteLine($"Exception occurred - Stacktrace - {ex.StackTrace}");
            }

            Console.ReadLine();
        }

        /// <summary>
        /// This method listens to the observable folder activities indefinitely and processes the file as soon as they are placed
        /// Generate single Output Report  
        /// Please note if multiple xml files are placed one after another with same name - GenerationReport.xml.
        /// The output file- GenerationOutput.xml gets overwritten to have the latest data as per logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">event arguments for the folder activity </param>
        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File {e.Name} created at {DateTime.Now}");
            IReportProcessor businessLogicLayer = new ReportProcessor();
            var generationReport = businessLogicLayer.ReadInputFile(e.FullPath);
            if (generationReport != null)
                businessLogicLayer.GenerateOutputFile(generationReport);
            else
                Console.WriteLine($"Invalid Report Input");
        }





    }

}
