using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.MauVaChePham
{
    public interface IMauVaChePhamService : IMasterFileService<Core.Domain.Entities.MauVaChePhams.MauVaChePham>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo,bool isPrint);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        List<LookupItemVo> GetListPhanLoaiMau(LookupQueryInfo queryInfo);
        Task<bool> IsTenExists(string ten, long id = 0);
        Task<bool> IsMaExists(string ma, long id = 0);
    }
}
