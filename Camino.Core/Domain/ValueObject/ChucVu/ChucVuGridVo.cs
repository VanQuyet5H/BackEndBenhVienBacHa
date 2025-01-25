using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.ChucVu
{
    public class ChucVuGridVo : GridItem
    {
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }


    public class TestChiSoXNGridVo : GridItem
    {
        public TestChiSoXNGridVo()
        {
            ChiSoXNChild = new List<TestChiSoXNGridVo>();
        }
        public string Ten { get; set; }
        public string TenCha { get; set; }
        public long? NhomDichVuBenhVienChaId { get; set; }
        public long CapNhom { get; set; }
        public string MoTa { get; set; }
        public string SearchString { get; set; }
        public bool HasChildren { get; set; }
        public virtual List<TestChiSoXNGridVo> ChiSoXNChild { get; set; }


    }
    public class ChucVuItemVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
    }
}
