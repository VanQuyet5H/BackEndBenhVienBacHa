using AutoMapper;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class HDPPMappingProfile : Profile
    {
        public HDPPMappingProfile()
        {
            CreateMap<Core.Domain.Entities.HDPP.HDPP, HDPPViewModel>().IgnoreAllNonExisting();
            CreateMap<HDPPViewModel, Core.Domain.Entities.HDPP.HDPP>().IgnoreAllNonExisting();
        }
    }
}
