using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DanhMucMarketings;
using Camino.Core.Domain.Entities.BenhNhans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class ThongTinGoiMarketingMappingProfile : Profile
    {
        public ThongTinGoiMarketingMappingProfile()
        {
            CreateMap<BenhNhan, ThongTinGoiMarketingViewModel>().IgnoreAllNonExisting();
                //.ForMember(d => d.NgayThangNamSinh, o => o.MapFrom(s => (s.NgaySinh != null && s.NgaySinh != 0 
                //    &&  s.ThangSinh != null && s.ThangSinh != 0
                //    && s.NamSinh != null && s.NamSinh != 0) ? new DateTime(s.NamSinh ?? 0, s.ThangSinh ?? 0, s.NgaySinh ?? 0) : null);
            CreateMap<ThongTinGoiMarketingViewModel, BenhNhan>().IgnoreAllNonExisting();
        }
    }
}
