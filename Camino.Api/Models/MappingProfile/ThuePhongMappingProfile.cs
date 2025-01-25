using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;

namespace Camino.Api.Models.MappingProfile
{
    public class ThuePhongMappingProfile : Profile
    {
        public ThuePhongMappingProfile()
        {
            CreateMap<LichSuThuePhongGridVo, LichSuThuePhongExportExcel>().IgnoreAllNonExisting();
        }
    }
}
