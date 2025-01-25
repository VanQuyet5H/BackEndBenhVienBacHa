using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.NhomChucDanh
{
    public interface INhomChucDanhService : IMasterFileService<Core.Domain.Entities.NhomChucDanhs.NhomChucDanh>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<bool> IsTenExists(string ten, long id = 0);
        Task<bool> IsMaExists(string ma, long id = 0);
    }
}
