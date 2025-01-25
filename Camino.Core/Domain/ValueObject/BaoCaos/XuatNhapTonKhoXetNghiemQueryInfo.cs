using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class XuatNhapTonKhoXetNghiemQueryInfo : QueryInfo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }        
        public long KhoId { get; set; }     
    }   

    public class DanhSachXuatNhapTonKhoXetNghiem : GridItem
    {       
        public string Nhom { get; set; }

        public string MaDuocPham { get; set; }
        public string DuocPham { get; set; }
        public string DonViTinh { get; set; }

        public double? TongDauKy { get; set; }
        public double? NhapTrongKy { get; set; }
        public double? XuatTrongKy { get; set; }
        public double? TonCuoiKy => (TongDauKy ?? 0) + (NhapTrongKy ?? 0) - (XuatTrongKy ?? 0);

        public string SoLoSX { get; set; }
        public DateTime HanDung { get; set; }
        public string HanDungDisplay => HanDung.ApplyFormatDate();

        public string GhiChu { get; set; }
    }
}
