using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhYeuCauVatTuBenhVienMappingProfile : Profile
    {
        public KhamBenhYeuCauVatTuBenhVienMappingProfile() 
        {
            CreateMap<KhamBenhYeuCauVatTuBenhVienViewModel, YeuCauVatTuBenhVien>().IgnoreAllNonExisting();
            CreateMap<YeuCauVatTuBenhVien, KhamBenhYeuCauVatTuBenhVienViewModel>().IgnoreAllNonExisting();
        }
       
    }
}
