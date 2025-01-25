using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class NoiDungMauKhamBenh : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string NoiDung { get; set; }
        public long BacSiId { get; set; }
        public virtual NhanVien BacSi { get; set; }
    }
}
