using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuKyThuatBenhVien;
using Camino.Core.Domain.Entities.DichVuKyThuats;

namespace Camino.Api.Models.MappingProfile
{
    public class DichVuKyThuatBenhVienTiemChungMappingProfile : Profile
    {
        public DichVuKyThuatBenhVienTiemChungMappingProfile()
        {
            CreateMap<DichVuKyThuatBenhVienTiemChung, DichVuKyThuatBenhVienTiemChungViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.SoDangKy = s.DuocPhamBenhVien?.DuocPham?.SoDangKy;
                    d.MaDuocPhamBenhVien = s.DuocPhamBenhVien?.MaDuocPhamBenhVien;
                    d.Ten = s.DuocPhamBenhVien?.DuocPham?.Ten;
                    d.MaHoatChat = s.DuocPhamBenhVien?.DuocPham?.MaHoatChat;
                    d.HoatChat = s.DuocPhamBenhVien?.DuocPham?.HoatChat;
                    d.NhaSanXuat = s.DuocPhamBenhVien?.DuocPham?.NhaSanXuat;
                    d.NuocSanXuat = s.DuocPhamBenhVien?.DuocPham?.NuocSanXuat;
                    d.QuyCach = s.DuocPhamBenhVien?.DuocPham?.QuyCach;
                });
            CreateMap<DichVuKyThuatBenhVienTiemChungViewModel, DichVuKyThuatBenhVienTiemChung>().IgnoreAllNonExisting();
        }
    }
}
