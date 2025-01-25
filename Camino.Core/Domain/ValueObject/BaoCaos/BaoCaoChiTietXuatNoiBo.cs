using System;
using System.Collections.Generic;
using System.ComponentModel;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoChiTietXuatNoiBoQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }

    public class BaoCaoChiTietXuatNoiBoGridVo : GridItem
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

        public decimal? KhoTongKSNKNew { get; set; }
        public decimal? KhoVatTuThamMy { get; set; }
        public decimal? TuTrucTiepKhoaKSNKTM { get; set; }
        public decimal? TuTrucKhoaGMHSNew { get; set; }
        public decimal? TuTrucKhoaNhiSoSinh { get; set; }
        public decimal? TuTrucKhoaNoiNew { get; set; }
        public decimal? TuTrucKhoaSanNew { get; set; }
        public decimal? TuTrucPhongCapCuuNew { get; set; }
        public decimal? TongCong => KhoTongKSNKNew + KhoVatTuThamMy + TuTrucTiepKhoaKSNKTM
                + TuTrucKhoaGMHSNew + TuTrucKhoaNhiSoSinh + TuTrucKhoaNoiNew + TuTrucKhoaSanNew
                + TuTrucPhongCapCuuNew;
    }
}
