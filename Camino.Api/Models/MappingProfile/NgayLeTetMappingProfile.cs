using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NgayLeTet;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.NgayLeTet;

namespace Camino.Api.Models.MappingProfile
{
    public class NgayLeTetMappingProfile : Profile
    {
        public NgayLeTetMappingProfile()
        {
            CreateMap<Core.Domain.Entities.CauHinhs.NgayLeTet, NgayLeTetViewModel>();
            CreateMap<NgayLeTetViewModel, Core.Domain.Entities.CauHinhs.NgayLeTet>();

            CreateMap<NgayLeTetGridVo, NgayLeTetExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.LeHangNam, o => o.MapFrom(p => p.LeHangNam == true ? "Phải" : "Không"));
        }
    }
}
