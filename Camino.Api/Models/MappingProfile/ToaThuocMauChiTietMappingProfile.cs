using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain.Entities.Thuocs;

namespace Camino.Api.Models.MappingProfile
{
    public class ToaThuocMauChiTietMappingProfile : Profile
    {
        public ToaThuocMauChiTietMappingProfile()
        {
            CreateMap<DuocPham, DuocPhamViewModel>().IgnoreAllNonExisting();

            CreateMap<ToaThuocMauChiTiet, ToaThuocMau.ToaThuocMauChiTietViewModel>().IgnoreAllNonExisting()
            .ForMember(d => d.DuocPham, o => o.MapFrom(s => s.DuocPham));

            CreateMap<DuocPham, DuocPhamKhamBenhViewModel>().IgnoreAllNonExisting();
            CreateMap<DuocPhamKhamBenhViewModel, DuocPham>().IgnoreAllNonExisting();


            CreateMap<ToaThuocMau.ToaThuocMauChiTietViewModel, ToaThuocMauChiTiet>().IgnoreAllNonExisting()
                .ForMember(d => d.DuocPham, o => o.Ignore())
                .ForMember(d => d.ToaThuocMau, o => o.Ignore());
        }
    }
}
