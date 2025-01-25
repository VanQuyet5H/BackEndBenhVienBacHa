using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.VatTuBenhViens
{
   public class VatTuBenhVienGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? NhomVatTuId { get; set; }
        public string TenNhomVatTu { get; set; }
        public long? DonViTinhId { get; set; }
        public string TenDonViTinh { get; set; }
        public int? TyLeBaoHiemThanhToan { get; set; }
        public string QuyCach { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string BaoHiemChiTra { get; set; }
        public string HieuLuc { get; set; }

        public bool? IsDisabled { get; set; }
    }
    public class VatTuYTeDropdownTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string Nhom { get; set; }
        public string DonVi { get; set; }
    }
}
