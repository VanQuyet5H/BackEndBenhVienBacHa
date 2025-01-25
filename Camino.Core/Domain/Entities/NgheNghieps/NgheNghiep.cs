using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.NgheNghieps
{
    public class NgheNghiep : BaseEntity
    {
        public string Ten { get; set; }

        public string TenVietTat { get; set; }

        public string MoTa { get; set; }

        public bool? IsDisabled { get; set; }

        private ICollection<BenhNhan> _benhNhans;
        public virtual ICollection<BenhNhan> BenhNhans
        {
            get => _benhNhans ?? (_benhNhans = new List<BenhNhan>());
            protected set => _benhNhans = value;
        }

        #region Update 12/2/2020

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        #endregion Update 12/2/2020


        #region nghề nghiệp của bố, mẹ
        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNgheNghiepCuaBos;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNgheNghiepCuaBos
        {
            get => _yeuCauTiepNhanNgheNghiepCuaBos ?? (_yeuCauTiepNhanNgheNghiepCuaBos = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNgheNghiepCuaBos = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNgheNghiepCuaMes;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNgheNghiepCuaMes
        {
            get => _yeuCauTiepNhanNgheNghiepCuaMes ?? (_yeuCauTiepNhanNgheNghiepCuaMes = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNgheNghiepCuaMes = value;
        }
        #endregion

        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeNhanViens
        {
            get => _hopDongKhamSucKhoeNhanViens ?? (_hopDongKhamSucKhoeNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeNhanViens = value;
        }
    }
}