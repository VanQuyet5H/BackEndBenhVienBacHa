using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService
    {
        #region Grid
        Task<GridDataSource> GetDataForGridNoiDungMauKhamBenhAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNoiDungMauKhamBenhAsync(QueryInfo queryInfo);

        #endregion

        #region Get data
        Task<NoiDungMauKhamBenh> GetThongTinNoiDungMauKhamBenhAsync(long id, bool isEditData = false);

        #endregion

        #region Xử lý data
        Task XoaThongTinNoiDungMauKhamBenhAsync(long id);
        Task LuuThongTinNoiDungMauKhamBenhAsync(NoiDungMauKhamBenh noiDungMau);
        #endregion
    }
}
