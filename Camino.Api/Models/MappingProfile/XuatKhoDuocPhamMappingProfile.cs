using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class XuatKhoDuocPhamMappingProfile : Profile
    {
        public XuatKhoDuocPhamMappingProfile()
        {
            CreateMap<XuatKhoDuocPham, XuatKhoDuocPhamViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.NguoiXuatDisplay, o => o.MapFrom(s => s.NguoiXuat.User.HoTen))
                .ForMember(d => d.NguoiNhanDisplay, o => o.MapFrom(s => s.NguoiNhan.User.HoTen))
                .ForMember(d => d.LoaiXuatKhoDisplay, o => o.MapFrom(s => s.LoaiXuatKho.GetDescription()))
                .ForMember(d => d.KhoDuocPhamXuatDisplay, o => o.MapFrom(s => s.KhoDuocPhamXuat.Ten))
                .ForMember(d => d.KhoDuocPhamNhapDisplay, o => o.MapFrom(s => s.KhoDuocPhamNhap.Ten))
                .ForMember(d => d.KhoDuocPhamXuatDisplay, o => o.MapFrom(s => s.KhoDuocPhamXuat.Ten))
                .ForMember(d => d.KhoDuocPhamXuatId, o => o.MapFrom(s => s.KhoXuatId))
                .ForMember(d => d.KhoDuocPhamNhapId, o => o.MapFrom(s => s.KhoNhapId));
            CreateMap<XuatKhoDuocPhamViewModel, XuatKhoDuocPham>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.KhoXuatId, o => o.MapFrom(s => s.KhoDuocPhamXuatId))
                .ForMember(d => d.KhoNhapId, o => o.MapFrom(s => s.KhoDuocPhamNhapId));

            CreateMap<XuatKhoDuocPhamChiTiet, DuocPhamXuatChiTiet>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.TenDuocPham, o => o.MapFrom(s => s.DuocPhamBenhVien.DuocPham.Ten))
                .ForMember(d => d.TenNhom, o => o.MapFrom(s => s.XuatKhoDuocPhamChiTietViTris.First().NhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhom != null ? s.XuatKhoDuocPhamChiTietViTris.First().NhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhom.Ten : ""))
                .ForMember(d => d.DVT, o => o.MapFrom(s => s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten))
                .ForMember(d => d.Loai, o => o.MapFrom(s => s.XuatKhoDuocPhamChiTietViTris.First().NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT ? "BHYT" : "Không BHYT"))
                .ForMember(d => d.SoLuongXuat, o => o.MapFrom(s => s.XuatKhoDuocPhamChiTietViTris.Sum(p => p.SoLuongXuat)))
                ;

            CreateMap<DuocPhamXuatChiTiet, XuatKhoDuocPhamChiTiet>()
                .ForMember(d => d.Id, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    long.TryParse(s.Id.Split(",").First(), out long duocPhamId);
                    d.DuocPhamBenhVienId = duocPhamId;
                })
                ;

            CreateMap<XuatKhoDuocPhamChiTiet, XuatKhoDuocPhamChiTietViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.TongSoLuongXuat, o => o.MapFrom(s => s.XuatKhoDuocPhamChiTietViTris.Select(p => p.SoLuongXuat).Sum()))
                .ForMember(d => d.DuocPhamId, o => o.MapFrom(s => s.DuocPhamBenhVienId))
                .ForMember(d => d.TenDuocPham, o => o.MapFrom(s => s.DuocPhamBenhVien.DuocPham.Ten))
                //TODO update entity kho on 9/9/2020
                //.ForMember(d => d.ChatLuong, o => o.MapFrom(s => s.DatChatLuong ? "Đạt" : "Không đạt"))
                .ForMember(d => d.VAT, o => o.MapFrom(s => s.XuatKhoDuocPham.NhapKhoDuocPhams
                    .FirstOrDefault(p => p.XuatKhoDuocPhamId == s.XuatKhoDuocPhamId).NhapKhoDuocPhamChiTiets
                    .FirstOrDefault(p => p.DuocPhamBenhVienId == s.DuocPhamBenhVienId).VAT));
            //TODO update entity kho on 9/9/2020
            //.ForMember(d => d.DonGiaBan, o => o.MapFrom(s => s.XuatKhoDuocPham.NhapKhoDuocPhams
            //    .FirstOrDefault(p => p.XuatKhoDuocPhamId == s.XuatKhoDuocPhamId).NhapKhoDuocPhamChiTiets
            //    .FirstOrDefault(p => p.DuocPhamBenhVienId == s.DuocPhamBenhVienId).DonGiaBan))
            //.ForMember(d => d.ChietKhau, o => o.MapFrom(s => s.XuatKhoDuocPham.NhapKhoDuocPhams
            //    .FirstOrDefault(p => p.XuatKhoDuocPhamId == s.XuatKhoDuocPhamId).NhapKhoDuocPhamChiTiets
            //    .FirstOrDefault(p => p.DuocPhamBenhVienId == s.DuocPhamBenhVienId).ChietKhau));
            CreateMap<XuatKhoDuocPhamChiTietViewModel, XuatKhoDuocPhamChiTiet>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.DuocPhamBenhVienId, o => o.MapFrom(s => s.DuocPhamId));

            CreateMap<XuatKhoDuocPhamChiTietViTri, XuatKhoDuocPhamChiTietViTriViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.SoLo, o => o.MapFrom(s => s.NhapKhoDuocPhamChiTiet.Solo))
                .ForMember(d => d.HanSuDungDisplay, o => o.MapFrom(s => s.NhapKhoDuocPhamChiTiet.HanSuDung.ApplyFormatDate()))
                .ForMember(d => d.SoLuongXuat, o => o.MapFrom(s => s.SoLuongXuat))
                .ForMember(d => d.ViTri, o => o.MapFrom(s => s.NhapKhoDuocPhamChiTiet.KhoDuocPhamViTri.Ten));
            CreateMap<XuatKhoDuocPhamChiTietViTriViewModel, XuatKhoDuocPhamChiTietViTri>()
                .IgnoreAllNonExisting();

            #region export excel

            CreateMap<XuatKhoDuocPhamGridVo, XuatKhoDuocPhanExportExcel>();
            CreateMap<XuatKhoDuocPhamChildrenGridVo, XuatKhoDuocPhanExportExcelChild>();

            #endregion export excel


            #region XuatKhac
            CreateMap<YeuCauXuatKhoDuocPham, XuatKhoDuocPhamKhacViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauXuatKhoDuocPhamChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoXuat = s.KhoXuat?.Ten;
                    d.TenNguoiXuat = s.NguoiXuat?.User.HoTen;
                    d.TenNguoiNhan = s.NguoiNhan?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                    d.TenNhaThau = s.NhaThau?.Ten;
                });
            CreateMap<XuatKhoDuocPhamKhacViewModel, YeuCauXuatKhoDuocPham>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauXuatKhoDuocPhamChiTiets, o => o.Ignore())
                ;

            CreateMap<XuatKhoDuocPham, XuatKhoDuocPhamKhacViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauXuatKhoDuocPhamChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoXuat = s.KhoDuocPhamXuat?.Ten;
                    d.TenNguoiXuat = s.NguoiXuat?.User.HoTen;
                    d.TenNguoiNhan = s.NguoiNhan?.User.HoTen;
                    d.TenNhaThau = s.NhaThau?.Ten;
                    //d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                });
            CreateMap<XuatKhoDuocPhamKhacViewModel, XuatKhoDuocPham>().IgnoreAllNonExisting();

            CreateMap<XuatKhoDuocPhamKhacGridVo, XuatKhoDuocPhamKhacExportExcel>();
            CreateMap<YeuCauXuatKhoDuocPhamGridVo, XuatKhoDuocPhamKhacExportExcelChild>();
            #endregion
        }
    }
}