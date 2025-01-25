
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.ThongKeDanhSachKhamDoan
{
    public class ThongKeDanhSachKhamDoanVo : GridItem
    {
        public long? YeuCauTiepNhanId { get; set; }
        public string TenCongTy { get; set; }
        public string TenHopDong { get; set; }
        public string HoTen { get; set; }
        public string NgayThangNamSinh { get; set; }
        public string MaNguoiBenh { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh?.GetDescription();

        public string DichVuTrongGoiDaThucHien { get; set; }
        public string ThoiGianThucHienDisplay { get; set; }
        public string DichVuTrongGoiChuaThucHien { get; set; }

        public string DichVuPhatSinhDaThucHien { get; set; }
        public string ThoiGianThucHienDichVuPhatSinhDisplay { get; set; }
        public string DichVuPhatSinhChuaThucHien { get; set; }
    }

    public class CongTyJson
    {
        public string CongTyKhamSucKhoeId { get; set; }
    }

    public class ThongKeDichVuKhamSucKhoeQueryInfo
    {
        public int? CongTyKhamSucKhoeId { get; set; }
        public int? HopDongKhamSucKhoeId { get; set; }
        public string SearchString { get; set; }
        public string TimKiem { get; set; }        
    }
}
