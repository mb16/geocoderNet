using Elmah;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace geocoderNet.Models
{
    public class Feature : Item
    {


        public Feature() : base()
        {
        }


        public static List<Item> features_by_street(List<string> streets, List<string> tokens)
        {
            return features_by_street_and_zip_internal(streets, tokens, null, false);
        }

        public static List<Item> more_features_by_street_and_zip(List<string> streets, List<string> tokens, string[] zips)
        {
            return features_by_street_and_zip_internal(streets, tokens, zips, true);
        }

        public static List<Item> features_by_street_and_zip(List<string> streets, List<string> tokens, string[] zips)
        {
            return features_by_street_and_zip_internal(streets, tokens, zips, false);
        }

        public static List<Item> features_by_street_and_zip_internal(List<string> streets, List<string> tokens, string[] zips, bool truncateZips = false)
        {
            List<Item> features = new List<Item>();

            if (streets == null || streets.Count == 0) return features;

            string tokenList = "";
            if (tokens != null) { 
                foreach (var token in tokens)
                {
                    if (Utilities.getMetaphoneFunction() == "dbo.fnDoubleMetaphoneScalar")
                        tokenList += (tokenList.Length > 0 ? ", " : "") + Utilities.getMetaphoneFunction() + "(1, '" + token + "')";
                    else
                        tokenList += (tokenList.Length > 0 ? ", " : "") + Utilities.getMetaphoneFunction() + "('" + token + "')";
                }
            }

            string zipQuery = "";
            if (truncateZips)
            {
                if (zips != null && zips.Length > 0)
                {
                    zipQuery += " AND (";
                    foreach (var zip in zips)
                    {
                        zipQuery += (zipQuery.Length > 8 ? " OR" : "") + " zip LIKE '" + zip.Substring(0, 3) + "%' ";
                    }
                }

                zipQuery += ")";
            }
            else {
                string zipList = "";
                if (zips != null){
                    foreach (var zip in zips)
                    {
                        zipList += (zipList.Length > 0 ? "," : "") + "'" + zip + "'";
                    }
                }

                zipQuery = (String.IsNullOrEmpty(zipList) ? ";" : " AND zip IN (" + zipList + ");");
            }

            var street_phone = Utilities.getMetaphoneFunction() == "dbo.fnDoubleMetaphoneScalar" ? "street_phone1" : "street_phone";

            string queryString = "SELECT fid, street, street_phone, street_phone1, paflag, zip FROM feature " +
                "WHERE " + street_phone + " IN (" + tokenList + ") " + zipQuery + ";";


            using (var connection = new SqlConnection(Utilities.getConnectionString()))
            {
                var command = connection.CreateCommand();
                command.CommandText = queryString;
                command.CommandTimeout = 80;
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foreach (var street in streets)
                        {
                            var ft = new Item();
                            ft.fid = reader["fid"] as int? ?? default(int);
                            ft.street = reader["street"] as string;
                            ft.street_phone = reader["street_phone"] as string;
                            ft.street_phone1 = reader["street_phone1"] as string;
                            ft.paflag = reader["paflag"] as string;
                            ft.zip = reader["zip"] as string;
                            ft.street_score =
                                ((double)
                                    Utilities.EditDistance(street.Trim().ToLower(),
                                        ((string) reader["street"]).Trim().ToLower()))/
                                Math.Max(street.Length, ((string) reader["street"]).Length);

                            features.Add(ft);
                        }
                    }

                    reader.Close();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex); //ELMAH Signaling
                    return new List<Item>();
                }

            }
            return features;
        }

    }
}