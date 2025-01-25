using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;

namespace Camino.Services.NoiTruBenhAn
{
    [ScopedDependency(ServiceType = typeof(INoiTruBenhAnService))]

    public class NoiTruBenhAnService : MasterFileService<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn>, INoiTruBenhAnService
    {
        public NoiTruBenhAnService(IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> repository) : base(repository)
        {
          
        }
    }
}
