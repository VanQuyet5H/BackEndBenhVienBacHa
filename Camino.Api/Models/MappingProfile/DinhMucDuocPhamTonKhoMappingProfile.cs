using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.DinhMucDuocPhamTonKho;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class DinhMucDuocPhamTonKhoMappingProfile : Profile
    {
        public DinhMucDuocPhamTonKhoMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho, DinhMucDuocPhamTonKho.DinhMucDuocPhamTonKhoViewModel>().IgnoreAllNonExisting().AfterMap((s, d) =>
            {
                d.TenDuocPham = s.DuocPhamBenhVien?.DuocPham?.Ten;
                d.TenKhoDuocPham = s.KhoDuocPham?.Ten;
            });

            CreateMap<DinhMucDuocPhamTonKho.DinhMucDuocPhamTonKhoViewModel, Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho>().IgnoreAllNonExisting()
                .ForMember(x => x.KhoDuocPham, o => o.Ignore())
                .ForMember(x => x.DuocPhamBenhVien, o => o.Ignore());

            CreateMap<DinhMucDuocPhamTonKhoGridVo, DinhMucDuocPhamTonKhoExportExcel>().IgnoreAllNonExisting();
        }
    }
}
