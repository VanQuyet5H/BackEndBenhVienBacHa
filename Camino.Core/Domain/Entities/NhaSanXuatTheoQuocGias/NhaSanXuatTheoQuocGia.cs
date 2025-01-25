using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NhaSanXuatTheoQuocGias
{
    public class NhaSanXuatTheoQuocGia :BaseEntity
    {
        public long NhaSanXuatId { get; set; }
        public long QuocGiaId { get; set; }
        public string DiaChi { get; set; }
        public virtual  NhaSanXuats.NhaSanXuat NhaSanXuat { get; set; }
        public virtual QuocGias.QuocGia QuocGia { get; set; }

    }
}
