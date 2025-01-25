using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.LichSuKhamChuaBenhs
{
    public class LichSuKhamChuaBenhTheoNguoiBenhVo
    {
        public LichSuKhamChuaBenhTheoNguoiBenhVo()
        {
            LichSuTiepNhans = new List<LichSuTiepNhanNguoiBenhVo>();
        }
        public List<LichSuTiepNhanNguoiBenhVo> LichSuTiepNhans { get; set; }
    }

    public class LichSuTiepNhanNguoiBenhVo
    {
        public LichSuTiepNhanNguoiBenhVo()
        {
            ChiTietKhamChuaBenhs = new List<ChiTietKhamChuaBenhVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public DateTime NgayTiepNhan { get; set; }
        public string NgayTiepNhanDisplay => NgayTiepNhan.ApplyFormatDate();
        public string DisplayName => $"Ngày tiếp nhận - {NgayTiepNhanDisplay}";

        public List<ChiTietKhamChuaBenhVo> ChiTietKhamChuaBenhs { get; set; }

        public bool HienLichSuChiTiet { get; set; }
        // dùng trên UI để xử lý ẩn hiện phần lịch sử nội trú theo phân quyền
        public bool CoLichSuYLenh => ChiTietKhamChuaBenhs.Any(x => x.LoaiLichSuKhamChuaBenh == Enums.LoaiLichSuKhamChuaBenh.YLenh);
        public bool HienLichSuNoiTru { get; set; }
    }

    public class ChiTietKhamChuaBenhVo
    {
        public Enums.LoaiLichSuKhamChuaBenh LoaiLichSuKhamChuaBenh { get; set; }
        public string TenLoaiLichSuKhamChuaBenh => LoaiLichSuKhamChuaBenh.GetDescription();
        public bool IsGroupItem { get; set; }


        public Enums.LoaiLichSuKhamChuaBenhChiTiet? LoaiLichSuKhamChuaBenhChiTiet { get; set; }
        public long? YeuCauDichVuId { get; set; }
        public string TenDichVu { get; set; }
        public DateTime? ThoiDiemKhamTu { get; set; }
        public DateTime? ThoiDiemKhamDen { get; set; }
        public string ThoiDiemKhamDisplay => ThoiDiemKhamTu?.ApplyFormatDate() + (ThoiDiemKhamDen == null ? "" : "- " + ThoiDiemKhamDen?.ApplyFormatDate());
        public string DisplayName => IsGroupItem ? TenDichVu : ((ThoiDiemKhamTu == null && ThoiDiemKhamDen == null) ? $"{TenDichVu}" : $"{TenDichVu} - {ThoiDiemKhamDisplay}");

        public long? NoiTruBenhAnId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru? LoaiHoSoDieuTriNoiTru { get; set; }
        public long? NoiTruHoSoKhacId { get; set; }
    }

    public class HtmlXuatPdfInfoVo
    {
        public HtmlXuatPdfInfoVo()
        {
            ListHtml = new List<HtmlToPdfVo>();
        }
        public string Html { get; set; }
        public List<HtmlToPdfVo> ListHtml { get; set; }
        public string BreakTag { get; set; }
        public string TypeSizeDefault { get; set; }
        public string TypeLayoutDefault { get; set; }
        public string TypeLayoutLandscapeDefault { get; set; }
        public string FormatPageOpenTagDefault { get; set; }
        public string FormatScriptTagDefault { get; set; }
        public string FormatPageCloseTagDefault { get; set; }
        public bool TypeLandscapeLayout { get; set; }
    }
}
