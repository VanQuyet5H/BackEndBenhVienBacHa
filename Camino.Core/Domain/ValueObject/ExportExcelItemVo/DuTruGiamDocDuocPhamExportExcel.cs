using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DuTruGiamDocDuocPhamExportExcel : GridItem
    {
        public DuTruGiamDocDuocPhamExportExcel()
        {
            DuTruGiamDocDuocPhamExportExcelChild = new List<DuTruGiamDocDuocPhamExportExcelChild>();
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

        public List<DuTruGiamDocDuocPhamExportExcelChild> DuTruGiamDocDuocPhamExportExcelChild { get; set; }
    }

    public class DuTruGiamDocDuocPhamExportExcelChild
    {
        [TitleGridChild("Loại")]
        [Width(30)]
        public string Loai { get; set; }

        [TitleGridChild("Dược phẩm")]
        [Width(30)]
        public string DuocPham { get; set; }

        [TitleGridChild("Hoạt chất")]
        [Width(30)]
        public string HoatChat { get; set; }

        [TitleGridChild("Nồng độ/ Hàm lượng")]
        [Width(30)]
        public string NongDo { get; set; }

        [TitleGridChild("SĐK")]
        [Width(30)]
        public string Sdk { get; set; }

        [TitleGridChild("ĐVT")]
        [Width(30)]
        public string Dvt { get; set; }

        [TitleGridChild("ĐD")]
        [Width(30)]
        public string DuongDung { get; set; }

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
