using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ChuanDoanHinhAnh
{
    public class ChuanDoanHinhAnhGridVo : GridItem
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenTiengAnh { get; set; }

        public string LoaiChuanDoanHinhAnhDisplay { get; set; }

        public string MoTa { get; set; }

        public bool HieuLuc { get; set; }
    }
}
