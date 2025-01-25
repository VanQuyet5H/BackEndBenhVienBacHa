using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.DinhMucDuocPhamTonKho
{
    public class DinhMucDuocPhamTonKhoGridVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public long KhoDuocPhamId { get; set; }
        public string TenKhoDuocPham { get; set; }
        public string TenDuocPham { get; set; }
        public int? TonToiThieu { get; set; }
        public string TonToiThieuDisplay { get; set; }
        public int? TonToiDa { get; set; }
        public string TonToiDaDisplay { get; set; }
        public int? SoNgayTruocKhiHetHan { get; set; }
        public string SoNgayTruocKhiHetHanDisplay { get; set; }
        public string MoTa { get; set; }
    }
    public class VatTuDropdownTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string NhaSanXuat { get; set; }
    }

    public class VatTuDinhMucVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public string Ma { get; set; }

        public string Ten { get; set; }

        public string NhaSanXuat { get; set; }
    }
}
