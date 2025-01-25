using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;


namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhYeuCauDuocPhamMappingProfile : Profile
    {
        public KhamBenhYeuCauDuocPhamMappingProfile()
        {
            CreateMap<YeuCauDuocPhamBenhVien, KhamBenhYeuCauDuocPhamViewModel>().IgnoreAllNonExisting();
            CreateMap<KhamBenhYeuCauDuocPhamViewModel, YeuCauDuocPhamBenhVien>().IgnoreAllNonExisting();
        }
    }
}
