using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{

    public class ThongKeCacDichVuChuaLayLenBienLaiThuTienQueryInfoVo : QueryInfo
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }
        public string SearchString { get; set; }
    }


    public class DanhSachThongKeCacDichVuChuaLayLenBienLaiThuTien : GridItem
    {
        public string MaBenhAn { get; set; }
        public string MaBN { get; set; }
        public string MaTN { get; set; }
        public string HoVaTen { get; set; }
        public int? NamSinh { get; set; }
        public string HinhThucDen { get; set; }
        public string DiaChi { get; set; }
        public string Loai { get; set; }
        public bool TrongGoi { get; set; }
        public decimal TongTienChuaThanhToan { get; set; }
    }

    public class ThongTinDichVuChuaThuTienVo
    {
        public long YeucauTiepNhanId { get; set; }
        public string MaYeucauTiepNhan { get; set; }
        public string TenDichVu { get; set; }

        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal? GiaBan { get; set; }
        public decimal? SoTienMienGiamTheoDichVu { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal? SoTienMienGiam => KhongTinhPhi == true ? (GiaBan ?? (decimal)((double)DonGia * SoLuong)) : SoTienMienGiamTheoDichVu;

        public bool DuocHuongBHYT { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public decimal? DonGiaBHYT { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public decimal? DonGiaBHYTThanhToan => DuocHuongBHYT ? (DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100) : (decimal?)null;
        public decimal? BHYTThanhToan => DuocHuongBHYT ? (decimal)(SoLuong * (double)DonGiaBHYTThanhToan.GetValueOrDefault()) : (decimal?)null;
        public decimal ThanhToan => KhongTinhPhi == true ? 0 : ((GiaBan ?? (decimal)((double)DonGia * SoLuong)) - SoTienMienGiam.GetValueOrDefault() - BHYTThanhToan.GetValueOrDefault());

        public bool LaTrongGoi { get; set; }
        public long? YeuCauDichVuGiuongChiPhiBenhVienId { get; set; }

        #region //25/07/2022: Cập nhật cách tính giá bán dược phẩm và vật tư
        //public bool? KhongTinhPhi { get; set; }
        public long? XuatKhoChiTietId { get; set; }
        //public bool LaTuTinhGiaBan { get; set; }
        //public decimal? SoTienMienGiam { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public int? VAT { get; set; }
        public int? TiLeTheoThapGia { get; set; }
        public Enums.PhuongPhapTinhGiaTriTonKho PhuongPhapTinhGiaTriTonKho => PhuongPhapTinhGiaTriTonKhos.Any() ? PhuongPhapTinhGiaTriTonKhos.First() : Enums.PhuongPhapTinhGiaTriTonKho.ApVAT;
        public decimal? GiaTonKho => DonGiaNhap.GetValueOrDefault() + (DonGiaNhap.GetValueOrDefault() * (PhuongPhapTinhGiaTriTonKho == Enums.PhuongPhapTinhGiaTriTonKho.ApVAT ? VAT.GetValueOrDefault() : 0) / 100);
        public decimal? DonGiaBan => Math.Round(GiaTonKho.GetValueOrDefault() + (GiaTonKho.GetValueOrDefault() * TiLeTheoThapGia.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
        public decimal? GiaBanDuocPhamVatTu => Math.Round((decimal)SoLuong * DonGiaBan.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
        public List<Enums.PhuongPhapTinhGiaTriTonKho> PhuongPhapTinhGiaTriTonKhos { get; set; }
        public Enums.PhuongPhapTinhGiaTriTonKho? PhuongPhapTinhGiaTriTonKhoDonThuoc { get; set; }
        #endregion
    }

    public class ThongTinTiepNhanChuaThuTienCoHinhThucDenVo : GridItem
    {
        public string MaBenhAn { get; set; }
        public string MaBN { get; set; }
        public string HoVaTen { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }
        public string Loai => LaTiepNhanNoiTru ? "Nội trú" : "Ngoại trú";
        public bool TrongGoi { get; set; }
        public decimal TongTienChuaThanhToan { get; set; }

        public long YeucauTiepNhanId { get; set; }
        public string MaYeucauTiepNhan { get; set; }
        public long BenhNhanId { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }

        public string NoiGioiThieuDisplay { get; set; }
        public string TenHinhThucDen { get; set; }
        public bool? LaGioiThieu { get; set; }
        public string HinhThucDenDisplay => LaGioiThieu == true
            ? (TenHinhThucDen + ((!string.IsNullOrEmpty(TenHinhThucDen) && !string.IsNullOrEmpty(NoiGioiThieuDisplay)) ? "/ " : "") + NoiGioiThieuDisplay)
            : TenHinhThucDen;

        public bool? LaDataTheoDieuKienTimKiem { get; set; }
        public bool LaTiepNhanNoiTru { get; set; }
        public bool LaBenhAnSoSinh { get; set; }

        #region clone
        public ThongTinTiepNhanChuaThuTienCoHinhThucDenVo Clone()
        {
            return (ThongTinTiepNhanChuaThuTienCoHinhThucDenVo)this.MemberwiseClone();
        }
        #endregion

        // Cập nhật 25/07/2022: thay thế tính tổng
        public decimal TongTienChuaThanhToanHienThiGrid { get; set; }
    }
}
