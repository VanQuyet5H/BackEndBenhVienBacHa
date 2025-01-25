using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;

namespace Camino.Core.Domain.Entities.KetQuaSinhHieus
{
    public class KetQuaSinhHieu : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }

        public int? KSKPhanLoaiTheLuc { get; set; }

        public int? NhipTim { get; set; }

        public int? NhipTho { get; set; }

        public double? ThanNhiet { get; set; }

        public int? HuyetApTamThu { get; set; }

        public int? HuyetApTamTruong { get; set; }

        public double? ChieuCao { get; set; }

        public double? CanNang { get; set; }

        public double? Bmi { get; set; }
        public double? Glassgow { get; set; }
        public double? SpO2 { get; set; }

        public DateTime? ThoiDiemThucHien { get; set; }

        public long? NoiThucHienId { get; set; }

        public long NhanVienThucHienId { get; set; }

        public long? YeuCauDichVuKyThuatKhamSangLocTiemChungId { get; set; }

        public virtual NhanVien NhanVienThucHien { get; set; }

        public virtual PhongBenhVien NoiThucHien { get; set; }

        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }

        public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }

        public virtual YeuCauDichVuKyThuatKhamSangLocTiemChung YeuCauDichVuKyThuatKhamSangLocTiemChung { get; set; }
    }
}
