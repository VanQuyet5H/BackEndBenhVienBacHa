using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.RaVienNoiTru
{
    public class RaVien : CommonRaVien
    {
        //phukhoa
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


        public long? NoiChuanDoanLucVaoDeId { get; set; }
        public string TenNoiChuanDoanLucVaoDe { get; set; }
        public string GhiChuNoiChuanDoanLucVaoDe { get; set; }

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

        //khoanoi va kho nhi      
        public bool? DoThuThuat { get; set; }

        //ngoaikha va thâm mỹ


        //san khoa mo      

        public DateTime? NgayDe { get; set; }
        public string NgoiThai { get; set; }

        public string CachThucDe { get; set; }
        public string KiemXoatTuCung { get; set; }

        public bool? TaiBien { get; set; }
        public bool? BienChung { get; set; }

        public bool? TinhTrangCapCuuChuDong { get; set; }
        public bool? CapCuu { get; set; }
        public bool? ChuDong { get; set; }
        public string PhuongPhapPhauThuat { get; set; }


        public List<TreSoSinh> DanhSachTreSoSinhs { get; set; }


        //câp nhât 29/01/2021
        public bool? ChonThai { get; set; }
        public bool? ChonTraiGai { get; set; }
        public bool? ChonSongChet { get; set; }

        public bool? ChonTrai { get; set; }
        public int? SoLuongTrai { get; set; }
        public bool? ChonTraiSong { get; set; }
        public int? SoLuongTraiSong { get; set; }
        public bool? ChonTraiChet { get; set; }
        public int? SoLuongTraiChet { get; set; }

        public bool? ChonGai { get; set; }
        public int? SoLuongGai { get; set; }
        public bool? ChonGaiSong { get; set; }
        public int? SoLuongGaiSong { get; set; }
        public bool? ChonGaiChet { get; set; }
        public int? SoLuongGaiChet { get; set; }

        public string DiTatThai { get; set; }
        public decimal? CanNang { get; set; }
        //san khoa thuong      
    }

    public class KiemTraThongTinKetThucBenhAnError
    {
        public string NgayDieuTri { get; set; }
        public string Loai { get; set; }
        public string TenDichVu { get; set; }
        public string KhoaChiDinh { get; set; }
        public string NoiDung { get; set; }

    }
}
