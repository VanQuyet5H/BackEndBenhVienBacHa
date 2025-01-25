using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauKhamBenhICDKhacMappingProfile : Profile
    {
        public YeuCauKhamBenhICDKhacMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhICDKhac, YeuCauKhamBenhICDKhacViewModel>()
                .IgnoreAllNonExisting()
                 .AfterMap((model, viewModel) =>
                 {
                     //viewModel.TenICD = model.ICD?.Ma + " - " + model.ICD?.TenTiengViet;
                     viewModel.TenICD = model.ICD != null ? model.ICD.Ma + " - " + model.ICD.TenTiengViet : "";

                 })
                 ;
            CreateMap<YeuCauKhamBenhICDKhacViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhICDKhac>()
                .IgnoreAllNonExisting();
        }
    }
}
