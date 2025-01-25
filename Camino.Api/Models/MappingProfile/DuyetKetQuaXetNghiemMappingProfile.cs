using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class DuyetKetQuaXetNghiemMappingProfile : Profile
    {
        public DuyetKetQuaXetNghiemMappingProfile()
        {
            CreateMap<DuyetKetQuaXetNghiemGridVo, DuyetKetQuaXetNghiemExportExcel>().IgnoreAllNonExisting();
            CreateMap<DuyetKetQuaXetNghiemDetailGridVo, DuyetKetQuaXetNghiemExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
