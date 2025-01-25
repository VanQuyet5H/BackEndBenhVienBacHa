using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.Voucher
{
    public class VoucherDichVuVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ma { get; set; }
        public EnumDichVuTongHop LoaiDichVu { get; set; }
    }
}