using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Data;

namespace Camino.Services.NoiTruDonThuocs
{
    [ScopedDependency(ServiceType = typeof(INoiTruDonThuocChiTietServices))]
    public class NoiTruDonThuocChiTietServices : MasterFileService<NoiTruDonThuocChiTiet>, INoiTruDonThuocChiTietServices
    {
        public NoiTruDonThuocChiTietServices(IRepository<NoiTruDonThuocChiTiet> repository) : base(repository)
        {
        }
    }
}
