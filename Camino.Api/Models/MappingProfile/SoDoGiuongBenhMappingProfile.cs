using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GiuongBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class SoDoGiuongBenhMappingProfile : Profile
    {
        public SoDoGiuongBenhMappingProfile()
        {
            CreateMap<SoDoGiuongBenhKhoaGridVo, SoDoGiuongBenhKhoaExportExcel>().IgnoreAllNonExisting();
        }
    }
}
