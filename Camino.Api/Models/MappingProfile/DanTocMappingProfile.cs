using AutoMapper;
using Camino.Api.Models.DanToc;
using Camino.Core.Domain.ValueObject.DanToc;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class DanTocMappingProfile :Profile
    {
        public DanTocMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DanTocs.DanToc, DanTocViewModel>().AfterMap((s, d) =>
            {
                d.TenQuocGia = s.QuocGia?.Ten;

            }); ;
            CreateMap<DanTocViewModel, Core.Domain.Entities.DanTocs.DanToc>().ForMember(d => d.QuocGia, o => o.Ignore());

            CreateMap<DanTocGridVo, DanTocExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));
        }
    }
}
