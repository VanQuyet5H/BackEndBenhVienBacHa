using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.YeuCauDuTruDuocPhamTheoKhoa
{
    [ScopedDependency(ServiceType = typeof(IYeuCauDuTruDuocPhamTheoKhoaService))]
    public class YeuCauDuTruDuocPhamTheoKhoaService : MasterFileService<DuTruMuaDuocPhamTheoKhoa>, IYeuCauDuTruDuocPhamTheoKhoaService
    {
        public YeuCauDuTruDuocPhamTheoKhoaService(IRepository<DuTruMuaDuocPhamTheoKhoa> repository)
            : base(repository)
        {
        }
    }
   
}
