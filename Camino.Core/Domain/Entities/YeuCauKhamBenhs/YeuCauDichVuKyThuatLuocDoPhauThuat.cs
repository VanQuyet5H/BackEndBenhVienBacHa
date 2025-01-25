namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauDichVuKyThuatLuocDoPhauThuat : BaseEntity
    {
        public long YeuCauDichVuKyThuatTuongTrinhPTTTId { get; set; }

        public string LuocDo { get; set; }

        public string MoTa { get; set; }

        public virtual YeuCauDichVuKyThuatTuongTrinhPTTT YeuCauDichVuKyThuatTuongTrinhPTTT { get; set; }
    }
}
