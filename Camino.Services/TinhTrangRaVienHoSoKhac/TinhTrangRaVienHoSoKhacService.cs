using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.TinhTrangRaVienHoSoKhac
{
    [ScopedDependency(ServiceType = typeof(ITinhTrangRaVienHoSoKhacService))]
    public class TinhTrangRaVienHoSoKhacService : MasterFileService<Core.Domain.Entities.TinhTrangRaVienHoSoKhacs.TinhTrangRaVienHoSoKhac>, ITinhTrangRaVienHoSoKhacService
    {
        public TinhTrangRaVienHoSoKhacService(IRepository<Core.Domain.Entities.TinhTrangRaVienHoSoKhacs.TinhTrangRaVienHoSoKhac> repository) : base(repository)
        {
        }
    }
}
