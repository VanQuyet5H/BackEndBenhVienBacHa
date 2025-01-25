using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.TemplateKhamBenhTheoDichVus;

namespace Camino.Api.Models.MappingProfile
{
    public class TemplateKhamBenhTheoDichVuMappingProfile : Profile
    {
        public TemplateKhamBenhTheoDichVuMappingProfile()
        {
            CreateMap<TemplateKhamBenhTheoDichVu, KhamBenhTemplateKhamBenhTheoDichVuViewModel>().IgnoreAllNonExisting();
            CreateMap<KhamBenhTemplateKhamBenhTheoDichVuViewModel, TemplateKhamBenhTheoDichVu>().IgnoreAllNonExisting();
        }
    }
}