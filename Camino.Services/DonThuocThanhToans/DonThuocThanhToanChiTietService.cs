using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Data;

namespace Camino.Services.DonThuocThanhToans
{
    [ScopedDependency(ServiceType = typeof(IDonThuocThanhToanChiTietService))]
    public class DonThuocThanhToanChiTietService : MasterFileService<DonThuocThanhToanChiTiet>, IDonThuocThanhToanChiTietService
    {
        public DonThuocThanhToanChiTietService(IRepository<DonThuocThanhToanChiTiet> repository) : base(repository)
        {
        }
    }
}
