using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.TuDienKyThuat
{
    public class TuDienKyThuatGridVo : GridItem
    {        
        public string IdCap { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }

        public long? DichVuKyThuatId { get; set; }
        public string TenDichVuKyThuat { get; set; }

        public string Ma { get; set; }
        public string Ten { get; set; }
       
        public string TenCha { get; set; }
        public string ChiSoCha { get; set; }       
        public long? DichVuChaId { get; set; }
        public long? DichVuId { get; set; }
        public int? CapDichVu { get; set; }
        public string MoTa { get; set; }
        public string SearchString { get; set; }
        public bool HasChildren { get; set; }

        public string TenKetQuaMau { get; set; }
        public string MaSo { get; set; }
        public string KetQua { get; set; }
        public string KetLuan { get; set; }

        public long UserLoginId { get; set; }
        public string TenUserLogin { get; set; }
    }  
}
