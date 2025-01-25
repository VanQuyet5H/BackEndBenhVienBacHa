using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class HoanTraVatTuThuocVo
    {
        public string Id { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauBenhVienId { get; set; }
        public long KhoId { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public EnumNhomGoiDichVu NhomYeuCauId { get; set; }
    }
}