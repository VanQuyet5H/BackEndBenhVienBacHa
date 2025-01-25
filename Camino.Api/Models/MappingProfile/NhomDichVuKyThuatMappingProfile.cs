using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.NhomDichVuKyThuat;
using Microsoft.Data.OData.Query.SemanticAst;

namespace Camino.Api.Models.MappingProfile
{
    public class NhomDichVuKyThuatMappingProfile :Profile
    {
        public NhomDichVuKyThuatMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DichVuKyThuats.NhomDichVuKyThuat, NhomDichVuKyThuatViewModel>();

            CreateMap<NhomDichVuKyThuatViewModel, Core.Domain.Entities.DichVuKyThuats.NhomDichVuKyThuat>();

        }
    }
}
