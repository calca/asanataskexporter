using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asana_Exporter_Lib.Model
{
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
}
