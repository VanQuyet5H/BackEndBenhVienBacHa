using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.NhanSanXuatTheoQuocGia;

namespace Camino.Api.Models.MappingProfile
{
    public class NhaSanXuatTheoQuocGiaMappingProfile : Profile
    {
        public NhaSanXuatTheoQuocGiaMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGia, NhaSanXuatTheoQuocGiaViewModel>()
                .AfterMap((s, d) =>
                {
                    d.TenQuocGia = s.QuocGia?.Ten;

                });
            CreateMap<NhaSanXuatTheoQuocGiaViewModel, Core.Domain.Entities.NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGia>();
        }
    }
}
