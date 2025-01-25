using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe
{
    public class GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGrid
    {
        public string TenNhanVien { get; set; }
        public string NgayThucHien { get; set; }
        public List<DanhSachDichVuKyThuatThuocVatTuGrid> DanhSachDichVuKyThuatThuocVatTuList { get; set; }
    }
    public class DanhSachDichVuKyThuatThuocVatTuGrid : GridItem
    {
        public int STT { get; set; }
        public string Nhom { get; set; }
        public string TenDichVu { get; set; }
        public int SoLuong { get; set; }
        public string SoLuongDisPlay { get; set; }
        public decimal DonGia { get; set; }
        public string DonGiaDisplay => DonGia != 0 ? DonGia.ApplyFormatMoneyVND() : Convert.ToDecimal(0).ApplyFormatMoneyVND();
        public decimal TongTien { get; set; }
        public string TongTienDisplay => SoLuong != 0 && DonGia != 0 ? Convert.ToDecimal((SoLuong * DonGia)).ApplyFormatMoneyVND() : Convert.ToDecimal(0).ApplyFormatMoneyVND();
        public bool   FormatSoLuongDuocPhamGayNghienHuongThan { get; set; }
        public bool TruyenMau { get; set; }
    }
    public class XacNhanInPhieuGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
    }
    public class InPhieuGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe
    {
        public string HoTen { get; set; }
        public List<DanhSach> DanhSachArrPrint { get; set; }
        public bool  SelectBenhNhanHoacNguoiNha { get; set; }
    }
    public class DanhSach
    {
        public long Id { get; set; }
        public int STT { get; set; }
        public string TenDichVu { get; set; }
        public string Nhom { get; set; }
        public double SoLuong { get; set; }
        public string SoLuongDisPlay { get; set; }
        public decimal DonGia { get; set; }
        public decimal TongTien { get; set; }
        public bool FormatSoLuongDuocPhamGayNghienHuongThan { get; set; }
        public bool TruyenMau { get; set; }
    }
}
