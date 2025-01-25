using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuInTienQuayThuoc
{
    public class PhieuInTienQuayThuocViewModel : BaseViewModel
    {
        public string hostingName { get; set; }
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
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
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
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
        // xac nhan in 
        public long TaiKhoanBenhNhanThuId { get; set; }
        public bool InPhieuThu { get; set; }
        public bool InBangKe { get; set; }

    }
}
