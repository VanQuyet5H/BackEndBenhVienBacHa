using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.QuanHeThanNhans
{
    public class QuanHeThanNhan : BaseEntity
    {
        public string Ten { get; set; }

        public string TenVietTat { get; set; }

        public string MoTa { get; set; }

        public bool? IsDisabled { get; set; }

        private ICollection<BenhNhan> _nguoiLienHeQuanHeNhanThanBenhNhans;
        public virtual ICollection<BenhNhan> NguoiLienHeQuanHeNhanThanBenhNhans
        {
            get => _nguoiLienHeQuanHeNhanThanBenhNhans ?? (_nguoiLienHeQuanHeNhanThanBenhNhans = new List<BenhNhan>());
            protected set => _nguoiLienHeQuanHeNhanThanBenhNhans = value;
        }

        //private ICollection<BenhNhan> _nguoiLienHePhuongXaBenhNhans;
        //public virtual ICollection<BenhNhan> NguoiLienHePhuongXaBenhNhans
        //{
        //    get => _nguoiLienHePhuongXaBenhNhans ?? (_nguoiLienHePhuongXaBenhNhans = new List<BenhNhan>());
        //    protected set => _nguoiLienHePhuongXaBenhNhans = value;
        //}

        //private ICollection<BenhNhan> _nguoiLienHeQuanHuyenBenhNhans;
        //public virtual ICollection<BenhNhan> NguoiLienHeQuanHuyenBenhNhans
        //{
        //    get => _nguoiLienHeQuanHuyenBenhNhans ?? (_nguoiLienHeQuanHuyenBenhNhans = new List<BenhNhan>());
        //    protected set => _nguoiLienHeQuanHuyenBenhNhans = value;
        //}

        //private ICollection<BenhNhan> _nguoiLienHeTinhThanhBenhNhans;
        //public virtual ICollection<BenhNhan> NguoiLienHeTinhThanhBenhNhans
        //{
        //    get => _nguoiLienHeTinhThanhBenhNhans ?? (_nguoiLienHeTinhThanhBenhNhans = new List<BenhNhan>());
        //    protected set => _nguoiLienHeTinhThanhBenhNhans = value;
        //}

        #region Update 12/2/2020

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        #endregion Update 12/2/2020
    }
}
