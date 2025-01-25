using AutoMapper;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class InputStringStoredMappingProfile : Profile
    {
        public InputStringStoredMappingProfile()
        {
            CreateMap<Core.Domain.Entities.InputStringStoreds.InputStringStored, InputStringStoreds.InputStringStoredViewModel>().IgnoreAllNonExisting();
            CreateMap<InputStringStoreds.InputStringStoredViewModel, Core.Domain.Entities.InputStringStoreds.InputStringStored>().IgnoreAllNonExisting();
        }
    }
}
