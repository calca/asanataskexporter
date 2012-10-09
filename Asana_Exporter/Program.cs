using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;

namespace Asana_Exporter
{

    class Program
    {
        //classes for tasks (thanks, http://json2csharp.com/)
        public class Assignee
        {
            public object id { get; set; }
            public string name { get; set; }
        }

        public class Task
        {
            public object id { get; set; }
            public string name { get; set; }
            public Assignee assignee { get; set; }
            public bool completed { get; set; }
            public string completed_at { get; set; }
            public string notes { get; set; }
            public object due_on { get; set; }
        }

        public class AsanaTaskList
        {
            public List<Task> data { get; set; }
        }

        //classes for projects
        public class Project
        {
            public object id { get; set; }
            public string name { get; set; }
        }

        public class AsanaProjectList
        {
            public List<Project> data { get; set; }
        }

        static int Main(string[] args)
        {
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            var projectRequest = string.Empty;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("You must enter your API key in the App.config");
                return 1; 
            }
            if (!string.IsNullOrWhiteSpace(args[0]))
            {
                projectRequest = args[0];
            }

            //array for the CSV data (well, actually tab seperated)
            List<string> CSVdata = new List<string>();

            //get API key and encode it
            string convertedAPI = EncodeTo64(args[0]+ ":");
            string headerAPIkey = "Basic " + convertedAPI;
            string allProjectsURL = "https://app.asana.com/api/1.0/projects/";

            try
            {
                //open up new webclient and add headers
                var client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers.Add("Authorization", headerAPIkey);
                //call API
                var response = client.DownloadString(new Uri(allProjectsURL));
                //convert JSON
                var convertedresponse = JsonConvert.DeserializeObject<AsanaProjectList>(response);


                var prjs = convertedresponse.data;
                if (!string.IsNullOrWhiteSpace(projectRequest))
                {
                    prjs = prjs
                        .Where(x => { return x.name.CompareTo(projectRequest) == 0; })
                        .ToList();
                }

                //for each project returned, call API again to get tasks
                foreach (Project project in prjs)
                {
                    string projectName = project.name;
                    string projectTasksURL = "https://app.asana.com/api/1.0/projects/" + project.id + "/tasks/?opt_fields=name,notes,due_on,completed,completed_at,assignee,assignee.name";
                    var taskresponse = client.DownloadString(new Uri(projectTasksURL));
                    var convertedtasks = JsonConvert.DeserializeObject<AsanaTaskList>(taskresponse);
                    //now take everything in there and turn it into a line of CSV text
                    foreach (Task task in convertedtasks.data)
                    {
                        string taskName = task.name;
                        string taskNotes = task.notes;
                        //cram notes onto one line so they don't screw the pooch.  Remove any stray \n and \r, which Asana sometimes throws in
                        taskNotes = taskNotes.Replace(Environment.NewLine, " ");
                        taskNotes = taskNotes.Replace("\n", " ");
                        taskNotes = taskNotes.Replace("\r", " ");
                        string taskAssignee;
                        string taskDue;
                        string Completed;
                        string CompletedDate;
                        if (task.assignee != null)
                        {
                            taskAssignee = task.assignee.name;
                        }
                        else
                        {
                            taskAssignee = "";
                        }
                        if (task.due_on != null)
                        {
                            taskDue = task.due_on.ToString();
                        }
                        else
                        {
                            taskDue = "";
                        }
                        if (task.completed != null)
                        {
                            Completed = task.completed.ToString();
                        }
                        else
                        {
                            Completed = "";
                        }
                        if (task.completed_at != null)
                        {
                            CompletedDate = task.completed_at.ToString();
                        }
                        else
                        {
                            CompletedDate = "";
                        }

                        string taskInfo = projectName + "\t" + taskName + "\t" + taskNotes + "\t" + taskAssignee + "\t" + taskDue
                             + "\t" + Completed + "\t" + CompletedDate;
                        CSVdata.Add(taskInfo);
                    }
                }

                //convert list to array
                string[] CSVresults = CSVdata.ToArray();
                //save results
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = path + "\\asana_export.csv";
                File.WriteAllLines(fileName, CSVresults);
            }
            catch (Exception err)
            {
                Console.WriteLine("Error running importer " + err);
                return 0;
            }

            Console.WriteLine("Export complete.");
            return 1; //return 1 if program ran successfully
        }


        //for encoding the API kay
        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
    }
}
