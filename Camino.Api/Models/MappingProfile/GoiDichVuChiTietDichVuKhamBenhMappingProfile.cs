using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.GoiDichVu;

namespace Camino.Api.Models.MappingProfile
{
    public class GoiDichVuChiTietDichVuKhamBenhMappingProfile : Profile
    {
        public GoiDichVuChiTietDichVuKhamBenhMappingProfile()
        {
            CreateMap<GoiDichVuChiTietDichVuKhamBenhViewModel, Core.Domain.Entities.GoiDichVus.GoiDichVuChiTietDichVuKhamBenh>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.GoiDichVus.GoiDichVuChiTietDichVuKhamBenh, GoiDichVuChiTietDichVuKhamBenhViewModel>().IgnoreAllNonExisting();
        }
    }
}
