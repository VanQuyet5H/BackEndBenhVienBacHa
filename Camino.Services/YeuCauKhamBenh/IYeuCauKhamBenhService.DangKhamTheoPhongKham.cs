using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService
    {
        #region Grid
        Task<GridDataSource> GetDataForGridKhamBenhDangKhamAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridKhamBenhDangKhamAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridKhamBenhDangKhamTheoPhongKhamAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridKhamBenhDangKhamTheoPhongKhamAsync(QueryInfo queryInfo);

        #endregion

        #region get list
        Task<List<LookupItemTemplateVo>> GetListPhongBenhVienAsync(DropDownListRequestModel model);
        Task<List<LookupItemTemplateVo>> GetListKhoaBenhVienAsync(DropDownListRequestModel model);


        #endregion

        #region get data
        Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamAsync(long phongBenhVienHangDoiId);
        Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamLuuTabKhamBenhAsync(long phongBenhVienHangDoiId);
        Task KiemTraDatayeuCauKhamBenhDangKhamAsync(long yeuCauKhamBenhId);

        #endregion
    }
}
