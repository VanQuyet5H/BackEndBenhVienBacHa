using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        #region báo cáo biên bản kiểm kê - kế toán

        Task<List<LookupItemVo>> GetTatCaKhoaBaoCaoKiemKeKTs(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetTatCaKhoTheoKhoaBaoCaoKiemKeKTs(DropDownListRequestModel queryInfo, long khoaId);
        Task<GridDataSource> GetDataBaoCaoBienBanKiemKeKTForGridAsync(BaoCaoBienBanKiemKeKTQueryInfo queryInfo, bool exportExcel = false);
        byte[] ExportBaoCaoBienBanKiemKeKTGridVo(GridDataSource gridDataSource, BaoCaoBienBanKiemKeKTQueryInfo query);
        byte[] ExportBaoCaoBienBanKiemKe28092021GridVo(BaoCaoBienBanKiemKeKTQueryInfo query);

        #endregion
    }
}
