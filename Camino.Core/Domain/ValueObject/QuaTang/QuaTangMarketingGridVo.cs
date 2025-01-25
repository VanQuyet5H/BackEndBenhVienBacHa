using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.Marketing
{
    public class QuaTangMarketingGridVo : GridItem
    {
        public string Ten { get; set; }
        public string DonViTinh { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public string HieuLucDisplay => HieuLuc ? "Đang sử dụng" : "Ngừng sử dụng";
    }

    public class XuatKhoQuaTangMarketingGridVo : GridItem
    {
        public string SoPhieu { get; set; }
        public string NoiXuat { get; set; }
        public long? NguoiXuatId { get; set; }
        public string NhanVienXuat { get; set; }
        public long? BenhNhanId { get; set; }
        public string NguoiNhan { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string NgayXuatDisplay { get; set; }
        public string GhiChu { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string SearchString { get; set; }

    }

    public class XuatKhoQuaTangMarketingChiTietGridVo : GridItem
    {
        public string Ten { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuongXuat { get; set; }
        public long KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
    }

    public class QuaTangMarketingExporExcel
    {
        [Width(40)]
        public string Ten { get; set; }
        [Width(30)]
        public string DonViTinh { get; set; }
        [Width(40)]
        public string MoTa { get; set; }
        [Width(20)]
        public string HieuLuc { get; set; }
    }

    public class XuatKhoMarketingExporExcel
    {
        [Width(20)]
        public string SoPhieu { get; set; }
        [Width(30)]
        public string NoiXuat { get; set; }
        [Width(40)]
        public string NhanVienXuat { get; set; }
        [Width(20)]
        public string NguoiNhan { get; set; }
        [Width(20)]
        public string NgayXuatDisplay { get; set; }
        [Width(20)]
        public string GhiChu { get; set; }
    }
}