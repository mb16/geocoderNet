using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebGrease.Css.Extensions;

namespace geocoderNet.Models
{
    public class Numbers
    {

        private static Regex BuildRegex(IEnumerable<string> keys)
        {
            return new Regex(@"\b(" + String.Join("|", keys) + @")\b", RegexOptions.IgnoreCase);

        }


        // The Cardinals constant maps digits to cardinal number words and back.
        private static List<string> cardinal_tens = new List<string>
            {
            "twenty","thirty","forty","fifty","sixty","seventy","eighty","ninety"
            };
        public static Regex Cardinal_Tens()
        {
            return BuildRegex(cardinal_tens);
        }


        // The Cardinals constant maps digits to cardinal number words and back.
        private static List<string> cardinals = new List<string>
            {
            "zero", "one", "two", "three", "four", "five", "six", "seven",
            "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen",
            "sixteen", "seventeen", "eighteen", "nineteen"
            };
        public static List<string> Cardinals()
        {
            var temp = new List<string>();
            temp.AddRange(cardinals);
            temp.AddRange(cardinal_tens.SelectMany(t =>
            {
                var lst = new List<string>();
                lst.Add(t);
                for (int n = 1; n <= 9; n++)
                {
                    lst.Add(t + "-" + cardinals[n]);
                }
                return lst;
            }).ToList());

            return temp;
        }
        public static Regex CardinalsRegex()
        {
            return BuildRegex(Cardinals());
        }


        // The Ordinals constant maps digits to ordinal number words and back.
        private static List<string> ordinals = new List<string>
            {
            "zeroth", "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth", "ninth",
            "tenth","eleventh", "twelfth", "thirteenth", "fourteenth", "fifteenth", "sixteenth",
            "seventeenth","eighteenth", "nineteenth"
            };
        public static List<string> Ordinals()
        {
            var temp = new List<string>();
            temp.AddRange(ordinals);
            temp.AddRange(cardinal_tens.SelectMany(t =>
            {
                var lst = new List<string>();
                lst.Add(t.Replace("y", "ieth"));
                for (int n = 1; n <= 9; n++)
                {
                    lst.Add(t + "-" + ordinals[n]);
                }
                return lst;
            }).ToList());

            return temp;
        }
        public static Regex OrdinalsRegex()
        {
            return BuildRegex(Ordinals());
        }


        // The Ordinals constant maps digits to ordinal number words and back.
        private static List<string> numerics = new List<string>
            {
            "0th", "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th"
            };

        public static List<string> Numerics()
        {
            var temp = new List<string>();
            temp.AddRange(numerics);

            for (int j = 1; j <= 9; j++)
            {
                foreach (var num in numerics)
                {
                    temp.Add(j + num);
                }
            }

            return temp;
        }
        public static Regex NumericsRegex()
        {
            return BuildRegex(Numerics());
        }

    }
}