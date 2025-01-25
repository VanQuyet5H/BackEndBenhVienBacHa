using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DuTruGiamDocVatTuExportExcel:GridItem
    {
        public DuTruGiamDocVatTuExportExcel()
        {
            DuTruGiamDocVatTuExportExcelChild = new List<DuTruGiamDocVatTuExportExcelChild>();
        }

        public string SoPhieu { get; set; }

        [Width(30)]
        public string KyDuTruDisplay { get; set; }

        public string NguoiYeuCau { get; set; }

        [Width(25)]
        public string NgayYeuCauDisplay { get; set; }

        public string TrangThai { get; set; }

        [Width(25)]
        public string NgayDuyetDisplay { get; set; }

        public List<DuTruGiamDocVatTuExportExcelChild> DuTruGiamDocVatTuExportExcelChild { get; set; }
    }

    public class DuTruGiamDocVatTuExportExcelChild
    {
        [TitleGridChild("Loại")]
        [Width(30)]
        public string Loai { get; set; }

        [TitleGridChild("Vật tư")]
        [Width(30)]
        public string VatTu { get; set; }

        [TitleGridChild("ĐVT")]
        [Width(30)]
        public string Dvt { get; set; }

        public string QuyCach { get; set; }

        [TitleGridChild("Nhà SX")]
        [Width(30)]
        public string NhaSx { get; set; }

        [TitleGridChild("Nước SX")]
        [Width(30)]
        public string NuocSx { get; set; }

        [TitleGridChild("SL Dự trù")]
        [Width(30)]
        [TextAlign("right")]
        public string SoLuongDuTru { get; set; }

        [TitleGridChild("SL D.Kiến S.Dụng Trong Kỳ")]
        [Width(30)]
        [TextAlign("right")]
        public string SoLuongDuKienTrongKy { get; set; }

        [TitleGridChild("SL Dự trù T.Khoa Duyệt")]
        [Width(30)]
        [TextAlign("right")]
        public string SoLuongDuTruTrKhoa { get; set; }

        [TitleGridChild("SL Dự trù K.Dược Duyệt")]
        [Width(30)]
        [TextAlign("right")]
        public string SoLuongDuTruKhDuoc { get; set; }

        [TitleGridChild("SL Dự trù G.Đốc Duyệt")]
        [Width(30)]
        [TextAlign("right")]
        public string SoLuongDuTruDirector { get; set; }
    }
}
