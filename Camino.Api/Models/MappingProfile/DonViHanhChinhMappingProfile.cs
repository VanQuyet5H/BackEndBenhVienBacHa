using AutoMapper;
using Camino.Api.Extensions;
namespace Camino.Api.Models.MappingProfile
{
    public class DonViHanhChinhMappingProfile : Profile
    {
        public DonViHanhChinhMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh, DonViHanhChinh.DonViHanhChinhViewModel>().IgnoreAllNonExisting();

            CreateMap<DonViHanhChinh.DonViHanhChinhViewModel, Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh>().IgnoreAllNonExisting();
        }
    }
}
