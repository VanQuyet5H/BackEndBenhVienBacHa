using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LoaiThuePhongNoiThucHien;

namespace Camino.Api.Models.MappingProfile
{
    public class LoaiThuePhongNoiThucHienMappingProfile : Profile
    {
        public LoaiThuePhongNoiThucHienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.CauHinhs.LoaiThuePhongNoiThucHien, LoaiThuePhongNoiThucHienViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<LoaiThuePhongNoiThucHienViewModel, Core.Domain.Entities.CauHinhs.LoaiThuePhongNoiThucHien>()
                .IgnoreAllNonExisting();
        }
    }
}
