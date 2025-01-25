using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.HopDongThauDuocPham;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class HopDongThauDuocPhamExportExcel : GridItem
    {
        public HopDongThauDuocPhamExportExcel()
        {
            HopDongThauDuocPhamExportExcelChild = new List<HopDongThauDuocPhamExportExcelChild>();
        }

        [Width(85)]
        public string NhaThau { get; set; }

        [Width(42)]
        public string SoHopDong { get; set; }

        [Width(22)]
        public string SoQuyetDinh { get; set; }

        public string CongBoDisplay { get; set; }

        [Width(27)]
        public string NgayKyDisplay { get; set; }

        public string NgayHieuLucDisplay { get; set; }

        public string NgayHetHanDisplay { get; set; }

        public string TenLoaiThau { get; set; }

        public string TenLoaiThuocThau { get; set; }

        public string NhomThau { get; set; }

        public string GoiThau { get; set; }

        public string Nam { get; set; }

        public List<HopDongThauDuocPhamExportExcelChild> HopDongThauDuocPhamExportExcelChild { get; set; }
    }
}
