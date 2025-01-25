using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DinhMucVatTuTonKhos;
using Camino.Core.Domain.Entities.DinhMucVatTuTonKhos;
using Camino.Core.Domain.ValueObject.DinhMucVatTuTonKho;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class DinhMucVatTuTonKhoMappingProfile : Profile
    {
        public DinhMucVatTuTonKhoMappingProfile()
        {
            CreateMap<DinhMucVatTuTonKho, DinhMucVatTuTonKhoViewModel>()
                .IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TenVatTu = s.VatTuBenhVien.VatTus.Ten;
                    d.TenKhoVatTu = s.Kho.Ten;
                });

            CreateMap<DinhMucVatTuTonKhoViewModel, DinhMucVatTuTonKho>().IgnoreAllNonExisting()
                .ForMember(x => x.VatTuBenhVien, o => o.Ignore())
                .ForMember(x => x.Kho, o => o.Ignore());

            CreateMap<DinhMucVatTuTonKhoGridVo, DinhMucVatTuTonKhoExportExcel>().IgnoreAllNonExisting();
        }
    }
}
