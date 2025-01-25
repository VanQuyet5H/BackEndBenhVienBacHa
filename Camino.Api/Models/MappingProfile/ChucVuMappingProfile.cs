using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ChucVu;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class ChucVuMappingProfile : Profile
    {
        public ChucVuMappingProfile()
        {
            CreateMap<Core.Domain.Entities.ChucVus.ChucVu, ChucVu.ChucVuViewModel>().IgnoreAllNonExisting();
              
            CreateMap<ChucVu.ChucVuViewModel, Core.Domain.Entities.ChucVus.ChucVu>().IgnoreAllNonExisting();

            CreateMap<ChucVuGridVo, ChucVuExportExcel>().IgnoreAllNonExisting()
                    .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));
        }
    }
}
