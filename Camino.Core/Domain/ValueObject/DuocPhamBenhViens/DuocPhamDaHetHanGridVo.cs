using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuocPhamBenhViens
{
    public class DuocPhamDaHetHanGridVo : GridItem
    {
        public string Kho { get; set; }

        public string DuocPham { get; set; }

        public string HoatChat { get; set; }

        public string DonViTinh { get; set; }

        public string ViTri { get; set; }

        public long? ViTriId { get; set; }

        public double SoLuongTon { get; set; }

        public string NgayHetHanDisplay { get; set; }

        public DateTime NgayHetHan { get; set; }

        public long? KhoId { get; set; }
        public string MaDuocPham { get; set; }
        public string SoLo { get; set; }
        public decimal DonGiaNhap { get; set; }
        public long NhapKhoDuocPhamId { get; set; }
        public long DuocPhamId { get; set; }
        public double ThanhTien => SoLuongTon != 0 && DonGiaNhap != 0 ? (SoLuongTon * Convert.ToDouble(DonGiaNhap)).MathRoundNumber(2) : 0;
        public string HamLuong { get; set; }

        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public Enums.LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public bool LaDuocPhamHayVatTu { get; set; }
    }

    public class DataVaLueHtml
    {
        public string TemplateDuocPham { get; set; }

        public string Now { get; set; }
    }

    public class YeuCauDuocPhamBenhVienTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string DichVu { get; set; }
        public string Ma { get; set; }
        public string TenKhoa { get; set; }
        public string HoatChat { get; set; }
        public string SoDangKy { get; set; }
        public decimal? Gia { get; set; }
        public long? NhomGiaDuocPhamBenhVienId { get; set; }
    }
}
