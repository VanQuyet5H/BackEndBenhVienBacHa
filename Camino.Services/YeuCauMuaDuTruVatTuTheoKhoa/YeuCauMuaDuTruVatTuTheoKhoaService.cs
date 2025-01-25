using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.YeuCauMuaDuTruVatTuTheoKhoa
{
    [ScopedDependency(ServiceType = typeof(IYeuCauMuaDuTruVatTuTheoKhoaService))]
    public class YeuCauMuaDuTruVatTuTheoKhoaService : MasterFileService<DuTruMuaVatTuTheoKhoa>, IYeuCauMuaDuTruVatTuTheoKhoaService
    {
        public YeuCauMuaDuTruVatTuTheoKhoaService(IRepository<DuTruMuaVatTuTheoKhoa> repository)
            : base(repository)
        {
        }
    }
}
