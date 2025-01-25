using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class ThongTinKhoaPhongVo
    {
        public ThongTinKhoaPhongVo()
        {
            LuocDoPhauThuats = new List<LuocDoPhauThuatTaiLieuDinhKemVo>();
            MaPtttFormat = new List<string>();
        }

        public string ChanDoanKhoa { get; set; }

        public string GhiChuChanDoanKhoa { get; set; }

        public string KhoaPhong { get; set; }

        public long? ICDTruocId { get; set; }

        public string ICDTruoc { get; set; }

        public string MoTaCDTruocPhauThuat { get; set; }

        public long? ICDSauId { get; set; }

        public string ICDSau { get; set; }

        public string MoTaCDSauPhauThuat { get; set; }

        public string MaPttt { get; set; }

        public List<string> MaPtttFormat { get; set; }

        public string PhuongPhapPttt { get; set; }

        //public Enums.LoaiPhauThuatThuThuat? LoaiPttt { get; set; }

        //public string LoaiPtttDisplay => LoaiPttt.GetDescription();
        //Update loại PTTT (BVHD-3180)
        public string LoaiPhauThuatThuThuat { get; set; }

        public long? PpVoCamId { get; set; }

        public string PpVoCam { get; set; }

        public Enums.EnumTinhHinhPhauThuatThuThuat? TinhHinhPttt { get; set; }

        public string TinhHinHPtttDisplay => TinhHinhPttt.GetDescription();

        public Enums.EnumTaiBienPTTT? TaiBienPttt { get; set; }

        public string TaiBienPtttDisplay => TaiBienPttt.GetDescription();

        public string TrinhTuPttt { get; set; }

        public DateTime? ThoiGianPt { get; set; }

        public DateTime? ThoiGianBatDauGayMe { get; set; }

        public DateTime? ThoiGianKetThucPt { get; set; }

        public bool IsPhauThuat { get; set; }

        public long? NhanVienThucHienId { get; set; }
        public string NhanVienThucHienDisplay { get; set; }

        //BVHD-3877
        public string GhiChuCaPTTT { get; set; }

        public List<LuocDoPhauThuatTaiLieuDinhKemVo> LuocDoPhauThuats { get; set; }
    }

    public class LuocDoPhauThuatTaiLieuDinhKemVo : GridItem
    {
        public long? IdYeuCauDichVuKyThuat { get; set; }

        public string LuocDoPhauThuat { get; set; }

        public string MoTa { get; set; }
    }
}
