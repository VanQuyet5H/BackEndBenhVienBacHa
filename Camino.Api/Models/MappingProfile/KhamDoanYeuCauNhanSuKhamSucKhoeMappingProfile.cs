using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain.Entities.KhamDoans;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamDoanYeuCauNhanSuKhamSucKhoeMappingProfile : Profile
    {
        public KhamDoanYeuCauNhanSuKhamSucKhoeMappingProfile()
        {

            CreateMap<YeuCauNhanSuKhamSucKhoe, YeuCauNhanSuKhamSucKhoeViewModel>()
                .ForMember(q => q.CongTyId, o => o.MapFrom(s => s.HopDongKhamSucKhoe.CongTyKhamSucKhoeId))
                .ForMember(q => q.CongTy, o => o.MapFrom(s => s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten))
                .ForMember(q => q.NhanVienGui, o => o.MapFrom(s => s.NhanVienGuiYeuCau.User.HoTen))
                .ForMember(q => q.SoNguoiKham, o => o.MapFrom(s => s.HopDongKhamSucKhoe.SoNguoiKham))
                .ForMember(q => q.NgayHieuLuc, o => o.MapFrom(s => s.HopDongKhamSucKhoe.NgayHieuLuc))
                .ForMember(q => q.NgayKetThuc, o => o.MapFrom(s => s.HopDongKhamSucKhoe.NgayKetThuc))
                .ForMember(q => q.TongSoBs, o => o.MapFrom(s => s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 1)))
                .ForMember(q => q.TongSoDd, o => o.MapFrom(s => s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 5)))
                .ForMember(q => q.TongNvKhac, o => o.MapFrom(s => s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId != 1 && w.NhanVien.ChucDanh.NhomChucDanhId != 5 || w.NhanVien.ChucDanh == null)))
                .ForMember(q => q.HopDongKhamSucKhoeDiaDiems, o => o.MapFrom(s => s.HopDongKhamSucKhoe.HopDongKhamSucKhoeDiaDiems))
                .ForMember(q => q.NhanVienKhthDuyet, o => o.MapFrom(s => s.NhanVienKHTHDuyet.User.HoTen))
                .ForMember(q => q.NhanVienNhanSuDuyet, o => o.MapFrom(s => s.NhanVienNhanSuDuyet.User.HoTen))
                .ForMember(q => q.GiamDoc, o => o.MapFrom(s => s.GiamDoc.User.HoTen))
                .ForMember(q => q.YeuCauNhanSuKhamSucKhoeChiTiets, o => o.MapFrom(s => s.YeuCauNhanSuKhamSucKhoeChiTiets));

            CreateMap<YeuCauNhanSuKhamSucKhoeViewModel, YeuCauNhanSuKhamSucKhoe>()
                .ForMember(q => q.HopDongKhamSucKhoe, w => w.Ignore())
                .ForMember(q => q.YeuCauNhanSuKhamSucKhoeChiTiets, w => w.Ignore());

            CreateMap<YeuCauNhanSuKhamSucKhoeChiTiet, YeuCauNhanSuKhamSucKhoeChiTietViewModel>()
                .ForMember(q => q.NguoiGioiThieu, o => o.MapFrom(s => s.NguoiGioiThieu.User.HoTen))
                .ForMember(q => q.NhanSuKhamSucKhoeTaiLieuDinhKem, o => o.Ignore())
                .AfterMap((source, viewModel) =>
                {
                    
                    viewModel.LoaiNhanVien = source.NhanVien != null && source.NhanVien.ChucDanh != null && source.NhanVien.ChucDanh.NhomChucDanhId == 1 ? LoaiNhanVien.BacSi :
                        source.NhanVien != null && source.NhanVien.ChucDanh != null && source.NhanVien.ChucDanh.NhomChucDanhId == 5 ? LoaiNhanVien.DieuDuong : LoaiNhanVien.NhanVienKhac;
                    if (source.NhanSuKhamSucKhoeTaiLieuDinhKemId != null)
                    {
                        var nhanSuKhamSucKhoeTaiLieuDinhKemViewModel = source.NhanSuKhamSucKhoeTaiLieuDinhKem.ToModel<NhanSuKhamSucKhoeTaiLieuDinhKemViewModel>();
                        viewModel.NhanSuKhamSucKhoeTaiLieuDinhKem.Add(nhanSuKhamSucKhoeTaiLieuDinhKemViewModel);
                    }
                });

            CreateMap<YeuCauNhanSuKhamSucKhoeChiTietViewModel, YeuCauNhanSuKhamSucKhoeChiTiet>()
                .ForMember(q => q.NhanVien, o => o.Ignore())
                .ForMember(q => q.NguoiGioiThieu, o => o.Ignore())
                .ForMember(q => q.YeuCauNhanSuKhamSucKhoe, o => o.Ignore())
                .ForMember(q => q.NhanSuKhamSucKhoeTaiLieuDinhKem, o => o.MapFrom(s => s.NhanSuKhamSucKhoeTaiLieuDinhKem.FirstOrDefault()))
                .AfterMap((source, entity) =>
                {
                    if (source.IsCreate == true)
                    {
                        entity.Id = 0;
                    }
                });

            CreateMap<NhanSuKhamSucKhoeTaiLieuDinhKem, NhanSuKhamSucKhoeTaiLieuDinhKemViewModel>().IgnoreAllNonExisting();
            CreateMap<NhanSuKhamSucKhoeTaiLieuDinhKemViewModel, NhanSuKhamSucKhoeTaiLieuDinhKem>()
                .ForMember(q => q.YeuCauNhanSuKhamSucKhoeChiTiets, w => w.Ignore());

            CreateMap<HopDongKhamSucKhoeDiaDiem, HopDongKhamSucKhoeDiaDiemViewModel>().IgnoreAllNonExisting();
        }
    }
}
