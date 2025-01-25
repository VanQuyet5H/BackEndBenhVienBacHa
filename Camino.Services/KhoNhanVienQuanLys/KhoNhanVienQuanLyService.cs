using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Data;

namespace Camino.Services.KhoNhanVienQuanLys
{
    [ScopedDependency(ServiceType = typeof(IKhoNhanVienQuanLyService))]
    public class KhoNhanVienQuanLyService : MasterFileService<KhoNhanVienQuanLy>, IKhoNhanVienQuanLyService
    {
        public KhoNhanVienQuanLyService(IRepository<KhoNhanVienQuanLy> repository) : base(repository)
        { }
    }
}
