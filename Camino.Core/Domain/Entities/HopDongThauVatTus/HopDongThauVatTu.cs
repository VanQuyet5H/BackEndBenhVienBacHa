using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;

namespace Camino.Core.Domain.Entities.HopDongThauVatTus
{
    public class HopDongThauVatTu : BaseEntity
    {
        public long NhaThauId { get; set; }
        public string SoHopDong { get; set; }
        public string SoQuyetDinh { get; set; }
        public DateTime CongBo { get; set; }
        public DateTime? NgayKy { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHan { get; set; }
        public Enums.EnumLoaiThau LoaiThau { get; set; }
        public string NhomThau { get; set; }
        public string GoiThau { get; set; }
        public int Nam { get; set; }
        public bool? HeThongTuPhatSinh { get; set; }
        public virtual NhaThau NhaThau { get; set; }

        private ICollection<HopDongThauVatTuChiTiet> _hopDongThauVatTuChiTiets;
        public virtual ICollection<HopDongThauVatTuChiTiet> HopDongThauVatTuChiTiets
        {
            get => _hopDongThauVatTuChiTiets ?? (_hopDongThauVatTuChiTiets = new List<HopDongThauVatTuChiTiet>());
            protected set => _hopDongThauVatTuChiTiets = value;
        }

        private ICollection<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTiets;
        public virtual ICollection<NhapKhoVatTuChiTiet> NhapKhoVatTuChiTiets
        {
            get => _nhapKhoVatTuChiTiets ?? (_nhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>());
            protected set => _nhapKhoVatTuChiTiets = value;

        }

        private ICollection<YeuCauNhapKhoVatTuChiTiet> _yeuCauNhapKhoVatTuChiTiets;

        public virtual ICollection<YeuCauNhapKhoVatTuChiTiet> YeuCauNhapKhoVatTuChiTiets
        {
            get => _yeuCauNhapKhoVatTuChiTiets ?? (_yeuCauNhapKhoVatTuChiTiets = new List<YeuCauNhapKhoVatTuChiTiet>());
            protected set => _yeuCauNhapKhoVatTuChiTiets = value;
        }

        private ICollection<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTiets;
        public virtual ICollection<DonVTYTThanhToanChiTiet> DonVTYTThanhToanChiTiets
        {
            get => _donVTYTThanhToanChiTiets ?? (_donVTYTThanhToanChiTiets = new List<DonVTYTThanhToanChiTiet>());
            protected set => _donVTYTThanhToanChiTiets = value;
        }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }

        private ICollection<YeuCauTraVatTuChiTiet> _yeuCauTraVatTuChiTiets;
        public virtual ICollection<YeuCauTraVatTuChiTiet> YeuCauTraVatTuChiTiets
        {
            get => _yeuCauTraVatTuChiTiets ?? (_yeuCauTraVatTuChiTiets = new List<YeuCauTraVatTuChiTiet>());
            protected set => _yeuCauTraVatTuChiTiets = value;

        }

        private ICollection<YeuCauXuatKhoVatTuChiTiet> _yeuCauXuatKhoVatTuChiTiets;
        public virtual ICollection<YeuCauXuatKhoVatTuChiTiet> YeuCauXuatKhoVatTuChiTiets
        {
            get => _yeuCauXuatKhoVatTuChiTiets ?? (_yeuCauXuatKhoVatTuChiTiets = new List<YeuCauXuatKhoVatTuChiTiet>());
            protected set => _yeuCauXuatKhoVatTuChiTiets = value;
        }
    }
}
