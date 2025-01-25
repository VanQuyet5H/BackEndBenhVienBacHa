using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.CauHinh;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoTraCuuDuLieuForGridAsync(BaoCaoTraCuuDuLieuQueryInfo queryInfo)
        {
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();
            var nhaThaus = _nhaThauRepository.TableNoTracking.Select(o => new { o.Id, o.Ma, o.Ten }).ToList();
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            List<long> nhomHoaChats = new List<long> { cauHinhBaoCao.DuocPhamBenhVienNhomHoaChat };
            var nhomHoaChatCons = duocPhamBenhVienPhanNhoms.Where(o => o.NhomChaId != null && nhomHoaChats.Contains(o.NhomChaId.Value) && !nhomHoaChats.Contains(o.Id)).ToList();
            while (nhomHoaChatCons.Count > 0)
            {
                nhomHoaChats.AddRange(nhomHoaChatCons.Select(o => o.Id));
                nhomHoaChatCons = duocPhamBenhVienPhanNhoms.Where(o => o.NhomChaId != null && nhomHoaChats.Contains(o.NhomChaId.Value) && !nhomHoaChats.Contains(o.Id)).ToList();
            }

            var dsDuocPhamNhapKho = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking
                .Where(o => o.DuocKeToanDuyet == true && o.NgayNhap >= queryInfo.FromDate &&
                            o.NgayNhap < queryInfo.ToDate)
                .Select(o => new BaoCaoTraCuuDuLieuQueryData
                {
                    Id = o.Id*10,
                    DaXuatExcel = o.DaXuatExcel,
                    NgayNhap = o.NgayNhap,
                    SoPhieu = o.SoPhieu,
                    NgayHoaDon = o.NgayHoaDon,
                    SoHoaDon = o.SoChungTu,
                    KyHieuHoaDon = o.KyHieuHoaDon,
                    DuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                    BaoCaoTraCuuDuLieuQueryDataDetails = o.YeuCauNhapKhoDuocPhamChiTiets.Select(ct => new BaoCaoTraCuuDuLieuQueryDataDetail
                    {
                        KhoNhapSauKhiDuyetId = ct.KhoNhapSauKhiDuyetId,
                        NhaThauId = ct.HopDongThauDuocPham.NhaThauId,
                        DuocPhamBenhVienId = ct.DuocPhamBenhVienId,
                        DuocPhamBenhVienPhanNhomId = ct.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                        DonGiaNhap = ct.DonGiaNhap,
                        SoLuongNhap = ct.SoLuongNhap,
                        VAT = ct.VAT,
                        ThanhTienTruocVat = ct.ThanhTienTruocVat,
                        ThanhTienSauVat = ct.ThanhTienSauVat,
                        ThueVatLamTron = ct.ThueVatLamTron.GetValueOrDefault(),
                        Nhom = "T"
                    }).ToList()
                }).ToList();
            foreach (var baoCaoTinhHinhNhapTuNhaCungCapQueryData in dsDuocPhamNhapKho)
            {
                foreach (var baoCaoTinhHinhNhapTuNhaCungCapChiTiet in baoCaoTinhHinhNhapTuNhaCungCapQueryData.BaoCaoTraCuuDuLieuQueryDataDetails)
                {
                    if (baoCaoTinhHinhNhapTuNhaCungCapChiTiet.DuocPhamBenhVienPhanNhomId != null && nhomHoaChats.Contains(baoCaoTinhHinhNhapTuNhaCungCapChiTiet.DuocPhamBenhVienPhanNhomId.Value))
                    {
                        baoCaoTinhHinhNhapTuNhaCungCapChiTiet.Nhom = "H";
                    }
                }
            }

            var dsVatTuNhapKho = _yeuCauNhapKhoVatTuRepository.TableNoTracking
                .Where(o => o.DuocKeToanDuyet == true && o.NgayNhap >= queryInfo.FromDate &&
                            o.NgayNhap < queryInfo.ToDate)
                .Select(o => new BaoCaoTraCuuDuLieuQueryData
                {
                    Id = o.Id*10+1,
                    DaXuatExcel = o.DaXuatExcel,
                    NgayNhap = o.NgayNhap,
                    SoPhieu = o.SoPhieu,
                    NgayHoaDon = o.NgayHoaDon,
                    SoHoaDon = o.SoChungTu,
                    KyHieuHoaDon = o.KyHieuHoaDon,
                    DuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                    BaoCaoTraCuuDuLieuQueryDataDetails = o.YeuCauNhapKhoVatTuChiTiets.Select(ct => new BaoCaoTraCuuDuLieuQueryDataDetail
                    {
                        KhoNhapSauKhiDuyetId = ct.KhoNhapSauKhiDuyetId,
                        NhaThauId = ct.HopDongThauVatTu.NhaThauId,
                        VatTuBenhVienId = ct.VatTuBenhVienId,
                        DonGiaNhap = ct.DonGiaNhap,
                        SoLuongNhap = ct.SoLuongNhap,
                        VAT = ct.VAT,
                        ThanhTienTruocVat = ct.ThanhTienTruocVat,
                        ThanhTienSauVat = ct.ThanhTienSauVat,
                        ThueVatLamTron = ct.ThueVatLamTron.GetValueOrDefault(),
                        Nhom = "V"
                    }).ToList()
                }).ToList();

            var dsNhapKho = dsDuocPhamNhapKho.Concat(dsVatTuNhapKho).OrderBy(o=>o.NgayNhap);

            var returnData = new List<BaoCaoTraCuuDuLieu>();
            foreach (var baoCaoTraCuuDuLieuQueryData in dsNhapKho)
            {
                var chiTiets = baoCaoTraCuuDuLieuQueryData.BaoCaoTraCuuDuLieuQueryDataDetails.ToList();
                if (chiTiets.Any())
                {
                    if (baoCaoTraCuuDuLieuQueryData.DuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien)
                    {
                        var chiTietTheoNhaThauGroup = chiTiets.GroupBy(o => o.NhaThauId);
                        foreach (var chiTietTheoNhaThau in chiTietTheoNhaThauGroup)
                        {
                            //var thanhTienThuoc = chiTietTheoNhaThau.Where(o => o.Nhom == 1).Select(o => o.ThanhTienTruocVat).DefaultIfEmpty().Sum();
                            //var thanhTienVatTu = chiTietTheoNhaThau.Where(o => o.Nhom == 2).Select(o => o.ThanhTienTruocVat).DefaultIfEmpty().Sum();
                            //var thanhTienHoaChat = chiTietTheoNhaThau.Where(o => o.Nhom == 3).Select(o => o.ThanhTienTruocVat).DefaultIfEmpty().Sum();
                            //var thanhTienVAT = chiTietTheoNhaThau.Select(o => o.ThueVatLamTron).DefaultIfEmpty().Sum();
                            var thanhTien = chiTietTheoNhaThau.Select(o => o.ThanhTienSauVat).DefaultIfEmpty().Sum();
                            var nhaThau = nhaThaus.FirstOrDefault(o => o.Id == chiTietTheoNhaThau.Key);
                            var dienGiai = $"Nhập Vật tư kho Hành chính Theo hoá đơn {baoCaoTraCuuDuLieuQueryData.SoHoaDon}";
                            var gridItem = new BaoCaoTraCuuDuLieu
                            {
                                Id = baoCaoTraCuuDuLieuQueryData.Id,
                                DaXuatExcel = baoCaoTraCuuDuLieuQueryData.DaXuatExcel == true,
                                HienThiTrenSo = string.Empty,
                                NgayChungTu = baoCaoTraCuuDuLieuQueryData.NgayNhap,
                                NgayHachToan = baoCaoTraCuuDuLieuQueryData.NgayNhap,
                                SoChungTu = baoCaoTraCuuDuLieuQueryData.SoPhieu,
                                DienGiai = dienGiai,
                                HanThanhToan = string.Empty,
                                DienGiaiHachToan = dienGiai,
                                TaiKhoanNo = "152",
                                TaiKhoanCo = "331",
                                SoTien = thanhTien,
                                DoiTuongNo = string.Empty,
                                DoiTuongCo = nhaThau?.Ma,
                                TaiKhoanNganHang = string.Empty,
                                KhoanMucCP = chiTietTheoNhaThau.First().Nhom,
                                DonVi = string.Empty,
                                DoiTuongTHCP = string.Empty,
                                CongTrinh = string.Empty,
                                HopDongBan = string.Empty,
                                CPKhongHopLy = string.Empty,
                                MaThongKe = string.Empty,
                                DienGiaiThue = dienGiai,
                                TKThueGTGT = "1331",
                                TienThueGTGT = null,
                                PhanTramThueGTGT = 0,
                                GiaTriHHDVChuaThue = thanhTien,
                                MauSoHopDong = string.Empty,
                                NgayHoaDon = baoCaoTraCuuDuLieuQueryData.NgayHoaDon,
                                KyHieuHopDong = baoCaoTraCuuDuLieuQueryData.KyHieuHoaDon,
                                SoHoaDon = baoCaoTraCuuDuLieuQueryData.SoHoaDon,
                                NhomHHDVMuaVao = 1,
                                MaDoiTuongThue = nhaThau?.Ma,
                                TenDoiTuongThue = string.Empty,
                                MaSoThueDoiTuongThue = string.Empty,
                            };
                            returnData.Add(gridItem);
                        }
                    }
                    else
                    {
                        var chiTietThuocGroups = chiTiets.GroupBy(o => new {o.NhaThauId, o.Nhom, NhapNhaThuoc = (o.KhoNhapSauKhiDuyetId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc)});
                        foreach (var chiTietThuocGroup in chiTietThuocGroups)
                        {
                            if (!chiTietThuocGroup.Key.NhapNhaThuoc)
                            {
                                var thanhTien = chiTietThuocGroup.Select(o => o.ThanhTienSauVat).DefaultIfEmpty().Sum();
                                var nhaThau = nhaThaus.FirstOrDefault(o => o.Id == chiTietThuocGroup.Key.NhaThauId);
                                var dienGiai = $"Nhập Thuốc (BV) Theo hoá đơn {baoCaoTraCuuDuLieuQueryData.SoHoaDon}";
                                var gridItem = new BaoCaoTraCuuDuLieu
                                {
                                    Id = baoCaoTraCuuDuLieuQueryData.Id,
                                    DaXuatExcel = baoCaoTraCuuDuLieuQueryData.DaXuatExcel == true,
                                    HienThiTrenSo = string.Empty,
                                    NgayChungTu = baoCaoTraCuuDuLieuQueryData.NgayNhap,
                                    NgayHachToan = baoCaoTraCuuDuLieuQueryData.NgayNhap,
                                    SoChungTu = baoCaoTraCuuDuLieuQueryData.SoPhieu,
                                    DienGiai = dienGiai,
                                    HanThanhToan = string.Empty,
                                    DienGiaiHachToan = dienGiai,
                                    TaiKhoanNo = "152",
                                    TaiKhoanCo = "331",
                                    SoTien = thanhTien,
                                    DoiTuongNo = string.Empty,
                                    DoiTuongCo = nhaThau?.Ma,
                                    TaiKhoanNganHang = string.Empty,
                                    KhoanMucCP = chiTietThuocGroup.Key.Nhom,
                                    DonVi = string.Empty,
                                    DoiTuongTHCP = string.Empty,
                                    CongTrinh = string.Empty,
                                    HopDongBan = string.Empty,
                                    CPKhongHopLy = string.Empty,
                                    MaThongKe = string.Empty,
                                    DienGiaiThue = dienGiai,
                                    TKThueGTGT = "1331",
                                    TienThueGTGT = null,
                                    PhanTramThueGTGT = 0,
                                    GiaTriHHDVChuaThue = thanhTien,
                                    MauSoHopDong = string.Empty,
                                    NgayHoaDon = baoCaoTraCuuDuLieuQueryData.NgayHoaDon,
                                    KyHieuHopDong = baoCaoTraCuuDuLieuQueryData.KyHieuHoaDon,
                                    SoHoaDon = baoCaoTraCuuDuLieuQueryData.SoHoaDon,
                                    NhomHHDVMuaVao = 1,
                                    MaDoiTuongThue = nhaThau?.Ma,
                                    TenDoiTuongThue = string.Empty,
                                    MaSoThueDoiTuongThue = string.Empty,
                                };
                                returnData.Add(gridItem);
                            }
                            else
                            {
                                var chiTietThuocGroupVATs = chiTietThuocGroup.GroupBy(o => o.VAT );
                                foreach (var chiTietThuocGroupVAT in chiTietThuocGroupVATs)
                                {
                                    var thanhTienVAT = chiTietThuocGroupVAT.Select(o => o.ThueVatLamTron).DefaultIfEmpty().Sum();
                                    var thanhTienTruocVat = chiTietThuocGroupVAT.Select(o => o.ThanhTienTruocVat).DefaultIfEmpty().Sum();
                                    //var thanhTienSauVat = chiTietThuocGroupVAT.Select(o => o.ThanhTienSauVat).DefaultIfEmpty().Sum();
                                    var nhaThau = nhaThaus.FirstOrDefault(o => o.Id == chiTietThuocGroup.Key.NhaThauId);
                                    var dienGiai = $"Nhập Thuốc (BV) Theo hoá đơn {baoCaoTraCuuDuLieuQueryData.SoHoaDon}";
                                    var gridItem = new BaoCaoTraCuuDuLieu
                                    {
                                        Id = baoCaoTraCuuDuLieuQueryData.Id,
                                        DaXuatExcel = baoCaoTraCuuDuLieuQueryData.DaXuatExcel == true,
                                        HienThiTrenSo = string.Empty,
                                        NgayChungTu = baoCaoTraCuuDuLieuQueryData.NgayNhap,
                                        NgayHachToan = baoCaoTraCuuDuLieuQueryData.NgayNhap,
                                        SoChungTu = baoCaoTraCuuDuLieuQueryData.SoPhieu,
                                        DienGiai = dienGiai,
                                        HanThanhToan = string.Empty,
                                        DienGiaiHachToan = dienGiai,
                                        TaiKhoanNo = "1561",
                                        TaiKhoanCo = "331",
                                        SoTien = thanhTienTruocVat,
                                        DoiTuongNo = string.Empty,
                                        DoiTuongCo = nhaThau?.Ma,
                                        TaiKhoanNganHang = string.Empty,
                                        KhoanMucCP = chiTietThuocGroup.Key.Nhom,
                                        DonVi = string.Empty,
                                        DoiTuongTHCP = string.Empty,
                                        CongTrinh = string.Empty,
                                        HopDongBan = string.Empty,
                                        CPKhongHopLy = string.Empty,
                                        MaThongKe = string.Empty,
                                        DienGiaiThue = dienGiai,
                                        TKThueGTGT = "1331",
                                        TienThueGTGT = thanhTienVAT,
                                        PhanTramThueGTGT = chiTietThuocGroupVAT.Key,
                                        GiaTriHHDVChuaThue = thanhTienTruocVat,
                                        MauSoHopDong = string.Empty,
                                        NgayHoaDon = baoCaoTraCuuDuLieuQueryData.NgayHoaDon,
                                        KyHieuHopDong = baoCaoTraCuuDuLieuQueryData.KyHieuHoaDon,
                                        SoHoaDon = baoCaoTraCuuDuLieuQueryData.SoHoaDon,
                                        NhomHHDVMuaVao = 1,
                                        MaDoiTuongThue = nhaThau?.Ma,
                                        TenDoiTuongThue = string.Empty,
                                        MaSoThueDoiTuongThue = string.Empty,
                                    };
                                    returnData.Add(gridItem);
                                    if (chiTietThuocGroupVAT.Key != 0)
                                    {
                                        var dienGiaiVAT = $"Thuế VAT Nhập Thuốc (BV) Theo hoá đơn {baoCaoTraCuuDuLieuQueryData.SoHoaDon}";
                                        var gridItemVAT = new BaoCaoTraCuuDuLieu
                                        {
                                            Id = baoCaoTraCuuDuLieuQueryData.Id,
                                            DaXuatExcel = baoCaoTraCuuDuLieuQueryData.DaXuatExcel == true,
                                            HienThiTrenSo = string.Empty,
                                            NgayChungTu = baoCaoTraCuuDuLieuQueryData.NgayNhap,
                                            NgayHachToan = baoCaoTraCuuDuLieuQueryData.NgayNhap,
                                            SoChungTu = baoCaoTraCuuDuLieuQueryData.SoPhieu,
                                            DienGiai = dienGiaiVAT,
                                            HanThanhToan = string.Empty,
                                            DienGiaiHachToan = dienGiaiVAT,
                                            TaiKhoanNo = "1331",
                                            TaiKhoanCo = "331",
                                            SoTien = thanhTienVAT,
                                            DoiTuongNo = string.Empty,
                                            DoiTuongCo = nhaThau?.Ma,
                                            TaiKhoanNganHang = string.Empty,
                                            KhoanMucCP = chiTietThuocGroup.Key.Nhom,
                                            DonVi = string.Empty,
                                            DoiTuongTHCP = string.Empty,
                                            CongTrinh = string.Empty,
                                            HopDongBan = string.Empty,
                                            CPKhongHopLy = string.Empty,
                                            MaThongKe = string.Empty,
                                            DienGiaiThue = dienGiaiVAT,
                                            TKThueGTGT = "1331",
                                            TienThueGTGT = null,
                                            PhanTramThueGTGT = null,
                                            GiaTriHHDVChuaThue = null,
                                            MauSoHopDong = string.Empty,
                                            NgayHoaDon = baoCaoTraCuuDuLieuQueryData.NgayHoaDon,
                                            KyHieuHopDong = baoCaoTraCuuDuLieuQueryData.KyHieuHoaDon,
                                            SoHoaDon = baoCaoTraCuuDuLieuQueryData.SoHoaDon,
                                            NhomHHDVMuaVao = 1,
                                            MaDoiTuongThue = nhaThau?.Ma,
                                            TenDoiTuongThue = string.Empty,
                                            MaSoThueDoiTuongThue = string.Empty,
                                        };
                                        returnData.Add(gridItemVAT);
                                    }
                                }
                            }
                            
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.TimKiem))
            {
                returnData = returnData
                    .Where(o => (o.SoChungTu != null && o.SoChungTu.ToLower().Contains(queryInfo.TimKiem.ToLower())) 
                                || (o.DienGiai != null && o.DienGiai.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.DienGiaiHachToan != null && o.DienGiaiHachToan.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.TaiKhoanNo != null && o.TaiKhoanNo.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.TaiKhoanCo != null && o.TaiKhoanCo.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.DoiTuongNo != null && o.DoiTuongNo.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.DoiTuongCo != null && o.DoiTuongCo.ToLower().Contains(queryInfo.TimKiem.ToLower())) 
                                || (o.TaiKhoanNganHang != null && o.TaiKhoanNganHang.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.KhoanMucCP != null && o.KhoanMucCP.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.DonVi != null && o.DonVi.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.DoiTuongTHCP != null && o.DoiTuongTHCP.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.CongTrinh != null && o.CongTrinh.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.HopDongBan != null && o.HopDongBan.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.CPKhongHopLy != null && o.CPKhongHopLy.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.MaThongKe != null && o.MaThongKe.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.DienGiaiThue != null && o.DienGiaiThue.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.TKThueGTGT != null && o.TKThueGTGT.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.MauSoHopDong != null && o.MauSoHopDong.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.KyHieuHopDong != null && o.KyHieuHopDong.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.SoHoaDon != null && o.SoHoaDon.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.MaDoiTuongThue != null && o.MaDoiTuongThue.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.TenDoiTuongThue != null && o.TenDoiTuongThue.ToLower().Contains(queryInfo.TimKiem.ToLower()))
                                || (o.MaSoThueDoiTuongThue != null && o.MaSoThueDoiTuongThue.ToLower().Contains(queryInfo.TimKiem.ToLower())))
                    .ToList();
            }

            return new GridDataSource { Data = returnData.ToArray(), TotalRowCount = returnData.Count() };
            /*
            var allData = new List<BaoCaoTraCuuDuLieu>()
            {
                new BaoCaoTraCuuDuLieu
                {
                    HienThiTrenSo = string.Empty,

                    NgayChungTu = DateTime.Now,
                    NgayHachToan = DateTime.Now,
                    SoChungTu ="MH0821-0072",
                    DienGiai = "Nhập Thuốc (BV) Theo hoá đơn 0084156",
                    HanThanhToan = string.Empty,
                    DienGiaiHachToan ="Nhập Thuốc (BV) Theo hoá đơn 0084156",
                    TaiKhoanNo = "152",
                    TaiKhoanCo = "331",
                    SoTien = 20967712,
                    DoiTuongNo = string.Empty,
                    DoiTuongCo = "HAPHARCO",
                    TaiKhoanNganHang = string.Empty,
                    KhoanMucCP = "T",
                    DonVi =string.Empty,
                    DoiTuongTHCP =string.Empty,
                    CongTrinh =string.Empty,
                    HopDongBan =string.Empty,
                    CPKhongHopLy =string.Empty,
                    MaThongKe =string.Empty,
                    DienGiaiThue = "Nhập Thuốc (BV) Theo hoá đơn 0084156",
                    TKThueGTGT = "1331",

                    TienThueGTGT = 0,
                    PhanTramThueGTGT = 10,
                    GiaTriHHDVChuaThue = 20967712,
                    MauSoHopDong = string.Empty,
                    NgayHoaDon = DateTime.Now,
                    KyHieuHopDong = "HB/20E",
                    SoHoaDon = "84156",
                    NhomHHDVMuaVao = string.Empty,
                    MaDoiTuongThue ="HAPHARCO",
                    TenDoiTuongThue= string.Empty,
                    MaSoThueDoiTuongThue = string.Empty,
                }
            };
            return new GridDataSource { Data = allData.ToArray(), TotalRowCount = allData.Count() };
            */
        }

        public virtual byte[] ExporBaoCaoTraCuuDuLieuGridVo(GridDataSource gridDataSource, BaoCaoTraCuuDuLieuQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTraCuuDuLieu>)gridDataSource.Data;
            //BVHD-3957: Dòng nào đã xuất excel đánh dấu/đổi màu
            if (query.BaoCaoTraCuuDuLieuIds != null)
            {
                var yeuCauNhapKhoDuocPhamIds = query.BaoCaoTraCuuDuLieuIds.Where(o=>o % 10 == 0).Select(o=>o / 10).ToList();
                var yeuCauNhapKhoVatTuIds = query.BaoCaoTraCuuDuLieuIds.Where(o => o % 10 == 1).Select(o => (o-1) / 10).ToList();

                var yeuCauNhapKhoDuocPhams = _yeuCauNhapKhoDuocPhamRepository.Table.Where(o => yeuCauNhapKhoDuocPhamIds.Contains(o.Id)).ToList();
                var yeuCauNhapKhoVatTus = _yeuCauNhapKhoVatTuRepository.Table.Where(o => yeuCauNhapKhoVatTuIds.Contains(o.Id)).ToList();

                foreach(var dp in yeuCauNhapKhoDuocPhams)
                {
                    dp.DaXuatExcel = true;
                }
                foreach (var vt in yeuCauNhapKhoVatTus)
                {
                    vt.DaXuatExcel = true;
                }
                _yeuCauNhapKhoDuocPhamRepository.Context.SaveChanges();

                datas = datas.Where(o => query.BaoCaoTraCuuDuLieuIds.Contains(o.Id)).ToList();
            }
            else
            {
                datas = new List<BaoCaoTraCuuDuLieu>();
            }

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTraCuuDuLieu>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Tra cứu dữ liệu");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;
                    worksheet.Column(16).Width = 30;

                    worksheet.Column(17).Width = 30;
                    worksheet.Column(18).Width = 30;
                    worksheet.Column(19).Width = 30;
                    worksheet.Column(20).Width = 30;
                    worksheet.Column(21).Width = 30;
                    worksheet.Column(22).Width = 30;
                    worksheet.Column(23).Width = 30;
                    worksheet.Column(24).Width = 30;
                    worksheet.Column(25).Width = 30;
                    worksheet.Column(26).Width = 30;
                    worksheet.Column(27).Width = 30;
                    worksheet.Column(28).Width = 30;
                    worksheet.Column(29).Width = 30;
                    worksheet.Column(30).Width = 30;
                    worksheet.Column(31).Width = 30;
                    worksheet.Column(32).Width = 30;
                    worksheet.Column(33).Width = 30;

                    worksheet.DefaultColWidth = 7;                   

                    // SET title head cho bảng excel  
                    using (var range = worksheet.Cells["A1:AG1"])
                    {
                        range.Worksheet.Cells["A1:AG1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:AG1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:AG1"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A1:AG1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:AG1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:AG1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A1"].Value = "Hiển thị trên sổ";

                        range.Worksheet.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B1"].Value = "Ngày chứng từ (*)";

                        range.Worksheet.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C1"].Value = "Ngày hạch toán (*)";

                        range.Worksheet.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D1"].Value = "Số chứng từ (*)";
                    
                        range.Worksheet.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E1"].Value = "Diễn giải";

                        range.Worksheet.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F1"].Value = "Hạn thanh toán";

                        range.Worksheet.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G1"].Value = "Diễn giải (Hạch toán)";

                        range.Worksheet.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H1"].Value = "TK Nợ (*)";

                        range.Worksheet.Cells["I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I1"].Value = "TK Có (*)";

                        range.Worksheet.Cells["J1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J1"].Value = "Số tiền";                      

                        range.Worksheet.Cells["K1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K1"].Value = "Đối tượng Nợ";

                        range.Worksheet.Cells["L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L1"].Value = "Đối tượng Có";

                        range.Worksheet.Cells["M1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M1"].Value = "TK ngân hàng";

                        range.Worksheet.Cells["N1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N1"].Value = "Khoản mục CP";

                        range.Worksheet.Cells["O1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O1"].Value = "Đơn vị";

                        range.Worksheet.Cells["P1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P1"].Value = "Đối tượng THCP";
                    
                        range.Worksheet.Cells["Q1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q1"].Value = "Công trình";

                        range.Worksheet.Cells["R1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R1"].Value = "Hợp đồng bán";
                        
                        range.Worksheet.Cells["S1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["S1"].Value = "CP không hợp lý";

                        range.Worksheet.Cells["T1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["T1"].Value = "Mã thống kê";

                        range.Worksheet.Cells["U1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["U1"].Value = "Diễn giải (Thuế)";

                        range.Worksheet.Cells["V1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["V1"].Value = "TK thuế GTGT";

                        range.Worksheet.Cells["W1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["W1"].Value = "Tiền thuế GTGT";

                        range.Worksheet.Cells["X1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["X1"].Value = "% thuế GTGT";

                        range.Worksheet.Cells["Y1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Y1"].Value = "Giá trị HHDV chưa thuế";

                        range.Worksheet.Cells["Z1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Z1"].Value = "Mẫu số HĐ";

                        range.Worksheet.Cells["AA1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AA1"].Value = "Ngày hóa đơn";
                        
                        range.Worksheet.Cells["AB1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AB1"].Value = "Ký hiệu HĐ";

                        range.Worksheet.Cells["AC1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AC1"].Value = "Số hóa đơn";

                        range.Worksheet.Cells["AD1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AD1"].Value = "Nhóm HHDV mua vào";

                        range.Worksheet.Cells["AE1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AE1"].Value = "Mã đối tượng thuế";

                        range.Worksheet.Cells["AF1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AF1"].Value = "Tên đối tượng thuế";

                        range.Worksheet.Cells["AG1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["AG1"].Value = "Mã số thuế đối tượng thuế";
                    }


                    //write data from line 9       
                    int index = 2;
                    var stt = 1;
                   
                    if (datas.Any())
                    {



                        foreach (var item in datas)
                        {
                            // format border, font chữ,....
                            worksheet.Cells["A" + index + ":AG" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":AG" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":AG" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":AG" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":AG" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                      
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index].Value = item.HienThiTrenSo;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Value = item.NgayChungTuDisplay;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["C" + index].Value = item.NgayHachToanDisplay;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["D" + index].Value = item.SoChungTu;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["E" + index].Value = item.DienGiai;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["F" + index].Value = item.HanThanhToan;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["G" + index].Value = item.DienGiaiHachToan;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["H" + index].Value = item.TaiKhoanNo;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["I" + index].Value = item.TaiKhoanCo;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Value = item.SoTien;
                            
                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["K" + index].Value = item.DoiTuongNo;

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["L" + index].Value = item.DoiTuongCo;


                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["M" + index].Value = item.TaiKhoanNganHang;

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["N" + index].Value = item.KhoanMucCP;

                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["O" + index].Value = item.DonVi;

                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["P" + index].Value = item.DoiTuongTHCP;

                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["Q" + index].Value = item.CongTrinh;

                            worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["R" + index].Value = item.HopDongBan;

                            worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["S" + index].Value = item.CPKhongHopLy;

                            worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["T" + index].Value = item.MaThongKe;

                            worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["U" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["U" + index].Value = item.DienGiaiThue;

                            worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["V" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["V" + index].Value = item.TKThueGTGT;

                            worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["W" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["W" + index].Value = item.TienThueGTGT;

                            worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["X" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["X" + index].Value = item.PhanTramThueGTGT;

                            worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["Y" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Y" + index].Value = item.GiaTriHHDVChuaThue;

                            worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["Z" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;                        
                            worksheet.Cells["Z" + index].Value = item.MauSoHopDong;

                            worksheet.Cells["AA" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AA" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["AA" + index].Value = item.NgayHoaDonDisplay;

                            worksheet.Cells["AB" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AB" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["AB" + index].Value = item.KyHieuHopDong;

                            worksheet.Cells["AC" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AC" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["AC" + index].Value = item.SoHoaDon;

                            worksheet.Cells["AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AD" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["AD" + index].Value = item.NhomHHDVMuaVao;

                            worksheet.Cells["AE" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AE" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["AE" + index].Value = item.MaDoiTuongThue;

                            worksheet.Cells["AF" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AF" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["AF" + index].Value = item.TenDoiTuongThue;

                            worksheet.Cells["AG" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AG" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["AG" + index].Value = item.MaSoThueDoiTuongThue;

                            stt++;
                            index++;
                        }
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}