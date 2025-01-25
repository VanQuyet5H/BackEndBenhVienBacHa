using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuInTienQuayThuoc
{
    public class PhieuInTienQuayThuocGridVo :GridItem
    {
        public string LogoUrl { get; set; }
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay { get; set; }
        public string DiaChi { get; set; }
        public string DienGiai { get; set; }
        public int STT { get; set; }
        public string TenThuocVatTu { get; set; }
        public string DVT { get; set; }
        public decimal? SL { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }
        public decimal? TongCong { get; set; }
        public string BangChu { get; set; }
        public string NguoiPhatThuoc { get; set; }
        public string NgayThangNam { get; set; }
        //public string Thang { get; set; }
        //public string Nam { get; set; }
        public string SoBangKe { get; set; }
        //PhieuThu
        public string MauSo { get; set; }
        public string ThongTinCapNhatMauSo { get; set; }
        public string NoiDung { get; set; }
        public string ChiPhiKCB { get; set; }
        public string BHYTTT { get; set; }
        public string MienGiam { get; set; }
        public string TienMat { get; set; }
        public string ThanhTienPhieuThu { get; set; }
        public string ChuyenKhoan { get; set; }
        public string CongNo { get; set; }
        public string BangTienPhieuThu { get; set; }
        public string NgayLapPhieu { get; set; }
        public string SoQuyen { get; set; }
        public string No { get; set; }
        public string SoPhieu { get; set; }
        public string Co { get; set; }
        public string GioHienTai { get; set; }
    }
}
