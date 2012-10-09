using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
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
            if (read.CreateCVS(args[0]))
            {
                Console.WriteLine("Export complete.");
                return 1; 
            }

            Console.WriteLine("Error running importer ");
            return 0;
        }

    }
}
