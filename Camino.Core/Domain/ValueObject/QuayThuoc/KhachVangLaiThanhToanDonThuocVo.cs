using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.QuayThuoc
{
    public class KhachVangLaiThanhToanDonThuocVo
    {
        public KhachVangLaiThanhToanDonThuocVo()
        {
            DanhSachDonThuoc = new List<KhachVangLaiThuocChoThanhToanVo>();
        }

        public KhachVangLaiThongTinHanhChinhVo ThongTinKhach { get; set; }

        public KhachVangLaiThongTinDonThuocVo ThongTinThuChi { get; set; }

        public List<KhachVangLaiThuocChoThanhToanVo> DanhSachDonThuoc { get; set; }
    }

    public class KhachVangLaiThongTinHanhChinhVo
    {
        public long? BenhNhanId { get; set; }

        public string HoTen { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public int? NamSinh { get; set; }

        public string DiaChi { get; set; }

        public string SoDienThoai { get; set; }

        public long? TinhThanhId { get; set; }

        public long? QuanHuyenId { get; set; }

        public long? PhuongXaId { get; set; }
    }
    public class KhachVangLaiThongTinDonThuocVo
    {
        public decimal? TienMat { get; set; }

        public decimal? ChuyenKhoan { get; set; }

        public decimal? POS { get; set; }

        public decimal? SoTienCongNo { get; set; }

        public DateTime NgayThu { get; set; }

        public string NoiDungThu { get; set; }

        public string GhiChu { get; set; }
    }
    public class KhachVangLaiThuocChoThanhToanVo
    {
        public int? STT { get; set; }

        public long DuocPhamId { get; set; }

        public string MaHoatChat { get; set; }

        public string TenDuocPham { get; set; }

        public double SoLuongTon { get; set; }

        public long NhapKhoDuocPhamChiTietId { get; set; }

        public string TenHoatChat { get; set; }

        public string DonViTinh { get; set; }

        public double SoLuongToa { get; set; }

        public double SoLuongMua { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }

        public string Solo { get; set; }

        public string ViTri { get; set; }

        public DateTime HanSuDung { get; set; }

        public string HanSuDungHientThi { get; set; }

        public string BacSiKeToa { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
    }

    public class KhachVangLaiNavigateVo
    {
        public long TaiKhoanBenhNhanId { get; set; }

        public long YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
    }
    public class TinhHuyenTemplateVo
    {
        public string TenTinh { get; set; }
        public string TenHuyen { get; set; }
        public long TinhId { get; set; }
        public long HuyenId { get; set; }
    }
}
