using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asana_Exporter_Lib;

namespace Asana_Exporter_Tests
{
    [TestClass]
    public class Exporter_Lib
    {
        [TestMethod]
        public void CreateCVS()
        {
            var r = new ReadTasks("dYdEDDm.j6Z2c5HeN7S4w10AvrhGqFtc");
            r.CreateCVS();
        }

        [TestMethod]
        public void CreateCVSForProject()
        {
            var r = new ReadTasks("dYdEDDm.j6Z2c5HeN7S4w10AvrhGqFtc");
            r.CreateCVS("Ferrero - DCP");
        }
    }
}
