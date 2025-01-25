using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.NhomDichVuBenhVien;

namespace Camino.Core.Helpers
{
    public static class CalculateHelper
    {
        public static Enums.LoaiDichVuKyThuat GetLoaiDichVuKyThuat(long nhomDichVuBenhVienId, List<NhomDichVuBenhVien> tatCaNhomDichVuBenhVien)
        {
            var root = nhomDichVuBenhVienId;
            var nhomDichVuBenhVien = tatCaNhomDichVuBenhVien.FirstOrDefault(o => o.Id == root);
            while (nhomDichVuBenhVien?.NhomDichVuBenhVienChaId != null)
            {
                root = nhomDichVuBenhVien.NhomDichVuBenhVienChaId.Value;
                nhomDichVuBenhVien = tatCaNhomDichVuBenhVien.FirstOrDefault(o => o.Id == root);
            }

            if (typeof(Enums.LoaiDichVuKyThuat).IsEnumDefined((int)root)) return (Enums.LoaiDichVuKyThuat)root;
            return Enums.LoaiDichVuKyThuat.Khac;
        }
        //update BACHA-427: Cập nhật lại công thức tính tiền Dược phẩm cho người bệnh theo tháp giá
        public static decimal TinhDonGiaBan(decimal donGiaNhap, int tiLeTheoThapGia, int vat, bool laDuocPham, bool laDuocPhamTaiNhaThuoc, Enums.PhuongPhapTinhGiaTriTonKho phuongPhapTinhGiaTriTonKho)
        {
            if (laDuocPham)
            {
                var giaTonKho = donGiaNhap + (donGiaNhap * (phuongPhapTinhGiaTriTonKho == Enums.PhuongPhapTinhGiaTriTonKho.ApVAT ? vat : 0)) / 100;
                var giaBanNhaThuocChuaVat = giaTonKho + (giaTonKho * tiLeTheoThapGia / 100);
                if (laDuocPhamTaiNhaThuoc)
                {
                    return Math.Round(giaBanNhaThuocChuaVat + (giaBanNhaThuocChuaVat * vat / 100) , 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    return Math.Round(giaTonKho + (giaTonKho * tiLeTheoThapGia / 100), 2, MidpointRounding.AwayFromZero);
                }
            }
            return Math.Round(donGiaNhap + donGiaNhap * tiLeTheoThapGia / 100 + donGiaNhap * vat / 100 + donGiaNhap * tiLeTheoThapGia * vat / 10000, 2, MidpointRounding.AwayFromZero);
        }
        public static int TinhTongSoThangCuaTuoi(int? day, int? month, int? year)
        {
            DateTime today = DateTime.Today;
            if (day != null && month != null && year != null)
            {
                int months = today.Month - (int)month;
                int years = today.Year - (int)year;

                if (today.Day < day)
                {
                    months--;
                }

                if (months < 0)
                {
                    years--;
                    months += 12;
                }

                return (int)years * 12 + (int)months;
            }
            else
            {
                if (year != null)
                {
                    return (int)year * 12;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Kết quả <3 tháng tuổi hiển thị ngày tuổi . Ví dụ: NB sinh ngày 01/07/2022, nhập viện ngày 20.07.2022. Phiếu điều trị ngày 20.07.2022 khi in hiển thị trường Tuổi: 20 ngày
        /// 3 tháng tuổi ≤ Kết quả< 3 tuổi hiển thị tháng tuổi.Ví dụ: NB sinh ngày 01/07/2021, nhập viện ngày 20.07.2022. Phiếu điều trị ngày 20.07.2022 khi in hiển thị trường Tuổi: 12 tháng
        /// Kết quả ≥ 3 tuổi hiển thị số tuổi.Ví dụ: NB sinh ngày 01/10/2000, nhập viện ngày 20.07.2022. Phiếu điều trị ngày 20.07.2022 khi in hiển thị trường Tuổi: 22
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="denNgay"></param>
        /// <returns></returns>
        public static string TinhTuoiHienThiTrenBieuMau(int? day, int? month, int? year, DateTime denNgay)
        {
            if(year == null)
                return string.Empty;
            else
            {
                if(day != null && day != 0 && month != null && month != 0)
                {
                    var birthdate = new DateTime(year.Value, month.Value, day.Value).Date;

                    if (birthdate > denNgay)
                        return string.Empty;

                    var months = GetMonthsBetween(birthdate, denNgay);
                    if(months < 3)
                    {
                        return $"{(denNgay - birthdate).TotalDays + 1} ngày";
                    }    
                    else if(months < 36)
                    {
                        return $"{months} tháng";
                    }
                    else
                    {
                        return $"{denNgay.Year - year}";
                    }
                }
                else
                {
                    return $"{denNgay.Year - year}";
                }
            }
        }

        public static int GetMonthsBetween(DateTime from, DateTime to)
        {
            if (from > to) return GetMonthsBetween(to, from);
            var monthDiff = (to.Year - from.Year) * 12 + (to.Month - from.Month);
            //var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

            if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
            {
                return monthDiff - 1;
            }
            else
            {
                return monthDiff;
            }
        }

        /// <summary>
        /// tính tuổi chính xác theo ngày tháng năm
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        public static int TinhTuoi(int? day, int? month, int? year, DateTime? today = null)
        {
            DateTime birthdate;
            try
            {
                birthdate = new DateTime(year.Value, month ?? 1, day ?? 1);
            }
            catch
            {
                birthdate = new DateTime(year ?? DateTime.Now.Year, 1, 1);
            }
            var todayValue = today ?? DateTime.Today;
            var age = todayValue.Year - birthdate.Year;
            if (birthdate.Date > todayValue.AddYears(-age)) age--;
            return age;
        }

        public static long GetDuocPhamBenhVienPhanNhomCha(long duocPhamBenhVienPhanNhomId, List<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhoms)
        {
            var root = duocPhamBenhVienPhanNhomId;
            var nhomDichVuBenhVien = duocPhamBenhVienPhanNhoms.FirstOrDefault(o => o.Id == root);
            while (nhomDichVuBenhVien?.NhomChaId != null)
            {
                root = nhomDichVuBenhVien.NhomChaId.Value;
                nhomDichVuBenhVien = duocPhamBenhVienPhanNhoms.FirstOrDefault(o => o.Id == root);
            }
            return root;
        }
    }
}
