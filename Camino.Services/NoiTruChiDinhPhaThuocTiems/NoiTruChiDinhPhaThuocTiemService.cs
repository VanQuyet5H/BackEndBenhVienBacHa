using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Data;

namespace Camino.Services.NoiTruChiDinhPhaThuocTiems
{
    [ScopedDependency(ServiceType = typeof(INoiTruChiDinhPhaThuocTiemService))]
    public class NoiTruChiDinhPhaThuocTiemService : MasterFileService<NoiTruChiDinhPhaThuocTiem>, INoiTruChiDinhPhaThuocTiemService
    {
        public NoiTruChiDinhPhaThuocTiemService(IRepository<NoiTruChiDinhPhaThuocTiem> repository) : base(repository)
        {

        }
    }
}
