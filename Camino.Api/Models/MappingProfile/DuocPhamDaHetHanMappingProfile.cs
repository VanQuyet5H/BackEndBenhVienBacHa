using System.Globalization;
using AutoMapper;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class DuocPhamDaHetHanMappingProfile : Profile
    {
        public DuocPhamDaHetHanMappingProfile()
        {
            CreateMap<DuocPhamDaHetHanGridVo, DuocPhamDaHetHanExportExcel>()
                .AfterMap((source, dest) => { dest.SoLuongTon = source.SoLuongTon.ToString(CultureInfo.InvariantCulture); });
        }
    }
}
