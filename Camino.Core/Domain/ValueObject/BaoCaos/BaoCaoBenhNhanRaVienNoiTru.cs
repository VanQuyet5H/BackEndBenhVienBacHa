using System;
using System.Collections.Generic;
using System.ComponentModel;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{

    public class BaoCaoRaVienNoiTruQueryInfoVo : QueryInfo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        public string TimKiem { get; set; }
        public long KhoaId { get; set; }

        public LoaiYeuCauTiepNhanNoiTru? LoaiYeuCauTiepNhan { get; set; }
        public TrangThaiDieuTriNoiTru? TrangThaiDieuTri { get; set; }
    }

    public enum LoaiYeuCauTiepNhanNoiTru
    {          
        [Description("Nội trú")]
        KhamChuaBenhNoiTru = 1,
    }

    public enum TrangThaiDieuTriNoiTru
    {
        [Description("Tất cả")] 
        TatCa = 0,
        [Description("Đang điều trị")]
        DangDieuTri = 1,
        [Description("Đã ra viện")]
        DaRaVien = 2,
    }  

    public class DanhSachBangThongKeTiepNhanNoiTruQueryData
    {
        public long Id { get; set; }
        public long? YeuCauTiepNhanNgoaiTruCanQuyetToanId { get; set; }
        public string MaNB { get; set; }
        public string MaTN { get; set; }
        
        public DateTime ThoiGianTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public bool? CoBHYT { get; set; }
        public int? BHYTMucHuong { get; set; }
        public string BHYTMaSoThe { get; set; }
        public bool? QuyetToanTheoNoiTru { get; set; }
        public string LoaiYeuCauTiepNhan { get; set; }
        public string SoBenhAn { get; set; }
        public long? KhoaDieuTriId { get; set; }
        public string KhoaDieuTri { get; set; }
        public DateTime? ThoiGianNhapVien { get; set; }
        public long? ChanDoanNhapVienICDId { get; set; }
        public string ChanDoanNhapVienGhiChu { get; set; }
        public List<ChanDoanKemTheo> ChanDoanNhapVienKemTheos { get; set; }
        public List<KhoaPhongDieuTri> KhoaPhongDieuTris{ get; set; }
        public long? ChanDoanChinhRaVienICDId { get; set; }
        public string ChanDoanChinhRaVienGhiChu { get; set; }
        public string DanhSachChanDoanKemTheoRaVienICDId { get; set; }
        public string DanhSachChanDoanKemTheoRaVienGhiChu { get; set; }
        public DateTime? ThoiGianRaVien { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTri { get; set; }
        public List<TheBHYT> TheBHYTs { get; set; }
    }

    public class KhoaPhongDieuTriNoiTru
    {
        public string KhoaPhongChuyenDen { get; set; }
        public DateTime ThoiDiemVaoKhoa { get; set; }
    }

    public class ChanDoanKemTheoNoiTru
    {
        public long ChanDoanICDId { get; set; }
        public string GhiChu { get; set; }
    }

    public class TheBHYTNoiTru
    {
        public string MaSoThe { get; set; }
        public int MucHuong { get; set; }
    }

    public class DanhSachBangThongKeTiepNhanNoiTru : GridItem
    {
        public string MaNB { get; set; }
        public string MaTN { get; set; }
        public string SoBenhAn { get; set; }

        public DateTime ThoiGianTiepNhan { get; set; }
        public string ThoiGianTiepNhanStr => ThoiGianTiepNhan.ApplyFormatDateTimeSACH();

        public string HoTen { get; set; }
        public string NamSinh { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhStr => GioiTinh.GetDescription();

        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string KhoaDieuTri { get; set; }

        public DateTime? ThoiGianNhapVien { get; set; }
        public string ThoiGianNhapVienStr => ThoiGianNhapVien?.ApplyFormatDateTimeSACH();

        public string ChanDoanVaoVien { get; set; }
        public string KhoaChuyenDen { get; set; }
        public DateTime? ThoiGianChuyenKhoa { get; set; }
        public string ThoiGianChuyenKhoaStr { get; set; }

        public string ChanDoanRaVien { get; set; }
        public DateTime? ThoiGianRaVien { get; set; }
        public string ThoiGianRaVienStr => ThoiGianRaVien?.ApplyFormatDateTimeSACH();

        public string KetQuaDieuTri { get; set; }
        public string DoiTuong { get; set; }
        public string SoTheBHYT { get; set; }
        public string LoaiYeuCauTiepNhan { get; set; }
        public string TrangThaiDieuTri { get; set; }
    }
}
