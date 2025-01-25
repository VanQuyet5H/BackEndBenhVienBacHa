using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCao
{

    public class BaoCaoBenhNhanLamXetNghiemGridVo : GridItem
    {
        public int STT { get; set; }
        public string MaBN { get; set; }
        public string MaBarcode { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string NamSinh { get; set; }
        public string DoiTuong => LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
            ? LoaiYeuCauTiepNhan.GetDescription()
            : (MucHuongBHYT != 0 ? $"BHYT ({MucHuongBHYT}%)" : "Viện phí");
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public int MucHuongBHYT { get; set; }
        public string DiaChi { get; set; }
    }
}