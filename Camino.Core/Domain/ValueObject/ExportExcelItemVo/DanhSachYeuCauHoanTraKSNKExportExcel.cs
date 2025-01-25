using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DanhSachYeuCauHoanTraKSNKExportExcel : GridItem
    {
        public DanhSachYeuCauHoanTraKSNKExportExcel()
        {
            DanhSachYeuCauHoanTraKSNKExportExcelChild = new List<DanhSachYeuCauHoanTraKSNKChiTietExportExcelChild>();
        }

        public string Ma { get; set; }
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }

        [Width(24)]
        public string NguoiYeuCau { get; set; }

        [Width(30)]
        public string KhoHoanTraTu { get; set; }

        public string KhoHoanTraVe { get; set; }

        [Width(25)]
        public string NgayYeuCauText { get; set; }

        [Width(25)]
        public string TinhTrangDisplay { get; set; }

        [Width(30)]
        public string NguoiDuyet { get; set; }

        [Width(25)]
        public string NgayDuyetText { get; set; }

        public List<DanhSachYeuCauHoanTraKSNKChiTietExportExcelChild> DanhSachYeuCauHoanTraKSNKExportExcelChild { get; set; }
    }
}
