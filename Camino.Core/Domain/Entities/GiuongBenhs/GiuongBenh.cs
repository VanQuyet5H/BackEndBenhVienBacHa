using System.Collections.Generic;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.GiuongBenhs
{
    public class GiuongBenh : BaseEntity
    {
        public long PhongBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? LaGiuongNoi { get; set; }

        public virtual PhongBenhVien PhongBenhVien { get; set; }

        private ICollection<HoatDongGiuongBenh> _hoatDongGiuongBenhs { get; set; }
        public virtual ICollection<HoatDongGiuongBenh> HoatDongGiuongBenhs
        {
            get => _hoatDongGiuongBenhs ?? (_hoatDongGiuongBenhs = new List<HoatDongGiuongBenh>());
            protected set => _hoatDongGiuongBenhs = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhViens
        {
            get => _yeuCauDichVuGiuongBenhViens ?? (_yeuCauDichVuGiuongBenhViens = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhViens = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _yeuCauDichVuGiuongBenhVienChiPhiBHYTs;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhiBHYT> YeuCauDichVuGiuongBenhVienChiPhiBHYTs
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhiBHYTs ?? (_yeuCauDichVuGiuongBenhVienChiPhiBHYTs = new List<YeuCauDichVuGiuongBenhVienChiPhiBHYT>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhiBHYTs = value;
        }
        private ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> _yeuCauDichVuGiuongBenhVienChiPhiBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> YeuCauDichVuGiuongBenhVienChiPhiBenhViens
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens ?? (_yeuCauDichVuGiuongBenhVienChiPhiBenhViens = new List<YeuCauDichVuGiuongBenhVienChiPhiBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens = value;
        }
    }
}