using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.KhoDuocPhams
{
    public class DuocPhamVaVatTuTrongKhoVo
    {
        public long Id { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DonViTinh { get; set; }
        public string DuongDung { get; set; }
        public double SoLuongTon { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HamLuong { get; set; }
        public string NhaSanXuat { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }

    }
    public class DuocPhamTrongKhoChiTiet
    {
        public long Id { get; set; }
        public long KhoId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public double SoLuongDaXuat { get; set; }
        public double SoLuongNhap { get; set; }
        public DateTime HanSuDung { get; set; }
        public DateTime NgayNhapVaoBenhVien { get; set; }
    }
    public class VatTuTrongKhoChiTiet
    {
        public long Id { get; set; }
        public long KhoId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public double SoLuongDaXuat { get; set; }
        public double SoLuongNhap { get; set; }
        public DateTime HanSuDung { get; set; }
        public DateTime NgayNhapVaoBenhVien { get; set; }
    }
}
