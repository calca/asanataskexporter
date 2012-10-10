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

        public bool CreateCVS(FileInfo file, string projectRequest = "")
        {

            //array for the CSV data (well, actually tab seperated)
            List<string> CSVdata = new List<string>();
            var query = new Query(ApiKey);

            try
            {
                var convertedresponse = query.GetProjects();

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
                    var convertedtasks = query.GetTasks(project);
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
                File.WriteAllLines(file.ToString(), CSVresults);
            }
            catch (Exception err)
            {
                return false;
            }

            return true;
        }
    }
}
