using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.CheDoAn;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CheDoAn;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class CheDoAnMappingProfile :Profile
    {
        public CheDoAnMappingProfile()
        {
            CreateMap<Core.Domain.Entities.CheDoAns.CheDoAn, CheDoAnViewModel>()
                .AfterMap((s, d) =>
            {

            });
            CreateMap<CheDoAnViewModel, Core.Domain.Entities.CheDoAns.CheDoAn>();

            CreateMap<CheDoAnGridVo, CheDoAnExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));
        }
       
    }
}
