using System.Collections.Generic;

namespace Camino.Api.Models.YeuCauTiepNhan
{
    public class ChoXacNhanConfirmViewModel
    {
        public List<DanhSachChoXacNhanBHYTViewModel> listDataChoXacNhan { get; set; }

        public long idTiepNhan { get; set; }
    }
}
