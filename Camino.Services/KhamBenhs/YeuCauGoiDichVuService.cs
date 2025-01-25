using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Data;


namespace Camino.Services.KhamBenhs
{
    [ScopedDependency(ServiceType = typeof(IYeuCauGoiDichVuService))]
    public class YeuCauGoiDichVuService : MasterFileService<YeuCauGoiDichVu>, IYeuCauGoiDichVuService
    {
        public YeuCauGoiDichVuService(IRepository<YeuCauGoiDichVu> repository) : base(repository)
        {
        }
    }
}
