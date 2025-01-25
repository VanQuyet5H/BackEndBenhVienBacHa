using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Thuoc;
using Camino.Api.Models.TonKho;
using Camino.Api.Models.VatTu;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Domain.ValueObject.TonKhos;

namespace Camino.Api.Models.MappingProfile
{
    public class DuocPhamMappingProfile : Profile
    {
        public DuocPhamMappingProfile()
        {
            CreateMap<DuocPham, DuocPhamViewModel>()
                .ForMember(x => x.DonViTinh, o => o.MapFrom(y => y.DonViTinh))
                .ForMember(x => x.TenDuongDung, o => o.MapFrom(y => y.DuongDung))
                 .AfterMap((s, d) =>
                 {
                     d.DuocPhamBenhVienPhanNhomModelText = s.Ten;
                 })
                .IgnoreAllNonExisting();
            CreateMap<DuocPhamViewModel, DuocPham>()
                .ForMember(x => x.DonViTinh, o => o.Ignore())
                .ForMember(x => x.DuongDung, o => o.Ignore()).IgnoreAllNonExisting();
            CreateMap<DuocPhamGridVo, DuocPhamExportExcel>().IgnoreAllNonExisting();



            CreateMap<CapNhatTonKhoDuocPhamViewModel, CapNhatTonKhoItem>().IgnoreAllNonExisting();
            CreateMap<CapNhatTonKhoItem, CapNhatTonKhoDuocPhamViewModel>().IgnoreAllNonExisting();

            CreateMap<CapNhatTonKhoDuocPhamChiTietViewModel, NhapXuatTonKhoCapNhatDetailGridVo>().IgnoreAllNonExisting();
            CreateMap<NhapXuatTonKhoCapNhatDetailGridVo, CapNhatTonKhoDuocPhamChiTietViewModel>().IgnoreAllNonExisting();
        }
    }
}
