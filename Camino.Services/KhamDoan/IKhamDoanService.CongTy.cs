using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore.Query;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task<GridDataSource> GetDataListCongTyForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageListCongTyForGridAsync(QueryInfo queryInfo);

        Task<CongTyKhamSucKhoe> GetByCongTyIdAsync(long id, Func<IQueryable<CongTyKhamSucKhoe>, IIncludableQueryable<CongTyKhamSucKhoe, object>> includes = null);

        Task AddCongTyAsync(CongTyKhamSucKhoe entity);

        Task UpdateCongTyAsync(CongTyKhamSucKhoe entity);

        Task DeleteByCongTyIdAsync(long id);
    }
}
