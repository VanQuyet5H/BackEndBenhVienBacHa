using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NhomChucDanh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.NhomChucDanh;

namespace Camino.Api.Models.MappingProfile
{
    public class NhomChucDanhMappingProfile :Profile
    {
        public NhomChucDanhMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NhomChucDanhs.NhomChucDanh, NhomChucDanhViewModel>();
            CreateMap<NhomChucDanhViewModel, Core.Domain.Entities.NhomChucDanhs.NhomChucDanh>();

            CreateMap<NhomChucDanhGridVo, NhomChucDanhExportExcel>().IgnoreAllNonExisting();
        }
    }
}
