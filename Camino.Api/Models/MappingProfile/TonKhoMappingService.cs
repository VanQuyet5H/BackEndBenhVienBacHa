using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.TonKhos;
using static Camino.Core.Domain.ValueObject.TonKhos.VatTuDaHetHan;

namespace Camino.Api.Models.MappingProfile
{
    public class TonKhoMappingService : Profile
    {
        public TonKhoMappingService()
        {
            CreateMap<TonKhoTatCaGridVo, TonKhoTatCaExportExcel>().IgnoreAllNonExisting();
            CreateMap<NhapXuatTonKhoGridVo, NhapXuatTonKhoExportExcel>().IgnoreAllNonExisting();
            CreateMap<TonKhoGridVo, CanhBaoTonKhoExportExcel>().IgnoreAllNonExisting();
            CreateMap<VatTuDaHetHanGridVo, VatTuDaHetHanExportExcel>().IgnoreAllNonExisting();
        }
    }
}
