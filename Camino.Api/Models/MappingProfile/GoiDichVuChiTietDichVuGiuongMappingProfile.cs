using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.GoiDichVu;
using Camino.Core.Domain.Entities.GoiDichVus;

namespace Camino.Api.Models.MappingProfile
{
    public class GoiDichVuChiTietDichVuGiuongMappingProfile : Profile
    {
        public GoiDichVuChiTietDichVuGiuongMappingProfile()
        {
            CreateMap<GoiDichVuChiTietDichVuGiuongViewModel, GoiDichVuChiTietDichVuGiuong>().IgnoreAllNonExisting();
            CreateMap<GoiDichVuChiTietDichVuGiuong, GoiDichVuChiTietDichVuGiuongViewModel>().IgnoreAllNonExisting();
        }
    }
}
