using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.CauHinhThuePhong;
using Camino.Core.Domain.ValueObject.CauHinhThuePhong;

namespace Camino.Api.Models.MappingProfile
{
    public class CauHinhThuePhongMappingProfile : Profile
    {
        public CauHinhThuePhongMappingProfile()
        {
            CreateMap<CauHinhThuePhongViewModel, Core.Domain.Entities.CauHinhs.CauHinhThuePhong>()
                .IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.CauHinhs.CauHinhThuePhong, CauHinhThuePhongViewModel>()
                .IgnoreAllNonExisting();

            CreateMap<CauHinhThuePhongGridVo, CauHinhThuePhongExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.HieuLuc, o => o.MapFrom(p => p.HieuLuc ? "Đang sử dụng" : "Ngừng sử dụng"));
        }
    }
}
