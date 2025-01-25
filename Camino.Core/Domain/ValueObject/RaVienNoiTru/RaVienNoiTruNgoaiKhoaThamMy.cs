using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.RaVienNoiTru
{
    public class RaVienNoiTruNgoaiKhoaThamMy : CommonRaVien
    {
        public long YeuCauTiepNhanId { get; set; }

        public long? ChuanDoanNoiChuyenDenId { get; set; }
        public string TenChuanDoanNoiChuyenDen { get; set; }
        public string GhiChuChuanDoanNoiChuyenDen { get; set; }

        public long? ChuanDoanKKBCapCuuId { get; set; }
        public string TenChuanDoanKKBCapCuu { get; set; }
        public string GhiChuChuanDoanKKBCapCuu { get; set; }

        public long? NoiChuanDoanKhiVaoKhoaDieuTriId { get; set; }
        public string TenNoiChuanDoanKhiVaoKhoaDieuTri { get; set; }
        public string GhiChuNoiChuanDoanKhiVaoKhoaDieuTri { get; set; }

        public bool? TrieuChung { get; set; }
        public bool? DoPhauThuat { get; set; }
        public bool? DoGayMe { get; set; }
        public bool? DoNhiemKhau { get; set; }
        public bool? Khac { get; set; }

        public int? TongSoNgayDieuTriSauPT { get; set; }
        public int? TongSoLanPT { get; set; }

        public long? ChuanDoanRaVienId { get; set; }
        public string TenChuanDoanRaVien { get; set; }
        public string GhiChuChuanDoanRaVien { get; set; }
        public string NguyenNhan { get; set; }

        public List<ThongTinChuanDoanKemTheo> ChuanDoanKemTheos { get; set; }


        public long? ChuanDoanTruocPhauThuatId { get; set; }
        public string TenChuanDoanTruocPhauThuat { get; set; }
        public string GhiChuChuanDoanTruocPhauThuat { get; set; }

        public long? ChuanDoanSauPhauThuatId { get; set; }
        public string TenChuanDoanSauPhauThuat { get; set; }
        public string GhiChuChuanDoanSauPhauThuat { get; set; }
    }

    public class CommonRaVien
    {
        public Enums.EnumKetQuaDieuTri?  KetQuaDieuTriId { get; set; }
        public string TenKetQuaDieuTri { get; set; }

        public Enums.LoaiGiaPhauThuat? GiaPhauThuatId { get; set; }
        public string TenGiaPhauThuat { get; set; }
        public DateTime? ThoiGianRaVien { get; set; }

        public Enums.EnumHinhThucRaVien? HinhThucRaVienId { get; set; }
        public string TenHinhThucRaVien { get; set; }
        public DateTime? ThoiGianTuVong { get; set; }

        public Enums.LyDoTuVong? LyDoTuVongId { get; set; }
        public string TenLyDoTuVong { get; set; }
     
        public int? NguyenNhanChinhTuVongId { get; set; }
        public string TenNguyenNhanChinhTuVong { get; set; }

        public string ChiTietChuanDoanTuVong { get; set; }
        public bool? KhamNghiemTuThi { get; set; }

        public int? ChuanDoanTuThiId { get; set; }
        public string TenChuanDoanTuThi { get; set; }
        public string ChiTietChuanDoanTuThi { get; set; }

        public bool? HenTaiKham { get; set; }
        public DateTime? NgayHienTaiKham { get; set; }
        public string GhiChuTaiKham { get; set; }

        public int? TongSoNgayDieuTri { get; set; }
        public string NguoiSuaCuoiCung { get; set; }
        public DateTime NgaySuaCuoiCung { get; set; }
        public string NgaySuaCuoiCungDislay { get; set; }      

        public decimal? SoNgayDieuTriBenhAnSoSinh { get; set; }

        public Enums.LoaiChuyenTuyen? TuyenChuyenId { get; set; }
        public string TenTuyenChuyen { get; set; }

        public int? BenhVienId { get; set; }
        public string TenBenhVien { get; set; }
        public string TinhTrangBenhNhanLucChuyenVien { get; set; }

        public Enums.DieuKienDuChuyenTuyen? LyDoChuyenTuyenId { get; set; }
        public string TenLyDoChuyenTuyen { get; set; }
        public DateTime? ThoiGianChuyenVien { get; set; }

        public int? NguoiHoTongId { get; set; }
        public string TenNguoiHoTong { get; set; }

        public int? ChucVuNguoiHoTongId { get; set; }
        public string TenChucVuNguoiHoTong { get; set; }

        public int? TrinhDoNguoiHoTongId { get; set; }
        public string TenTrinhDoNguoiHoTong { get; set; }

        public string PhuongTienNguoiHoTong { get; set; }

        public Enums.LoaiBenhAn LoaiBenhAn { get; set; }
    }
}
