using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.YeuCauTiepNhans
{
    public interface IYeuCauKhamBenhTrieuChungService : IMasterFileService<YeuCauKhamBenhTrieuChung>
    {
        Task<ActionResult<GridDataSource>> GetDataGridYeuCauKhamBenhTrieuChung(long id);

        Task<long> GetIdTask(long yeuCauKhamBenhId, long trieuChungId);
    }
}