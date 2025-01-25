using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Data;

namespace Camino.Services.DuyetBaoHiems
{
    [ScopedDependency(ServiceType = typeof(IDuyetBaoHiemService))]
    public class DuyetBaoHiemService : MasterFileService<DuyetBaoHiem>, IDuyetBaoHiemService
    {
        public DuyetBaoHiemService(IRepository<DuyetBaoHiem> repository) : base(repository)
        {
        }
    }
}
