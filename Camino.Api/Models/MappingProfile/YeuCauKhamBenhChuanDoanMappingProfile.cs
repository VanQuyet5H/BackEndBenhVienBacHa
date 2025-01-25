using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauKhamBenhChuanDoanMappingProfile : Profile
    {
        public YeuCauKhamBenhChuanDoanMappingProfile()
        {
            CreateMap<YeuCauKhamBenhChuanDoanViewModel, YeuCauKhamBenhChuanDoan>().IgnoreAllNonExisting();
            CreateMap<YeuCauKhamBenhChuanDoan, YeuCauKhamBenhChuanDoanViewModel>().IgnoreAllNonExisting();
        }
    }
}
