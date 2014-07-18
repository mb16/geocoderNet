using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geocoderNet.Models;

namespace geocoderNet.Tests.Models
{
    [TestClass]
    public class PlaceTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            var data = Place.places_by_zip("Portsmouth", "00210");
            Assert.AreEqual(data.Count, 1, "Place Missing");


            Assert.AreEqual(data[0].zip, "00210", "Place zip Missing");
            Assert.AreEqual(data[0].city, "Portsmouth", "Place city Missing");
            Assert.AreEqual(data[0].state, "NH", "Place state Missing");
            Assert.AreEqual(data[0].city_phone, "PRTSM", "Place city_phone Missing");
            Assert.AreEqual(data[0].lat, 43.005895, "Place lat Missing");
            Assert.AreEqual(data[0].lon, -71.013202, "Place lon Missing");
            Assert.AreEqual(data[0].status, "U", "Place status Missing");
            Assert.AreEqual(data[0].fips_class, "C5", "Place fips_class Missing");
            Assert.AreEqual(data[0].fips_place, "3362900", "Place fips_place Missing");
            Assert.AreEqual(data[0].fips_county, "33015", "Place fips_county Missing");
            Assert.AreEqual(data[0].priority, "1", "Place priority Missing");


            data = Place.places_by_city("Portsmouth", new List<string>(){"Portsmouth"}, "NH");
            Assert.AreEqual(data.Count, 10, "Place Missing");


            Assert.AreEqual(data[0].zip, "00210", "Place zip Missing");
            Assert.AreEqual(data[0].city, "Portsmouth", "Place city Missing");
            Assert.AreEqual(data[0].state, "NH", "Place state Missing");
            Assert.AreEqual(data[0].city_phone, "P632", "Place city_phone Missing");
            Assert.AreEqual(data[0].lat, 43.005895, "Place lat Missing");
            Assert.AreEqual(data[0].lon, -71.013202, "Place lon Missing");
            Assert.AreEqual(data[0].status, "U", "Place status Missing");
            Assert.AreEqual(data[0].fips_class, "C5", "Place fips_class Missing");
            Assert.AreEqual(data[0].fips_place, "3362900", "Place fips_place Missing");
            Assert.AreEqual(data[0].fips_county, "33015", "Place fips_county Missing");
            Assert.AreEqual(data[0].priority, "1", "Place priority Missing");


            data = Place.primary_places(new string[] { "00210" });
            Assert.AreEqual(data.Count, 1, "Place Missing");


            Assert.AreEqual(data[0].zip, "00210", "Place zip Missing");
            Assert.AreEqual(data[0].city, "Portsmouth", "Place city Missing");
            Assert.AreEqual(data[0].state, "NH", "Place state Missing");
            Assert.AreEqual(data[0].city_phone, "P632", "Place city_phone Missing");
            Assert.AreEqual(data[0].lat, 43.005895, "Place lat Missing");
            Assert.AreEqual(data[0].lon, -71.013202, "Place lon Missing");
            Assert.AreEqual(data[0].status, "U", "Place status Missing");
            Assert.AreEqual(data[0].fips_class, "C5", "Place fips_class Missing");
            Assert.AreEqual(data[0].fips_place, "3362900", "Place fips_place Missing");
            Assert.AreEqual(data[0].fips_county, "33015", "Place fips_county Missing");
            Assert.AreEqual(data[0].priority, "1", "Place priority Missing");


        }

        [TestMethod]
        public void TestMethod2()
        {

            var data = Place.zips_by_county("23301", "VA");
            Assert.AreEqual(data.Count, 41, "Failure in zips_by_county");
            
        }

    }
}
