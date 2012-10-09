using Asana_Exporter_Lib.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Asana_Exporter_Lib
{
    public class ReadTasks
    {
        private string ApiKey { get; set; }

        public ReadTasks(string apiKey)
        {
            ApiKey = apiKey;
        }

        public bool CreateCVS(string projectRequest = "")
        {

            //array for the CSV data (well, actually tab seperated)
            List<string> CSVdata = new List<string>();

            //get API key and encode it
            string convertedAPI = EncodeTo64(ApiKey + ":");
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
                    foreach (Asana_Exporter_Lib.Model.Task task in convertedtasks.data)
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
                return false;
            }

            return true;
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
