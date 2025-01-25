using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Helpers
{
    public static class BenhVienHelper
    {
        /// <summary>
        /// result: 1: normal, 2: not normal, 3: danger
        /// </summary>
        public static int GetStatusForXetNghiem(string strGiaTriMin, string strGiaTriMax, string strGiaTriNguyHiemMin
            , string strGiaTriNguyHiemMax, string strGiaTriSoSanh)
        {
            if (double.TryParse(strGiaTriSoSanh, out var giaTriSoSanh))
            {
                if (double.TryParse(strGiaTriNguyHiemMax, out var giaTriNguyHiemMax))
                {
                    if (giaTriSoSanh > giaTriNguyHiemMax)
                        return 3;
                }
                else if(KiemTraNguyHiem(giaTriSoSanh, strGiaTriNguyHiemMax))
                {
                    return 3;
                }

                if (double.TryParse(strGiaTriNguyHiemMin, out var giaTriNguyHiemMin))
                {
                    if (giaTriSoSanh < giaTriNguyHiemMin)
                        return 3;
                }
                else if(KiemTraNguyHiem(giaTriSoSanh, strGiaTriNguyHiemMin))
                {
                    return 3;
                }

                if (double.TryParse(strGiaTriMax, out var giaTriMax))
                {
                    if(giaTriSoSanh > giaTriMax)
                        return 2;
                }
                else if(KiemTraKhacThuong(giaTriSoSanh, strGiaTriMax))
                {
                    return 2;
                }

                if (double.TryParse(strGiaTriMin, out var giaTriMin))
                {
                    if(giaTriSoSanh < giaTriMin)
                        return 2;
                }
                else if (KiemTraKhacThuong(giaTriSoSanh, strGiaTriMin))
                {
                    return 2;
                }
            }
            return 1;
        }

        private static bool KiemTraKhacThuong(double giaTriSoSanh, string str)
        {
            if (str == null)
            {
                return false;
            }
            str = str.Trim();
            if (str.StartsWith(">="))
            {
                if (double.TryParse(str.Replace(">=", "").Replace(" ", ""), out var giaTri))
                {
                    if (!(giaTriSoSanh >= giaTri))
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith("<="))
            {
                if (double.TryParse(str.Replace("<=", "").Replace(" ", ""), out var giaTri))
                {
                    if (!(giaTriSoSanh <= giaTri))
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith("≥"))
            {
                if (double.TryParse(str.Replace("≥", "").Replace(" ", ""), out var giaTri))
                {
                    if (!(giaTriSoSanh >= giaTri))
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith("≤"))
            {
                if (double.TryParse(str.Replace("≤", "").Replace(" ", ""), out var giaTri))
                {
                    if (!(giaTriSoSanh <= giaTri))
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith(">"))
            {
                if (double.TryParse(str.Replace(">", "").Replace(" ", ""), out var giaTri))
                {
                    if (!(giaTriSoSanh > giaTri))
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith("<"))
            {
                if (double.TryParse(str.Replace("<", "").Replace(" ", ""), out var giaTri))
                {
                    if (!(giaTriSoSanh < giaTri))
                        return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private static bool KiemTraNguyHiem(double giaTriSoSanh, string str)
        {
            if (str == null)
            {
                return false;
            }
            str = str.Trim();
            if (str.StartsWith(">="))
            {
                if (double.TryParse(str.Replace(">=", "").Replace(" ", ""), out var giaTri))
                {
                    if (giaTriSoSanh >= giaTri)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith("<="))
            {
                if (double.TryParse(str.Replace("<=", "").Replace(" ", ""), out var giaTri))
                {
                    if (giaTriSoSanh <= giaTri)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith("≥"))
            {
                if (double.TryParse(str.Replace("≥", "").Replace(" ", ""), out var giaTri))
                {
                    if (giaTriSoSanh >= giaTri)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith("≤"))
            {
                if (double.TryParse(str.Replace("≤", "").Replace(" ", ""), out var giaTri))
                {
                    if (giaTriSoSanh <= giaTri)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith(">"))
            {
                if (double.TryParse(str.Replace(">", "").Replace(" ", ""), out var giaTri))
                {
                    if (giaTriSoSanh > giaTri)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (str.StartsWith("<"))
            {
                if (double.TryParse(str.Replace("<", "").Replace(" ", ""), out var giaTri))
                {
                    if (giaTriSoSanh < giaTri)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        //BVHD-3959: Thứ tự thuốc hiển thị theo thông tư 23/2011-BYT: Thuốc truyền -> Thuốc tiêm -> Thuốc uống -> Thuốc Đặt -> Thuốc dùng ngoài (bôi, ...) -> Thuốc có đường dùng khác (khí dung, ...)
        public static int GetSoThuThuocTheoDuongDung(long duongDungId)
        {
            //16  2.14	  Truyền tĩnh mạch
            //17  2.15    Tiêm truyền
            var stt1 = new List<long> { 16, 17 };

            //58  2.01	  Tiêm bắp
            //5   2.02    Tiêm dưới da
            //6   2.03    Tiêm trong da
            //7   2.04    Tiêm tĩnh mạch
            //8   2.05    Tiêm truyền tĩnh mạch
            //59  2.06    Tiêm vào ổ khớp
            //9   2.07    Tiêm nội nhãn cầu
            //10  2.08    Tiêm trong dịch kính của mắt
            //11  2.09    Tiêm vào các khoang của cơ thể
            //12  2.10    Tiêm
            //13  2.11    Tiêm động mạch khối u
            //14  2.12    Tiêm vào khoang tự nhiên
            //15  2.13    Tiêm vào khối u
            var stt2 = new List<long> { 58, 5, 6, 7, 8, 59, 9, 10, 11, 12, 13, 14, 15 };

            //1   1.01    Uống
            //2   1.02    Ngậm
            //3   1.03    Nhai
            //57  1.04    Đặt dưới lưỡi
            //4   1.05    Ngậm dưới lưỡi
            var stt3 = new List<long> { 1, 2, 3, 57, 4 };

            //23  4.01    Đặt âm đạo
            //24  4.02    Đặt hậu môn
            //25  4.03    Thụt hậu môn - trực tràng
            //26  4.04    Đặt
            //27  4.05    Đặt tử cung
            //28  4.06    Thụt
            var stt4 = new List<long> { 23, 24, 25, 26, 27, 28 };

            //18  3.01    Bôi
            //19  3.02    Xoa ngoài
            //20  3.03    Dán trên da
            //21  3.04    Xịt ngoài da
            //22  3.05    Dùng ngoài
            var stt5 = new List<long> { 18, 19, 20, 21, 22 };

            if(stt1.Contains(duongDungId))
                return 1;
            if (stt2.Contains(duongDungId))
                return 2;
            if (stt3.Contains(duongDungId))
                return 3;
            if (stt4.Contains(duongDungId))
                return 4;
            if (stt5.Contains(duongDungId))
                return 5;
            return 6;
        }
    }
}
