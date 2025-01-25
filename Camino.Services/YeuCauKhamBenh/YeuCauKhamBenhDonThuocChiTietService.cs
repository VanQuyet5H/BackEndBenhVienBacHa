using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Services.YeuCauKhamBenh
{
    [ScopedDependency(ServiceType = typeof(IYeuCauKhamBenhDonThuocChiTietService))]
    public class YeuCauKhamBenhDonThuocChiTietService : MasterFileService<YeuCauKhamBenhDonThuocChiTiet>, IYeuCauKhamBenhDonThuocChiTietService
    {
        public YeuCauKhamBenhDonThuocChiTietService(IRepository<YeuCauKhamBenhDonThuocChiTiet> repository) : base(repository)
        {
        }
    }
}
