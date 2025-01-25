using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class PhauThuatThuThuatTuongTrinhViewModel
    {
        public PhauThuatThuThuatTuongTrinhViewModel()
        {
            LuocDoPhauThuats = new List<LuocDoPhauThuatTaiLieuDinhKem>();
        }

        public int? ICDTruocId { get; set; }

        public string ICDTruoc { get; set; }

        public int? ICDSauId { get; set; }

        public string ICDSau { get; set; }

        public string MaPttt { get; set; }

        public string PhuongPhapPttt { get; set; }

        public Enums.LoaiPhauThuatThuThuat? LoaiPttt { get; set; }

        public long? PpVoCamId { get; set; }

        public Enums.EnumTinhHinhPhauThuatThuThuat? TinhHinhPttt { get; set; }

        public Enums.EnumTaiBienPTTT? TaiBienPttt { get; set; }

        public string TrinhTuPttt { get; set; }

        public long YeuCauDichVuKyThuatId { get; set; }

        public string MoTaCDTruocPhauThuat { get; set; }

        public string MoTaCDSauPhauThuat { get; set; }

        public DateTime? ThoiGianBatDauGayMe { get; set; }

        public DateTime? ThoiGianPt { get; set; }

        public DateTime? ThoiGianKetThucPt { get; set; }

        public List<LuocDoPhauThuatTaiLieuDinhKem> LuocDoPhauThuats { get; set; }
    }

    public class LuocDoPhauThuatTaiLieuDinhKem : BaseViewModel
    {
        public long? IdYeuCauDichVuKyThuat { get; set; }

        public string LuocDoPhauThuat { get; set; }

        public string MoTa { get; set; }
    }

    public class TuongTrinhTuVongViewModel
    {
        public Enums.EnumThoiGianTuVongPTTTTheoNgay? TgTuVong { get; set; }

        public string TgTuVongDisplay => TgTuVong.GetDescription();

        public Enums.EnumTuVongPTTTTheoNgay? TuVong { get; set; }

        public string TuVongDisplay => TuVong.GetDescription();

        public long? IdDvkt { get; set; }

        public long? YeuCauTiepNhanId { get; set; }
        public long? TheoDoiSauPhauThuatThuThuatId { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public long? PhongBenhVienId { get; set; }
    }

    public class RutTuongTrinhViewModel
    {
        public long YcdvktId { get; set; }

        public string GhiChu { get; set; }

        //BVHD-3860
        public bool? XoaThuocVaTu { get; set; }
    }
}
