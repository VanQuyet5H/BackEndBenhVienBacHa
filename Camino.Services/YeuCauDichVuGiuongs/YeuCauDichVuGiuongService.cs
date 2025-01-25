using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Data;

namespace Camino.Services.YeuCauDichVuGiuongs
{
    [ScopedDependency(ServiceType = typeof(IYeuCauDichVuGiuongService))]
    public class YeuCauDichVuGiuongService : MasterFileService<YeuCauDichVuGiuongBenhVien>,
        IYeuCauDichVuGiuongService
    {
        public YeuCauDichVuGiuongService(IRepository<YeuCauDichVuGiuongBenhVien> repository)
            : base(repository)
        {
        }
    }
}

