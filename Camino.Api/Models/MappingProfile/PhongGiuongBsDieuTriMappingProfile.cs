using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Api.Models.MappingProfile
{
    public class PhongGiuongBsDieuTriMappingProfile : Profile
    {
        public PhongGiuongBsDieuTriMappingProfile()
        {
            CreateMap<NoiTruEkipDieuTri, EkipDieuTriViewModel>();
            CreateMap<EkipDieuTriViewModel, NoiTruEkipDieuTri>();

            CreateMap<NoiTruKhoaPhongDieuTri, KhoaPhongDieuTriViewModel>()
                .AfterMap((s, d) =>
                {
                    d.KhoaPhongChuyenDiDisplay = s.KhoaPhongChuyenDi?.Ten;
                    d.KhoaPhongChuyenDenDisplay = s.KhoaPhongChuyenDen?.Ten;
                    d.ChanDoanVaoKhoaICDDisplay = s.ChanDoanVaoKhoaICD?.TenTiengViet;
                });

            CreateMap<KhoaPhongDieuTriViewModel, NoiTruKhoaPhongDieuTri>();

            CreateMap<ChiTietSuDungGiuongTheoNgayViewModel, ChiTietSuDungGiuongTheoNgayVo>().IgnoreAllNonExisting();
            CreateMap<SuDungGiuongTheoNgayViewModel, SuDungGiuongTheoNgayVo>().IgnoreAllNonExisting();
            //CreateMap<GiuongBenhVienChiPhiViewModel, GiuongBenhVienChiPhiVo>().IgnoreAllNonExisting();
            CreateMap<ChiTietGiuongBenhVienChiPhiViewModel, ChiTietGiuongBenhVienChiPhiVo>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
//                    d.LaDichVuTrongGoi = s.YeuCauGoiDichVuId != null;
//                    d.LaDichVuKhuyenMai = s.CoDichVuNayTrongGoiKhuyenMai && s.CoThongTinMienGiam;
                });

            CreateMap<ChiTietSuDungGiuongTheoNgayVo, ChiTietSuDungGiuongTheoNgayViewModel>().IgnoreAllNonExisting();
            CreateMap<SuDungGiuongTheoNgayVo, SuDungGiuongTheoNgayViewModel>().IgnoreAllNonExisting();
            //CreateMap<GiuongBenhVienChiPhiVo, GiuongBenhVienChiPhiViewModel>().IgnoreAllNonExisting();
            CreateMap<ChiTietGiuongBenhVienChiPhiVo, ChiTietGiuongBenhVienChiPhiViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.LaDichVuTrongGoi = s.YeuCauGoiDichVuId != null;
                    d.LaDichVuKhuyenMai = s.CoDichVuNayTrongGoiKhuyenMai && s.CoThongTinMienGiam;
                });
        }
    }
}
