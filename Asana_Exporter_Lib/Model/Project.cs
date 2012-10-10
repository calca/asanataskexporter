using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asana_Exporter_Lib.Model
{
    public class Project
    {
        public object id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

}
