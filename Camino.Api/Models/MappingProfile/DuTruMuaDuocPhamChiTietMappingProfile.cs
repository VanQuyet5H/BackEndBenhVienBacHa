using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DuTruMuaDuocPham;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;

namespace Camino.Api.Models.MappingProfile
{
    public class DuTruMuaDuocPhamChiTietMappingProfile : Profile
    {
        public DuTruMuaDuocPhamChiTietMappingProfile()
        {
            CreateMap<DuTruMuaDuocPhamChiTiet, DuTruMuaDuocPhamChiTietViewModel>().IgnoreAllNonExisting()
              .AfterMap((s, d) =>
              {
                  d.Ten = s.DuocPham.Ten;
                  d.HamLuong = s.DuocPham.HamLuong;
                  d.HoatChat = s.DuocPham.HoatChat;
                  d.DonViTinhId = s.DuocPham.DonViTinhId;
                  d.DVT = s.DuocPham.DonViTinh.Ten;
                  d.DuongDungId = s.DuocPham.DuongDungId;
                  d.TenDuongDung = s.DuocPham.DuongDung.Ten;
                  d.NhaSX = s.DuocPham.NhaSanXuat;
                  d.NuocSX = s.DuocPham.NuocSanXuat;
                  d.SoLuongDuTruTruongKhoaDuyet = s.SoLuongDuTruTruongKhoaDuyet;
              });

            CreateMap<DuTruMuaDuocPhamChiTietViewModel, DuTruMuaDuocPhamChiTiet>().IgnoreAllNonExisting();


            CreateMap<DuocPham, DuocPhamDuTruGridViewModel>().IgnoreAllNonExisting();
            CreateMap<DuocPhamDuTruGridViewModel, DuocPham>().IgnoreAllNonExisting();
            
        }
    }
}
