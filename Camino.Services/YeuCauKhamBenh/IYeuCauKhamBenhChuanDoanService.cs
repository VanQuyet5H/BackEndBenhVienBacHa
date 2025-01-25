using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.YeuCauKhamBenh
{
    public interface IYeuCauKhamBenhChuanDoanService : IMasterFileService<YeuCauKhamBenhChuanDoan>
    {
        Task<ActionResult<GridDataSource>> GetDataGridYeuCauKhamBenhChuanDoan(long id);

        Task<long> GetIdTask(long yeuCauKhamBenhId, long chuanDoanId);
    }
}