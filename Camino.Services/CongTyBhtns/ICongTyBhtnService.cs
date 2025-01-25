using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.CongTyBhtns
{
    public interface ICongTyBhtnService : IMasterFileService<CongTyBaoHiemTuNhan>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsMaExists(string ma = null, long id = 0);

        Task<bool> IsTenExists(string ten = null, long id = 0);

        List<LookupItemVo> GetHinhThucBaoHiem();

        List<LookupItemVo> GetPhamViBaoHiem();
    }
}
