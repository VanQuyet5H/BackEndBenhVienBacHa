using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Data;

namespace Camino.Services.DuyetBaoHiems
{
    [ScopedDependency(ServiceType = typeof(IDuyetBaoHiemChiTietService))]
    public class DuyetBaoHiemChiTietService : MasterFileService<DuyetBaoHiemChiTiet>, IDuyetBaoHiemChiTietService
    {
        public DuyetBaoHiemChiTietService(IRepository<DuyetBaoHiemChiTiet> repository) : base(repository)
        {
        }
    }
}
