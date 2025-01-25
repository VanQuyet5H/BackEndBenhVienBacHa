using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.ChiSoXetNghiems;
using Camino.Core.Domain.Entities.ChiSoXetNghiems;
using Camino.Core.Domain.ValueObject.ChiSoXetNghiems;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class ChiSoXetNghiemMappingProfile : Profile
    {
        public ChiSoXetNghiemMappingProfile()
        {
            CreateMap<ChiSoXetNghiem, ChiSoXetNghiemViewModel>()
             .AfterMap((source, viewmodel) =>
             {
                     viewmodel.TenLoaiXetNghiem = source.LoaiXetNghiem.GetDescription();
             });
            CreateMap<ChiSoXetNghiemViewModel,ChiSoXetNghiem>();
            CreateMap<ChiSoXetNghiemGridVo, ChiSoXetNghiemExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.HieuLuc, o => o.MapFrom(p => p.HieuLuc == true ? "Đang sử dụng" : "Ngừng sử dụng"));
        }
    }
}
