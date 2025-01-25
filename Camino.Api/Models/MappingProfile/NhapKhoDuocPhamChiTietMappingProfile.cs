using AutoMapper;
using Camino.Api.Models.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class NhapKhoDuocPhamChiTietMappingProfile : Profile
    {
        public NhapKhoDuocPhamChiTietMappingProfile()
        {
            CreateMap<NhapKhoDuocPhamChiTiet, NhapKhoDuocPhamChiTietViewModel>()
                 .ForMember(d => d.DuocPhamBenhViens, o => o.MapFrom(s => s.DuocPhamBenhViens))
                 .ForMember(d => d.HopDongThauDuocPhams, o => o.MapFrom(s => s.HopDongThauDuocPhams))
                 .ForMember(d => d.KhoDuocPhamViTri, o => o.MapFrom(s => s.KhoDuocPhamViTri))
                    .AfterMap((source, viewmodel) =>
                    {
                        //viewmodel.TenDuocPham = source?.DuocPhamBenhViens?.DuocPham?.Ten +
                        //                        "-" + source?.DuocPhamBenhViens?.DuocPham?.HoatChat +
                        //                        "-" + source?.DuocPhamBenhViens?.DuocPham?.NhaSanXuat;

                        //viewmodel.TenHopDongThau = source?.HopDongThauDuocPhams?.SoHopDong +
                        //                        "-" + source?.HopDongThauDuocPhams?.NhaThau?.Ten +
                        //                        "-" + source?.HopDongThauDuocPhams?.NhaThau?.DiaChi;
                        viewmodel.TenDuocPham = source?.DuocPhamBenhViens?.DuocPham?.Ten ?? "";
                        viewmodel.TenHopDongThau = source?.HopDongThauDuocPhams?.SoHopDong ?? "";
                        viewmodel.Solo = source?.Solo ?? "";
                        viewmodel.ViTri = source?.KhoDuocPhamViTri?.Ten ?? "";

                        //TODO update entity kho on 9/9/2020
                        //viewmodel.TenDatChatLuong = source?.DatChatLuong == true ? "Đạt" : "Không đạt";
                        viewmodel.TextHanSuDung = source?.HanSuDung.ApplyFormatDate();
                    });
            CreateMap<NhapKhoDuocPhamChiTietViewModel, NhapKhoDuocPhamChiTiet>()
                .ForMember(d => d.KhoViTriId, o => o.MapFrom(s => s.KhoDuocPhamViTriId))
                 .ForMember(d => d.KhoDuocPhamViTri, o => o.Ignore())
                 .ForMember(x => x.DuocPhamBenhViens, o => o.Ignore())
                 .ForMember(x => x.HopDongThauDuocPhams, o => o.Ignore());
        }
    }
}
