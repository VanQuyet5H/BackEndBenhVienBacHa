using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaCLS;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        //Task<GridDataSource> GetDataDanhSachKetLuanCLSKhamSucKhoeDoanForGridAsync(QueryInfo queryInfo);
        //Task<GridDataSource> GetTotalPageDanhSachKetLuanCLSKhamSucKhoeDoanForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncKetQuaCLS(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncKetQuaCLS(QueryInfo queryInfo);

        Task<List<ListKetQuaCLS>> GetListKetQuaCLS(long yeuCauTiepNhanId);
        Task<List<ListKetQuaCLS>> GetListKetQuaCLSNew(long yeuCauTiepNhanId);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachKetLuanCLSKhamSucKhoe(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncDanhSachKetLuanCLSKhamSucKhoe(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncDanhSachChuaKetLuanCLSKhamSucKhoe(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachChuaKetLuanCLSKhamSucKhoe(QueryInfo queryInfo);
        #region check hợp đồng đã kết thúc chưa
        bool CheckHopDongKetThuc(ThongTinCheckHopDong checkHopDong);
        #endregion
    }
}
