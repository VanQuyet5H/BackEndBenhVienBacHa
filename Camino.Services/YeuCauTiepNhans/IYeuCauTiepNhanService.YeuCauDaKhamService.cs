using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial interface IYeuCauTiepNhanService
    {

        Task<GridDataSource> GetDataForGridAsyncDanhSachDaKham(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachDaKham(QueryInfo queryInfo);
      
        Task<List<LookupItemVo>> GetDoiTuongs(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetLyDos(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetTinhTrangKhamBenhs();
        string SetSoPhanTramHuongBHYT(string maThe);
    }
}
