using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.DoiTuongUuTienKhamChuaBenhs
{
    public class DoiTuongUuTienKhamChuaBenh : BaseEntity
    {
        public string Ten { get; set; }

        public string TenVietTat { get; set; }
        public int ThuTuUuTien { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
        //private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        //public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        //{
        //    get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
        //    protected set => _yeuCauKhamBenhs = value;
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