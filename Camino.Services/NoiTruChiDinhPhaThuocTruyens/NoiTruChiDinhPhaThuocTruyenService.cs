using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Data;

namespace Camino.Services.NoiTruChiDinhPhaThuocTruyens
{
    [ScopedDependency(ServiceType = typeof(INoiTruChiDinhPhaThuocTruyenService))]
    public class NoiTruChiDinhPhaThuocTruyenService : MasterFileService<NoiTruChiDinhPhaThuocTruyen>, INoiTruChiDinhPhaThuocTruyenService
    {
        public NoiTruChiDinhPhaThuocTruyenService(IRepository<NoiTruChiDinhPhaThuocTruyen> repository) : base(repository)
        {

        }
    }
}
