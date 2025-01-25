using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Data;

namespace Camino.Services.DonThuocThanhToans
{
    [ScopedDependency(ServiceType = typeof(IDonThuocThanhToanService))]
    public class DonThuocThanhToanService : MasterFileService<DonThuocThanhToan>, IDonThuocThanhToanService
    {
        public DonThuocThanhToanService(IRepository<DonThuocThanhToan> repository) : base(repository)
        {
        }
    }
}
