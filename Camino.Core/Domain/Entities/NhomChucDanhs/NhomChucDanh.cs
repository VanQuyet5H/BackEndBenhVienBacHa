using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ChucDanhs;

namespace Camino.Core.Domain.Entities.NhomChucDanhs
{
    public class NhomChucDanh : BaseEntity
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string MoTa { get; set; }
        //public Enums.EnumNhomChucDanh? Loai { get; set; }

        private ICollection<ChucDanh> _chucDanhs;

        public virtual ICollection<ChucDanh> ChucDanhs
        {
            get => _chucDanhs ?? (_chucDanhs = new List<ChucDanh>());
            protected set => _chucDanhs = value;
        }
    }
}
