using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;
using Camino.Core.Domain.Entities.XuatKhos;

namespace Camino.Services.YeuCauXuatKhacDuocPhams
{
    [ScopedDependency(ServiceType = typeof(IYeuCauXuatKhoDuocPhamService))]
    public class YeuCauXuatKhoDuocPhamService : MasterFileService<YeuCauXuatKhoDuocPham>, IYeuCauXuatKhoDuocPhamService
    {
        public YeuCauXuatKhoDuocPhamService(IRepository<YeuCauXuatKhoDuocPham> repository) : base(repository)
        {
        }
    }
}
