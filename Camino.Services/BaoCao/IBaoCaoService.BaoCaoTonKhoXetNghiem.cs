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
        #region báo cáo tồn kho xét nghiệm

        List<LookupItemVo> GetTatCaKhoTheoKhoaXetNghiems(DropDownListRequestModel queryInfo);   
        Task<GridDataSource> GetDataBaoCaoTonKhoXetNghiemForGridAsync(BaoCaoTonKhoXetNghiemQueryInfo queryInfo);
        byte[] ExportBaoCaoTonKhoXetNghiemGridVo(GridDataSource gridDataSource, BaoCaoTonKhoXetNghiemQueryInfo query);
      
        #endregion
    }
}
