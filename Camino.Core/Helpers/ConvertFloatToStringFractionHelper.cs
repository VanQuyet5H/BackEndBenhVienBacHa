using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Helpers
{
    //Hiển thị số double(float) thành dạng a/b
    public static class ConvertFloatToStringFractionHelper
    {
        private static Int64 Numerator;
        private static Int64 Denominator;
        private static string Fractions(double f, Int64 MaximumDenominator = 4096)
        {

            Int64 a;
            var h = new Int64[3] { 0, 1, 0 };
            var k = new Int64[3] { 1, 0, 0 };
            Int64 x, d, n = 1;
            int i, neg = 0;

            if (MaximumDenominator <= 1)
            {
                Denominator = 1;
                Numerator = (Int64)f;
                string.Format("{0}/{1}", Numerator, Denominator);
            }

            if (f < 0) { neg = 1; f = -f; }

            while (f != Math.Floor(f)) { n <<= 1; f *= 2; }
            d = (Int64)f;

            /* continued fraction and check denominator each step */
            for (i = 0; i < 64; i++)
            {
                a = (n != 0) ? d / n : 0;
                if ((i != 0) && (a == 0)) break;

                x = d; d = n; n = x % n;

                x = a;
                if (k[1] * a + k[0] >= MaximumDenominator)
                {
                    x = (MaximumDenominator - k[0]) / k[1];
                    if (x * 2 >= a || k[1] >= MaximumDenominator)
                        i = 65;
                    else
                        break;
                }

                h[2] = x * h[1] + h[0]; h[0] = h[1]; h[1] = h[2];
                k[2] = x * k[1] + k[0]; k[0] = k[1]; k[1] = k[2];
            }
            Denominator = k[1];
            Numerator = neg != 0 ? -h[1] : h[1];
            var result = string.Empty;
            if (Denominator == 1)
            {
                result = string.Format("{0}", Numerator);
            }
            else
            {
                result = string.Format("{0}/{1}", Numerator, Denominator);
            }
            return result;
        }
        public static string FloatToStringFraction(this double? number)
        {
            if (number != null)
            {
                return Fractions(number.Value, 32);
            }
            else
            {
                return null;
            }
        }
    }
}
