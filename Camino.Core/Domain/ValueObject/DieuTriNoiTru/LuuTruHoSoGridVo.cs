using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class LuuTruHoSoGridVo : GridItem
    {
        public LuuTruHoSoGridVo()
        {
            NoiTruPhieuDieuTriInfos = new List<NoiTruPhieuDieuTriInfoGridVo>();
        }
        public string MaTN { get; set; }
        public string SoBA { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string KhoaNhapVien { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh?.GetDescription();
        public string ThuTuSapXepLuuTru { get; set; }
        public int TinhTrang => !string.IsNullOrEmpty(ThuTuSapXepLuuTru) ? 1 : 0; // 1: đã sắp xếp, 0: chưa sắp xếp
        public string TinhTrangDisplay => TinhTrang == 1 ? "Đã sắp xếp" : "Chưa sắp xếp";
        public string SearchString { get; set; }
        //public string DoiTuong { get; set; }
        public bool? CoBHYT { get; set; }
        public int? MucHuong { get; set; }
        public string DoiTuong => CoBHYT != true ? "Viện phí" : "BHYT (" + MucHuong + "%)";
        public bool ChuaSapXep { get; set; }
        public bool DaSapXep { get; set; }
        #region cập nhật BVHD-3648
        public string NamSinh { get; set; }
        public DateTime? NgayVaoVien { get; set; }
        public DateTime? NgayRaVien { get; set; }
        public string NgayVaoVienDisplay => NgayVaoVien != null ? NgayVaoVien.Value.ApplyFormatDateTimeSACH() : "";
        public string NgayRaVienDisplay => NgayRaVien != null ? NgayRaVien.Value.ApplyFormatDateTimeSACH() : "";
        public string ICDChanDoanDieuTri { get; set; }
        public string NoiTruPhieuDieuTriIdOrChanDoanKemTheoIds { get; set; }
        public string ChanDoanICDChinhGhiChuOrDanhSachChanDoanKemTheoRaVienGhiChu { get; set; }
        
        public string TinhTrangRaVien { get; set; }
        public Enums.EnumKetQuaDieuTri? EnumKetQuaDieuTri { get; set; }
        public Enums.LoaiBenhAn LoaiBenhAn { get; set; }
        public NoiTruPhieuDieuTriInfoGridVo NoiTruPhieuDieuTriInfo => NoiTruPhieuDieuTriInfos?.OrderByDescending(c => c.NgayDieuTri).FirstOrDefault();
        public List<NoiTruPhieuDieuTriInfoGridVo> NoiTruPhieuDieuTriInfos { get; set; }
        public string SoLuuTru{ get; set; }
        public string ChuyenVien { get; set; }

        public string ThongTinRaVien { get; set; }
        public long? ChanDoanChinhRaVienICDId { get; set; }
        public string ChanDoanChinhRaVienGhiChu { get; set; }

        #endregion
        #region cập nhật thêm 3648
        public bool? CheckBHYT { get; set; }
        public bool? CheckVienPhi { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TuNgayText { get; set; }
        public string DenNgayText { get; set; }
        public long? KhoaPhongId { get; set; }
        #endregion
    }

    public class LuuTruHoSoExportExcel
    {
        [Width(30)]
        public string MaTN { get; set; }
        [Width(30)]
        public string SoBA { get; set; }
        [Width(20)]
        public string MaBN { get; set; }
        [Width(40)]
        public string HoTen { get; set; }
        [Width(20)]
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        [Width(20)]
        public string GioiTinhDisplay => GioiTinh?.GetDescription();
        [Width(20)]
        public string DoiTuong { get; set; }
        [Width(20)]
        public string KhoaNhapVien { get; set; }
        [Width(20)]
        public string TinhTrangDisplay { get; set; }
        [Width(20)]
        public int TinhTrang { get; set; }

    }

    public class ThongTiLuuTruBenhAnNoiTru : GridItem
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string Tuoi { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string SoBenhAn { get; set; }
        public string LoaiBenhAn { get; set; }
        public string Khoa { get; set; }
        public int? MucHuong { get; set; }
        public string DoiTuong => CoBHYT != true ? "Viện phí" : "BHYT (" + MucHuong + "%)";
        public bool? CoBHYT { get; set; }
        public string SoLuuTru { get; set; }
        public string ThuTuSapXepLuuTru { get; set; }
        public string NhanVienThucHien { get; set; }
        public string NgayThucHien { get; set; }
        public string TrangThaiSapXep => !string.IsNullOrEmpty(ThuTuSapXepLuuTru) ? "Đã sắp xếp" : "Chưa sắp xếp";

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public bool? CoBaoHiemTuNhan { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    public class CapNhatLuuTruBA
    {
        public long NoiTruBenhAnId { get; set; }
        public string ThuTuSapXepLuuTru { get; set; }
    }
    public class NoiTruPhieuDieuTriInfoGridVo
    {
        
        //public string NoiTruThamKhamChanDoanKemTheos { get; set; }
        public List<long> NoiTruThamKhamChanDoanKemTheoICDIds { get; set; }
        public long NoiTruBenhAnId { get; set; }
        public long? ChanDoanChinhICDId { get; set; }
        public string ChanDoanChinhGhiChu { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }
        public DateTime NgayDieuTri { get; set; }
    }
}
