using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.ComponentModel;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DanhSachDeNghiThanhToanChiPhiKCB
{
    public class DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        public MauBaoBaoBHYT MauBaoBaoBHYT { get; set; }
        public HinhThucXuatBaoBao HinhThucXuatBaoBao { get; set; }
    }

    public class DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru : GridItem
    {
        public NoiDangKyKCBBanDau NoiDangKyKCBBanDau { get; set; }
        public string NoiDangKyKCBBanDauDisplay => NoiDangKyKCBBanDau.GetDescription();
        public EnumLyDoVaoVien KhamChuaBenh { get; set; }
        public string KhamChuaBenhDisplay => KhamChuaBenh.GetDescription();

        public int SoLanDenKham { get; set; }

        public decimal? ChiPhiXetNghiemKhongApDungTLTT { get; set; }
        public decimal? ChiPhiCDHATDCNKhongApDungTLTT { get; set; }
        public decimal? ChiPhiThuocKhongApDungTLTT { get; set; }
        public decimal? ChiPhiMauKhongApDungTLTT { get; set; }
        public decimal? ChiPhiTTPTKhongApDungTLTT { get; set; }
        public decimal? ChiPhiVTYTKhongApDungTLTT { get; set; }

        public decimal? ChiPhiDVKTThanhToanTiLe { get; set; }
        public decimal? ChiPhiThuocThanhToanTiLe { get; set; }
        public decimal? ChiPhiVTYTThanhToanTiLe { get; set; }

        public decimal? TienKham { get; set; }
        public decimal? VanChuyen { get; set; }

        public decimal? TongCongChiPhiKCB { get; set; }

        public decimal? NguoiBenhChiTra => TongCongChiPhiKCB - TongCongChiPhiBHXHThanhToan;
        public decimal? TongCongChiPhiBHXHThanhToan { get; set; } 
        public decimal? ChiPhiNgoaiQuyDinhSuat { get; set; }
    }

    public class DanhSachDeNghiThanhToanChiPhiKCBNoiTru : GridItem
    {
        public NoiDangKyKCBBanDau NoiDangKyKCBBanDau { get; set; }
        public string NoiDangKyKCBBanDauDisplay => NoiDangKyKCBBanDau.GetDescription();
        public EnumLyDoVaoVien KhamChuaBenh { get; set; }
        public string KhamChuaBenhDisplay => KhamChuaBenh.GetDescription();

        public int SoLanDenKham { get; set; }
        public int? TongSoNgayDieuTri { get; set; }

        public decimal? ChiPhiXetNghiemKhongApDungTLTT { get; set; }
        public decimal? ChiPhiCDHATDCNKhongApDungTLTT { get; set; }
        public decimal? ChiPhiThuocKhongApDungTLTT { get; set; }
        public decimal? ChiPhiMauKhongApDungTLTT { get; set; }
        public decimal? ChiPhiTTPTKhongApDungTLTT { get; set; }
        public decimal? ChiPhiVTYTKhongApDungTLTT { get; set; }

        public decimal? ChiPhiDVKTThanhToanTiLe { get; set; }
        public decimal? ChiPhiThuocThanhToanTiLe { get; set; }
        public decimal? ChiPhiVTYTThanhToanTiLe { get; set; }

        public decimal? TienKham { get; set; }
        public decimal? TienGiuong { get; set; }
        public decimal? VanChuyen { get; set; }

        public decimal? TongCongChiPhiKCB { get; set; }

        public decimal? NguoiBenhChiTra => TongCongChiPhiKCB - TongCongChiPhiBHXHThanhToan;

        public decimal? TongCongChiPhiBHXHThanhToan { get; set; }

        public decimal? ChiPhiNgoaiQuyDinhSuat { get; set; }
    }


    public enum NoiDangKyKCBBanDau
    {
        [Description("BỆNH NHÂN NỘI TỈNH KHÁM, CHỮA BỆNH BAN ĐẦU")]
        BenhNhanNoiTinhKhamChuaBenhBanDau = 1,
        [Description("BỆNH NHÂN NỘI TỈNH ĐẾN")]
        BenhNhanNoiTinhDen = 2,
        [Description("BỆNH NHÂN NGOẠI TỈNH ĐẾN")]
        BenhNhanNgoaiTinhDen = 3,
    }

    public enum MauBaoBaoBHYT
    {
        [Description("Báo cáo 79a (ngoại trú)")]
        BaoCao79 = 1,
        [Description("Báo cáo 80a (nội trú)")]
        BaoCao80 = 2,
    }

    public enum HinhThucXuatBaoBao
    {
        [Description("Báo cáo theo ngày")]
        BaoCaoTheoNgay = 1,
        [Description("Báo cáo theo tháng")]
        BaoCaoTheoThang = 2,
        [Description("Báo cáo theo quý")]
        BaoCaoTheoQuy = 3,
    }
}
