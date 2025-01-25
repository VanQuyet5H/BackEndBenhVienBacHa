using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhBuDuocPham;
using Camino.Api.Models.LinhDuocPhamTrucTiep;
using Camino.Api.Models.LinhThuongDuocPham;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using System;
using System.Linq;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Services.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauLinhDuocPhamMappingProfile : Profile
    {
        public YeuCauLinhDuocPhamMappingProfile()
        {
            #region LinhThuong
            CreateMap<YeuCauLinhDuocPhamChiTiet, LinhThuongDuocPhamChiTietViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) => 
                {
                    d.Ten = s.DuocPhamBenhVien.DuocPham.Ten;
                    d.HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong;
                    d.HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat;
                    d.DVTId = s.DuocPhamBenhVien.DuocPham.DonViTinhId;
                    d.DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten;
                    d.DuongDungId = s.DuocPhamBenhVien.DuocPham.DuongDungId;
                    d.DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten;
                    d.NhaSX = s.DuocPhamBenhVien.DuocPham.NhaSanXuat;
                    d.NuocSX = s.DuocPhamBenhVien.DuocPham.NuocSanXuat;
                    d.SLYeuCau = s.SoLuong;
                    d.SoLuongCoTheXuat = s.SoLuongCoTheXuat;
                    d.KhoXuatId = s.YeuCauLinhDuocPham.KhoXuatId;                   
                });

            CreateMap<LinhThuongDuocPhamChiTietViewModel,YeuCauLinhDuocPhamChiTiet>().IgnoreAllNonExisting();

            #endregion

            #region LinhBu
            CreateMap<YeuCauDuocPhamBenhVien, LinhBuDuocPhamChiTietViewModel>().IgnoreAllNonExisting()
                    .AfterMap((s, d) =>
                    {
                        d.Ten = s.DuocPhamBenhVien.DuocPham.Ten;
                        d.HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong;
                        d.HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat;
                        d.DVTId = s.DuocPhamBenhVien.DuocPham.DonViTinhId;
                        d.DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten;
                        d.DuongDungId = s.DuocPhamBenhVien.DuocPham.DuongDungId;
                        d.DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten;
                        d.NhaSX = s.DuocPhamBenhVien.DuocPham.NhaSanXuat;
                        d.NuocSX = s.DuocPhamBenhVien.DuocPham.NuocSanXuat;
                        d.SLYeuCau = s.SoLuong;
                        d.LaDuocPhamBHYT = s.LaDuocPhamBHYT;
                    });

            CreateMap<LinhBuDuocPhamChiTietViewModel, YeuCauDuocPhamBenhVien>().IgnoreAllNonExisting();

            CreateMap<YeuCauLinhDuocPhamChiTiet, LinhBuDuocPhamChiTietViewModel>().IgnoreAllNonExisting();
            CreateMap<LinhBuDuocPhamChiTietViewModel, YeuCauLinhDuocPhamChiTiet>().IgnoreAllNonExisting();
            #endregion

            #region Duyệt
            CreateMap<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham, DuyetYeuCauLinhDuocPhamViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.DuyetYeuCauLinhDuocPhamChiTiets, o => o.MapFrom(y => y.YeuCauLinhDuocPhamChiTiets))
                .AfterMap((s, d) =>
                {
                    d.TenKhoNhap = s.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau?.KhoaPhong?.Ten : s.KhoNhap?.Ten; //s.KhoNhap != null ? s.KhoNhap.Ten : "";
                    d.TenKhoXuat = s.KhoXuat != null ? s.KhoXuat.Ten : "";
                    d.TenNhanVienYeuCau = s.NhanVienYeuCau != null ? s.NhanVienYeuCau.User.HoTen : "";
                    d.TenNhanVienDuyet = s.NhanVienDuyetId != null ? s.NhanVienDuyet.User.HoTen : "";
                    d.NgayDuyet = d.NgayDuyet ?? DateTime.Now;
                    if (s.XuatKhoDuocPhams.Any(x => x.NguoiXuat != null))
                    {
                        var xuatChiTiet = s.XuatKhoDuocPhams.First(x => x.NguoiXuat != null);
                        d.NguoiXuatKhoId = xuatChiTiet.NguoiXuatId;
                        d.TenNguoiXuatKho = xuatChiTiet.NguoiXuat.User.HoTen;
                    }

                    if (s.XuatKhoDuocPhams.Any(x => x.NguoiNhan != null))
                    {
                        var xuatChiTiet = s.XuatKhoDuocPhams.First(x => x.NguoiNhan != null);
                        d.NguoiNhapKhoId = xuatChiTiet.NguoiNhanId;
                        d.TenNguoiNhapKho = xuatChiTiet.NguoiNhan.User.HoTen;
                    }
                    else
                    {
                        d.NguoiNhapKhoId = s.NhanVienYeuCauId;
                        d.TenNguoiNhapKho = s.NhanVienYeuCau?.User.HoTen;
                    }

                });

            CreateMap<DuyetYeuCauLinhDuocPhamViewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauLinhDuocPhamChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (d.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru)
                    {
                        AddOrUpdateDuyetYeuCauLinhThuongChiTiet(s, d);
                    }
                });

            CreateMap<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet, DuyetYeuCauLinhDuocPhamChiTietViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TenDuocPham = s.DuocPhamBenhVien.DuocPham.Ten;
                    d.NongDoHamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong;
                    d.HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat;
                    d.DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten;
                    d.HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat;
                    d.NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat;
                    d.SoLuongCoTheXuat = s.YeuCauLinhDuocPham.DuocDuyet == null && (s.SoLuongCoTheXuat == null || s.SoLuongCoTheXuat != s.SoLuong) ? s.SoLuong : s.SoLuongCoTheXuat;
                    d.Nhom = s.LaDuocPhamBHYT ? "Thuốc BHYT" : "Thuốc không BHYT";
                    d.isTuChoi = s.YeuCauLinhDuocPham.DuocDuyet == false;
                });

            CreateMap<DuyetYeuCauLinhDuocPhamChiTietViewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet>().IgnoreAllNonExisting();

            #endregion
            #region LinhTT
            CreateMap<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet, LinhTrucTiepDuocPhamChiTietViewModel>().IgnoreAllNonExisting();

            CreateMap<LinhTrucTiepDuocPhamChiTietViewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet>().IgnoreAllNonExisting();
            #endregion

            #region Không duyệt
            CreateMap<KhongDuyetYeuCauLinhViewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauLinhDuocPhamChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (d.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru)
                    {
                        AddOrUpdateDuyetYeuCauLinhThuong(s, d);
                    }
                });

            #endregion

        }

        private void AddOrUpdateDuyetYeuCauLinhThuong(KhongDuyetYeuCauLinhViewModel viewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham model)
        {
            foreach (var item in viewModel.DuyetYeuCauLinhDuocPhamChiTiets)
            {
                if (item.Id == 0)
                {
                    var duyetChiTietEntity = new YeuCauLinhDuocPhamChiTiet();
                    model.YeuCauLinhDuocPhamChiTiets.Add(item.ToEntity(duyetChiTietEntity));
                }
                else
                {
                    var result = model.YeuCauLinhDuocPhamChiTiets.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.YeuCauLinhDuocPhamChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.DuyetYeuCauLinhDuocPhamChiTiets.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

        private void AddOrUpdateDuyetYeuCauLinhThuongChiTiet(DuyetYeuCauLinhDuocPhamViewModel viewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham model)
        {
            foreach (var item in viewModel.DuyetYeuCauLinhDuocPhamChiTiets)
            {
                if (item.Id == 0)
                {
                    var duyetChiTietEntity = new YeuCauLinhDuocPhamChiTiet();
                    model.YeuCauLinhDuocPhamChiTiets.Add(item.ToEntity(duyetChiTietEntity));
                }
                else
                {
                    var result = model.YeuCauLinhDuocPhamChiTiets.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.YeuCauLinhDuocPhamChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.DuyetYeuCauLinhDuocPhamChiTiets.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }
    }
}
