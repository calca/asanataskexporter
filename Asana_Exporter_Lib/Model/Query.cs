using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Asana_Exporter_Lib.Model
{
    public class Query
    {
        private WebClient Client { get; set; }

        public Query(string apiKey)
        {
            Client = new WebClient();

            string convertedAPI = EncodeTo64(apiKey + ":");
            string headerAPIkey = "Basic " + convertedAPI;
            Client.Encoding = System.Text.Encoding.UTF8;
            Client.Headers.Add("Authorization", headerAPIkey);
        }

        public AsanaProjectList GetProjects(){
            string allProjectsURL = "https://app.asana.com/api/1.0/projects/";
            var response = Client.DownloadString(new Uri(allProjectsURL));
            var convertedresponse = JsonConvert.DeserializeObject<AsanaProjectList>(response);
            return convertedresponse;
        }

        public AsanaTaskList GetTasks(Project project)
        {
            string projectTasksURL = "https://app.asana.com/api/1.0/projects/" 
                + project.id 
                + "/tasks/?opt_fields=name,notes,due_on,completed,completed_at,assignee,assignee.name";

            var taskresponse = Client.DownloadString(new Uri(projectTasksURL));
            var convertedtasks = JsonConvert.DeserializeObject<AsanaTaskList>(taskresponse);
            return convertedtasks;
        }

        //for encoding the API kay
        private string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
    }
}
