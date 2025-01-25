using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class YeuCauChiDinhDuocPhamVuGridVo 
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
    }

    public class GoiDichVuChiDinhGridVo 
    {
        public GoiDichVuChiDinhGridVo()
        {
            GoiChietKhaus = new List<KhamBenhGoiDichVuGridVo>();
            GoiKhacs = new List<KhamBenhGoiDichVuGridVo>();
        }
        public string Ten { get; set; }
        public decimal? TongThanhTien { get; set; }
        public decimal? TongThanhTienTrongGoi { get; set; }
        public decimal? TongTienBNThanhToan { get; set; }
        public decimal? TLChietKhau { get; set; }
        public List<KhamBenhGoiDichVuGridVo> GoiChietKhaus { get; set; }
        public List<KhamBenhGoiDichVuGridVo> GoiKhacs { get; set; }

    }
    public class GoiDichVuChiDinhLSKBGridVo : GridItem
    {
        public GoiDichVuChiDinhLSKBGridVo()
        {
            GoiChietKhaus = new List<LichSuKhamBenhGridVo>();
            GoiKhacs = new List<LichSuKhamBenhGridVo>();
        }
        public string Ten { get; set; }
        public decimal? TongThanhTien { get; set; }
        public decimal? TongThanhTienTrongGoi { get; set; }
        public decimal? TongTienBNThanhToan { get; set; }
        public decimal? TLChietKhau { get; set; }
 
        public List<LichSuKhamBenhGridVo> GoiChietKhaus { get; set; }
        public List<LichSuKhamBenhGridVo> GoiKhacs { get; set; }

    }
    #region tìm kiếm popup in
    public class TimKiemPopupInKhamBenhGoiDichVuVo
    {
        public string DanhSachCanSearchs { get; set; }
        public string Searching { get; set; }
    }
    public class TimKiemPopupInKhamBenhKhamBenhGoiDichVuGridVo : GridItem
    {
        public long? Id { get; set; }
        public string Nhom { get; set; }
        public int? NhomId { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string TenTT43 { get; set; }
        //public string TenLoaiGia { get; set; }
        //public long? LoaiGia { get; set; }

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
        public bool? DuocHuongBaoHiem { get; set; }
        public decimal? SoTienMG { get; set; }
        //public decimal? BNThanhToan => KhongTinhPhi != true ? (ThanhTien - (DonGiaBaoHiem * (decimal)SoLuong)) : 0;
        public bool? KhongTinhPhi { get; set; }
        //khong co chiet khau

        //co chiet khau
        public decimal? TLChietKhau { get; set; }
        public decimal? DonGiaTrongGoi { get; set; }
        public decimal? ThanhTienTrongGoi { get; set; }
        //co chiet khau

        public string NoiThucHien { get; set; }
        //public long? NoiThucHienId { get; set; }
        public string NoiThucHienIdStr { get; set; }
        public string TenNguoiThucHien { get; set; }
        public long? NguoiThucHienId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? LoaiYeuCauDichVuId { get; set; }
        public long? NhomGiaDichVuBenhVienId { get; set; }
        public string MaTT37 { get; set; }
        public string Ma4350 { get; set; }
        public long? KhoaPhongId { get; set; }
        public int? NhomChiPhiId { get; set; }
        public string MaGiaDichVu { get; set; }
        public string TrangThaiDichVu { get; set; }
        public int? TrangThaiDichVuId { get; set; }
        public Domain.Enums.EnumDanhMucNhomTheoChiPhi? NhomChiPhiDichVuKyThuatId { get; set; }
        public long? NhomDichVuKyThuatBenhVienId { get; set; }
        public bool? KiemTraBHYTXacNhan { get; set; }
        public bool? isCheckRowItem { get; set; }
        public long? GiuongBenhId { get; set; }
        public string TenGiuongBenh { get; set; }
        public int? STT { get; set; }
        public bool? DaThanhToan { get; set; }
        public bool? KhongThucHien { get; set; }
        public string LyDoKhongThucHien { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public string TenGoiDichVu { get; set; }
        public bool? IsDichVuXetNghiem { get; set; }
        public string BenhPhamXetNghiem { get; set; }
        public DateTime? ThoiGianChiDinh { get; set; }
        public string ThoiGianChiDinhDisplay => ThoiGianChiDinh?.ApplyFormatDateTime();

        public bool? IsDichVuHuyThanhToan { get; set; }
        public string LyDoHuyDichVu { get; set; }

        public string NguoiChiDinhDisplay { get; set; }
        public bool? CoDichVuNayTrongGoi { get; set; }
        public bool? LaDichVuTrongGoi => YeuCauGoiDichVuId != null;
        //public bool? CoDichVuNayTrongGoiKhuyenMai { get; set; }
        //public bool? CoThongTinMienGiam { get; set; }
        //public bool? LaDichVuKhuyenMai => CoDichVuNayTrongGoiKhuyenMai != null  && CoThongTinMienGiam;

        // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
        public LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }

        public bool ShowButtonHoanThanhDichVu => LoaiDichVuKyThuat != null
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem
                                                 && TrangThaiDichVuId != (int)EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                 && TrangThaiDichVuId != (int)EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
        public bool ShowButtonHuyHoanThanhDichVu => LoaiDichVuKyThuat != null
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem
                                                    && TrangThaiDichVuId == (int)EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
        public string LyDoHuyTrangThaiDaThucHien { get; set; }

    }
    #endregion
    public class KhamBenhGoiDichVuGridVo : GridItem
    {
        public long? Id { get; set; }
        public string Nhom { get; set; }
        public int NhomId { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string TenTT43 { get; set; }
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
        public decimal? BNThanhToan => KhongTinhPhi != true ? (ThanhTien - (DonGiaBaoHiem * (decimal)SoLuong)) : 0;
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
        public long? NguoiThucHienId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? LoaiYeuCauDichVuId { get; set; }
        public long NhomGiaDichVuBenhVienId { get; set; }
        public string MaTT37 { get; set; }
        public string Ma4350 { get; set; }
        public long? KhoaPhongId { get; set; }
        public int NhomChiPhiId { get; set; }
        public string MaGiaDichVu { get; set; }
        public string TrangThaiDichVu { get; set; }
        public int TrangThaiDichVuId { get; set; }
        public Domain.Enums.EnumDanhMucNhomTheoChiPhi? NhomChiPhiDichVuKyThuatId { get; set; }
        public long? NhomDichVuKyThuatBenhVienId { get; set; }
        public bool? KiemTraBHYTXacNhan { get; set; }
        public bool? isCheckRowItem { get; set; }
        public long? GiuongBenhId { get; set; }
        public string TenGiuongBenh { get; set; }
        public int STT { get; set; }
        public bool DaThanhToan { get; set; }
        public bool KhongThucHien { get; set; }
        public string LyDoKhongThucHien { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long? ChuongTrinhGoiDichVuId { get; set; }
        public string TenGoiDichVu { get; set; }
        public bool IsDichVuXetNghiem { get; set; }
        public string BenhPhamXetNghiem { get; set; }
        public DateTime? ThoiGianChiDinh { get; set; }
        public string ThoiGianChiDinhDisplay => ThoiGianChiDinh?.ApplyFormatDateTime();

        public bool IsDichVuHuyThanhToan { get; set; }
        public string LyDoHuyDichVu { get; set; }

        public string NguoiChiDinhDisplay { get; set; }
        public bool CoDichVuNayTrongGoi { get; set; }
        public bool LaDichVuTrongGoi => YeuCauGoiDichVuId != null;
        public bool CoDichVuNayTrongGoiKhuyenMai { get; set; }
        public bool CoThongTinMienGiam { get; set; }
        public bool LaDichVuKhuyenMai => CoDichVuNayTrongGoiKhuyenMai && CoThongTinMienGiam;

        // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
        // BVHD-3268: bổ sung them nhóm khàm sàng lọc tiêm chủng
        public LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }

        public bool ShowButtonHoanThanhDichVu => LoaiDichVuKyThuat != null
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                                 && TrangThaiDichVuId != (int)EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                 && TrangThaiDichVuId != (int)EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
        public bool ShowButtonHuyHoanThanhDichVu => LoaiDichVuKyThuat != null
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                                    && TrangThaiDichVuId == (int)EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
        public string LyDoHuyTrangThaiDaThucHien { get; set; }
        public bool LaDichVuKhamSangLoc { get; set; }
        public bool LaDichVuVacxin { get; set; }
        public string HighLightClass { get; set; }


        //BVHD-3668
        public long? BenhNhanId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public int? SoThuTuXetNghiem { get; set; }
        public long? DichVuXetNghiemId { get; set; }

        //BVHD-3889
        public long? YeuCauKhamBenhId { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");
    }
    public class LichSuKhamBenhGridVo : GridItem
    {
        public long? Id { get; set; }
        public string Nhom { get; set; }
        public int NhomId { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }

        //khong co chiet khau
        public decimal? BHYTThanhToan { get; set; }
        public int? TLMG { get; set; }
        public int? TLBaoHiemThanhToan { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public decimal? SoTienMG { get; set; }
        public decimal? BNThanhToan { get; set; }
        //khong co chiet khau

        //co chiet khau
        public decimal? TLChietKhau { get; set; }
        public decimal? DonGiaTrongGoi { get; set; }
        public decimal? TTTrongGoi { get; set; }
        //co chiet khau

        public string NoiThucHien { get; set; }
        public long? LoaiYeuCauDichVuId { get; set; }
        public long NhomGiaDichVuBenhVienId { get; set; }
        public string MaTT37 { get; set; }
        public string Ma4350 { get; set; }
        public long? KhoaPhongId { get; set; }
        public int NhomChiPhiId { get; set; }
        public string MaGiaDichVu { get; set; }
        public string TrangThaiDichVu { get; set; }
        public long? NoiThucHienId { get; set; }
        public bool? KiemTraBHYTXacNhan { get; set; }
        public long? NguoiThucHienId { get; set; }
        public string TenNguoiThucHien { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public string TenGiuongBenh { get; set; }
        public int TrangThaiDichVuId { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public string BenhPhamXetNghiem { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public string TenGoiDichVu { get; set; }
        public DateTime? ThoiGianChiDinh { get; set; }
        public string ThoiGianChiDinhDisplay => ThoiGianChiDinh?.ApplyFormatDateTime();
        public string NguoiChiDinhDisplay { get; set; }
    }
    public class SoLuong
    {
        public int SoLuongKhamBenh { get; set; }
        public int SoLuongKyThuat{ get; set; }
        public int SoLuongVatTu { get; set; }
        public int SoLuongDuocPham { get; set; }
        public int SoLuongGiuong { get; set; }
    }
    public class GoiDichKhongCoChietKhau
    {
        public long? Id { get; set; }
        public string Nhom { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }
        public decimal? BHYTThanhToan { get; set; }
        public int? TLMG { get; set; }
        public decimal? SoTienMG { get; set; }
        public decimal? BNThanhToan { get; set; }
        //khong co chiet khau
        //co chiet khau
        public int? TLChietKhau { get; set; }
        public decimal? DonGiaTrongGoi { get; set; }
        public decimal? ThanhTienTrongGoi { get; set; }
        //co chiet khau

        public string NoiThucHien { get; set; }
    }
    public class KhamBenhGoiDichVu : GridItem
    {
        public long? Id { get; set; }
        public string Nhom { get; set; }
        public int NhomId { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }

        //khong co chiet khau
        public decimal? BHYTThanhToan { get; set; }
        public int? TLMG { get; set; }
        public decimal? SoTienMG { get; set; }
        public decimal? BNThanhToan { get; set; }
        //khong co chiet khau

        //co chiet khau
        public int? TLChietKhau { get; set; }
        public decimal? DonGiaTrongGoi { get; set; }
        public decimal? ThanhTienTrongGoi { get; set; }
        //co chiet khau

        public string NoiThucHien { get; set; }

    }

    public class GridChiDinhDichVuQueryInfoVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public int? NhomDichVuId { get; set; }
        public bool? IsKhamDoanTatCa { get; set; }
    }
}