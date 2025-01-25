using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.NoiDuTruHoSoKhac
{
    [ScopedDependency(ServiceType = typeof(INoiDuTruHoSoKhacService))]
    public class NoiDuTruHoSoKhacService : MasterFileService<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>, INoiDuTruHoSoKhacService
    {
        public NoiDuTruHoSoKhacService(IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac> repository) : base(repository)
        {
        }
    }
}
