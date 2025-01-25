using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DoiTuongUuDais
{
    public class DoiTuongUuDaiGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ma4350 { get; set; }
        public long DichVuKyThuatId { get; set; }
        public string Ten { get; set; }
        public string TenKhoa { get; set; }
    }
    public class DoiTuongUuDaiChildGridVo : GridItem
    {
        public string DoiTuong { get; set; }
        public string TiLeUuDai { get; set; }
        
    }
    public class DoiTuongUuDaiTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string DichVu { get; set; }
        public string Ma { get; set; }
        public string Ma4350 { get; set; }
        public string TenKhoa { get; set; }
    }
}
