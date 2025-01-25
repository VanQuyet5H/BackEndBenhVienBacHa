using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoDichVuTiaPlasMaHoTroDeuTriVetThuongQueryInfoVo : QueryInfo
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }

        public long? DichVuId { get; set; }
        public Enums.EnumNhomGoiDichVu? NhomDichVu { get; set; }
        public string TenDichVu { get; set; }
        public long? KhoaId { get; set; }
        public string SearchString { get; set; }
    }

    public class DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong : GridItem
    {
        public string MaNB { get; set; }
        public string MaTN { get; set; }
        public string SoBenhAn { get; set; }
        public long? NoiTruBenhAnId { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhStr => GioiTinh.GetDescription();

        public string NoiDungThucHien { get; set; }

        public string ChanDoanRaVien { get; set; }
        public string ChanDoanTheoPhieuDieuTriCuoiCung { get; set; }
        public string ChanDoanNhapVien { get; set; }
        public string ChanDoanICDChinh { get; set; }
        public string ChanDoanSoBo { get; set; }
        public string ChanDoan => ChanDoanRaVien ?? ChanDoanTheoPhieuDieuTriCuoiCung ?? ChanDoanNhapVien ?? ChanDoanICDChinh ?? ChanDoanSoBo;

        public long SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal? SoTienMienGiamTheoDichVu { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal? SoTienMienGiam => KhongTinhPhi == true ? (DonGia * SoLuong) : SoTienMienGiamTheoDichVu;

        public bool DuocHuongBHYT { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public decimal? DonGiaBHYT { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public decimal? DonGiaBHYTThanhToan => DuocHuongBHYT ? (DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100) : (decimal?)null;
        public decimal? BHYTThanhToan => DuocHuongBHYT ? (decimal)(SoLuong * (double)DonGiaBHYTThanhToan.GetValueOrDefault()) : (decimal?)null;
        public decimal ThanhToan => KhongTinhPhi == true ? 0 : ((DonGia * SoLuong) - SoTienMienGiam.GetValueOrDefault() - BHYTThanhToan.GetValueOrDefault());

        public string NoiChiDinh { get; set; }
        public string NoiThucHien { get; set; }
        public string NguoiChiDinh { get; set; }

        public DateTime? NgayChiDinh { get; set; }
        public string NgayChiDinhStr => NgayChiDinh?.ApplyFormatDateTimeSACH();

        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienStr => NgayThucHien?.ApplyFormatDateTimeSACH();

        public long TongCongSoLuong { get; set; }
        public decimal TongCongThanhToan { get; set; }
    }

    public class LookupItemTongHopDichVuVo : LookupItemTemplateVo
    {
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
    }

    public class ChanDoanIcdTheoNgayDieuTriVo
    {
        public long NoiTruBenhAnId { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string ChanDoanIcd { get; set; }
    }
}
