using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.TiemChung;

namespace Camino.Api.Models.MappingProfile
{
    public class TiemChungYeuCauKhamBenhMappingProfile : Profile
    {
        public TiemChungYeuCauKhamBenhMappingProfile()
        {
            CreateMap<TiemChungYeuCauKhamBenhSangLocViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh, TiemChungYeuCauKhamBenhSangLocViewModel > ().IgnoreAllNonExisting();
        }
    }
}
