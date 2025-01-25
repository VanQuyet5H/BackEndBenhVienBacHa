using Camino.Api.Models.PhongBenhVien;
using Camino.Core.Domain;
using Camino.Api.Models.KhamBenh;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class ThongTinBoPhanCoTheModel : BaseViewModel
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }     
        public string Url { get; set; }
    }
}
