using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Data;

namespace Camino.Services.YeuCauDuocPhams
{
    [ScopedDependency(ServiceType = typeof(IYeuCauDuocPhamService))]
    public class YeuCauDuocPhamService : MasterFileService<YeuCauDuocPhamBenhVien>,
        IYeuCauDuocPhamService
    {
        public YeuCauDuocPhamService(IRepository<YeuCauDuocPhamBenhVien> repository)
            : base(repository)
        {
        }
    }
}



