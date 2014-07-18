using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geocoderNet.Models;

namespace geocoderNet.Tests.Models
{
    [TestClass]
    public class RangeTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var data = Range.range_ends(new string[] {  "114595326" });
            Assert.AreEqual(data.Count, 1, "Range Missing");


            Assert.AreEqual(data[0].tlid, 114595326, "Range tlid Missing");
            Assert.AreEqual(data[0].side, "L", "Range side Missing");
            Assert.AreEqual(data[0].flipped, true, "Range flipped Missing");
            Assert.AreEqual(data[0].fromhn, 18264, "Range fromhn Missing");
            Assert.AreEqual(data[0].tohn, 18398, "Range tohn Missing");

        }
    }
}
