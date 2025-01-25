using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.NhapKhoVatTus
{
    [ScopedDependency(ServiceType = typeof(IYeuCauNhapKhoVatTuService))]
    public class YeuCauNhapKhoVatTuService : MasterFileService<YeuCauNhapKhoVatTu>, IYeuCauNhapKhoVatTuService
    {
        public YeuCauNhapKhoVatTuService(IRepository<YeuCauNhapKhoVatTu> repository) : base(repository)
        {
        }
    }
}
