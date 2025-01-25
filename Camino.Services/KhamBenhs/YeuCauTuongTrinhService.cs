using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Data;

namespace Camino.Services.KhamBenhs
{
    [ScopedDependency(ServiceType = typeof(IYeuCauTuongTrinhService))]
    public class YeuCauTuongTrinhService : MasterFileService<YeuCauDichVuKyThuatTuongTrinhPTTT>, IYeuCauTuongTrinhService
    {
        public YeuCauTuongTrinhService(IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> repository) : base(repository)
        {
        }
    }
}
