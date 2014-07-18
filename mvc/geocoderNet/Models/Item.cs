using System;
using System.Collections.Generic;
using System.Reflection;

namespace geocoderNet.Models
{
    public class Item
    {

        public static string PRECISION_RANGE = "RANGE";
        public static string PRECISION_INTERSECTION = "INTERSECTION";
        public static string PRECISION_STREET = "STREET";
        public static string PRECISION_ZIP = "ZIP";
        public static string PRECISION_CITY = "CITY";

        public static int INVALID_TLID = Int32.MinValue;

        public int fid1 { get; set; }
        public int fid2 { get; set; }
        public string point { get; set; }

        public int tlid { get; set; }
        public string side { get; set; }
        public bool flipped { get; set; }
        public int to0 { get; set; }
        public int to1 { get; set; }


        public string geometry { get; set; }

        public int fid { get; set; }
        public string street { get; set; }
        public string street_phone { get; set; }
        public string street_phone1 { get; set; }
        public string paflag { get; set; }


        public string city { get; set; }
        public string city_phone { get; set; }
        public string city_phone1 { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string status { get; set; }
        public string fips_class { get; set; }
        public string fips_place { get; set; }
        public string fips_county { get; set; }
        public string priority { get; set; }


        public string prenum { get; set; }
        public string sufnum { get; set; }


        public int fromhn { get; set; }
        public int tohn { get; set; }


        public double city_score { get; set; }
        public double street_score { get; set; }

        public double score { get; set; }

        public string precision { get; set; }

        public int number { get; set; }

        public string street1 { get; set; }
        public string street2 { get; set; }

        public string plus4 { get; set; }
        public string full_state { get; set; }


        public Dictionary<string, object> components = null;
        
        public Item()
        {
            tlid = INVALID_TLID;

            city_score = 0.0;
            street_score = 0.0;
            score = 0.0;

            precision = PRECISION_RANGE;

            number = 0;

            components = new Dictionary<string, object>();
        }
        

        // helper functions.
        // -----------------------------------------------------------------------------------

        public void Merge(Item src)
        {
            Merge(this, src);
        }

        public static Item Merge(Item dest, Item src)
        {
            if (dest == null || src == null) return dest;

            Type type = dest.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(string) && property.GetValue(src, null) != null)
                    property.SetValue(dest, String.Copy(property.GetValue(src, null).ToString()));
                if (property.PropertyType == typeof(int) && Convert.ToInt32(property.GetValue(src, null)) != 0)
                    property.SetValue(dest, property.GetValue(src, null));
                if (property.PropertyType == typeof(double) && Convert.ToDouble(property.GetValue(src, null)) != 0.0)
                    property.SetValue(dest, property.GetValue(src, null));
                if (property.PropertyType == typeof(bool))
                    property.SetValue(dest, property.GetValue(src, null));
            }
            return dest;
        }

        public List<string> values_at(string[] keys)
        {
            var values = new List<string>();

            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var key in keys)
            {
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name == key)
                    {
                        values.Add(property.GetValue(this, null) != null ? property.GetValue(this, null).ToString() : null);
                        break;
                    }
                }
            }
            return values;
        }

        public string getValue(string key)
        {

            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name == key)
                    return property.GetValue(this, null) != null ? property.GetValue(this, null).ToString() : null;

            }
            return "";
        }
    }
}