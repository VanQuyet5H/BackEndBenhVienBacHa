using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Data;
namespace Camino.Services.YeuCauKhamBenh
{

    [ScopedDependency(ServiceType = typeof(IYeuCauKhamBenhICDKhacService))]
    public class YeuCauKhamBenhICDKhacService
       : MasterFileService<YeuCauKhamBenhICDKhac>
           , IYeuCauKhamBenhICDKhacService
    {
        public YeuCauKhamBenhICDKhacService
        (
            IRepository<YeuCauKhamBenhICDKhac> repository
        )
            : base(repository)
        {
        }
    }
}
