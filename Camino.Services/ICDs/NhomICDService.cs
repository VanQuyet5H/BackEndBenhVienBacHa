using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Data;

namespace Camino.Services.ICDs
{
    [ScopedDependency(ServiceType = typeof(INhomICDService))]
    public class NhomICDService : MasterFileService<NhomICD>, INhomICDService
    {
        public NhomICDService(IRepository<NhomICD> repository) : base(repository)
        {
            
        }
    }
}
