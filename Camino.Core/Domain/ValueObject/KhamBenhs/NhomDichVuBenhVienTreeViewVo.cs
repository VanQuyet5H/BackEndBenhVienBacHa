using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class NhomDichVuBenhVienTreeViewVo : LookupItemVo
    {
        public NhomDichVuBenhVienTreeViewVo()
        {
            Items = new List<NhomDichVuBenhVienTreeViewVo>();
        }

        public bool IsDisabled { get; set; }
        public int Level { get; set; }
        public long? ParentId { get; set; }
        public string Ma { get; set; }
        public bool IsDefault { get; set; }
        public List<NhomDichVuBenhVienTreeViewVo> Items { get; set; }
    }
    public class NhomDichVuBenhVienJSONVo {
        public bool LaPhieuDieuTri { get; set; }
    }

}
