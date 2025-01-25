using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.XetNghiems
{
    public class PhienXetNghiemChiTiet : BaseEntity
    {
        public long PhienXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public int LanThucHien { get; set; }
        public bool? DaGoiDuyet { get; set; }
        public string KetLuan { get; set; }

        public long? NhanVienKetLuanId { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }
        public DateTime? ThoiDiemCoKetQua { get; set; }
        public string GhiChu { get; set; }
        public bool? ChayLaiKetQua { get; set; }
        public long? YeuCauChayLaiXetNghiemId { get; set; }

        public DateTime? ThoiDiemLayMau { get; set; }
        public long? PhongLayMauId { get; set; }
        public long? NhanVienLayMauId { get; set; }
        public DateTime? ThoiDiemNhanMau { get; set; }
        public long? PhongNhanMauId { get; set; }
        public long? NhanVienNhanMauId { get; set; }

        public virtual PhienXetNghiem PhienXetNghiem { get; set; }
        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual NhanVien NhanVienKetLuan { get; set; }
        public virtual YeuCauChayLaiXetNghiem YeuCauChayLaiXetNghiem { get; set; }

        public virtual PhongBenhVien PhongLayMau { get; set; }
        public virtual NhanVien NhanVienLayMau { get; set; }
        public virtual PhongBenhVien PhongNhanMau { get; set; }
        public virtual NhanVien NhanVienNhanMau { get; set; }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTiets;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets
        {
            get => _ketQuaXetNghiemChiTiets ?? (_ketQuaXetNghiemChiTiets = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTiets = value;
        }
    }
}
