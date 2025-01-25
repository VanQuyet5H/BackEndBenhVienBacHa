using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.ICDs
{
    [ScopedDependency(ServiceType = typeof(IChuanDoanLienKetICDService))]
    public class ChuanDoanLienKetICDService : MasterFileService<ChuanDoanLienKetICD>, IChuanDoanLienKetICDService
    {
        public ChuanDoanLienKetICDService(IRepository<ChuanDoanLienKetICD> repository) : base(repository)
        {
            
        }
    }
}
