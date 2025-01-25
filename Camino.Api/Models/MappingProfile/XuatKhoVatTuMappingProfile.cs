using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class XuatKhoVatTuMappingProfile : Profile
    {
        public XuatKhoVatTuMappingProfile()
        {
            CreateMap<XuatKhoVatTu, XuatKhoVatTuViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.NguoiXuatDisplay, o => o.MapFrom(s => s.NguoiXuat.User.HoTen))
                .ForMember(d => d.NguoiNhanDisplay, o => o.MapFrom(s => s.NguoiNhan.User.HoTen))
                .ForMember(d => d.LoaiXuatKhoDisplay, o => o.MapFrom(s => s.LoaiXuatKho.GetDescription()))
                //.ForMember(d => d.KhoDuocPhamXuatDisplay, o => o.MapFrom(s => s.KhoDuocPhamXuat.Ten))
                //.ForMember(d => d.KhoDuocPhamNhapDisplay, o => o.MapFrom(s => s.KhoDuocPhamNhap.Ten))
                //.ForMember(d => d.KhoDuocPhamXuatDisplay, o => o.MapFrom(s => s.KhoDuocPhamXuat.Ten))
                .ForMember(d => d.KhoXuatId, o => o.MapFrom(s => s.KhoXuatId))
                .ForMember(d => d.KhoNhapId, o => o.MapFrom(s => s.KhoNhapId));

            CreateMap<XuatKhoDuocPham, XuatKhoVatTuViewModel>()
               .IgnoreAllNonExisting()
               .ForMember(d => d.NguoiXuatDisplay, o => o.MapFrom(s => s.NguoiXuat.User.HoTen))
               .ForMember(d => d.NguoiNhanDisplay, o => o.MapFrom(s => s.NguoiNhan.User.HoTen))
               .ForMember(d => d.LoaiXuatKhoDisplay, o => o.MapFrom(s => s.LoaiXuatKho.GetDescription()))
               .ForMember(d => d.KhoXuatId, o => o.MapFrom(s => s.KhoXuatId))
               .ForMember(d => d.KhoNhapId, o => o.MapFrom(s => s.KhoNhapId));

            CreateMap<XuatKhoVatTuViewModel, XuatKhoVatTu>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.KhoXuatId, o => o.MapFrom(s => s.KhoXuatId))
                .ForMember(d => d.KhoNhapId, o => o.MapFrom(s => s.KhoNhapId));

            CreateMap<XuatKhoVatTuChiTiet, VatTuXuatChiTiet>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.TenVatTu, o => o.MapFrom(s => s.VatTuBenhVien.VatTus.Ten))
                //.ForMember(d => d.loais, o => o.MapFrom(s => s.XuatKhoVatTuChiTietViTris.First().NhapKhoVatTuChiTiet.VatTuBenhVienPhanNhom.Ten))
                .ForMember(d => d.DVT, o => o.MapFrom(s => s.VatTuBenhVien.VatTus.DonViTinh))
                .ForMember(d => d.Loai, o => o.MapFrom(s => s.XuatKhoVatTuChiTietViTris.First().NhapKhoVatTuChiTiet.LaVatTuBHYT ? "BHYT" : "Không BHYT"))
                .ForMember(d => d.SoLuongXuat, o => o.MapFrom(s => s.XuatKhoVatTuChiTietViTris.Sum(p => p.SoLuongXuat)))
                ;

            CreateMap<VatTuXuatChiTiet, XuatKhoVatTuChiTiet>()
                .ForMember(d => d.Id, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    long.TryParse(s.Id.Split(",").First(), out long vatTuId);
                    d.VatTuBenhVienId = vatTuId;
                })
                ;


            CreateMap<VatTuXuatChiTiet, XuatKhoVatTuChiTiet>()
                .ForMember(d => d.Id, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    long.TryParse(s.Id.Split(",").First(), out long vatTuId);
                    d.VatTuBenhVienId = vatTuId;
                })
                ;

            CreateMap<XuatKhoVatTuChiTiet, XuatKhoVatTuChiTietViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.TongSoLuongXuat, o => o.MapFrom(s => s.XuatKhoVatTuChiTietViTris.Select(p => p.SoLuongXuat).Sum()))
                .ForMember(d => d.VatTuId, o => o.MapFrom(s => s.VatTuBenhVienId))
                .ForMember(d => d.TenVatTu, o => o.MapFrom(s => s.VatTuBenhVien.VatTus.Ten))
                //TODO update entity kho on 9/9/2020
                //.ForMember(d => d.ChatLuong, o => o.MapFrom(s => s.DatChatLuong ? "Đạt" : "Không đạt"))
                .ForMember(d => d.VAT, o => o.MapFrom(s => s.XuatKhoVatTu.NhapKhoVatTus
                    .FirstOrDefault(p => p.XuatKhoVatTuId == s.XuatKhoVatTuId).NhapKhoVatTuChiTiets
                    .FirstOrDefault(p => p.VatTuBenhVienId == s.VatTuBenhVienId).VAT));
            //TODO update entity kho on 9/9/2020
            //.ForMember(d => d.DonGiaBan, o => o.MapFrom(s => s.XuatKhoDuocPham.NhapKhoDuocPhams
            //    .FirstOrDefault(p => p.XuatKhoDuocPhamId == s.XuatKhoDuocPhamId).NhapKhoDuocPhamChiTiets
            //    .FirstOrDefault(p => p.DuocPhamBenhVienId == s.DuocPhamBenhVienId).DonGiaBan))
            //.ForMember(d => d.ChietKhau, o => o.MapFrom(s => s.XuatKhoDuocPham.NhapKhoDuocPhams
            //    .FirstOrDefault(p => p.XuatKhoDuocPhamId == s.XuatKhoDuocPhamId).NhapKhoDuocPhamChiTiets
            //    .FirstOrDefault(p => p.DuocPhamBenhVienId == s.DuocPhamBenhVienId).ChietKhau));
            //CreateMap<XuatKhoDuocPhamChiTietViewModel, XuatKhoDuocPhamChiTiet>()
            //    .IgnoreAllNonExisting()
            //    .ForMember(d => d.DuocPhamBenhVienId, o => o.MapFrom(s => s.DuocPhamId));
            CreateMap<XuatKhoVatTuChiTietViewModel, XuatKhoVatTuChiTiet>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.VatTuBenhVienId, o => o.MapFrom(s => s.VatTuId));

            CreateMap<XuatKhoVatTuChiTietViTri, XuatKhoVatTuChiTietViTriViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.SoLo, o => o.MapFrom(s => s.NhapKhoVatTuChiTiet.Solo))
                .ForMember(d => d.HanSuDungDisplay, o => o.MapFrom(s => s.NhapKhoVatTuChiTiet.HanSuDung.ApplyFormatDate()))
                .ForMember(d => d.SoLuongXuat, o => o.MapFrom(s => s.SoLuongXuat))
                .ForMember(d => d.ViTri, o => o.MapFrom(s => s.NhapKhoVatTuChiTiet.KhoViTri.Ten));

            CreateMap<XuatKhoVatTuChiTietViTriViewModel, XuatKhoVatTuChiTietViTri>()
                .IgnoreAllNonExisting();

            #region export excel

            CreateMap<XuatKhoVatTuGridVo, XuatKhoVatTuExportExcel>().IgnoreAllNonExisting();
            CreateMap<XuatKhoVatTuChildrenGridVo, XuatKhoVatTuExportExcelChild>().IgnoreAllNonExisting();


            CreateMap<Camino.Core.Domain.ValueObject.XuatKhoKSNK.XuatKhoKSNKGridVo, XuatKhoKSNKExportExcel>().IgnoreAllNonExisting();
            CreateMap<Camino.Core.Domain.ValueObject.XuatKhoKSNK.XuatKhoKSNKChildrenGridVo, XuatKhoKSNKVatTuVaDuocPhamExportExcelChild>().IgnoreAllNonExisting();
            CreateMap<Camino.Core.Domain.ValueObject.XuatKhoKSNK.XuatKhoDuocPhamChildrenGridVo, XuatKhoKSNKVatTuVaDuocPhamExportExcelChild>().IgnoreAllNonExisting();


            #endregion export excel


            #region XuatKhac
            CreateMap<YeuCauXuatKhoVatTu, XuatKhoVatTuKhacViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauXuatKhoVatTuChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoXuat = s.KhoXuat?.Ten;
                    d.TenNguoiXuat = s.NguoiXuat?.User.HoTen;
                    d.TenNguoiNhan = s.NguoiNhan?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                    d.TenNhaThau = s.NhaThau?.Ten;
                });
            CreateMap<XuatKhoVatTuKhacViewModel, YeuCauXuatKhoVatTu>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauXuatKhoVatTuChiTiets, o => o.Ignore())
                ;

            CreateMap<XuatKhoVatTu, XuatKhoVatTuKhacViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauXuatKhoVatTuChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoXuat = s.KhoVatTuXuat?.Ten;
                    d.TenNguoiXuat = s.NguoiXuat?.User.HoTen;
                    d.TenNguoiNhan = s.NguoiNhan?.User.HoTen;
                    //d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                    d.TenNhaThau = s.NhaThau?.Ten;

                });
            CreateMap<XuatKhoVatTuKhacViewModel, XuatKhoVatTu>().IgnoreAllNonExisting();

            CreateMap<XuatKhoVatTuKhacGridVo, XuatKhoVatTuKhacExportExcel>();
            CreateMap<YeuCauXuatKhoVatTuGridVo, XuatKhoVatTuKhacExportExcelChild>();
            #endregion

            #region XuatKhac ksnk

            CreateMap<XuatKhoKSNKs.XuatKhoKSNKKhacViewModel, Core.Domain.ValueObject.XuatKhoKSNK.XuatKhoKhacKSNKVo>().IgnoreAllNonExisting();
            CreateMap<YeuCauXuatKhoVatTu, XuatKhoKSNKs.XuatKhoKSNKKhacViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauXuatKhoVatTuChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoXuat = s.KhoXuat?.Ten;
                    d.TenNguoiXuat = s.NguoiXuat?.User.HoTen;
                    d.TenNguoiNhan = s.NguoiNhan?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                    d.TenNhaThau = s.NhaThau?.Ten;
                });
            CreateMap<XuatKhoKSNKs.XuatKhoKSNKKhacViewModel, YeuCauXuatKhoVatTu>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauXuatKhoVatTuChiTiets, o => o.Ignore())
                ;

            CreateMap<XuatKhoVatTu, XuatKhoKSNKs.XuatKhoKSNKKhacViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauXuatKhoVatTuChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoXuat = s.KhoVatTuXuat?.Ten;
                    d.TenNguoiXuat = s.NguoiXuat?.User.HoTen;
                    d.TenNguoiNhan = s.NguoiNhan?.User.HoTen;
                    //d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                    d.TenNhaThau = s.NhaThau?.Ten;

                });


            CreateMap<XuatKhoDuocPham, XuatKhoKSNKs.XuatKhoKSNKKhacViewModel>().IgnoreAllNonExisting()
             .ForMember(x => x.YeuCauXuatKhoVatTuChiTiets, o => o.Ignore())
             .AfterMap((s, d) =>
             {
                 d.TenKhoXuat = s.KhoDuocPhamXuat?.Ten;
                 d.TenNguoiXuat = s.NguoiXuat?.User.HoTen;
                 d.TenNguoiNhan = s.NguoiNhan?.User.HoTen;
                 d.TenNhaThau = s.NhaThau?.Ten;
             });

            CreateMap<YeuCauXuatKhoDuocPham, XuatKhoKSNKs.XuatKhoKSNKKhacViewModel>().IgnoreAllNonExisting()
               .ForMember(x => x.YeuCauXuatKhoVatTuChiTiets, o => o.Ignore())
               .AfterMap((s, d) =>
               {
                   d.TenKhoXuat = s.KhoXuat?.Ten;
                   d.TenNguoiXuat = s.NguoiXuat?.User.HoTen;
                   d.TenNguoiNhan = s.NguoiNhan?.User.HoTen;
                   d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                   d.TenNhaThau = s.NhaThau?.Ten;
               });

            CreateMap<XuatKhoKSNKs.XuatKhoKSNKKhacViewModel, XuatKhoVatTu>().IgnoreAllNonExisting();

            CreateMap<XuatKhoVatTuKhacGridVo, XuatKhoVatTuKhacExportExcel>();
            CreateMap<YeuCauXuatKhoVatTuGridVo, XuatKhoVatTuKhacExportExcelChild>();

            CreateMap<Core.Domain.ValueObject.XuatKhoKSNK.XuatKhoVatTuKhacGridVo, Core.Domain.ValueObject.XuatKhoKSNK.XuatKhoKhacKSNKExportExcel>();
            CreateMap<Core.Domain.ValueObject.XuatKhoKSNK.YeuCauXuatKhoKSNKGridVo, Core.Domain.ValueObject.XuatKhoKSNK.XuatKhoKhacKSNKExportExcelChild>();

            #endregion
        }
    }
}
