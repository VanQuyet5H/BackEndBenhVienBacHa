using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;

namespace Camino.Core.Domain.Entities.DinhMucDuocPhamTonKhos
{
    public class DinhMucDuocPhamTonKho : BaseEntity
    {
        public long DuocPhamBenhVienId { get; set; }
        //public long KhoDuocPhamId { get; set; }
        public long KhoId { get; set; }
        public int? TonToiThieu { get; set; }
        public int? TonToiDa { get; set; }
        public int? SoNgayTruocKhiHetHan { get; set; }
        public string MoTa { get; set; }

        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual Kho KhoDuocPham { get; set; }
        //public virtual DuocPham DuocPham { get; set; }

    }
}
