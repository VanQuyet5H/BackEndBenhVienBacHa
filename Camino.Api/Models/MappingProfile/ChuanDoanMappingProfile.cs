using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.ICDs;
using System.Linq;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.ICDs;

namespace Camino.Api.Models.MappingProfile
{
    public class ChuanDoanMappingProfile : Profile
    {
        public ChuanDoanMappingProfile()
        {
            CreateMap<Core.Domain.Entities.ICDs.ChuanDoan, ChuanDoanViewModel>().AfterMap((source, dest) =>
            {
                dest.ChuanDoanLienKetICDIds = source.ChuanDoanLienKetICDs.Select(r => r.ICDId).ToList();
                dest.DanhMucChuanDoanName = source.DanhMucChuanDoan == null ? string.Empty : source.DanhMucChuanDoan.TenTiengViet;
            });

            CreateMap<ChuanDoanViewModel, Core.Domain.Entities.ICDs.ChuanDoan>().IgnoreAllNonExisting();

            CreateMap<ChuanDoanGridVo, ChuanDoanExportExcel>()
                .IgnoreAllNonExisting();
        }
    }
}