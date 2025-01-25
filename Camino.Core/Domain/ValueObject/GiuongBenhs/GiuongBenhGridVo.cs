using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.GiuongBenhs
{
    public class GiuongBenhGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string Khoa { get; set; }
        public long KhoaId { get; set; }
        public string Phong { get; set; }
        public long PhongId { get; set; }
        public string MoTa { get; set; }
        public bool? CoHieuLuc { get; set; }
        public bool? LaGiuongNoi { get; set; }
    }

    public class GiuongBenhSearchHeader
    {
        public long? KhoaId { get; set; }
        public long? PhongId { get; set; }
        public string SearchValue { get; set; }
    }

    #region So do giuong benh

    public class SoDoGiuongBenhKhoaGridVo : GridItem
    {
        public int STT { get; set; }
        public string Ten { get; set; }
        public int GiuongTrong { get; set; }
        public int GiuongCoBenhNhan { get; set; }
        public int TongGiuongBenhCuaKhoa { get; set; }
        public int SoGiuongGhep { get; set; }

        public int TongSoGiuongGhep { get; set; }
        public int TongSoGiuongTrong { get; set; }
        public int TongSoGiuongCoBenhNhan { get; set; }
        public int TongSoTongGiuongBenhCuaKhoa { get; set; }
        //public long KhoaId { get; set; }
    }

    public class SoDoGiuongBenhKhoaPhongGridVo : GridItem
    {
        public SoDoGiuongBenhKhoaPhongGridVo()
        {
            lstBenhNhanGiuong = new List<LstBenhNhanGiuong>();
            lstYCTN = new List<long>();
        }
        public int STT { get; set; }
        public string TenGiuong { get; set; }
        public string DonGiaDisplay { get; set; }
        public List<LstBenhNhanGiuong> lstBenhNhanGiuong { get; set; }
        public List<long> lstYCTN{ get; set; }

        public int TongSoGiuong { get; set; }
        public int TongSoGiuongTrong { get; set; }
        public int TongSoGiuongCoBenhNhan { get; set; }
        public int TongSoGiuongNamGhep { get; set; }
    }

    public class LstBenhNhanGiuong
    {
        public string HoVaTen { get; set; }
        public string KhoaDieuTri { get; set; }
        public string Phong { get; set; }
        public string MaBenhNhan { get; set; }
        public string NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string TinhTrangBenh { get; set; }
        public string NgayVaoVien { get; set; }
        public string NgayRaVien { get; set; }
    }

    public class SoDoGiuongBenhSearchHeader
    {
        public long? KhoaId { get; set; }
        public long? PhongId { get; set; }
    }

    public class ResultSoDoPopup
    {
        public ResultSoDoPopup()
        {
            lstPhong = new List<LstPhong>();
        }
        public long? PhongId { get; set; }
        public long? KhoaId { get; set; }
        public string TenKhoa { get; set; }
        //
        public bool? GiuongTrong { get; set; }
        public bool? GiuongDaCoBenhNhan { get; set; }
        //
        public List<LstPhong> lstPhong { get; set; }
    }

    public class LstPhong
    {
        public LstPhong()
        {
            lstGiuong = new List<LstGiuong>();
        }
        public List<LstGiuong> lstGiuong { get; set; }
        public long? PhongId { get; set; }
        public string DisplayName { get; set; }

    }

    public class LstGiuong
    {
        public string TenGiuong { get; set; }
        public string SoBenhNhan { get; set; }
        public bool IsGiuongTrong { get; set; }
    }
    #endregion So do giuong benh
}