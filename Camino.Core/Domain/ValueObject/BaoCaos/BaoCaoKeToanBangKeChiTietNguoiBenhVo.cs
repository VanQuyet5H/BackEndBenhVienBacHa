using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoKeToanBangKeChiTietNguoiBenhTimKiemNangCaoVo : QueryInfo
    {
        public long? HinhThucDenId { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string SearchString { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime? TuNgayFormat { get; set; }
        public DateTime? DenNgayFormat { get; set; }
        public bool? LaNguoiBenhNgoaiTru { get; set; }
    }

    public class BaoCaoKeToanBangKeChiTietNguoiBenhGridVo : GridItem
    {
        public BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
        {
            PhuongPhapTinhGiaTriTonKhos = new List<Enums.PhuongPhapTinhGiaTriTonKho>();
            ChiPhiBHYTDichVuGiuongs = new List<ThongTinChiPhiBHYTDichVuVo>();
        }
        public int? STT { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long YeucauTiepNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay => ThoiDiemTiepNhan?.ApplyFormatDate();
        public Enums.EnumTrangThaiYeuCauTiepNhan TrangThaiYeuCauTiepNhan { get; set; }
        public string NoiGioiThieuDisplay { get; set; }
        public long BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        //public int? NgaySinh { get; set; }
        //public int? ThangSinh { get; set; }
        //public int? NamSinh { get; set; }
        //public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string NoiDung { get; set; }
        public Enums.EnumNhomGoiDichVu Nhom { get; set; }
        public string TenNhom { get; set; }
        public string NhomDichVu
        {
            get
            {
                switch (Nhom)
                {
                    case Enums.EnumNhomGoiDichVu.DichVuKhamBenh:
                        return "Khám";
                    case Enums.EnumNhomGoiDichVu.DichVuKyThuat:
                        return TenNhom;
                    case Enums.EnumNhomGoiDichVu.DichVuGiuongBenh:
                        return "Giường";
                    case Enums.EnumNhomGoiDichVu.DonThuocThanhToan:
                        return "Thuốc";
                    case Enums.EnumNhomGoiDichVu.DuocPham:
                        return "Thuốc";
                    case Enums.EnumNhomGoiDichVu.VatTuTieuHao:
                        return "Vật tư y tế";
                    case Enums.EnumNhomGoiDichVu.TruyenMau:
                        return "Truyền máu";
                    default:
                        return "Khác";
                }
            }
        }

        public string DonViTinh { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGiaNhapKho { get; set; }
        public decimal? DonGiaBan { get; set; }
        public decimal? MienGiam { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public decimal DonGiaBHYT { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public int MucHuongBaoHiem { get; set; }
        public decimal DonGiaBHYTThanhToan => DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100;
        public decimal BHYTThanhToan => (DuocHuongBHYT && BaoHiemChiTra == true) ? (decimal)(SoLuong * (double)DonGiaBHYTThanhToan) : 0;

        public long? XuatKhoChiTietId { get; set; }
        public bool LaTuTinhGiaBan { get; set; }
        public int? VAT { get; set; }
        public int? TiLeTheoThapGia { get; set; }
        public Enums.PhuongPhapTinhGiaTriTonKho PhuongPhapTinhGiaTriTonKho => PhuongPhapTinhGiaTriTonKhos.Any() ? PhuongPhapTinhGiaTriTonKhos.First() : Enums.PhuongPhapTinhGiaTriTonKho.ApVAT;
        public decimal? GiaTonKho => DonGiaNhapKho.GetValueOrDefault() + (DonGiaNhapKho.GetValueOrDefault() * (PhuongPhapTinhGiaTriTonKho == Enums.PhuongPhapTinhGiaTriTonKho.ApVAT ? VAT.GetValueOrDefault() : 0) / 100);
        public decimal? DonGiaBanThuocVatTu => Math.Round(GiaTonKho.GetValueOrDefault() + (GiaTonKho.GetValueOrDefault() * TiLeTheoThapGia.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
        public decimal? GiaBanThuocVatTu => Math.Round((decimal)SoLuong * DonGiaBan.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
        public List<Enums.PhuongPhapTinhGiaTriTonKho> PhuongPhapTinhGiaTriTonKhos { get; set; }

        public decimal? DonGiaBanThucTe
        {
            get
            {
                decimal? giaBan = null;
                if (LaTuTinhGiaBan)
                {
                    giaBan = DonGiaBanThuocVatTu;
                }
                else
                {
                    giaBan = DonGiaBan;
                }

                if (giaBan == null)
                    return null;
                return giaBan.GetValueOrDefault() - ((DuocHuongBHYT && BaoHiemChiTra == true) ? DonGiaBHYTThanhToan : 0);
            }
        }

        public decimal? ThanhTienBan => (DonGiaBanThucTe.GetValueOrDefault() * (decimal)SoLuong.GetValueOrDefault()) - MienGiam.GetValueOrDefault();


        public string TenHinhThucDen { get; set; }
        public bool? LaGioiThieu { get; set; }
        public string HinhThucDenDisplay => LaGioiThieu == true
            ? (TenHinhThucDen + ((!string.IsNullOrEmpty(TenHinhThucDen) && !string.IsNullOrEmpty(NoiGioiThieuDisplay)) ? "/ " : "") + NoiGioiThieuDisplay)
            : TenHinhThucDen;

        public bool? LaDataTheoDieuKienTimKiem { get; set; }

        // get thông tin chi phí BHYT cho trường hợp dịch vụ giường
        public List<ThongTinChiPhiBHYTDichVuVo> ChiPhiBHYTDichVuGiuongs { get; set; }
    }

    public class BaoCaoKeToanBangKeChiTietNguoiBenhTongTienVo
    {
        public decimal TongTienDonGiaNhapKho { get; set; }
        public decimal TongTienDonGiaBan { get; set; }
        public decimal TongTienMienGiam { get; set; }
        public decimal TongTienThanhTienBan { get; set; }
    }
}
