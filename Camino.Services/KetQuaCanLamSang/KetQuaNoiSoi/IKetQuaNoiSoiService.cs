using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.KetQuaCanLamSang.KetQuaNoiSoi
{
    public interface IKetQuaNoiSoiService : IMasterFileService<KetQuaSinhHieu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
    }
}
