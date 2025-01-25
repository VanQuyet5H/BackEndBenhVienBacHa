using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class SreachHopDongKhamVo : GridItem
    {
        public string SearchString { get; set; }
    }

    public class NhanSuGridVo
    {
        public long? Id { get; set; }
        public string TenNhanSu { get; set; }
    }

    public class KiemTraHopDongNhanVienChuaKham
    {
        public bool NhanVienKhamXong { get; set; }

        public long TrangThai { get; set; }
        public string TenTrangThai { get; set; }
        public string TenNhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string NgaySinh { get; set; }
        public string GioiTinh { get; set; }
    }

    public class KhamDoanHopDongKhamGridVo : GridItem
    {
        public string SoHopDong { get; set; }
        public string TenCongTy { get; set; }
        public DateTime NgayHopDong { get; set; }
        public string NgayHopDongDisplay => NgayHopDong.ApplyFormatDate();
        public string DiaChiKham { get; set; }
        public LoaiHopDong LoaiHopDong { get; set; }
        public string LoaiHopDongDisplay => LoaiHopDong.GetDescription();
        public string NgayKham { get; set; }
        public int TrangThai { get; set; }
        public string TenTrangThai { get; set; }
    }

    public class HopDongKhamSucKhoeNhanVienGridVo : GridItem
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public int? STTNhanVien { get; set; }
        public string MaBN { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string HoTenKhac { get; set; }
        public string TenDonVi { get; set; }
        public string ViTriCongTac { get; set; }
        public string GioiTinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string CMTSHC { get; set; }
        public string DanToc { get; set; }
        public string TinhThanhPho { get; set; }
        public string NhomKham { get; set; }
        public string GhiChu { get; set; }
        public bool? DaLapGiaDinh { get; set; }
    }

    public class DanhSachPhongKhamTaiCongTyGridVo : GridItem
    {
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public string GhiChu { get; set; }
        public List<long> DanhSachNhanSu { get; set; }
    }

    public class BaoCaoBangKeDichVuHopDongKSKVo : GridItem
    {
        public BaoCaoBangKeDichVuHopDongKSKVo()
        {
            YeuCauTiepNhanVos = new List<BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanVo>();
            //GoiKhamSucKhoeYeuCauDichVuKyThuats = new List<BaoCaoBangKeDichVuKyThuatBenhVienKSKVo>();
            GoiKhamSucKhoeYeuCauKhams = new List<BaoCaoBangKeDichVuKhamBenhKSKVo>();
            GoiKhamSucKhoeIds = new List<long?>();
        }
        public long GoiKhamSucKhoeId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public int? NamSinh { get; set; }
        public string CMND { get; set; }
        public decimal DonGiaUuDaiKhamBenhTrongGoi { get; set; }
        public decimal TongChiPhiTrongGoiKhamBenh { get; set; }
        public decimal TongChiPhiTrongGoiDichVuKyThuat { get; set; }
        public decimal? TongChiPhiNgoaiGoiGoiKhamBenh { get; set; }
        public decimal? TongChiPhiNgoaiGoiDichVuKyThuat { get; set; }
        public decimal TongChiPhiTrongGoi => TongChiPhiTrongGoiKhamBenh + TongChiPhiTrongGoiDichVuKyThuat;
        public decimal? TongChiPhiNgoaiGoi => TongChiPhiNgoaiGoiGoiKhamBenh + TongChiPhiNgoaiGoiDichVuKyThuat;
        public List<long?> GoiKhamSucKhoeIds { get; set; }
        public List<BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanVo> YeuCauTiepNhanVos { get; set; }
        public List<BaoCaoBangKeDichVuKhamBenhKSKVo> GoiKhamSucKhoeYeuCauKhams { get; set; }
        //public List<BaoCaoBangKeDichVuKyThuatBenhVienKSKVo> GoiKhamSucKhoeYeuCauDichVuKyThuats { get; set; }
    }
    public class BaoCaoBangKeDichVuKhamBenhKSKVo
    {
        public long? GoiKhamSucKhoeId { get; set; }
    }

    public class BaoCaoBangKeDichVuKyThuatBenhVienKSKVo
    {
        public long? GoiKhamSucKhoeId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public decimal? DonGiaUuDai { get; set; }
        public int SoLuong { get; set; }
        public bool CoTiepNhanMau { get; set; }
    }

    public class BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanExcelVo
    {
        public BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanExcelVo()
        {
            YeuCauKhamBenhVos = new List<DichVuTrongGoiCuaNhanVien>();
            YeuCauDichVuKyThuatVos = new List<DichVuTrongGoiCuaNhanVien>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public List<DichVuTrongGoiCuaNhanVien> YeuCauKhamBenhVos { get; set; }
        public List<DichVuTrongGoiCuaNhanVien> YeuCauDichVuKyThuatVos { get; set; }
    }


    public class BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanVo
    {
        public BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanVo()
        {
            YeuCauKhamBenhVos = new List<BaoCaoBangKeYeuCauKhamBenhKSKVo>();
            YeuCauDichVuKyThuatVos = new List<BaoCaoBangKeYeuCauDichVuKyThuatKSKVo>();
        }
        public long YeuCauTiepNhanId { get; set; }

        public List<BaoCaoBangKeYeuCauKhamBenhKSKVo> YeuCauKhamBenhVos { get; set; }
        public List<BaoCaoBangKeYeuCauDichVuKyThuatKSKVo> YeuCauDichVuKyThuatVos { get; set; }

    }
    public class BaoCaoBangKeYeuCauKhamBenhKSKVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public EnumTrangThaiYeuCauKhamBenh TrangThai { get; set; }
        public decimal? DonGiaUuDai { get; set; }
        public decimal? Gia { get; set; }
        public int SoLuong { get; set; }
    }

    public class BaoCaoBangKeYeuCauDichVuKyThuatKSKVo
    {
        public long YeuCauDichVuKyThuatId { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat? TrangThai { get; set; }
        public LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
        public decimal? DonGiaUuDai { get; set; }
        public decimal? Gia { get; set; }
        public int SoLuong { get; set; }
        public bool CoTiepNhanMau { get; set; }
    }

    public class BangKeDichVuGridVo : GridItem
    {
        public long GoiKhamSucKhoeId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public int? NamSinh { get; set; }
        public string CMND { get; set; }
        public decimal TongChiPhiTrongGoiKhamBenh { get; set; }
        public decimal TongChiPhiTrongGoiDichVuKyThuat { get; set; }
        public decimal? TongChiPhiNgoaiGoiGoiKhamBenh { get; set; }
        public decimal? TongChiPhiNgoaiGoiDichVuKyThuat { get; set; }
        public decimal TongChiPhiTrongGoi => TongChiPhiTrongGoiKhamBenh + TongChiPhiTrongGoiDichVuKyThuat;
        public decimal? TongChiPhiNgoaiGoi => TongChiPhiNgoaiGoiGoiKhamBenh + TongChiPhiNgoaiGoiDichVuKyThuat;

    }

    public class TongChiPhiTrongGridVo : GridItem
    {
        public LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
        //public NhomDichVuChiDinhKhamSucKhoe? NhomDichVuChiDinhKhamSucKhoe { get; set; }

        public NhomDichVuChiDinhKhamSucKhoe NhomDichVuChiDinhKhamSucKhoe
        {
            get
            {
                var nhom = LaDichVuKham ? NhomDichVuChiDinhKhamSucKhoe.KhamBenh : NhomDichVuChiDinhKhamSucKhoe.KH;
                switch (LoaiDichVuKyThuat)
                {
                    case LoaiDichVuKyThuat.XetNghiem:
                        nhom = NhomDichVuChiDinhKhamSucKhoe.XetNghiem; break;
                    case LoaiDichVuKyThuat.ChuanDoanHinhAnh:
                        nhom = NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh; break;
                    case LoaiDichVuKyThuat.ThamDoChucNang:
                        nhom = NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang; break;
                    case LoaiDichVuKyThuat.Khac:
                        return nhom;
                }
                return nhom;
            }
        }
        public string Nhom => NhomDichVuChiDinhKhamSucKhoe.GetDescription();
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public int SoLuong { get; set; }
        public bool LaDichVuKham { get; set; }
        public decimal DonGiaUuDai { get; set; }
        public decimal ThanhTien => SoLuong * DonGiaUuDai;
        public string NoiThucHien { get; set; }
        public EnumTrangThaiYeuCauKhamBenh TrangThaiYeuCauKhamBenh { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat TrangThaiYeuCauDichVuKyThuat { get; set; }
        public string TrangThaiYeuCauKhamBenhDisplay => TrangThaiYeuCauKhamBenh.GetDescription();
        public string TrangThaiYeuCauDichVuKyThuatDisplay => TrangThaiYeuCauDichVuKyThuat.GetDescription();
    }

    public class XuatFileExcelTrongGoi
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public bool LaDichVuTrongGoi { get; set; }
    }

    public class ExportExcelNhanVienDichVuTrongGoi : GridItem
    {
        public ExportExcelNhanVienDichVuTrongGoi()
        {
            DichVuTrongGoiCuaNhanViens = new List<DichVuTrongGoiCuaNhanVien>();
            MaVaTenDichVuTrongGoiCuaNhanViens = new List<MaVaTenDichVuTrongGoiCuaNhanVien>();
            YeuCauTiepNhanVos = new List<BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanExcelVo>();
        }
        public long GoiKhamSucKhoeId { get; set; }
        public string TenDonViHoacBoPhan { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public int? NamSinh { get; set; }
        public List<BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanExcelVo> YeuCauTiepNhanVos { get; set; }

        public List<DichVuTrongGoiCuaNhanVien> DichVuTrongGoiCuaNhanViens { get; set; }
        public List<MaVaTenDichVuTrongGoiCuaNhanVien> MaVaTenDichVuTrongGoiCuaNhanViens { get; set; }

    }
    public class DichVuTrongGoiCuaNhanVien
    {
        public long Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int SoLuong { get; set; }
        public decimal? DonGiaUuDai { get; set; }
        public bool CoTiepNhanMau { get; set; }
        public decimal ThanhTien => SoLuong * DonGiaUuDai.GetValueOrDefault();
        public long? GoiKhamSucKhoeId { get; set; }
        public LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
    }

    public class MaVaTenDichVuTrongGoiCuaNhanVien
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public decimal ThanhTien { get; set; }
        public bool CoTiepNhanMau { get; set; }
        public LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
    }

    public class MaVaThanhTienDichVuTrongGoiCuaNhanVien
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class MoHopDongKhamViewModel
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public string LyDoMoLaiHopDong { get; set; }
    }

    public class DichVuKhamDoanChiTiet
    {
        public DichVuKhamDoanChiTiet()
        {
            GoiKhamSucKhoeIds = new List<long?>();
        }
        public long? YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public bool LaDichVuTrongGoi { get; set; }
        public List<long?> GoiKhamSucKhoeIds { get; set; }
    }
}
