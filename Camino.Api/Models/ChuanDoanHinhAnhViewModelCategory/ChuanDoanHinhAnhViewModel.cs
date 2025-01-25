using Camino.Core.Domain;

namespace Camino.Api.Models.ChuanDoanHinhAnhViewModelCategory
{
    public class ChuanDoanHinhAnhViewModel : BaseViewModel
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenTiengAnh { get; set; }

        public Enums.EnumLoaiChuanDoanHinhAnh LoaiChuanDoanHinhAnh { get; set; }

        public string LoaiChuanDoanHinhAnhDisplay { get; set; }

        public bool HieuLuc { get; set; }

        public string MoTa { get; set; }
    }
}
