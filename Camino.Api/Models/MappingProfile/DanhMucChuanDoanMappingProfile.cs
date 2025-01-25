using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.ICDs;

namespace Camino.Api.Models.MappingProfile
{
    public class DanhMucChuanDoanMappingProfile : Profile
    {
        public DanhMucChuanDoanMappingProfile()
        {
            CreateMap<Core.Domain.Entities.ICDs.DanhMucChuanDoan, ICDs.DanhMucChuanDoanViewModel>().IgnoreAllNonExisting();

            CreateMap<ICDs.DanhMucChuanDoanViewModel, Core.Domain.Entities.ICDs.DanhMucChuanDoan>().IgnoreAllNonExisting();

            CreateMap<DanhMucChuanDoanGridVo, DanhMucChanDoanExportExcel>().IgnoreAllNonExisting();
        }
    }
}
