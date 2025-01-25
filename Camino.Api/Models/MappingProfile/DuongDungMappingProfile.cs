using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Api.Models.MappingProfile
{
    public class DuongDungMappingProfile : Profile
    {
        public DuongDungMappingProfile()
        {
            CreateMap<Core.Domain.Entities.Thuocs.DuongDung, Thuoc.DuongDungViewModel>().IgnoreAllNonExisting();

            CreateMap<Thuoc.DuongDungViewModel, Core.Domain.Entities.Thuocs.DuongDung>().IgnoreAllNonExisting();

            CreateMap<DuongDungGridVo, DuongDungExportExcel>()
                .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));
        }
    }
}
