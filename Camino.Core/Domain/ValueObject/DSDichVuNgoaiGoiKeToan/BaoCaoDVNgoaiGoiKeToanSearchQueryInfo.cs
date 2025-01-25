using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DSDichVuNgoaiGoiKeToan
{
    
    public class BaoCaoDVNgoaiGoiKeToanSearchQueryInfo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }

    public class BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo : QueryInfo
    {
        public long? CongTyId { get; set; }
        public long? HopDongId { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TuNgayUTC { get; set; }
        public string DenNgayUTC { get; set; }
    }
    public class BaoCaoDVNgoaiGoiKeToanGridVo : GridItem
    {
        public BaoCaoDVNgoaiGoiKeToanGridVo()
        {
            DataPhieuChis = new List<BaoCaoDVNgoaiGoiKeDataPhieuChi>();
        }
        public int STT { get; set; }
        public DateTime NgayBienLai { get; set; }
        public string NgayBienLaiStr => NgayBienLai.ApplyFormatDateTimeSACH();
        public string SoBienLai { get; set; }
        public string SoBienLaiRemoveSpecial => !string.IsNullOrEmpty(SoBienLai) ? MaskHelper.RemoveSpecialCharacters(SoBienLai).Replace(".", "") : "";
        public string MaNhanVien { get; set; }
        public string MaNguoiBenh { get; set; }
        public string MaTiepNhan { get; set; }
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        

        public List<DichVuKyThuatInFo> TenDichVus { get; set; }
      
       
       
       

        public string NguoiGioiThieu { get; set; }

        public string ChiTietCongNo => (ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? string.Join("; ", ChiTietCongNoTuNhans.Distinct()) : string.Empty)
                                       + (CongNoCaNhan.GetValueOrDefault() > 0 ? $"{(ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? "; " : string.Empty)}{HoTen}" : string.Empty);
        public string SoHoaDonChiTiet { get; set; }
        public bool GoiDichVu { get; set; }

        public decimal? TamUng => (LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng)
            ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
            : (decimal?)null;

        public decimal? HoanUng => (LaPhieuHuy == false && LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng)
            ? SoTienThuTienMat.GetValueOrDefault()
            : (decimal?)null;

        public decimal? SoTienThu => (LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi)
            ? (TongChiPhiBNTT.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault())
            : (LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault()) : (decimal?)null;

        public decimal? HuyThu => LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu ? (SoTienThuTienMat.GetValueOrDefault() * (LaPhieuHuy ? -1 : 1))
                : (LaPhieuHuy == true && LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng) ? SoTienThuTienMat.GetValueOrDefault()
                : (LaPhieuHuy == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
                : (LaPhieuHuy == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi) ? (TongChiPhiBNTT.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault())
                : (LaPhieuHuy == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
                : (decimal?)null;
        public decimal? ThucThu => TamUng.GetValueOrDefault() - HoanUng.GetValueOrDefault() + SoTienThu.GetValueOrDefault() - HuyThu.GetValueOrDefault() + ThuNoTienMat.GetValueOrDefault() + ThuNoChuyenKhoan.GetValueOrDefault() + ThuNoPos.GetValueOrDefault();
        public decimal? CongNo => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
            ? ((CongNoCaNhan.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault()) * (LaPhieuHuy ? -1 : 1))
            : (decimal?)null;

        public decimal? TienMat => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?)null
            : (LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) ? (SoTienThuTienMat.GetValueOrDefault() * -1 * (LaPhieuHuy ? -1 : 1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuTienMat.GetValueOrDefault() : SoTienThuTienMat.GetValueOrDefault() * (-1))
            //: LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuTienMat.GetValueOrDefault() : (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault()) * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (LaPhieuHuy == false ? (TongChiPhiBNTT.GetValueOrDefault() - CongNoCaNhan.GetValueOrDefault() - SoTienThuChuyenKhoan.GetValueOrDefault() - SoTienThuPos.GetValueOrDefault()) : (TongChiPhiBNTT.GetValueOrDefault() - CongNoCaNhan.GetValueOrDefault() - SoTienThuChuyenKhoan.GetValueOrDefault() - SoTienThuPos.GetValueOrDefault()) * (-1))
            : (decimal?)null;

        public decimal? ChuyenKhoan => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?)null
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuChuyenKhoan.GetValueOrDefault() : SoTienThuChuyenKhoan.GetValueOrDefault() * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (LaPhieuHuy == false ? SoTienThuChuyenKhoan.GetValueOrDefault() : SoTienThuChuyenKhoan.GetValueOrDefault() * (-1))
            : (decimal?)null;
        public decimal? Pos => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?)null
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuPos.GetValueOrDefault() : SoTienThuPos.GetValueOrDefault() * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (LaPhieuHuy == false ? SoTienThuPos.GetValueOrDefault() : SoTienThuPos.GetValueOrDefault() * (-1))
            : (decimal?)null;

        public decimal? Voucher { get; set; }
        public string LyDo { get; set; }
      

        public Enums.LoaiThuTienBenhNhan LoaiThuTienBenhNhan { get; set; }
        public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        public bool LaPhieuHuy { get; set; }
        public decimal? ThuNoTienMat => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? LaPhieuHuy == false ? SoTienThuTienMat.GetValueOrDefault() : SoTienThuTienMat.GetValueOrDefault() * (-1)
            : (decimal?)null;
        public decimal? ThuNoChuyenKhoan => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? LaPhieuHuy == false ? SoTienThuChuyenKhoan.GetValueOrDefault() : SoTienThuChuyenKhoan.GetValueOrDefault() * (-1)
            : (decimal?)null;
        //? SoTienThuChuyenKhoan.GetValueOrDefault()
        //: (decimal?)null;
        public decimal? ThuNoPos => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? LaPhieuHuy == false ? SoTienThuPos.GetValueOrDefault() : SoTienThuPos.GetValueOrDefault() * (-1)
            : (decimal?)null;
        //? SoTienThuPos.GetValueOrDefault()
        //: (decimal?)null;
        public string SoPhieuThuGhiNo { get; set; }
        public decimal? TongChiPhiBNTT { get; set; }
        public decimal? CongNoTuNhan { get; set; }
        public decimal? CongNoCaNhan { get; set; }
        public decimal? SoTienThuTamUng { get; set; }
        public decimal? SoTienThuTienMat { get; set; }
        public decimal? SoTienThuChuyenKhoan { get; set; }
        public decimal? SoTienThuPos { get; set; }
        public IEnumerable<string> ChiTietCongNoTuNhans { get; set; }
        public bool CoTiemChung { get; set; }
        public List<ChiTietBHYT> ChiTietBHYTs { get; set; }
        public decimal? BHYT { get; set; }

        public string SoHoaDon { get; set; }

        public List<BaoCaoDVNgoaiGoiKeDataPhieuChi> DataPhieuChis { get; set; }
    }
    public class BaoCaoDVNgoaiGoiKeDataPhieuChi
    {
        public long Id { get; set; }

        public DateTime NgayChi { get; set; }

        public decimal? TienChiPhi { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? DonThuocThanhToanChiTietId { get; set; }

        public long? YeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }
        public long? DonVTYTThanhToanChiTietId { get; set; }
    }
    public class BaoCaoDVNgoaiGoiDataDichVu
    {
        public long Id { get; set; }
        public string TenDichVu { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long DichVuNhomGiaId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public decimal Gia { get; set; }
        public decimal? DonGiaUuDai { get; set; }
        public decimal? DonGiaChuaUuDai { get; set; }
        public bool YeuCauKhamBenh { get; set; }
        public bool YeuCauDichVuKyThuat { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public bool YeuCauGiuong { get; set; }
        public bool YeuCauDuocPham { get; set; }
        public bool DonThuocBHYT { get; set; }
        public bool YeuCauVatTu { get; set; }
        public bool YeuCauTruyenMau { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? NoiThucHienKhamBenhId { get; set; }
        public long? NoiThucHienPTTTId { get; set; }
        public long? NoiChiDinhId { get; set; }
    }
    public class DichVuKyThuatInFo : GridItem
    {
        public string TenDichVu { get; set; }
        public decimal? DonGiaBenhVien { get; set; }

        public decimal? DonGiaMoi { get; set; }

        public decimal? SoTienDuocMienGiam { get; set; }

        public decimal? SoTienThucThu { get; set; }
    }

    public class BaoCaoDVNgoaiGoiKeToanExelGridVo : GridItem
    {
        public int STT { get; set; }
        public DateTime NgayBienLai { get; set; }
        public string NgayBienLaiStr => NgayBienLai.ApplyFormatDateTimeSACH();
        public string SoBienLai { get; set; }
        public string SoBienLaiRemoveSpecial { get; set; }
        public string MaNhanVien { get; set; }
        public string MaNguoiBenh { get; set; }
        public string MaTiepNhan { get; set; }
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }


        public List<DichVuKyThuatInFo> TenDichVus { get; set; }





        public string NguoiGioiThieu { get; set; }

        public string ChiTietCongNo => (ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? string.Join("; ", ChiTietCongNoTuNhans.Distinct()) : string.Empty)
                                       + (CongNoCaNhan.GetValueOrDefault() > 0 ? $"{(ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? "; " : string.Empty)}{HoTen}" : string.Empty);
        public string SoHoaDonChiTiet { get; set; }
        public bool GoiDichVu { get; set; }

        public decimal? TamUng => (LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng)
            ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
            : (decimal?)null;

        public decimal? HoanUng => (LaPhieuHuy == false && LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng)
            ? SoTienThuTienMat.GetValueOrDefault()
            : (decimal?)null;

        public decimal? SoTienThu => (LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi)
            ? (TongChiPhiBNTT.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault())
            : (LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault()) : (decimal?)null;

        public decimal? HuyThu => LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu ? (SoTienThuTienMat.GetValueOrDefault() * (LaPhieuHuy ? -1 : 1))
                : (LaPhieuHuy == true && LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng) ? SoTienThuTienMat.GetValueOrDefault()
                : (LaPhieuHuy == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
                : (LaPhieuHuy == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi) ? (TongChiPhiBNTT.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault())
                : (LaPhieuHuy == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
                : (decimal?)null;
        public decimal? ThucThu => TamUng.GetValueOrDefault() - HoanUng.GetValueOrDefault() + SoTienThu.GetValueOrDefault() - HuyThu.GetValueOrDefault() + ThuNoTienMat.GetValueOrDefault() + ThuNoChuyenKhoan.GetValueOrDefault() + ThuNoPos.GetValueOrDefault();
        public decimal? CongNo => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
            ? ((CongNoCaNhan.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault()) * (LaPhieuHuy ? -1 : 1))
            : (decimal?)null;

        public decimal? TienMat => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?)null
            : (LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) ? (SoTienThuTienMat.GetValueOrDefault() * -1 * (LaPhieuHuy ? -1 : 1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuTienMat.GetValueOrDefault() : SoTienThuTienMat.GetValueOrDefault() * (-1))
            //: LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuTienMat.GetValueOrDefault() : (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault()) * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (LaPhieuHuy == false ? (TongChiPhiBNTT.GetValueOrDefault() - CongNoCaNhan.GetValueOrDefault() - SoTienThuChuyenKhoan.GetValueOrDefault() - SoTienThuPos.GetValueOrDefault()) : (TongChiPhiBNTT.GetValueOrDefault() - CongNoCaNhan.GetValueOrDefault() - SoTienThuChuyenKhoan.GetValueOrDefault() - SoTienThuPos.GetValueOrDefault()) * (-1))
            : (decimal?)null;

        public decimal? ChuyenKhoan => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?)null
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuChuyenKhoan.GetValueOrDefault() : SoTienThuChuyenKhoan.GetValueOrDefault() * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (LaPhieuHuy == false ? SoTienThuChuyenKhoan.GetValueOrDefault() : SoTienThuChuyenKhoan.GetValueOrDefault() * (-1))
            : (decimal?)null;
        public decimal? Pos => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?)null
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuPos.GetValueOrDefault() : SoTienThuPos.GetValueOrDefault() * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (LaPhieuHuy == false ? SoTienThuPos.GetValueOrDefault() : SoTienThuPos.GetValueOrDefault() * (-1))
            : (decimal?)null;

        public decimal? Voucher { get; set; }
        public string LyDo { get; set; }


        public Enums.LoaiThuTienBenhNhan LoaiThuTienBenhNhan { get; set; }
        public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        public bool LaPhieuHuy { get; set; }
        public decimal? ThuNoTienMat => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? LaPhieuHuy == false ? SoTienThuTienMat.GetValueOrDefault() : SoTienThuTienMat.GetValueOrDefault() * (-1)
            : (decimal?)null;
        public decimal? ThuNoChuyenKhoan => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? LaPhieuHuy == false ? SoTienThuChuyenKhoan.GetValueOrDefault() : SoTienThuChuyenKhoan.GetValueOrDefault() * (-1)
            : (decimal?)null;
        //? SoTienThuChuyenKhoan.GetValueOrDefault()
        //: (decimal?)null;
        public decimal? ThuNoPos => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? LaPhieuHuy == false ? SoTienThuPos.GetValueOrDefault() : SoTienThuPos.GetValueOrDefault() * (-1)
            : (decimal?)null;
        //? SoTienThuPos.GetValueOrDefault()
        //: (decimal?)null;
        public string SoPhieuThuGhiNo { get; set; }
        public decimal? TongChiPhiBNTT { get; set; }
        public decimal? CongNoTuNhan { get; set; }
        public decimal? CongNoCaNhan { get; set; }
        public decimal? SoTienThuTamUng { get; set; }
        public decimal? SoTienThuTienMat { get; set; }
        public decimal? SoTienThuChuyenKhoan { get; set; }
        public decimal? SoTienThuPos { get; set; }
        public IEnumerable<string> ChiTietCongNoTuNhans { get; set; }
        public bool CoTiemChung { get; set; }
        public List<ChiTietBHYT> ChiTietBHYTs { get; set; }
        public decimal? BHYT { get; set; }

        public string SoHoaDon { get; set; }

        public List<BaoCaoDVNgoaiGoiKeDataPhieuChi> DataPhieuChis { get; set; }
        public string TenDichVu { get; set; }
        public decimal? DonGiaBenhVien { get; set; }

        public decimal? DonGiaMoi { get; set; }

        public decimal? SoTienDuocMienGiam { get; set; }

        public decimal? SoTienThucThu { get; set; }
    }
}
