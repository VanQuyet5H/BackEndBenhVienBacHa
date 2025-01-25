using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.DanTocs
{
    public class DanToc :BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long QuocGiaId { get; set; }
        public bool? IsDisabled { get; set; }

        private ICollection<BenhNhan> _danTocBenhNhans;
        public virtual ICollection<BenhNhan> DanTocBenhNhans
        {
            get => _danTocBenhNhans ?? (_danTocBenhNhans = new List<BenhNhan>());
            protected set => _danTocBenhNhans = value;
        }

        #region Update 12/2/2020

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        #endregion Update 12/2/2020
        #region #region Update 8/7/2020
        public virtual QuocGias.QuocGia QuocGia { get; set; }
        #endregion

        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeNhanViens
        {
            get => _hopDongKhamSucKhoeNhanViens ?? (_hopDongKhamSucKhoeNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeNhanViens = value;
        }
    }
}
