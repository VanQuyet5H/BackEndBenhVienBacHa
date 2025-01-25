using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Cauhinh;
using Camino.Core.Domain.Entities.CauHinhs;

namespace Camino.Api.Models.MappingProfile
{
    public class CauHinhTheoThoiGianChiTietMappingProfile : Profile
    {
        public CauHinhTheoThoiGianChiTietMappingProfile()
        {
            CreateMap<CauHinhTheoThoiGianChiTiet, CauHinhTheoThoiGianChiTietViewModel>().IgnoreAllNonExisting();
            CreateMap<CauHinhTheoThoiGianChiTietViewModel, CauHinhTheoThoiGianChiTiet>().IgnoreAllNonExisting();
        }
    }
}
