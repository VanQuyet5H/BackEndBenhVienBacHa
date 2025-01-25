using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.KhamBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauKhamBenhDonThuocChiTietMappingProfile : Profile
    {
        public YeuCauKhamBenhDonThuocChiTietMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuocChiTiet, YeuCauKhamBenh.YeuCauKhamBenhDonThuocChiTietViewModel>().IgnoreAllNonExisting();
            CreateMap<YeuCauKhamBenh.YeuCauKhamBenhDonThuocChiTietViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuocChiTiet>().IgnoreAllNonExisting();
            //CreateMap<DonThuocChiTietVo, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuocChiTiet>().IgnoreAllNonExisting();
        }
    }
}
