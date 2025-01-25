using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTonKhoKTGridVo : GridItem
    {
        public string MaVTYT { get; set; }
        public string TenVTYT { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public string HanSuDung { get; set; }
        public double? TonDau { get; set; }
        public double? Nhap { get; set; }
        public double? TongSo => ((TonDau ?? 0) + (Nhap ?? 0)).MathRoundNumber(2);
        public double? Xuat { get; set; }
        public double? TonCuoi => ((TongSo ?? 0) - (Xuat ?? 0)).MathRoundNumber(2);
        public long? NhomId { get; set; }
        public string Nhom { get; set; }
        public string Loai { get; set; }//Viện Phí / Thuốc BHYTYT
    }

    public class BaoCaoChiTietTonKhoKTGridVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string SoLo { get; set; }
        public string DVT { get; set; }
        public DateTime HanSuDungDateTime { get; set; }
        public string HanSuDung { get; set; }
        public double? SLNhap { get; set; }
        public double? SLXuat { get; set; }
        public DateTime NgayNhapXuat { get; set; }
        public string Nhom { get; set; }
        public string Loai { get; set; }//Viện Phí / Thuốc BHYT
        public bool LaVatTuBHYT { get; set; }
    }

    public class LoaiGroupVo
    {
        public string Loai { get; set; }
        public string Nhom { get; set; }
    }

    public class NhomGroupVo
    {
        public string Loai { get; set; }
        public string Nhom { get; set; }
    }
}
