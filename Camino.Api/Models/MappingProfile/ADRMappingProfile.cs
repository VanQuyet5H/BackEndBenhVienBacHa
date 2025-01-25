using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Helpers;
namespace Camino.Api.Models.MappingProfile
{
    public class ADRMappingProfile : Profile
    {
        public ADRMappingProfile()
        {
            CreateMap<Core.Domain.Entities.Thuocs.ADR, Thuoc.ADRViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TenThuocHoacHoatChat1 = s.ThuocHoacHoatChat1?.Ten;
                    d.TenThuocHoacHoatChat2 = s.ThuocHoacHoatChat2?.Ten;
                    d.Ma1ThuocHoacHoatChat = s.ThuocHoacHoatChat1?.Ma;
                    d.Ma2ThuocHoacHoatChat = s.ThuocHoacHoatChat2?.Ma;
                    d.MaTenHoatChat1 = s.ThuocHoacHoatChat1?.Ma + "-" + s.ThuocHoacHoatChat1?.Ten;
                    d.MaTenHoatChat2 = s.ThuocHoacHoatChat2?.Ma + "-" + s.ThuocHoacHoatChat2?.Ten;
                    d.MucDoChuYKhiChiDinhDisplay = s.MucDoChuYKhiChiDinh.GetDescription();
                    d.MucDoTuongTacDisplay = s.MucDoTuongTac.GetDescription();
                });

            CreateMap<Thuoc.ADRViewModel, Core.Domain.Entities.Thuocs.ADR>().IgnoreAllNonExisting()
                .ForMember(x => x.ThuocHoacHoatChat1, o => o.Ignore())
                .ForMember(x => x.ThuocHoacHoatChat2, o => o.Ignore()); ;

            CreateMap<ADRGridVo, ADRExportExcel>().IgnoreAllNonExisting();
        }
    }
}
