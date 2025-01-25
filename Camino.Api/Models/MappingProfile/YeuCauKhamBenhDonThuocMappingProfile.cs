using AutoMapper;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauKhamBenhDonThuocMappingProfile : Profile
    {
        public YeuCauKhamBenhDonThuocMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuoc, YeuCauKhamBenh.YeuCauKhamBenhDonThuocViewModel>().IgnoreAllNonExisting();
            CreateMap<YeuCauKhamBenh.YeuCauKhamBenhDonThuocViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuoc>().IgnoreAllNonExisting();
        }
    }
}
