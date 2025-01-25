using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.LoaiThuePhongPhauThuats
{
    public interface ILoaiThuePhongPhauThuatService : IMasterFileService<LoaiThuePhongPhauThuat>
    {
        #region Grid
        Task<GridDataSource> GetDataForGridCauHinhThuePhongAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridCauHinhThuePhongAsync(QueryInfo queryInfo);


        #endregion

        #region Check data
        Task<bool> KiemTraTrungTenAsync(long loaiThuePhongPhauThuatId, string ten);


        #endregion

        #region Get data
        Task<List<LookupItemVo>> GetListLoaiThuePhongPhauThuatAsync(DropDownListRequestModel model);


        #endregion
    }
}
