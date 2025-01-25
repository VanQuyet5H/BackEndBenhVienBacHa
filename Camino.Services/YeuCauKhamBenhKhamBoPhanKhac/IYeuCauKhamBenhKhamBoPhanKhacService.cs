using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauKhamBenhKhamBoPhanKhac
{
    public interface IYeuCauKhamBenhKhamBoPhanKhacService : IMasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhKhamBoPhanKhac>
    {
        Task<ActionResult<GridDataSource>> GetDataGridBoPhanKhac(long idYCKB);
    }
}
