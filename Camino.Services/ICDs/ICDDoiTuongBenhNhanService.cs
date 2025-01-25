using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Data;

namespace Camino.Services.ICDs
{
    [ScopedDependency(ServiceType = typeof(IICDDoiTuongBenhNhanService))]
    public class ICDDoiTuongBenhNhanService : MasterFileService<ICDDoiTuongBenhNhan>, IICDDoiTuongBenhNhanService
    {
        public ICDDoiTuongBenhNhanService(IRepository<ICDDoiTuongBenhNhan> repository) : base(repository)
        {
            
        }
    }
}
