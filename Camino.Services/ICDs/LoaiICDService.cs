using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Data;

namespace Camino.Services.ICDs
{
    [ScopedDependency(ServiceType = typeof(ILoaiICDService))]
    public class LoaiICDService : MasterFileService<LoaiICD>, ILoaiICDService
    {
        public LoaiICDService(IRepository<LoaiICD> repository) : base(repository)
        {
            
        }
    }
}
