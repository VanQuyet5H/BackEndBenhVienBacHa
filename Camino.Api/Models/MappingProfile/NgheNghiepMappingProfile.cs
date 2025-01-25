using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NgheNghiep;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.NgheNghiep;

namespace Camino.Api.Models.MappingProfile
{
    public class NgheNghiepMappingProfile : Profile
    {
        public NgheNghiepMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NgheNghieps.NgheNghiep, NgheNghiepViewModel>().IgnoreAllNonExisting();
            CreateMap<NgheNghiepViewModel, Core.Domain.Entities.NgheNghieps.NgheNghiep>().IgnoreAllNonExisting();

            CreateMap<NgheNghiepGridVo, NgheNghiepExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));
        }
    }
}
