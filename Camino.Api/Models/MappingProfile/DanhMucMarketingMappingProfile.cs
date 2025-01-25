using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.DanhMucMarketing;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class DanhMucMarketingMappingProfile : Profile
    {
        public DanhMucMarketingMappingProfile()
        {
            CreateMap<DanhSachMarketingGridVo, DanhMucMarketingExportExcel>().IgnoreAllNonExisting();
            CreateMap<DanhSachMarketingChildGridVo, DanhMucMarketingExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
