using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DonViTinh;
using Camino.Core.Domain.ValueObject.DonViTinh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class DonViTinhMappingProfile :Profile
    {
        public DonViTinhMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DonViTinhs.DonViTinh, DonViTinhViewModel>();
            CreateMap<DonViTinhViewModel, Core.Domain.Entities.DonViTinhs.DonViTinh>();
            CreateMap<DonViTinhGridVo, ExportDonViTinhExportExcel>()
             .IgnoreAllNonExisting();
        }
    }
}
