using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.CauHinhThuePhong
{
    public interface ICauHinhThuePhongService: IMasterFileService<Core.Domain.Entities.CauHinhs.CauHinhThuePhong>
    {
        #region Grid
        Task<GridDataSource> GetDataForGridCauHinhThuePhongAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridCauHinhThuePhongAsync(QueryInfo queryInfo);


        #endregion

        #region Get data
        Task<List<LookupItemVo>> GetListCauHinhThuePhong(DropDownListRequestModel model);


        #endregion

        #region check data
        Task<bool> KiemTraTrungTenAsync(long cauHinhThuePhongId, string ten);


        #endregion
    }
}
