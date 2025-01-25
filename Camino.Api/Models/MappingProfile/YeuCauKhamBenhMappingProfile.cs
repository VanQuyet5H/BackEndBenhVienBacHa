using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauKhamBenhMappingProfile : Profile
    {
        public YeuCauKhamBenhMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh, YeuCauKhamBenh.YeuCauKhamBenhViewModel>()
                .IgnoreAllNonExisting()
                //.ForMember(d => d.PhongKham, o => o.MapFrom(s => s.PhongKham))
                //.ForMember(d => d.BenhNhan, o => o.MapFrom(s => s.BenhNhan))
                ;
            CreateMap<YeuCauKhamBenh.YeuCauKhamBenhViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>().IgnoreAllNonExisting()
                //.ForMember(d => d.YeuCauDichVuKyThuats, o => o.Ignore())
                //.ForMember(d => d.PhongKham, o => o.MapFrom(s => s.PhongKham))
                ;

            CreateMap<ChuyenKhamYeuCauKhamBenhViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>().IgnoreAllNonExisting();
        }
    }
}
