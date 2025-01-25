using System.Collections.Generic;

namespace Camino.Api.Models.YeuCauTiepNhan
{
    public class DaXacNhanConfirmViewModel
    {
        public List<DanhSachChoXacNhanBHYTViewModel> listDataDaXacNhan { get; set; }

        public long idTiepNhan { get; set; }
    }
}
