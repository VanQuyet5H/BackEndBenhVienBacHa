using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.TonKhos;

namespace Camino.Api.Models.MappingProfile
{
    public class DuocPhamSapHetHanMappingProfile : Profile
    {
        public DuocPhamSapHetHanMappingProfile()
        {
            CreateMap<DuocPhamSapHetHanGridVo, DuocPhamSapHetHanExportExcel>().IgnoreAllNonExisting();
        }
    }
}
