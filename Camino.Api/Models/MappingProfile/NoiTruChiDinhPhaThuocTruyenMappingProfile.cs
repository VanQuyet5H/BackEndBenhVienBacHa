using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiTruChiDinhPhaThuocTruyenMappingProfile : Profile
    {
        public NoiTruChiDinhPhaThuocTruyenMappingProfile()
        {
            //CreateMap<DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel, PhaThuocTiemBenhVienVo>().IgnoreAllNonExisting();
            //CreateMap<PhaThuocTiemBenhVienVo, DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel>().IgnoreAllNonExisting();
        }
    }
}
