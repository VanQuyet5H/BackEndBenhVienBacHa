using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Camino.Core.Helpers
{
    public static class DateHelper
    {
        /// <summary>
        /// Compare date with toDate: equal -> 0, greater than -> 1, less than -> -1
        /// </summary>
        /// <param name="date"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static int CompareDateTo(this DateTime date, DateTime toDate)
        {
            if (date.Year < toDate.Year)
                return -1;
            if (date.Year > toDate.Year)
                return 1;
            if (date.DayOfYear < toDate.DayOfYear)
                return -1;
            if (date.DayOfYear > toDate.DayOfYear)
                return 1;
            return 0;
        }
        public static string ApplyFormatDateTime(this DateTime date)
        {
            //update AM/PM -> SA/CH by VuLe 17/8/2020
            return date.ToString("dd/MM/yyyy HH:mm").Replace("AM", "SA").Replace("PM", "CH");
        }
        public static string ApplyFormatTimeDate(this DateTime date)
        {
            //update AM/PM -> SA/CH by VuLe 17/8/2020
            return date.ToString("HH:mm dd/MM/yyyy").Replace("AM", "SA").Replace("PM", "CH");
        }
        public static string ApplyFormatDate(this DateTime date)
        {
            //update AM/PM -> SA/CH by VuLe 17/8/2020
            return date.ToString("dd/MM/yyyy").Replace("AM", "SA").Replace("PM", "CH");
        }
        public static string ApplyFormatTime(this DateTime date)
        {
            return date.ToString("HH:mm").Replace("AM", "SA").Replace("PM", "CH");
        }
        public static string ApplyFormatFullTime(this DateTime date)
        {
            return date.ToString("HH:mm:ss").Replace("AM", "SA").Replace("PM", "CH");
        }

        public static string ApplyFormatFullDateTime(this DateTime date)
        {
            return date.ToString("HH:mm:ss dd/MM/yyyy").Replace("AM", "SA").Replace("PM", "CH");
        }

        public static void TryParseExactCustom(this string date, out DateTime d)
        {
            DateTime.TryParseExact(date, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out d);
        }

        public static string FormatNgayGioTimKiemTrenBaoCao(this DateTime date)
        {
            return date.ToString("HH:mm dd/MM/yyyy");
        }

        public static string FormatNgayTimKiemTrenBaoCao(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static string FormatNgayGioTimKiemTrenBaoCao(this string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                DateTime.TryParseExact(date, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime d);
                return d.ToString("HH:mm dd/MM/yyyy");
            }
            return null;
        }

        public static string ApplyFormatDateTimeSACH(this DateTime date)
        {
            var result = date.ToString("dd/MM/yyyy HH:mm");
            var AM = "AM";
            var PM = "PM";
            if (result.Contains(AM))
            {
                result = Regex.Replace(result, AM, "SA", RegexOptions.IgnoreCase);
            }
            else if (result.Contains(PM))
            {
                result = Regex.Replace(result, PM, "CH", RegexOptions.IgnoreCase);
            }
            return result;
        }
        public static string ConvertHourToString(this int hour)
        {
            return hour > 9 ? hour.ToString() : "0" + hour.ToString();
        }
        public static string ConvertMinuteToString(this int minute)
        {
            return minute > 9 ? minute.ToString() : "0" + minute.ToString();
        }

        public static string ConvertDateToString(this int date)
        {
            return date > 9 ? date.ToString() : "0" + date.ToString();
        }

        public static string ConvertMonthToString(this int month)
        {
            return month > 9 ? month.ToString() : "0" + month.ToString();
        }

        public static string ConvertYearToString(this int year)
        {
            return year.ToString();
        }

        public static string ConvertDatetimeToString(this DateTime date, bool isNotMinutesString = true)
        {
            var hour = date.Hour.ConvertHourToString();
            var minute = date.Minute.ConvertMinuteToString();
            var day = date.Day.ConvertDateToString();
            var month = date.Month.ConvertMonthToString();
            var year = date.Year.ToString();
            if (isNotMinutesString)
            {
                return hour + " giờ " + minute + " phút, ngày " + day + " tháng " + month + " năm " + year;

            }
            else
            {
                return hour + " giờ " + minute + " ngày " + day + " tháng " + month + " năm " + year;
            }
        }
        /// <summary>
        /// Example: 08 giờ 05 phút, ngày 20/11/2020
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ApplyFormatGioPhutNgay(this DateTime date, bool laBaoCao = false)
        {
            if (!laBaoCao)
            {
                return $"{date.Hour:D2} giờ {date.Minute:D2} phút, ngày {date.ToString("dd/MM/yyyy")}";
            }
            else
            {
                return $"{date.Hour:D2}:{date.Minute:D2} Ngày {date.ToString("dd/MM/yyyy")}";
            }
        }
        /// <summary>
        /// Example: Ngày 01 tháng 02 năm 2020
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ApplyFormatNgayThangNam(this DateTime date)
        {
            return $"Ngày {date.Day:D2} tháng {date.Month:D2} năm {date.Year:D2}";
        }
        public static string DOBFormat(int? ngaySinh, int? thangSinh, int? namSinh)
        {
            string result = string.Empty;
            if ((ngaySinh == 0 || ngaySinh == null) && (thangSinh == 0 || thangSinh == null) && (namSinh != 0 || namSinh != null))
            {
                return namSinh.ToString();
            }
            else if (ngaySinh != null && thangSinh != null && namSinh != null)
            {
                var Ngay = ngaySinh > 9 ? ngaySinh.ToString() : "0" + ngaySinh.ToString();
                var Thang = thangSinh > 9 ? thangSinh.ToString() : "0" + thangSinh.ToString();
                var Nam = namSinh.ToString();
                result = Ngay + "/" + Thang + "/" + Nam;
            }
            return result;
        }

        public static string DOBFormatYYYYMMDD(int? ngaySinh, int? thangSinh, int? namSinh)
        {
            string result = string.Empty;
            if ((ngaySinh == 0 || ngaySinh == null) && (thangSinh == 0 || thangSinh == null) && (namSinh != 0 || namSinh != null))
            {
                return namSinh.ToString();
            }
            else if (ngaySinh != null && thangSinh != null && namSinh != null)
            {
                var Ngay = ngaySinh > 9 ? ngaySinh.ToString() : "0" + ngaySinh.ToString();
                var Thang = thangSinh > 9 ? thangSinh.ToString() : "0" + thangSinh.ToString();
                var Nam = namSinh.ToString();
                result = Nam + Thang + Ngay;
            }
            return result;
        }

        public static string ConvertIntSecondsToTime12h(this int seconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            DateTime dateTime = DateTime.Today.Add(ts);
            var result = dateTime.ToString("HH:mm");
            var AM = "AM";
            var PM = "PM";
            if (result.Contains(AM))
            {
                result = Regex.Replace(result, AM, "SA", RegexOptions.IgnoreCase);
            }
            if (result.Contains(PM))
            {
                result = Regex.Replace(result, PM, "CH", RegexOptions.IgnoreCase);
            }
            return result;
        }

        public static string ConvertIntSecondsToTime(this int seconds)
        {
            var hours = seconds / 3600;
            var minutes = (seconds / 60) % 60;

            return $"{hours:00}:{minutes:00}";
        }

        public static DateTime ToDate(this string date, string format = "MM-dd-yyyy")
        {
            try
            {
                return Convert.ToDateTime(DateTime.Parse(date, CultureInfo.GetCultureInfo("vi-vn")), CultureInfo.GetCultureInfo("en-US"));
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        public static bool KiemTraNgayLamViecTrongTuan(this DateTime date, string ngay)
        {
            if (ngay.Length < 7)
            {
                for (int i = ngay.Length; i < 7; i++)
                {
                    ngay += "0";
                }
            }
            var day = date.Date.DayOfWeek;
            var array = ngay.ToCharArray();

            switch (day)
            {
                case DayOfWeek.Monday:
                    int mon = array[0] - '0';
                    if (mon != 0)
                        return true;
                    else
                        return false;
                case DayOfWeek.Tuesday:
                    int tues = array[1] - '0';
                    if (tues != 0)
                        return true;
                    else
                        return false;
                case DayOfWeek.Wednesday:
                    int wed = array[2] - '0';
                    if (wed != 0)
                        return true;
                    else
                        return false;
                case DayOfWeek.Thursday:
                    int thur = array[3] - '0';
                    if (thur != 0)
                        return true;
                    else
                        return false;
                case DayOfWeek.Friday:
                    int fri = array[4] - '0';
                    if (fri != 0)
                        return true;
                    else
                        return false;
                case DayOfWeek.Saturday:
                    int sat = array[5] - '0';
                    if (sat != 0)
                        return true;
                    else
                        return false;
                case DayOfWeek.Sunday:
                    int sun = array[6] - '0';
                    if (sun != 0)
                        return true;
                    else
                        return false;
            }

            return true;
        }

        public static string ConvertDOBToTimeJson(int? ngaySinh, int? thangSinh, int? namSinh)
        {
            var dobFormat = string.Empty;

            if ((ngaySinh == 0 || ngaySinh == null) && (thangSinh == 0 || thangSinh == null) && (namSinh != 0 || namSinh != null))
            {
                var res = new
                {
                    Days = 0,
                    Months = 0,
                    Years = DateTime.Now.Year - namSinh
                };
                return JsonConvert.SerializeObject(res);
            }
            else if (ngaySinh != null && thangSinh != null && namSinh != null)
            {
                var Ngay = ngaySinh > 9 ? ngaySinh.ToString() : "0" + ngaySinh.ToString();
                var Thang = thangSinh > 9 ? thangSinh.ToString() : "0" + thangSinh.ToString();
                var Nam = namSinh.ToString();
                dobFormat = Ngay + "/" + Thang + "/" + Nam;
            }

            if (string.IsNullOrEmpty(dobFormat))
            {
                return null;
            }
            var dateNow = DateTime.ParseExact(DateTime.Now.ApplyFormatDate(), "dd/MM/yyyy", null);
            var dateDOBFormat = DateTime.ParseExact(dobFormat, "dd/MM/yyyy", null);

            //data test
            //var dateNow = DateTime.ParseExact("01/01/2020", "dd/MM/yyyy", null);
            //var dateDOBFormat = DateTime.ParseExact("01/01/2019", "dd/MM/yyyy", null);

            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            var increment = 0;
            var days = 0;
            var months = 0;
            var years = 0;

            //////////Day Calculation
            if (dateDOBFormat.Day > dateNow.Day)
            {
                increment = monthDay[dateDOBFormat.Month - 1];
            }

            if (increment == -1)
            {
                ////////kiểm tra năm nhuận
                if (DateTime.IsLeapYear(dateDOBFormat.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }

            if (increment != 0)
            {
                days = (dateNow.Day + increment) - dateDOBFormat.Day;
                increment = 1;
            }
            else
            {
                days = dateNow.Day - dateDOBFormat.Day;
            }

            //////////Month Calculation
            ///
            if ((dateDOBFormat.Month + increment) > dateNow.Month)
            {
                months = (dateNow.Month + 12) - (dateDOBFormat.Month + increment);
                increment = 1;
            }
            else
            {
                months = (dateNow.Month) - (dateDOBFormat.Month + increment);
                increment = 0;
            }

            //////////Year Calculation
            years = dateNow.Year - (dateDOBFormat.Year + increment);

            var result = new
            {
                Days = days,
                Months = months,
                Years = years
            };
            return JsonConvert.SerializeObject(result);
        }

        public static string DaysCalculateJson(DateTime date)
        {
            var dateNow = DateTime.ParseExact(DateTime.Now.ApplyFormatDate(), "dd/MM/yyyy", null);
            var dateFormat = DateTime.ParseExact(date.ApplyFormatDate(), "dd/MM/yyyy", null);
            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            var increment = 0;
            var days = 0;
            var months = 0;
            var years = 0;

            //////////Day Calculation
            if (dateFormat.Day > dateNow.Day)
            {
                increment = monthDay[dateFormat.Month - 1];
            }

            if (increment == -1)
            {
                ////////kiểm tra năm nhuận
                if (DateTime.IsLeapYear(dateFormat.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }

            if (increment != 0)
            {
                days = (dateNow.Day + increment) - dateFormat.Day;
                increment = 1;
            }
            else
            {
                days = dateNow.Day - dateFormat.Day;
            }

            //////////Month Calculation
            ///
            if ((dateFormat.Month + increment) > dateNow.Month)
            {
                months = (dateNow.Month + 12) - (dateFormat.Month + increment);
                increment = 1;
            }
            else
            {
                months = (dateNow.Month) - (dateFormat.Month + increment);
                increment = 0;
            }

            //////////Year Calculation
            years = dateNow.Year - (dateFormat.Year + increment);

            var result = new
            {
                Days = days,
                Months = months,
                Years = years
            };
            return JsonConvert.SerializeObject(result);
        }

        public static string CalculateHanSuDung(int soNgaySuDung, DateTime? ngayTao = null)
        {
            var result = string.Empty;
            var ngayHienTai = DateTime.Now;
            if (ngayTao == null)
            {
                ngayHienTai = ngayHienTai.AddDays(soNgaySuDung);
                result = ngayHienTai.ApplyFormatDate();
            }
            else
            {
                var ngayTaoGoi = ngayTao.Value;
                ngayTaoGoi = ngayTaoGoi.AddDays(soNgaySuDung);
                result = ngayTaoGoi.ApplyFormatDate();
            }
            return result;
        }

        public static bool KiemTraDoTuoiTheoNgaySinh(int? ngay, int? thang, int? nam, int soTuoi, bool layBang = true)
        {
            var now = DateTime.Now;
            return (nam != null
                    && (((now.Year - nam) < soTuoi)
                        || (((now.Year - nam) == soTuoi)
                            && ((layBang && (thang == null || ((now.Month - thang) >= 0))
                                        && (ngay == null || ((now.Day - ngay) >= 0)))
                                || (!layBang 
                                        && thang != null 
                                        && (now.Month < thang || (now.Month == thang && ngay != null && now.Day < ngay)))))
                        ));
        }
        public static string ApplyFormatDateTimeYearMonthDayGioPhut(this DateTime date, int type)
        {
            //yyyMMddHHmm 
            var result = string.Empty;
            switch (type)
            {
                case 1 :
                    result = date.Year + date.Month + date.Day + date.Hour + date.Minute + ""; ;
                break;
            }
            return result;
        }
    }
}
