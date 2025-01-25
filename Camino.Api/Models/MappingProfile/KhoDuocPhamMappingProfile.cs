using Camino.Api;
using AutoMapper;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Api.Models.KhoDuocPhams;
using Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Api.Extensions;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class KhoDuocPhamMappingProfile : Profile
    {
        public KhoDuocPhamMappingProfile()
        {
            CreateMap<Kho, KhoDuocPhamViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienIds, o => o.MapFrom(s => s.KhoNhanVienQuanLys.Select(p2 => p2.NhanVienId).Distinct()))
                .ForMember(p => p.KhoaPhong, o => o.MapFrom(s => s.KhoaPhong.Ten))
                .ForMember(p => p.PhongBenhVien, o => o.MapFrom(s => s.PhongBenhVien.Ten));

            CreateMap<KhoDuocPhamViewModel, Kho>()
                .ForMember(d => d.KhoNhanVienQuanLys, o => o.Ignore())
                .ForMember(d => d.KhoaPhong, o => o.Ignore())
                .ForMember(d => d.PhongBenhVien, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore())
                .IgnoreAllNonExisting();
            CreateMap<KhoDuocPhamGridVo, KhoDuocPhamExportExcel>().IgnoreAllNonExisting();
        }
    }
}
