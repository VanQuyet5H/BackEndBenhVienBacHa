using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class DuyetHoanTraDuocPhamMappingProfile : Profile
    {
        public DuyetHoanTraDuocPhamMappingProfile()
        {
            CreateMap<DanhSachDuyetHoanTraDuocPhamVo, DuyetHoanTraDuocPhamExportExcel>();
            CreateMap<DanhSachDuyetHoanTraDuocPhamChiTietVo, DuyetHoanTraDuocPhamExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
