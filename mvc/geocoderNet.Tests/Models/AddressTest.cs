using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geocoderNet.Models;

namespace geocoderNet.Tests.Models
{
    [TestClass]
    public class AddressTest
    {

        [TestMethod]
        public void CleanTest()
        {

            Address address = new Address();
            address.text = "P.O.     Box 12 Washington D.C.";
            Assert.AreEqual(address.clean(address.text), "PO Box 12 Washington DC", "Failed to clean address");

            address.text = "Post Office Box 12      Washington             D.C.";
            Assert.AreEqual(address.clean(address.text), "Post Office Box 12 Washington DC", "Failed to clean address");

            address.text = "P O Box 12 Washington D.C.         ";
            Assert.AreEqual(address.clean(address.text), "P O Box 12 Washington DC", "Failed to clean address");

        }

        [TestMethod]
        public void expand_numbersTest()
        {

            Address address = new Address();
            address.text = "1st Street";
            var lst = (List<string>)address.expand_numbers(address.text);
            Assert.AreEqual(lst[0], "1 Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[1], "first Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[2], "one Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[3], "1st Street", "Failed to expand_numbers");

            address.text = "3rd Street";
            lst = (List<string>)address.expand_numbers(address.text);
            Assert.AreEqual(lst[0], "3 Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[1], "third Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[2], "three Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[3], "3rd Street", "Failed to expand_numbers");

            address.text = "8th Street";
            lst = (List<string>)address.expand_numbers(address.text);
            Assert.AreEqual(lst[0], "8 Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[1], "eighth Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[2], "eight Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[3], "8th Street", "Failed to expand_numbers");

            // ---------------------------------------
            address.text = "first Street";
            lst = (List<string>)address.expand_numbers(address.text);
            Assert.AreEqual(lst[0], "1 Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[1], "1st Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[2], "one Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[3], "first Street", "Failed to expand_numbers");

            address.text = "third Street";
            lst = (List<string>)address.expand_numbers(address.text);
            Assert.AreEqual(lst[0], "3 Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[1], "3rd Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[2], "three Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[3], "third Street", "Failed to expand_numbers");

            address.text = "eighth Street";
            lst = (List<string>)address.expand_numbers(address.text);
            Assert.AreEqual(lst[0], "8 Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[1], "8th Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[2], "eight Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[3], "eighth Street", "Failed to expand_numbers");



            // -------------------------------------
            address.text = "one Street";
            lst = (List<string>)address.expand_numbers(address.text);
            Assert.AreEqual(lst[0], "1 Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[1], "first Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[2], "1st Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[3], "one Street", "Failed to expand_numbers");

            address.text = "three Street";
            lst = (List<string>)address.expand_numbers(address.text);
            Assert.AreEqual(lst[0], "3 Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[1], "third Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[2], "3rd Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[3], "three Street", "Failed to expand_numbers");

            address.text = "eight Street";
            lst = (List<string>)address.expand_numbers(address.text);
            Assert.AreEqual(lst[0], "8 Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[1], "eighth Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[2], "8th Street", "Failed to expand_numbers");
            Assert.AreEqual(lst[3], "eight Street", "Failed to expand_numbers");


        }


        [TestMethod]
        public void parse_zipTest()
        {

            var address = new Address();
            var txt = "P.O. Box 12 Washington D.C. 20002";
            var match = Match.zip.Match(txt);
            var zip = match.Value;
            Assert.AreEqual(zip, "20002", "Failed to parse_zip");
            txt = address.parse_zip(match, txt);
            Assert.AreEqual(txt, "P.O. Box 12 Washington D.C.", "Failed to parse_zip");
            Assert.AreEqual(address.zip, "20002", "Failed to parse_zip");
            Assert.AreEqual(address.plus4, null, "Failed to parse_zip");


            address = new Address();
            txt = "P.O. Box 12 Washington D.C. 20002-0011";
            match = Match.zip.Match(txt);
            zip = match.Value;
            Assert.AreEqual(zip, "20002-0011", "Failed to parse_zip");
            txt = address.parse_zip(match, txt);
            Assert.AreEqual(txt, "P.O. Box 12 Washington D.C.", "Failed to parse_zip");
            Assert.AreEqual(address.zip, "20002", "Failed to parse_zip");
            Assert.AreEqual(address.plus4, "0011", "Failed to parse_zip");


            address = new Address();
            txt = "P.O. Box 12 Washington D.C., 20002";
            match = Match.zip.Match(txt);
            zip = match.Value;
            Assert.AreEqual(zip, "20002", "Failed to parse_zip");
            txt = address.parse_zip(match, txt);
            Assert.AreEqual(txt, "P.O. Box 12 Washington D.C.", "Failed to parse_zip");
            Assert.AreEqual(address.zip, "20002", "Failed to parse_zip");
            Assert.AreEqual(address.plus4, null, "Failed to parse_zip");
        }


        [TestMethod]
        public void parse_stateTest()
        {

            var address = new Address();
            var txt = "P.O. Box 12 Frederick, MD";
            var match = Match.state.Match(txt);
            var state = match.Value;
            Assert.AreEqual(state, "MD", "Failed to parse_zip");
            txt = address.parse_state(match, txt);
            Assert.AreEqual(txt, "P.O. Box 12 Frederick", "Failed to parse_state");
            Assert.AreEqual(address.state, "MD", "Failed to parse_state");
            Assert.AreEqual(address.full_state, "MD", "Failed to parse_state");


            address = new Address();
            txt = "P.O. Box 12 Frederick, Maryland";
            match = Match.state.Match(txt);
            state = match.Value;
            Assert.AreEqual(state, "Maryland", "Failed to parse_zip");
            txt = address.parse_state(match, txt);
            Assert.AreEqual(txt, "P.O. Box 12 Frederick", "Failed to parse_state");
            Assert.AreEqual(address.state, "MD", "Failed to parse_state");
            Assert.AreEqual(address.full_state, "Maryland", "Failed to parse_state");
        }


        [TestMethod]
        public void parse_numberTest()
        {

            var address = new Address();
            var txt = "12 west Partick Street Frederick, MD";
            var match = Match.number.Match(txt);
            var num = match.Value;
            Assert.AreEqual(num, "12", "Failed to parse_number");
            txt = address.parse_number(match, txt);
            Assert.AreEqual(txt, "west Partick Street Frederick, MD", "Failed to parse_number");
            Assert.AreEqual(address.prenum, null, "Failed to parse_number");
            Assert.AreEqual(address.number, "12", "Failed to parse_number");
            Assert.AreEqual(address.sufnum, "", "Failed to parse_number");

             address = new Address();
             txt = "A12 west Partick Street Frederick, MD";
             match = Match.number.Match(txt);
             num = match.Value;
            Assert.AreEqual(num, "A12", "Failed to parse_number");
            txt = address.parse_number(match, txt);
            Assert.AreEqual(txt, "west Partick Street Frederick, MD", "Failed to parse_number");
            Assert.AreEqual(address.prenum, "A", "Failed to parse_number");
            Assert.AreEqual(address.number, "12", "Failed to parse_number");
            Assert.AreEqual(address.sufnum, "", "Failed to parse_number");


            address = new Address();
            txt = "12B west Partick Street Frederick, MD";
            match = Match.number.Match(txt);
            num = match.Value;
            Assert.AreEqual(num, "12B", "Failed to parse_number");
            txt = address.parse_number(match, txt);
            Assert.AreEqual(txt, "west Partick Street Frederick, MD", "Failed to parse_number");
            Assert.AreEqual(address.prenum, null, "Failed to parse_number");
            Assert.AreEqual(address.number, "12", "Failed to parse_number");
            Assert.AreEqual(address.sufnum, "B", "Failed to parse_number");
        }

        [TestMethod]
        public void parseTest()
        {
            var address = new Address();
            address.street = new List<string>();
            address.city = new List<string>();
            address.text = "110 E Patrick Sq Frederick, MD 21701";

            address.parse();
            Assert.AreEqual(address.city.Count(), 2, "Failed to find parse");
            Assert.AreEqual(address.city[0], "E Patrick Sq Frederick".ToLower(), "Failed to find parse");
            Assert.AreEqual(address.city[1], "E Patrick Square Frederick".ToLower(), "Failed to find parse");
         
        }



        [TestMethod]
        public void expand_streetsTest()
        {
            var address = new Address();

            var st = new List<string>();
            st.Add("North James Johnson St");
            st.Add("S Johnson Bay Ct");
            st.Add("East Patrick Johnson Cyn");
            var res = (List<string>)address.expand_streets(st);
            Assert.AreEqual(res.Count(), 5, "Failed to find street_parts");
            Assert.AreEqual(res[0], "North James Johnson St", "Failed to find street_parts");
            Assert.AreEqual(res[1], "S Johnson Bay Ct", "Failed to find street_parts");
            Assert.AreEqual(res[2], "East Patrick Johnson Cyn", "Failed to find street_parts");
            Assert.AreEqual(res[3], "S Johnson Bay Court", "Failed to find street_parts");
            Assert.AreEqual(res[4], "East Patrick Johnson Canyon", "Failed to find street_parts");
        }


        [TestMethod]
        public void street_partsTest()
        {
            var address = new Address();

            address.street.Add("North James Johnson St");
            address.street.Add("S Johnson Bay St");
            address.street.Add("East Patrick Johnson St");
            var res = (List<string>)address.street_parts();
            Assert.AreEqual(res.Count(), 5, "Failed to find street_parts");
            Assert.AreEqual(res[0], "Johnson", "Failed to find street_parts");
            Assert.AreEqual(res[1], "James Johnson", "Failed to find street_parts");
            Assert.AreEqual(res[2], "Bay", "Failed to find street_parts");
            Assert.AreEqual(res[3], "Johnson Bay", "Failed to find street_parts");
            Assert.AreEqual(res[4], "Patrick Johnson", "Failed to find street_parts");
        }

        [TestMethod]
        public void remove_noise_wordsTest()
        {
            var address = new Address();
            var strings = new List<string>();
            strings.Add("North Johnson St");
            strings.Add(" S  Johnson St");
            strings.Add(" East  Johnson St");
            strings.Add("W  Johnson St");
            var res = (List<string>)address.remove_noise_words(strings);
            Assert.AreEqual(res.Count(), 4, "Failed to find remove_noise_words");
            Assert.AreEqual(res[0], "Johnson", "Failed to find remove_noise_words");
            Assert.AreEqual(res[1], "Johnson", "Failed to find remove_noise_words");
            Assert.AreEqual(res[2], "Johnson", "Failed to find remove_noise_words");
            Assert.AreEqual(res[3], "Johnson", "Failed to find remove_noise_words");
            
        }


        [TestMethod]
        public void city_partsTest()
        {
            var address = new Address();

            address.city.Add("San Francisco");
            address.city.Add("Baltimore");
            address.city.Add("North Chevy Chase");
            var res = (List<string>)address.city_parts();
            Assert.AreEqual(res.Count(), 6, "Failed to find city_parts");
            Assert.AreEqual(res[0], "Francisco", "Failed to find city_parts");
            Assert.AreEqual(res[1], "San Francisco", "Failed to find city_parts");
            Assert.AreEqual(res[2], "Baltimore", "Failed to find city_parts");
            Assert.AreEqual(res[3], "Chase", "Failed to find city_parts");
            Assert.AreEqual(res[4], "Chevy Chase", "Failed to find city_parts");
            Assert.AreEqual(res[5], "North Chevy Chase", "Failed to find city_parts");
        }

        [TestMethod]
        public void city_xTest()
        {
            var address = new Address();

           // not clear this is being used.
        }


        [TestMethod]
        public void PO_BoxTest()
        {

            var address = new Address();
            address.text = "P.O. Box 12 Washington D.C.";
            Assert.AreEqual(address.po_box(), true, "Failed to find PO Box");

            address.text = "Post Office Box 12 Washington D.C.";
            Assert.AreEqual(address.po_box(), true, "Failed to find PO Box");

            address.text = "P O Box 12 Washington D.C.";
            Assert.AreEqual(address.po_box(), true, "Failed to find PO Box");

        }

        [TestMethod]
        public void IntersectionTest()
        {

            Address address = new Address();
            address.text = "5th and 3rd street washington dc";
            Assert.AreEqual(address.intersection(), true, "Failed to find intersection");

        }

    }
}
