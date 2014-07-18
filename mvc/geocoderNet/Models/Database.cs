using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using geocoderNet.WellKnowBinary;
using Microsoft.Ajax.Utilities;
using WebGrease;

namespace geocoderNet.Models
{
    public class Database
    {

        private double Street_Weight = 3.0;
        private double Number_Weight = 2.0;
        private double Parity_Weight = 1.25;
        private double City_Weight = 1.0;

        public Database()
        {


        }

        public List<string> unique_values(List<Item> rows, string key)
        {
            return rows.Select(o => o.getValue(key)).Distinct().ToList();
        }

        public Dictionary<List<string>, List<Item>> rows_to_h(List<Item> rows, string[] keys)
        {
            var hash = new Dictionary<List<string>, List<Item>>();

            if (rows == null) return hash;

            var tempKeyList = new List<List<string>>();
            foreach (var row in rows)
            {
                // must do this because of reference to List.
                var keyPair = row.values_at(keys);
                var tempKey = tempKeyList.FirstOrDefault(o => Utilities.IsStringListSame(o, keyPair));

                if (tempKey == null)
                {
                    tempKeyList.Add(keyPair);
                    tempKey = keyPair;
                }

                if (!hash.ContainsKey(tempKey))
                    hash.Add(tempKey, new List<Item>());
                hash[tempKey].Add(row);

            }
            return hash;
        }

        public List<Item> merge_rows(List<Item> dest, List<Item> src, string[] keys)
        {
            var newDest = new List<Item>();
            var newSrc = rows_to_h(src, keys);
            foreach (var row in dest)
            {
                var vals = row.values_at(keys);
                var srcItemDict = newSrc.Where(o => Utilities.IsStringListSame(o.Key, vals));
                bool didAdd = false;
                if (srcItemDict != null)
                    foreach (var srcItemD in srcItemDict)
                    {
                        foreach (var srcItem in srcItemD.Value)
                        {
                            newDest.Add(Item.Merge(Item.Merge(new Item(), row), srcItem));
                            didAdd = true;
                        }
                    }

                if (!didAdd)
                    newDest.Add(Item.Merge(new Item(), row));

            }
            return newDest;
        }

        public List<Item> find_candidates(Address address)
        {
            var places = new List<Item>();
            var candidates = new List<Item>();

            string city = address.city.OrderBy(o => o.Length).FirstOrDefault();
            if (!String.IsNullOrEmpty(address.zip))
            {
                places = Place.places_by_zip(city, address.zip);
            }
            if (places == null || places.Count == 0)
                places = Place.places_by_city(city, address.city_parts(), address.state);

            if (places == null || places.Count == 0) return null;

            address.city = unique_values(places, "city");
            if (address.street == null || address.street.Count == 0) return places;

            var zips = unique_values(places, "zip");
            var street = address.street.OrderBy(o => o.Length).FirstOrDefault();
            candidates = Feature.features_by_street_and_zip(new List<string>(){ street}, address.street_parts(), zips.ToArray());

            if (candidates == null || candidates.Count == 0)
            {
                candidates = Feature.more_features_by_street_and_zip(new List<string>() { street }, address.street_parts(), zips.ToArray());
            }

            candidates = merge_rows(candidates, places, new string[] { "zip" });
            return candidates;
        }

        public List<Item> filter_by_score(List<Item> items, double score, string key)
        {
            var result = new List<Item>();
            var val = 0.0;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (Double.TryParse(item.getValue(key), out val) && val < score)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }


        public bool filter_by_score_is_empty(List<Item> items, double score, string key)
        {
            var lst = filter_by_score(items, score, key);
            if (lst == null || lst.Count() == 0)
                return true;
            return false;
        }

        public List<Item> find_candidates_new(Address address)
        {
            var score_threshold = 0.65;
            var city_score_threshold = 0.5;

            var places = new List<Item>();
            var candidates = new List<Item>();

            var lowest_city_score = 1.0;

            string city = ""; //= address.city.OrderBy(o => o.Length).FirstOrDefault();
            if (address.city != null)
            {
                foreach (var cty in address.city)
                {
                    if (cty != null && !String.IsNullOrEmpty(address.zip))
                    {
                        var parts = cty.Split(' ');
                        var count = parts.Count();
                        var temp_city = "";

                        for (var i = (count - 1); i >= 0; i--)
                        {
                            temp_city = (parts[i] + " " + temp_city).Trim();
                            var temp_places = Place.places_by_zip(temp_city, address.zip);

                            if (temp_places != null && temp_places.Count() > 0)
                            {
                                foreach (var a_place in temp_places)
                                {
                                    if (a_place.city_score < lowest_city_score)
                                    {
                                        places = temp_places;
                                        city = cty;
                                        lowest_city_score = a_place.city_score;
                                    }
                                }

                            }
                        }
                    }
                }
            }

            //places = filter_by_score(places, city_score_threshold, "city_score");

            if (filter_by_score_is_empty(places, city_score_threshold, "city_score"))
            {
                if (places == null)
                    places = new List<Item>();

                if (city != null && city != "")
                    places.AddRange(Place.places_by_city(city, address.city_parts(), address.state));
                else
                {

                    foreach (var mCity in address.city)
                    {
                        var tempPlaces = Place.places_by_city(mCity, address.city_parts(), address.state);
                        if (tempPlaces != null && tempPlaces.Count > 0)
                            places.AddRange(tempPlaces);
                    }
                }
            }

            if (places == null || places.Count == 0) return null;


            address.city = unique_values(places, "city");


            if (address.city != null && address.street != null)
            {
                var newStreets = new List<string>();
                foreach (var ct in address.city)
                {
                    foreach (var st in address.street)
                    {
                        if (st.ToLower().EndsWith(ct.ToLower()))
                            newStreets.Add(st.Substring(0, st.ToLower().LastIndexOf(ct.ToLower())));
                        else
                            newStreets.Add(st);
                    }
                }
                address.street = newStreets.Distinct().ToList();
            }

            if (address.street == null || address.street.Count == 0) return places;

            var zips = unique_values(places, "zip");
            //var street = address.street.OrderBy(o => o.Length).FirstOrDefault();
            

            candidates.AddRange(Feature.features_by_street_and_zip(address.street, address.street_parts(), zips.ToArray()));


            //candidates = filter_by_score(candidates, score_threshold, "street_score");

            if (filter_by_score_is_empty(candidates, score_threshold, "street_score") && !String.IsNullOrEmpty(address.zip) &&
                !String.IsNullOrEmpty(address.state))
            {
                if (candidates == null)
                    candidates = new List<Item>();

                var extrazips = Place.zips_by_county(address.zip, address.state);
                if (extrazips != null && extrazips.Count > 0)
                {

                    candidates.AddRange(Feature.features_by_street_and_zip(address.street, address.street_parts(), extrazips.ToArray()));
                    
                    foreach (var zip in extrazips)
                    {
                        places.AddRange(Place.places_by_zip(city, zip));
                    }
                }
            }

            if (filter_by_score_is_empty(candidates, score_threshold, "street_score"))
            {
                if (candidates == null)
                    candidates = new List<Item>();
                
                candidates.AddRange(Feature.more_features_by_street_and_zip(address.street, address.street_parts(), zips.ToArray()));

            }

            if (candidates == null) return null;

            candidates = merge_rows(candidates, places, new string[] { "zip" });
            return candidates;
        }


        public List<Item> assign_number(int hn, List<Item> candidates)
        {
            if (hn < 0) hn = 0;
            foreach (var candidate in candidates)
            {
                var fromhn = candidate.fromhn;
                var tohn = candidate.tohn;
                if ((hn >= fromhn && hn <= tohn) || (hn <= fromhn && hn >= tohn))
                {
                    candidate.number = hn;
                    candidate.precision = Item.PRECISION_RANGE;
                }
                else
                {
                    candidate.number = (Math.Abs(hn - fromhn) < Math.Abs(hn - tohn) ? candidate.fromhn : candidate.tohn);
                    candidate.precision = Item.PRECISION_STREET;
                }
            }
            return candidates.ToList();
        }

        public List<Item> add_ranges(Address address, List<Item> candidates)
        {
            var number = 0;
            if (Int32.TryParse(address.number, out number))
            {

                var fids = unique_values(candidates, "fid");
                var ranges = FeatureEdgeRange.ranges_by_feature(fids.ToArray(), number, address.prenum);
                if (ranges == null || ranges.Count == 0)
                    ranges = FeatureEdgeRange.ranges_by_feature(fids.ToArray(), number, null);
                candidates = merge_rows(candidates, ranges, new string[] { "fid" });
                candidates = assign_number(number, candidates);
            }
            return candidates;
        }

        public List<string> merge_edges(ref List<Item> candidates)
        {
            var edge_ids = unique_values(candidates, "tlid");
            var records = Edge.edges(edge_ids.ToArray());
            candidates = merge_rows(candidates, records, new string[] { "tlid" });
            candidates = candidates.Where(o => o.tlid != Item.INVALID_TLID).ToList();
            return edge_ids;
        }



        public List<Item> extend_ranges(List<Item> candidates)
        {
            var edge_ids = merge_edges(ref candidates);
            var full_ranges = Range.range_ends(edge_ids.ToArray());
            candidates = merge_rows(candidates, full_ranges, new string[] { "tlid", "side" });
            return candidates;
        }

        /* Score a list of candidates. For each candidate:
         * For each item in the query:
         ** if the query item is blank but the candidate is not, score 0.15;
            otherwise, if both are blank, score 1.0.
         ** If both items are set, compute the scaled Levenshtein-Damerau distance
            between them, and add that value (between 0.0 and 1.0) to the score.
         * Add 0.5 to the score for each numbered end of the range that matches
           the parity of the query number.
         * Add 1.0 if the query number is in the candidate range, otherwise
           add a fractional value for the notional distance between the
           closest candidate corner and the query.
         * Finally, divide the score by the total number of comparisons.
           The result should be between 0.0 and 1.0, with 1.0 indicating a
           perfect match. */

        public void score_candidates(Address address, ref List<Item> candidates)
        {


            foreach (var candidate in candidates)
            {
                candidate.components = new Dictionary<string, object>();
                var compare = new string[3] { "prenum", "state", "zip" };
                double denominator = compare.Length + Street_Weight + City_Weight;

                double street_score = (1.0 - (double)candidate.street_score) * Street_Weight;
                candidate.components["street"] = street_score;
                double city_score = (1.0 - (double)candidate.city_score) * City_Weight;
                candidate.components["city"] = city_score;
                double score = street_score + city_score;



                foreach (var key in compare)
                {

                    string src = (string)address.GetType().GetProperty(key).GetValue(address, null);
                    src = String.IsNullOrEmpty(src) ? "" : src.ToLower();
                    string dest = (string)candidate.GetType().GetProperty(key).GetValue(candidate, null);
                    dest = String.IsNullOrEmpty(dest) ? "" : dest.ToLower();
                    double item_score = (src == dest) ? 1 : 0;
                    candidate.components["key"] = item_score;
                    score += item_score;
                }


                if (!String.IsNullOrEmpty(address.number))
                {
                    double parity = 0.0;
                    double subscore = 0.0;

                    int fromhn = Convert.ToInt32(candidate.fromhn);
                    int tohn = Convert.ToInt32(candidate.tohn);
                    int assigned = Convert.ToInt32(candidate.number);
                    int hn = Convert.ToInt32(address.number);

                    if (candidate.precision == Item.PRECISION_RANGE)
                        subscore += Number_Weight;
                    else if (assigned > 0)
                    {
                        // only credit number subscore if assigned
                        subscore += (double)Math.Abs(Number_Weight / (assigned - hn));
                    }
                    candidate.components["number"] = subscore;

                    if (hn > 0 && assigned > 0)
                    {
                        // only credit parity if a number was given *and* assigned
                        if (fromhn % 2 == hn % 2)
                            parity += Parity_Weight / 2.0;
                        if (tohn % 2 == hn % 2)
                            parity += Parity_Weight / 2.0;
                    }

                    candidate.components["parity"] = parity;
                    score += subscore + parity;
                    denominator += Number_Weight + Parity_Weight;
                }


                candidate.components["total"] = (double)score;
                candidate.components["denominator"] = denominator;
                candidate.score = ((double)score) / denominator;

            }
        }


        public List<Item> best_candidates(List<Item> candidates)
        {
            if (candidates == null || candidates.Count == 0) return null;
            List<Item> newList = candidates.OrderByDescending(o => o.score).ToList();
            return newList.Where(o => o.score == newList[0].score).ToList();
        }

        public double interpolation_distance(Item candidate)
        {
            var fromhn = candidate.fromhn;
            var tohn = candidate.tohn;
            var number = candidate.number;
            if (fromhn > tohn)
            {
                var temp = fromhn;
                fromhn = tohn;
                tohn = temp;
            }
            if (fromhn > number) return 0.0;
            else if (tohn < number) return 1.0;
            else if (tohn == fromhn) return 0.0;
            else return ((double)(number - fromhn)) / ((double)(tohn - fromhn));
        }


        public static double[][] unpack_geometry(string geometry)
        {
            if (!String.IsNullOrEmpty(geometry))
            {

                var numberChars = geometry.Length;
                var bytes = new byte[numberChars / 2];
                for (var i = 0; i < numberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(geometry.Substring(i, 2), 16);

                WkbShape shape = WkbDecoder.Parse(bytes);

                if (shape.GetType() == typeof(WkbMultiLineString))
                {
                    var lineString = (WkbMultiLineString)shape;
                    if (lineString.LineString.Count() > 0)
                    {
                        var index = 0;
                        var points = new double[lineString.LineString[0].Points.Count()][];
                        foreach (var point in lineString.LineString[0].Points)
                        {
                            points[index] = new[] { point.X, point.Y };
                            index++;
                        }
                        return points;
                    }
                }
            }
            return new double[][] { };

        }

        // Calculate the longitude scaling for the average of two latitudes.
        public double scale_lon(double lat1, double lat2)
        {
            /* an approximation in place of lookup.rst (10e) and (10g)
            = scale longitude distances by the cosine of the latitude
            (or, actually, the mean of two latitudes)
            -- is this even necessary? */
            return Math.Cos((lat1 + lat2) / 2 * Math.PI / 180);
        }

        /* Simple Euclidean distances between two 2-D coordinate pairs, scaled
         along the longitudinal axis by scale_lon. */
        public double distance(double[] a, double[] b)
        {
            var dx = (b[0] - a[0]) * scale_lon(a[1], b[1]);
            var dy = (b[1] - a[1]);
            return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        }

        public double[] street_side_offset(double b, double[] p1, double[] p2)
        {
            // Find a point (x2, y2) that is at a distance b from a line A=(x0, y0)-(x1, y1)
            // along a tangent B passing through (x1,y1)-(x2,y2).
            // Let a = the length of line A 
            var a = Math.Sqrt(Math.Pow((p2[0] - p1[0]), 2.0) + Math.Pow((p2[1] - p1[1]), 2.0));
            // theta is the angle between the line A and the x axis
            var theta = Math.Atan2(p2[1] - p1[1], p2[0] - p1[0]);
            // Now lines A and B form a right triangle where the third vertex is (x2, y2).
            // Let c = the length of the hypotenuse line C=(x0,y0)-(x2,y2)
            var c = Math.Sqrt(Math.Pow(a, 2.0) + Math.Pow(b, 2.0));
            // Now alpha is the angle between lines A and C.
            var alpha = Math.Atan2(b, a);
            // Therefore the difference between theta and alpha is the angle between C and
            // the x axis. Since we know the length of C, the lengths of the two lines
            // parallel to the axes that form a right triangle C, and hence the
            // coordinates (x2, y2), fall out.
            return new double[] { p1[0] + c * Math.Cos(theta - alpha), p1[1] + c * Math.Sin(theta - alpha) };
        }

        /* Find an interpolated point along a list of linestring vertices
        proportional to the given fractional distance along the line.
        Offset is in degrees and defaults to ~8 meters. */
        public double[] interpolate(double[][] points, double fraction, int side, double offset = 0.000075)
        {
            if (fraction == 0.0) return points[0];
            if (fraction == 1.0) return points[points.Count() - 1];
            var total = 0.0;
            for (int n = 1; n < points.Count(); n++)
            {
                total += distance(points[n - 1], points[n]);
            }
            var target = total * fraction;
            for (int n = 1; n < points.Count(); n++)
            {
                if (points[n - 1] == points[n]) continue;
                var step = distance(points[n - 1], points[n]);
                if (step < target)
                    target -= step;
                else
                {
                    var scale = scale_lon(points[n][1], points[n - 1][1]);
                    var dx = (points[n][0] - points[n - 1][0]) * (target / step) * scale;
                    var dy = (points[n][1] - points[n - 1][1]) * (target / step);
                    var found = new double[] { points[n - 1][0] + dx, points[n - 1][1] + dy };
                    return street_side_offset(offset * side, points[n - 1], found);
                }
            }
            // in a pathological case, points[n-1] == points[n] for n==-1
            // so *sigh* just forget interpolating and return points[-1]
            return points[points.Count() - 1];
        }

        public List<Item> canonicalize_places(List<Item> candidates)
        {
            var zips_used = unique_values(candidates, "zip");
            var tempPlaces = Place.primary_places(zips_used.ToArray());


            var pri_places = rows_to_h(tempPlaces, new string[] { "zip" });

            foreach (var record in candidates)
            {
                var key = pri_places.Keys.FirstOrDefault(o => Utilities.IsStringListSame(o, new List<string>() { record.zip }));
                var current_places = key != null ? pri_places[key] : null;
                if (current_places == null || current_places.Count == 0) return null;
                var top_priority = current_places.Select(p => Convert.ToInt32(p.priority)).Min();

                // not sure if this is precisely correct..... **********************
                current_places.Where(p => Convert.ToInt32(p.priority) == Convert.ToInt32(top_priority))
                    .ForEach(p =>
                    {
                        record.city = p.city;
                        record.state = p.state;
                        record.fips_county = p.fips_county;
                    });
            }

            return candidates;

        }


        public void clean_record(Place record)
        {
            // skipped formatting score.  its double here, and format should be where display happens.
            // no need to clean up nulls, best handled when display happens.
            // skip delete of components.
            // can't delete properties since this is not a hash.
        }

        public List<Item> best_places(Address address, List<Item> places, bool canonicalize = false)
        {
            if (places == null || places.Count == 0) return null;

            score_candidates(address, ref places);
            places = best_candidates(places);
            if (canonicalize)
                places = canonicalize_places(places);

            var by_name = rows_to_h(places, new string[] { "city", "state" });

            var by_name2 = new Dictionary<List<string>, List<Item>>();

            if (by_name.Count > 0)
            {
                // WHy sort and then grab the first record??????
                foreach (var key in by_name.Keys)
                {
                    by_name2[key] = by_name[key].OrderBy(x => x.zip).ToList();
                }

                places = by_name2.Select(o => (o.Value.Count > 0 ? o.Value[0] : new Item())).ToList();
                //clean_record(places); // see comments in clean_record.

                places.ForEach(o => o.precision = (o.zip == address.zip ? Item.PRECISION_ZIP : Item.PRECISION_CITY));
            }

            return places;
        }

        public List<Item> geocode_place(Address address, bool canonicalize = false)
        {
            List<Item> places = null;

            if (!String.IsNullOrEmpty(address.zip))
                places = Place.places_by_zip(address.text, address.zip);
            if (places == null)
                places = Place.places_by_city(address.text, address.city_parts(), address.state);

            places = best_places(address, places, canonicalize);

            return places;
        }

        public List<Item> geocode_intersection(Address address, bool canonical_place = false, bool use_find_candidates_new = false)
        {
            List<Item> candidates;
            if (use_find_candidates_new)
                candidates = find_candidates_new(address);
            else
                candidates = find_candidates(address);
            if (candidates == null || candidates.Count == 0) return null;
            if (String.IsNullOrEmpty(candidates[0].street)) return best_places(address, candidates, canonical_place);
            var features = rows_to_h(candidates, new string[] { "fid" });
            var temp = features.Keys;
            var intersects = Intersection.intersections_by_fid(features.Keys.SelectMany(x => x).ToArray()); // SelectMany effectively does a flatten.
            foreach (var record in intersects)
            {
                var feat1 = features.Where(k => Convert.ToInt32(k.Key.FirstOrDefault()) == record.fid1).Select(k => k.Value.FirstOrDefault()).FirstOrDefault();
                var feat2 = features.Where(k => Convert.ToInt32(k.Key.FirstOrDefault()) == record.fid2).Select(k => k.Value.FirstOrDefault()).FirstOrDefault();
                record.Merge(feat1);
                record.street1 = record.street;
                record.street = null;
                record.street2 = (feat2 != null ? feat2.street : "");
                var geo = unpack_geometry(record.point)[0];
                record.point = null;
                record.lon = geo[0];
                record.lat = geo[1];
                record.precision = Item.PRECISION_INTERSECTION;
                if (feat1 != null && feat2 != null)
                    record.street_score = ((double)feat1.street_score + (double)feat2.street_score) / 2.0; // not sure these have ever been scored???
            }

            score_candidates(address, ref intersects);
            intersects = best_candidates(intersects);

            var by_point = rows_to_h(intersects, new string[] { "lon", "lat" });
            candidates = by_point.Values.Select(r => r[0]).ToList();

            if (canonical_place)
                candidates = canonicalize_places(candidates);

            //candidates.each {|record| clean_record! record}
            return candidates;
        }

        public List<Item> geocode_address(Address address, bool canonical_place = false, bool use_find_candidates_new = false)
        {
            List<Item> candidates;
            if (use_find_candidates_new)
                candidates = find_candidates_new(address);
            else
                candidates = find_candidates(address);
            if (candidates == null || candidates.Count == 0) return null;
            if (String.IsNullOrEmpty(candidates[0].street)) return best_places(address, candidates, canonical_place);

            score_candidates(address, ref candidates);
            candidates = best_candidates(candidates);

            candidates = add_ranges(address, candidates);
            score_candidates(address, ref candidates);
            candidates = best_candidates(candidates);

            var by_tlid = rows_to_h(candidates, new string[] { "tlid" });
            candidates = by_tlid.Values.Select(o => (o.Count > 0 ? o[0] : null)).ToList();

            if (!String.IsNullOrEmpty(address.number))
                candidates = extend_ranges(candidates);
            else
            {
                var by_street = rows_to_h(candidates, new string[] { "street", "zip" });
                candidates = by_street.Values.Select(o => (o.Count > 0 ? o[0] : null)).ToList();

                /* (This section only runs when street number is not known.) The function below is commented
                 * because it merges based on tlid values.  But no tlid values have been acquired at this point,
                 * which results in all candidates being eliminated.  This doesn't appear to accomplish anything. 
                 * Example: "franklin st hagerstown md 21740" will return no results.
                 */
                //merge_edges(ref candidates);
            }


            foreach (var record in candidates)
            {
                var dist = interpolation_distance(record);
                if (!String.IsNullOrEmpty(record.geometry))
                {
                    var points = unpack_geometry(record.geometry);
                    var side = (record.side == "R" ? 1 : -1);
                    if (record.flipped)
                    {
                        side *= -1;
                        points = points.Reverse().ToArray();
                    }

                    var found = interpolate(points, dist, side);
                    record.lon = found[0];
                    record.lat = found[1];
                    // record[:lon], record[:lat] = found.map {|x| format("%.6f", x).to_f} // not a string here...
                }
            }

            if (canonical_place)
                candidates = canonicalize_places(candidates);

            //candidates.ForEach(o => clean_record(o));  // not using clean_record at this time.
            return candidates;
        }


        public string find_city_zip(Address address)
        {

            string zip = null;
            var places = new List<Item>();

            if (address.city != null && address.city.Count > 0)
            {
                foreach (var mCity in address.city)
                {
                    string[] parts = mCity.Split(' ');
                    string tempCity = "";
                    for ( var i = parts.Length - 1; i >= 0; i--)
                    {
                        tempCity = parts[i] + " " + tempCity; // build string backwards.
                        var tempPlaces = Place.places_by_city(mCity, new List<string>() { tempCity }, address.state);
                        if (tempPlaces != null && tempPlaces.Count > 0)
                            places.AddRange(tempPlaces);

                    }
                }

            }


            double bestScore = 1.0;
            foreach (var place in places)
            {
                if (place.city_score < bestScore)
                {
                    bestScore = place.city_score;
                    zip = place.zip;
                }
            }

            return zip;
        }



        public List<Item> geocode(string info_to_geocode, bool canonical_place, bool use_find_candidates_new = false)
        {
            List<Item> result = null;

            var address = new Address(info_to_geocode);

            if (address.city != null && address.city.Count > 0 && String.IsNullOrEmpty(address.zip))
            {
                // we make a shot at getting a zip in the city, and then try to match.
                address.zip = find_city_zip(address);
                if (String.IsNullOrEmpty(address.zip))
                    return null;
            }

            if (address.po_box() && !String.IsNullOrEmpty(address.zip))
                result = geocode_place(address, canonical_place);

            if (address.intersection() && address.street != null && address.street.Count > 0 && String.IsNullOrEmpty(address.number))
                result = geocode_intersection(address, canonical_place, use_find_candidates_new);

            if (result == null && address.street != null && address.street.Count > 0)
                result = geocode_address(address, canonical_place, use_find_candidates_new);

            if (result == null)
                result = geocode_place(address, canonical_place);

            return result;
        }



    }
}