using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauHoanTra
{
    public class DuocPhamHoanTraGridVo : GridItem
    {
        public long NhaKhoDuocPhamChiTietId { get; set; }
        public long DuocPhamBenhVienId { get; set; }

        public string Id { get; set; }
        public int STT { get; set; }
        public string TenDuocPham { get; set; }
        public string DVT { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string Loai { get { return LaDuocPhamBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay
        {
            get { return SoLuongTon.ApplyNumber(); }
        }
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay
        {
            get { return HanSuDung.ApplyFormatDate(); }
        }

        public DateTime NgayNhap { get; set; }
        public string NgayNhapDisplay
        {
            get { return NgayNhap.ApplyFormatDateTime(); }
        }

        public double SoLuongXuat { get; set; }

        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string TenNhom { get; set; }

        public string MaDuocPham { get; set; }
        public string SoDangKy { get; set; }
        public string SoLo { get; set; }
    }

    public class DaSuaSoLuongXuat
    {
        public string Id { get; set; }
        public int SoLuongXuat { get; set; }
    }

    public class DuocPhamDaChonVo
    {
        public long? DuocPhamBenhVienId { get; set; }
        public string MaDuocPham { get; set; }
        public string TenDuocPham { get; set; }
        public string SoLo { get; set; }
        public string SoDangKy { get; set; }
        public double? SoLuongTon { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public DateTime? HanSuDung { get; set; }
    }

    public class GetDuocPhamOnGroupModel
    {
        public GetDuocPhamOnGroupModel()
        {
            ListDaChon = new List<DaSuaSoLuongXuat>();
        }
        public List<DaSuaSoLuongXuat> ListDaChon { get; set; }
        public long Id { get; set; }
        public long KhoXuatId { get; set; }
        public string SearchString { get; set; }
    }

    public class GetVatTuOnGroupModel
    {
        public GetVatTuOnGroupModel()
        {
            ListDaChon = new List<DaSuaSoLuongXuat>();
        }
        public List<DaSuaSoLuongXuat> ListDaChon { get; set; }
        public Enums.LoaiSuDung Id { get; set; }
        public long KhoXuatId { get; set; }
        public string SearchString { get; set; }
    }

    public class GetDpVtOnGroupModel
    {
        public GetDpVtOnGroupModel()
        {
            ListDaChon = new List<DaSuaSoLuongXuat>();
        }
        public List<DaSuaSoLuongXuat> ListDaChon { get; set; }
        public string TenNhom { get; set; }
        public long KhoXuatId { get; set; }
        public string SearchString { get; set; }
    }

    public class ThemDuocPhamHoanTra
    {
        public long? NhapKhoDuocPhamChiTietId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public int? ChatLuong { get; set; }
        public double? SoLuongXuat { get; set; }
        public double? SoLuongTon { get; set; }
        public long? KhoId { get; set; }

        public decimal? DonGia { get; set; }
        public int? VAT { get; set; }
        public int? ChietKhau { get; set; }

        public bool LaDuocPhamBHYT { get; set; }

        public long? NhomDuocPhamId { get; set; }

        public Enums.EnumLoaiKhoDuocPham? loaiKhoDuocPhamXuat { get; set; }
        public Enums.XuatKhoDuocPham? loaiXuatKho { get; set; }
    }

    public class YeuCauTraDuocPhamTuTrucVoSearch
    {
        public YeuCauTraDuocPhamTuTrucVoSearch()
        {
            DuocPhamBenhVienVos = new List<YeuCauTraDuocPhamTuTrucChiTietVo>();
        }
        public long? KhoXuatId { get; set; }
        public string SearchString { get; set; }
        public List<YeuCauTraDuocPhamTuTrucChiTietVo> DuocPhamBenhVienVos { get; set; }
        public long? YeuCauTraDuocPhamId { get; set; }
        public bool IsCreate { get; set; }
    }

    public class YeuCauTraDuocPhamTuTrucChiTietVo
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public double? SoLuongTra { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public long? KhoXuatId { get; set; }
        public long? XuatKhoDuocPhamChiTietViTriId { get; set; }
    }

    public class YeuCauTraDuocPhamGridVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string Loai { get { return LaDuocPhamBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon.ApplyNumber();
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string TenNhom { get; set; }
        public string Ma { get; set; }
        public string SoLo { get; set; }
        public string SoDangKy { get; set; }
        public double SoLuongTra { get; set; }
        public decimal DonGiaNhap { get; set; }
        public DateTime? HanSuDung { get; set; }
        public DateTime? NgayNhap { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
        public string NgayNhapDisplay => NgayNhap?.ApplyFormatDate();
        public long? XuatKhoDuocPhamChiTietViTriId { get; set; }
        public long? KhoXuatId { get; set; }
        public long? XuatKhoDuocPhamId { get; set; }
        public string SearchString { get; set; }

    }

    public class YeuCauHoanTraDuocPhamChiTietTheoKhoXuatVos
    {
        public YeuCauHoanTraDuocPhamChiTietTheoKhoXuatVos()
        {
            YeuCauHoanTraDuocPhamChiTiets = new List<YeuCauTraDuocPhamGridVo>();
        }
        public List<YeuCauTraDuocPhamGridVo> YeuCauHoanTraDuocPhamChiTiets { get; set; }
    }
}
