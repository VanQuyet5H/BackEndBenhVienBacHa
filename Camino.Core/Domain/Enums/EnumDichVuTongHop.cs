using System.ComponentModel;

namespace Camino.Core.Domain
{

    public partial class Enums
    {
        public enum EnumDichVuTongHop
        {
            [Description("Dịch vụ khám bệnh")]
            KhamBenh = 1,
            [Description("Dịch vụ kỹ thuật")]
            KyThuat = 2,
            [Description("Dịch vụ giường bệnh")]
            GiuongBenh = 3,
        }
        public enum LoaiDuocPhamHoacVatTu
        {
            [Description("Dược phẩm bệnh viện")]
            DuocPhamBenhVien = 1,
            [Description("Vật tư bệnh viện")]
            VatTuBenhVien = 2
        }
    }
}