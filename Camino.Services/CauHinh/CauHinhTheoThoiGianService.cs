using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Data;

namespace Camino.Services.CauHinh
{
    [ScopedDependency(ServiceType = typeof(ICauHinhTheoThoiGianService))]
    public class CauHinhTheoThoiGianService : MasterFileService<CauHinhTheoThoiGian>, ICauHinhTheoThoiGianService
    {
        public CauHinhTheoThoiGianService(IRepository<CauHinhTheoThoiGian> repository) : base(repository)
        { }
    }
}
