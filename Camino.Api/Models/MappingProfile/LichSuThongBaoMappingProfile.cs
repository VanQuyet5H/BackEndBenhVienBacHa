using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Messages;

namespace Camino.Api.Models.MappingProfile
{
    public class LichSuThongBaoMappingProfile : Profile
    {
        public LichSuThongBaoMappingProfile()
        {
            CreateMap<LichSuThongBaoGripVo, LichSuThongBaoExportExcel>().IgnoreAllNonExisting();
        }
    }
}
