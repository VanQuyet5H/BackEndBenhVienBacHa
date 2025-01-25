using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class PhauThuatThuThuatCanLamSanGridVo : GridItem
    {
        public PhauThuatThuThuatCanLamSanGridVo()
        {
            LichSuPhienXetNghiemIds = new List<long>();
        }

        //PhieuDieuTri
        public Domain.Enums.LoaiDichVuKyThuat LoaiDichVuKyThuatEnum { get; set; }
        public long? PhieuDieuTriId { get; set; }
        public DateTime? GioBatDau { get; set; }
        public string GioBatDauDisplay => GioBatDau?.ApplyFormatTime();

        public string NgayYLenh { get; set; }
        public bool CoBHYT { get; set; }
        //public long? Id { get; set; }
        public string Nhom { get; set; }
        public string NhomTheoPhienXetNghiem => $"{Nhom}@PhienXetNghiemId={PhienXetNghiemId}"; //cheat tách nhóm XN ra nhiều phiên
        public long NhomDichVuBenhVienId { get; set; }
        public long NhomId { get; set; }
        public int LoaiDichVuKyThuat { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string TenLoaiGia { get; set; }
        public long? LoaiGia { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }
        //khong co chiet khau
        public decimal? BHYTThanhToan { get; set; }
        public int? TLMG { get; set; }
        public int? TLBaoHiemThanhToan { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public decimal? SoTienMG { get; set; }
        public decimal? BNThanhToan { get; set; }
        public bool? KhongTinhPhi { get; set; }
        //khong co chiet khau
        //co chiet khau
        public decimal? TLChietKhau { get; set; }
        public decimal? DonGiaTrongGoi { get; set; }
        public decimal? ThanhTienTrongGoi { get; set; }
        //co chiet khau
        public string NoiThucHien { get; set; }
        public long NoiThucHienId { get; set; }
        public string NoiThucHienIdStr { get; set; }
        public string TenNguoiThucHien { get; set; }
        public string ThoiDiemThucHienDisplay { get; set; }
        public long? NguoiThucHienId { get; set; }
        public long? LoaiYeuCauDichVuId { get; set; }
        public long NhomGiaDichVuBenhVienId { get; set; }
        public string MaTT37 { get; set; }
        public string Ma4350 { get; set; }
        public string TenTT43 { get; set; }
        public long? KhoaPhongId { get; set; }
        public int NhomChiPhiId { get; set; }
        public string MaGiaDichVu { get; set; }
        public string TrangThaiDichVu { get; set; }
        public int TrangThaiDichVuId { get; set; }
        public EnumDanhMucNhomTheoChiPhi? NhomChiPhiDichVuKyThuatId { get; set; }
        public long? NhomDichVuKyThuatBenhVienId { get; set; }
        public bool? KiemTraBHYTXacNhan { get; set; }
        public bool? isCheckRowItem { get; set; }
        public long? GiuongBenhId { get; set; }
        public string TenGiuongBenh { get; set; }
        public int STT { get; set; }
        public bool DaThanhToan { get; set; }
        public long NhanVienTaoYeuCauDichVuKyThuatId { get; set; }
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
        public List<KetQuaCanLamSanPTTT> KetQuaCanLamSanPTTT { get; set; }
        public int LoaiTapTin { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public string TenGoiDichVu { get; set; }
        public bool IsDichVuXetNghiem { get; set; }
        public string BenhPhamXetNghiem { get; set; }
        public bool IsCheckPhieuIn { get; set; }
        public string LoaiDichVuKyThuatEnumDecription { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();
        public long? PhienXetNghiemId { get; set; }
        public int? LanThucHien { get; set; }
        public bool? GoiXetNghiemLai { get; set; }
        public List<long> LichSuPhienXetNghiemIds { get; set; }
        public bool IsDichVuHuyThanhToan { get; set; }
        public string LyDoHuyDichVu { get; set; }
        public string NguoiChiDinhDisplay { get; set; }

        public bool CoDichVuNayTrongGoi { get; set; }
        public bool LaDichVuTrongGoi => YeuCauGoiDichVuId != null;
        public bool CoDichVuNayTrongGoiKhuyenMai { get; set; }
        public bool CoThongTinMienGiam { get; set; }
        public bool LaDichVuKhuyenMai => CoDichVuNayTrongGoiKhuyenMai && CoThongTinMienGiam;
        public bool? KhongThucHien { get; set; }
        public string LyDoKhongThucHien { get; set; }

        //public long? YeuCauDichVuKyThuatKhamSangLocTiemChungId { get; set; }
        public bool IsThuocNhomDichVuTiemChung { get; set; }

        //BVHD-3654
        public bool TinhPhi { get; set; }

        //BVHD-3575
        public long? YeuCauTiepNhanNgoaiTruId { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");

        // BVHD-3959 
        public bool ChecBoxItem { get; set; }
        public DateTime? ThoiGianDienBien { get; set; }
        public string ThoiGianDienBienDisplayname => ThoiGianDienBien != null ? ThoiGianDienBien.GetValueOrDefault().ApplyFormatDateTime():"";

        //Cập nhật 14/12/2022: grid load chậm
        public long? BenhNhanId { get; set; }
        public long? ChuongTrinhGoiDichVuId { get; set; }
        public int? SoThuTuXetNghiem { get; set; }
        public long? DichVuXetNghiemId { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class KetQuaCanLamSanPTTT
    {
        public string TenFile { get; set; }
        public string Url { get; set; }
        //public string LoaiFile { get; set; }
        public LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
    }

    public class ListDichVuChiDinhCLSPTTT
    {
        public long NhomChiDinhId { get; set; }
        public long DichVuChiDinhId { get; set; }
        public string TenNhom { get; set; }
        public string TenNhomCon { get; set; }
        public int ThuTuIn { get; set; }
        public int LoaiDichVuKyThuatEnum { get; set; }
        public long NhanVienChiDinhId { get; set; }
    }
    public class ListDichVuChiDinhCLSTheoNhom
    {
        public ListDichVuChiDinhCLSTheoNhom()
        {
            ListDichVuChiDinhId = new List<long>();
        }
        public long NhomChiDinhId { get; set; }
        public List<long> ListDichVuChiDinhId { get; set; }
        public string TenNhom { get; set; }
        public string TenNhomCon { get; set; }
        public int ThuTuIn { get; set; }
    }
    public class DienBienQueryInfo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long PhieuDieuTriId { get; set; }
    }
    public class LookupItemDienBienVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }
        public DateTime ThoiGian { get; set; }
    }
    public class ApDungThoiGianDienBienVo
    {
        public ApDungThoiGianDienBienVo()
        {
            DataGridDichVuChons = new List<PhauThuatThuThuatCanLamSanGridVo>();
        }
        public List<PhauThuatThuThuatCanLamSanGridVo> DataGridDichVuChons { get; set; }
      
        public DateTime? ThoiGianDienBien { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long PhieuDieuTriId { get; set; }
    }
   
}
