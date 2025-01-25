using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class ExportChungTuMappingProfile : Profile
    {
        public ExportChungTuMappingProfile()
        {          
            CreateMap<GiayRaVienExportExcel, Camino.Core.Domain.ValueObject.ExcelChungTu.GiayRaVienVo>();
            CreateMap<Camino.Core.Domain.ValueObject.ExcelChungTu.GiayRaVienVo, GiayRaVienExportExcel>().IgnoreAllNonExisting();

            CreateMap<GiayTomTatBenhAnExportExcel, Camino.Core.Domain.ValueObject.ExcelChungTu.GiayTomTatBenhAn>();
            CreateMap<Camino.Core.Domain.ValueObject.ExcelChungTu.GiayTomTatBenhAn, GiayTomTatBenhAnExportExcel>().IgnoreAllNonExisting();

            CreateMap<GiayNghiHuongBHXHExportExcel, Camino.Core.Domain.ValueObject.ExcelChungTu.GiayNghiHuongBHXH>();
            CreateMap<Camino.Core.Domain.ValueObject.ExcelChungTu.GiayNghiHuongBHXH, GiayNghiHuongBHXHExportExcel>().IgnoreAllNonExisting();
        }       
    }
}
