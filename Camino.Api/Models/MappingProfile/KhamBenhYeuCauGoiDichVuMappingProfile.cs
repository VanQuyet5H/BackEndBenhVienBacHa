using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhYeuCauGoiDichVuMappingProfile : Profile
    {
        public KhamBenhYeuCauGoiDichVuMappingProfile()
        {
            CreateMap<YeuCauGoiDichVu, KhamBenhYeuCauGoiDichVuViewModel>().IgnoreAllNonExisting();
            CreateMap<KhamBenhYeuCauGoiDichVuViewModel, YeuCauGoiDichVu>().IgnoreAllNonExisting();
        }
    }
}
