using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.CauHinh;
using static Camino.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore.Internal;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        public YeuCauTiepNhan GetYeuCauTiepNhanForGhiNhanVatTuThuoc(long yeuCauTiepNhanId)
        {
            //return _yeuCauTiepNhanRepository.Table.Where(p => p.Id == yeuCauTiepNhanId)
            //                                      .Include(p => p.YeuCauDuocPhamBenhViens)
            //                                          .ThenInclude(p => p.YeuCauLinhDuocPhamChiTiets)
            //                                          .ThenInclude(p => p.YeuCauLinhDuocPham)
            //                                      .Include(p => p.YeuCauDuocPhamBenhViens)
            //                                          .ThenInclude(p => p.XuatKhoDuocPhamChiTiet)
            //                                          .ThenInclude(p => p.XuatKhoDuocPhamChiTietViTris)
            //                                          .ThenInclude(p => p.NhapKhoDuocPhamChiTiet)
            //                                      .Include(p => p.YeuCauVatTuBenhViens)
            //                                          .ThenInclude(p => p.YeuCauLinhVatTuChiTiets)
            //                                          .ThenInclude(p => p.YeuCauLinhVatTu)
            //                                      .Include(p => p.YeuCauVatTuBenhViens)
            //                                          .ThenInclude(p => p.XuatKhoVatTuChiTiet)
            //                                          .ThenInclude(p => p.XuatKhoVatTuChiTietViTris)
            //                                          .ThenInclude(p => p.NhapKhoVatTuChiTiet)
            //                                      .Include(p => p.YeuCauKhamBenhs)
            //                                      .Include(p => p.NoiTruBenhAn)
            //                                      .FirstOrDefault();

            var yctn = _yeuCauTiepNhanRepository.Table.Where(p => p.Id == yeuCauTiepNhanId)                                                  
                                                  .Include(p => p.YeuCauKhamBenhs)
                                                  .Include(p => p.NoiTruBenhAn)
                                                  .FirstOrDefault();
            //Explicit loading
            var yeuCauDuocPhamBenhViens = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauDuocPhamBenhViens);
            yeuCauDuocPhamBenhViens.Query().Include(p => p.YeuCauLinhDuocPhamChiTiets).ThenInclude(p => p.YeuCauLinhDuocPham).Load();
            yeuCauDuocPhamBenhViens.Query().Include(p => p.XuatKhoDuocPhamChiTiet).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTris).ThenInclude(p => p.NhapKhoDuocPhamChiTiet).Load();

            var yeuCauVatTuBenhViens = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauVatTuBenhViens);
            yeuCauVatTuBenhViens.Query().Include(p => p.YeuCauLinhVatTuChiTiets).ThenInclude(p => p.YeuCauLinhVatTu).Load();
            yeuCauVatTuBenhViens.Query().Include(p => p.XuatKhoVatTuChiTiet).ThenInclude(p => p.XuatKhoVatTuChiTietViTris).ThenInclude(p => p.NhapKhoVatTuChiTiet).Load();

            return yctn;
        }

        public async Task<List<GhiNhanVatTuTieuHaoThuocGridVo>> GetGridDataGhiNhanVTTHThuocAsync(long yeuCauDichVuKyThuatId)
        {
            var lstGhiNhanVTHHThuoc = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)EnumNhomGoiDichVu.DuocPham,
                    TenNhomYeuCau = EnumNhomGoiDichVu.DuocPham.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuatId,
                    NhomChiDinhId = (int)EnumNhomGoiDichVu.DichVuKyThuat,

                    //MaDichVuYeuCau = "",
                    MaDichVuYeuCau = item.DuocPhamBenhVien.DuocPham.SoDangKy,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,

                    //DonViTinh = item.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    DonViTinh = item.DonViTinh.Ten,
                    TenDuongDung = item.DuongDung.Ten,
                    GiaiDoanPhauThuat = item.GiaiDoanPhauThuat,
                    GiaiDoanPhauThuatDisplay = item.GiaiDoanPhauThuat.GetDescription(),

                    DonGia = item.DonGiaBan,//item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    SoLuongDisplay = Camino.Core.Helpers.ConvertFloatToStringFractionHelper.FloatToStringFraction(item.SoLuong),
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaDuocPhamBHYT,

                    PhieuLinh = item.YeuCauLinhDuocPham != null ?
                        item.YeuCauLinhDuocPham.SoPhieu : (item.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false) ? item.YeuCauLinhDuocPhamChiTiets.Where(a => a.YeuCauLinhDuocPham.DuocDuyet != false).Select(a => a.YeuCauLinhDuocPham.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null ? item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi != true,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    //Ma = item.Ma,
                    Ten = item.Ten,
                    //DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.DuocPhamBenhVienId,
                    TinhTrang = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu,
                    IsKhoTong = item.KhoLinh != null && (item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2),
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any(),
                })
                .Union(
                    _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                    .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                    {
                        YeuCauId = item.Id,
                        NhomYeuCauId = (int)EnumNhomGoiDichVu.VatTuTieuHao,
                        TenNhomYeuCau = EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                        TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                        DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                        MaDichVuYeuCau = item.Ma,
                        TenDichVuYeuCau = item.Ten,

                        KhoId = item.KhoLinhId ?? 0,
                        TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                        LoaiKho = item.KhoLinh.LoaiKho,

                        DonViTinh = item.DonViTinh,
                        TenDuongDung = "", //item.DuongDung,
                        GiaiDoanPhauThuat = item.GiaiDoanPhauThuat,
                        GiaiDoanPhauThuatDisplay = item.GiaiDoanPhauThuat.GetDescription(),

                        DonGia = item.DonGiaBan,//item.DonGiaNhap,


                        SoLuong = item.SoLuong,
                        SoLuongDisplay = Camino.Core.Helpers.ConvertFloatToStringFractionHelper.FloatToStringFraction(item.SoLuong),

                        ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                        DonGiaBaoHiem = item.DonGiaBaoHiem,
                        DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                        LaBHYT = item.LaVatTuBHYT,

                        PhieuLinh = item.YeuCauLinhVatTu != null ?
                            item.YeuCauLinhVatTu.SoPhieu : (item.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false) ? item.YeuCauLinhVatTuChiTiets.Where(a => a.YeuCauLinhVatTu.DuocDuyet != false).Select(a => a.YeuCauLinhVatTu.SoPhieu).Join(", ") : ""),
                        PhieuXuat = item.XuatKhoVatTuChiTiet.XuatKhoVatTu != null ? item.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : "",

                        TinhPhi = item.KhongTinhPhi != true,

                        CreatedOn = item.CreatedOn,
                        ThoiGianChiDinh = item.ThoiDiemChiDinh,
                        TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                        Ma = item.Ma,
                        Ten = item.Ten,
                        DVT = item.DonViTinh,
                        NhaSX = item.NhaSanXuat,
                        NuocSX = item.NuocSanXuat,

                        VatTuThuocBenhVienId = item.VatTuBenhVienId,
                        TinhTrang = item.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null,
                        IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu,
                        IsKhoTong = item.KhoLinh != null && (item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2),
                        IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraVatTuTuBenhNhanChiTiets.Any()
                    }))
                //.OrderBy(p => p.TenDichVu)
                //.OrderBy(p => p.GiaiDoanPhauThuat)
                .OrderBy(p => p.CreatedOn).ThenBy(p => p.TenDichVu)
                .ToListAsync();

            return lstGhiNhanVTHHThuoc;
        }
        public async Task<List<GhiNhanVatTuTieuHaoThuocGroupParentGridVo>> GetGridDataGhiNhanVTTHThuocAsyncVer2(long yeuCauDichVuKyThuatId)
        {
            var lstGhiNhanVTHHThuoc = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)EnumNhomGoiDichVu.DuocPham,
                    TenNhomYeuCau = EnumNhomGoiDichVu.DuocPham.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuatId,
                    NhomChiDinhId = (int)EnumNhomGoiDichVu.DichVuKyThuat,

                    //MaDichVuYeuCau = "",
                    MaDichVuYeuCau = item.DuocPhamBenhVien.DuocPham.SoDangKy,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,

                    //DonViTinh = item.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    DonViTinh = item.DonViTinh.Ten,
                    TenDuongDung = item.DuongDung.Ten,
                    GiaiDoanPhauThuat = item.GiaiDoanPhauThuat,
                    GiaiDoanPhauThuatDisplay = item.GiaiDoanPhauThuat.GetDescription(),

                    DonGia = item.DonGiaBan,//item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    #region Cập nhật 28/12/2022
                    //SoLuongDisplay = Camino.Core.Helpers.ConvertFloatToStringFractionHelper.FloatToStringFraction(item.SoLuong),
                    #endregion
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaDuocPhamBHYT,

                    //PhieuLinh = item.YeuCauLinhDuocPham != null ?
                    //    item.YeuCauLinhDuocPham.SoPhieu : (item.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false) ? item.YeuCauLinhDuocPhamChiTiets.Where(a => a.YeuCauLinhDuocPham.DuocDuyet != false).Select(a => a.YeuCauLinhDuocPham.SoPhieu).Join(", ") : ""),
                    PhieuLinh = item.YeuCauLinhDuocPham.SoPhieu,

                    PhieuXuat = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null ? item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    //Ma = item.Ma,
                    Ten = item.Ten,
                    //DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.DuocPhamBenhVienId,
                    TinhTrang = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu,
                    IsKhoTong = item.KhoLinh != null && (item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2),
                    //IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any(),
                })
                .Union(
                    _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                    .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                    {
                        YeuCauId = item.Id,
                        NhomYeuCauId = (int)EnumNhomGoiDichVu.VatTuTieuHao,
                        TenNhomYeuCau = EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                        TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                        DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                        MaDichVuYeuCau = item.Ma,
                        TenDichVuYeuCau = item.Ten,

                        KhoId = item.KhoLinhId ?? 0,
                        TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                        LoaiKho = item.KhoLinh.LoaiKho,

                        DonViTinh = item.DonViTinh,
                        TenDuongDung = "", //item.DuongDung,
                        GiaiDoanPhauThuat = item.GiaiDoanPhauThuat,
                        GiaiDoanPhauThuatDisplay = item.GiaiDoanPhauThuat.GetDescription(),

                        DonGia = item.DonGiaBan,//item.DonGiaNhap,


                        SoLuong = item.SoLuong,
                        #region Cập nhật 28/12/2022
                        //SoLuongDisplay = Camino.Core.Helpers.ConvertFloatToStringFractionHelper.FloatToStringFraction(item.SoLuong),
                        #endregion

                        ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                        DonGiaBaoHiem = item.DonGiaBaoHiem,
                        DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                        LaBHYT = item.LaVatTuBHYT,

                        //PhieuLinh = item.YeuCauLinhVatTu != null ?
                        //    item.YeuCauLinhVatTu.SoPhieu : (item.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false) ? item.YeuCauLinhVatTuChiTiets.Where(a => a.YeuCauLinhVatTu.DuocDuyet != false).Select(a => a.YeuCauLinhVatTu.SoPhieu).Join(", ") : ""),
                        PhieuLinh = item.YeuCauLinhVatTu.SoPhieu,

                        PhieuXuat = item.XuatKhoVatTuChiTiet.XuatKhoVatTu != null ? item.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : "",

                        TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                        CreatedOn = item.CreatedOn,
                        ThoiGianChiDinh = item.ThoiDiemChiDinh,
                        TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                        Ma = item.Ma,
                        Ten = item.Ten,
                        DVT = item.DonViTinh,
                        NhaSX = item.NhaSanXuat,
                        NuocSX = item.NuocSanXuat,

                        VatTuThuocBenhVienId = item.VatTuBenhVienId,
                        TinhTrang = item.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null,
                        IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu,
                        IsKhoTong = item.KhoLinh != null && (item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2),
                        //IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraVatTuTuBenhNhanChiTiets.Any()
                    }))
                //.OrderBy(p => p.TenDichVu)
                //.OrderBy(p => p.GiaiDoanPhauThuat)
                .OrderBy(p => p.CreatedOn).ThenBy(p => p.TenDichVu)
                .ToList();

            #region Cập nhật 28/12/2022
            if (lstGhiNhanVTHHThuoc.Any())
            {
                lstGhiNhanVTHHThuoc.ForEach(x => x.SoLuongDisplay = ConvertFloatToStringFractionHelper.FloatToStringFraction(x.SoLuong));
            }
            #endregion

            #region Xử lý get phiếu lĩnh, có yêu cầu trả DP/VT

            #region Kiểm tra phiếu lĩnh
            var lstYeuCauChuaCoPhieuLinh = lstGhiNhanVTHHThuoc.Where(x => string.IsNullOrEmpty(x.PhieuLinh)).ToList();
            var lstYeuCauDuocPhamChuaCoPhieuLinh = lstYeuCauChuaCoPhieuLinh
                .Where(x => x.NhomYeuCauId == (int) EnumNhomGoiDichVu.DuocPham).Select(x => x.YeuCauId).Distinct().ToList();
            var lstYeuCauVatTuChuaCoPhieuLinh = lstYeuCauChuaCoPhieuLinh
                .Where(x => x.NhomYeuCauId == (int)EnumNhomGoiDichVu.VatTuTieuHao).Select(x => x.YeuCauId).Distinct().ToList();

            var lstYeuCauLinhDuocPham = new List<LookupItemVo>();
            var lstYeuCauLinhVatTu = new List<LookupItemVo>();

            if (lstYeuCauDuocPhamChuaCoPhieuLinh.Any())
            {
                lstYeuCauLinhDuocPham = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauDuocPhamBenhVienId != null 
                                && lstYeuCauDuocPhamChuaCoPhieuLinh.Contains(x.YeuCauDuocPhamBenhVienId.Value) 
                                && x.YeuCauLinhDuocPham.DuocDuyet != false)
                    .Select(x => new LookupItemVo()
                    {
                        KeyId = x.YeuCauDuocPhamBenhVienId.Value,
                        DisplayName = x.YeuCauLinhDuocPham.SoPhieu
                    })
                    .Distinct()
                    .ToList();
            }
            if (lstYeuCauVatTuChuaCoPhieuLinh.Any())
            {
                lstYeuCauLinhVatTu = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauVatTuBenhVienId != null
                                && lstYeuCauDuocPhamChuaCoPhieuLinh.Contains(x.YeuCauVatTuBenhVienId.Value)
                                && x.YeuCauLinhVatTu.DuocDuyet != false)
                    .Select(x => new LookupItemVo()
                    {
                        KeyId = x.YeuCauVatTuBenhVienId.Value,
                        DisplayName = x.YeuCauLinhVatTu.SoPhieu
                    })
                    .Distinct()
                    .ToList();
            }
            #endregion

            #region Kiểm tra yêu cầu trả
            var lstYeuCauDuocPhamId = lstGhiNhanVTHHThuoc.Where(x => x.NhomYeuCauId == (int)EnumNhomGoiDichVu.DuocPham).Select(x => x.YeuCauId).Distinct().ToList();
            var lstYeuCauVatTuId = lstGhiNhanVTHHThuoc.Where(x => x.NhomYeuCauId == (int)EnumNhomGoiDichVu.VatTuTieuHao).Select(x => x.YeuCauId).Distinct().ToList();
            var lstYeuCauDuocPhamCoYeuCauTra = new List<long>();
            var lstYeuCauVatTuCoYeuCauTra = new List<long>();

            if (lstYeuCauDuocPhamId.Any())
            {
                lstYeuCauDuocPhamCoYeuCauTra = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                    .Where(x => lstYeuCauDuocPhamId.Contains(x.YeuCauDuocPhamBenhVienId))
                    .Select(x => x.YeuCauDuocPhamBenhVienId)
                    .Distinct()
                    .ToList();
            }
            if (lstYeuCauVatTuId.Any())
            {
                lstYeuCauVatTuCoYeuCauTra = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                    .Where(x => lstYeuCauVatTuId.Contains(x.YeuCauVatTuBenhVienId))
                    .Select(x => x.YeuCauVatTuBenhVienId)
                    .Distinct()
                    .ToList();
            }
            #endregion


            if (lstYeuCauLinhDuocPham.Any() || lstYeuCauLinhVatTu.Any())
            {
                foreach (var ghiNhanItem in lstGhiNhanVTHHThuoc)
                {
                    if (string.IsNullOrEmpty(ghiNhanItem.PhieuLinh))
                    {
                        var lstPhieuLinh = new List<LookupItemVo>();
                        if (ghiNhanItem.NhomYeuCauId == (int) EnumNhomGoiDichVu.DuocPham)
                        {
                            lstPhieuLinh = lstYeuCauLinhDuocPham.Where(x => x.KeyId == ghiNhanItem.YeuCauId).ToList();
                        }
                        else if (ghiNhanItem.NhomYeuCauId == (int) EnumNhomGoiDichVu.VatTuTieuHao)
                        {
                            lstPhieuLinh = lstYeuCauLinhVatTu.Where(x => x.KeyId == ghiNhanItem.YeuCauId).ToList();
                        }

                        if (lstPhieuLinh.Any())
                        {
                            ghiNhanItem.PhieuLinh = string.Join(", ", lstPhieuLinh.Select(x => x.DisplayName).Distinct().ToList());
                        }
                    }

                    if (ghiNhanItem.NhomYeuCauId == (int) EnumNhomGoiDichVu.DuocPham)
                    {
                        ghiNhanItem.IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = lstYeuCauDuocPhamCoYeuCauTra.Contains(ghiNhanItem.YeuCauId);
                    }
                    else if(ghiNhanItem.NhomYeuCauId == (int)EnumNhomGoiDichVu.VatTuTieuHao)
                    {
                        ghiNhanItem.IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = lstYeuCauVatTuCoYeuCauTra.Contains(ghiNhanItem.YeuCauId);
                    }
                }
            }


            #endregion

            var result = lstGhiNhanVTHHThuoc
                .GroupBy(x => new { x.NhomYeuCauId, x.MaDichVuYeuCau, x.TenDichVuYeuCau, x.TenKho, x.YeuCauDichVuChiDinhId, x.NhomChiDinhId, x.TinhPhi, x.CreatedOn })
                .Select(item => new GhiNhanVatTuTieuHaoThuocGroupParentGridVo()
                {
                    NhomYeuCauId = (int)item.First().NhomYeuCauId,
                    TenDichVu = item.First().TenDichVu,
                    DichVuId = item.First().DichVuId,

                    YeuCauDichVuChiDinhId = item.First().YeuCauDichVuChiDinhId,
                    NhomChiDinhId = item.First().NhomChiDinhId,

                    MaDichVuYeuCau = item.First().MaDichVuYeuCau,
                    TenDichVuYeuCau = item.First().TenDichVuYeuCau,

                    KhoId = item.First().KhoId,
                    TenKho = item.First().TenKho,
                    LoaiKho = item.First().LoaiKho,

                    DonViTinh = item.First().DonViTinh,
                    TenDuongDung = item.First().TenDuongDung,
                    SoLuong = item.Sum(a => a.SoLuong),

                    ThanhTien = item.Sum(a => a.ThanhTien ?? 0),

                    //DonGiaBaoHiem = item.Select(a => a.DonGiaBaoHiem).Distinct().Count() > 1 ? null : item.Select(a => a.DonGiaBaoHiem).First(),
                    DuocHuongBaoHiem = item.First().DuocHuongBaoHiem,
                    LaBHYT = item.First().LaBHYT,

                    PhieuLinh = item.First().PhieuLinh,
                    PhieuXuat = item.First().PhieuXuat,

                    GiaiDoanPhauThuat = item.First().GiaiDoanPhauThuat,

                    TinhPhi = item.First().TinhPhi,
                    CreatedOn = item.First().CreatedOn,
                    ThoiGianChiDinh = item.First().ThoiGianChiDinh,
                    TenNhanVienChiDinh = item.First().TenNhanVienChiDinh,

                    // cập nhật 24/05/2021
                    Ma = item.First().Ma,
                    Ten = item.First().Ten,
                    DVT = item.First().DonViTinh,
                    NhaSX = item.First().NhaSX,
                    NuocSX = item.First().NuocSX,

                    VatTuThuocBenhVienId = item.First().VatTuThuocBenhVienId,
                    TinhTrang = item.First().TinhTrang,
                    IsKhoLe = item.First().IsKhoLe,
                    IsKhoTong = item.First().IsKhoTong,
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.Any(p => p.IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet),

                    YeuCauGhiNhanVTTHThuocs = item.ToList(),
                    ThongTinGias = item.GroupBy(a => new { a.DonGia, a.DonGiaBaoHiem }).Select(a => new GhiNhanVatTuTieuHaoThuocGroupGiaGridVo
                    {
                        IsTinhPhi = a.First().TinhPhi,
                        DonGia = a.First().DonGia,
                        SoLuong = a.Sum(b => b.SoLuong),
                        DonGiaBaoHiem = a.First().DonGiaBaoHiem
                    }).ToList()
                })
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToList();
            return result;
        }

        public async Task XuLyThemGhiNhanVatTuBenhVienAsync(ChiDinhGhiNhanVatTuThuocPTTTVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet)
        {
            //var thongTinDichVuChiDinh = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(yeuCauVo.DichVuChiDinhId);
            var thongTinDichVuGhiNhan = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(yeuCauVo.DichVuGhiNhanId);
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var kho = await _khoRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauVo.KhoId);

            if (kho == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            // trường hợp ghi nhận dược phẩm
            if (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.DuocPham) // xử lý ghi nhận dược phẩm
            {
                var yeuCauDuocPham = new YeuCauDuocPhamBenhVien()
                {
                    YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                    KhoLinhId = yeuCauVo.KhoId,
                };

                // bỏ bớt include bị dư, và await
                var yeuCauDichVuKyThuat = BaseRepository.TableNoTracking
                    //.Include(p => p.YeuCauTiepNhan)
                    .First(p => p.Id == yeuCauVo.DichVuChiDinhId);
                yeuCauDuocPham.NoiTruPhieuDieuTriId = yeuCauDichVuKyThuat.NoiTruPhieuDieuTriId;

                //var yeuCauKhamBenh = await _yeuCauKhamBenhRepository.TableNoTracking
                //    .Include(x => x.YeuCauTiepNhan)
                //    .FirstAsync(x => x.Id == yeuCauVo.YeuCauKhamBenhId);

                //if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKhamBenh)
                //{
                //    yeuCauDuocPham.YeuCauKhamBenhId = thongTinDichVuChiDinh.Id;
                //}
                //else if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKyThuat)
                //{
                //    yeuCauDuocPham.YeuCauDichVuKyThuatId = thongTinDichVuChiDinh.Id;
                //}

                yeuCauDuocPham.YeuCauDichVuKyThuatId = yeuCauVo.DichVuChiDinhId;

                // bỏ bớt include bị dư, và await
                var duocPhamBenhVien = _duocPhamBenhVienService.TableNoTracking.Include(x => x.DuocPham).ThenInclude(y => y.HopDongThauDuocPhamChiTiets)
                                                                                     .FirstOrDefault(x => x.Id == thongTinDichVuGhiNhan.Id);

                if (duocPhamBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.DichVuChiDinhId.NotExists"));
                }

                if (kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                {
                    yeuCauDuocPham.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan;
                }
                else
                {
                    yeuCauDuocPham.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu;
                }

                yeuCauDuocPham.DuocPhamBenhVienId = duocPhamBenhVien.Id;
                yeuCauDuocPham.Ten = duocPhamBenhVien.DuocPham.Ten;
                yeuCauDuocPham.TenTiengAnh = duocPhamBenhVien.DuocPham.TenTiengAnh;
                yeuCauDuocPham.SoDangKy = duocPhamBenhVien.DuocPham.SoDangKy;
                yeuCauDuocPham.STTHoatChat = duocPhamBenhVien.DuocPham.STTHoatChat;
                yeuCauDuocPham.MaHoatChat = duocPhamBenhVien.DuocPham.MaHoatChat;
                yeuCauDuocPham.HoatChat = duocPhamBenhVien.DuocPham.HoatChat;
                yeuCauDuocPham.LoaiThuocHoacHoatChat = duocPhamBenhVien.DuocPham.LoaiThuocHoacHoatChat;
                yeuCauDuocPham.NhaSanXuat = duocPhamBenhVien.DuocPham.NhaSanXuat;
                yeuCauDuocPham.NuocSanXuat = duocPhamBenhVien.DuocPham.NuocSanXuat;
                yeuCauDuocPham.DuongDungId = duocPhamBenhVien.DuocPham.DuongDungId;
                yeuCauDuocPham.HamLuong = duocPhamBenhVien.DuocPham.HamLuong;
                yeuCauDuocPham.QuyCach = duocPhamBenhVien.DuocPham.QuyCach;
                yeuCauDuocPham.TieuChuan = duocPhamBenhVien.DuocPham.TieuChuan;
                yeuCauDuocPham.DangBaoChe = duocPhamBenhVien.DuocPham.DangBaoChe;
                yeuCauDuocPham.DonViTinhId = duocPhamBenhVien.DuocPham.DonViTinhId;
                yeuCauDuocPham.HuongDan = duocPhamBenhVien.DuocPham.HuongDan;
                yeuCauDuocPham.MoTa = duocPhamBenhVien.DuocPham.MoTa;
                yeuCauDuocPham.ChiDinh = duocPhamBenhVien.DuocPham.ChiDinh;
                yeuCauDuocPham.ChongChiDinh = duocPhamBenhVien.DuocPham.ChongChiDinh;
                yeuCauDuocPham.LieuLuongCachDung = duocPhamBenhVien.DuocPham.LieuLuongCachDung;
                yeuCauDuocPham.TacDungPhu = duocPhamBenhVien.DuocPham.TacDungPhu;
                yeuCauDuocPham.ChuYdePhong = duocPhamBenhVien.DuocPham.ChuYDePhong;

                yeuCauDuocPham.KhongTinhPhi = !yeuCauVo.TinhPhi;
                yeuCauDuocPham.LaDuocPhamBHYT = yeuCauVo.LaDuocPhamBHYT;

                yeuCauDuocPham.SoLuong = yeuCauVo.SoLuong.Value;
                yeuCauDuocPham.NhanVienChiDinhId = currentUserId;
                yeuCauDuocPham.NoiChiDinhId = phongHienTaiId;
                yeuCauDuocPham.ThoiDiemChiDinh = DateTime.Now;

                yeuCauDuocPham.DaCapThuoc = false;
                yeuCauDuocPham.TrangThai = EnumYeuCauDuocPhamBenhVien.ChuaThucHien;
                yeuCauDuocPham.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
                yeuCauDuocPham.GiaiDoanPhauThuat = yeuCauVo.GiaiDoanPhauThuat;

                // thông tin bảo hiểm
                //var giaBaoHiem = duocPhamBenhVien.DuocPhamBenhVienGiaBaoHiems.FirstOrDefault(x => x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));

                //if (giaBaoHiem != null)
                //{
                //    yeuCauDuocPham.DonGiaBaoHiem = giaBaoHiem.Gia;
                //    yeuCauDuocPham.TiLeBaoHiemThanhToan = giaBaoHiem.TiLeBaoHiemThanhToan;
                //}

                //yeuCauDuocPham.DuocHuongBaoHiem = yeuCauDichVuKyThuat.YeuCauTiepNhan.CoBHYT == true && (yeuCauDichVuKyThuat?.YeuCauKhamBenh?.DuocHuongBaoHiem ?? false) && giaBaoHiem != null && giaBaoHiem.Gia > 0;
                yeuCauDuocPham.DuocHuongBaoHiem = yeuCauVo.LaDuocPhamBHYT;

                var lstNhapChiTietTheoDuocPham = new List<NhapKhoDuocPhamChiTiet>();
                if (!yeuCauVo.NhapKhoDuocPhamChiTiets.Any())
                {
                    lstNhapChiTietTheoDuocPham = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauVo.KhoId &&
                                                                                                                    x.DuocPhamBenhVienId == duocPhamBenhVien.Id &&
                                                                                                                    x.NhapKhoDuocPhams.DaHet != true &&
                                                                                                                    x.LaDuocPhamBHYT == yeuCauVo.LaDuocPhamBHYT &&
                                                                                                                    x.SoLuongNhap > x.SoLuongDaXuat

                                                                                                                    //BVHD-3821
                                                                                                                    // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                                                                                                                    && x.HanSuDung.Date >= DateTime.Now.Date)
                                                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                                                        .ToListAsync();
                }
                else
                {
                    lstNhapChiTietTheoDuocPham = yeuCauVo.NhapKhoDuocPhamChiTiets.Where(p => p.DuocPhamBenhVienId == duocPhamBenhVien.Id &&
                                                                                             p.LaDuocPhamBHYT == yeuCauVo.LaDuocPhamBHYT &&
                                                                                             p.SoLuongNhap > p.SoLuongDaXuat

                                                                                             //BVHD-3821
                                                                                             // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                                                                                             && p.HanSuDung.Date >= DateTime.Now.Date)
                                                                                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                                    .ToList();
                }

                //if ((lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) || (lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauVo.SoLuong))
                //{
                //    throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                //}

                if (yeuCauDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                {
                    var soLuongTonTrongKho = Math.Round(lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat), 2);
                    if ((soLuongTonTrongKho < 0) || (soLuongTonTrongKho < yeuCauVo.SoLuong))
                    {
                        throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                    }

                    // xử lý thêm yêu cầu dược phẩm, với mỗi thông tin giá, VAT, tỉ lệ tháp giá khác nhau tạo 1 yêu cầu
                    // thêm xuất kho dược phẩm chi tiết
                    var yeuCauNew = yeuCauDuocPham.Clone();

                    var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                    {
                        DuocPhamBenhVienId = thongTinDichVuGhiNhan.Id
                    };

                    var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();
                    foreach (var item in lstNhapChiTietTheoDuocPham)
                    {
                        if (yeuCauVo.SoLuong > 0)
                        {
                            var giaTheoHopDong = duocPhamBenhVien.DuocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == item.HopDongThauDuocPhamId).Gia;
                            var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;

                            var tileBHYTThanhToanTheoNhap = item.LaDuocPhamBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;
                            if (yeuCauNew.DonGiaNhap != 0
                                && (yeuCauNew.DonGiaNhap != item.DonGiaNhap || yeuCauNew.VAT != item.VAT || yeuCauNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap))
                            {
                                yeuCauNew.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                                yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat), 2);
                                lstYeuCau.Add(yeuCauNew);

                                yeuCauNew = yeuCauDuocPham.Clone();
                                yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauNew.VAT = item.VAT;
                                yeuCauNew.PhuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKho;
                                yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap; //item.TiLeBHYTThanhToan ?? 100;

                                xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                                {
                                    DuocPhamBenhVienId = thongTinDichVuGhiNhan.Id
                                };
                            }
                            else
                            {
                                yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauNew.VAT = item.VAT;
                                yeuCauNew.PhuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKho;
                                yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap; //item.TiLeBHYTThanhToan ?? 100;
                            }

                            var xuatViTri = new XuatKhoDuocPhamChiTietViTri()
                            {
                                NhapKhoDuocPhamChiTietId = item.Id
                            };

                            var tonTheoItem = Math.Round(item.SoLuongNhap - item.SoLuongDaXuat, 2);
                            if (yeuCauVo.SoLuong < tonTheoItem || yeuCauVo.SoLuong.Value.AlmostEqual(tonTheoItem))
                            {
                                xuatViTri.SoLuongXuat = yeuCauVo.SoLuong.Value;
                                item.SoLuongDaXuat = Math.Round(item.SoLuongDaXuat + yeuCauVo.SoLuong.Value, 2);
                                yeuCauVo.SoLuong = 0;
                            }
                            else
                            {
                                xuatViTri.SoLuongXuat = tonTheoItem;
                                item.SoLuongDaXuat = item.SoLuongNhap;
                                yeuCauVo.SoLuong = Math.Round(yeuCauVo.SoLuong.Value - tonTheoItem, 2);
                            }

                            xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);

                            if (yeuCauVo.SoLuong.Value.AlmostEqual(0))
                            {
                                yeuCauNew.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                                yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat), 2);
                                lstYeuCau.Add(yeuCauNew);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    //await _yeuCauDuocPhamBenhVienRepository.AddRangeAsync(lstYeuCau);
                    //await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(lstNhapKhoDuocPhamChiTiet);

                    foreach (var item in lstYeuCau)
                    {
                        yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Add(item);
                    }

                    yeuCauVo.NhapKhoDuocPhamChiTiets = lstNhapChiTietTheoDuocPham;
                }

                if (yeuCauDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                {
                    if ((lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) || (lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauVo.SoLuong))
                    {
                        throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                    }

                    // tạo tạm thông tin yêu cầu dược phẩm khi ghi nhận trực tiếp
                    // khi duyệt phiếu lĩnh trực tiếp sẽ xử lý cập nhật lại thông tin đúng theo nhập chi tiết
                    // cập nhật yêu cầu dược phẩm (sửa, thêm), tạo phiếu xuất
                    if (yeuCauVo.SoLuong != null && yeuCauVo.SoLuong > 0)
                    {
                        var yeuCauNew = yeuCauDuocPham.Clone();

                        var thongTinNhapDuocPham = lstNhapChiTietTheoDuocPham.OrderByDescending(x => x.HanSuDung).First();
                        var giaTheoHopDong = duocPhamBenhVien.DuocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == thongTinNhapDuocPham.HopDongThauDuocPhamId).Gia;
                        var donGiaBaoHiem = thongTinNhapDuocPham.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapDuocPham.DonGiaNhap;

                        yeuCauNew.DonGiaNhap = thongTinNhapDuocPham.DonGiaNhap;
                        yeuCauNew.VAT = thongTinNhapDuocPham.VAT;
                        yeuCauNew.PhuongPhapTinhGiaTriTonKho = thongTinNhapDuocPham.PhuongPhapTinhGiaTriTonKho;
                        yeuCauNew.TiLeTheoThapGia = thongTinNhapDuocPham.TiLeTheoThapGia;
                        yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                        yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapDuocPham.TiLeBHYTThanhToan ?? 100;
                        yeuCauNew.SoLuong = yeuCauVo.SoLuong ?? 0;
                        yeuCauVo.SoLuong = 0;
                        yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                    }

                    //var yeuCauNew = yeuCauDuocPham.Clone();

                    //var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();
                    //double soLuongXuat = 0;
                    //foreach (var item in lstNhapChiTietTheoDuocPham)
                    //{
                    //    if (yeuCauVo.SoLuong > 0)
                    //    {
                    //        if (yeuCauNew.DonGiaNhap != 0
                    //            && (yeuCauNew.DonGiaNhap != item.DonGiaNhap || yeuCauNew.VAT != item.VAT || yeuCauNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != item.TiLeBHYTThanhToan))
                    //        {
                    //            //yeuCauNew.SoLuong = soLuongXuat;
                    //            //lstYeuCau.Add(yeuCauNew);

                    //            yeuCauNew = yeuCauDuocPham.Clone();
                    //            yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                    //            yeuCauNew.VAT = item.VAT;
                    //            yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                    //            yeuCauNew.DonGiaBaoHiem = item.DonGiaNhap;
                    //            yeuCauNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;
                    //        }
                    //        else
                    //        {
                    //            yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                    //            yeuCauNew.VAT = item.VAT;
                    //            yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                    //            yeuCauNew.DonGiaBaoHiem = item.DonGiaNhap;
                    //            yeuCauNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;
                    //        }

                    //        var tonTheoItem = item.SoLuongNhap - item.SoLuongDaXuat;
                    //        if (yeuCauVo.SoLuong <= tonTheoItem)
                    //        {
                    //            soLuongXuat += yeuCauVo.SoLuong.Value;
                    //            yeuCauVo.SoLuong = 0;
                    //        }
                    //        else
                    //        {
                    //            soLuongXuat += item.SoLuongNhap;
                    //            yeuCauVo.SoLuong -= tonTheoItem;
                    //        }

                    //        if (yeuCauVo.SoLuong == 0)
                    //        {
                    //            yeuCauNew.SoLuong = soLuongXuat;
                    //            lstYeuCau.Add(yeuCauNew);
                    //            break;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        break;
                    //    }
                    //}

                    //foreach (var item in lstYeuCau)
                    //{
                    //    yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Add(item);
                    //}
                }
            }

            // trường hợp ghi nhận vật tư
            else //thongTinDichVuGhiNhan.NhomId == (int) EnumNhomGoiDichVu.VatTu
            {
                var yeuCauVatTu = new YeuCauVatTuBenhVien()
                {
                    YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                    KhoLinhId = yeuCauVo.KhoId,
                    LoaiNoiChiDinh = yeuCauVo.LoaiNoiChiDinh
                };

                // bỏ bớt include bị dư, và await
                var yeuCauDichVuKyThuat =  BaseRepository.TableNoTracking
                    //.Include(p => p.YeuCauTiepNhan)
                    .First(p => p.Id == yeuCauVo.DichVuChiDinhId);
                yeuCauVatTu.NoiTruPhieuDieuTriId = yeuCauDichVuKyThuat.NoiTruPhieuDieuTriId;

                //if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKhamBenh)
                //{
                //    yeuCauVatTu.YeuCauKhamBenhId = thongTinDichVuChiDinh.Id;
                //}
                //else if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKyThuat)
                //{
                //    yeuCauVatTu.YeuCauDichVuKyThuatId = thongTinDichVuChiDinh.Id;
                //}

                yeuCauVatTu.YeuCauDichVuKyThuatId = yeuCauVo.DichVuChiDinhId;


                // bỏ bớt include bị dư, và await
                var vatTuBenhVien =  _vatTuBenhVienRepository.TableNoTracking.Include(p => p.VatTus)
                                                                                  .ThenInclude(y => y.HopDongThauVatTuChiTiets)
                                                                                  .FirstOrDefault(p => p.Id == thongTinDichVuGhiNhan.Id);

                if (vatTuBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.DichVuChiDinhId.NotExists"));
                }

                if (kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                {
                    yeuCauVatTu.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan;
                }
                else
                {
                    yeuCauVatTu.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu;
                }

                yeuCauVatTu.VatTuBenhVienId = vatTuBenhVien.Id;
                yeuCauVatTu.Ten = vatTuBenhVien.VatTus.Ten;
                yeuCauVatTu.Ma = vatTuBenhVien.VatTus.Ma;
                yeuCauVatTu.NhomVatTuId = vatTuBenhVien.VatTus.NhomVatTuId;
                yeuCauVatTu.DonViTinh = vatTuBenhVien.VatTus.DonViTinh;
                yeuCauVatTu.NhaSanXuat = vatTuBenhVien.VatTus.NhaSanXuat;
                yeuCauVatTu.NuocSanXuat = vatTuBenhVien.VatTus.NuocSanXuat;
                yeuCauVatTu.QuyCach = vatTuBenhVien.VatTus.QuyCach;
                yeuCauVatTu.MoTa = vatTuBenhVien.VatTus.MoTa;

                yeuCauVatTu.KhongTinhPhi = !yeuCauVo.TinhPhi;
                yeuCauVatTu.LaVatTuBHYT = yeuCauVo.LaDuocPhamBHYT;

                yeuCauVatTu.SoLuong = yeuCauVo.SoLuong.Value;

                yeuCauVatTu.NhanVienChiDinhId = currentUserId;
                yeuCauVatTu.NoiChiDinhId = phongHienTaiId;
                yeuCauVatTu.ThoiDiemChiDinh = DateTime.Now;
                yeuCauVatTu.GiaiDoanPhauThuat = yeuCauVo.GiaiDoanPhauThuat;

                yeuCauVatTu.DaCapVatTu = false;
                yeuCauVatTu.TrangThai = EnumYeuCauVatTuBenhVien.ChuaThucHien;
                yeuCauVatTu.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;

                // thông tin bảo hiểm
                //var giaBaoHiem = vatTuBenhVien.VatTuBenhVienGiaBaoHiems.FirstOrDefault(x => x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));

                //if (giaBaoHiem != null)
                //{
                //    yeuCauVatTu.DonGiaBaoHiem = giaBaoHiem.Gia;
                //    yeuCauVatTu.TiLeBaoHiemThanhToan = giaBaoHiem.TiLeBaoHiemThanhToan;

                //}
                //yeuCauVatTu.DuocHuongBaoHiem = yeuCauKhamBenh.YeuCauTiepNhan.CoBHYT == true && yeuCauKhamBenh.DuocHuongBaoHiem && giaBaoHiem != null && giaBaoHiem.Gia > 0;
                yeuCauVatTu.DuocHuongBaoHiem = yeuCauVo.LaDuocPhamBHYT;

                //Update nơi chỉ định
                yeuCauVatTu.LoaiNoiChiDinh = yeuCauVo.LoaiNoiChiDinh;

                var lstNhapChiTietTheoVatTu = new List<NhapKhoVatTuChiTiet>();
                if (!yeuCauVo.NhapKhoVatTuChiTiets.Any())
                {
                    lstNhapChiTietTheoVatTu = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoVatTu.KhoId == yeuCauVo.KhoId &&
                                    x.VatTuBenhVienId == vatTuBenhVien.Id &&
                                    x.NhapKhoVatTu.DaHet != true &&
                                    x.LaVatTuBHYT == yeuCauVo.LaDuocPhamBHYT &&
                                    x.SoLuongNhap > x.SoLuongDaXuat

                                    //BVHD-3821
                                    // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                                    && x.HanSuDung.Date >= DateTime.Now.Date)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToListAsync();
                }
                else
                {
                    lstNhapChiTietTheoVatTu = yeuCauVo.NhapKhoVatTuChiTiets.Where(p => p.VatTuBenhVienId == vatTuBenhVien.Id &&
                                                                                       p.LaVatTuBHYT == yeuCauVo.LaDuocPhamBHYT &&
                                                                                       p.SoLuongNhap > p.SoLuongDaXuat

                                                                                       //BVHD-3821
                                                                                       // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                                                                                       && p.HanSuDung.Date >= DateTime.Now.Date)
                                                                           .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                           .ToList();
                }

                //if ((lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) || (lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauVo.SoLuong))
                //{
                //    throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                //}

                if (yeuCauVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                {
                    var soLuongTonTrongKho = Math.Round(lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat), 2);
                    if ((soLuongTonTrongKho < 0) || (soLuongTonTrongKho < yeuCauVo.SoLuong))
                    {
                        throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                    }

                    var yeuCauNew = yeuCauVatTu.Clone();
                    var xuatChiTiet = new XuatKhoVatTuChiTiet()
                    {
                        VatTuBenhVienId = thongTinDichVuGhiNhan.Id
                    };

                    var lstYeuCau = new List<YeuCauVatTuBenhVien>();
                    foreach (var item in lstNhapChiTietTheoVatTu)
                    {
                        if (yeuCauVo.SoLuong > 0)
                        {
                            var giaTheoHopDong = vatTuBenhVien.VatTus.HopDongThauVatTuChiTiets.First(o => o.HopDongThauVatTuId == item.HopDongThauVatTuId).Gia;
                            var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;

                            var tileBHYTThanhToanTheoNhap = item.LaVatTuBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;
                            if (yeuCauNew.DonGiaNhap != 0
                                && (yeuCauNew.DonGiaNhap != item.DonGiaNhap || yeuCauNew.VAT != item.VAT || yeuCauNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap))
                            {
                                yeuCauNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                                yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat), 2);
                                lstYeuCau.Add(yeuCauNew);

                                yeuCauNew = yeuCauVatTu.Clone();
                                yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauNew.VAT = item.VAT;
                                yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap; //item.TiLeBHYTThanhToan ?? 100;

                                xuatChiTiet = new XuatKhoVatTuChiTiet()
                                {
                                    VatTuBenhVienId = thongTinDichVuGhiNhan.Id
                                };
                            }
                            else
                            {
                                yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauNew.VAT = item.VAT;
                                yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap; //item.TiLeBHYTThanhToan ?? 100;
                            }

                            var xuatViTri = new XuatKhoVatTuChiTietViTri()
                            {
                                NhapKhoVatTuChiTietId = item.Id
                            };

                            //var tonTheoItem = item.SoLuongNhap - item.SoLuongDaXuat;
                            var tonTheoItem = Math.Round(item.SoLuongNhap - item.SoLuongDaXuat, 2);
                            if (yeuCauVo.SoLuong < tonTheoItem || yeuCauVo.SoLuong.Value.AlmostEqual(tonTheoItem))
                            {
                                xuatViTri.SoLuongXuat = yeuCauVo.SoLuong.Value;
                                item.SoLuongDaXuat = Math.Round(item.SoLuongDaXuat + yeuCauVo.SoLuong.Value, 2);
                                yeuCauVo.SoLuong = 0;
                            }
                            else
                            {
                                xuatViTri.SoLuongXuat = tonTheoItem;
                                item.SoLuongDaXuat = item.SoLuongNhap;
                                yeuCauVo.SoLuong = Math.Round(yeuCauVo.SoLuong.Value - tonTheoItem, 2);
                            }

                            xuatChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatViTri);

                            if (yeuCauVo.SoLuong.Value.AlmostEqual(0))
                            {
                                yeuCauNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                                yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat), 2);
                                lstYeuCau.Add(yeuCauNew);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    //yeuCauVatTu.XuatKhoVatTuChiTiet = xuatChiTiet;
                    //await _yeuCauVatTuBenhVienRepository.AddAsync(yeuCauVatTu);

                    //await _yeuCauVatTuBenhVienRepository.AddRangeAsync(lstYeuCau);
                    //await _nhapKhoVatTuChiTietRepository.UpdateAsync(lstNhapKhoVatTuChiTiet);

                    foreach (var item in lstYeuCau)
                    {
                        yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Add(item);
                    }

                    yeuCauVo.NhapKhoVatTuChiTiets = lstNhapChiTietTheoVatTu;
                }

                if (yeuCauVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                {
                    if ((lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) || (lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauVo.SoLuong))
                    {
                        throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                    }

                    // tạo tạm thông tin yêu cầu dược phẩm khi ghi nhận trực tiếp
                    // khi duyệt phiếu lĩnh trực tiếp sẽ xử lý cập nhật lại thông tin đúng theo nhập chi tiết
                    // cập nhật yêu cầu dược phẩm (sửa, thêm), tạo phiếu xuất
                    if (yeuCauVo.SoLuong != null && yeuCauVo.SoLuong > 0)
                    {
                        var yeuCauNew = yeuCauVatTu.Clone();

                        var thongTinNhapDuocPham = lstNhapChiTietTheoVatTu.First();
                        var giaTheoHopDong = vatTuBenhVien.VatTus.HopDongThauVatTuChiTiets.First(o => o.HopDongThauVatTuId == thongTinNhapDuocPham.HopDongThauVatTuId).Gia;
                        var donGiaBaoHiem = thongTinNhapDuocPham.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapDuocPham.DonGiaNhap;

                        yeuCauNew.DonGiaNhap = thongTinNhapDuocPham.DonGiaNhap;
                        yeuCauNew.VAT = thongTinNhapDuocPham.VAT;
                        yeuCauNew.TiLeTheoThapGia = thongTinNhapDuocPham.TiLeTheoThapGia;
                        yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                        yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapDuocPham.TiLeBHYTThanhToan ?? 100;
                        yeuCauNew.SoLuong = yeuCauVo.SoLuong ?? 0;
                        yeuCauVo.SoLuong = 0;
                        yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Add(yeuCauNew);
                    }

                    //var yeuCauNew = yeuCauVatTu.Clone();

                    //var lstYeuCau = new List<YeuCauVatTuBenhVien>();
                    //double soLuongXuat = 0;
                    //foreach (var item in lstNhapChiTietTheoVatTu)
                    //{
                    //    if (yeuCauVo.SoLuong > 0)
                    //    {
                    //        if (yeuCauNew.DonGiaNhap != 0
                    //            && (yeuCauNew.DonGiaNhap != item.DonGiaNhap || yeuCauNew.VAT != item.VAT || yeuCauNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != item.TiLeBHYTThanhToan))
                    //        {
                    //            //yeuCauNew.SoLuong = soLuongXuat;
                    //            //lstYeuCau.Add(yeuCauNew);

                    //            yeuCauNew = yeuCauVatTu.Clone();
                    //            yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                    //            yeuCauNew.VAT = item.VAT;
                    //            yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                    //            yeuCauNew.DonGiaBaoHiem = item.DonGiaNhap;
                    //            yeuCauNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;
                    //        }
                    //        else
                    //        {
                    //            yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                    //            yeuCauNew.VAT = item.VAT;
                    //            yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                    //            yeuCauNew.DonGiaBaoHiem = item.DonGiaNhap;
                    //            yeuCauNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;
                    //        }

                    //        var tonTheoItem = item.SoLuongNhap - item.SoLuongDaXuat;
                    //        if (yeuCauVo.SoLuong <= tonTheoItem)
                    //        {
                    //            soLuongXuat += yeuCauVo.SoLuong.Value;
                    //            yeuCauVo.SoLuong = 0;
                    //        }
                    //        else
                    //        {
                    //            soLuongXuat += item.SoLuongNhap;
                    //            yeuCauVo.SoLuong -= tonTheoItem;
                    //        }

                    //        if (yeuCauVo.SoLuong == 0)
                    //        {
                    //            yeuCauNew.SoLuong = soLuongXuat;
                    //            lstYeuCau.Add(yeuCauNew);
                    //            break;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        break;
                    //    }
                    //}

                    //foreach (var item in lstYeuCau)
                    //{
                    //    yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Add(item);
                    //}
                }
            }
        }

        public void XuLyXoaYeuCauGhiNhanVTTHThuocAsync(YeuCauTiepNhan yeuCauTiepNhanChiTiet, string yeuCauGhiNhanId)
        {
            //var yeuCauGhiNhanObj = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(yeuCauGhiNhanId);
            var lstYeuCauCanXoaId = yeuCauGhiNhanId.Split(";");
            var yeuCauGhiNhanObjs = new List<DichVuGhiNhanVo>();
            foreach (var strId in lstYeuCauCanXoaId)
            {
                yeuCauGhiNhanObjs.Add(JsonConvert.DeserializeObject<DichVuGhiNhanVo>(strId));
            }

            foreach (var yeuCauGhiNhanObj in yeuCauGhiNhanObjs)
            {
                if (yeuCauGhiNhanObj.NhomId == (int)EnumNhomGoiDichVu.DuocPham)
                {
                    var yeuCauDuocPham =
                        yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.FirstOrDefault(x => x.Id == yeuCauGhiNhanObj.Id);
                    if (yeuCauDuocPham == null)
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.NotExists"));
                    }

                    //if (yeuCauDuocPham.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    //{
                    //    throw new Exception(_localizationService.GetResource("XoaGhiNhanVTTHThuoc.YeuCauDuocPham.DaThanhToan"));
                    //}

                    //if (yeuCauDuocPham.XuatKhoDuocPhamChiTiet?.XuatKhoDuocPhamId != null)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaXuat"));
                    //}

                    if (yeuCauDuocPham.YeuCauLinhDuocPhamId != null ||
                        yeuCauDuocPham.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false))
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaLinh"));
                    }

                    yeuCauDuocPham.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;

                    if (yeuCauDuocPham.XuatKhoDuocPhamChiTiet != null)
                    {
                        foreach (var thongTinXuat in yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
                        {
                            thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = Math.Round(thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - thongTinXuat.SoLuongXuat, 2);
                        }

                        var xuatKhoDpViTris =
                            yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.ToList();
                        foreach (var item in xuatKhoDpViTris)
                        {
                            var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                            {
                                XuatKhoDuocPhamChiTietId = item.XuatKhoDuocPhamChiTietId,
                                NhapKhoDuocPhamChiTietId = item.NhapKhoDuocPhamChiTietId,
                                SoLuongXuat = -item.SoLuongXuat,
                                NgayXuat = DateTime.Now,
                                GhiChu = yeuCauDuocPham.TrangThai.GetDescription()
                            };
                            yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(
                                xuatKhoDuocPhamChiTietViTri);
                        }
                    }

                    //yeuCauDuocPham.WillDelete = true;
                }
                else if (yeuCauGhiNhanObj.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao)
                {
                    var yeuCauVatTu =
                        yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.FirstOrDefault(x => x.Id == yeuCauGhiNhanObj.Id);
                    if (yeuCauVatTu == null)
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.NotExists"));
                    }

                    //if (yeuCauVatTu.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    //{
                    //    throw new Exception(_localizationService.GetResource("XoaGhiNhanVTTHThuoc.YeuCauVatTu.DaThanhToan"));
                    //}

                    //if (yeuCauVatTu?.XuatKhoVatTuChiTiet?.XuatKhoVatTuId != null)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaXuat"));
                    //}

                    if (yeuCauVatTu.YeuCauLinhVatTuId != null ||
                        yeuCauVatTu.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false))
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaLinh"));
                    }

                    yeuCauVatTu.TrangThai = EnumYeuCauVatTuBenhVien.DaHuy;

                    if (yeuCauVatTu.XuatKhoVatTuChiTiet != null)
                    {
                        foreach (var thongTinXuat in yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris)
                        {
                            thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat = Math.Round(thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat - thongTinXuat.SoLuongXuat, 2);
                        }

                        var xuatKhoVtViTris = yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.ToList();
                        foreach (var item in xuatKhoVtViTris)
                        {
                            var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                            {
                                XuatKhoVatTuChiTietId = item.XuatKhoVatTuChiTietId,
                                NhapKhoVatTuChiTietId = item.NhapKhoVatTuChiTietId,
                                SoLuongXuat = -item.SoLuongXuat,
                                NgayXuat = DateTime.Now,
                                GhiChu = yeuCauVatTu.TrangThai.GetDescription()
                            };

                            yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                        }
                    }

                    //yeuCauVatTu.WillDelete = true;
                }
                else
                {
                    throw new Exception(_localizationService.GetResource("XoaGhiNhanVTTHThuoc.NotExists"));
                }
            }
        }

        public async Task XuLyXuatYeuCauGhiNhanVTTHThuocAsync(ChiDinhGhiNhanVatTuThuocPTTTVo yeuCauVo)
        {
            //var yeuCauDichVuKyThuat = await _yeuCauDichVuKyThuatRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == yeuCauVo.YeuCauDichVuKyThuatId);
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            bool coPhieuXuat = false;

            //var lstGhiNhanDuocPham = await _yeuCauDuocPhamBenhVienRepository.Table.Include(p => p.XuatKhoDuocPhamChiTiet)
            //                                                                      //.Include(p => p.YeuCauKhamBenh).ThenInclude(p => p.YeuCauTiepNhan)
            //                                                                      .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauTiepNhan)
            //                                                                      .Where(p => p.KhoLinh != null &&
            //                                                                                  p.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
            //                                                                                  //(p.TrangThaiThanhToan == TrangThaiThanhToan.BaoLanhThanhToan || p.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan) &&
            //                                                                                  p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
            //                                                                                  p.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null &&
            //                                                                                  p.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
            //                                                                                  p.YeuCauDichVuKyThuatId == yeuCauVo.YeuCauDichVuKyThuatId)
            //                                                                      .ToListAsync();

            var lstAllGhiNhanDuocPham = _yeuCauDuocPhamBenhVienRepository.Table.Include(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(y => y.XuatKhoDuocPham)
                                                                                     .Include(x => x.YeuCauTiepNhan)
                                                                                     .Where(x => x.KhoLinh != null &&
                                                                                                 x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                                                                                                 x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                                                                                                 //x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null &&
                                                                                                 x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                                                                                 x.YeuCauDichVuKyThuatId == yeuCauVo.YeuCauDichVuKyThuatId)
                                                                                     .ToList();

            var lstPhieuXuatDaXuat = lstAllGhiNhanDuocPham.Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null)
                                                          .Select(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham)
                                                          .Distinct()
                                                          .ToList();

            var lstGhiNhanDuocPhamChuaXuat = lstAllGhiNhanDuocPham.Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null).ToList();

            if (lstGhiNhanDuocPhamChuaXuat.Any())
            {
                var lstPhieuXuatDuocPham = new List<Core.Domain.Entities.XuatKhos.XuatKhoDuocPham>();
                var lstKhoId = lstGhiNhanDuocPhamChuaXuat.Where(x => x.KhoLinhId != null).Select(x => x.KhoLinhId.Value).Distinct().ToList();

                var phieuXuatTemp = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham()
                {
                    //KhoXuatId = khoId,
                    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                    LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                    NguoiXuatId = currentUserId,
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                    NgayXuat = DateTime.Now
                };

                foreach (var khoId in lstKhoId)
                {
                    var phieuXuatNew = phieuXuatTemp.Clone();
                    phieuXuatNew.KhoXuatId = khoId;

                    var lstYeuCauTheoKho = lstGhiNhanDuocPhamChuaXuat.Where(x => x.KhoLinhId == khoId && x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null).ToList();
                    if (lstYeuCauTheoKho.Any())
                    {
                        //var phieuXuatNew = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham()
                        //{
                        //    KhoXuatId = khoId,
                        //    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                        //    LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                        //    NguoiXuatId = currentUserId,
                        //    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                        //    NgayXuat = DateTime.Now
                        //};

                        foreach (var yeuCau in lstYeuCauTheoKho)
                        {
                            coPhieuXuat = true;
                            // lấy thông tin tên người bệnh
                            var tenBenhNhan = yeuCau.YeuCauTiepNhan.HoTen;
                            //if (yeuCau.YeuCauKhamBenh != null)
                            //{
                            //    tenBenhNhan = yeuCau.YeuCauKhamBenh.YeuCauTiepNhan.HoTen;
                            //}
                            //else if (yeuCau.YeuCauDichVuKyThuat != null)
                            //{
                            //    tenBenhNhan = yeuCau.YeuCauDichVuKyThuat.YeuCauTiepNhan.HoTen;
                            //}

                            //if (string.IsNullOrEmpty(phieuXuatNew.TenNguoiNhan))
                            //{
                            //    phieuXuatNew.TenNguoiNhan = tenBenhNhan;
                            //}
                            //else
                            //{
                            //    if (phieuXuatNew.TenNguoiNhan.Trim().ToLower() != tenBenhNhan.Trim().ToLower())
                            //    {
                            //        phieuXuatNew = phieuXuatTemp.Clone();
                            //        phieuXuatNew.KhoXuatId = khoId;
                            //    }
                            //}

                            // xử lý kiểm tra gộp phiếu xuất
                            var phieuXuatDaXuat = lstPhieuXuatDaXuat.FirstOrDefault(x => x.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatChoBenhNhan &&
                                                                                         x.TenNguoiNhan != null &&
                                                                                         x.TenNguoiNhan.Trim().ToLower() == tenBenhNhan.Trim().ToLower() &&
                                                                                         x.NguoiXuatId == currentUserId &&
                                                                                         x.KhoXuatId == khoId);
                            if (phieuXuatDaXuat != null)
                            {
                                phieuXuatNew = phieuXuatDaXuat;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(phieuXuatNew.TenNguoiNhan))
                                {
                                    phieuXuatNew.TenNguoiNhan = tenBenhNhan;
                                }
                                else
                                {
                                    if (phieuXuatNew.TenNguoiNhan.Trim().ToLower() != tenBenhNhan.Trim().ToLower())
                                    {
                                        phieuXuatNew = phieuXuatTemp.Clone();
                                        phieuXuatNew.KhoXuatId = khoId;
                                    }
                                }
                            }

                            yeuCau.XuatKhoDuocPhamChiTiet.NgayXuat = DateTime.Now;
                            yeuCau.TrangThai = EnumYeuCauDuocPhamBenhVien.DaThucHien;
                            yeuCau.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = phieuXuatNew;
                            //phieuXuatNew.XuatKhoDuocPhamChiTiets.Add(yeuCau.XuatKhoDuocPhamChiTiet);
                        }

                        //lstPhieuXuatDuocPham.Add(phieuXuatNew);
                    }
                }

                if (coPhieuXuat)
                {
                    //await _yeuCauDuocPhamBenhVienRepository.UpdateAsync(lstAllGhiNhanDuocPham);
                    await _yeuCauDuocPhamBenhVienRepository.Context.SaveChangesAsync();
                }

                //if (lstPhieuXuatDuocPham.Any())
                //{
                //    coPhieuXuat = true;
                //    await _xuatKhoDuocPhamRepository.AddRangeAsync(lstPhieuXuatDuocPham);
                //}
            }

            //var lstGhiNhanVatTu = await _yeuCauVatTuBenhVienRepository.Table.Include(p => p.XuatKhoVatTuChiTiet)
            //                                                                //.Include(p => p.YeuCauKhamBenh).ThenInclude(p => p.YeuCauTiepNhan)
            //                                                                .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauTiepNhan)
            //                                                                .Where(p => p.KhoLinh != null &&
            //                                                                            p.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
            //                                                                            p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
            //                                                                            p.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null &&
            //                                                                            p.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
            //                                                                            p.YeuCauDichVuKyThuatId == yeuCauVo.YeuCauDichVuKyThuatId)
            //                                                                .ToListAsync();

            var lstAllGhiNhanVatTu = _yeuCauVatTuBenhVienRepository.Table
                .Include(x => x.XuatKhoVatTuChiTiet).ThenInclude(y => y.XuatKhoVatTu)
                .Include(x => x.YeuCauTiepNhan)
                .Where(x => x.KhoLinh != null &&
                            x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                            x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
                            //x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null &&
                            x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                            x.YeuCauDichVuKyThuatId == yeuCauVo.YeuCauDichVuKyThuatId)
                .ToList();

            var lstPhieuXuatVatTuDaXuat = lstAllGhiNhanVatTu.Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null)
                .Select(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTu)
                .Distinct()
                .ToList();

            var lstGhiNhanVatTuChuaXuat = lstAllGhiNhanVatTu.Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null).ToList();
            if (lstGhiNhanVatTuChuaXuat.Any())
            {
                var lstPhieuXuatVatTu = new List<XuatKhoVatTu>();
                var lstKhoId = lstGhiNhanVatTuChuaXuat.Where(x => x.KhoLinhId != null).Select(x => x.KhoLinhId.Value).Distinct().ToList();

                var phieuXuatTemp = new XuatKhoVatTu()
                {
                    //KhoXuatId = khoId,
                    LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                    LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                    NguoiXuatId = currentUserId,
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                    NgayXuat = DateTime.Now
                };

                foreach (var khoId in lstKhoId)
                {
                    var phieuXuatNew = phieuXuatTemp.Clone();
                    phieuXuatNew.KhoXuatId = khoId;

                    var lstYeuCauTheoKho = lstGhiNhanVatTuChuaXuat.Where(x => x.KhoLinhId == khoId && x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null).ToList();
                    if (lstYeuCauTheoKho.Any())
                    {
                        //var phieuXuatNew = new XuatKhoVatTu()
                        //{
                        //    KhoXuatId = khoId,
                        //    LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                        //    LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                        //    NguoiXuatId = currentUserId,
                        //    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                        //    NgayXuat = DateTime.Now
                        //};
                        coPhieuXuat = true;

                        foreach (var yeuCau in lstYeuCauTheoKho)
                        {
                            // lấy thông tin tên người bệnh
                            var tenBenhNhan = yeuCau.YeuCauTiepNhan.HoTen;
                            //if (yeuCau.YeuCauKhamBenh != null)
                            //{
                            //    tenBenhNhan = yeuCau.YeuCauKhamBenh.YeuCauTiepNhan.HoTen;
                            //}
                            //else if (yeuCau.YeuCauDichVuKyThuat != null)
                            //{
                            //    tenBenhNhan = yeuCau.YeuCauDichVuKyThuat.YeuCauTiepNhan.HoTen;
                            //}

                            //if (string.IsNullOrEmpty(phieuXuatNew.TenNguoiNhan))
                            //{
                            //    phieuXuatNew.TenNguoiNhan = tenBenhNhan;
                            //}
                            //else
                            //{
                            //    if (phieuXuatNew.TenNguoiNhan.Trim().ToLower() != tenBenhNhan.Trim().ToLower())
                            //    {
                            //        phieuXuatNew = phieuXuatTemp.Clone();
                            //        phieuXuatNew.KhoXuatId = khoId;
                            //    }
                            //}

                            var phieuXuatDaXuat = lstPhieuXuatVatTuDaXuat.FirstOrDefault(x =>
                                x.LoaiXuatKho == EnumLoaiXuatKho.XuatChoBenhNhan
                                && x.TenNguoiNhan != null
                                && x.TenNguoiNhan.Trim().ToLower() == tenBenhNhan.Trim().ToLower()
                                && x.NguoiXuatId == currentUserId
                                && x.KhoXuatId == khoId);
                            if (phieuXuatDaXuat != null)
                            {
                                phieuXuatNew = phieuXuatDaXuat;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(phieuXuatNew.TenNguoiNhan))
                                {
                                    phieuXuatNew.TenNguoiNhan = tenBenhNhan;
                                }
                                else
                                {
                                    if (phieuXuatNew.TenNguoiNhan.Trim().ToLower() != tenBenhNhan.Trim().ToLower())
                                    {
                                        phieuXuatNew = phieuXuatTemp.Clone();
                                        phieuXuatNew.KhoXuatId = khoId;
                                    }
                                }
                            }

                            yeuCau.XuatKhoVatTuChiTiet.NgayXuat = DateTime.Now;
                            yeuCau.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                            yeuCau.XuatKhoVatTuChiTiet.XuatKhoVatTu = phieuXuatNew;
                            //phieuXuatNew.XuatKhoVatTuChiTiets.Add(yeuCau.XuatKhoVatTuChiTiet);
                        }

                        //lstPhieuXuatVatTu.Add(phieuXuatNew);
                    }
                }

                //if (lstPhieuXuatVatTu.Any())
                //{
                //    coPhieuXuat = true;
                //    await _xuatKhoVatTuRepository.AddRangeAsync(lstPhieuXuatVatTu);
                //}

                if (coPhieuXuat)
                {
                    //await _yeuCauVatTuBenhVienRepository.UpdateAsync(lstAllGhiNhanVatTu);
                    await _yeuCauVatTuBenhVienRepository.Context.SaveChangesAsync();
                }
            }

            //Update xuất khi thêm
            //if (!coPhieuXuat)
            //{
            //    throw new Exception(_localizationService.GetResource("XuatGhiNhatVTTHThuoc.DanhSachYeuCauChuaXuat.KhongCo"));
            //}
        }

        public async Task CapNhatGridItemGhiNhanVTTHThuocAsync(YeuCauTiepNhan yeuCauTiepNhanChiTiet, ChiDinhGhiNhanVatTuThuocPTTTVo ghiNhanVo)
        {
            string templateKeyId = "\"Id\": {0}, \"NhomId\": {1}";
            //var yeuCauGhiNhanObj = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(ghiNhanVo.YeuCauGhiNhanVTTHThuocId);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            var lstYeuCauCanXoaId = ghiNhanVo.YeuCauGhiNhanVTTHThuocId.Split(";");
            var yeuCauGhiNhanObjs = new List<DichVuGhiNhanVo>();
            foreach (var strId in lstYeuCauCanXoaId)
            {
                yeuCauGhiNhanObjs.Add(JsonConvert.DeserializeObject<DichVuGhiNhanVo>(strId));
            }

            foreach (var yeuCauGhiNhanObj in yeuCauGhiNhanObjs)
            {
                if (yeuCauGhiNhanObj.NhomId == (int)EnumNhomGoiDichVu.DuocPham)
                {
                    var yeuCauDuocPham =
                        yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.FirstOrDefault(p => p.Id == yeuCauGhiNhanObj.Id);
                    if (yeuCauDuocPham == null)
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.NotExists"));
                    }

                    //if (yeuCauDuocPham.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaThanhToan"));
                    //}

                    //if (yeuCauDuocPham.XuatKhoDuocPhamChiTiet?.XuatKhoDuocPhamId != null)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaXuat"));
                    //}

                    if (yeuCauDuocPham.YeuCauLinhDuocPhamId != null ||
                        yeuCauDuocPham.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false))
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaLinh"));
                    }

                    //Trường hợp cập nhật số lượng
                    if (ghiNhanVo.IsCapNhatSoLuong)
                    {
                        if (yeuCauDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                        {
                            // get thông tin nhập kho chi tiết
                            var lstNhapKhoDuocPhamChiTiet = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauDuocPham.KhoLinhId
                                            && x.HanSuDung >= DateTime.Now
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                .OrderBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                .ThenBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                .ToListAsync();

                            var chiTietXuat = yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris
                                .OrderByDescending(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung
                                        ? p.NhapKhoDuocPhamChiTiet.HanSuDung
                                        : p.NhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien)
                                .ThenByDescending(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung
                                        ? p.NhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien
                                        : p.NhapKhoDuocPhamChiTiet.HanSuDung);
                            var soLuongHienTai = chiTietXuat.Sum(p => p.SoLuongXuat);

                            // trường hợp tăng số lượng
                            if (soLuongHienTai < ghiNhanVo.SoLuongCapNhat)
                            {
                                var soLuongTang = ghiNhanVo.SoLuongCapNhat.Value - soLuongHienTai;
                                foreach (var thongTinXuat in chiTietXuat)
                                {
                                    if (soLuongTang > 0)
                                    {
                                        var soLuongTon = thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongNhap -
                                                         thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                                        if (soLuongTon > 0)
                                        {
                                            if (soLuongTon <= soLuongTang)
                                            {
                                                thongTinXuat.SoLuongXuat +=
                                                    (thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongNhap -
                                                     thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat);
                                                thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat =
                                                    thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongNhap;

                                                var nhapChiTiet = lstNhapKhoDuocPhamChiTiet.First(x =>
                                                    x.Id == thongTinXuat.NhapKhoDuocPhamChiTietId);
                                                nhapChiTiet.SoLuongDaXuat =
                                                    thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat;

                                                soLuongTang -= soLuongTon;
                                            }
                                            else
                                            {
                                                thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongTang;
                                                thongTinXuat.SoLuongXuat += soLuongTang;

                                                var nhapChiTiet = lstNhapKhoDuocPhamChiTiet.First(x =>
                                                    x.Id == thongTinXuat.NhapKhoDuocPhamChiTietId);
                                                nhapChiTiet.SoLuongDaXuat += soLuongTang;

                                                soLuongTang = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                yeuCauDuocPham.SoLuong = yeuCauDuocPham.XuatKhoDuocPhamChiTiet
                                    .XuatKhoDuocPhamChiTietViTris.Where(p => p.WillDelete != true)
                                    .Sum(x => x.SoLuongXuat);

                                // nếu vẫn còn dư số lượng, xử lý thêm mới YeuCauDuocPham
                                // kiếm tra nếu tồn dược phẩm ko đủ, thông báo
                                if (soLuongTang > 0)
                                {
                                    //var lstNhapKhoDuocPhamChiTiet = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(p => p.NhapKhoDuocPhams.KhoId == yeuCauDuocPham.KhoLinhId &&
                                    //                                                                                                   p.NhapKhoDuocPhams.DaHet != true)
                                    //                                                                                       .OrderBy(x => x.NgayNhapVaoBenhVien)
                                    //                                                                                       .ToListAsync();

                                    if ((lstNhapKhoDuocPhamChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) ||
                                        (lstNhapKhoDuocPhamChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                                         soLuongTang))
                                    {
                                        throw new Exception(
                                            _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                                    }

                                    var yeuCauNew = new YeuCauDuocPhamBenhVien();
                                    var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                                    {
                                        DuocPhamBenhVienId = yeuCauDuocPham.DuocPhamBenhVienId
                                    };

                                    bool flagTaoMoi = false;
                                    bool flagChangeTonNhapChiTiet = false;
                                    foreach (var item in lstNhapKhoDuocPhamChiTiet)
                                    {
                                        if (soLuongTang > 0)
                                        {
                                            if (yeuCauDuocPham.DonGiaNhap == item.DonGiaNhap &&
                                                yeuCauDuocPham.VAT == item.VAT && yeuCauDuocPham.TiLeTheoThapGia ==
                                                item.TiLeTheoThapGia)
                                            {
                                                flagChangeTonNhapChiTiet = true;
                                                var newXuatViTri = new XuatKhoDuocPhamChiTietViTri()
                                                {
                                                    NhapKhoDuocPhamChiTietId = item.Id
                                                };

                                                var slTon = item.SoLuongNhap - item.SoLuongDaXuat;
                                                if (slTon >= soLuongTang)
                                                {
                                                    newXuatViTri.SoLuongXuat = soLuongTang;
                                                    item.SoLuongDaXuat += soLuongTang;
                                                    soLuongTang = 0;
                                                }
                                                else
                                                {
                                                    newXuatViTri.SoLuongXuat = item.SoLuongNhap - item.SoLuongDaXuat;
                                                    soLuongTang -= slTon;
                                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                                }

                                                yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(
                                                    newXuatViTri);
                                                yeuCauDuocPham.SoLuong = yeuCauDuocPham.XuatKhoDuocPhamChiTiet
                                                    .XuatKhoDuocPhamChiTietViTris.Where(p => p.WillDelete != true)
                                                    .Sum(x => x.SoLuongXuat);
                                            }
                                            else
                                            {
                                                // gọi function tạo mới yêu cầu dược phẩm
                                                flagTaoMoi = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }

                                        yeuCauDuocPham.SoLuong = yeuCauDuocPham.XuatKhoDuocPhamChiTiet
                                            .XuatKhoDuocPhamChiTietViTris.Where(p => p.WillDelete != true)
                                            .Sum(x => x.SoLuongXuat);
                                    }

                                    if (soLuongTang > 0 && flagTaoMoi)
                                    {
                                        ghiNhanVo.SoLuong = soLuongTang;
                                        ghiNhanVo.LaDuocPhamBHYT = yeuCauDuocPham.LaDuocPhamBHYT;
                                        ghiNhanVo.KhoId = yeuCauDuocPham.KhoLinhId;
                                        ghiNhanVo.NhapKhoDuocPhamChiTiets = lstNhapKhoDuocPhamChiTiet;
                                        ghiNhanVo.TinhPhi = yeuCauDuocPham.KhongTinhPhi == null
                                            ? true
                                            : !yeuCauDuocPham.KhongTinhPhi;

                                        //long dichVuId = 0;
                                        //string loaiDichVu = "";
                                        //int loaiDichVuId = 0;

                                        //if (yeuCauDuocPham.YeuCauKhamBenhId != null)
                                        //{
                                        //    dichVuId = yeuCauDuocPham.YeuCauKhamBenhId.Value;
                                        //    loaiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                                        //}
                                        //else if (yeuCauDuocPham.YeuCauDichVuKyThuatId != null)
                                        //{
                                        //    dichVuId = yeuCauDuocPham.YeuCauDichVuKyThuatId.Value;
                                        //    loaiDichVu = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                        //}

                                        //YCDVKT
                                        //dichVuId = yeuCauDuocPham.YeuCauDichVuKyThuatId.Value;
                                        //loaiDichVu = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                        //loaiDichVuId = (int)EnumNhomGoiDichVu.DichVuKyThuat;

                                        //ghiNhanVo.DichVuChiDinhId = "{" + string.Format(templateKeyId, dichVuId, loaiDichVu) + "}"; // dịch chỉ định trong khám bệnh
                                        ghiNhanVo.DichVuChiDinhId = ghiNhanVo.YeuCauDichVuKyThuatId;
                                        ghiNhanVo.DichVuGhiNhanId = "{" + string.Format(templateKeyId,
                                                                        yeuCauDuocPham.DuocPhamBenhVienId,
                                                                        (int)EnumNhomGoiDichVu.DuocPham) +
                                                                    "}"; // dịch vụ dược phẩm/vật tư

                                        await XuLyThemGhiNhanVatTuBenhVienAsync(ghiNhanVo, yeuCauTiepNhanChiTiet);
                                    }
                                    else
                                    {
                                        if (flagChangeTonNhapChiTiet)
                                        {
                                            ghiNhanVo.NhapKhoDuocPhamChiTiets = lstNhapKhoDuocPhamChiTiet;
                                        }
                                    }
                                }
                                else
                                {
                                    yeuCauDuocPham.SoLuong = yeuCauDuocPham.XuatKhoDuocPhamChiTiet
                                        .XuatKhoDuocPhamChiTietViTris.Where(p => p.WillDelete != true)
                                        .Sum(x => x.SoLuongXuat);
                                }
                            }
                            // trường hợp giảm số lượng
                            else if (soLuongHienTai > ghiNhanVo.SoLuongCapNhat)
                            {
                                var soLuongGiam = soLuongHienTai - ghiNhanVo.SoLuongCapNhat.Value;
                                foreach (var thongTinXuat in chiTietXuat)
                                {
                                    if (soLuongGiam > 0)
                                    {
                                        if (thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat > soLuongGiam)
                                        {
                                            thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= soLuongGiam;
                                            thongTinXuat.SoLuongXuat -= soLuongGiam;
                                            soLuongGiam = 0;
                                        }
                                        else
                                        {
                                            soLuongGiam -= thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                                            thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = 0;
                                            thongTinXuat.WillDelete = true;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                yeuCauDuocPham.SoLuong = yeuCauDuocPham.XuatKhoDuocPhamChiTiet
                                    .XuatKhoDuocPhamChiTietViTris.Where(p => p.WillDelete != true)
                                    .Sum(x => x.SoLuongXuat);
                            }
                        }
                        else
                        {
                            // get thông tin nhập kho chi tiết
                            var lstNhapKhoDuocPhamChiTiet = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauDuocPham.KhoLinhId
                                            && x.DuocPhamBenhVienId == yeuCauDuocPham.DuocPhamBenhVienId
                                            && x.HanSuDung >= DateTime.Now
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.LaDuocPhamBHYT == yeuCauDuocPham.LaDuocPhamBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                .OrderBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                .ThenBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                .ToListAsync();

                            if (lstNhapKhoDuocPhamChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                                ghiNhanVo.SoLuongCapNhat)
                            {
                                throw new Exception(
                                    _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                            }

                            yeuCauDuocPham.SoLuong = ghiNhanVo.SoLuongCapNhat ?? 0;
                        }
                    }

                    if (ghiNhanVo.IsCapNhatTinhPhi)
                    {
                        yeuCauDuocPham.KhongTinhPhi = !ghiNhanVo.TinhPhi;
                    }
                }
                else if (yeuCauGhiNhanObj.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao)
                {
                    var yeuCauVatTu =
                        yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.FirstOrDefault(x => x.Id == yeuCauGhiNhanObj.Id);
                    if (yeuCauVatTu == null)
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.NotExists"));
                    }

                    //if (yeuCauVatTu.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaThanhToan"));
                    //}

                    //if (yeuCauVatTu.XuatKhoVatTuChiTiet?.XuatKhoVatTuId != null)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaXuat"));
                    //}

                    if (yeuCauVatTu.YeuCauLinhVatTuId != null ||
                        yeuCauVatTu.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false))
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaLinh"));
                    }

                    //Trường hợp cập nhật số lượng
                    if (ghiNhanVo.IsCapNhatSoLuong)
                    {
                        if (yeuCauVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                        {
                            //get thông tin nhập chi tiết
                            var lstNhapKhoVatTuChiTiet = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauVatTu.KhoLinhId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.SoLuongNhap > x.SoLuongDaXuat)
                                .OrderBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                .ThenBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                .ToListAsync();

                            var chiTietXuat = yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                .OrderByDescending(x =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung
                                        ? x.NhapKhoVatTuChiTiet.HanSuDung
                                        : x.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien)
                                .ThenByDescending(x =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung
                                        ? x.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien
                                        : x.NhapKhoVatTuChiTiet.HanSuDung);
                            var soLuongHienTai = chiTietXuat.Sum(x => x.SoLuongXuat);

                            // trường hợp tăng số lượng
                            if (soLuongHienTai < ghiNhanVo.SoLuongCapNhat)
                            {
                                var soLuongTang = ghiNhanVo.SoLuongCapNhat.Value - soLuongHienTai;
                                foreach (var thongTinXuat in chiTietXuat)
                                {
                                    if (soLuongTang > 0)
                                    {
                                        var soLuongTon = thongTinXuat.NhapKhoVatTuChiTiet.SoLuongNhap -
                                                         thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat;
                                        if (soLuongTon > 0)
                                        {
                                            if (soLuongTon <= soLuongTang)
                                            {
                                                thongTinXuat.SoLuongXuat +=
                                                    (thongTinXuat.NhapKhoVatTuChiTiet.SoLuongNhap -
                                                     thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat);
                                                thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat =
                                                    thongTinXuat.NhapKhoVatTuChiTiet.SoLuongNhap;

                                                var nhapChiTiet = lstNhapKhoVatTuChiTiet.First(x =>
                                                    x.Id == thongTinXuat.NhapKhoVatTuChiTietId);
                                                nhapChiTiet.SoLuongDaXuat =
                                                    thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat;

                                                soLuongTang -= soLuongTon;
                                            }
                                            else
                                            {
                                                thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat += soLuongTang;
                                                thongTinXuat.SoLuongXuat += soLuongTang;

                                                var nhapChiTiet = lstNhapKhoVatTuChiTiet.First(x =>
                                                    x.Id == thongTinXuat.NhapKhoVatTuChiTietId);
                                                nhapChiTiet.SoLuongDaXuat += soLuongTang;

                                                soLuongTang = 0;
                                            }

                                            yeuCauVatTu.SoLuong = yeuCauVatTu.XuatKhoVatTuChiTiet
                                                .XuatKhoVatTuChiTietViTris.Where(p => p.WillDelete != true)
                                                .Sum(x => x.SoLuongXuat);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                // nếu vẫn còn dư số lượng, xử lý thêm mới YeuCauDuocPham
                                // kiếm tra nếu tồn dược phẩm ko đủ, thông báo
                                if (soLuongTang > 0)
                                {
                                    //var lstNhapKhoVatTuChiTiet = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                                    //    .Where(x => x.NhapKhoVatTu.KhoId == yeuCauVatTu.KhoLinhId && x.NhapKhoVatTu.DaHet != true)
                                    //    .OrderBy(x => x.NgayNhapVaoBenhVien)
                                    //    .ToListAsync();
                                    if ((lstNhapKhoVatTuChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) ||
                                        (lstNhapKhoVatTuChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                                         soLuongTang))
                                    {
                                        throw new Exception(
                                            _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                                    }

                                    bool flagTaoMoi = false;
                                    bool flagChangeTonNhapChiTiet = false;
                                    foreach (var item in lstNhapKhoVatTuChiTiet)
                                    {
                                        if (soLuongTang > 0)
                                        {
                                            if (yeuCauVatTu.DonGiaNhap == item.DonGiaNhap &&
                                                yeuCauVatTu.VAT == item.VAT &&
                                                yeuCauVatTu.TiLeTheoThapGia == item.TiLeTheoThapGia)
                                            {
                                                flagChangeTonNhapChiTiet = true;
                                                var newXuatViTri = new XuatKhoVatTuChiTietViTri()
                                                {
                                                    NhapKhoVatTuChiTietId = item.Id
                                                };

                                                var slTon = item.SoLuongNhap - item.SoLuongDaXuat;
                                                if (slTon >= soLuongTang)
                                                {
                                                    newXuatViTri.SoLuongXuat = soLuongTang;
                                                    item.SoLuongDaXuat += soLuongTang;
                                                    soLuongTang = 0;
                                                }
                                                else
                                                {
                                                    newXuatViTri.SoLuongXuat = item.SoLuongNhap - item.SoLuongDaXuat;
                                                    soLuongTang -= slTon;
                                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                                }

                                                yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(
                                                    newXuatViTri);
                                                yeuCauVatTu.SoLuong = yeuCauVatTu.XuatKhoVatTuChiTiet
                                                    .XuatKhoVatTuChiTietViTris.Where(p => p.WillDelete != true)
                                                    .Sum(x => x.SoLuongXuat);
                                            }
                                            else
                                            {
                                                // gọi function tạo mới yêu cầu vật tư
                                                flagTaoMoi = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }

                                        yeuCauVatTu.SoLuong = yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                            .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                                    }

                                    if (soLuongTang > 0 && flagTaoMoi)
                                    {
                                        ghiNhanVo.SoLuong = soLuongTang;
                                        ghiNhanVo.LaDuocPhamBHYT = yeuCauVatTu.LaVatTuBHYT;
                                        ghiNhanVo.KhoId = yeuCauVatTu.KhoLinhId;
                                        ghiNhanVo.NhapKhoVatTuChiTiets = lstNhapKhoVatTuChiTiet;
                                        ghiNhanVo.TinhPhi = yeuCauVatTu.KhongTinhPhi == null
                                            ? true
                                            : !yeuCauVatTu.KhongTinhPhi;

                                        //long dichVuId = 0;
                                        //string loaiDichVu = "";
                                        //int loaiDichVuId = 0;

                                        //if (yeuCauVatTu.YeuCauKhamBenhId != null)
                                        //{
                                        //    dichVuId = yeuCauVatTu.YeuCauKhamBenhId.Value;
                                        //    loaiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                                        //}
                                        //else if (yeuCauVatTu.YeuCauDichVuKyThuatId != null)
                                        //{
                                        //    dichVuId = yeuCauVatTu.YeuCauDichVuKyThuatId.Value;
                                        //    loaiDichVu = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                        //}

                                        //YCDVKT
                                        //dichVuId = yeuCauVatTu.YeuCauDichVuKyThuatId.Value;
                                        //loaiDichVu = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                        //loaiDichVuId = (int)EnumNhomGoiDichVu.DichVuKyThuat;

                                        ghiNhanVo.DichVuChiDinhId = ghiNhanVo.YeuCauDichVuKyThuatId;
                                        ghiNhanVo.DichVuGhiNhanId = "{" + string.Format(templateKeyId,
                                                                        yeuCauVatTu.VatTuBenhVienId,
                                                                        (int)EnumNhomGoiDichVu.VatTuTieuHao) +
                                                                    "}"; // dịch vụ dược phẩm/vật tư

                                        await XuLyThemGhiNhanVatTuBenhVienAsync(ghiNhanVo, yeuCauTiepNhanChiTiet);
                                    }
                                    else
                                    {
                                        if (flagChangeTonNhapChiTiet)
                                        {
                                            ghiNhanVo.NhapKhoVatTuChiTiets = lstNhapKhoVatTuChiTiet;
                                        }
                                    }
                                }
                                else
                                {
                                    yeuCauVatTu.SoLuong = yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                        .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                                }
                            }
                            // trường hợp giảm số lượng
                            else if (soLuongHienTai > ghiNhanVo.SoLuongCapNhat)
                            {
                                var soLuongGiam = soLuongHienTai - ghiNhanVo.SoLuongCapNhat.Value;
                                foreach (var thongTinXuat in chiTietXuat)
                                {
                                    if (soLuongGiam > 0)
                                    {
                                        if (thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat > soLuongGiam)
                                        {
                                            thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat -= soLuongGiam;
                                            thongTinXuat.SoLuongXuat -= soLuongGiam;
                                            soLuongGiam = 0;
                                        }
                                        else
                                        {
                                            soLuongGiam -= thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat;
                                            thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat = 0;
                                            thongTinXuat.WillDelete = true;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                yeuCauVatTu.SoLuong = yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                    .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                            }
                        }
                        else
                        {
                            //get thông tin nhập chi tiết
                            var lstNhapKhoVatTuChiTiet = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauVatTu.KhoLinhId
                                            && x.VatTuBenhVienId == yeuCauVatTu.VatTuBenhVienId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.LaVatTuBHYT == yeuCauVatTu.LaVatTuBHYT
                                            && x.SoLuongNhap > x.SoLuongDaXuat)
                                .OrderBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                .ThenBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                .ToListAsync();

                            if (lstNhapKhoVatTuChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                                ghiNhanVo.SoLuongCapNhat)
                            {
                                throw new Exception(
                                    _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                            }

                            yeuCauVatTu.SoLuong = ghiNhanVo.SoLuongCapNhat ?? 0;
                        }
                    }

                    if (ghiNhanVo.IsCapNhatTinhPhi)
                    {
                        yeuCauVatTu.KhongTinhPhi = !ghiNhanVo.TinhPhi;
                    }
                }
                else
                {
                    throw new Exception(_localizationService.GetResource("XoaGhiNhanVTTHThuoc.NotExists"));
                }
            }
        }

        public void XuLyXuatYeuCauGhiNhanVTTHThuocChoNhungYeuCauQuenNhanXuat(ChiDinhGhiNhanVatTuThuocPTTTVo yeuCauVo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            bool coPhieuXuat = false;

            var lstAllGhiNhanDuocPham = _yeuCauDuocPhamBenhVienRepository.Table.Include(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(y => y.XuatKhoDuocPham)
                                                                                     .Include(x => x.YeuCauTiepNhan)
                                                                                     .Where(x => x.KhoLinh != null &&
                                                                                                 x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                                                                                                 x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                                                                                                 //x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null &&
                                                                                                 x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                                                                                 x.YeuCauDichVuKyThuatId == yeuCauVo.YeuCauDichVuKyThuatId)
                                                                                     .ToList();

            var lstPhieuXuatDaXuat = lstAllGhiNhanDuocPham.Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null)
                                                          .Select(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham)
                                                          .Distinct()
                                                          .ToList();

            var lstGhiNhanDuocPhamChuaXuat = lstAllGhiNhanDuocPham.Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null).ToList();

            if (lstGhiNhanDuocPhamChuaXuat.Any())
            {
                var lstPhieuXuatDuocPham = new List<Core.Domain.Entities.XuatKhos.XuatKhoDuocPham>();
                var lstKhoId = lstGhiNhanDuocPhamChuaXuat.Where(x => x.KhoLinhId != null).Select(x => x.KhoLinhId.Value).Distinct().ToList();

                var phieuXuatTemp = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham()
                {
                    //KhoXuatId = khoId,
                    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                    LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                    NguoiXuatId = currentUserId,
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                    NgayXuat = DateTime.Now
                };

                foreach (var khoId in lstKhoId)
                {
                    var phieuXuatNew = phieuXuatTemp.Clone();
                    phieuXuatNew.KhoXuatId = khoId;

                    var lstYeuCauTheoKho = lstGhiNhanDuocPhamChuaXuat.Where(x => x.KhoLinhId == khoId && x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null).ToList();
                    if (lstYeuCauTheoKho.Any())
                    {

                        foreach (var yeuCau in lstYeuCauTheoKho)
                        {
                            coPhieuXuat = true;
                            // lấy thông tin tên người bệnh
                            var tenBenhNhan = yeuCau.YeuCauTiepNhan.HoTen;
                            // xử lý kiểm tra gộp phiếu xuất
                            var phieuXuatDaXuat = lstPhieuXuatDaXuat.FirstOrDefault(x => x.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatChoBenhNhan &&
                                                                                         x.TenNguoiNhan != null &&
                                                                                         x.TenNguoiNhan.Trim().ToLower() == tenBenhNhan.Trim().ToLower() &&
                                                                                         x.NguoiXuatId == currentUserId &&
                                                                                         x.KhoXuatId == khoId);
                            if (phieuXuatDaXuat != null)
                            {
                                phieuXuatNew = phieuXuatDaXuat;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(phieuXuatNew.TenNguoiNhan))
                                {
                                    phieuXuatNew.TenNguoiNhan = tenBenhNhan;
                                }
                                else
                                {
                                    if (phieuXuatNew.TenNguoiNhan.Trim().ToLower() != tenBenhNhan.Trim().ToLower())
                                    {
                                        phieuXuatNew = phieuXuatTemp.Clone();
                                        phieuXuatNew.KhoXuatId = khoId;
                                    }
                                }
                            }

                            yeuCau.XuatKhoDuocPhamChiTiet.NgayXuat = DateTime.Now;
                            yeuCau.TrangThai = EnumYeuCauDuocPhamBenhVien.DaThucHien;
                            yeuCau.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = phieuXuatNew;
                        }

                    }
                }

                if (coPhieuXuat)
                {
                    _yeuCauDuocPhamBenhVienRepository.Update(lstAllGhiNhanDuocPham);
                }
            }

            var lstAllGhiNhanVatTu = _yeuCauVatTuBenhVienRepository.Table
                .Include(x => x.XuatKhoVatTuChiTiet).ThenInclude(y => y.XuatKhoVatTu)
                .Include(x => x.YeuCauTiepNhan)
                .Where(x => x.KhoLinh != null &&
                            x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                            x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
                            //x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null &&
                            x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                            x.YeuCauDichVuKyThuatId == yeuCauVo.YeuCauDichVuKyThuatId)
                .ToList();

            var lstPhieuXuatVatTuDaXuat = lstAllGhiNhanVatTu.Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null)
                .Select(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTu)
                .Distinct()
                .ToList();

            var lstGhiNhanVatTuChuaXuat = lstAllGhiNhanVatTu.Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null).ToList();
            if (lstGhiNhanVatTuChuaXuat.Any())
            {
                var lstPhieuXuatVatTu = new List<XuatKhoVatTu>();
                var lstKhoId = lstGhiNhanVatTuChuaXuat.Where(x => x.KhoLinhId != null).Select(x => x.KhoLinhId.Value).Distinct().ToList();

                var phieuXuatTemp = new XuatKhoVatTu()
                {
                    //KhoXuatId = khoId,
                    LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                    LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                    NguoiXuatId = currentUserId,
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                    NgayXuat = DateTime.Now
                };

                foreach (var khoId in lstKhoId)
                {
                    var phieuXuatNew = phieuXuatTemp.Clone();
                    phieuXuatNew.KhoXuatId = khoId;

                    var lstYeuCauTheoKho = lstGhiNhanVatTuChuaXuat.Where(x => x.KhoLinhId == khoId && x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null).ToList();
                    if (lstYeuCauTheoKho.Any())
                    {
                        coPhieuXuat = true;

                        foreach (var yeuCau in lstYeuCauTheoKho)
                        {
                            // lấy thông tin tên người bệnh
                            var tenBenhNhan = yeuCau.YeuCauTiepNhan.HoTen;
                            var phieuXuatDaXuat = lstPhieuXuatVatTuDaXuat.FirstOrDefault(x =>
                                x.LoaiXuatKho == EnumLoaiXuatKho.XuatChoBenhNhan
                                && x.TenNguoiNhan != null
                                && x.TenNguoiNhan.Trim().ToLower() == tenBenhNhan.Trim().ToLower()
                                && x.NguoiXuatId == currentUserId
                                && x.KhoXuatId == khoId);
                            if (phieuXuatDaXuat != null)
                            {
                                phieuXuatNew = phieuXuatDaXuat;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(phieuXuatNew.TenNguoiNhan))
                                {
                                    phieuXuatNew.TenNguoiNhan = tenBenhNhan;
                                }
                                else
                                {
                                    if (phieuXuatNew.TenNguoiNhan.Trim().ToLower() != tenBenhNhan.Trim().ToLower())
                                    {
                                        phieuXuatNew = phieuXuatTemp.Clone();
                                        phieuXuatNew.KhoXuatId = khoId;
                                    }
                                }
                            }

                            yeuCau.XuatKhoVatTuChiTiet.NgayXuat = DateTime.Now;
                            yeuCau.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                            yeuCau.XuatKhoVatTuChiTiet.XuatKhoVatTu = phieuXuatNew;
                        }
                    }
                }

                if (coPhieuXuat)
                {
                    _yeuCauVatTuBenhVienRepository.Update(lstAllGhiNhanVatTu);
                }
            }
        }

        public async Task<ThongTinHoanTraVatTuThuocPTTTVo> GetThongTinHoanTraVatTuThuocPTTT(HoanTraVatTuThuocVo hoanTraVatTuThuocVo)
        {
            //Tham khảo hoàn trả nội trú
            var kho = _khoRepository.TableNoTracking.Where(p => p.Id == hoanTraVatTuThuocVo.KhoId).FirstOrDefault();

            var lstYeuCauCanHoanTraId = hoanTraVatTuThuocVo.Id.Split(";");
            var yeuCauGhiNhanObjs = new List<DichVuGhiNhanVo>();
            foreach (var strId in lstYeuCauCanHoanTraId)
            {
                yeuCauGhiNhanObjs.Add(JsonConvert.DeserializeObject<DichVuGhiNhanVo>(strId));
            }

            var nhanVienDangNhap = _nhanVienRepository.TableNoTracking.Where(p => p.Id == _userAgentHelper.GetCurrentUserId()).Select(p => p.User.HoTen).FirstOrDefault();
            var yeuCaIds = yeuCauGhiNhanObjs.Select(o => o.Id).ToList();
            var nhanVienYeuCau = new Core.Domain.Entities.NhanViens.NhanVien();
            //var thongTin = new ThongTinHoanTraVatTuThuocPTTTVo();
            var lstThongTinHoanTra = new List<ThongTinHoanTraVatTuThuocPTTTVo>();

            //if (hoanTraVatTuThuocVo.NhomYeuCauId == EnumNhomGoiDichVu.DuocPham)
            if (yeuCauGhiNhanObjs[0].NhomId == (int)EnumNhomGoiDichVu.DuocPham)
            {
                nhanVienYeuCau = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking.Where(p => p.YeuCauDuocPhamBenhVienId == hoanTraVatTuThuocVo.YeuCauBenhVienId)
                                                                                              .Select(p => p.NhanVienYeuCau)
                                                                                              .Include(p => p.User)
                                                                                              .FirstOrDefault();

                //thongTin = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(p => p.Id == hoanTraVatTuThuocVo.YeuCauBenhVienId &&
                //                                                                           p.LaDuocPhamBHYT == hoanTraVatTuThuocVo.DuocHuongBaoHiem &&
                //                                                                           p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                //                                                                           p.KhoLinh.LoaiKho == kho.LoaiKho)
                lstThongTinHoanTra = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(p => yeuCaIds.Contains(p.Id) &&
                                                                                                        p.LaDuocPhamBHYT == hoanTraVatTuThuocVo.DuocHuongBaoHiem &&
                                                                                                        p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                                                                                                        p.KhoLinh.LoaiKho == kho.LoaiKho)
                                                                                            .Select(s => new ThongTinHoanTraVatTuThuocPTTTVo
                                                                                            {
                                                                                                Id = s.Id,
                                                                                                VatTuThuocBenhVienId = s.DuocPhamBenhVienId,
                                                                                                Ten = s.Ten,
                                                                                                TenKho = kho.Ten,
                                                                                                NhanVienYeuCauId = nhanVienYeuCau != null ? nhanVienYeuCau.Id : _userAgentHelper.GetCurrentUserId(),
                                                                                                TenNhanVienYeuCau = nhanVienYeuCau != null && nhanVienYeuCau.User != null && !string.IsNullOrEmpty(nhanVienYeuCau.User.HoTen) ? nhanVienYeuCau.User.HoTen : nhanVienDangNhap,
                                                                                                YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                                                                                NgayYeuCau = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu ? (DateTime?)null : s.YeuCauTraDuocPhamTuBenhNhanChiTiets.FirstOrDefault(c => c.YeuCauTraDuocPhamTuBenhNhanId == null).NgayYeuCau,

                                                                                                DonGiaNhap = s.DonGiaNhap,
                                                                                                DonGia = s.DonGiaBan,
                                                                                                SoLuong = s.SoLuong,
                                                                                                //SoLuongDisplay = !string.IsNullOrEmpty(s.SoLuong.ApplyVietnameseFloatNumber()) ? (s.SoLuong < 1 ? $"0{s.SoLuong.ApplyVietnameseFloatNumber()}" : s.SoLuong.ApplyVietnameseFloatNumber()) : "0",
                                                                                                SoLuongDaTra = s.SoLuongDaTra.GetValueOrDefault(),
                                                                                                TiLeTheoThapGia = s.TiLeTheoThapGia,
                                                                                                VAT = s.VAT,
                                                                                                KhongTinhPhi = s.KhongTinhPhi,
                                                                                                SoLuongTra = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ?
                                                                                                    s.YeuCauTraDuocPhamTuBenhNhanChiTiets.Where(x => x.LaDuocPhamBHYT == s.LaDuocPhamBHYT && x.YeuCauTraDuocPhamTuBenhNhanId == null)
                                                                                                                                        .Select(z => z.SoLuongTra)
                                                                                                                                        .FirstOrDefault() :
                                                                                                    0,
                                                                                                NhomYeuCauId = EnumNhomGoiDichVu.DuocPham
                                                                                            }).ToListAsync();
            }
            else
            {
                nhanVienYeuCau = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking.Where(p => p.YeuCauVatTuBenhVienId == hoanTraVatTuThuocVo.YeuCauBenhVienId)
                                                                                           .Select(p => p.NhanVienYeuCau)
                                                                                           .Include(p => p.User)
                                                                                           .FirstOrDefault();

                //thongTin = await _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(p => p.Id == hoanTraVatTuThuocVo.YeuCauBenhVienId &&
                //                                                                           p.LaVatTuBHYT == hoanTraVatTuThuocVo.DuocHuongBaoHiem &&
                //                                                                           p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
                //                                                                           p.KhoLinh.LoaiKho == kho.LoaiKho)
                lstThongTinHoanTra = await _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(p => yeuCaIds.Contains(p.Id) &&
                                                                                                     p.LaVatTuBHYT == hoanTraVatTuThuocVo.DuocHuongBaoHiem &&
                                                                                                     p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
                                                                                                     p.KhoLinh.LoaiKho == kho.LoaiKho)
                                                                                        .Select(s => new ThongTinHoanTraVatTuThuocPTTTVo
                                                                                        {
                                                                                            Id = s.Id,
                                                                                            VatTuThuocBenhVienId = s.VatTuBenhVienId,
                                                                                            Ten = s.Ten,
                                                                                            TenKho = kho.Ten,
                                                                                            NhanVienYeuCauId = nhanVienYeuCau != null ? nhanVienYeuCau.Id : _userAgentHelper.GetCurrentUserId(),
                                                                                            TenNhanVienYeuCau = nhanVienYeuCau != null && nhanVienYeuCau.User != null && !string.IsNullOrEmpty(nhanVienYeuCau.User.HoTen) ? nhanVienYeuCau.User.HoTen : nhanVienDangNhap,
                                                                                            NgayYeuCau = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu ? (DateTime?)null : s.YeuCauTraVatTuTuBenhNhanChiTiets.FirstOrDefault(c => c.YeuCauTraVatTuTuBenhNhanId == null).NgayYeuCau,

                                                                                            DonGiaNhap = s.DonGiaNhap,
                                                                                            DonGia = s.DonGiaBan,
                                                                                            SoLuong = s.SoLuong,
                                                                                            //SoLuongDisplay = !string.IsNullOrEmpty(s.SoLuong.ApplyVietnameseFloatNumber()) ? (s.SoLuong < 1 ? $"0{s.SoLuong.ApplyVietnameseFloatNumber()}" : s.SoLuong.ApplyVietnameseFloatNumber()) : "0",
                                                                                            SoLuongDaTra = s.SoLuongDaTra.GetValueOrDefault(),
                                                                                            TiLeTheoThapGia = s.TiLeTheoThapGia,
                                                                                            VAT = s.VAT,
                                                                                            KhongTinhPhi = s.KhongTinhPhi,
                                                                                            SoLuongTra = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ?
                                                                                                s.YeuCauTraVatTuTuBenhNhanChiTiets.Where(x => x.LaVatTuBHYT == s.LaVatTuBHYT && x.YeuCauTraVatTuTuBenhNhanId == null)
                                                                                                                                    .Select(z => z.SoLuongTra)
                                                                                                                                    .FirstOrDefault() :
                                                                                                0,
                                                                                            NhomYeuCauId = EnumNhomGoiDichVu.VatTuTieuHao
                                                                                        }).ToListAsync();
            }

            var thongTin = new ThongTinHoanTraVatTuThuocPTTTVo
            {
                VatTuThuocBenhVienId = lstThongTinHoanTra.First().VatTuThuocBenhVienId,
                Ten = lstThongTinHoanTra.First().Ten,
                TenKho = lstThongTinHoanTra.First().TenKho,
                NhanVienYeuCauId = lstThongTinHoanTra.First().NhanVienYeuCauId,
                TenNhanVienYeuCau = lstThongTinHoanTra.First().TenNhanVienYeuCau,
                NgayYeuCau = lstThongTinHoanTra.First().NgayYeuCau,
                YeuCauDuocPhamVatTuBenhViens = lstThongTinHoanTra.Select(p => new ThongTinHoanTraVatTuThuocChiTietPTTTVo
                {
                    Id = p.Id,
                    KhongTinhPhi = p.KhongTinhPhi,
                    VatTuThuocBenhVienId = p.VatTuThuocBenhVienId,
                    SoLuong = p.SoLuong,
                    DonGiaNhap = p.DonGiaNhap,
                    DonGia = p.DonGia,
                    TiLeTheoThapGia = p.TiLeTheoThapGia,
                    VAT = p.VAT,
                    //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                    SoLuongDaTra = p.SoLuongDaTra - p.SoLuongTra.GetValueOrDefault(),
                    SoLuongTra = p.SoLuongTra,
                    NhomYeuCauId = p.NhomYeuCauId
                }).ToList()
            };

            return thongTin;
        }

        public async Task<GridDataSource> GetDataForGridDanhSachVatTuThuocHoanTraPTTT(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }


            var thongTinDichVuGhiNhan = JsonConvert.DeserializeObject<YeuCauTraVatThuThuocTuBenhNhanSearchVo>(queryInfo.AdditionalSearchString);

            var lstYeuCauCanHoanTraId = thongTinDichVuGhiNhan.YeuCauDuocPhamVatTuBenhVienId.Split(";");
            var yeuCauGhiNhanObjs = new List<DichVuGhiNhanVo>();
            foreach (var strId in lstYeuCauCanHoanTraId)
            {
                yeuCauGhiNhanObjs.Add(JsonConvert.DeserializeObject<DichVuGhiNhanVo>(strId));
            }

            //var queryObj = queryInfo.AdditionalSearchString.Split(";");
            //var yeuCauVatTuDuocPhamBenhVienId = long.Parse(queryObj[0]);
            //var duocHuongBHYT = bool.Parse(queryObj[1]);
            //var nhomYeuCauId = (EnumNhomGoiDichVu)int.Parse(queryObj[2]);
            var yeuCaIds = yeuCauGhiNhanObjs.Select(o => o.Id).ToList();
            IQueryable<YeuCauTraVatTuThuocTuBenhNhanChiTietGridVo> query;

            if (thongTinDichVuGhiNhan.NhomYeuCauId == EnumNhomGoiDichVu.DuocPham)
            {
                //query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking.Where(p => p.YeuCauDuocPhamBenhVienId == yeuCauVatTuDuocPhamBenhVienId &&
                query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking.Where(p => yeuCaIds.Contains(p.YeuCauDuocPhamBenhVienId) &&
                                                                                                 p.LaDuocPhamBHYT == thongTinDichVuGhiNhan.DuocHuongBaoHiem &&
                                                                                                 p.YeuCauTraDuocPhamTuBenhNhanId != null)
                                                                                     .Select(s => new YeuCauTraVatTuThuocTuBenhNhanChiTietGridVo
                                                                                     {
                                                                                         Id = s.Id,
                                                                                         NgayTra = s.NgayYeuCau,
                                                                                         SoLuongTra = ((double?)s.SoLuongTra).FloatToStringFraction(),
                                                                                         NhanVienTra = s.NhanVienYeuCau.User.HoTen,
                                                                                         //SoPhieu = s.YeuCauDuocPhamBenhVien.SoP,
                                                                                         DuocDuyet = s.YeuCauTraDuocPhamTuBenhNhan.DuocDuyet,
                                                                                         NgayTao = s.CreatedOn,
                                                                                     });
            }
            else
            {
                //query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking.Where(p => p.YeuCauVatTuBenhVienId == yeuCauVatTuDuocPhamBenhVienId &&
                query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking.Where(p => yeuCaIds.Contains(p.YeuCauVatTuBenhVienId) &&
                                                                                              p.LaVatTuBHYT == thongTinDichVuGhiNhan.DuocHuongBaoHiem &&
                                                                                              p.YeuCauTraVatTuTuBenhNhanId != null)
                                                                                  .Select(s => new YeuCauTraVatTuThuocTuBenhNhanChiTietGridVo
                                                                                  {
                                                                                      Id = s.Id,
                                                                                      NgayTra = s.NgayYeuCau,
                                                                                      SoLuongTra = ((double?)s.SoLuongTra).FloatToStringFraction(),
                                                                                      NhanVienTra = s.NhanVienYeuCau.User.HoTen,
                                                                                      SoPhieu = s.YeuCauTraVatTuTuBenhNhan.SoPhieu,
                                                                                      DuocDuyet = s.YeuCauTraVatTuTuBenhNhan.DuocDuyet,
                                                                                      NgayTao = s.CreatedOn,
                                                                                  });
            }

            var countTask = query.CountAsync();
            var queryTask = query.Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task XuLyHoanTraYeuCauGhiNhanVTTHThuocAsync(YeuCauHoanTraVatTuThuocPTTTVo yeuCauHoanTraVatTuThuoc)
        {
            foreach (var item in yeuCauHoanTraVatTuThuoc.YeuCauDuocPhamVatTuBenhViens)
            {
                if (item.NhomYeuCauId == EnumNhomGoiDichVu.DuocPham)
                {
                    var yeuCau = await _yeuCauDuocPhamBenhVienRepository.GetByIdAsync(item.Id, s => s
                                            .Include(p => p.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                                            .Include(p => p.NoiTruChiDinhDuocPham)
                                            .Include(p => p.XuatKhoDuocPhamChiTiet).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTris).ThenInclude(p => p.NhapKhoDuocPhamChiTiet)
                                            .Include(p => p.XuatKhoDuocPhamChiTiet).ThenInclude(p => p.XuatKhoDuocPham));
                    if (yeuCau == null)
                    {
                        throw new Exception(_localizationService.GetResource("PhieuDieuTri.VatTu.NotExists"));
                    }
                    var ycChiTiet = yeuCau.YeuCauTraDuocPhamTuBenhNhanChiTiets.FirstOrDefault(p => p.YeuCauTraDuocPhamTuBenhNhanId == null);
                    var khoXuatId = yeuCau.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId;
                    var loaiKho = _khoRepository.TableNoTracking.Where(p => p.Id == khoXuatId).First().LoaiKho;
                    var traVeTuTruc = false;
                    if (loaiKho == EnumLoaiKhoDuocPham.KhoLe)
                    {
                        traVeTuTruc = true;
                        ycChiTiet = null;
                    }
                    if (ycChiTiet == null)
                    {
                        if (item.SoLuongTra > 0)
                        {
                            if (!traVeTuTruc)
                            {
                                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                                if (yeuCau.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && yeuCau.KhongTinhPhi != true)
                                {
                                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                                }
                                if (yeuCau.SoLuongDaTra == null)
                                {
                                    yeuCau.SoLuongDaTra = 0;
                                }
                                yeuCau.SoLuongDaTra += item.SoLuongTra;
                                yeuCau.SoLuong -= item.SoLuongTra.Value;
                                if(yeuCau.NoiTruChiDinhDuocPham != null)
                                {
                                    yeuCau.NoiTruChiDinhDuocPham.SoLuong -= item.SoLuongTra.Value;
                                }
                                if (yeuCau.SoLuong < 0)
                                {
                                    throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                                }
                                //end update BVHD-3411
                                var ycHoanTraDuocPhamNew = new YeuCauTraDuocPhamTuBenhNhanChiTiet
                                {
                                    DuocPhamBenhVienId = yeuCauHoanTraVatTuThuoc.VatTuThuocBenhVienId,
                                    LaDuocPhamBHYT = yeuCauHoanTraVatTuThuoc.DuocHuongBaoHiem,
                                    SoLuongTra = item.SoLuongTra ?? 0,
                                    KhoTraId = khoXuatId,
                                    TraVeTuTruc = traVeTuTruc,
                                    NgayYeuCau = yeuCauHoanTraVatTuThuoc.NgayYeuCau,
                                    NhanVienYeuCauId = yeuCauHoanTraVatTuThuoc.NhanVienYeuCauId
                                };

                                yeuCau.YeuCauTraDuocPhamTuBenhNhanChiTiets.Add(ycHoanTraDuocPhamNew);
                            }
                            else
                            {
                                var xuatKhoDuocPhamViTris = yeuCau.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.ToList();
                                var xuatKhoDuocPhamViTriHoanTras = new List<XuatKhoDuocPhamChiTietViTri>();
                                if (yeuCau.SoLuongDaTra == null)
                                {
                                    yeuCau.SoLuongDaTra = 0;
                                }
                                //yeuCau.SoLuong -= item.SoLuongTra.Value;
                                //yeuCau.SoLuongDaTra += item.SoLuongTra;
                                yeuCau.SoLuong = Math.Round(yeuCau.SoLuong - item.SoLuongTra.Value, 2);
                                yeuCau.SoLuongDaTra = Math.Round((double)yeuCau.SoLuongDaTra + (double)item.SoLuongTra, 2);
                                if (yeuCau.XuatKhoDuocPhamChiTiet != null)
                                {
                                    foreach (var thongTinXuat in xuatKhoDuocPhamViTris)
                                    {
                                        //thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= item.SoLuongTra.Value;
                                        thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = Math.Round(thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - item.SoLuongTra.Value, 2);
                                    }
                                }
                                foreach (var xuatKho in xuatKhoDuocPhamViTris)
                                {
                                    var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                                    {
                                        XuatKhoDuocPhamChiTietId = xuatKho.XuatKhoDuocPhamChiTietId,
                                        NhapKhoDuocPhamChiTietId = xuatKho.NhapKhoDuocPhamChiTietId,
                                        SoLuongXuat = -item.SoLuongTra.Value,
                                        NgayXuat = DateTime.Now,
                                        //GhiChu = item.NoiTruChiDinhDuocPham.TrangThai.GetDescription()
                                        GhiChu = "Hoàn trả thuốc"
                                    };

                                    yeuCau.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (traVeTuTruc)
                        {
                            break;
                        }
                        //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                        if (yeuCau.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && yeuCau.KhongTinhPhi != true)
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                        }
                        var soLuongThayDoi = item.SoLuongTra.GetValueOrDefault() - ycChiTiet.SoLuongTra;
                        if (yeuCau.SoLuongDaTra == null)
                        {
                            yeuCau.SoLuongDaTra = 0;
                        }
                        yeuCau.SoLuongDaTra += soLuongThayDoi;
                        yeuCau.SoLuong -= soLuongThayDoi;
                        if (yeuCau.NoiTruChiDinhDuocPham != null)
                        {
                            yeuCau.NoiTruChiDinhDuocPham.SoLuong -= soLuongThayDoi;
                        }
                        if (yeuCau.SoLuong < 0)
                        {
                            throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                        }
                        //end update BVHD-3411
                        if (item.SoLuongTra > 0) // Cập nhật
                        {
                            ycChiTiet.NhanVienYeuCauId = yeuCauHoanTraVatTuThuoc.NhanVienYeuCauId;
                            ycChiTiet.NgayYeuCau = yeuCauHoanTraVatTuThuoc.NgayYeuCau;
                            ycChiTiet.SoLuongTra = item.SoLuongTra.GetValueOrDefault();
                        }
                        else // Xóa
                        {
                            ycChiTiet.WillDelete = true;
                        }
                    }

                    await _yeuCauDuocPhamBenhVienRepository.UpdateAsync(yeuCau);
                }
                else if (item.NhomYeuCauId == EnumNhomGoiDichVu.VatTuTieuHao)
                {
                    var yeuCau = await _yeuCauVatTuBenhVienRepository.GetByIdAsync(item.Id, s => s
                                            .Include(p => p.YeuCauTraVatTuTuBenhNhanChiTiets)
                                            .Include(p => p.XuatKhoVatTuChiTiet).ThenInclude(p => p.XuatKhoVatTuChiTietViTris).ThenInclude(p => p.NhapKhoVatTuChiTiet)
                                            .Include(p => p.XuatKhoVatTuChiTiet).ThenInclude(p => p.XuatKhoVatTu));
                    if (yeuCau == null)
                    {
                        throw new Exception(_localizationService.GetResource("PhieuDieuTri.VatTu.NotExists"));
                    }
                    var ycChiTiet = yeuCau.YeuCauTraVatTuTuBenhNhanChiTiets.FirstOrDefault(p => p.YeuCauTraVatTuTuBenhNhanId == null);
                    var khoXuatId = yeuCau.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId;
                    var loaiKho = _khoRepository.TableNoTracking.Where(p => p.Id == khoXuatId).First().LoaiKho;
                    var traVeTuTruc = false;
                    if (loaiKho == EnumLoaiKhoDuocPham.KhoLe)
                    {
                        traVeTuTruc = true;
                        ycChiTiet = null;
                    }
                    if (ycChiTiet == null)
                    {
                        if (item.SoLuongTra > 0)
                        {
                            if (!traVeTuTruc)
                            {
                                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                                if (yeuCau.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && yeuCau.KhongTinhPhi != true)
                                {
                                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                                }
                                if (yeuCau.SoLuongDaTra == null)
                                {
                                    yeuCau.SoLuongDaTra = 0;
                                }
                                yeuCau.SoLuongDaTra += item.SoLuongTra;
                                yeuCau.SoLuong -= item.SoLuongTra.Value;
                                if (yeuCau.SoLuong < 0)
                                {
                                    throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                                }
                                //end update BVHD-3411
                                var ycHoanTraVTNew = new YeuCauTraVatTuTuBenhNhanChiTiet
                                {
                                    VatTuBenhVienId = yeuCauHoanTraVatTuThuoc.VatTuThuocBenhVienId,
                                    LaVatTuBHYT = yeuCauHoanTraVatTuThuoc.DuocHuongBaoHiem,
                                    SoLuongTra = item.SoLuongTra ?? 0,
                                    KhoTraId = khoXuatId,
                                    TraVeTuTruc = traVeTuTruc,
                                    NgayYeuCau = yeuCauHoanTraVatTuThuoc.NgayYeuCau,
                                    NhanVienYeuCauId = yeuCauHoanTraVatTuThuoc.NhanVienYeuCauId
                                };
                                yeuCau.YeuCauTraVatTuTuBenhNhanChiTiets.Add(ycHoanTraVTNew);
                            }
                            else
                            {
                                var xuatKhoVtViTris = yeuCau.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.ToList();
                                var xuatKhoVatTuViTriHoanTras = new List<XuatKhoVatTuChiTietViTri>();
                                if (yeuCau.SoLuongDaTra == null)
                                {
                                    yeuCau.SoLuongDaTra = 0;
                                }
                                //yeuCau.SoLuong -= item.SoLuongTra.Value;
                                //yeuCau.SoLuongDaTra += item.SoLuongTra;
                                yeuCau.SoLuong = Math.Round(yeuCau.SoLuong - item.SoLuongTra.Value, 2);
                                yeuCau.SoLuongDaTra = Math.Round((double)yeuCau.SoLuongDaTra + (double)item.SoLuongTra, 2);
                                if (yeuCau.XuatKhoVatTuChiTiet != null)
                                {
                                    foreach (var thongTinXuat in xuatKhoVtViTris)
                                    {
                                        //thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat -= item.SoLuongTra.Value;
                                        thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat = Math.Round(thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat - item.SoLuongTra.Value, 2);
                                    }
                                }
                                foreach (var viTri in xuatKhoVtViTris)
                                {
                                    var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                                    {
                                        XuatKhoVatTuChiTietId = viTri.XuatKhoVatTuChiTietId,
                                        NhapKhoVatTuChiTietId = viTri.NhapKhoVatTuChiTietId,
                                        SoLuongXuat = -item.SoLuongTra.Value,
                                        NgayXuat = DateTime.Now,
                                        GhiChu = "Hoàn trả vật tư"
                                    };
                                    yeuCau.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (traVeTuTruc)
                        {
                            break;
                        }
                        //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                        if (yeuCau.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && yeuCau.KhongTinhPhi != true)
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                        }
                        var soLuongThayDoi = item.SoLuongTra.GetValueOrDefault() - ycChiTiet.SoLuongTra;
                        if (yeuCau.SoLuongDaTra == null)
                        {
                            yeuCau.SoLuongDaTra = 0;
                        }
                        yeuCau.SoLuongDaTra += soLuongThayDoi;
                        yeuCau.SoLuong -= soLuongThayDoi;
                        if (yeuCau.SoLuong < 0)
                        {
                            throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                        }
                        //end update BVHD-3411
                        if (item.SoLuongTra > 0) // Cập nhật
                        {
                            ycChiTiet.NhanVienYeuCauId = yeuCauHoanTraVatTuThuoc.NhanVienYeuCauId;
                            ycChiTiet.NgayYeuCau = yeuCauHoanTraVatTuThuoc.NgayYeuCau;
                            ycChiTiet.SoLuongTra = item.SoLuongTra.GetValueOrDefault();
                        }
                        else // Xóa
                        {
                            ycChiTiet.WillDelete = true;
                        }
                    }

                    await _yeuCauVatTuBenhVienRepository.UpdateAsync(yeuCau);
                }
            }
        }

        public async Task<List<KhoSapXepUuTienLookupItemVo>> GetListKhoGhiNhanPTTTAsync(DropDownListRequestModel queryInfo)
        {
            //BVHD-3336: Chỉ hiển thị các tủ của tài khoản đăng nhập được gán và tủ thuốc khoa đang đăng nhập
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var phongLamViecHienTai = await _phongBenhVienRepository.TableNoTracking.FirstAsync(x => x.Id == phongHienTaiId);

            var lstKhoUuTien = await _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                                                                               (p.KhoaPhongId == phongLamViecHienTai.KhoaPhongId ||
                                                                               p.KhoNhanVienQuanLys.Any(o => o.NhanVienId == currentUserId)))
                                                                   .ApplyLike(queryInfo.Query, x => x.Ten)
                                                                   .Select(item => new KhoSapXepUuTienLookupItemVo()
                                                                   {
                                                                       KeyId = item.Id,
                                                                       DisplayName = item.Ten,
                                                                       LoaiKho = item.LoaiKho
                                                                   })
                                                                   .Take(queryInfo.Take)
                                                                   .ToListAsync();

            return lstKhoUuTien;
        }

        public async Task<GridDataSource> GetGridDataGoiDuocPhamVatTuTrongDichVu(QueryInfo queryInfo)
        {
            var thongTinGhiNhan = new GhiNhanGoiDuocPhamVatTuPTTTVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                thongTinGhiNhan = JsonConvert.DeserializeObject<GhiNhanGoiDuocPhamVatTuPTTTVo>(queryInfo.AdditionalSearchString);
            }

            if (thongTinGhiNhan.YeuCauDichVuKyThuatId <= 0)
            {
                throw new Exception("PhauThuatThuThuat.ChiDinhGoiDuocPhamVatTu.YeuCauDichVuKyThuatId.Required");
            }

            if (thongTinGhiNhan.KhoLuaChonId < 0)
            {
                throw new Exception("PhauThuatThuThuat.ChiDinhGoiDuocPhamVatTu.KhoLuaChonId.Required");
            }

            var yeuCauDichVuKyThuat = await BaseRepository.GetByIdAsync(thongTinGhiNhan.YeuCauDichVuKyThuatId, s => s.Include(z => z.YeuCauTiepNhan));
            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var dichVuKyThuatBenhVienDinhMucDuocPhamVatTus = _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.Id == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId)
                                                                                        .SelectMany(p => p.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus)
                                                                                        .Include(dv => dv.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dpbv => dpbv.DonViTinh)
                                                                                        .Include(dv => dv.VatTuBenhVien).ThenInclude(vtbv => vtbv.VatTus)
                                                                                        .ToList();

            var duocPhamBenhViens = dichVuKyThuatBenhVienDinhMucDuocPhamVatTus.Where(p => p.DuocPhamBenhVienId != null).ToList();
            var vatTuBenhViens = dichVuKyThuatBenhVienDinhMucDuocPhamVatTus.Where(p => p.VatTuBenhVienId != null).ToList();

            var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
             .Where(o => o.HanSuDung >= DateTime.Now
                      && o.SoLuongNhap > o.SoLuongDaXuat);

            var nhapKhoVTChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
            .Where(o => o.HanSuDung >= DateTime.Now
                     && o.SoLuongNhap > o.SoLuongDaXuat);

            var laDuocPhamVatTuBHYT = true;
            if (yeuCauDichVuKyThuat.YeuCauTiepNhan.CoBHYT != true)
            {
                laDuocPhamVatTuBHYT = false;
            }
            var chiDinhGoiDuocPhamVatTuGridVos = _duocPhamBenhVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(p => new ChiDinhGoiDuocPhamVatTuGridVo { }).ToList();

            if (thongTinGhiNhan.KhoLuaChonId == 0)
            {
                var lstKhoQuanLy = await _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
                                                                                && p.KhoNhanVienQuanLys.Any(o => o.NhanVienId == currentUserId)
                                                                                && (p.LoaiDuocPham == true || p.LoaiVatTu == true)).ToListAsync();
                foreach (var thuoc in duocPhamBenhViens)
                {
                    foreach (var kho in lstKhoQuanLy)
                    {
                        if (nhapKhoDuocPhamChiTiets.Any(z => z.DuocPhamBenhVienId == thuoc.DuocPhamBenhVienId && z.NhapKhoDuocPhams.KhoId == kho.Id))
                        {
                            var soLuongTon = nhapKhoDuocPhamChiTiets
                                        .Where(o => o.NhapKhoDuocPhams.KhoId == kho.Id
                                                    && o.DuocPhamBenhVienId == thuoc.DuocPhamBenhVienId
                                                    && o.LaDuocPhamBHYT == laDuocPhamVatTuBHYT
                                                    && o.HanSuDung >= DateTime.Now
                                                    && o.SoLuongNhap > o.SoLuongDaXuat).Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                            var chiDinhGoiDuocPhamVatTuGridVo = new ChiDinhGoiDuocPhamVatTuGridVo()
                            {
                                Id = thuoc.DuocPhamBenhVienId.GetValueOrDefault(),
                                KhoId = kho.Id,
                                KhoDisplay = kho.Ten,
                                DuocPhamVatTuBenhVienId = thuoc.DuocPhamBenhVienId.GetValueOrDefault(),
                                Ten = thuoc.DuocPhamBenhVien.DuocPham.Ten,
                                DonViTinh = thuoc.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                SoLuongTon = soLuongTon,
                                SoLuongKe = thuoc.SoLuong,
                                LaDuocPhamVatTuBHYT = laDuocPhamVatTuBHYT,
                                GiaiDoanPhauThuat = EnumGiaiDoanPhauThuat.GayMe,
                                KhongTinhPhi = thuoc.KhongTinhPhi,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DuocPham,
                                LaDuocPham = true
                            };
                            chiDinhGoiDuocPhamVatTuGridVos.Add(chiDinhGoiDuocPhamVatTuGridVo);
                        }
                    }
                }

                foreach (var vatTu in vatTuBenhViens)
                {
                    foreach (var kho in lstKhoQuanLy)
                    {
                        if (nhapKhoVTChiTiets.Any(z => z.VatTuBenhVienId == vatTu.VatTuBenhVienId && z.NhapKhoVatTu.KhoId == kho.Id))
                        {
                            var soLuongTon = nhapKhoVTChiTiets
                                    .Where(o => o.NhapKhoVatTu.KhoId == kho.Id
                                     && o.VatTuBenhVienId == vatTu.VatTuBenhVienId
                                     && o.LaVatTuBHYT == false
                                     && o.HanSuDung >= DateTime.Now
                                     && o.SoLuongNhap > o.SoLuongDaXuat).Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                            var chiDinhGoiDuocPhamVatTuGridVo = new ChiDinhGoiDuocPhamVatTuGridVo()
                            {
                                Id = vatTu.VatTuBenhVienId.GetValueOrDefault(),
                                KhoId = kho.Id,
                                KhoDisplay = kho.Ten,
                                DuocPhamVatTuBenhVienId = vatTu.VatTuBenhVienId.GetValueOrDefault(),
                                Ten = vatTu.VatTuBenhVien.VatTus.Ten,
                                DonViTinh = vatTu.VatTuBenhVien.VatTus.DonViTinh,
                                SoLuongTon = soLuongTon,
                                SoLuongKe = vatTu.SoLuong,
                                LaDuocPhamVatTuBHYT = false,
                                KhongTinhPhi = vatTu.KhongTinhPhi,
                                GiaiDoanPhauThuat = EnumGiaiDoanPhauThuat.GayMe,
                                NhomGoiDichVu = EnumNhomGoiDichVu.VatTuTieuHao,
                                LaDuocPham = false
                            };
                            chiDinhGoiDuocPhamVatTuGridVos.Add(chiDinhGoiDuocPhamVatTuGridVo);
                        }
                    }
                }
            }
            else
            {
                var kho = await _khoRepository.GetByIdAsync(thongTinGhiNhan.KhoLuaChonId);
                foreach (var thuoc in duocPhamBenhViens)
                {
                    if (nhapKhoDuocPhamChiTiets.Any(z => z.DuocPhamBenhVienId == thuoc.DuocPhamBenhVienId && z.NhapKhoDuocPhams.KhoId == kho.Id))
                    {
                        var soLuongTon = nhapKhoDuocPhamChiTiets
                                .Where(o => o.NhapKhoDuocPhams.KhoId == kho.Id
                                            && o.DuocPhamBenhVienId == thuoc.DuocPhamBenhVienId
                                            && o.LaDuocPhamBHYT == laDuocPhamVatTuBHYT
                                            && o.HanSuDung >= DateTime.Now
                                            && o.SoLuongNhap > o.SoLuongDaXuat).Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                        var chiDinhGoiDuocPhamVatTuGridVo = new ChiDinhGoiDuocPhamVatTuGridVo()
                        {
                            Id = thuoc.DuocPhamBenhVienId.GetValueOrDefault(),
                            KhoId = kho.Id,
                            KhoDisplay = kho.Ten,
                            DuocPhamVatTuBenhVienId = thuoc.DuocPhamBenhVienId.GetValueOrDefault(),
                            Ten = thuoc.DuocPhamBenhVien.DuocPham.Ten,
                            DonViTinh = thuoc.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            SoLuongTon = soLuongTon,
                            SoLuongKe = thuoc.SoLuong,
                            LaDuocPhamVatTuBHYT = laDuocPhamVatTuBHYT,
                            KhongTinhPhi = thuoc.KhongTinhPhi,
                            NhomGoiDichVu = EnumNhomGoiDichVu.DuocPham,
                            GiaiDoanPhauThuat = EnumGiaiDoanPhauThuat.GayMe,
                            LaDuocPham = true
                        };
                        chiDinhGoiDuocPhamVatTuGridVos.Add(chiDinhGoiDuocPhamVatTuGridVo);
                    }
                }

                foreach (var vatTu in vatTuBenhViens)
                {
                    if (nhapKhoVTChiTiets.Any(z => z.VatTuBenhVienId == vatTu.VatTuBenhVienId && z.NhapKhoVatTu.KhoId == kho.Id))
                    {
                        var soLuongTon = nhapKhoVTChiTiets
                                .Where(o => o.NhapKhoVatTu.KhoId == kho.Id
                                 && o.VatTuBenhVienId == vatTu.VatTuBenhVienId
                                 && o.LaVatTuBHYT == false
                                 && o.HanSuDung >= DateTime.Now
                                 && o.SoLuongNhap > o.SoLuongDaXuat).Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                        var chiDinhGoiDuocPhamVatTuGridVo = new ChiDinhGoiDuocPhamVatTuGridVo()
                        {
                            Id = vatTu.VatTuBenhVienId.GetValueOrDefault(),
                            KhoId = kho.Id,
                            KhoDisplay = kho.Ten,
                            DuocPhamVatTuBenhVienId = vatTu.VatTuBenhVienId.GetValueOrDefault(),
                            Ten = vatTu.VatTuBenhVien.VatTus.Ten,
                            DonViTinh = vatTu.VatTuBenhVien.VatTus.DonViTinh,
                            SoLuongTon = soLuongTon,
                            LaDuocPhamVatTuBHYT = false,
                            SoLuongKe = vatTu.SoLuong,
                            KhongTinhPhi = vatTu.KhongTinhPhi,
                            NhomGoiDichVu = EnumNhomGoiDichVu.VatTuTieuHao,
                            GiaiDoanPhauThuat = EnumGiaiDoanPhauThuat.GayMe,
                            LaDuocPham = false
                        };
                        chiDinhGoiDuocPhamVatTuGridVos.Add(chiDinhGoiDuocPhamVatTuGridVo);
                    }

                }

            }
            var countTask = queryInfo.LazyLoadPage == true ? 0 : chiDinhGoiDuocPhamVatTuGridVos.Count();
            var queryTask = chiDinhGoiDuocPhamVatTuGridVos.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPagesGoiDuocPhamVatTuTrongDichVu(QueryInfo queryInfo)
        {
            return new GridDataSource
            {
                TotalRowCount = 0
            };
        }

        public async Task<List<LookupItemVo>> GetKhoLeNhanVienQuanLy(DropDownListRequestModel queryInfo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var lstKho = new List<LookupItemVo>();
            lstKho.Add(new LookupItemVo
            {
                KeyId = 0,
                DisplayName = "Tất cả"
            });

            //Update: Lấy tất cả các kho lẻ mà user dc phân quyền thuộc khoa đang login và kho tổng cấp 2
            var lstKhoQuanLy = await _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
                                                                            && p.KhoNhanVienQuanLys.Any(o => o.NhanVienId == currentUserId)
                                                                            && (p.LoaiDuocPham == true || p.LoaiVatTu == true)
                                                                            )
                                                                   .ApplyLike(queryInfo.Query, x => x.Ten)
                                                                   .OrderBy(p => p.Ten)
                                                                   .Select(item => new LookupItemVo()
                                                                   {
                                                                       KeyId = item.Id,
                                                                       DisplayName = item.Ten
                                                                   })
                                                                   .Take(queryInfo.Take)
                                                                   .ToListAsync();

            lstKho.AddRange(lstKhoQuanLy);

            return lstKho;
        }

        public async Task XuLyThemGhiNhanThuocVatTusAsync(ChiDinhGhiNhanVatTuThuocPTTTTheoGoiDVKTVo model, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            foreach (var thuocVatTu in model.ChiDinhGhiNhanVatTuThuocPTTTChiTiets)
            {
                var thongTinDichVuGhiNhan = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(thuocVatTu.DichVuGhiNhanId);
                if (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.DuocPham) // xử lý ghi nhận dược phẩm
                {
                    var duocPham = _duocPhamRepository.GetById(thuocVatTu.Id,
                                    x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                                    .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
                    var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                         .Where(o => o.NhapKhoDuocPhams.KhoId == thuocVatTu.KhoId
                                  && o.LaDuocPhamBHYT == thuocVatTu.LaDuocPhamBHYT
                                  && o.DuocPhamBenhVienId == thuocVatTu.Id
                                  && o.HanSuDung >= DateTime.Now
                                  && o.SoLuongNhap > o.SoLuongDaXuat)
                                  .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                    var SLTon = nhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    if (SLTon < thuocVatTu.SoLuong)
                    {
                        throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                    }
                    double soLuongCanXuat = thuocVatTu.SoLuong;
                    var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;
                    var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
                    {
                        YeuCauTiepNhanId = thuocVatTu.YeuCauTiepNhanId,
                        YeuCauDichVuKyThuatId = thuocVatTu.YeuCauDichVuKyThuatId,
                        DuocPhamBenhVienId = thuocVatTu.Id,
                        Ten = duocPham.Ten,
                        TenTiengAnh = duocPham.TenTiengAnh,
                        SoDangKy = duocPham.SoDangKy,
                        STTHoatChat = duocPham.STTHoatChat,
                        MaHoatChat = duocPham.MaHoatChat,
                        HoatChat = duocPham.HoatChat,
                        LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat,
                        NhaSanXuat = duocPham.NhaSanXuat,
                        NuocSanXuat = duocPham.NuocSanXuat,
                        DuongDungId = duocPham.DuongDungId,
                        HamLuong = duocPham.HamLuong,
                        QuyCach = duocPham.QuyCach,
                        TieuChuan = duocPham.TieuChuan,
                        DangBaoChe = duocPham.DangBaoChe,
                        DonViTinhId = duocPham.DonViTinhId,
                        HuongDan = duocPham.HuongDan,
                        MoTa = duocPham.MoTa,
                        ChiDinh = duocPham.ChiDinh,
                        ChongChiDinh = duocPham.ChongChiDinh,
                        LieuLuongCachDung = duocPham.LieuLuongCachDung,
                        TacDungPhu = duocPham.TacDungPhu,
                        ChuYdePhong = duocPham.ChuYDePhong,
                        SoLuong = soLuongXuat,
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        TrangThai = EnumYeuCauDuocPhamBenhVien.DaThucHien,
                        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                        DuocHuongBaoHiem = thuocVatTu.LaDuocPhamBHYT,
                        LaDuocPhamBHYT = thuocVatTu.LaDuocPhamBHYT,
                        SoTienBenhNhanDaChi = 0,
                        KhoLinhId = thuocVatTu.KhoId,
                        TheTich = duocPham.TheTich,
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                        GhiChu = null,
                        NoiTruPhieuDieuTriId = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(z => z.Id == thuocVatTu.YeuCauDichVuKyThuatId).Select(z => z.NoiTruPhieuDieuTriId).FirstOrDefault(),
                        KhongTinhPhi = !thuocVatTu.KhongTinhPhi,
                        GiaiDoanPhauThuat = thuocVatTu.GiaiDoanPhauThuat
                    };

                    soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                    ycDuocPhamBenhVien.HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiets.First().HopDongThauDuocPhamId;
                    var yeuCauDuocPhamBenhVienNew = ycDuocPhamBenhVien.Clone();
                    var xuatKhoDuocPham = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham
                    {
                        LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                        LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                        TenNguoiNhan = yeuCauTiepNhan.HoTen,
                        NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
                        LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                        NgayXuat = DateTime.Now,
                        KhoXuatId = thuocVatTu.KhoId
                    };
                    var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                    {
                        DuocPhamBenhVienId = thuocVatTu.Id,
                        XuatKhoDuocPham = xuatKhoDuocPham,
                        NgayXuat = DateTime.Now
                    };

                    var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();

                    foreach (var item in nhapKhoDuocPhamChiTiets)
                    {
                        if (thuocVatTu.SoLuong > 0)
                        {
                            var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.Where(o => o.HopDongThauDuocPhamId == item.HopDongThauDuocPhamId).Select(p => p.Gia).FirstOrDefault();

                            var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;
                            var tileBHYTThanhToanTheoNhap = item.LaDuocPhamBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;
                            if (yeuCauDuocPhamBenhVienNew.DonGiaNhap != 0 && (yeuCauDuocPhamBenhVienNew.DonGiaNhap != item.DonGiaNhap || yeuCauDuocPhamBenhVienNew.VAT != item.VAT || yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap)) // nếu khác đơn giá(lô) thì tạo ra 1 th dp mới
                            {
                                yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                                yeuCauDuocPhamBenhVienNew.SoLuong = yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat);
                                lstYeuCau.Add(yeuCauDuocPhamBenhVienNew);

                                yeuCauDuocPhamBenhVienNew = ycDuocPhamBenhVien.Clone();
                                yeuCauDuocPhamBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauDuocPhamBenhVienNew.DaCapThuoc = true;
                                yeuCauDuocPhamBenhVienNew.VAT = item.VAT;
                                yeuCauDuocPhamBenhVienNew.PhuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKho;
                                yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauDuocPhamBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;

                                xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                                {
                                    DuocPhamBenhVienId = thuocVatTu.Id,
                                    XuatKhoDuocPham = xuatKhoDuocPham,
                                    NgayXuat = DateTime.Now
                                };
                            }
                            else // tạo bình thường
                            {
                                yeuCauDuocPhamBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauDuocPhamBenhVienNew.VAT = item.VAT;
                                yeuCauDuocPhamBenhVienNew.DaCapThuoc = true;
                                yeuCauDuocPhamBenhVienNew.PhuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKho;
                                yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauDuocPhamBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;
                            }
                            if (item.SoLuongNhap > item.SoLuongDaXuat)
                            {
                                var xuatViTri = new XuatKhoDuocPhamChiTietViTri()
                                {
                                    NhapKhoDuocPhamChiTietId = item.Id,
                                    NgayXuat = DateTime.Now,
                                    GhiChu = xuatChiTiet.XuatKhoDuocPham.LyDoXuatKho
                                };

                                var tonTheoItem = (item.SoLuongNhap - item.SoLuongDaXuat).MathRoundNumber(2);
                                if (thuocVatTu.SoLuong <= tonTheoItem)
                                {
                                    xuatViTri.SoLuongXuat = thuocVatTu.SoLuong;
                                    item.SoLuongDaXuat = (item.SoLuongDaXuat + thuocVatTu.SoLuong).MathRoundNumber(2);
                                    thuocVatTu.SoLuong = 0;
                                }
                                else
                                {
                                    xuatViTri.SoLuongXuat = tonTheoItem;
                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                    thuocVatTu.SoLuong = (thuocVatTu.SoLuong - tonTheoItem).MathRoundNumber(2);
                                }

                                xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);
                            }

                            if (thuocVatTu.SoLuong == 0)
                            {
                                yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                                yeuCauDuocPhamBenhVienNew.SoLuong = yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat).MathRoundNumber(2);
                                lstYeuCau.Add(yeuCauDuocPhamBenhVienNew);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    foreach (var item in lstYeuCau)
                    {
                        yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Add(item);
                    }
                }
                else
                {
                    var vatTu = _vatTuRepository.GetById(thuocVatTu.Id,
                              x => x.Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.HopDongThauVatTu).Include(o => o.HopDongThauVatTuChiTiets)
                                    .Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.NhapKhoVatTu).ThenInclude(nk => nk.Kho));

                    var nhapKhoVatTuChiTiets = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
                            .Where(o => o.NhapKhoVatTu.KhoId == thuocVatTu.KhoId
                                && o.LaVatTuBHYT == thuocVatTu.LaDuocPhamBHYT
                                && o.HanSuDung >= DateTime.Now
                                && o.VatTuBenhVienId == thuocVatTu.Id
                                && o.SoLuongNhap > o.SoLuongDaXuat)
                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                    var SLTon = nhapKhoVatTuChiTiets.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    double soLuongCanXuat = thuocVatTu.SoLuong;
                    var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;
                    var ycVatTuBenhVien = new YeuCauVatTuBenhVien
                    {
                        YeuCauTiepNhanId = thuocVatTu.YeuCauTiepNhanId,
                        YeuCauDichVuKyThuatId = thuocVatTu.YeuCauDichVuKyThuatId,
                        VatTuBenhVienId = thuocVatTu.Id,
                        Ten = vatTu.Ten,
                        Ma = vatTu.Ma,
                        NhomVatTuId = vatTu.NhomVatTuId,
                        DonViTinh = vatTu.DonViTinh,
                        NhaSanXuat = vatTu.NhaSanXuat,
                        NuocSanXuat = vatTu.NuocSanXuat,
                        QuyCach = vatTu.QuyCach,
                        MoTa = vatTu.MoTa,
                        SoLuong = soLuongXuat,
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                        LaVatTuBHYT = thuocVatTu.LaDuocPhamBHYT,
                        SoTienBenhNhanDaChi = 0,
                        KhoLinhId = thuocVatTu.KhoId,
                        DuocHuongBaoHiem = thuocVatTu.LaDuocPhamBHYT,
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                        LoaiNoiChiDinh = LoaiNoiChiDinh.PhauThuatThuThuat,
                        GiaiDoanPhauThuat = thuocVatTu.GiaiDoanPhauThuat,
                        KhongTinhPhi = !thuocVatTu.KhongTinhPhi,
                        NoiTruPhieuDieuTriId = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(z => z.Id == thuocVatTu.YeuCauDichVuKyThuatId).Select(z => z.NoiTruPhieuDieuTriId).FirstOrDefault(),
                    };
                    var yeuCauVatTuBenhVienNew = ycVatTuBenhVien.Clone();
                    var xuatKhoVatTu = new XuatKhoVatTu
                    {
                        LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                        LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                        TenNguoiNhan = yeuCauTiepNhan.HoTen,
                        NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
                        LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                        NgayXuat = DateTime.Now,
                        KhoXuatId = thuocVatTu.KhoId
                    };

                    var xuatChiTiet = new XuatKhoVatTuChiTiet()
                    {
                        VatTuBenhVienId = thuocVatTu.Id,
                        XuatKhoVatTu = xuatKhoVatTu,
                        NgayXuat = DateTime.Now
                    };
                    var lstYeuCau = new List<YeuCauVatTuBenhVien>();

                    foreach (var item in nhapKhoVatTuChiTiets)
                    {
                        if (thuocVatTu.SoLuong > 0)
                        {
                            var giaTheoHopDong = vatTu.HopDongThauVatTuChiTiets.Where(o => o.HopDongThauVatTuId == item.HopDongThauVatTuId).Select(p => p.Gia).FirstOrDefault();
                            var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;
                            var tileBHYTThanhToanTheoNhap = item.LaVatTuBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;

                            if (yeuCauVatTuBenhVienNew.DonGiaNhap != 0 && (yeuCauVatTuBenhVienNew.DonGiaNhap != item.DonGiaNhap || yeuCauVatTuBenhVienNew.VAT != item.VAT || yeuCauVatTuBenhVienNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap))
                            {
                                yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                                yeuCauVatTuBenhVienNew.SoLuong = yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat);
                                lstYeuCau.Add(yeuCauVatTuBenhVienNew);

                                yeuCauVatTuBenhVienNew = ycVatTuBenhVien.Clone();
                                yeuCauVatTuBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauVatTuBenhVienNew.DaCapVatTu = true;
                                yeuCauVatTuBenhVienNew.VAT = item.VAT;
                                yeuCauVatTuBenhVienNew.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                                yeuCauVatTuBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauVatTuBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;

                                xuatChiTiet = new XuatKhoVatTuChiTiet()
                                {
                                    VatTuBenhVienId = thuocVatTu.Id,
                                    XuatKhoVatTu = xuatKhoVatTu,
                                    NgayXuat = DateTime.Now
                                };
                            }
                            else
                            {
                                yeuCauVatTuBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauVatTuBenhVienNew.DaCapVatTu = true;
                                yeuCauVatTuBenhVienNew.VAT = item.VAT;
                                yeuCauVatTuBenhVienNew.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                                yeuCauVatTuBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauVatTuBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;
                            }
                            if (item.SoLuongNhap > item.SoLuongDaXuat)
                            {
                                var xuatViTri = new XuatKhoVatTuChiTietViTri()
                                {
                                    NhapKhoVatTuChiTietId = item.Id,
                                    NgayXuat = DateTime.Now,
                                    GhiChu = xuatChiTiet.XuatKhoVatTu.LyDoXuatKho
                                };

                                var tonTheoItem = (item.SoLuongNhap - item.SoLuongDaXuat).MathRoundNumber(2);
                                if (thuocVatTu.SoLuong <= tonTheoItem)
                                {
                                    xuatViTri.SoLuongXuat = thuocVatTu.SoLuong;
                                    item.SoLuongDaXuat = (item.SoLuongDaXuat + thuocVatTu.SoLuong).MathRoundNumber(2);
                                    thuocVatTu.SoLuong = 0;
                                }
                                else
                                {
                                    xuatViTri.SoLuongXuat = tonTheoItem;
                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                    thuocVatTu.SoLuong = (thuocVatTu.SoLuong - tonTheoItem).MathRoundNumber(2);
                                }

                                xuatChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatViTri);
                            }

                            if (thuocVatTu.SoLuong == 0)
                            {
                                yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                                yeuCauVatTuBenhVienNew.SoLuong = yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat).MathRoundNumber(2);
                                lstYeuCau.Add(yeuCauVatTuBenhVienNew);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    foreach (var item in lstYeuCau)
                    {
                        yeuCauTiepNhan.YeuCauVatTuBenhViens.Add(item);
                    }
                }
            }
        }

        public async Task<long> GetYeuCauTiepNhanIdTheoYeuCauKyThuatIdAsync(long yeuCauDichVuKyThuatId)
        {
            var yeuCauTiepNhanId = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.Id == yeuCauDichVuKyThuatId)
                .Select(x => x.YeuCauTiepNhanId)
                .First();
            return yeuCauTiepNhanId;
        }
    }
}
