using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhKSNK;
using Camino.Api.Models.LinhThuongKSNK;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauLinhKSNKMappingProfile : Profile
    {
        public YeuCauLinhKSNKMappingProfile()
        {
            CreateMap<YeuCauLinhVatTu, DuyetYeuCauLinhKSNKViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.DuyetYeuCauLinhVatTuChiTiets, o => o.MapFrom(y => y.YeuCauLinhVatTuChiTiets))
                .AfterMap((s, d) =>
                {
                    d.TenKhoNhap = s.NoiYeuCau?.KhoaPhong?.Ten; //s.KhoNhap.KhoaPhong.Ten; //s.KhoNhap != null ? s.KhoNhap.Ten : "";
                    d.TenKhoXuat = s.KhoXuat != null ? s.KhoXuat.Ten : "";
                    d.TenNhanVienYeuCau = s.NhanVienYeuCau != null ? s.NhanVienYeuCau.User.HoTen : "";
                    d.TenNhanVienDuyet = s.NhanVienDuyetId != null ? s.NhanVienDuyet.User.HoTen : "";
                    d.NgayDuyet = s.NgayDuyet ?? DateTime.Now;
                    if (s.XuatKhoVatTus.Any(x => x.NguoiXuat != null))
                    {
                        var xuatChiTiet = s.XuatKhoVatTus.First(x => x.NguoiXuat != null);
                        d.NguoiXuatKhoId = xuatChiTiet.NguoiXuatId;
                        d.TenNguoiXuatKho = xuatChiTiet.NguoiXuat.User.HoTen;
                    }

                    if (s.XuatKhoVatTus.Any(x => x.NguoiNhan != null))
                    {
                        var xuatChiTiet = s.XuatKhoVatTus.First(x => x.NguoiNhan != null);
                        d.NguoiNhapKhoId = xuatChiTiet.NguoiNhanId;
                        d.TenNguoiNhapKho = xuatChiTiet.NguoiNhan.User.HoTen;
                    }
                    else
                    {
                        d.NguoiNhapKhoId = s.NhanVienYeuCauId;
                        d.TenNguoiNhapKho = s.NhanVienYeuCau?.User.HoTen;
                    }
                });

            CreateMap<YeuCauLinhVatTuChiTiet, DuyetYeuCauLinhKSNKChiTietViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.LaVatTuBHYT = s.LaVatTuBHYT;
                    d.TenVatTu = s.VatTuBenhVien.VatTus.Ten;
                    d.DVT = s.VatTuBenhVien.VatTus.DonViTinh;
                });

            CreateMap<DuyetYeuCauLinhKSNKChiTietViewModel, YeuCauLinhVatTuChiTiet>().IgnoreAllNonExisting();

            #region Lĩnh thường Vật tư
            CreateMap<YeuCauLinhVatTuChiTiet, LinhThuongKNSKChiTietViewModel>().IgnoreAllNonExisting()
                 .AfterMap((s, d) =>
                 {
                     d.Ten = s.VatTuBenhVien.VatTus.Ten;
                     d.Ma = s.VatTuBenhVien.VatTus.Ma;
                     d.DVT = s.VatTuBenhVien.VatTus.DonViTinh;
                     d.NhaSX = s.VatTuBenhVien.VatTus.NhaSanXuat;
                     d.NuocSX = s.VatTuBenhVien.VatTus.NuocSanXuat;
                     d.SLYeuCau = s.SoLuong;
                     d.KhoXuatId = s.YeuCauLinhVatTu.KhoXuatId;
                     //d.SoLuongCoTheXuat = s.SoLuongCoTheXuat;
                 });

            CreateMap<LinhThuongKNSKChiTietViewModel, YeuCauLinhVatTuChiTiet>().IgnoreAllNonExisting();
            #endregion

            //#region Lĩnh bù vật tư
            //CreateMap<YeuCauVatTuBenhVien, LinhBuVatTuChiTietViewModel>().IgnoreAllNonExisting()
            //        .AfterMap((s, d) =>
            //        {
            //            d.Ten = s.VatTuBenhVien.VatTus.Ten;
            //            d.DVT = s.VatTuBenhVien.VatTus.DonViTinh;
            //            d.NhaSX = s.VatTuBenhVien.VatTus.NhaSanXuat;
            //            d.NuocSX = s.VatTuBenhVien.VatTus.NuocSanXuat;
            //            d.SLYeuCau = s.SoLuong;
            //            d.LaVatTuBHYT = s.LaVatTuBHYT;
            //        });

            //CreateMap<LinhBuVatTuChiTietViewModel, YeuCauVatTuBenhVien>().IgnoreAllNonExisting();

            //CreateMap<YeuCauLinhVatTuChiTiet, LinhBuVatTuChiTietViewModel>().IgnoreAllNonExisting();
            //CreateMap<LinhBuVatTuChiTietViewModel, YeuCauLinhVatTuChiTiet>().IgnoreAllNonExisting();
            //#endregion

            #region Duyệt
            CreateMap<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu, DuyetYeuCauLinhKSNKViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.DuyetYeuCauLinhVatTuChiTiets, o => o.MapFrom(y => y.YeuCauLinhVatTuChiTiets))
                .AfterMap((s, d) =>
                {
                    d.TenKhoNhap = s.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau?.KhoaPhong?.Ten : s.KhoNhap?.Ten; //s.KhoNhap != null ? s.KhoNhap.Ten : "";
                    d.TenKhoXuat = s.KhoXuat != null ? s.KhoXuat.Ten : "";
                    d.TenNhanVienYeuCau = s.NhanVienYeuCau != null ? s.NhanVienYeuCau.User.HoTen : "";
                    d.TenNhanVienDuyet = s.NhanVienDuyetId != null ? s.NhanVienDuyet.User.HoTen : "";
                    d.NgayDuyet = d.NgayDuyet ?? DateTime.Now;
                    if (s.XuatKhoVatTus.Any(x => x.NguoiXuat != null))
                    {
                        var xuatChiTiet = s.XuatKhoVatTus.First(x => x.NguoiXuat != null);
                        d.NguoiXuatKhoId = xuatChiTiet.NguoiXuatId;
                        d.TenNguoiXuatKho = xuatChiTiet.NguoiXuat.User.HoTen;
                    }

                    if (s.XuatKhoVatTus.Any(x => x.NguoiNhan != null))
                    {
                        var xuatChiTiet = s.XuatKhoVatTus.First(x => x.NguoiNhan != null);
                        d.NguoiNhapKhoId = xuatChiTiet.NguoiNhanId;
                        d.TenNguoiNhapKho = xuatChiTiet.NguoiNhan.User.HoTen;
                    }
                    else
                    {
                        d.NguoiNhapKhoId = s.NhanVienYeuCauId;
                        d.TenNguoiNhapKho = s.NhanVienYeuCau?.User.HoTen;
                    }

                });

            CreateMap<DuyetYeuCauLinhKSNKViewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauLinhVatTuChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (d.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru)
                    {
                        AddOrUpdateDuyetYeuCauLinhThuongChiTiet(s, d);
                    }
                });

            CreateMap<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTuChiTiet, DuyetYeuCauLinhKSNKChiTietViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TenVatTu = s.VatTuBenhVien.VatTus.Ten;
                    d.DVT = s.VatTuBenhVien.VatTus.DonViTinh;
                    d.HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat;
                    d.NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat;
                    d.SoLuongCoTheXuat = s.YeuCauLinhVatTu.DuocDuyet == null && (s.SoLuongCoTheXuat == null || s.SoLuongCoTheXuat != s.SoLuong) ? s.SoLuong : s.SoLuongCoTheXuat;
                    d.Nhom = s.LaVatTuBHYT ? "Thuốc BHYT" : "Thuốc không BHYT";
                    d.isTuChoi = s.YeuCauLinhVatTu.DuocDuyet == false;
                });

            CreateMap<DuyetYeuCauLinhKSNKChiTietViewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTuChiTiet>().IgnoreAllNonExisting();

            #endregion
            #region Không duyệt
            CreateMap<KhongDuyetYeuCauLinhKSNKViewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauLinhVatTuChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (d.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru)
                    {
                        AddOrUpdateDuyetYeuCauLinhThuong(s, d);
                    }
                });
            #endregion
        }

        private void AddOrUpdateDuyetYeuCauLinhThuong(KhongDuyetYeuCauLinhKSNKViewModel viewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu model)
        {
            foreach (var item in viewModel.DuyetYeuCauLinhVatTuChiTiets)
            {
                if (item.Id == 0)
                {
                    var duyetChiTietEntity = new YeuCauLinhVatTuChiTiet();
                    model.YeuCauLinhVatTuChiTiets.Add(item.ToEntity(duyetChiTietEntity));
                }
                else
                {
                    var result = model.YeuCauLinhVatTuChiTiets.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.YeuCauLinhVatTuChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.DuyetYeuCauLinhVatTuChiTiets.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

        private void AddOrUpdateDuyetYeuCauLinhThuongChiTiet(DuyetYeuCauLinhKSNKViewModel viewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu model)
        {
            foreach (var item in viewModel.DuyetYeuCauLinhVatTuChiTiets)
            {
                if (item.Id == 0)
                {
                    var duyetChiTietEntity = new YeuCauLinhVatTuChiTiet();
                    model.YeuCauLinhVatTuChiTiets.Add(item.ToEntity(duyetChiTietEntity));
                }
                else
                {
                    var result = model.YeuCauLinhVatTuChiTiets.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.YeuCauLinhVatTuChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.DuyetYeuCauLinhVatTuChiTiets.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

    }

}
