using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhDichVuGiuongBenhVienMappingProfile : Profile
    {
        public KhamBenhDichVuGiuongBenhVienMappingProfile()
        {
            CreateMap<KhamBenhYeuCauGiuongBenhViewModel, YeuCauDichVuGiuongBenhVien>().IgnoreAllNonExisting();
            CreateMap<YeuCauDichVuGiuongBenhVien, KhamBenhYeuCauGiuongBenhViewModel>().IgnoreAllNonExisting();
        }
    }
}
