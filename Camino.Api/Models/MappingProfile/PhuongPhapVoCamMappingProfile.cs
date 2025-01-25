using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.PhuongPhapVoCams;
using Camino.Core.Domain.Entities.PhuongPhapVoCams;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.PhuongPhapVoCams;

namespace Camino.Api.Models.MappingProfile
{
    public class PhuongPhapVoCamMappingProfile : Profile
    {
        public PhuongPhapVoCamMappingProfile()
        {
            CreateMap<PhuongPhapVoCam, PhuongPhapVoCamViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<PhuongPhapVoCamViewModel, PhuongPhapVoCam>()
                .IgnoreAllNonExisting();
            CreateMap<PhuongPhapVoCamGridVo, PhuongPhapVoCamExportExcel>()
                .IgnoreAllNonExisting();
        }
    }
}