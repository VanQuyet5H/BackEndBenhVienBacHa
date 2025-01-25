using AutoMapper;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiDungMauLoiDanBacSiMappingProfile : Profile
    {
        public NoiDungMauLoiDanBacSiMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NoiDungMauLoiDanBacSi.NoiDungMauLoiDanBacSi, NoiDungMauLoiDanBacSiViewModel>().IgnoreAllNonExisting();
            CreateMap<NoiDungMauLoiDanBacSiViewModel, Core.Domain.Entities.NoiDungMauLoiDanBacSi.NoiDungMauLoiDanBacSi>().IgnoreAllNonExisting();
        }
    }
}
