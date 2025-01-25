using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Data;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.YeuCauKhamBenh
{
    [ScopedDependency(ServiceType = typeof(IPhongBenhVienHangDoiService))]
    public class PhongBenhVienHangDoiService : MasterFileService<PhongBenhVienHangDoi>, IPhongBenhVienHangDoiService
    {
        public PhongBenhVienHangDoiService(IRepository<PhongBenhVienHangDoi> repository) : base(repository)
        {

        }
    }
}
