using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;

namespace Camino.Services.KetQuaCanLamSang.KetQuaNoiSoi
{
    [ScopedDependency(ServiceType = typeof(IKetQuaNoiSoiService))]
    public class KetQuaNoiSoiService
        : MasterFileService<KetQuaSinhHieu>
            , IKetQuaNoiSoiService
    {
        public KetQuaNoiSoiService
        (
            IRepository<KetQuaSinhHieu> repository
        )
            : base(repository)
        {
        }

        public Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}
