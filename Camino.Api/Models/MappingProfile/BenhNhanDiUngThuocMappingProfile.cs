using AutoMapper;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class BenhNhanDiUngThuocMappingProfile : Profile
    {
        public BenhNhanDiUngThuocMappingProfile()
        {
            CreateMap<Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc, BenhNhans.BenhNhanDiUngThuocsViewModel>().IgnoreAllNonExisting();

            CreateMap<BenhNhans.BenhNhanDiUngThuocsViewModel, Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc>().IgnoreAllNonExisting();
        }
    }
}
