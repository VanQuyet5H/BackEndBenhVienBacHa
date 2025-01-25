using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KyDuTru;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KyDuTru;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class KyDuTruMappingProfile : Profile
    {
        public KyDuTruMappingProfile()
        {
            CreateMap<KyDuTruMuaDuocPhamVatTu, KyDuTruViewModel>().IgnoreAllNonExisting();
            CreateMap<KyDuTruViewModel, KyDuTruMuaDuocPhamVatTu>().IgnoreAllNonExisting();

            CreateMap<KyDuTruGridVo, KyDuTruExportExcel>().IgnoreAllNonExisting();
        }
    }
}
