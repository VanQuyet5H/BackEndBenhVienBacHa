using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.ChuanDoanHinhAnhs
{
    public class ChuanDoanHinhAnh : BaseEntity
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenTiengAnh { get; set; }

        public Enums.EnumLoaiChuanDoanHinhAnh LoaiChuanDoanHinhAnh { get; set; }

        public string MoTa { get; set; }

        public bool HieuLuc { get; set; }

        private ICollection<KetQuaChuanDoanHinhAnh> _ketQuaChuanDoanHinhAnhs;
        public virtual ICollection<KetQuaChuanDoanHinhAnh> KetQuaChuanDoanHinhAnhs
        {
            get => _ketQuaChuanDoanHinhAnhs ?? (_ketQuaChuanDoanHinhAnhs = new List<KetQuaChuanDoanHinhAnh>());
            protected set => _ketQuaChuanDoanHinhAnhs = value;

        }
    }
}
