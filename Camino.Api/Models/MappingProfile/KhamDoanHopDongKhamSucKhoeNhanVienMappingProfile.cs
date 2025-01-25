using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoanChiSoSinhTons;
using Camino.Core.Domain.Entities.KhamDoans;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamDoanHopDongKhamSucKhoeNhanVienMappingProfile : Profile
    {
        public KhamDoanHopDongKhamSucKhoeNhanVienMappingProfile()
        {
            CreateMap<HopDongKhamSucKhoeNhanVien, KhamDoanHopDongKhamSucKhoeNhanVienViewModel>().IgnoreAllNonExisting();
            CreateMap<KhamDoanHopDongKhamSucKhoeNhanVienViewModel, HopDongKhamSucKhoeNhanVien>().IgnoreAllNonExisting();
        }
    }
}
