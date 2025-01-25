using System;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.GiuongBenhs
{
    public class HoatDongGiuongBenh : BaseEntity
    {
        public long GiuongBenhId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public DateTime ThoiDiemBatDau { get; set; }
        public DateTime? ThoiDiemKetThuc { get; set; }
        public bool? NamGhep { get; set; }

        public Enums.TinhTrangBenhNhan? TinhTrangBenhNhan { get; set; }

        public virtual GiuongBenh GiuongBenh { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual YeuCauDichVuGiuongBenhVien YeuCauDichVuGiuongBenhVien { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }

    }
}