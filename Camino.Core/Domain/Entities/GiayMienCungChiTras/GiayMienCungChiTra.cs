using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauTiepNhanLichSuChuyenDoiTuongs;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;


namespace Camino.Core.Domain.Entities.GiayMienCungChiTras
{
    public class GiayMienCungChiTra : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }

        //public virtual ICollection<BenhNhan> BenhNhan { get; set; }
        //public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhan { get; set; }

        #region Update 12/2/2020

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        private ICollection<BenhNhan> _benhNhans;
        public virtual ICollection<BenhNhan> BenhNhans
        {
            get => _benhNhans ?? (_benhNhans = new List<BenhNhan>());
            protected set => _benhNhans = value;
        }

        #endregion Update 12/2/2020

        private ICollection<YeuCauTiepNhanLichSuChuyenDoiTuong> _yeuCauTiepNhanLichSuChuyenDoiTuongs;
        public virtual ICollection<YeuCauTiepNhanLichSuChuyenDoiTuong> YeuCauTiepNhanLichSuChuyenDoiTuongs
        {
            get => _yeuCauTiepNhanLichSuChuyenDoiTuongs ?? (_yeuCauTiepNhanLichSuChuyenDoiTuongs = new List<YeuCauTiepNhanLichSuChuyenDoiTuong>());
            protected set => _yeuCauTiepNhanLichSuChuyenDoiTuongs = value;
        }

        private ICollection<YeuCauTiepNhanTheBHYT> _yeuCauTiepNhanTheBHYTs;
        public virtual ICollection<YeuCauTiepNhanTheBHYT> YeuCauTiepNhanTheBHYTs
        {
            get => _yeuCauTiepNhanTheBHYTs ?? (_yeuCauTiepNhanTheBHYTs = new List<YeuCauTiepNhanTheBHYT>());
            protected set => _yeuCauTiepNhanTheBHYTs = value;
        }
    }
}
