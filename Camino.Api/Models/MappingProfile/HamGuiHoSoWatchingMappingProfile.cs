using AutoMapper;
using Camino.Api.Models.HamGuiHoSoWatchings;
using Camino.Core.Domain.Entities.HamGuiHoSoWatchings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class HamGuiHoSoWatchingMappingProfile : Profile
    {
        public HamGuiHoSoWatchingMappingProfile()
        {
            CreateMap<HamGuiHoSoWatching, HamGuiHoSoWatchingViewModel>();
            CreateMap<HamGuiHoSoWatchingViewModel, HamGuiHoSoWatching>();
        }
    }
}
