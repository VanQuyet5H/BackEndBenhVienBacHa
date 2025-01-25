using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DonViMaus;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.DonViMaus
{
    [ScopedDependency(ServiceType = typeof(IDonViMauService))]

    public class DonViMauService : MasterFileService<DonViMau>, IDonViMauService
    {
        public DonViMauService(IRepository<DonViMau> repository) : base(repository)
        {

        }
    }
}
