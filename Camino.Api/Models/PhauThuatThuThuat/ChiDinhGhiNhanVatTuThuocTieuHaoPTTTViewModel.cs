using System.ComponentModel.DataAnnotations.Schema;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class ChiDinhGhiNhanVatTuThuocTieuHaoPTTTViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long DichVuChiDinhId { get; set; }
        public long? KhoId { get; set; }
        public string DichVuGhiNhanId { get; set; }
        public double? SoLuong { get; set; }
        public bool? TinhPhi { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public EnumGiaiDoanPhauThuat GiaiDoanPhauThuat { get; set; }
        public LoaiNoiChiDinh LoaiNoiChiDinh { get; set; }
        [NotMapped]
        public string strDonViTinh { get; set; }
    }
}
