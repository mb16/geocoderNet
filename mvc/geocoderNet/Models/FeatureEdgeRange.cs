using Elmah;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace geocoderNet.Models
{
    public class FeatureEdgeRange : Item
    {


        public static List<Item> ranges_by_feature(string[] fids, int number, string prenum)
        {
            List<Item> feature_edge_range = new List<Item>();

            string fidsList = "";
            if (fids != null)
            {
                foreach (var fid in fids)
                {
                    fidsList += (fidsList.Length > 0 ? ", " : "") + fid;
                }
            }

            //TOP " + (fids.Length * 4).ToString() + " // must use this with the order by else many are missed. not sure how to use the order by in sql server.
            string queryString = "SELECT feature_edge.fid AS fid, range.tlid, range.fromhn, range.tohn, range.prenum, range.zip, range.side FROM feature_edge, range " +
                "WHERE fid IN (" + fidsList + ")" + " AND feature_edge.tlid = range.tlid " +
                (String.IsNullOrEmpty(prenum) ? "" : " AND prenum = @prenum "); // +
               // "ORDER BY min(abs(fromhn - @number), abs(tohn - @number));"; // not sure about this...


            using (var connection = new SqlConnection(Utilities.getConnectionString()))
            {
                var command = connection.CreateCommand();
                command.CommandText = queryString;

                command.Parameters.Add("@number", SqlDbType.Int);
                command.Parameters["@number"].Value = number;

                if (!String.IsNullOrEmpty(prenum))
                {
                    command.Parameters.Add("@prenum", SqlDbType.NVarChar);
                    command.Parameters["@prenum"].Value = prenum;
                }

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Item fer = new Item();
                        fer.fid = reader["fid"] as int? ?? default(int);
                        fer.tlid = reader["tlid"] as int? ?? default(int);
                        fer.fromhn = reader["fromhn"] as int? ?? default(int);
                        fer.tohn = reader["tohn"] as int? ?? default(int);
                        fer.prenum = reader["prenum"] as string;
                       fer.zip = reader["zip"] as string;
                       fer.side = reader["side"] as string;

                        feature_edge_range.Add(fer);
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
            return feature_edge_range;
        }




    }
}