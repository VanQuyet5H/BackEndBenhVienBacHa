using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Helpers
{
    public static class ConvertStringFractionToFloatHelper
    {
        private static Double Parse(String input)
        {
            input = (input ?? String.Empty).Trim();
            //if (String.IsNullOrEmpty(input))
            //{
            //    throw new ArgumentNullException("input");
            //}
            // standard decimal number (e.g. 1.125)
            if (input.IndexOf('.') != -1 || (input.IndexOf(' ') == -1 && input.IndexOf('/') == -1 && input.IndexOf('\\') == -1))
            {
                Double result;
                if (Double.TryParse(input, out result))
                {
                    return result;
                }
            }

            String[] parts = input.Split(new[] { ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            // stand-off fractional (e.g. 7/8)
            if (input.IndexOf(' ') == -1 && parts.Length == 2)
            {
                Double num, den;
                if (Double.TryParse(parts[0], out num) && Double.TryParse(parts[1], out den))
                {
                    return num / den;
                }
            }

            //// Number and fraction (e.g.2 1/2) ==> input = "2 1/2" => result = 2.5
            //if (parts.Length == 3)
            //{
            //    Double whole, num, den;
            //    if (Double.TryParse(parts[0], out whole) && Double.TryParse(parts[1], out num) && Double.TryParse(parts[2], out den))
            //    {
            //        return whole + (num / den);
            //    }
            //}

            // Bogus / unable to parse
            return Double.NaN;
        }
        public static double? ToFloatFromFraction(this string number)
        {
            var convertNumber = Parse(number);
            if (!Double.IsNaN(convertNumber))
            {
                return Math.Round(convertNumber, 3);
            }
            else
            {
                return null;
            }
        }

        public static bool IsNumberFraction(this string input)
        {
            return ToFloatFromFraction(input) != null;
        }
    }
}
