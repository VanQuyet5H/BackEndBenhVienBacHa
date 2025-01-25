using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.HamGuiHoSoWatchings;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.HamGuiHoSoWatchings
{
    [ScopedDependency(ServiceType = typeof(IHamGuiHoSoWatchingService))]
    public class HamGuiHoSoWatchingService : MasterFileService<HamGuiHoSoWatching>, IHamGuiHoSoWatchingService
    {
        public HamGuiHoSoWatchingService(IRepository<HamGuiHoSoWatching> repository) : base(repository)
        {

        }
    }
}
