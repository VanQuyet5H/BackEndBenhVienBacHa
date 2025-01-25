using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.VatTu;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.TonKhos;
using Camino.Core.Domain.ValueObject.VatTu;

namespace Camino.Api.Models.MappingProfile
{
    public class VatTuMappingProfile: Profile
    {
        public VatTuMappingProfile()
        {
            CreateMap<Core.Domain.Entities.VatTus.VatTu, VatTu.VatTuViewModel>().IgnoreAllNonExisting();

            CreateMap<VatTu.VatTuViewModel, Core.Domain.Entities.VatTus.VatTu>().IgnoreAllNonExisting();

            CreateMap<NhapKhoVatTuTonKhoViewModel, CapNhatTonKhoVatTuVo>().IgnoreAllNonExisting();
            CreateMap<CapNhatTonKhoVatTuVo, NhapKhoVatTuTonKhoViewModel>().IgnoreAllNonExisting();

            CreateMap<NhapKhoVatTuChiTietTonKhoViewModel, CapNhatTonKhoVatTuChiTietVo>().IgnoreAllNonExisting();
            CreateMap<CapNhatTonKhoVatTuChiTietVo, NhapKhoVatTuChiTietTonKhoViewModel>().IgnoreAllNonExisting();

            CreateMap<VatTuGridVo, VatTuExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));
        }
    }
}
