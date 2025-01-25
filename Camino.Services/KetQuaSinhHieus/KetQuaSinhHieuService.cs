using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Data;

namespace Camino.Services.KetQuaSinhHieus
{
    [ScopedDependency(ServiceType = typeof(IKetQuaSinhHieuService))]
    public class KetQuaSinhHieuService
        : MasterFileService<KetQuaSinhHieu>
            , IKetQuaSinhHieuService
    {
        public KetQuaSinhHieuService
        (
            IRepository<KetQuaSinhHieu> repository
        )
            : base(repository)
        {
        }
    }
}
