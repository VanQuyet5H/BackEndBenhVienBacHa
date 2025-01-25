using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Messages;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class LichSuEmailMappingProfile : Profile
    {
        public LichSuEmailMappingProfile()
        {
            CreateMap<LichSuEmailGrid, LichSuEmailExportExcel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.NoiDung = CommonHelper.StripBodyHTML(s.NoiDung);
                    d.TapTinDinhKem = string.IsNullOrEmpty(s.TapTinDinhKem) ? "." : s.TapTinDinhKem;
                });
        }
    }
}
