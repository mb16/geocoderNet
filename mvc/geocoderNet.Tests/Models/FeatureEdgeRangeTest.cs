using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geocoderNet.Models;

namespace geocoderNet.Tests.Models
{
    [TestClass]
    public class FeatureEdgeRangeTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            var data = FeatureEdgeRange.ranges_by_feature(new string[] {"1"}, 4450,"");
            Assert.AreEqual(data.Count, 2, "feature_edge_range Missing");


            Assert.AreEqual(data[0].fid, 1, "feature_edge_range fid Missing");
            Assert.AreEqual(data[0].fromhn, 4400, "feature_edge_range fromhn Missing");
            Assert.AreEqual(data[0].tohn, 4498, "feature_edge_range tohn Missing");
            Assert.AreEqual(data[0].prenum, "", "feature_edge_range prenum Missing");
            Assert.AreEqual(data[0].zip, "23336", "feature_edge_range zip Missing");
            Assert.AreEqual(data[0].side, "L", "feature_edge_range side Missing");
         


        }
    }
}
