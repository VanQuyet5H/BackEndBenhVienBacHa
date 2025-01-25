using AutoMapper;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiDungGhiChuMiemGiamMappingProfile : Profile
    {
        public NoiDungGhiChuMiemGiamMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NoiDungGhiChuMiemGiams.NoiDungGhiChuMiemGiam, NoiDungGhiChuMiemGiamViewModel>().IgnoreAllNonExisting();
            CreateMap<NoiDungGhiChuMiemGiamViewModel, Core.Domain.Entities.NoiDungGhiChuMiemGiams.NoiDungGhiChuMiemGiam>().IgnoreAllNonExisting();
        }
    }
}
