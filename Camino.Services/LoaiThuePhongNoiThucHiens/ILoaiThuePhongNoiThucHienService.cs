using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.LoaiThuePhongNoiThucHiens
{
    public interface ILoaiThuePhongNoiThucHienService : IMasterFileService<LoaiThuePhongNoiThucHien>
    {
        #region Grid
        Task<GridDataSource> GetDataForGridNoiThucHienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNoiThucHienAsync(QueryInfo queryInfo);


        #endregion

        #region Check data
        Task<bool> KiemTraTrungTenAsync(long loaiThuePhongNoiThucHienId, string ten);


        #endregion

        #region Get data
        Task<List<LookupItemVo>> GetListLoaiThuePhongNoiThucHienAsync(DropDownListRequestModel model);


        #endregion
    }
}
