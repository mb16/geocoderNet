using Elmah;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace geocoderNet.Models
{
    public class Place : Item
    {


        
        public Place()
            : base()
        {
        }

        public static List<Item> places_by_zip(string city, string zip)
        {
            var places = new List<Item>();

            if (String.IsNullOrEmpty(city)) return places;

            var queryString = "SELECT zip, city, state, city_phone, city_phone1, lat, lon, status, fips_class, fips_place, fips_county, priority FROM place WHERE zip = @zip order by priority desc;";

            using (var connection = new SqlConnection(Utilities.getConnectionString()))
            {
                var command = connection.CreateCommand();
                command.CommandText = queryString;

                command.Parameters.Add("@zip", SqlDbType.NChar);
                command.Parameters["@zip"].Value = zip;


                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var pl = new Item();
                        pl.zip = reader["zip"] as string;
                        pl.city = reader["city"] as string;
                        pl.state =reader["state"] as string;
                        pl.city_phone = reader["city_phone"] as string;
                        pl.city_phone1 = reader["city_phone1"] as string;
                        pl.lat = reader["lat"] as double? ?? default(double);
                        pl.lon = reader["lon"] as double? ?? default(double);
                        pl.status = reader["status"] as string;
                        pl.fips_class = reader["fips_class"] as string;
                        pl.fips_place = reader["fips_place"] as string;
                        pl.fips_county = reader["fips_county"] as string;
                        pl.priority = reader["priority"] as string;
                        pl.city_score = ((double)Utilities.EditDistance(city.Trim().ToLower(), ((string)reader["city"]).Trim().ToLower())) / Math.Max(city.Length, ((string)reader["city"]).Length);


                        places.Add(pl);
                    }

                    reader.Close();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex); //ELMAH Signaling
                    return null;
                }

            }
            return places;
        }


        public static List<Item> places_by_city(string city, List<string> tokens, string state)
        {
            var places = new List<Item>();

            if (String.IsNullOrEmpty(city)) return places;

            var tokenList = "";
            foreach (var token in tokens)
            {
                if (Utilities.getMetaphoneFunction() == "dbo.fnDoubleMetaphoneScalar")
                    tokenList += (tokenList.Length > 0 ? ", " : "") + Utilities.getMetaphoneFunction() + "(1, '" + token + "')";
                else
                    tokenList += (tokenList.Length > 0 ? ", " : "") + Utilities.getMetaphoneFunction() + "('" + token + "')";
            }

            var phone = "city_phone";
            if (Utilities.getMetaphoneFunction() == "dbo.fnDoubleMetaphoneScalar")
                phone = "city_phone1";



            var queryString = "SELECT zip, city, state, city_phone, city_phone1, lat, lon, status, fips_class, fips_place, fips_county, priority FROM place WHERE " +
                phone + " IN (" + tokenList + ") " +
                (String.IsNullOrEmpty(state) ? "" : " AND state = @state ") +
                 "order by priority desc;";

            using (var connection = new SqlConnection(Utilities.getConnectionString()))
            {
                var command = connection.CreateCommand();
                command.CommandText = queryString;

                if (!String.IsNullOrEmpty(state))
                {
                    command.Parameters.Add("@state", SqlDbType.NChar);
                    command.Parameters["@state"].Value = state;
                }

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var pl = new Item();
                        pl.zip = reader["zip"] as string;
                        pl.city = reader["city"] as string;
                        pl.state = reader["state"] as string;
                        pl.city_phone = reader["city_phone"] as string;
                        pl.city_phone1 = reader["city_phone1"] as string;
                        pl.lat = reader["lat"] as double? ?? default(double);
                        pl.lon = reader["lon"] as double? ?? default(double);
                        pl.status = reader["status"] as string;
                        pl.fips_class = reader["fips_class"] as string;
                        pl.fips_place = reader["fips_place"] as string;
                        pl.fips_county = reader["fips_county"] as string;
                        pl.priority = reader["priority"] as string;
                        pl.city_score = ((double)Utilities.EditDistance(city.Trim().ToLower(), ((string)reader["city"]).Trim().ToLower())) / Math.Max(city.Length, ((string)reader["city"]).Length);


                        places.Add(pl);
                    }

                    reader.Close();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex); //ELMAH Signaling
                    return null;
                }

            }
            return places;
        }


        public static List<Item> primary_places(string[] zips)
        {
            var places = new List<Item>();

            if (zips.Count() == 0) return places;

            var zipList = "";
            foreach (var zip in zips)
            {
                zipList += (zipList.Length > 0 ? "," : "") + "'" + zip + "'";
            }


            var queryString = "SELECT zip, city, state, city_phone, city_phone1, lat, lon, status, fips_class, fips_place, fips_county, priority FROM place " +
                "WHERE zip IN (" + zipList + ") order by priority desc;";


            using (var connection = new SqlConnection(Utilities.getConnectionString()))
            {
                var command = connection.CreateCommand();
                command.CommandText = queryString;


                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Item pl = new Item();
                        pl.zip = reader["zip"] as string;
                        pl.city = reader["city"] as string;
                        pl.state = reader["state"] as string;
                        pl.city_phone = reader["city_phone"] as string;
                        pl.city_phone1 = reader["city_phone1"] as string;
                        pl.lat = reader["lat"] as double? ?? default(double);
                        pl.lon = reader["lon"] as double? ?? default(double);
                        pl.status = reader["status"] as string;
                        pl.fips_class = reader["fips_class"] as string;
                        pl.fips_place = reader["fips_place"] as string;
                        pl.fips_county = reader["fips_county"] as string;
                        pl.priority = reader["priority"] as string;
                        pl.city_score = 0.0;

                        places.Add(pl);
                    }

                    reader.Close();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex); //ELMAH Signaling
                    return null;
                }

            }
            return places;
        }


        public static List<string> zips_by_county(string zip, string state)
        {
            var zips = new List<string>();

            if (String.IsNullOrEmpty(zip) || String.IsNullOrEmpty(state)) return zips;

            var queryString = "SELECT DISTINCT zip FROM place WHERE fips_county IN (SELECT fips_county FROM place WHERE zip = @zip AND state = @state);";

            using (var connection = new SqlConnection(Utilities.getConnectionString()))
            {
                var command = connection.CreateCommand();
                command.CommandText = queryString;

                command.Parameters.Add("@zip", SqlDbType.NChar);
                command.Parameters["@zip"].Value = zip;

                command.Parameters.Add("@state", SqlDbType.NChar);
                command.Parameters["@state"].Value = state;

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        zips.Add(reader["zip"] as string);
                    }

                    reader.Close();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex); //ELMAH Signaling
                    return null;
                }

            }
            return zips;
        }
    }
}