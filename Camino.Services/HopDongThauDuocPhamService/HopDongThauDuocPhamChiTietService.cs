using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Data;

namespace Camino.Services.HopDongThauDuocPhamService
{
    [ScopedDependency(ServiceType = typeof(IHopDongThauDuocPhamChiTietService))]
    public class HopDongThauDuocPhamChiTietService
                : MasterFileService<HopDongThauDuocPhamChiTiet>
                , IHopDongThauDuocPhamChiTietService
    {
        public HopDongThauDuocPhamChiTietService(
            IRepository<HopDongThauDuocPhamChiTiet> repository
        )
               : base(repository)
        {
        }
    }
}
