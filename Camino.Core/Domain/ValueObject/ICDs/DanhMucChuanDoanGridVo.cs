using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ICDs
{
    public class DanhMucChuanDoanGridVo : GridItem
    {
        public string TenTiengViet { get; set; }
        public string TenTiengAnh { get; set; }
        public string GhiChu { get; set; }
    }
    public class ChuanDoanGridVo : GridItem
    {
        public string Ma { get; set; }
        public string TenTiengViet { get; set; }
        public string TenTiengAnh { get; set; }
    }
    public class ChuanDoanLienKetGridVo : GridItem
    {
        public string Ma { get; set; }
        public string TenTiengViet { get; set; }
        public string TenTiengAnh { get; set; }
        public long ICDId { get; set; }
        public long ChuanDoanId { get; set; }
    }
    public class DanhMucGridCombobox
    {
        public long KeyId { get; set; }
        public string DisplayName => TenTiengViet;
        public string TenTiengViet { get; set; }
        public string TenTiengAnh { get; set; }
        public string Ma { get; set; }
    }
}
