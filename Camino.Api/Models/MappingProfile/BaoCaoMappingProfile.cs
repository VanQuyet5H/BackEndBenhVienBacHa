using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoThongKeDonThuoc;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class BaoCaoMappingProfile : Profile
    {
        public BaoCaoMappingProfile()
        {
            CreateMap<BaoCaoLuuHoSoBenhAnGridVo, LuuTruHoSoBenhAnExport>().IgnoreAllNonExisting();
            CreateMap<BaoCaoThongKeDonThuocGridVo, ThongKeDonThuocExportExcel>().IgnoreAllNonExisting();
        }
    }
}
