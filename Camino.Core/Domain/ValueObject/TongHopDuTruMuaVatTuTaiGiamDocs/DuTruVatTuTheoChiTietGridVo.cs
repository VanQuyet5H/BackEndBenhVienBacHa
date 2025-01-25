using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.TongHopDuTruMuaVatTuTaiGiamDocs
{
    public class DuTruVatTuTheoChiTietGridVo : GridItem
    {
        public long KhoaId { get; set; }

        public string Khoa { get; set; }

        public string Kho { get; set; }

        public string KyDuTruDisplay { get; set; }

        public int SoLuongDuTru { get; set; }

        public int SoLuongDuKienTrongKy { get; set; }
    }
}
