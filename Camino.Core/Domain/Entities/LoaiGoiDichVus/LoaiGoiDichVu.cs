using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.LoaiGoiDichVus
{
    public class LoaiGoiDichVu : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public bool IsDefault { get; set; }

        private ICollection<ChuongTrinhGoiDichVu> _chuongTrinhGoiDichVus { get; set; }
        public virtual ICollection<ChuongTrinhGoiDichVu> ChuongTrinhGoiDichVus
        {
            get => _chuongTrinhGoiDichVus ?? (_chuongTrinhGoiDichVus = new List<ChuongTrinhGoiDichVu>());
            protected set => _chuongTrinhGoiDichVus = value;
        }
    }
}
