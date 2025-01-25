using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.VanBangChuyenMon;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.VanBangChuyenMon;

namespace Camino.Api.Models.MappingProfile
{
    public class VanBangChuyenMonMappingProfile : Profile
    {
        public VanBangChuyenMonMappingProfile()
        {
            CreateMap<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon, VanBangChuyenMonViewModel>();
            CreateMap<VanBangChuyenMonViewModel, Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon>();

            CreateMap<VanBangChuyenMonGridVo, VanBangChuyenMonExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng"))); ;
        }
    }
}
