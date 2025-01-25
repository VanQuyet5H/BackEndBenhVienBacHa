using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ToaThuocMau
{
    public class ToaThuocMauGridVo : GridItem
    {
        public string Ten { get; set; }
        public long? ICDId { get; set; }
        public string MaICD { get; set; }
        public string TenTiengVietICD { get; set; }
        public string TenICD { get; set; }
        public long? TrieuChungId { get; set; }
        public string TenTrieuChung { get; set; }
        public long? ChuanDoanId { get; set; }
        public string ChuanDoan { get; set; }
        public string MaChuanDoan { get; set; }
        public string TenTiengVietChuanDoan { get; set; }
        public long? BacSiKeToaId { get; set; }
        public string TenBacSiKeToa { get; set; }
        public string GhiChu { get; set; }
        public bool? IsDisabled { get; set; }
    }
    public class ToaThuocMauChiTietGridVo : GridItem
    {
        public long? ToaThuocMauId { get; set; }
        public long? DuocPhamId { get; set; }
        public string TenDuocPham { get; set; }
        public double? SoLuong { get; set; }
        public string SoLuongDisplay { get; set; }
        public int? SoNgayDung { get; set; }
        public string DungSangDisplay { get; set; }
        public string DungTruaDisplay { get; set; }
        public string DungChieuDisplay { get; set; }
        public string DungToiDisplay { get; set; }
        public string ThoiGianDungSangDisplay { get; set; }
        public string ThoiGianDungTruaDisplay { get; set; }
        public string ThoiGianDungChieuDisplay { get; set; }
        public string ThoiGianDungToiDisplay { get; set; }
        public string GhiChu { get; set; }
    }
    public class ICDTemplateVos
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public int? Rank { get; set; }
    }
    public class NhanVienTemplateVos
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string SoDienThoai { get; set; }
        public int? Rank { get; set; }
    }

    public class ChuanDoanTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
    }

    public class DuocPhamTemplateGridVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string MaDuocPhamBenhVien { get; set; }
        public bool? SuDungTaiBenhVien { get; set; }
        public string DuongDung { get; set; }
        public int? Rank { get; set; }
        public string HamLuong { get; set; }
        public string NhaSanXuat { get; set; }

    }
}
