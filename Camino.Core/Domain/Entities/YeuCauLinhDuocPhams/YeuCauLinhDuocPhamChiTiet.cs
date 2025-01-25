using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauLinhDuocPhams
{
    public class YeuCauLinhDuocPhamChiTiet : BaseEntity
    {
        public long YeuCauLinhDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public double SoLuong { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }

        public double? SoLuongDaLinhBu { get; set; }
        public double? SoLuongCanBu { get; set; }
        public string DanhSachMayXetNghiemId { get; set; }

        public virtual YeuCauLinhDuocPham YeuCauLinhDuocPham { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual YeuCauDuocPhamBenhVien YeuCauDuocPhamBenhVien { get; set; }

    }
}
