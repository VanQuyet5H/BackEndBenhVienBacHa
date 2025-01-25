using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Helpers
{
    public static class RandomHelper
    {
        public static string RandomPassCode(int num)
        {
            var rnd = new Random();
            var result = 1;
            for (var i = 1; i <= num; i++)
            {
                result = result * 10;
            }
            var code = rnd.Next(1, result);
            return code.ToString("D" + num);
        }

    }
}
