using System;

namespace Camino.Core.Domain.Entities
{
    public class BaseLichSuEntity : BaseEntity
    {
        public string GoiDen { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayGui { get; set; }
        public Enums.TrangThaiLishSu TrangThai { get; set; }
    }
}



