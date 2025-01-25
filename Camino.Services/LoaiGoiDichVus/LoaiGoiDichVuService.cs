using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.LoaiGoiDichVus;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.LoaiGoiDichVus
{


    [ScopedDependency(ServiceType = typeof(ILoaiGoiDichVuService))]

    public class LoaiGoiDichVuService : MasterFileService<LoaiGoiDichVu>, ILoaiGoiDichVuService
    {
        public LoaiGoiDichVuService(IRepository<LoaiGoiDichVu> repository) : base(repository)
        {

        }
    }
}
