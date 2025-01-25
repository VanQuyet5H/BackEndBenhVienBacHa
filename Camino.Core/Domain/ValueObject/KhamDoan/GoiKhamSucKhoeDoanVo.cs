using Camino.Core.Domain.ValueObject.Grid;
using System;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class GoiKhamSucKhoeDoanVo : GridItem
    {
        public string MaGoiKham { get; set; }
        public string TenGoiKham { get; set; }
        public string TenCongTy { get; set; }
        public string SoHopDong { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string NgayHieuLucDisplay => NgayHieuLuc?.ApplyFormatDate();
        public string NgayKetThucDisplay => NgayKetThuc?.ApplyFormatDate();
        public string SearchString { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public bool? IsHopDongKhamSucKhoe { get; set; }
    }

    public class GoiKhamSucKhoeDoanKetLuanVo : GridItem
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public string DonVi { get; set; }
        public string GioiTinh { get; set; }
        public string NamSinh { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string CMND { get; set; }
        public string SHC { get; set; }
        public string DanToc { get; set; }
        public string TinhTP { get; set; }
        public string NhomKham { get; set; }
        public string GhiChu { get; set; }
        public string TinhTrangDV { get; set; }
        public int TinhTrangTu { get; set; }
        public int TinhTrangMau { get; set; }
        public int KetLuan { get; set; }
        public string KetLuanDisplay => KetLuan == 1 ? "Rồi" : "Chưa";
        //search
        public string SearchString { get; set; }
        public string TenCongTy { get; set; }
        public string TenSoHopDong { get; set; }
        public bool? ChuaKetLuan { get; set; }
        public bool? DaKetLuan { get; set; }
    }

    public class LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public int Loai { get; set; }
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public string MaNhomDichVuBenhVien { get; set; }
        public string MaNhomDichVuBenhVienCha { get; set; }
        public string LoaiDisplay => Loai == 1 ? "DV Khám" : "DVKT";
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }

        public Enums.NhomDichVuChiDinhKhamSucKhoe NhomDichVu
        {
            get
            {
                switch (LoaiDichVuKyThuat)
                {
                    case Enums.LoaiDichVuKyThuat.XetNghiem:
                        return NhomDichVuChiDinhKhamSucKhoe.XetNghiem;
                    case Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh:
                        return NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh;
                    case Enums.LoaiDichVuKyThuat.ThamDoChucNang:
                        return NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang;
                    default:
                        return NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
                }
            }
        }

        public string TenNhom => NhomDichVu.GetDescription();
        public string TenLoaiDichVu => LoaiDichVuKyThuat == null ? NhomDichVuChiDinhKhamSucKhoe.KhamBenh.GetDescription() : LoaiDichVuKyThuat.GetDescription();
    }
    public class LookupItemDVKhamBenhKyThuatBvVo
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public int Loai { get; set; }
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public string MaNhomDichVuBenhVien { get; set; }
        public string MaNhomDichVuBenhVienCha { get; set; }
        public string LoaiDisplay => Loai == 1 ? "DV Khám" : "DVKT";
        public LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public NhomDichVuChiDinhKhamSucKhoe NhomDichVu { get; set; }
        public string TenNhom { get; set; }
        public string TenLoaiDichVu { get; set; }
    }

    public class KeyIdStringGoiDichVuKhamSucKhoeVo
    {
        public long DichVuId { get; set; }
        public int Loai { get; set; }
    }


    public class DichVuKhamBenhBVHoacDVKTBenhVienParams
    {
        //public DichVuKhamBenhBVHoacDVKTBenhVienParams()
        //{
        //    DichVuKhamIds = new List<long>();
        //}
        public bool LaTaoGoiPhatSinh { get; set; } = false;
        public bool CoGoiPhatSinh { get; set; } = false;
        public bool ChonDichVuKham { get; set; } = true;
        public bool ChonDichVuKyThuat { get; set; } = true;
        public bool FullNhomDichVu { get; set; } = false;
        public bool KhongLayTiemChung { get; set; }
        public List<long> DichVuKhamIds { get; set; } = null;
        public List<long> DichVuKyThuatIds { get; set; } = null;
    }

    public class DichVuKhamBenhBVHoacDVKTBenhVienJSON
    {
        public bool? CoGoiPhatSinh { get; set; }
        //public bool? LaDichVuKham { get; set; }

    }

    public class DichVuKhamBenhGiaBenhVienVo
    {
        public long DichVuKhamBenhHoacKyThuatBenhVienId { get; set; }
        public long NhomGiaDichVuKhamBenhHoacKyThuatBenhVienId { get; set; }
        public bool LaDichVuKham { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
    }

    public class LookupItemHopDingKhamSucKhoeTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public long CongTyKhamSucKhoeId { get; set; }
        public string SoHopDong { get; set; }
        public string NgayHieuLucDisplay => NgayHieuLuc.ApplyFormatDate();
        public string NgayKetThucDisplay => NgayKetThuc?.ApplyFormatDate();
    }

    public class DichVuKhamBenhBvDVKTBvVo : GridItem
    {
        public int LoaiKhamBenhHoacKyThuat { get; set; }
    }

    public class MultiselectKhoaPhongTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Khoa { get; set; }
        public string Phong { get; set; }
    }
    public class KhoaPhongJsonVo
    {
        public bool? LaDichVuKham { get; set; }
        public int? HinhThucKhamBenh { get; set; }
        public long? DichVuId { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public KeyIdStringGoiDichVuKhamSucKhoeVo DichVuStringId { get; set; }
    }

    public class GoiKhamSucKhoeDoanExportExcel
    {
        [Width(20)]
        public string MaGoiKham { get; set; }
        [Width(40)]
        public string TenGoiKham { get; set; }
        [Width(30)]
        public string TenCongTy { get; set; }
        [Width(30)]
        public string SoHopDong { get; set; }
        [Width(30)]
        public string NgayHieuLucDisplay { get; set; }
        [Width(30)]
        public string NgayKetThucDisplay { get; set; }
    }

    public class LookupItemDichVuMultiselectTemplateVo
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public decimal? DonGia { get; set; }

        //BVHD-3668
        public long? GoiKhamSucKhoeDichVuPhatSinhId { get; set; }
    }

    public class KeyIdStringDichVuKhamSucKhoeVo
    {
        public long DichVuId { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public decimal? DonGia { get; set; }
        public long? GoiKhamSucKhoeDichVuPhatSinhId { get; set; }
    }
    public class GoiKhamSucKhoeDoanChungExportExcel
    {
        [Width(20)]
        public string MaGoiKham { get; set; }
        [Width(40)]
        public string TenGoiKham { get; set; }
    }

    public class LookupItemNhomDichVuChiDinhKhamSucKhoeVo : LookupItemTemplateVo
    {
        public bool LaGoiPhatSinh { get; set; }
    }
}
