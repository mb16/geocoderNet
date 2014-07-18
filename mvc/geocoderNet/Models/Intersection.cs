using Elmah;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace geocoderNet.Models
{
    public class Intersection : Item
    {


        public static List<Item> intersections_by_fid(string[] fids)
        {
            List<Item> ranges = new List<Item>();

            string fidList = "";
            foreach (var fid in fids)
            {
                fidList += (fidList.Length > 0 ? "," : "") + fid;
            }


            //string subQuery = "SELECT fid, SUBSTRING (geometry,1,8) AS point FROM feature_edge, edge WHERE feature_edge.tlid = edge.tlid AND fid IN (" + fidList + ") " +
            //    "UNION " +
            //    "SELECT fid, SUBSTRING (geometry,LEN(geometry)-7,8) AS point FROM feature_edge, edge WHERE feature_edge.tlid = edge.tlid AND fid IN (" + fidList + ")";

            // Note, Geometry is not packed here, so return complete geometry field and parse later.
            string subQuery =
                "SELECT fid, geometry AS point FROM feature_edge, edge WHERE feature_edge.tlid = edge.tlid AND fid IN (" + fidList + ") ";// + 
            //"UNION " +
            //"SELECT fid, geometry AS point FROM feature_edge, edge WHERE feature_edge.tlid = edge.tlid AND fid IN (" + fidList + ")";


            string queryString = "SELECT a.fid AS fid1, b.fid AS fid2, a.point AS pointa, b.point AS pointb FROM (" + subQuery + ") a, (" + subQuery + ") b, feature f1, feature f2 " +
                "WHERE a.fid < b.fid AND f1.fid = a.fid AND f2.fid = b.fid AND f1.zip = f2.zip AND f1.paflag = 'P' AND f2.paflag = 'P';";
            // Note, original code had WHERE a.point = b.point AND, however we have full geometry text, hence these will never be joinable, must handle this requirement in calling function.

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

                        var pointa = Database.unpack_geometry((string)reader["pointa"]);
                        var pointb = Database.unpack_geometry((string)reader["pointb"]);

                        /* This replaces the join above.  Here we check that that pair of points has one lat/lon endpoint in common. */
                        if ((pointa[0][0] == pointb[0][0] && pointa[0][1] == pointb[0][1]) ||
                            (pointa[0][0] == pointb[pointb.Length - 1][0] &&
                             pointa[0][1] == pointb[pointb.Length - 1][1]) ||
                            (pointa[pointa.Length - 1][0] == pointb[0][0] &&
                             pointa[pointa.Length - 1][1] == pointb[0][1]) ||
                            (pointa[pointa.Length - 1][0] == pointb[pointb.Length - 1][0] &&
                             pointa[pointa.Length - 1][1] == pointb[pointb.Length - 1][1])
                            )
                        {

                            Item r = new Item();
                            r.fid1 = reader["fid1"] as int? ?? default(int);
                            r.fid2 = reader["fid2"] as int? ?? default(int);
                            r.point = reader["pointa"] as string;

                            ranges.Add(r);
                        }
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