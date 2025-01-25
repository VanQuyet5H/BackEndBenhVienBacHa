using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using Camino.Core.Domain.ValueObject.XetNghiem;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauXetNghiemLaiMappingProfile : Profile
    {
        public YeuCauXetNghiemLaiMappingProfile()
        {
            CreateMap<YeuCauGoiLaiXetNghiemGridVo, YeuCauXetNghiemChayLaiExportExcel>();   
            CreateMap<YeuCauGoiLaiXetNghiemChiTietGridVo, YeuCauXetNghiemChayLaiExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
