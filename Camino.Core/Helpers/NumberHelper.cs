using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Helpers
{
    public static class NumberHelper
    {
        public static long LamTronPhiTheXe(this long sotien)
        {
            return decimal.ToInt64(Math.Round((decimal)sotien / (decimal)100, 0) * 100);
        }

        public static bool AlmostEqual(this double x, double y)
        {
            double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;
            return Math.Abs(x - y) <= epsilon;
        }

        public static bool AlmostEqual(this decimal x, decimal y)
        {
            decimal epsilon = new decimal(0.001);
            return Math.Abs(x - y) <= epsilon;
        }

        public static string ChuyenSoRaText(double total, bool tien = true)
        {
            try
            {
                string rs = "";
                total = Math.Round(total, 0);
                string[] ch = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string[] rch = { "lẻ", "mốt", "", "", "", "lăm" };
                string[] u = { "", "mươi", "trăm", "ngàn", "", "", "triệu", "", "", "tỷ", "", "", "ngàn", "", "", "triệu" };
                string nstr = total.ToString();

                int[] n = new int[nstr.Length];
                int len = n.Length;
                for (int i = 0; i < len; i++)
                {
                    n[len - 1 - i] = Convert.ToInt32(nstr.Substring(i, 1));
                }

                for (int i = len - 1; i >= 0; i--)
                {
                    if (i % 3 == 2)// số 0 ở hàng trăm
                    {
                        if (n[i] == 0 && n[i - 1] == 0 && n[i - 2] == 0) continue;//nếu cả 3 số là 0 thì bỏ qua không đọc
                    }
                    else if (i % 3 == 1) // số ở hàng chục
                    {
                        if (n[i] == 0)
                        {
                            if (n[i - 1] == 0) { continue; }// nếu hàng chục và hàng đơn vị đều là 0 thì bỏ qua.
                            else
                            {
                                rs += " " + rch[n[i]]; continue;// hàng chục là 0 thì bỏ qua, đọc số hàng đơn vị
                            }
                        }
                        if (n[i] == 1)//nếu số hàng chục là 1 thì đọc là mười
                        {
                            rs += " mười"; continue;
                        }
                    }
                    else if (i != len - 1)// số ở hàng đơn vị (không phải là số đầu tiên)
                    {
                        if (n[i] == 0)// số hàng đơn vị là 0 thì chỉ đọc đơn vị
                        {
                            if (i + 2 <= len - 1 && n[i + 2] == 0 && n[i + 1] == 0) continue;
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 1)// nếu là 1 thì tùy vào số hàng chục mà đọc: 0,1: một / còn lại: mốt
                        {
                            rs += " " + ((n[i + 1] == 1 || n[i + 1] == 0) ? ch[n[i]] : rch[n[i]]);
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 5) // cách đọc số 5
                        {
                            if (n[i + 1] != 0) //nếu số hàng chục khác 0 thì đọc số 5 là lăm
                            {
                                rs += " " + rch[n[i]];// đọc số 
                                rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                                continue;
                            }
                        }
                    }

                    rs += " " + ch[n[i]];// đọc số
                    rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                }
                if (tien)
                {
                    if (rs[rs.Length - 1] != ' ')
                        rs += " đồng";
                    else
                        rs += "đồng";
                }

                if (rs.Length > 2)
                {
                    string rs1 = rs.Substring(0, 2);
                    rs1 = rs1.ToUpper();
                    rs = rs.Substring(2);
                    rs = rs1 + rs;
                }
                return rs.Trim().Replace("lẻ,", "lẻ").Replace("mươi,", "mươi").Replace("trăm,", "trăm").Replace("mười,", "mười");
            }
            catch
            {
                return "";
            }
        }

        public static bool ContainNumber(string param)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            return regex.IsMatch(param);
        }
        public static double MathRoundNumber(this double number, int numberDigits = 1)
        {
            return Math.Round(number, numberDigits);
        }

        public static double? MathRoundNumber(this double? number, int numberDigits = 1)
        {
            if (number == null || number == 0)
            {
                return 0;
            }
            return Math.Round(number.Value, numberDigits);
        }

        public static double MathCelling(this double number)
        {
            return Math.Ceiling(number);
        }

        public static double? MathCelling(this double? number)
        {
            if (number == null || number == 0)
            {
                return 0;
            }
            return Math.Ceiling(number.Value);
        }
        public static bool IsNumeric(string value, EnumNumber type)
        {
            var result = false;
            if (type == EnumNumber.Integer)
            {
                result = long.TryParse(value, out long number);
            }
            else
            {
                result = double.TryParse(value, out double number);
            }
            return result;
        }
        //public static bool IsNumericDouble(object obj)
        //{
        //    if (Equals(obj, null))
        //    {
        //        return false;
        //    }

        //    Type objType = obj.GetType();
        //    objType = Nullable.GetUnderlyingType(objType) ?? objType;

        //    if (objType.IsPrimitive)
        //    {
        //        return objType != typeof(bool) &&
        //            objType != typeof(char) &&
        //            objType != typeof(IntPtr) &&
        //            objType != typeof(UIntPtr);
        //    }

        //    return objType == typeof(double);
        //}
    }
}