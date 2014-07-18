using Elmah;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace geocoderNet.Models
{
    public class Range :Item
    {



        public static List<Item> range_ends(string[] edge_ids)
        {
            List<Item> ranges = new List<Item>();

            string edgeList = "";
            foreach (var edge_id in edge_ids)
            {
                edgeList += (edgeList.Length > 0 ? "," : "") + edge_id;
            }


            string queryString = "SELECT tlid, side, Convert(Bit, Case When ( min(fromhn) > min(tohn)) Then 1 Else 0 End) AS flipped, min(fromhn) AS from0, max(tohn) AS to0, min(tohn) AS from1, max(fromhn) AS to1 FROM range " +
                "WHERE tlid IN ( " + edgeList + ") GROUP BY tlid, side;";


            using (var connection = new SqlConnection(Utilities.getConnectionString()))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = queryString;


                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Item r = new Item();
                        r.tlid = reader["tlid"] as int? ?? default(int);
                        r.side = reader["side"] as string;
                        
                        if ((bool)reader["flipped"]){
                            r.flipped = true;
                            r.fromhn = reader["from1"] as int? ?? default(int);
                            r.tohn = reader["to1"] as int? ?? default(int);
                        }else{
                            r.flipped = false;
                            r.fromhn = reader["from0"] as int? ?? default(int);
                            r.tohn = reader["to0"] as int? ?? default(int);
                        }

                        ranges.Add(r);
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
            return ranges;
        }

    }
}