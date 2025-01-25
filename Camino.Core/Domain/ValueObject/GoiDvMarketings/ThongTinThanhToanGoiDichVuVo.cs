using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camino.Core.Domain.ValueObject.GoiDvMarketings
{
    public class ThongTinThanhToanGoiDichVuVo
    {
        public ThongTinThanhToanGoiDichVuVo()
        {
            DanhSachDichVuDaBaoLanhSuDung = new List<(decimal, decimal)>();
        }

        public long YeuCauGoiDichVuId { get; set; }
        public decimal SoTienBenhNhanDaChi { get; set; }
        public decimal SoTienConBaoLanh => SoTienBenhNhanDaChi - SoTienDaBaoLanhSuDung;
        public decimal SoTienDaBaoLanhSuDung => DanhSachDichVuDaBaoLanhSuDung.Sum(o => o.Item1 * o.Item2);
        public List<(decimal, decimal)> DanhSachDichVuDaBaoLanhSuDung { get; set; }
    }

    public class ThongTinSuDungGiuongTrongGoiDichVuVo
    {
        public ThongTinSuDungGiuongTrongGoiDichVuVo()
        {
            DanhSachDichVuGiuongTrongGoi = new List<ThongTinGiuongTrongGoiDichVuVo>();
            DanhSachDichVuGiuongDaSuDung = new List<ThongTinGiuongTrongGoiDichVuVo>();
        }

        public long YeuCauGoiDichVuId { get; set; }
        public List<ThongTinGiuongTrongGoiDichVuVo> DanhSachDichVuGiuongTrongGoi { get; set; }
        public List<ThongTinGiuongTrongGoiDichVuVo> DanhSachDichVuGiuongDaSuDung { get; set; }
    }

    public class ThongTinGiuongTrongGoiDichVuVo
    {
        public long DichVuGiuongBenhVienId { get; set; }
        public long NhomGiaDichVuGiuongBenhVienId { get; set; }
        public double SoLan { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaTruocChietKhau { get; set; }
        public decimal DonGiaSauChietKhau { get; set; }
    }
}
