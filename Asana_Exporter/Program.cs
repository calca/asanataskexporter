using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Configuration;
using Asana_Exporter_Lib.Model;
using Asana_Exporter_Lib;

namespace Asana_Exporter
{

    class Program
    {

        static int Main(string[] args)
        {
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            var projectRequest = string.Empty;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("You must enter your API key in the App.config");
                return 1; 
            }

            var read = new ReadTasks(apiKey);

            var name = "asana_export" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            string fileName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                name);
            FileInfo file = new FileInfo(fileName);

            if (read.CreateCVS(file,args[0]))
            {
                Console.WriteLine("Export complete.");
                return 1; 
            }

            Console.WriteLine("Error running importer ");
            return 0;
        }

    }
}
