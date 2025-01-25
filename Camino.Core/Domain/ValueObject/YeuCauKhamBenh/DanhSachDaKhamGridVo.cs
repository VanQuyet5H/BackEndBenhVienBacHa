using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenh
{
    public class DanhSachDaKhamGridVo : GridItem
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool LaKhamDoan { get; set; }
        //public int STT { get; set; }
        public long? PhongBenhVienId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public bool BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDangKy { get; set; }
        public string ThoiDiemDangKyDisplay { get; set; }
        public string BacSiThucHien { get; set; }
        public long BacSiThucHienId { get; set; }
        public string BacSiKetLuan { get; set; }
        public string ThongTinKhamTheoKhoa { get; set; }
        /// //
        /// </summary>
        public string MaYeuCauTiepNhan { get; set; }
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string NamSinhDisplay { get; set; }
        public string DiaChi { get; set; }
        public string TrangThaiYeuCauTiepNhanDisplay { get; set; }
        public string ThoiDiemTiepNhanDisplay { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public string TrieuChungTiepNhan { get; set; }
        public string DoiTuong { get; set; }
        public long? NoiTiepNhanId { get; set; }
        // 
        public string Phong { get; set; }
        public string BaSiKham { get; set; }
        public string LyDoKham { get; set; }
        public string TrieuChungLamSang { get; set; }
        public string ChuanDoanICD { get; set; }
        //don thuoc
        public string Duoc { get; set; }
        public string HoatChat { get; set; }
        public string DonViTinh { get; set; }
        public double? Sang { get; set; }
        public double? Trua { get; set; }
        public double? Toi { get; set; }
        public int? SoNgay { get; set; }
        public double SoLuong { get; set; }
        public string DuongDung { get; set; }
        public string GhiChu { get; set; }

        //
        public DateTime? ThoiDiemHoanThanhKham { get; set; }
        public string ThoiDiemHoanThanhKhamDisplay { get; set; }
        public string KhoaNhapVien { get; set; }

        public string MaBN { get; set; }
        // search k dấu
        public string HoTenRemoveDiacritics => HoTen.RemoveDiacritics();
        public string BacSiThucHienRemoveDiacritics => BacSiThucHien.RemoveDiacritics();
        public string NamSinhRemoveDiacritics => NamSinh.ToString().RemoveDiacritics();
        public string DiaChiRemoveDiacritics => DiaChi.RemoveDiacritics();
        public string TrieuChungTiepNhanRemoveDiacritics => TrieuChungTiepNhan.RemoveDiacritics();
        public string DoiTuongRemoveDiacritics => DoiTuong.RemoveDiacritics();
        //
        public string BaSiKhamRemoveDiacritics => BaSiKham.RemoveDiacritics();
        public string PhongRemoveDiacritics => Phong.RemoveDiacritics();
        public string LyDoKhamRemoveDiacritics => LyDoKham.RemoveDiacritics();
        public string TrieuChungLamSangDiacritics => TrieuChungLamSang.RemoveDiacritics();
        public string ChuanDoanICDDiacritics => ChuanDoanICD.RemoveDiacritics();
        public string CachGiaiQuyet { get; set; }
        public string CachGiaiQuyetDiacritics => CachGiaiQuyet.RemoveDiacritics();
        public DateTime? NgayThucHien { get; set; }
        public string NgayGioBenhNhanVaoPhongKham => NgayThucHien != null ? NgayThucHien.GetValueOrDefault().ApplyFormatDateTimeSACH() : "";


        public bool? BenhNhanDaTaoBenhAn { get; set; }
        public bool? YeuCauTiepNhanDaHoanTat { get; set; }

        // bổ sung searh khám đoàn -> tất cả công ty
        public long? CongTyId { get; set; }
        public long? HopDongId { get; set; }
        public bool? IsKhamDoanTatCa { get; set; }

        public long? KhamTheoPhongYeuCauKhamBenhId { get; set; }
        public string TenDichVuKham { get; set; }
        public bool? CoNhapVien { get; set; }

        //BVHD-3889
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDate();
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public string ThoiDiemHoanThanhDisplay => ThoiDiemHoanThanh?.ApplyFormatDate();

        //BVHD-3924
        public string TenCongTy { get; set; }
    }
    public class DanhSachDaKhamKhamBenhGridVo : GridItem
    {
        public string ThongTinKhamTheoKhoa { get; set; }
        public string LyDoKham { get; set; }
        public string GhiChuTrieuChungLamSang { get; set; }
        public List<Entities.YeuCauKhamBenhs.YeuCauKhamBenhTrieuChung> TrieuChungLamSang { get; set; }
        public List<Entities.YeuCauKhamBenhs.YeuCauKhamBenhChuanDoan> ChuanDoanBanDau { get; set; }

    }


    public class DanhSachDaKhamGridVoSearch
    {
        public long? NoiTiepNhanId { get; set; }
        public string Searching { get; set; }
    }
    public class TrieuChungBenhSu
    {
        public string TenDichVu { get; set; }
        public string TrieuChungTiepNhan { get; set; }
        public string BenhSu { get; set; }
        public string KhamToanThan { get; set; }
        public string ChanDoanSoBoICDId { get; set; }
        public string ChanDoanSoBoGhiChu { get; set; }

        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }
        public string NoiGioiThieu { get; set; }
        //
        public bool IsKhamDoan { get; set; }
        public Enums.PhanLoaiSucKhoe? KSKPhanLoaiTheLuc { get; set; }

        public string KSKPhanLoaiTheLucString => KSKPhanLoaiTheLuc != null ? KSKPhanLoaiTheLuc.GetValueOrDefault().GetDescription() : "";

        //BVHD-3895
        public bool? LaDichVuKhamVietTat { get; set; }
    }
    public class ChuyenKhoaBenhNhan
    {
        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }
    }
    public class ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu
    {
        public Enums.ChuyenKhoaKhamSucKhoe Id { get; set; }
        public string TenKhoa => Id.GetDescription();
        public string TenTemplate { get; set; }
        public string Ten { get; set; }
    }
    public class MatOBJ
    {
        public string KhongKinhMatPhai { get; set; }
        public string KhongKinhMatTrai { get; set; }
        public string CoKinhMatPhai { get; set; }
        public string CoKinhMatTrai { get; set; }
        public string CacBenhVeMat { get; set; }
        public string PhanLoai { get; set; }
    }
    public class TaiMuiHongOBJ
    {
        public string TaiPhaiNoiThuong { get; set; }
        public string TaiPhaiNoiTham { get; set; }
        public string TaiTraiNoiThuong { get; set; }
        public string TaiTraiNoiTham { get; set; }
        public string CacBenhTaiMuiHong { get; set; }
        public string PhanLoai { get; set; }
    }
    public class RangHamMatOBJ
    {
        public string HamTren { get; set; }
        public string HamDuoi { get; set; }

        public string CacBenhRangHamMat { get; set; }
        public string PhanLoai { get; set; }
    }
    public class ListAll
    {
        public List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu> ListNoiKhoa { get; set; }
        public List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu> ListNgoaiKhoa { get; set; }
        public List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu> ListSanPhuKhoa { get; set; }
        public List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu> ListDaLieu { get; set; }
        public MatOBJ Mat { get; set; }
        public TaiMuiHongOBJ TaiMuiHong { get; set; }
        public RangHamMatOBJ RangHamMat { get; set; }
    }
}
