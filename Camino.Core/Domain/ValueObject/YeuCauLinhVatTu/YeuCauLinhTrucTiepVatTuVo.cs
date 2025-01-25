using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhVatTu
{
    public class YeuCauLinhTrucTiepVatTuVo : GridItem
    {
        public int STT { get; set; }
        public string KeyId { get; set; }
        public long YeuCauLinhVatTuId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public string TenVatTu { get; set; }
        public string Nhom { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongYeuCau { get; set; }
        public long? KhoLinhId { get; set; }
        public long? PhongLinhVeId { get; set; }
        public string DichVuKham { get; set; }
        public string BacSiKeToa { get; set; }
        public string NgayKe { get; set; }
        public string HighLightClass { get; set; }
    }

    public class YeuCauLinhTrucTiepVatTuGridChildVo : GridItem
    {
        public int STT { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string DichVuKham { get; set; }
        public string BacSiKeToa { get; set; }
        public string NgayKe { get; set; }
        public double SoLuong { get; set; }
        public long? KhoLinhId { get; set; }
        public long? PhongLinhVeId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
    }
}
