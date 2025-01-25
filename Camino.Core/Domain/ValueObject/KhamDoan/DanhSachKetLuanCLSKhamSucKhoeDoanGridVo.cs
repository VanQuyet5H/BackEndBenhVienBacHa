using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class DanhSachKetLuanCLSKhamSucKhoeDoanGridVo : GridItem
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? CongTyKhamSucKhoeId { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }

        public TrangThaiVo TrangThai { get; set; }
        public string SearchString { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string TenNgheNghiep { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string SoChungMinhThu { get; set; }
        public string TenDanToc { get; set; }
        public string TenTinhThanh { get; set; }
        public string NhomDoiTuongKhamSucKhoe { get; set; }
        public int? DichVuDaThucHien { get; set; }
        public int? TongDichVu { get; set; }
        public string KSKKetLuanPhanLoaiSucKhoe { get; set; }
        public string KSKKetLuanDisplay => !string.IsNullOrEmpty(KSKKetLuanPhanLoaiSucKhoe) ? "Rồi" : "Chưa";
        public int? TinhTrang => !string.IsNullOrEmpty(KSKKetLuanPhanLoaiSucKhoe) ? 1 : 0;
        public bool? LaHopDongDaKetLuan { get; set; }
    }
    public class TrangThaiVo
    {
        public bool? ChuaKetLuan { get; set; }
        public bool? DaKetLuan { get; set; }
    }
}
