using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.Entities.KetQuaVaKetLuanMaus;

namespace Camino.Api.Models.MappingProfile
{
    public class KetQuaVaKetLuanMauMappingProfile : Profile
    {
        public KetQuaVaKetLuanMauMappingProfile()
        {
            CreateMap<Core.Domain.Entities.KetQuaVaKetLuanMaus.KetQuaVaKetLuanMau, KetQuaVaKetLuanMauViewModel>().IgnoreAllNonExisting();
            CreateMap<KetQuaVaKetLuanMauViewModel, Core.Domain.Entities.KetQuaVaKetLuanMaus.KetQuaVaKetLuanMau>().IgnoreAllNonExisting();
        }
    }
}
