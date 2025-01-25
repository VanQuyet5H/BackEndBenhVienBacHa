using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Camino.Core.Domain.ValueObject;
using Newtonsoft.Json;

namespace Camino.Core.Helpers
{
    public static class CommonHelper
    {
        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                var destinationConverter = TypeDescriptor.GetConverter(destinationType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);

                var sourceConverter = TypeDescriptor.GetConverter(sourceType);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);

                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);

                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// Convert enum for front-end
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertEnum(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            var result = string.Empty;
            foreach (var c in str)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();

            //ensure no spaces (e.g. when the first letter is upper case)
            result = result.TrimStart();
            return result;
        }

        /// <summary>
        /// Check mail valid
        /// </summary>
        /// <param name="mail">Input string</param>
        /// <returns></returns>
        public static bool IsMailValid(string mail)
        {
            if (string.IsNullOrEmpty(mail))
                return true;
            Regex rx = new Regex(
                @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(mail.Trim());
        }

        /// <summary>
        /// Check bien kiem soat xe may valid
        /// </summary>
        /// <param name="mail">Input string</param>
        /// <returns></returns>
        public static bool IsBienKiemSoatXeMayValid(string bks)
        {
            if (string.IsNullOrEmpty(bks))
                return true;
            Regex rx = new Regex(
                @"^[a-zA-Z0-9.-]+$");
            return rx.IsMatch(bks.Trim());
        }

        /// <summary>
        /// Check bien kiem soat xe o to valid
        /// </summary>
        /// <param name="mail">Input string</param>
        /// <returns></returns>
        public static bool IsBienKiemSoatOtoValid(string bks)
        {
            if (string.IsNullOrEmpty(bks))
                return true;
            Regex rx = new Regex(
                @"^[a-zA-Z0-9]+$");
            return rx.IsMatch(bks.Trim());
        }


        /// <summary>
        /// Check phone vaid
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsPhoneValid(string number)
        {
            if (string.IsNullOrEmpty(number) || number.Length > 10 || number.Length < 10)
                return false;
            return number.All(c => c >= '0' && c <= '9');
        }

        /// <summary>
        /// Get name clone file attachment
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetNameGuildFileClone(string guidName)
        {
            var newGuidName = string.Empty;
            if (string.IsNullOrEmpty(guidName)) return newGuidName;
            var index = guidName.LastIndexOf(".");
            var lengthString = guidName.Length;
            newGuidName = Guid.NewGuid().ToString() + guidName.Substring(index, lengthString - index);
            return newGuidName;
        }

        /// <summary>
        /// Get id from request dropdownlist
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static long GetIdFromRequestDropDownList(DropDownListRequestModel model)
        {
            var parameter = model != null ? model.ParameterDependencies : "";
            if (string.IsNullOrEmpty(parameter) || parameter.Contains("undefined") || parameter.Contains("null")) return 0;
            var getValue = JsonConvert.DeserializeObject<Dictionary<string, long>>(parameter);
            var id = getValue.Values.First();
            return id;
        }

        public static string FromBase64(this string str)
        {
            byte[] byteArray = Convert.FromBase64String(str);

            using (var msIn = new MemoryStream(byteArray))
            using (var msOut = new MemoryStream())
            {
                using (var ds = new DeflateStream(msIn, CompressionMode.Decompress))
                {
                    ds.CopyTo(msOut);
                }

                return Encoding.UTF8.GetString(msOut.ToArray());
            }
        }

        public static string ConvertUnicodeString(this string str)
        {
            str = str.Replace("\u0065\u0309", "\u1EBB"); //ẻ
            str = str.Replace("\u0065\u0301", "\u00E9"); //é
            str = str.Replace("\u0065\u0300", "\u00E8"); //è
            str = str.Replace("\u0065\u0323", "\u1EB9"); //ẹ
            str = str.Replace("\u0065\u0303", "\u1EBD"); //ẽ
            str = str.Replace("\u00EA\u0309", "\u1EC3"); //ể
            str = str.Replace("\u00EA\u0301", "\u1EBF"); //ế
            str = str.Replace("\u00EA\u0300", "\u1EC1"); //ề
            str = str.Replace("\u00EA\u0303", "\u1EC5"); //ễ
            str = str.Replace("\u0079\u0309", "\u1EF7"); //ỷ
            str = str.Replace("\u0079\u0301", "\u00FD"); //ý
            str = str.Replace("\u0079\u0300", "\u1EF3"); //ỳ
            str = str.Replace("\u0079\u0323", "\u1EF5"); //ỵ
            str = str.Replace("\u0079\u0303", "\u1EF9"); //ỹ
            str = str.Replace("\u0075\u0309", "\u1EE7"); //ủ
            str = str.Replace("\u0075\u0301", "\u00FA"); //ú
            str = str.Replace("\u0075\u0300", "\u00F9"); //ù
            str = str.Replace("\u0075\u0323", "\u1EE5"); //ụ
            str = str.Replace("\u0075\u0303", "\u0169"); //ũ
            str = str.Replace("\u01B0\u0309", "\u1EED"); //ử
            str = str.Replace("\u01B0\u0301", "\u1EE9"); //ứ
            str = str.Replace("\u01B0\u0300", "\u1EEB"); //ừ
            str = str.Replace("\u01B0\u0323", "\u1EF1"); //ự
            str = str.Replace("\u01B0\u0303", "\u1EEF"); //ữ
            str = str.Replace("\u0069\u0309", "\u1EC9"); //ỉ
            str = str.Replace("\u0069\u0301", "\u00ED"); //í
            str = str.Replace("\u0069\u0300", "\u00EC"); //ì
            str = str.Replace("\u0069\u0323", "\u1ECB"); //ị
            str = str.Replace("\u0069\u0303", "\u0129"); //ĩ
            str = str.Replace("\u006F\u0309", "\u1ECF"); //ỏ
            str = str.Replace("\u006F\u0301", "\u00F3"); //ó
            str = str.Replace("\u006F\u0300", "\u00F2"); //ò
            str = str.Replace("\u006F\u0323", "\u1ECD"); //ọ
            str = str.Replace("\u006F\u0303", "\u00F5"); //õ
            str = str.Replace("\u01A1\u0309", "\u1EDF"); //ở
            str = str.Replace("\u01A1\u0301", "\u1EDB"); //ớ
            str = str.Replace("\u01A1\u0300", "\u1EDD"); //ờ
            str = str.Replace("\u01A1\u0323", "\u1EE3"); //ợ
            str = str.Replace("\u01A1\u0303", "\u1EE1"); //ỡ
            str = str.Replace("\u00F4\u0309", "\u1ED5"); //ổ
            str = str.Replace("\u00F4\u0301", "\u1ED1"); //ố
            str = str.Replace("\u00F4\u0300", "\u1ED3"); //ồ
            str = str.Replace("\u00F4\u0323", "\u1ED9"); //ộ
            str = str.Replace("\u00F4\u0303", "\u1ED7"); //ỗ
            str = str.Replace("\u0061\u0309", "\u1EA3"); //ả
            str = str.Replace("\u0061\u0301", "\u00E1"); //á
            str = str.Replace("\u0061\u0300", "\u00E0"); //à
            str = str.Replace("\u0061\u0323", "\u1EA1"); //ạ
            str = str.Replace("\u0061\u0303", "\u00E3"); //ã
            str = str.Replace("\u0103\u0309", "\u1EB3"); //ẳ
            str = str.Replace("\u0103\u0301", "\u1EAF"); //ắ
            str = str.Replace("\u0103\u0300", "\u1EB1"); //ằ
            str = str.Replace("\u0103\u0323", "\u1EB7"); //ặ
            str = str.Replace("\u0103\u0303", "\u1EB5"); //ẵ
            str = str.Replace("\u00E2\u0309", "\u1EA9"); //ẩ
            str = str.Replace("\u00E2\u0301", "\u1EA5"); //ấ
            str = str.Replace("\u00E2\u0300", "\u1EA7"); //ầ
            str = str.Replace("\u00E2\u0323", "\u1EAD"); //ậ
            str = str.Replace("\u00E2\u0303", "\u1EAB"); //ẫ
            str = str.Replace("\u0045\u0309", "\u1EBA"); //Ẻ
            str = str.Replace("\u0045\u0301", "\u00C9"); //É
            str = str.Replace("\u0045\u0300", "\u00C8"); //È
            str = str.Replace("\u0045\u0323", "\u1EB8"); //Ẹ
            str = str.Replace("\u0045\u0303", "\u1EBC"); //Ẽ
            str = str.Replace("\u00CA\u0309", "\u1EC2"); //Ể
            str = str.Replace("\u00CA\u0301", "\u1EBE"); //Ế
            str = str.Replace("\u00CA\u0300", "\u1EC0"); //Ề
            str = str.Replace("\u00CA\u0323", "\u1EC6"); //Ệ
            str = str.Replace("\u00CA\u0303", "\u1EC4"); //Ễ
            str = str.Replace("\u0059\u0309", "\u1EF6"); //Ỷ
            str = str.Replace("\u0059\u0301", "\u00DD"); //Ý
            str = str.Replace("\u0059\u0300", "\u1EF2"); //Ỳ
            str = str.Replace("\u0059\u0323", "\u1EF4"); //Ỵ
            str = str.Replace("\u0059\u0303", "\u1EF8"); //Ỹ
            str = str.Replace("\u0055\u0309", "\u1EE6"); //Ủ
            str = str.Replace("\u0055\u0301", "\u00DA"); //Ú
            str = str.Replace("\u0055\u0300", "\u00D9"); //Ù
            str = str.Replace("\u0055\u0323", "\u1EE4"); //Ụ
            str = str.Replace("\u0055\u0303", "\u0168"); //Ũ
            str = str.Replace("\u01AF\u0309", "\u1EEC"); //Ử
            str = str.Replace("\u01AF\u0301", "\u1EE8"); //Ứ
            str = str.Replace("\u01AF\u0300", "\u1EEA"); //Ừ
            str = str.Replace("\u01AF\u0323", "\u1EF0"); //Ự
            str = str.Replace("\u01AF\u0303", "\u1EEE"); //Ữ
            str = str.Replace("\u0049\u0309", "\u1EC8"); //Ỉ
            str = str.Replace("\u0049\u0301", "\u00CD"); //Í
            str = str.Replace("\u0049\u0300", "\u00CC"); //Ì
            str = str.Replace("\u0049\u0323", "\u1ECA"); //Ị
            str = str.Replace("\u0049\u0303", "\u0128"); //Ĩ
            str = str.Replace("\u004F\u0309", "\u1ECE"); //Ỏ
            str = str.Replace("\u004F\u0301", "\u00D3"); //Ó
            str = str.Replace("\u004F\u0300", "\u00D2"); //Ò
            str = str.Replace("\u004F\u0323", "\u1ECC"); //Ọ
            str = str.Replace("\u004F\u0303", "\u00D5"); //Õ
            str = str.Replace("\u01A0\u0309", "\u1EDE"); //Ở
            str = str.Replace("\u01A0\u0301", "\u1EDA"); //Ớ
            str = str.Replace("\u01A0\u0300", "\u1EDC"); //Ờ
            str = str.Replace("\u01A0\u0323", "\u1EE2"); //Ợ
            str = str.Replace("\u01A0\u0303", "\u1EE0"); //Ỡ
            str = str.Replace("\u00D4\u0309", "\u1ED4"); //Ổ
            str = str.Replace("\u00D4\u0301", "\u1ED0"); //Ố
            str = str.Replace("\u00D4\u0300", "\u1ED2"); //Ồ
            str = str.Replace("\u00D4\u0323", "\u1ED8"); //Ộ
            str = str.Replace("\u00D4\u0303", "\u1ED6"); //Ỗ
            str = str.Replace("\u0041\u0309", "\u1EA2"); //Ả
            str = str.Replace("\u0041\u0301", "\u00C1"); //Á
            str = str.Replace("\u0041\u0300", "\u00C0"); //À
            str = str.Replace("\u0041\u0323", "\u1EA0"); //Ạ
            str = str.Replace("\u0041\u0303", "\u00C3"); //Ã
            str = str.Replace("\u0102\u0309", "\u1EB2"); //Ẳ
            str = str.Replace("\u0102\u0301", "\u1EAE"); //Ắ
            str = str.Replace("\u0102\u0300", "\u1EB0"); //Ằ
            str = str.Replace("\u0102\u0323", "\u1EB6"); //Ặ
            str = str.Replace("\u0102\u0303", "\u1EB4"); //Ẵ
            str = str.Replace("\u00C2\u0309", "\u1EA8"); //Ẩ
            str = str.Replace("\u00C2\u0301", "\u1EA4"); //Ấ
            str = str.Replace("\u00C2\u0300", "\u1EA6"); //Ầ
            str = str.Replace("\u00C2\u0323", "\u1EAC"); //Ậ
            str = str.Replace("\u00C2\u0303", "\u1EAA"); //Ẫ
            return str;
        }

        public static string ToHexString(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            byte[] bytesArray = Encoding.Default.GetBytes(value);
            var hexString = BitConverter.ToString(bytesArray);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        public static string ConvertToUnSign(this string input)
        {
            input = input.Trim();
            for (var i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            var regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            var str = input.Normalize(NormalizationForm.FormD);
            var str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2?.IndexOf("?") >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?"), 1);
            }
            return str2;
        }

        public static string DecodeHexString(this string hex)
        {
            var rtfBytes = FromHex(hex);
            var rtfText = Encoding.Default.GetString(rtfBytes);
            return rtfText;
        }

        private static byte[] FromHex(string hex)
        {
            var result = new byte[hex.Length / 2];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return result;
        }

        //Remove all tags but text
        public static string StripHTML(string HTMLText, bool decode = true)
        {
            var stripped = Regex.Replace(HTMLText, "<[^>]+>", "").Replace("\t", "").Trim();
            stripped = Regex.Replace(stripped, @"[\r\n]+", "\r\n");

            return decode ? HttpUtility.HtmlDecode(stripped) : stripped;
        }

        //Remove all tags but text inside body tag
        public static string StripBodyHTML(string HTMLText, bool decode = true)
        {
            HTMLText = HTMLText.Substring(HTMLText.IndexOf("<body>"), HTMLText.LastIndexOf("</body>") - HTMLText.IndexOf("<body>"));

            var stripped = Regex.Replace(HTMLText, "<[^>]+>", "").Replace("\t", "").Trim();
            stripped = Regex.Replace(stripped, @"[\r\n]+", "\r\n");

            return decode ? HttpUtility.HtmlDecode(stripped) : stripped;
        }

        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "No IPv4 address";
        }

        public static long GetIdFromRequestMultiSelect(MultiselectQueryInfo model)
        {
            var parameter = model != null ? model.ParameterDependencies : "";
            if (string.IsNullOrEmpty(parameter) || parameter.Contains("undefined") || parameter.Contains("null")) return 0;
            var getValue = JsonConvert.DeserializeObject<Dictionary<string, long>>(parameter);
            var id = getValue.Values.First();
            return id;
        }
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<[^>]*>", string.Empty).Replace("&nbsp;"," ");
        }

        public static T CopyObject<T>(this object objSource)
        {
            var s = JsonConvert.SerializeObject(objSource);
            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}
