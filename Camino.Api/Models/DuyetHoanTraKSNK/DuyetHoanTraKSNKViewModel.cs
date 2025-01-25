using Camino.Core.Domain;
namespace Camino.Api.Models.DuyetHoanTraKSNK
{
    public class DuyetHoanTraKSNKViewModel : BaseViewModel
    {
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public long NguoiTraId { get; set; }
        public long NguoiNhanId { get; set; }
    }
}