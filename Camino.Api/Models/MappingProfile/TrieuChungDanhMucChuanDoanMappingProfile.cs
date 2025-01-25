using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.TrieuChungDanhMucChuanDoan;

namespace Camino.Api.Models.MappingProfile
{
    public class TrieuChungDanhMucChuanDoanMappingProfile : Profile
    {
        public TrieuChungDanhMucChuanDoanMappingProfile()
        {
            CreateMap<Core.Domain.Entities.TrieuChungDanhMucChuanDoans.TrieuChungDanhMucChuanDoan, TrieuChungDanhMucChuanDoanViewModel>()
                .AfterMap((s, d) =>
                {
                    d.TenDanhMucChuan = s.DanhMucChuanDoan?.TenTiengViet;

                }); ;
            CreateMap<TrieuChungDanhMucChuanDoanViewModel, Core.Domain.Entities.TrieuChungDanhMucChuanDoans.TrieuChungDanhMucChuanDoan>();
        }
    }
  
}
