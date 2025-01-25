using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.DoiTuongUuDais
{
    public class DoiTuongUuDai : BaseEntity
    {
        public string Ten { get; set; }
       
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }

        #region Update 16/2/2020

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }
        private ICollection<DoiTuongUuDaiDichVuKyThuatBenhVien> _doiTuongUuDaiDichVuKyThuatBenhViens { get; set; }
        public virtual ICollection<DoiTuongUuDaiDichVuKyThuatBenhVien> DoiTuongUuDaiDichVuKyThuatBenhViens
        {
            get => _doiTuongUuDaiDichVuKyThuatBenhViens ?? (_doiTuongUuDaiDichVuKyThuatBenhViens = new List<DoiTuongUuDaiDichVuKyThuatBenhVien>());
            protected set => _doiTuongUuDaiDichVuKyThuatBenhViens = value;
        }

        private ICollection<DoiTuongUuDaiDichVuKhamBenhBenhVien> _doiTuongUuDaiDichVuKhamBenhBenhViens { get; set; }
        public virtual ICollection<DoiTuongUuDaiDichVuKhamBenhBenhVien> DoiTuongUuDaiDichVuKhamBenhBenhViens
        {
            get => _doiTuongUuDaiDichVuKhamBenhBenhViens ?? (_doiTuongUuDaiDichVuKhamBenhBenhViens = new List<DoiTuongUuDaiDichVuKhamBenhBenhVien>());
            protected set => _doiTuongUuDaiDichVuKhamBenhBenhViens = value;
        }
        #endregion Update 16/2/2020

        private ICollection<MienGiamChiPhi> _mienGiamChiPhis { get; set; }
        public virtual ICollection<MienGiamChiPhi> MienGiamChiPhis
        {
            get => _mienGiamChiPhis ?? (_mienGiamChiPhis = new List<MienGiamChiPhi>());
            protected set => _mienGiamChiPhis = value;
        }

    }
}