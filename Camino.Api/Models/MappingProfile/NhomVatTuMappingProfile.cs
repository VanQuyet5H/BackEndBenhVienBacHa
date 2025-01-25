using AutoMapper;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class NhomVatTuMappingProfile : Profile
    {
        public NhomVatTuMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NhomVatTus.NhomVatTu, NhomVatTu.NhomVatTuViewModel>().IgnoreAllNonExisting();

            CreateMap<NhomVatTu.NhomVatTuViewModel, Core.Domain.Entities.NhomVatTus.NhomVatTu>().IgnoreAllNonExisting();
        }
    }
}
