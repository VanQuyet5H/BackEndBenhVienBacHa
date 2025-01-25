using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.NhomDichVuBenhVien
{
    //public class NhomDichVuBenhVienGridVo : GridItem
    //{
    //    public string Ma { get; set; }

    //    public string NhomDichVuBenhVienCha { get; set; }

    //    public string Ten { get; set; }

    //    public string MoTa { get; set; }
    //    public bool? IsDefault { get; set; }
    //}

    public class NhomDichVuBenhVienTreeViewGridVo : GridItem
    {
        public string IdCap { get; set; }
        public int? CapNhomDichVuBenhVien { get; set; }
        public long? NhomDichVuBenhVienChaId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public string TenCha { get; set; }
        public string SearchString { get; set; }
        public bool HasChildren { get; set; }
        public bool? IsDefault { get; set; }
    }
}
