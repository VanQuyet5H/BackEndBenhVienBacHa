using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.CauHinhs
{
    public class CauHinhThapGia : BaseEntity
    {
        public Enums.LoaiThapGia LoaiThapGia { get; set; }
        public decimal? GiaTu { get; set; }
        public decimal? GiaDen { get; set; }
        public int TiLeTheoThapGia { get; set; }
    }
}
