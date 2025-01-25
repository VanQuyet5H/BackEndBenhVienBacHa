using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.TonKhos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class VatTuTonKhoMappingProfile : Profile
    {
        public VatTuTonKhoMappingProfile()
        {
            CreateMap<VatTuTonKhoCanhBaoGridVo, VatTuTonKhoCanhBaoExportExcel>().IgnoreAllNonExisting();

            CreateMap<VatTuTonKhoTongHopGridVo, VatTuTonKhoTongHopExportExcel>().IgnoreAllNonExisting();

            CreateMap<VatTuTonKhoNhapXuatGridVo, VatTuTonKhoNhapXuatExportExcel>().IgnoreAllNonExisting();

            CreateMap<VatTuTonKhoNhapXuatDetailGridVo, VatTuTonKhoNhapXuatChiTietExportExcel>().IgnoreAllNonExisting();
        }
    }
}
