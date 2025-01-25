using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Data;

namespace Camino.Services.KhamBenhs
{
    [ScopedDependency(ServiceType = typeof(IYeuCauVatTuBenhVienService))]
    public class YeuCauVatTuBenhVienService : MasterFileService<YeuCauVatTuBenhVien>, IYeuCauVatTuBenhVienService
    {
        public YeuCauVatTuBenhVienService(IRepository<YeuCauVatTuBenhVien> repository) : base(repository)
        {
        }
    }
}
