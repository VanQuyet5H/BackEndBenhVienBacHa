using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhomChucDanhs;

namespace Camino.Core.Domain.Entities.ChucDanhs
{
    public class ChucDanh :BaseEntity
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long NhomChucDanhId { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? IsDefault { get; set; }
        //public  string TenNhomChucDanh { get; set; }

        public virtual NhomChucDanh NhomChucDanh { get; set; }

        private ICollection<NhanVien> _nhanViens;

        public virtual ICollection<NhanVien> NhanViens
        {
            get => _nhanViens ?? (_nhanViens = new List<NhanVien>());
            protected set => _nhanViens = value;
        }
    }
}
