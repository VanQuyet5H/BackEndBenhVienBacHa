using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Data;

namespace Camino.Services.YeuCauXuatKhacVatTus
{
    [ScopedDependency(ServiceType = typeof(IYeuCauXuatKhoVatTuChiTietService))]
    public class YeuCauXuatKhoVatTuChiTietService : MasterFileService<YeuCauXuatKhoVatTuChiTiet>, IYeuCauXuatKhoVatTuChiTietService
    {
        public YeuCauXuatKhoVatTuChiTietService(IRepository<YeuCauXuatKhoVatTuChiTiet> repository) : base(repository)
        {
        }
    }
}
