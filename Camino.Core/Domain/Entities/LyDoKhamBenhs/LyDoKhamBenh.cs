using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.Entities.LyDoKhamBenhs
{
    public class LyDoKhamBenh : BaseEntity
    {
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }

        //private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        //public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        //{
        //    get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
        //    protected set => _yeuCauKhamBenhs = value;
        //}
    }
}
