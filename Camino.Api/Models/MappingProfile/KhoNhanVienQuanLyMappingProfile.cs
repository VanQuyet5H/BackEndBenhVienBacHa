using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhoDuocPhams;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;

namespace Camino.Api.Models.MappingProfile
{
    public class KhoNhanVienQuanLyMappingProfile : Profile
    {
        public KhoNhanVienQuanLyMappingProfile()
        {
            CreateMap<KhoNhanVienQuanLy, KhoDuocPhamViewModel>()
                .IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.Ten = s.Kho.Ten;
                    //d.NhanVien = s.NhanVien.User.HoTen;
                    d.PhongBenhVienId = s.Kho.PhongBenhVienId.GetValueOrDefault();
                    d.PhongBenhVien = s.Kho.PhongBenhVien != null ? s.Kho.PhongBenhVien.Ma + " - " + s.Kho.PhongBenhVien.Ten : string.Empty;
                    d.KhoaPhongId = s.Kho.KhoaPhongId.GetValueOrDefault();
                    d.KhoaPhong = s.Kho.KhoaPhong != null ? s.Kho.KhoaPhong.Ma + " - " + s.Kho.KhoaPhong.Ten : string.Empty;
                    d.LoaiKho = s.Kho.LoaiKho;
                });
        }
    }
}
