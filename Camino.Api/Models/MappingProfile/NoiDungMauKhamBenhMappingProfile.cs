using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Services.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiDungMauKhamBenhMappingProfile : Profile
    {
        public NoiDungMauKhamBenhMappingProfile()
        {
            CreateMap<NoiDungMauKhamBenhViewModel, NoiDungMauKhamBenh>().IgnoreAllNonExisting();
            CreateMap<NoiDungMauKhamBenh, NoiDungMauKhamBenhViewModel>().IgnoreAllNonExisting();
        }
    }
}
