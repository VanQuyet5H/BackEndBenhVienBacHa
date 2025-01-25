using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;


namespace Camino.Core.Domain.Entities.PhongBenhViens
{
    public class PhongBenhVienHangDoi : BaseEntity
    {
        public long PhongBenhVienId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public Enums.EnumTrangThaiHangDoi TrangThai { get; set; }
        public int SoThuTu { get; set; }

        public virtual PhongBenhVien PhongBenhVien { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
    }
}
