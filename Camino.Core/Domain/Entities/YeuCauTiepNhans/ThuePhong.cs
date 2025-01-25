using Camino.Core.Domain.Entities.CauHinhs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauTiepNhans
{
    public class ThuePhong : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatThuePhongId { get; set; }
        public long CauHinhThuePhongId { get; set; }
        public string TenCauHinhThuePhong { get; set; }
        public long LoaiThuePhongPhauThuatId { get; set; }
        public long LoaiThuePhongNoiThucHienId { get; set; }
        public int BlockThoiGianTheoPhut { get; set; }
        public decimal GiaThue { get; set; }
        public int PhanTramNgoaiGio { get; set; }
        public int PhanTramLeTet { get; set; }
        public decimal GiaThuePhatSinh { get; set; }
        public int PhanTramPhatSinhNgoaiGio { get; set; }
        public int PhanTramPhatSinhLeTet { get; set; }
        public DateTime ThoiDiemBatDau { get; set; }
        public DateTime ThoiDiemKetThuc { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public long NoiChiDinhId { get; set; }
        public long YeuCauDichVuKyThuatTinhChiPhiId { get; set; }

        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual YeuCauKhamBenhs.YeuCauDichVuKyThuat YeuCauDichVuKyThuatThuePhong { get; set; }
        public virtual CauHinhThuePhong CauHinhThuePhong { get; set; }
        public virtual LoaiThuePhongPhauThuat LoaiThuePhongPhauThuat { get; set; }
        public virtual LoaiThuePhongNoiThucHien LoaiThuePhongNoiThucHien { get; set; }
        public virtual NhanViens.NhanVien NhanVienChiDinh { get; set; }
        public virtual PhongBenhViens.PhongBenhVien NoiChiDinh { get; set; }
        public virtual YeuCauKhamBenhs.YeuCauDichVuKyThuat YeuCauDichVuKyThuatTinhChiPhi { get; set; }
    }
}
