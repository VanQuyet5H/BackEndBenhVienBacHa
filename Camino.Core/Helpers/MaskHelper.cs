using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Helpers
{
    public static class MaskHelper
    {
        public static string RemoveFormatPhone(this string maskPhoneNumber)
        {
            if (!string.IsNullOrEmpty(maskPhoneNumber))
            {
                var phone = maskPhoneNumber.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "")
                    .Replace("_", "");
                return phone;
            }
            return maskPhoneNumber;
        }

        public static string ApplyFormatPhone(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return string.Empty;

            phoneNumber = RemoveFormat(phoneNumber);

            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length != 10) return phoneNumber;

            return string.Format("{0:000 000 0000}", double.Parse(phoneNumber));
        }

        public static string ApplyFormatSsn(this string number)
        {
            if (string.IsNullOrEmpty(number)) return "";

            number = RemoveFormat(number);

            if (string.IsNullOrEmpty(number) || number.Length != 9) return number;

            return string.Format("{0:000-00-0000}", double.Parse(number));
        }

        public static string RemoveFormat(this string number)
        {
            return string.IsNullOrEmpty(number) ? number : Regex.Replace(number, "[^.0-9]+", "");
        }

        public static string ApplyFormatMoney(this double? money)
        {
            if (money == null) return string.Empty;
            return ((double)money).ToString("C");
        }


        public static string RemoveNpiFormat(this string number)
        {
            return number.Replace("_", "");
        }

        public static string RemoveAeFormat(this string number)
        {
            return number.Replace("_", "");
        }

        public static bool IsFormatPhone(this string phoneNumber)
        {
            var match = Regex.Match(phoneNumber, @"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}");
            return match.Success;
        }

        public static int ParseToInt(this string number)
        {
            int result;
            int.TryParse(number, out result);
            return result;
        }
        public static string RemoveFormatFax(this string maskFaxNumber)
        {
            if (!string.IsNullOrEmpty(maskFaxNumber))
            {
                var fax = maskFaxNumber.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "")
                    .Replace("_", "");
                return fax;
            }
            return maskFaxNumber;
        }

        public static string ApplyFormatFax(this string faxNumber)
        {
            if (string.IsNullOrEmpty(faxNumber)) return string.Empty;

            faxNumber = RemoveFormat(faxNumber);

            if (string.IsNullOrEmpty(faxNumber) || faxNumber.Length != 10) return faxNumber;

            return string.Format("{0:000 000 0000}", double.Parse(faxNumber));
        }

        public static string ApplyFormatMoneyVND(this long result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            var resultStr = string.Format(cul, "{0:n2}", result);
            return string.IsNullOrEmpty(resultStr) ? "0" : resultStr;
        }
        public static string ApplyFormatMoneyVNDToDouble(this double result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            var resultStr = string.Format(cul, "{0:n2}", result);
            return string.IsNullOrEmpty(resultStr) ? "0" : resultStr;
        }
        public static string ApplyFormatMoneyVND(this decimal result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            var resultStr = string.Format(cul, "{0:n2}", result);
            return string.IsNullOrEmpty(resultStr) ? "0" : resultStr;
        }

        public static string ApplyFormatMoneyVND(this decimal result,string format= "{0:n2}")
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            var resultStr = string.Format(cul, format, result);
            return string.IsNullOrEmpty(resultStr) ? "0" : resultStr;
        }
        public static string ApplyFormatMoneyToDouble(this double result, bool dongNgoacSoAm = false)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            if (dongNgoacSoAm && !result.AlmostEqual(0) && result < 0)
            {
                var resultStrDuong = string.Format(cul, "{0:n2}", result * -1);
                return string.IsNullOrEmpty(resultStrDuong) ? "0" : $"({resultStrDuong.Replace(" ₫", "")})";
            }

            var resultStr = string.Format(cul, "{0:n2}", result);
            return string.IsNullOrEmpty(resultStr) ? "0" : resultStr.Replace(" ₫", "");
        }

        public static string RoundAndApplyFormatMoney(this double result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");

            var resultStr = string.Format(cul, "{0:n0}", Math.Round(result, MidpointRounding.AwayFromZero));
            return string.IsNullOrEmpty(resultStr) ? "0" : resultStr.Replace(" ₫", "");
        }

        public static string ApplyNumber(this long result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return result.ToString("#,##", cul.NumberFormat);
        }
        public static string ApplyNumber(this double? result)
        {
            if (result == null) return "0";
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return (result ?? 0).ToString("#,##", cul.NumberFormat);
        }

        public static string ApplyNumber(this double result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return result.ToString("#,##", cul.NumberFormat);
        }
        public static string ApplyNumber(this decimal result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return result.ToString("#,##", cul.NumberFormat);
        }
        //public static string ApplyNumber(this object result)
        //{
        //    if (result == null)
        //    {
        //        return "";
        //    }
        //    var resultDouble = Convert.ToDouble(result);
        //    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
        //    return resultDouble.ToString("#,##", cul.NumberFormat);
        //}
        public static string ApplyNumber(this int result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return result.ToString("#,##", cul.NumberFormat);
        }

        public static string ApplyVietnameseFloatNumber(this double result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return result.ToString("#,###.###", cul.NumberFormat);
        }

        public static string ApplyVietnameseFloatNumber(this decimal result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return result.ToString("#,###.###", cul.NumberFormat);
        }
        public static string ApplyFormatMoneyVND(this decimal? result)
        {
            if (result == null)
                return string.Empty;
            return result.Value.ApplyVietnameseFloatNumber();
        }

        //Loại bỏ chữ có dấu, chữ số và các kí tự đặc biệt
        public static string RemoveDiacritics(this string result)
        {
            if (result == null)
                return string.Empty;
            Regex regex = new Regex(@"[^\s\p{L}A-Za-z_.,]");
            string temp = result.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static string RemoveNumberInString(this string result)
        {
            if (result == null)
                return string.Empty;
            return Regex.Replace(result, @"[\d-]", string.Empty);
        }

        //Loại bỏ các kí tự đặc biệt, chữ số và giữ lại Unikey(Unicode)
        public static string RemoveCharacters(this string result)
        {
            if (result == null)
                return string.Empty;
            return Regex.Replace(result, @"[^\s\p{L}A-Za-z_.,]", string.Empty);
        }
        //Loại bỏ các kí tự đặc biệt, và giữ lại Unikey(Unicode)
        public static string RemoveSpecialCharacters(this string result)
        {
            if (result == null)
                return string.Empty;
            return Regex.Replace(result, @"[^\s\p{L}A-Za-z0-9_.,]", string.Empty);
        }

        //Loại bỏ dấu tiếng việt
        public static string RemoveVietnameseDiacritics(this string result)
        {
            if (result == null)
                return string.Empty;
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = result.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty)
                        .Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        //Loại bỏ dấu phẩy và Unikey(Unicode), chuyển đổi ToLower()
        public static string RemoveUniKeyAndToLower(this string result)
        {
            if (result == null)
                return string.Empty;
            Regex regex = new Regex(@"[^\s\p{L}A-Za-z0-9_.]");
            string temp = result.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').ToLower();
        }


        // format tiền USD
        public static string ApplyFormatMoneyUSD(this decimal result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); //en-US //
            cul = (CultureInfo)cul.Clone();
            cul.NumberFormat.CurrencySymbol = "$";

            //CultureInfo culVietNam = CultureInfo.GetCultureInfo("vi-VN");
            //cul.NumberFormat.CurrencySymbol = "$";
            //cul.NumberFormat.CurrencyNegativePattern = culVietNam.NumberFormat.CurrencyNegativePattern;
            //cul.NumberFormat.CurrencyPositivePattern = culVietNam.NumberFormat.CurrencyPositivePattern;

            var resultStr = string.Format(cul, "{0:c}", result);
            return string.IsNullOrEmpty(resultStr) ? "0 $" : resultStr;
        }
        public static string ApplyFormatTien(this decimal result)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return result.ToString("#,##0.00", cul.NumberFormat);
        }

        public static string RemoveHtmlFromString(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                result = CommonHelper.StripHTML(Regex.Replace(result, "</p>(?![\n\r]+)", Environment.NewLine));
                if (result.Length > 2 && result.Substring(result.Length - 2) == "\r\n")
                {
                    result = result.Remove(result.Length - 2);
                }
                return result;
            }
            return string.Empty;
        }

        public static string ICDDisplay(string ma, string ten, string ghiChu, KieuHienThiICD type = KieuHienThiICD.MaGachNgangGhiChu)
        {
            var iCDDisplay = string.Empty;
            switch (type)
            {
                case KieuHienThiICD.MaGachNgangGhiChu:
                    iCDDisplay = ma + (!string.IsNullOrEmpty(ghiChu) ? " - " + ghiChu : ""); break;
                case KieuHienThiICD.TenGachNgangGhiChu:
                    iCDDisplay = ten + (!string.IsNullOrEmpty(ghiChu) ? " - " + ghiChu : ""); break;
                case KieuHienThiICD.MaNgoacTronGhiChu:
                    iCDDisplay = ma + (!string.IsNullOrEmpty(ghiChu) ? " (" + ghiChu + ")" : ""); break;
                case KieuHienThiICD.TenNgoacTronGhiChu:
                    iCDDisplay = ten + (!string.IsNullOrEmpty(ghiChu) ? " (" + ghiChu + ")" : ""); break;
                case KieuHienThiICD.MaGachNgangTen:
                    iCDDisplay = ma + " - " + ten; break;
                case KieuHienThiICD.MaGachNgangTenNgoacTronGhiChu:
                    iCDDisplay = ma + " - " + ten + (!string.IsNullOrEmpty(ghiChu) ? " (" + ghiChu + ")" : ""); break;
                case KieuHienThiICD.MaGachNgangTenHaiChamGhiChu:
                    iCDDisplay = ma + " - " + ten + (!string.IsNullOrEmpty(ghiChu) ? ": " + ghiChu : ""); break;
            }
            return iCDDisplay;
        }
    }
}
