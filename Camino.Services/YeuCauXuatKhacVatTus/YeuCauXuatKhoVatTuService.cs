using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Data;
namespace Camino.Services.YeuCauXuatKhacVatTus
{
    [ScopedDependency(ServiceType = typeof(IYeuCauXuatKhoVatTuService))]

    public class YeuCauXuatKhoVatTuService : MasterFileService<YeuCauXuatKhoVatTu>, IYeuCauXuatKhoVatTuService
    {
        public YeuCauXuatKhoVatTuService(IRepository<YeuCauXuatKhoVatTu> repository) : base(repository)
        {
        }
    }
}
