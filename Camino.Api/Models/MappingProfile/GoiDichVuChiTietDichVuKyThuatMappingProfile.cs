using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.GoiDichVu;
using Camino.Core.Domain.Entities.GoiDichVus;

namespace Camino.Api.Models.MappingProfile
{
    public class GoiDichVuChiTietDichVuKyThuatMappingProfile : Profile
    {
        public GoiDichVuChiTietDichVuKyThuatMappingProfile()
        {
            CreateMap<GoiDichVuChiTietDichVuKyThuatViewModel, GoiDichVuChiTietDichVuKyThuat>().IgnoreAllNonExisting();
            CreateMap<GoiDichVuChiTietDichVuKyThuat, GoiDichVuChiTietDichVuKyThuatViewModel>().IgnoreAllNonExisting();
        }
    }
}
