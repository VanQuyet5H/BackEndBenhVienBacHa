using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Domain.ValueObject.XetNghiems;

namespace Camino.Api.Models.MappingProfile
{
    public class KetNoiMayXetNghiemMappingProfile : Profile
    {
        public KetNoiMayXetNghiemMappingProfile()
        {
            CreateMap<MayXetNghiem, MayXetNghiemVo>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TenMauMayXetNghiem = s.MauMayXetNghiem.Ten;
                });
            CreateMap<MayXetNghiemVo, MayXetNghiem>().IgnoreAllNonExisting()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Ma, o => o.Ignore())
                .ForMember(x => x.Ten, o => o.Ignore())
                .ForMember(x => x.MauMayXetNghiemID, o => o.Ignore());
        }
    }
}
