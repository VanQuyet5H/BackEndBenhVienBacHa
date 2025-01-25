using AutoMapper;
using Camino.Api.Extensions;


namespace Camino.Api.Models.MappingProfile
{
    public class BenhNhanTienSuBenhMappingProfile : Profile
    {
        public BenhNhanTienSuBenhMappingProfile()
        {
            CreateMap<Core.Domain.Entities.BenhNhans.BenhNhanTienSuBenh, BenhNhans.BenhNhanTienSuBenhsViewModel>().IgnoreAllNonExisting();

            CreateMap<BenhNhans.BenhNhanTienSuBenhsViewModel, Core.Domain.Entities.BenhNhans.BenhNhanTienSuBenh>().IgnoreAllNonExisting();
        }
    }
}
