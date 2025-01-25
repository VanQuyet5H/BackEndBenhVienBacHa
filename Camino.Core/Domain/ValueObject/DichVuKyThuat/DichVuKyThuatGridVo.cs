using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.DichVuKyThuat
{
    public class DichVuKyThuatGridVo : GridItem
    {
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string Ma { get; set; }
        public string Ma4350 { get; set; }
        public string MaGia { get; set; }
        public string TenGia { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi NhomChiPhi { get; set; }
        public string TenNhomChiPhi { get; set; }
        public long NhomDichVuKyThuatId { get; set; }
        public string TenNhomDichVuKyThuat { get; set; }
        public Enums.LoaiPhauThuatThuThuat LoaiPhauThuatThuThuat { get; set; }
        public string TenLoaiPhauThuatThuThuat { get; set; }
        public string MoTa { get; set; }
    }

    public class NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo : LookupItemVo
    {
        public NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo()
        {
            Items = new List<NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo>();
        }

        //public bool IsDisabled { get; set; }
        public int Level { get; set; }
        public long? ParentId { get; set; }
        public List<NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo> Items { get; set; }
    }
}
