using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauDichVuGiuongBenhVienChiPhiBHYT : BaseEntity
    {
        public DateTime NgayPhatSinh { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long DichVuGiuongBenhVienId { get; set; }
        public long? GiuongBenhId { get; set; }
        public long PhongBenhVienId { get; set; }
        public long KhoaPhongId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string MaTT37 { get; set; }
        public Enums.EnumLoaiGiuong LoaiGiuong { get; set; }
        public string MoTa { get; set; }
        public double SoLuong { get; set; }
        public int SoLuongGhep { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public string GhiChu { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public bool? HeThongTuPhatSinh { get; set; }
        public long? ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public long? YeuCauTiepNhanTheBHYTId { get; set; }
        public string MaSoTheBHYT { get; set; }

        public virtual YeuCauTiepNhanTheBHYT YeuCauTiepNhanTheBHYT { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
        public virtual GiuongBenh GiuongBenh { get; set; }
        public virtual PhongBenhVien PhongBenhVien { get; set; }
        public virtual KhoaPhong KhoaPhong { get; set; }
        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }
        public virtual YeuCauDichVuGiuongBenhVienChiPhiBenhVien ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien { get; set; }

        private ICollection<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTiets;
        public virtual ICollection<DuyetBaoHiemChiTiet> DuyetBaoHiemChiTiets
        {
            get => _duyetBaoHiemChiTiets ?? (_duyetBaoHiemChiTiets = new List<DuyetBaoHiemChiTiet>());
            protected set => _duyetBaoHiemChiTiets = value;
        }
    }
}

