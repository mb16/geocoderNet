using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geocoderNet.Models;

namespace geocoderNet.Tests.Models
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void unique_valuesTestMethod1()
        {
            var db = new Database();
            List<Item> items = new List<Item>();
            items.Add(new Item() { city = "Frederick" });
            items.Add(new Item() { city = "Frederick" });
            items.Add(new Item() { city = "Hagerstown" });

            var list = db.unique_values(items, "city");
            Assert.AreEqual(list.Count, 2, "Failed to find unique_values");
            Assert.AreEqual(list[0], "Frederick", "Failed to find unique_values");
            Assert.AreEqual(list[1], "Hagerstown", "Failed to find unique_values");

        }

        [TestMethod]
        public void rows_to_hTestMethod1()
        {
            var db = new Database();
            List<Item> items = new List<Item>();
            items.Add(new Item() { city = "Frederick", street = "Partick St", fromhn = 123});
            items.Add(new Item() { city = "Frederick", street = "Partick St", fromhn = 456 });
            items.Add(new Item() { city = "Hagerstown", street = "College Ln", fromhn = 789 });

            var list = db.rows_to_h(items, new string[] {
            "city",
            "street"
            });

            var temp = list.Keys.ToList()[0];

            Assert.AreEqual(list.Keys.Count, 2, "Failed to find rows_to_h");
            Assert.AreEqual(list[list.Keys.ToList()[0]].Count, 2, "Failed to find rows_to_h");
            Assert.AreEqual(list[list.Keys.ToList()[1]].Count, 1, "Failed to find rows_to_h");

        }


        [TestMethod]
        public void merge_rowsTestMethod1()
        {
            var db = new Database();
            List<Item> dest = new List<Item>();
            dest.Add(new Item() { city = "Frederick", street = "Partick St", zip = "" });
            dest.Add(new Item() { city = "Frederick", street = "College Ln", zip = "" });
            dest.Add(new Item() { city = "Hagerstown", street = "College Ln", zip = "" });
            dest.Add(new Item() { city = "Hagerstown", street = "Franklin St", zip = "" });

            List<Item> src = new List<Item>();
            src.Add(new Item() { city = "Frederick", street = "Partick St", zip = "54321" });
            src.Add(new Item() { city = "Frederick", street = "College Ln", zip = "11111" });
            src.Add(new Item() { city = "Hagerstown", street = "College Ln", zip = "22222" });
            src.Add(new Item() { city = "Hagerstown", street = "Robinwood Ln", zip = "33333" });

            var list = db.merge_rows(dest, src, new string[] {
            "city",
            "street"
            });

            Assert.AreEqual(list.Count, 4, "Failed to find merge_rows");
            Assert.AreEqual(list[0].zip, "54321", "Failed to find merge_rows");
            Assert.AreEqual(list[1].zip, "11111", "Failed to find merge_rows");
            Assert.AreEqual(list[2].zip, "22222", "Failed to find merge_rows");
            Assert.AreEqual(list[3].zip, "", "Failed to find merge_rows");


        }

        [TestMethod]
        public void find_candidatesTestMethod1()
        {
            var address = new Address();
            address.city = new List<string>() { "Accomac" };
            address.street = new List<string>() { "Front St" };
            address.number = "23610";
            address.zip = "23301";
            address.state = "VA";

            var temp = address.street_parts();

            var db = new Database();
            var items = db.find_candidates(address);

            Assert.AreEqual(items[0].zip, "23301", "Failed to find find_candidates");
            Assert.AreEqual(items[0].street, "Front St", "Failed to find find_candidates");

        }

        [TestMethod]
        public void assign_numberTestMethod1()
        {
            var dest = new List<Item>();
            dest.Add(new Item() { fid = 793, city = "Frederick", street = "Partick St", fromhn = 10, tohn = 100 });
            dest.Add(new Item() { fid = 793, city = "Frederick", street = "College Ln", fromhn = 80, tohn = 100 });
            dest.Add(new Item() { fid = 793, city = "Frederick", street = "College Ln", fromhn = 10, tohn = 40 });

            var db = new Database();
            db.assign_number(50, dest);

            Assert.AreEqual(dest[0].number, 50, "Failed to find assign_number");
            Assert.AreEqual(dest[1].number, 80, "Failed to find assign_number");
            Assert.AreEqual(dest[2].number, 40, "Failed to find assign_number");

        }


        [TestMethod]
        public void add_rangesTestMethod1()
        {

            var address = new Address();
            address.city = new List<string>() { "Accomac" };
            address.street = new List<string>() { "Front St" };
            address.number = "23610";
            address.zip = "23301";
            address.state = "VA";


            var dest = new List<Item>();
            dest.Add(new Item() { fid = 793, city = "Accomac", street = "Front St", fromhn = 23000, tohn = 24000, flipped = true});


            var db = new Database();
            dest = db.add_ranges(address, dest);

            Assert.AreEqual(dest.Count, 69, "Failed to find add_ranges");
            Assert.AreNotEqual(dest.FirstOrDefault(o => o.number == 23610), null, "Failed to find add_ranges");          
        }

        [TestMethod]
        public void merge_edgesTestMethod1()
        {

            var dest = new List<Item>();
            dest.Add(new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L", fromhn = 23573, tohn = 23619});


            var db = new Database();
            var edge = db.merge_edges(ref dest);

            Assert.AreEqual(edge.Count, 1, "Failed to find merge_edges");
            Assert.AreEqual(edge[0], "114577368", "Failed to find merge_edges");
            Assert.AreEqual(dest[0].geometry.Trim(), "010500000001000000010200000002000000AB93331477EA52C00395F1EF33DC4240C537143E5BEA52C0FA07910C39DC4240", "Failed to find merge_edges");
        }


        [TestMethod]
        public void extend_rangesTestMethod1()
        {

            var dest = new List<Item>();
            dest.Add(new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L" });


            var db = new Database();
            var dest2 = db.extend_ranges(dest);

            Assert.AreEqual(dest2.Count, 1, "Failed to find extend_ranges");
            Assert.AreEqual(dest2[0].tohn, 23619, "Failed to find extend_ranges");
            Assert.AreEqual(dest2[0].fromhn, 23573, "Failed to find extend_ranges");
        }

        [TestMethod]
        public void best_candidatesTestMethod1()
        {

            var dest = new List<Item>();
            dest.Add(new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L", score = 2.0});
            dest.Add(new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L", score = 4.0});
            dest.Add(new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L", score = 2.0 });
            dest.Add(new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L", score = 3.0 });

            var db = new Database();
            var dest2 = db.best_candidates(dest);

            Assert.AreEqual(dest2.Count, 1, "Failed to find best_candidates");
            Assert.AreEqual(dest2[0].score, 4.0, "Failed to find best_candidates");


        }


        [TestMethod]
        public void interpolation_distanceTestMethod1()
        {

            var item = new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L", fromhn = 23573, tohn = 23619, number = 23610 };
            
            var db = new Database();
            var dest2 = db.interpolation_distance(item);

            Assert.AreEqual(dest2 > 0.8, true, "Failed to find interpolation_distance");
           
        }


        [TestMethod]
        public void scale_lonTestMethod1()
        {
            var db = new Database();
            var dest2 = db.scale_lon(37.0, 38.0);

            Assert.AreEqual(dest2 > 0.79, true, "Failed to find scale_lon");

        }


        [TestMethod]
        public void distanceTestMethod1()
        {
            var db = new Database();
            var dest2 = db.distance(new double[] { 39.6428, 77.7200 }, new double[] { 39.4263, 77.4204 });

            Assert.AreEqual(dest2 < 0.31, true, "Failed to find distance");

        }

        [TestMethod]
        public void street_side_offsetTestMethod1()
        {
            var db = new Database();
            var dest2 = db.street_side_offset(0.1, new double[] { 2.0, 2.0 }, new double[] { 4.0, 4.0 });

            Assert.AreEqual(dest2[0] > 4.0, true, "Failed to find street_side_offset");
            Assert.AreEqual(dest2[1] < 4.0, true, "Failed to find street_side_offset");
        }


        [TestMethod]
        public void interpolateTestMethod1()
        {
            var db = new Database();
            var dest2 = db.interpolate(new double[][] { new double[] { 2.0, 2.0 }, new double[] { 4.0, 4.0 } }, 0.3, 1);

            Assert.AreEqual(dest2[0] < 2.6, true, "Failed to find interpolate");
            Assert.AreEqual(dest2[1] < 2.6, true, "Failed to find interpolate");
        }


        [TestMethod]
        public void canonicalize_placesTestMethod1()
        {

            var dest = new List<Item>();
            dest.Add(new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L", zip = "23301" });

            var db = new Database();
            var dest2 = db.canonicalize_places(dest);

            Assert.AreEqual(dest2[0].fips_county, "51001",  "Failed to find canonicalize_places");
            Assert.AreEqual(dest2[0].state, "VA", "Failed to find canonicalize_places");
        }


        [TestMethod]
        public void score_candidatesTestMethod1()
        {

            var address = new Address();
            address.city = new List<string>() { "Accomac" };
            address.street = new List<string>() { "Front St" };
            address.number = "23610";
            address.zip = "23301";
            address.state = "VA";

            var dest = new List<Item>();
            dest.Add(new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L", zip = "23301", fromhn = 23573, tohn = 23619, number = 23610, street_score = 0.3, city_score = 0.4 });

            var db = new Database();
            db.score_candidates(address, ref dest);

            Assert.AreEqual(dest[0].score > 0.65, true, "Failed to find score_candidates");

        }

        [TestMethod]
        public void best_placesTestMethod1()
        {

            var address = new Address();
            address.city = new List<string>() { "Accomac" };
            address.street = new List<string>() { "Front St" };
            address.number = "23610";
            address.zip = "23301";
            address.state = "VA";

            var dest = new List<Item>();
            dest.Add(new Item() { fid = 793, tlid = 114577368, city = "Accomac", street = "Front St", side = "L", zip = "23301", fromhn = 23573, tohn = 23619, number = 23610, street_score = 0.3, city_score = 0.4 });

            var db = new Database();
            var dest2 = db.best_places(address, dest);

            Assert.AreEqual(dest2[0].precision, Item.PRECISION_ZIP, "Failed to find best_places");

        }


        [TestMethod]
        public void geocode_placeTestMethod1()
        {

            var address = new Address();
            address.text = "P. O. Box 12 Accomac, VA 23301";
            address.parse();
     

            var db = new Database();
            var dest2 = db.geocode_place(address);

            Assert.AreEqual(dest2[0].precision, Item.PRECISION_ZIP, "Failed to find geocode_place");
            Assert.AreEqual(dest2[0].city, "Accomac", "Failed to find geocode_place");
            Assert.AreEqual(dest2[0].score >= 0.9, true, "Failed to find geocode_place");

        }



        [TestMethod]
        public void geocode_intersectionTestMethod1()
        {

            var address = new Address();
            address.text = "Front St and Back St Accomac, VA 23301";
            address.parse(); // not sure the parser with for intersections.

            address.street = new List<string>() { "Front St", "Back St" };
            address.city = new List<string>() {"Accomac"};

            var db = new Database();
            var dest2 = db.geocode_intersection(address);

            Assert.AreEqual(dest2.Count, 1, "Failed to find geocode_intersection");
            Assert.AreEqual(dest2[0].fid1, 91, "Failed to find geocode_intersection");
            Assert.AreEqual(dest2[0].fid2, 793, "Failed to find geocode_intersection");
           

        }




        [TestMethod]
        public void geocode_addressTestMethod1()
        {

            var address = new Address();
            address.text = "23610 Front St Accomac, VA 23301";
            address.parse();


            var db = new Database();
            var dest2 = db.geocode_address(address);

            Assert.AreEqual(dest2.Count, 1, "Failed to find geocode_address");
            Assert.AreEqual(dest2[0].tlid, 114577368, "Failed to find geocode_address");
            Assert.AreEqual(dest2[0].score >= 0.75, true, "Failed to find geocode_address");
            Assert.AreEqual(dest2[0].lat > 37.7203, true, "Failed to find geocode_address");
            Assert.AreEqual(dest2[0].lon > -75.6624, true, "Failed to find geocode_address");

        }


        [TestMethod]
        public void filter_by_scoreTestMethod1()
        {

            var dest = new List<Item>();
            dest.Add(new Item() { score = 0.5 });
            dest.Add(new Item() { score = 0.6 });
            dest.Add(new Item() { score = 0.7 }); 
            dest.Add(new Item() { score = 0.8 }); 
            dest.Add(new Item() { score = 0.9 });

            var db = new Database();
            var dest2 = db.filter_by_score(dest, 0.75, "score");

            Assert.AreEqual(dest2.Count, 3, "Failed to find filter_by_score");

        }
        
        [TestMethod]
        public void find_candidates_newTestMethod1()
        {
            var address = new Address();
            address.city = new List<string>() { "Oak Hall" };
            address.street = new List<string>() { "Lankford Hwy" };
            address.number = "7027";
            address.zip = "23301"; // wrong zip, still should find it.
            address.state = "VA";

            var temp = address.street_parts();

            var db = new Database();
            var items = db.find_candidates_new(address);

            Assert.AreEqual(items[0].zip, "23416", "Failed to find find_candidates_new");
            Assert.AreEqual(items[0].street, "Front St", "Failed to find find_candidates_new");

        }
        
    }
}
