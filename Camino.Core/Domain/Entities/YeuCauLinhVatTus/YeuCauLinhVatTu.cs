using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.YeuCauLinhVatTus
{
    public class YeuCauLinhVatTu : BaseEntity
    {
        public long KhoXuatId { get; set; }
        public long KhoNhapId { get; set; }
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public long? NoiYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public bool? DuocDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string LyDoKhongDuyet { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }

        public DateTime? ThoiDiemLinhTongHopTuNgay { get; set; }
        public DateTime? ThoiDiemLinhTongHopDenNgay { get; set; }
        public bool? DaGui { get; set; }

        public virtual Kho KhoXuat { get; set; }
        public virtual Kho KhoNhap { get; set; }
        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }
        public virtual PhongBenhVien NoiYeuCau { get; set; }

        private ICollection<XuatKhoVatTu> _xuatKhoVatTus;
        public virtual ICollection<XuatKhoVatTu> XuatKhoVatTus
        {
            get => _xuatKhoVatTus ?? (_xuatKhoVatTus = new List<XuatKhoVatTu>());
            protected set => _xuatKhoVatTus = value;
        }

        private ICollection<NhapKhoVatTu> _nhapKhoVatTus;

        public virtual ICollection<NhapKhoVatTu> NhapKhoVatTus
        {
            get => _nhapKhoVatTus ?? (_nhapKhoVatTus = new List<NhapKhoVatTu>());
            protected set => _nhapKhoVatTus = value;
        }

        private ICollection<YeuCauLinhVatTuChiTiet> _yeuCauLinhVatTuChiTiets;

        public virtual ICollection<YeuCauLinhVatTuChiTiet> YeuCauLinhVatTuChiTiets
        {
            get => _yeuCauLinhVatTuChiTiets ?? (_yeuCauLinhVatTuChiTiets = new List<YeuCauLinhVatTuChiTiet>());
            protected set => _yeuCauLinhVatTuChiTiets = value;
        }


        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }
    }
}
