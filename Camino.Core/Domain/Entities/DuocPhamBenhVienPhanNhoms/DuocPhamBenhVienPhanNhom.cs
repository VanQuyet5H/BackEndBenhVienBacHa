using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms
{
    public class DuocPhamBenhVienPhanNhom : BaseEntity
    {
        public string Ten { get; set; }
        public long? NhomChaId { get; set; }
        public int CapNhom { get; set; }

        //BVHD-3454
        public string KyHieuPhanNhom { get; set; }
        public virtual DuocPhamBenhVienPhanNhom NhomCha { get; set; }

        private ICollection<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhoms;
        public virtual ICollection<DuocPhamBenhVienPhanNhom> DuocPhamBenhVienPhanNhoms
        {
            get => _duocPhamBenhVienPhanNhoms ?? (_duocPhamBenhVienPhanNhoms = new List<DuocPhamBenhVienPhanNhom>());
            protected set => _duocPhamBenhVienPhanNhoms = value;
        }

        private ICollection<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTiets;
        public virtual ICollection<YeuCauNhapKhoDuocPhamChiTiet> YeuCauNhapKhoDuocPhamChiTiets
        {
            get => _yeuCauNhapKhoDuocPhamChiTiets ?? (_yeuCauNhapKhoDuocPhamChiTiets = new List<YeuCauNhapKhoDuocPhamChiTiet>());
            protected set => _yeuCauNhapKhoDuocPhamChiTiets = value;

        }

        private ICollection<DuocPhamBenhVien> _duocPhamBenhViens;
        public virtual ICollection<DuocPhamBenhVien> DuocPhamBenhViens
        {
            get => _duocPhamBenhViens ?? (_duocPhamBenhViens = new List<DuocPhamBenhVien>());
            protected set => _duocPhamBenhViens = value;

        }

        private ICollection<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTiets;
        public virtual ICollection<NhapKhoDuocPhamChiTiet> NhapKhoDuocPhamChiTiets
        {
            get => _nhapKhoDuocPhamChiTiets ?? (_nhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>());
            protected set => _nhapKhoDuocPhamChiTiets = value;

        }

        private ICollection<YeuCauTraDuocPhamChiTiet> _yeuCauTraDuocPhamChiTiets { get; set; }
        public virtual ICollection<YeuCauTraDuocPhamChiTiet> YeuCauTraDuocPhamChiTiets
        {
            get => _yeuCauTraDuocPhamChiTiets ?? (_yeuCauTraDuocPhamChiTiets = new List<YeuCauTraDuocPhamChiTiet>());
            protected set => _yeuCauTraDuocPhamChiTiets = value;
        }
    }
}
