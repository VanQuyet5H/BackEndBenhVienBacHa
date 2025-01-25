using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.QuayThuoc
{
    public class ThongTinDuocPhamQuayThuocVo
    {
        public ThongTinDuocPhamQuayThuocVo()
        {
            DanhSachMienGiamVos = new List<DanhSachMienGiamVo>();
        }
        public long Id { get; set; }

        public long? STT { get; set; }

        public long DuocPhamId { get; set; }

        public string MaHoatChat { get; set; }
        public string NongDoVaHamLuong { get; set; }

        public string TenDuocPham { get; set; }

        public double SoLuongTon { get; set; }

        public long NhapKhoDuocPhamChiTietId { get; set; }

        public string TenHoatChat { get; set; }

        public string DonViTinh { get; set; }

        public double SoLuongToa { get; set; }

        public double SoLuongMua { get; set; }

        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);

        public decimal DonGiaNhap { get; set; }

        public int TiLeTheoThapGia { get; set; }

        public int VAT { get; set; }

        public decimal ThanhTien => (decimal) SoLuongMua * DonGia;

        public int TLMG { get; set; }
        public decimal SoTienMG { get; set; }

        public decimal BNConPhaiThanhToan => ThanhTien - TongCongNo - SoTienMG;

        public string Solo { get; set; }

        public string ViTri { get; set; }

        public bool CheckedDefault { get; set; }

        public DateTime HanSuDung { get; set; }

        public string HanSuDungHientThi => HanSuDung.ApplyFormatDate();

        public string BacSiKeToa { get; set; }

        public long? BacSiId { get; set; }

        public string HighLightClass { get; set; }

        public bool? isNew { get; set; }

        public bool LaDuocPhamBV { get; set; }

        public decimal TongCongNo => DanhSachCongNoChoThus?.Sum(o => o.SoTienCongNoChoThu) ?? 0;

        public IEnumerable<CongNoChoThuGridVo> DanhSachCongNoChoThus { get; set; }
        public List<long?> TaiKhoanbenhNhanThuId { get; set; }
        public List<long> TaiKhoanbenhNhanChiId { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public bool VatTuBHYT { get; set; }
        public decimal ThanhTienHienThi => DonGia * (decimal)SoLuongMua;
        public decimal BNConPhaiThanhToanHienThi => ThanhTienHienThi - TongCongNo - SoTienMG;
        // update 28/12/2020
        public IEnumerable<DanhSachMienGiamVo> DanhSachMienGiamVos { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public Enums.EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
    }

    public class CongNoChoThuGridVo
    {
        public long CongNoId { get; set; }

        public string TenCongTy { get; set; }

        public decimal SoTienCongNoChoThu { get; set; }

        public long ViTri { get; set; }
    }

    public class DuocPhamMuaThuocTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DonViTinh { get; set; }
        public string DuongDung { get; set; }

        public double? SoLuongTon { get; set; }
        public string HanSuDung { get; set; }
        public string Loai { get; set; }
    }

    public class DuocPhamVaVatTuTNhaThuocTemplateVo
    {
        public long KeyId { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DonViTinh { get; set; }
        public string DuongDung { get; set; }
        public double? SoLuongTon { get; set; }
        public string HanSuDung { get; set; }
        public string Loai { get; set; }
        public string HamLuong { get; set; }
        public string NhaSanXuat { get; set; }
    }

    public class ThongTinDuocPhamQuayThuocCuVo
    {
        public long Id { get; set; }

        public long? STT { get; set; }

        public long DuocPhamId { get; set; }

        public string MaHoatChat { get; set; }
        public string NongDoVaHamLuong { get; set; }

        public string Ma { get; set; }

        public string TenDuocPham { get; set; }

        public double SoLuongTon { get; set; }

        public long NhapKhoDuocPhamChiTietId { get; set; }

        public string TenHoatChat { get; set; }

        public string DonViTinh { get; set; }

        public double SoLuongToa { get; set; }

        public double SoLuongMua { get; set; }

        public decimal? DonGia { get; set; }

        public decimal ThanhTien { get; set; }

        public int TLMG { get; set; }
        public decimal SoTienMG { get; set; }

        public decimal BNConPhaiThanhToan => ThanhTien - TongCongNo - SoTienMG;

        public string Solo { get; set; }

        public string ViTri { get; set; }

        public bool CheckedDefault { get; set; }

        public DateTime HanSuDung { get; set; }

        public string HanSuDungHientThi => HanSuDung.ApplyFormatDate();

        public string BacSiKeToa { get; set; }


        public bool? isNew { get; set; }

        public bool LaDuocPhamBV { get; set; }

        public decimal TongCongNo => DanhSachCongNoChoThus?.Sum(o => o.SoTienCongNoChoThu) ?? 0;

        public List<CongNoChoThuGridVo> DanhSachCongNoChoThus { get; set; }
        public List<long?> TaiKhoanbenhNhanThuId { get; set; }
        public Enums.EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
        public string HighLightClass { get; set; }
        public bool ThuocTrongKhoExit { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }

    }
    public class DonThuocCuaBenhNhanGridVo : GridItem
    {
        public int STT { get; set; }
        public string MaDon { get;set; }
        public DateTime NgayKeDonDate { get; set; }
        public string NgayKeDon { get; set; }
        public string DVKham { get; set; }
        public string BSKham { get; set; }
        public double SoTien { get; set; }
        public string TotalSoTien { get; set; }
        public string SoTienDisPlay { get; set; }
        public string SoTienTongDisPlay { get; set; }
        public int TTThanhToan { get; set; }
        public int TTXuatThuoc { get; set; }
        public string TinhTrang { get; set; }
        public string LoaiDonThuoc { get; set; }
        public string HighLightClass { get; set; }
        public bool DonThuocBacSiKeToa { get; set; }
        public long DuocPhamId { get; set; }
         public long YeuCauKhamBenhDonThuocId { get; set; }
        public long? YeuCauKhambenhId { get; set; }
        public long DonThuocThanhToanId { get; set; }
    }
}
