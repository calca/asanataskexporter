using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asana_Exporter_Lib;
using System.IO;

namespace Asana_Exporter_Tests
{
    [TestClass]
    public class Exporter_Lib
    {
        [TestMethod]
        public void CreateCVS()
        {
            var r = new ReadTasks("dYdEDDm.j6Z2c5HeN7S4w10AvrhGqFtc");

            var name = "asana_export" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            string fileName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                name);
            FileInfo file = new FileInfo(fileName);

            Assert.IsTrue(r.CreateCVS(file));
        }

        [TestMethod]
        public void CreateCVSForProject()
        {
            var r = new ReadTasks("dYdEDDm.j6Z2c5HeN7S4w10AvrhGqFtc");

            var name = "asana_export" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            string fileName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                name);
            FileInfo file = new FileInfo(fileName);

            Assert.IsTrue(r.CreateCVS(file,"Ferrero - DCP"));
        }
    }
}
