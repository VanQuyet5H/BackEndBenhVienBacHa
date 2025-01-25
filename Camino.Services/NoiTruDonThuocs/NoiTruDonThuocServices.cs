using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Data;

namespace Camino.Services.NoiTruDonThuocs
{
    [ScopedDependency(ServiceType = typeof(INoiTruDonThuocServices))]

    public class NoiTruDonThuocServices : MasterFileService<NoiTruDonThuoc>, INoiTruDonThuocServices
    {
        public NoiTruDonThuocServices(IRepository<NoiTruDonThuoc> repository) : base(repository)
        {

        }
    }
}
