using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;

namespace Camino.Services.KhamDoan.GoiKhamSucKhoes
{
    [ScopedDependency(ServiceType = typeof(IGoiKhamSucKhoeService))]
    public class GoiKhamSucKhoeService : MasterFileService<Core.Domain.Entities.KhamDoans.GoiKhamSucKhoe>, IGoiKhamSucKhoeService
    {
        public GoiKhamSucKhoeService
        (
            IRepository<Core.Domain.Entities.KhamDoans.GoiKhamSucKhoe> repository
        )
            : base(repository)
        {
        }
    }
}
