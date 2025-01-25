using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NhatKyHeThong;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class NhatKyHeThongMappingProfile:Profile
    {
        public NhatKyHeThongMappingProfile()
        {
            CreateMap<Core.Domain.Entities.HeThong.NhatKyHeThong, NhatKyHeThongViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TenHoatDong = s.HoatDong.GetDescription();
                    d.NguoiTao = s.UserDetails.HoTen;
                });

            CreateMap<NhatKyHeThongViewModel, Core.Domain.Entities.HeThong.NhatKyHeThong>().IgnoreAllNonExisting();

            CreateMap<NhatKyHeThongGridVo, NhatKyHeThongExportExcel>().IgnoreAllNonExisting();
        }
    }
}
