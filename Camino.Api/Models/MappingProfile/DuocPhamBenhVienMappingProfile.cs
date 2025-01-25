using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DuocPhamBenhVien;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class DuocPhamBenhVienMappingProfile : Profile
    {
        public DuocPhamBenhVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien, DuocPhamBenhVienViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                    {
                        d.MaDuocPhamBenhVien = s.MaDuocPhamBenhVien;
                        d.Ten = s.DuocPham.Ten;
                        d.TenTiengAnh = s.DuocPham.TenTiengAnh;
                        d.SoDangKy = s.DuocPham.SoDangKy;
                        d.STTHoatChat = s.DuocPham.STTHoatChat;
                        d.MaHoatChat = s.DuocPham.MaHoatChat;
                        d.HoatChat = s.DuocPham.HoatChat;
                        d.TenLoaiThuocHoacHoatChat = s.DuocPham.LoaiThuocHoacHoatChat.GetDescription();
                        d.NhaSanXuat = s.DuocPham.NhaSanXuat;
                        d.NuocSanXuat = s.DuocPham.NuocSanXuat;
                        d.DuongDungId = s.DuocPham.DuongDungId;
                        d.TenDuongDung = s.DuocPham.DuongDung?.Ten;
                        d.DonViTinhId = s.DuocPham.DonViTinhId;
                        d.TenDonViTinh = s.DuocPham.DonViTinh?.Ten;
                        d.HamLuong = s.DuocPham.HamLuong;
                        d.QuyCach = s.DuocPham.QuyCach;
                        d.HeSoDinhMucDonViTinh = s.DuocPham.HeSoDinhMucDonViTinh;
                        d.TieuChuan = s.DuocPham.TieuChuan;
                        d.DangBaoChe = s.DuocPham.DangBaoChe;
                        d.TheTich = s.DuocPham.TheTich;
                        d.HuongDan = s.DuocPham.HuongDan;
                        d.MoTa = s.DuocPham.MoTa;
                        d.ChiDinh = s.DuocPham.ChiDinh;
                        d.ChongChiDinh = s.DuocPham.ChongChiDinh;
                        d.LieuLuongCachDung = s.DuocPham.LieuLuongCachDung;
                        d.TacDungPhu = s.DuocPham.TacDungPhu;
                        d.ChuYDePhong = s.DuocPham.ChuYDePhong;
                        d.LaThucPhamChucNang = s.DuocPham.LaThucPhamChucNang;
                        d.LaThuocHuongThanGayNghien = s.DuocPham.LaThuocHuongThanGayNghien;
                        d.TenDuocPhamBenhVienPhanNhomCha = s.DuocPhamBenhVienPhanNhom?.Ten;
                        d.TenDuocPhamBenhVienPhanNhomCon = s.DuocPhamBenhVienPhanNhom?.Ten;
                        d.HieuLuc = s.HieuLuc;
                        d.DieuKienBaoHiemThanhToan = s.DieuKienBaoHiemThanhToan;
                        d.TenPhanLoaiThuocTheoQuanLy = s.LoaiThuocTheoQuanLy.GetDescription();
                        d.MaATC = s.DuocPham.MaATC;
                        d.DuocPhamCoDau = s.DuocPham.DuocPhamCoDau;
                    });

            CreateMap<DuocPhamBenhVienViewModel, Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>().IgnoreAllNonExisting()
                .ForMember(d => d.DuocPham, o => o.Ignore())
                ; 



            CreateMap<DuocPhamBenhVienGridVo, ExportDuocPhamBenhVienExportExcel>()
              .IgnoreAllNonExisting();

        }


    }

}
