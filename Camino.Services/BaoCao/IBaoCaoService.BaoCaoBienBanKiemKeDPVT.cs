using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoBienBanKiemKeDPVT;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoBienBanKiemKeDPVTForGridAsync(BaoCaoBienBanKiemKeDPVTQueryInfo queryInfo, bool exportExcel = false);
        byte[] ExportBaoCaoBienBanKiemKeDPVT(GridDataSource datas, BaoCaoBienBanKiemKeDPVTQueryInfo query);
        Task<List<LookupItemVo>> GetKhoNhanVienLookupAsync(LookupQueryInfo queryInfo);
    }
}
