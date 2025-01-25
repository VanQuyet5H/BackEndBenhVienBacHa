using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;

namespace Camino.Services.KhamDoan.GoiKhamSucKhoes
{
    [ScopedDependency(ServiceType = typeof(IGoiKhamSucKhoeChungService))]
    public class GoiKhamSucKhoeChungService : MasterFileService<Core.Domain.Entities.KhamDoans.GoiKhamSucKhoeChung>,
        IGoiKhamSucKhoeChungService
    {
        public GoiKhamSucKhoeChungService
        (
            IRepository<Core.Domain.Entities.KhamDoans.GoiKhamSucKhoeChung> repository
        )
            : base(repository)
        {
        }
    }
}
