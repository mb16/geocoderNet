using Elmah;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace geocoderNet.Models
{
    public class Edge
    {

        public static List<Item> edges(string[] edge_ids)
        {
            List<Item> edges = new List<Item>();

            string edgeList = "";
            foreach (var edge_id in edge_ids)
            {
                edgeList += (edgeList.Length > 0 ? "," : "") + edge_id;
            }


            string queryString = "SELECT edge.tlid, edge.geometry FROM edge " +
                "WHERE edge.tlid IN ( " + edgeList + ");";
              

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
                        Item e = new Item();
                        e.tlid = reader["tlid"] as int? ?? default(int);
                        e.geometry = reader["geometry"] as string;
                       
                        edges.Add(e);
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
            return edges;
        }
    }
}