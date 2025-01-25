using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhYeuCauDichVuKyThuatMappingProfile : Profile
    {
        public KhamBenhYeuCauDichVuKyThuatMappingProfile()
        {
            CreateMap<YeuCauDichVuKyThuat, KhamBenhYeuCauDichVuKyThuatViewModel>().IgnoreAllNonExisting()
                .AfterMap((source, dest) =>
                {
                });

            CreateMap<KhamBenhYeuCauDichVuKyThuatViewModel, YeuCauDichVuKyThuat>().IgnoreAllNonExisting();
        }
    }
}
