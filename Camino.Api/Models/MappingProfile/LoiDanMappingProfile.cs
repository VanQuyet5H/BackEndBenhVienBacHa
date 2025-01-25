using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.LoiDan;

namespace Camino.Api.Models.MappingProfile
{
    public class LoiDanMappingProfile : Profile
    {
        public LoiDanMappingProfile()
        {
            CreateMap<LoiDanGridVo, LoiDanExportExcel>()
                .IgnoreAllNonExisting();
        }
    }
}
