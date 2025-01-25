using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Core.Domain.ValueObject
{
    //BACHA-448:Hiện tại cột huỷ hoàn đang để mặc định hình thức thanh toán là tiền mặt. Yêu cầu sửa lại: NB thanh toán hình thức nào thì số tiền huỷ hoàn sẽ ở cột đó
    public class BaoCaoThuPhiVienPhiGridVo : GridItem
    {
        public int STT { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr => NgayThu.ApplyFormatDateTimeSACH();
        public string SoBLHD { get; set; }
        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }
        public string MaYTe { get; set; }
        public string SoBenhAn { get; set; }
        public string NamSinh { get; set; }
        public string NguoiGioiThieu { get; set; }

        public string ChiTietCongNo => (ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? string.Join("; ", ChiTietCongNoTuNhans.Distinct()) : string.Empty)
                                       + (CongNoCaNhan.GetValueOrDefault() > 0 ? $"{(ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? "; " : string.Empty)}{TenBenhNhan}" : string.Empty);
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
        //public decimal? ChuyenKhoan => LaPhieuHuy == false && LoaiThuTienBenhNhan != Enums.LoaiThuTienBenhNhan.ThuNo
        //    ? SoTienThuChuyenKhoan.GetValueOrDefault()
        //    : (decimal?)null;
        //public decimal? Pos => LaPhieuHuy == false && LoaiThuTienBenhNhan != Enums.LoaiThuTienBenhNhan.ThuNo
        //    ? SoTienThuPos.GetValueOrDefault()
        //    : (decimal?)null;
        //will remove
        public decimal? Voucher { get; set; }
        public string LyDo { get; set; }
        public string NguoiThu { get; set; }
        public long? NguoiThuId { get; set; }
        public long? SoPhieuThuGhiNoId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        //public string StartDate { get; set; }
        //public string EndDate { get; set; }

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
        public bool BenhAnSoSinh { get; set; }
        public List<PhieuChiDataVo> PhieuChis { get; set; }
    }
    public class ChiTietBHYT
    {
        public double? SoLuong { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
    }

    public class PhieuChiDataVo
    {        
        public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        public decimal? TienChiPhi { get; set; }        
    }

    /* Logic cũ khi LaPhieuHuy = true luôn tính là tiền mặt
    public class BaoCaoThuPhiVienPhiGridVo : GridItem
    {
        public int STT { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr => NgayThu.ApplyFormatDateTimeSACH();
        public string SoBLHD { get; set; }
        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }
        public string MaYTe { get; set; }
        public string SoBenhAn { get; set; }
        public string NamSinh { get; set; }
        public string NguoiGioiThieu { get; set; }

        public string ChiTietCongNo => (ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? string.Join("; ", ChiTietCongNoTuNhans) : string.Empty)
                                       + (CongNoCaNhan.GetValueOrDefault() > 0 ? $"{(ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? "; " : string.Empty)}{TenBenhNhan}" : string.Empty);
        public string SoHoaDonChiTiet { get; set; }
        public bool GoiDichVu { get; set; }

        public decimal? TamUng => (LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng)
            ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
            : (decimal?) null;

        public decimal? HoanUng => LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng
            ? SoTienThuTienMat.GetValueOrDefault()
            : (decimal?) null;

        public decimal? SoTienThu => (LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi)
            ? (TongChiPhiBNTT.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault())
            : (LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault()) : (decimal?)null;

        public decimal? HuyThu => LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu ? SoTienThuTienMat.GetValueOrDefault()
                : (LaPhieuHuy == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
                : (LaPhieuHuy == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi) ? (TongChiPhiBNTT.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault())
                : (LaPhieuHuy == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
                : (decimal?)null;
        public decimal? ThucThu => TamUng.GetValueOrDefault() - HoanUng.GetValueOrDefault() + SoTienThu.GetValueOrDefault() - HuyThu.GetValueOrDefault() + ThuNoTienMat.GetValueOrDefault() + ThuNoChuyenKhoan.GetValueOrDefault() + ThuNoPos.GetValueOrDefault();
        public decimal? CongNo => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
            ? ((CongNoCaNhan.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault())*(LaPhieuHuy ? -1 : 1)) 
            : (decimal?)null;

        public decimal? TienMat => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?) null
            : (LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) ? (SoTienThuTienMat.GetValueOrDefault() * -1)
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuTienMat.GetValueOrDefault() : (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault()) * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (LaPhieuHuy == false ? (TongChiPhiBNTT.GetValueOrDefault() - CongNoCaNhan.GetValueOrDefault() - SoTienThuChuyenKhoan.GetValueOrDefault() - SoTienThuPos.GetValueOrDefault()) : (TongChiPhiBNTT.GetValueOrDefault() - CongNoCaNhan.GetValueOrDefault())*(-1)) 
            :(decimal?)null;
        
        public decimal? ChuyenKhoan => LaPhieuHuy == false && LoaiThuTienBenhNhan != Enums.LoaiThuTienBenhNhan.ThuNo
            ? SoTienThuChuyenKhoan.GetValueOrDefault()
            : (decimal?)null;
        public decimal? Pos => LaPhieuHuy == false && LoaiThuTienBenhNhan != Enums.LoaiThuTienBenhNhan.ThuNo
            ? SoTienThuPos.GetValueOrDefault()
            : (decimal?)null;
        //will remove
        public decimal? Voucher { get; set; }
        public string LyDo { get; set; }
        public string NguoiThu { get; set; }
        //public string StartDate { get; set; }
        //public string EndDate { get; set; }

        public Enums.LoaiThuTienBenhNhan LoaiThuTienBenhNhan { get; set; }
        public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        public bool LaPhieuHuy { get; set; }
        public decimal? ThuNoTienMat => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? LaPhieuHuy == false ? SoTienThuTienMat.GetValueOrDefault() : (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault()) * (-1)
            : (decimal?)null;
        public decimal? ThuNoChuyenKhoan => LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? SoTienThuChuyenKhoan.GetValueOrDefault()
            : (decimal?)null;
        public decimal? ThuNoPos => LaPhieuHuy == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? SoTienThuPos.GetValueOrDefault()
            : (decimal?)null;
        public string SoPhieuThuGhiNo { get; set; }
        public decimal? TongChiPhiBNTT { get; set; }
        public decimal? CongNoTuNhan { get; set; }
        public decimal? CongNoCaNhan { get; set; }
        public decimal? SoTienThuTamUng { get; set; }
        public decimal? SoTienThuTienMat { get; set; }
        public decimal? SoTienThuChuyenKhoan { get; set; }
        public decimal? SoTienThuPos { get; set; }
        public IEnumerable<string> ChiTietCongNoTuNhans { get; set; }
    }*/
    public class TotalBaoCaoThuPhiVienPhiGridVo : GridItem
    {
        public decimal? TamUng { get; set; }
        public decimal? HoanUng { get; set; }
        public decimal? SoTienThu { get; set; }
        public decimal? HuyThu { get; set; }
        public decimal? ThucThu => TamUng.GetValueOrDefault() - HoanUng.GetValueOrDefault() + SoTienThu.GetValueOrDefault() - HuyThu.GetValueOrDefault() + ThuNoTienMat.GetValueOrDefault() + ThuNoChuyenKhoan.GetValueOrDefault() + ThuNoPos.GetValueOrDefault();
        public decimal? CongNo { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }
        public decimal? ThuNoTienMat { get; set; }
        public decimal? ThuNoChuyenKhoan { get; set; }
        public decimal? ThuNoPos { get; set; }

        public decimal? DonGiaBenhVien { get; set; }

        public decimal? DonGiaMoi { get; set; }

        public decimal? SoTienDuocMienGiam { get; set; }

        public decimal? SoTienThucThu { get; set; }

    }
}
