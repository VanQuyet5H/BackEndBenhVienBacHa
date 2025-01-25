using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Helpers
{
    public static class StringHelper
    {

        /// <summary>
        /// Use to upper case for the fist character
        /// </summary>
        public static string ToUpperCaseTheFirstCharacter(this string model)
        {
            if (string.IsNullOrEmpty(model)) return string.Empty;

            var lstModelSplit = model.Split(" ");

            var lstModelSplitUpper = new List<string>();

            foreach (var modelSplit in lstModelSplit)
            {
                lstModelSplitUpper.Add(modelSplit.Length > 0 ? (char.ToUpper(modelSplit[0]) + modelSplit.Substring(1)) : modelSplit);
            }
            return string.Join(" ", lstModelSplitUpper);
        }
    }
}
