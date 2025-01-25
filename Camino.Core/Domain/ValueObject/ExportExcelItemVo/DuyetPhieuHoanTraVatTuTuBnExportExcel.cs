using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DuyetPhieuHoanTraVatTuTuBnExportExcel : GridItem
    {
        public DuyetPhieuHoanTraVatTuTuBnExportExcel()
        {
            DuyetPhieuHoanTraVatTuTuBnExportExcelChild = new List<DuyetPhieuHoanTraVatTuTuBnExportExcelChild>();
        }

        public string SoPhieu { get; set; }

        public string KhoaHoanTraDisplay { get; set; }

        [Width(40)]
        public string HoanTraVeKhoDisplay { get; set; }

        public string NguoiYeuCauDisplay { get; set; }

        public string NgayYeuCauDisplay { get; set; }

        public string TinhTrangDisplay { get; set; }

        public string NguoiDuyetDisplay { get; set; }

        public string NgayDuyetDisplay { get; set; }

        public List<DuyetPhieuHoanTraVatTuTuBnExportExcelChild> DuyetPhieuHoanTraVatTuTuBnExportExcelChild { get; set; }
    }

    public class DuyetPhieuHoanTraVatTuTuBnExportExcelChild
    {
        [TitleGridChild("Loại")]
        [Width(30)]
        public string Nhom { get; set; }

        [TitleGridChild("Vật tư")]
        [Width(30)]
        public string VatTu { get; set; }

        [TitleGridChild("ĐVT")]
        [Width(30)]
        public string Dvt { get; set; }

        [TitleGridChild("Tổng số lượng chỉ định")]
        [Width(30)]
        [TextAlign("right")]
        public double? TongSlChiDinhSetting { get; set; }

        [TitleGridChild("Tổng số lượng đã trả")]
        [Width(30)]
        [TextAlign("right")]
        public double? TongSlDaTraSetting { get; set; }

        [TitleGridChild("Tổng số lượng trả lần này")]
        [Width(30)]
        [TextAlign("right")]
        public double? TongSlTraLanNaySetting { get; set; }
    }
}
