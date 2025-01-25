using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.Thuoc
{
    public class VatTuNhapKhoTemplateVo : LookupItemVo
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        
        public decimal? Gia { get; set; }
        public double? SoLuongChuaNhap { get; set; }
        public string DVT { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }
        public long NhomVatTuId { get; set; }
        public string TenNhomVatTu { get; set; }
        public double SLTon { get; set; }
    }

    public class VatTuNhapKhoTemplate
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public string Ten { get; set; }

        public string HoatChat { get; set; }

        public string SoDangKy { get; set; }
    }
}
