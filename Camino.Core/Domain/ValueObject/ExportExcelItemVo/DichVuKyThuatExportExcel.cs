using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DichVuKyThuatExportExcel
    {
        public DichVuKyThuatExportExcel()
        {
            DichVuKyThuatExportExcelChild = new List<DichVuKyThuatExportExcelChild>();
        }

        [Width(20)]
        public string Ma { get; set; }
        [Width(185)]
        public string Ten { get; set; }
        [Width(20)]
        public string TenTiengAnh { get; set; }
        [Width(20)]
        public string Ma4350 { get; set; }
        [Width(30)]
        public string MaGia { get; set; }
        [Width(20)]
        public string TenGia { get; set; }
        [Width(22)]
        public string TenNhomChiPhi { get; set; }
        [Width(20)]
        public string TenNhomDichVuKyThuat { get; set; }
        [Width(30)]
        public string TenLoaiPhauThuatThuThuat { get; set; }
        [Width(100)]
        public string MoTa { get; set; }
        public long Id { get; set; }
        public List<DichVuKyThuatExportExcelChild> DichVuKyThuatExportExcelChild { get; set; }
    }

    public class DichVuKyThuatExportExcelChild
    {
        [TitleGridChild("Hạng bệnh viện")]
        public string TenHangBenhVien { get; set; }
        [TitleGridChild("Giá")]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string GiaFormat { get; set; }
        [TitleGridChild("Từ ngày")]
        public string TuNgayFormat { get; set; }
        [TitleGridChild("Đến ngày")]
        public string DenNgayFormat { get; set; }
        [TitleGridChild("Thông tư")]
        public string ThongTu { get; set; }
        [TitleGridChild("Quyết định")]
        public string QuyetDinh { get; set; }
        [TitleGridChild("Mô tả")]
        public string MoTa { get; set; }
        [TitleGridChild("Hiệu lực")]
        public string TenHieuLuc { get; set; }
    }
}
