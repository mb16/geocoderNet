using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geocoderNet.Models;

namespace geocoderNet.Tests.Models
{
    [TestClass]
    public class EdgeTest
    {
        [TestMethod]
        public void TestMethod1()
        {


            var data = Edge.edges(new string[] { "114519677" });
            Assert.AreEqual(data.Count, 1, "Edge Missing");


            Assert.AreEqual(data[0].tlid, 114519677, "Place tlid Missing");
            Assert.AreNotEqual(data[0].geometry, "", "Place geometry Missing");
          

        }
    }
}
