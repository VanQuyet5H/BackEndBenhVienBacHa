using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Data;

namespace Camino.Services.ICDs
{
    [ScopedDependency(ServiceType = typeof(IChuongICDService))]
    public class ChuongICDService : MasterFileService<ChuongICD>, IChuongICDService
    {
        public ChuongICDService(IRepository<ChuongICD> repository) : base(repository)
        {
            
        }
    }
}
