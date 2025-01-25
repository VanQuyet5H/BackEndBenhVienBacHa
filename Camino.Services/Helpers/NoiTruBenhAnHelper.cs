using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain;

namespace Camino.Services.Helpers
{
    public static class NoiTruBenhAnHelper
    {
        public static int? TinhSoNgayDieuTri(Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn noiTruBenhAn)
        {
            if (noiTruBenhAn != null)
            {
                int soNgay;
                var timeSpan = (noiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now) - noiTruBenhAn.ThoiDiemNhapVien;

                if (timeSpan.TotalHours < 4)
                {
                    soNgay = 0;
                }
                else if (timeSpan.TotalHours < 24)
                {
                    soNgay = 1;
                }
                else
                {
                    soNgay = ((noiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now).Date - noiTruBenhAn.ThoiDiemNhapVien.Date).Days;
                    if (noiTruBenhAn.HinhThucRaVien != Enums.EnumHinhThucRaVien.RaVien)
                    {
                        soNgay++;
                    }
                }
                return soNgay;
            }
            return null;
        }

        public static decimal? SoNgayDieuTriBenhAnSoSinh(List<Core.Domain.Entities.DieuTriNoiTrus.NoiTruThoiGianDieuTriBenhAnSoSinh> noiTruThoiGianDieuTriBenhAnSoSinh)
        {
            if (noiTruThoiGianDieuTriBenhAnSoSinh.Any())
            {
                double soGiay = 0;
                var groupNgayDieuTri = noiTruThoiGianDieuTriBenhAnSoSinh;
                foreach (var item in groupNgayDieuTri)
                {
                    TimeSpan gioBatDau = TimeSpan.FromSeconds(item.GioBatDau.Value);
                    TimeSpan gioKetThuc = TimeSpan.FromSeconds(item.GioKetThuc.Value);
                    soGiay += (gioKetThuc - gioBatDau).TotalSeconds;
                }

                double soGiayTrongNgay = 86400;
                decimal soNgay = (decimal)Math.Round(soGiay / soGiayTrongNgay, 1);
                return soNgay;
            }
            return null;
        }
    }
}
