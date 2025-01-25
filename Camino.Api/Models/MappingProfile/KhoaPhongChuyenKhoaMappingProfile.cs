using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.KhoaPhongChuyenKhoa;

namespace Camino.Api.Models.MappingProfile
{
    public class KhoaPhongChuyenKhoaMappingProfile : Profile
    {
        public KhoaPhongChuyenKhoaMappingProfile()
        {
            CreateMap<Core.Domain.Entities.KhoaPhongChuyenKhoas.KhoaPhongChuyenKhoa, KhoaPhongChuyenKhoaViewModel>()
                .AfterMap((s, d) =>
                {
                    d.TenKhoa = s.Khoa?.Ten;

                }); ;
            CreateMap<KhoaPhongChuyenKhoaViewModel, Core.Domain.Entities.KhoaPhongChuyenKhoas.KhoaPhongChuyenKhoa > ();
        }
    }
}
