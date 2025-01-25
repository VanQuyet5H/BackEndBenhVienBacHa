using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class KhamBenhPhauThuatThuThuatGridVo : GridItem
    {
        public string Ten { get; set; }

        public long? ICDTruocPhauThuatId { get; set; }

        public string ICDTruocPhauThuatDisplay { get; set; }

        public long? ICDSauPhauThuatId { get; set; }

        public string ICDSauPhauThuatDisplay { get; set; }

        public string GhiChuICDTruocPhauThuat { get; set; }

        public string GhiChuICDSauPhauThuat { get; set; }

        public string PhuongPhapPhauThuatThuThuatKey { get; set; }

        public string PhuongPhapPhauThuatThuThuatDisplay { get; set; }

        public Enums.LoaiPhauThuatThuThuat? LoaiPTTTEnum { get; set; }

        public string LoaiPTTTDisplay { get; set; }

        public long? PhuongPhapVoCamId { get; set; }

        public string PhuongPhapVoCamDisplay { get; set; }

        public Enums.EnumTinhHinhPhauThuatThuThuat? TinhHinhPttt { get; set; }

        public string TinhHinhPtttDisplay { get; set; }

        public Enums.EnumTaiBienPTTT? TaiBienPttt { get; set; }

        public string TaiBienPtttDisplay { get; set; }

        public Enums.EnumTuVongPTTTTheoNgay? TuVongPttt { get; set; }

        public string TuVongPtttDisplay { get; set; }

        public string TrinhTuPttt { get; set; }

        public string NhanVienThucHienDisplay { get; set; }

        // 25/6/2020 // TO DO
        public List<YeuCauDichVuKyThuatLuocDoPhauThuat> ListLuocDoPhauThuatThuThuatResultVo { get; set; }
    }

    public class ThongTinPhauThuat
    {
        public string TenKhoaPhong { get; set; }

        public string ChanDoanVaoKhoa { get; set; }

        public string MoTa { get; set; }
    }

    public class ListDichVuKyThuatParameterGridVo
    {
        public long[] Ids { get; set; }
    }

    public class LuocDoPhauThuatThuThuatResultVo : GridItem
    {
        public long IdYeuCauDichVuKyThuat { get; set; }

        public string LuocDoPhauThuat { get; set; }

        public string MoTa { get; set; }
    }

    public class TuongTrinhTuVongResultVo
    {
        public Enums.EnumThoiGianTuVongPTTTTheoNgay? TgTuVong { get; set; }

        public string TgTuVongDisplay => TgTuVong.GetDescription();

        public Enums.EnumTuVongPTTTTheoNgay? TuVong { get; set; }

        public string TuVongDisplay => TuVong.GetDescription();

        public long IdDvkt { get; set; }
    }
}
