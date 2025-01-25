using AutoMapper;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class BenhNhanBaoHiemTuNhanMappingProfile : Profile
    {
        public BenhNhanBaoHiemTuNhanMappingProfile()
        {
            CreateMap<Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans.BenhNhanCongTyBaoHiemTuNhan, BenhNhans.BenhNhanBaoHiemTuNhansViewModel>().IgnoreAllNonExisting();

            CreateMap<BenhNhans.BenhNhanBaoHiemTuNhansViewModel, Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans.BenhNhanCongTyBaoHiemTuNhan>().IgnoreAllNonExisting()
                 .ForMember(x => x.CongTyBaoHiemTuNhan, o => o.Ignore());
        }
    }
}
