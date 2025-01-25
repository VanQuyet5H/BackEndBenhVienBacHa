using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Thuoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class NhomThuocMappingProfile :Profile
    {
        public NhomThuocMappingProfile()
        {
            CreateMap<Core.Domain.Entities.Thuocs.NhomThuoc, NhomThuocViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
            {
                d.TenLoaiHoaChat = s.LoaiThuocHoacHoatChat.GetDescription();

            }); ;

            CreateMap<NhomThuocViewModel, Core.Domain.Entities.Thuocs.NhomThuoc>();
        }
    }
}
