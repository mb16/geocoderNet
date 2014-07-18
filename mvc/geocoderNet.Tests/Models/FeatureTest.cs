using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geocoderNet.Models;
using System.Collections.Generic;

namespace geocoderNet.Tests.Models
{
    [TestClass]
    public class FeatureTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            var data = Feature.features_by_street_and_zip(new List<string>(){"Abell Lane"},
                new List<string>() {"Abell", "Abell Lane", "Abell Ln"}, new string[] {"23336"});

            Assert.AreEqual(data.Count, 1, "Feature Missing");


            Assert.AreEqual(data[0].fid, 14, "Feature fid Missing");
            Assert.AreEqual(data[0].street, "Abell Ln", "Feature street Missing");
            Assert.AreEqual(data[0].street_phone, "A140", "Feature street_phone Missing");
            Assert.AreEqual(data[0].paflag, "P", "Feature paflag Missing");
            Assert.AreEqual(data[0].zip, "23336", "Feature zip Missing");



         

        }
    }
}
