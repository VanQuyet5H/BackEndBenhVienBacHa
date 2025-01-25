using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.VatTu
{
    public class VatTuGridVo : GridItem
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
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }
    }
    public class NhomVatTuTreeViewVo : LookupItemVo
    {
        public NhomVatTuTreeViewVo()
        {
            Items = new List<NhomVatTuTreeViewVo>();
        }

        public bool IsDisabled { get; set; }
        public int Level { get; set; }
        public long? ParentId { get; set; }
        public List<NhomVatTuTreeViewVo> Items { get; set; }
    }
}
