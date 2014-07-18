using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace geocoderNet.Models
{

    public class Match
    {
        // FIXME: shouldn't have to anchor :number and :zip at start/end
        //public static Regex number = new Regex(@"^(\d+\W|[a-z]+)?(\d+)([a-z]?)\b", RegexOptions.IgnoreCase); // original

        /* Attempt to also capture patters like 10 - 12 elm street, and 10 & 12 & 13 elem street */
        public static Regex number = new Regex(@"^(\d+\W|[a-z]+)?(\d+)([a-z]*)(\s*[&-,/]?\s*[a-z]*\d+[a-z]*)*\b", RegexOptions.IgnoreCase);
        
        public static Regex street = new Regex(@"(?:\b(?:\d+\w*|[a-z'-]+)\s*)+", RegexOptions.IgnoreCase);
        /* NOTE, set right to left on city.  This is because if a suite or apt number exists in the string, it matches
         * the left half and not the right.  But the right most likely has the city.  Hence we go right to left incase
         * a number exists in the string. */
        public static Regex city = new Regex(@"(?:\b[a-z'-]+\s*)+", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
        public static Regex state = new Regex(@"" + Constants.RegexSource(Constants.State) + @"\s*$", RegexOptions.IgnoreCase);
        public static Regex zip = new Regex(@"(\d{5})(-\d{4})?\s*$", RegexOptions.RightToLeft);
        public static Regex at = new Regex(@"\s(at|@|and|&)\s", RegexOptions.IgnoreCase);
        public static Regex po_box = new Regex(@"\b[P|p]*(OST|ost)*\.*\s*[O|o|0]*(ffice|FFICE)*\.*\s*[B|b][O|o|0][X|x]\b");
    }

    public class Address
    {

        public string text { get; set; }
        public string prenum { get; set; }
        public string number { get; set; }
        public string sufnum { get; set; }
        public List<string> street { get; set; }
        public List<string> city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string plus4 { get; set; }

        public string full_state { get; set; }

        // this should only be used for testing.
        public Address()
        {
            street = new List<string>();
            city = new List<string>();
        }

        public Address(string txt)
        {

            street = new List<string>();
            city = new List<string>();

            if (!String.IsNullOrEmpty(txt))
            {
                text = clean(txt);
                parse();
            }
        }


        public string clean(string value)
        {
            // removed single quote from this list, as it causes problems in sql, and it seems not to appear in the database. Removed comma too.
            var rgx1 = new Regex(@"[^a-z0-9 &@\/-]+", RegexOptions.IgnoreCase);
            var rgx2 = new Regex(@"\s+");
            value = value.Trim();
            value = rgx1.Replace(value, " "); // replace with space so as not to collapse items together, remove duplicate spaces next.
            value = rgx2.Replace(value, " ");
            return value;
        }

        public IEnumerable<string> expand_numbers(string str)
        {
            int num = -1;
            string match = "";
            var basic = new Regex(@"\b(\d)+(?:st|nd|rd|th)?\b", RegexOptions.RightToLeft);
            var m = basic.Match(str);

            var strings = new List<string>();

            if (m.Success && m.Groups.Count > 1)
            {
                num = Convert.ToInt32(m.Groups[1].Value);
                match = m.Value;

                if (num != -1 && num < 100)
                {
                    strings.Add(str.Replace(m.Value, num.ToString()));
                    strings.Add(str.Replace(m.Value, Numbers.Ordinals().Count < num ? Numbers.Ordinals()[num] : ""));
                    strings.Add(str.Replace(m.Value, Numbers.Cardinals().Count < num ? Numbers.Cardinals()[num] : ""));
                }
            }
            
            if (num == -1)
            {
                m = Numbers.OrdinalsRegex().Match(str);
                if (m.Success)
                {
                    num = Numbers.Ordinals().IndexOf(m.Value);
                    match = m.Value;
                    if (num != -1 && num < 100)
                    {
                        strings.Add(str.Replace(m.Value, num.ToString()));
                        strings.Add(str.Replace(m.Value, Numbers.Numerics().Count < num ? Numbers.Numerics()[num] : ""));
                        strings.Add(str.Replace(m.Value, Numbers.Cardinals().Count < num ? Numbers.Cardinals()[num] : ""));
                    }
                }
            }
            if (num == -1)
            {
                m = Numbers.CardinalsRegex().Match(str);
                if (m.Success)
                {
                    num = Numbers.Cardinals().IndexOf(m.Value);
                    match = m.Value;
                    if (num != -1 && num < 100)
                    {
                        strings.Add(str.Replace(m.Value, num.ToString()));
                        strings.Add(str.Replace(m.Value, Numbers.Ordinals().Count < num ? Numbers.Ordinals()[num] : ""));
                        strings.Add(str.Replace(m.Value, Numbers.Numerics().Count < num ? Numbers.Numerics()[num] : ""));
                    }
                }
            }
            
            
            if (num == -1 || num >= 100 || !String.IsNullOrEmpty(match))
                strings.Add(str);

            return strings;
        }

        public string parse_zip(System.Text.RegularExpressions.Match regex_match, string text)
        {
            if (regex_match == null || regex_match.Groups.Count == 0) return text;
            var idx = text.LastIndexOf(regex_match.Value);
            text = text.Substring(0, idx).Trim();
            text = Regex.Replace(text, @"\s*,?\s*$", "", RegexOptions.RightToLeft);

            if (regex_match.Groups.Count > 0 && regex_match.Groups[1].Captures.Count > 0) 
                zip = regex_match.Groups[1].Captures[0].Value.Trim();
            if (regex_match.Groups.Count > 1 && regex_match.Groups[2].Captures.Count > 0) 
                plus4 = regex_match.Groups[2].Captures[0].Value.Trim().Replace("-", "");
            
            return text;
        }

        public string parse_state(System.Text.RegularExpressions.Match regex_match, string text)
        {
            if (regex_match == null || regex_match.Groups.Count == 0) return text;
            var idx = text.LastIndexOf(regex_match.Value);
            text = text.Substring(0, idx);
            text = Regex.Replace(text, @"\s*,?\s*$", "", RegexOptions.RightToLeft);
            if (regex_match.Groups[1].Captures.Count > 0) full_state = regex_match.Groups[1].Captures[0].Value;
            state = Constants.State.ContainsKey(regex_match.Groups[1].Captures[0].Value) ? Constants.State[regex_match.Groups[1].Captures[0].Value] : full_state;
            return text;
        }

        public string parse_number(System.Text.RegularExpressions.Match regex_match, string text)
        {
            if (regex_match == null || regex_match.Groups.Count == 0) return text;
            var idx = text.IndexOf(regex_match.Value);
            text = text.Substring(0, idx) + text.Substring(idx + regex_match.Value.Length).Trim();
            text = Regex.Replace(text, @"/\s*,?\s*$/", "", RegexOptions.RightToLeft);
            if (regex_match.Groups.Count > 0 && regex_match.Groups[1].Captures.Count > 0) prenum = regex_match.Groups[1].Captures[0].Value;
            if (regex_match.Groups.Count > 1 && regex_match.Groups[2].Captures.Count > 0) number = regex_match.Groups[2].Captures[0].Value;
            if (regex_match.Groups.Count > 2 && regex_match.Groups[3].Captures.Count > 0) sufnum = regex_match.Groups[3].Captures[0].Value;
            return text;
        }

        public void parse()
        {
            var txt = text.ToLower();

            var match = Match.zip.Match(txt);
            if (match.Success) zip = match.Value;
            if (!String.IsNullOrEmpty(zip))
                txt = parse_zip(match, txt);
            else
                zip = plus4 = "";

            match = Match.state.Match(txt);
            if (match.Success) state = match.Value;
            if (!String.IsNullOrEmpty(state))
                txt = parse_state(match, txt);
            else
                full_state = state = "";

            match = Match.number.Match(txt);
            //if (match.Success) number = Convert.ToInt32(match.Value);
            if (match.Success)
                txt = parse_number(match, txt);
            else
            {
                prenum = number = sufnum = "";
            }

            match = Match.street.Match(txt);
            if (match.Success) street = new List<string>() { match.Value };
            street = expand_streets(street);

            // SPECIAL CASE: 1600 Pennsylvania 20050
            if ((street == null || street.Count == 0) && state.ToLower() != full_state.ToLower()) street = new List<string>() { full_state };


            match = Match.city.Match(txt);
            if (match.Success) city.Add(match.Value);
            if (city != null && city.Count > 0)
            {
                city = new List<string>() { city[city.Count - 1].Trim() };
                List<string> add = city.Select(i =>
                {
                    foreach (var key in Constants.Name_Abbr.Keys)
                    {
                        var rgx = new Regex(@"\b" + key + @"\b", RegexOptions.IgnoreCase); // fix this to match correctly.
                        i = rgx.Replace(i, Constants.Name_Abbr[key]);
                    }
                    return i;
                }).ToList();

                city.AddRange(add);
                city = city.Select(s => s.ToLower()).ToList();
                city = city.Distinct().ToList();
            }
            else
                city = new List<string>();

            // SPECIAL CASE: no city, but a state with the same name. e.g. "New York"
            if (state != null && full_state != null && state.ToLower() != full_state.ToLower())
                city.Add(full_state);

            text = txt; // assign back after everything is parsed.
        }

        public List<string> expand_streets(List<string> street)
        {
            if (street != null && street.Count > 0 && !String.IsNullOrEmpty(street[0]))
            {
                street.ForEach(s => s.Trim());
                List<string> add = street.Select(i =>
                {
                    foreach (var key in Constants.Name_Abbr.Keys)
                    {
                        var rgx = new Regex(@"\b" + key + @"\b", RegexOptions.IgnoreCase); // fix this to match correctly.
                        i = rgx.Replace(i, Constants.Name_Abbr[key]);
                    }
                    return i;
                }).ToList();
                street.AddRange(add);

                var Std_Abbr = Constants.Std_Abbr();
                add = street.Select(i =>
                {
                    foreach (var key in Std_Abbr.Keys)
                    {
                        var rgx = new Regex(@"\b" + key + @"\b", RegexOptions.IgnoreCase); // fix this to match correctly.
                        i = rgx.Replace(i, Std_Abbr[key]);
                    }
                    return i;
                }).ToList();

                street.AddRange(add);
                street = street.SelectMany(i => expand_numbers(i)).ToList();
                street.ForEach(s => s.Trim());
                return street.Distinct().ToList();
            }
            else
            {
                return new List<string>();
            }
            return street;
        }

        public List<string> street_parts()
        {
            var strings = new List<string>();
            // builds list forwards.
            street.ForEach(s =>
            {
                var tokens = s.Trim().Split(' ');
                for (int i = 0; i  <= tokens.Length - 1 ; i++)
                {
                    string temp = "";
                    for (int j = 0; j <= i; j++)
                        temp += " " + tokens[j];
                    strings.Add(temp.Trim());
                }

            });

            // builds list backwards. minimum length of two words.  sometimes there might be address numnber junk on the front which the previous code will not exclude
            street.ForEach(s =>
            {
                var tokens = s.Trim().Split(' ');
                if (tokens.Count() > 2)
                {
                    for (int i = tokens.Length - 2; i >= 0; i--)
                    {
                        string temp = "";
                        for (int j = i; j <= tokens.Length - 1; j++)
                            temp += tokens[j] + " ";
                        strings.Add(temp.Trim());
                    }
                }

            });

            strings = remove_noise_words(strings).ToList();

            var t1 = strings.Where(s => Constants.Std_Abbr().ContainsKey(s) || Constants.Name_Abbr.ContainsKey(s));
            if (t1 == null || t1.Count() == 0 && number != null)
                strings.Add(number.ToString());
            return strings.Distinct().ToList();
        }

        public IEnumerable<string> remove_noise_words(List<string> strings)
        {
            // Don't return strings that consist solely of abbreviations.
            // NOTE: Is this a micro-optimization that has edge cases that will break?
            // Answer: Yes, it breaks on simple things like "Prairie St" or "Front St" 
            Regex prefix_qal = new Regex(@"^" + Constants.RegexSource(Constants.Prefix_Qualifier) + @"\s+", RegexOptions.IgnoreCase);
            /* But we bind to beginning of string, because the suffix qualifiers can also be at the beginning of strings, and sometimes following the prefix qualifiers. */
            Regex suffix_qal = new Regex(@"^" + Constants.RegexSource(Constants.Suffix_Qualifier) + @"\s+", RegexOptions.IgnoreCase);
            Regex prefix = new Regex(@"^" + Constants.RegexSource(Constants.Prefix_Type()) + @"\s+", RegexOptions.IgnoreCase);
            /* Note changed from \s* to \s+, which requires atleast one space.  This prevents the situation where we have one item in the string,
             * say court street, which got reduced to 'court', and now it will get replaced with nothing.  there is no value in this, and reducing
             * the single remaining item to nothing means we loos information that could be valuable.  Plus, if any suffix item, such as 'court' happens
             * to be a street name, then we have no chance of matching since it will always be eliminated.  For suffix and sufdxn. */
            Regex suffix = new Regex(@"\s+" + Constants.RegexSource(Constants.Suffix_Type()) + @"$", RegexOptions.IgnoreCase);
            Regex predxn = new Regex(@"^" + Constants.RegexSource(Constants.Directional) + @"\s+", RegexOptions.IgnoreCase);
            Regex sufdxn = new Regex(@"\s+" + Constants.RegexSource(Constants.Directional) + @"$", RegexOptions.IgnoreCase);
            var good_strings = strings.Select(s =>
            {
                var s1 = s.Clone().ToString().Trim();
                s1 = prefix_qal.Replace(s1, "");
                s1 = suffix_qal.Replace(s1, "");
                s1 = prefix.Replace(s1, "");
                s1 = suffix.Replace(s1, "");
                s1 = predxn.Replace(s1, "");
                s1 = sufdxn.Replace(s1, "");
                return s1;
            }).ToList();


            good_strings = good_strings.Where(s => !String.IsNullOrEmpty(s)).ToList();
            var t = good_strings.Where(s => !Constants.Std_Abbr().ContainsKey(s) && !Constants.Name_Abbr.ContainsKey(s)).ToList();
            if (t != null && t.Count > 0)
                strings = good_strings;
            return strings;
        }

        public List<string> city_parts()
        {
            var strings = new List<string>();
            // builds list backwards
            city.ForEach(s =>
            {
                var tokens = s.Split(' ');
                for (int i = tokens.Length - 1; i >= 0 ; i--)
                {
                    string temp = "";
                    for (int j = i; j < tokens.Length; j++)
                        temp += tokens[j] + " ";
                    strings.Add(temp.Trim());
                }

            });

            var good_strings = strings.Where(s => !Constants.Std_Abbr().ContainsKey(s)).ToList();
            if (good_strings != null && good_strings.Count > 0)
                strings = good_strings;
            return strings.Distinct().ToList();
        }

        // not clear this is being used. ***********
        public List<string> city_x(List<string> strings)
        {
            //NOTE: This will still fail on: 100 Broome St, 33333 (if 33333 is
            //Broome, MT or what)
            strings = expand_streets(strings);
            var match = new Regex(@"\s*\b(?:" + String.Join("|", strings) + @")\b\s*$", RegexOptions.IgnoreCase);
            // only remove city from street strings if address was parsed
            if (!String.IsNullOrEmpty(text))
            {
                street.ForEach(s => match.Replace(s, ""));
                street = street.Where(s => !String.IsNullOrEmpty(s)).ToList();
            }
            return strings;
        }


        public bool po_box()
        {
            var match = Match.po_box.Match(text);
            return match.Success;
        }

        public bool intersection()
        {
            var match = Match.at.Match(text);
            return match.Success;
        }
    }
}