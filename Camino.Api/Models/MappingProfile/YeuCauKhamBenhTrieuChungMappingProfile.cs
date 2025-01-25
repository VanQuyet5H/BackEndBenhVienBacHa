using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauKhamBenhTrieuChungMappingProfile : Profile
    {
        public YeuCauKhamBenhTrieuChungMappingProfile()
        {
            CreateMap<YeuCauKhamBenhTrieuChungViewModel, YeuCauKhamBenhTrieuChung>().IgnoreAllNonExisting();
            CreateMap<YeuCauKhamBenhTrieuChung, YeuCauKhamBenhTrieuChungViewModel>().IgnoreAllNonExisting();
        }
    }
}
