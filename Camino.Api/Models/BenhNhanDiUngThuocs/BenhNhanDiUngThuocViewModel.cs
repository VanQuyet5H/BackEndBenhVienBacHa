using Camino.Core.Domain;

namespace Camino.Api.Models.BenhNhanDiUngThuocs
{
    public class BenhNhanDiUngThuocViewModel : BaseViewModel
    {
        public long ThuocId { get; set; }

        public string TenDiUng { get; set; }

        public string Ma { get; set; }

        public long BenhNhanId { get; set; }

        public string BieuHienDiUng { get; set; }

        public Enums.LoaiDiUng LoaiDiUng { get; set; }

        public Enums.EnumMucDoDiUng? MucDo { get; set; }
    }

    public class BenhNhanDiUngThuocUiReturnViewModel
    {
        public long Id { get; set; }

        public string LoaiDiUng { get; set; }

        public string TenDiUng { get; set; }
        public string TenMucDo { get; set; }
    }
}
