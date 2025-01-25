using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;
using Camino.Core.Domain.Entities.XuatKhos;

namespace Camino.Services.YeuCauXuatKhacDuocPhams
{
    [ScopedDependency(ServiceType = typeof(IYeuCauXuatKhoDuocPhamChiTietService))]

    public class YeuCauXuatKhoDuocPhamChiTietService : MasterFileService<YeuCauXuatKhoDuocPhamChiTiet>, IYeuCauXuatKhoDuocPhamChiTietService
    {
        public YeuCauXuatKhoDuocPhamChiTietService(IRepository<YeuCauXuatKhoDuocPhamChiTiet> repository) : base(repository)
        {
        }
    }
}
