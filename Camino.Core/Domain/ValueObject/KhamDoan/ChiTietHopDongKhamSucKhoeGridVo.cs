using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class ChiTietHopDongKhamSucKhoeGridVo :GridItem
    {
        public string TenCongTy { get; set; }
        public string SoHopDong { get; set; }
        public DateTime NgayHopDong { get; set; }
        public int LoaiHopDong { get; set; }
        public int  TrangThai { get; set; }
        public int SoBenhNhan { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public List<DSNhanVien> DanhSachNhanVien { get; set; }
    }
    public class DSNhanVien : GridItem
    {
        public int STT { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string DonViBoPhan { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh {get;set;}
        public string GioiTinhDisplay => GioiTinh != null ? GioiTinh.GetValueOrDefault().GetDescription() : "";
        public int NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string ChungMinhThu { get; set; }
        public string SHC { get; set; }
        public string DanToc { get; set; }
        public string TinhThanh { get; set; }
        public string NhomKham { get; set; }
        public string GhiChu { get; set; }
        public int TinhTrang { get; set; }
        public string TenTinhTrang => TinhTrang == 1 ? "Hủy Khám" : "Đã Khám";
    }
}
