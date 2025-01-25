using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KetQuaXetNghiem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class KetQuaXetNghiemMappingProfile : Profile
    {
        public KetQuaXetNghiemMappingProfile()
        {
            CreateMap<KetQuaXetNghiemGridVo, KetQuaXetNghiemExportExcel>().IgnoreAllNonExisting();
            CreateMap<KetQuaXetNghiemChildGridVo, KetQuaXetNghiemExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
