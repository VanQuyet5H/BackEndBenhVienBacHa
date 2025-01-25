using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoChiTietHoanTraNoiBoQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<long> KhoIds { get; set; }
    }

    public class BaoCaoChiTietHoanTraNoiBoGridVo : GridItem
    {
        public long VatTuBenhVienId { get; set; }
        public string TenVTYT => $"{Ten} ({(!string.IsNullOrEmpty(NhaSanXuat) && !string.IsNullOrEmpty(NuocSanXuat) ? $"{NhaSanXuat}, {NuocSanXuat}" : $"{NhaSanXuat}{NuocSanXuat}")})";
        public string Ten { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string DVT { get; set; }
        public string Kho { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien => (decimal)(SoLuong ?? 0) * (DonGia ?? 0);
    }
}
