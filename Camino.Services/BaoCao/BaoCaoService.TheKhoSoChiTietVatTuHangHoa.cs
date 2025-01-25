using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaoTheKhos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<List<LookupItemVo>> GetKhoHangHoa(LookupQueryInfo queryInfo)
        {
            var result = await _khoRepository.TableNoTracking
                .Select(s => new LookupItemVo
                {
                    KeyId = s.Id,
                    DisplayName = s.Ten
                }).ApplyLike(queryInfo.Query, o => o.DisplayName)
                .Take(queryInfo.Take).ToListAsync();

            var toanVien = new LookupItemVo { KeyId = 0, DisplayName = "Toàn viện" };
            result.Insert(0, toanVien);

            return result;
        }

        public async Task<List<LookupItemDuocPhamHoacVatTuVo>> GetKhoDuocPhamVatTuTheoKhoHangHoa(DropDownListRequestModel queryInfo, long khoId)
        {
            var query = _duocPhamBenhVienRepository.TableNoTracking
                .Where(o => khoId == 0 || o.NhapKhoDuocPhamChiTiets.Any(kho => kho.NhapKhoDuocPhams.KhoId == khoId))
                .ApplyLike(queryInfo.Query, g => g.DuocPham.Ten)
                .Select(s => new LookupItemDuocPhamHoacVatTuVo
                {
                    KeyId = s.Id,
                    DisplayName = s.DuocPham.Ten,
                    LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                    DuocPhamHoacVatTuBenhVienId = s.Id,

                })
                .Union(
                    _vatTuBenhVienRepository.TableNoTracking
                        .Where(o => khoId == 0 || o.NhapKhoVatTuChiTiets.Any(kho => kho.NhapKhoVatTu.KhoId == khoId))
                        .ApplyLike(queryInfo.Query, g => g.VatTus.Ten)
                        .Select(s => new LookupItemDuocPhamHoacVatTuVo
                        {
                            KeyId = s.Id,
                            DisplayName = s.VatTus.Ten,
                            LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                            DuocPhamHoacVatTuBenhVienId = s.Id,
                        })).OrderBy(o => o.DisplayName).Take(queryInfo.Take);

            return await query.ToListAsync();
        }

        private async Task<List<BaoCaoTheKhoSoChiTietVatTuHangHoaGridVo>> GetAllDataForBaoCaoTheKhoSoChiTietVatTuHangHoa(BaoCaoTheKhoSoChiTietVatTuHangHoaQueryInfo queryInfo)
        {
            var thongTinKhoaPhong = _KhoaPhongRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();

            if (queryInfo.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien)
            {
                var khoTongDuocPhamId = (long)EnumKhoDuocPham.KhoTongDuocPham;
                IQueryable<NhapKhoDuocPhamChiTiet> allDataNhapQuery = null;

                if (queryInfo.KhoId == 0)
                {
                    allDataNhapQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(o =>
                            o.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                            (o.NhapKhoDuocPhams.KhoId != khoTongDuocPhamId || o.KhoNhapSauKhiDuyetId == null) &&
                            o.NgayNhap <= queryInfo.ToDate);
                }
                else
                {
                    if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoTongDuocPham)
                    {
                        allDataNhapQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(o =>
                                o.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate);
                    }
                    else
                    {
                        allDataNhapQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(o =>
                                o.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId && o.KhoNhapSauKhiDuyetId == null &&
                                o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate);
                    }
                }

                    
                var allDataNhap = allDataNhapQuery
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        TenKho = o.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                        NhapTuKho = o.NhapKhoDuocPhams.XuatKhoDuocPhamId != null ? o.NhapKhoDuocPhams.XuatKhoDuocPham.KhoDuocPhamXuat.Ten : "",
                        SoHoaDon = o.NhapKhoDuocPhams.XuatKhoDuocPhamId == null ? o.NhapKhoDuocPhams.SoChungTu : "",
                        TenNhaThau = o.HopDongThauDuocPhams.NhaThau.Ten,
                        HopDongThauId = o.HopDongThauDuocPhamId,
                        SoHopDongThau = o.HopDongThauDuocPhams.SoHopDong,
                        //NhapTuNhaCungCap = o.HopDongThauDuocPhams.HeThongTuPhatSinh != null && o.HopDongThauDuocPhams.HeThongTuPhatSinh == true,
                        NgayThang = o.NgayNhap,
                        SCTNhap = o.NhapKhoDuocPhams.SoPhieu,
                        NhapTuXuatKhoId = o.NhapKhoDuocPhams.XuatKhoDuocPhamId,
                        DonGia = o.DonGiaTonKho,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

                var nhapTuXuatKhoIds = allDataNhap.Where(o => o.NhapTuXuatKhoId != null && o.NgayThang >= queryInfo.FromDate).Select(o => o.NhapTuXuatKhoId.Value).Distinct().ToList();
                var xuatKhoSauKhiDuyetIds = _xuatKhoDuocPhamRepository.TableNoTracking
                .Where(o => nhapTuXuatKhoIds.Contains(o.Id) && o.KhoXuatId == (long)EnumKhoDuocPham.KhoTongDuocPham && o.LyDoXuatKho == Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)
                .Select(o => o.Id).ToList();

                foreach (var baoCaoTheKhoChiTietData in allDataNhap)
                {
                    if (baoCaoTheKhoChiTietData.NhapTuXuatKhoId == null || xuatKhoSauKhiDuyetIds.Contains(baoCaoTheKhoChiTietData.NhapTuXuatKhoId.Value))
                    {
                        baoCaoTheKhoChiTietData.NhapTuNhaCungCap = true;
                    }
                }

                var groupDataNhap = allDataNhap.GroupBy(o => new
                {
                    SCTNhap = o.SCTNhap,
                    TenKho = o.TenKho,
                    NhapTuKho = o.NhapTuKho,
                    TenNhaThau = o.TenNhaThau,
                    HopDongThauId = o.HopDongThauId,
                    SoHopDongThau = o.SoHopDongThau,
                    NhapTuNhaCungCap = o.NhapTuNhaCungCap,
                    NgayThang = o.NgayThang,
                    DonGia = o.DonGia,
                }, o => o,
                    (k, v) => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = v.First().Id,
                        SCTNhap = k.SCTNhap,
                        NhapTuXuatKhoId = v.First().NhapTuXuatKhoId,
                        SoHoaDon = v.First().SoHoaDon,
                        TenKho = k.TenKho,
                        NhapTuKho = k.NhapTuKho,
                        TenNhaThau = k.TenNhaThau,
                        HopDongThauId = k.HopDongThauId,
                        SoHopDongThau = k.SoHopDongThau,
                        NhapTuNhaCungCap = k.NhapTuNhaCungCap,
                        NgayThang = k.NgayThang,
                        NgayThangCT = k.NgayThang,
                        SLNhap = v.Sum(x => x.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                        SLXuat = 0,
                        DonGia = k.DonGia
                    }).ToList();
                //var nhapTuXuatKhoIds = groupDataNhap.Where(o => o.NhapTuXuatKhoId != null && o.NgayThang >= queryInfo.FromDate).Select(o => o.NhapTuXuatKhoId.Value).ToList();
                var thongTinXuatKhos = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                nhapTuXuatKhoIds.Contains(o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId.Value))
                    .Select(o => new BaoCaoTheKhoThongTinXuatKhoDeNhapVe
                    {
                        XuatKhoId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId.Value,
                        KhoXuatId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId,
                        TenKhoXuat = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.Ten,
                        DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        HopDongThauId = o.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                        SoHoaDon = o.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.SoChungTu,
                        KhoKhacHoanTra = o.YeuCauTraDuocPhamChiTiets.Any()
                    }).ToList();

                IQueryable<XuatKhoDuocPhamChiTietViTri> allDataXuatQuery = null;

                if (queryInfo.KhoId == 0)
                {
                    
                    allDataXuatQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                        .Where(o => o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                    o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                    (o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId != khoTongDuocPhamId || o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)
                                    && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                        (o.NgayXuat == null &&
                                         o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)));
                }
                else
                {
                    if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoTongDuocPham)
                    {
                        allDataXuatQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                            .Where(o => o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                        o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                        o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId
                                        && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                            (o.NgayXuat == null &&
                                             o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)));
                    }
                    else
                    {
                        allDataXuatQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                            .Where(o => o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                        o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                        o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet
                                        && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                            (o.NgayXuat == null &&
                                             o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)));
                    }
                }

                

                var allDataXuat = allDataXuatQuery
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        XuatKhoDuocPhamChiTietId = o.XuatKhoDuocPhamChiTietId,
                        TenKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.Ten,
                        LoaiXuatKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LoaiXuatKho,
                        XuatQuaKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoNhapId != null
                            ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamNhap.Ten
                            : "",
                        NgayThang = o.NgayXuat != null
                            ? o.NgayXuat.Value
                            : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        NgayThangCT = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        SCTNhap = o.SoLuongXuat < 0 ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : string.Empty,
                        SCTXuat = o.SoLuongXuat > 0 ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : string.Empty,
                        SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                        SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                        DonGia = o.NhapKhoDuocPhamChiTiet.DonGiaTonKho,
                        //HoTenBenhNhan = o.DonThuocThanhToanChiTiets.Select(c => c.DonThuocThanhToan.YeuCauTiepNhan.HoTen).FirstOrDefault(),
                        //MaTiepNhan = o.DonThuocThanhToanChiTiets.Select(c => c.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan).FirstOrDefault(),
                        DonThuocThanhToanIds = o.DonThuocThanhToanChiTiets.Select(c => c.DonThuocThanhToanId).ToList(),
                        BenhNhanTraLai = o.SoLuongXuat < 0
                    }).ToList();

                var donThuocThanhToanIds = allDataXuat.SelectMany(o => o.DonThuocThanhToanIds).Select(o => o).Distinct().ToList();
                var thongTinDonThuocs = _donThuocThanhToanRepository.TableNoTracking
                    .Where(o => donThuocThanhToanIds.Contains(o.Id))
                    .Select(o => new
                    {
                        o.Id,
                        MaYeuCauTiepNhan = o.YeuCauTiepNhan != null ? o.YeuCauTiepNhan.MaYeuCauTiepNhan : "",
                        HoTen = o.YeuCauTiepNhan != null ? o.YeuCauTiepNhan.HoTen : ""
                    }).ToList();

                var xuatKhoChiTietIds = allDataXuat.Select(o => o.XuatKhoDuocPhamChiTietId).Distinct().ToList();
                var yeuCauDuocPhamXuatKhoChiTietData = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.XuatKhoDuocPhamChiTietId != null && xuatKhoChiTietIds.Contains(o.XuatKhoDuocPhamChiTietId.Value))
                    .Select(o => new { o.NoiChiDinh.KhoaPhongId, o.XuatKhoDuocPhamChiTietId, o.LoaiPhieuLinh, o.YeuCauTiepNhan.MaYeuCauTiepNhan, o.YeuCauTiepNhan.HoTen }).ToList();

                foreach (var baoCaoTheKhoChiTietGridVo in allDataXuat)
                {
                    if (baoCaoTheKhoChiTietGridVo.DonThuocThanhToanIds.Any())
                    {
                        var thongTinDonThuoc = thongTinDonThuocs.FirstOrDefault(o => baoCaoTheKhoChiTietGridVo.DonThuocThanhToanIds.Contains(o.Id));
                        if (thongTinDonThuoc != null)
                        {
                            baoCaoTheKhoChiTietGridVo.HoTenBenhNhan = thongTinDonThuoc.HoTen;
                            baoCaoTheKhoChiTietGridVo.MaTiepNhan = thongTinDonThuoc.MaYeuCauTiepNhan;
                        }
                    }
                    var yeuCauDuocPhamXuatKhoChiTiet = yeuCauDuocPhamXuatKhoChiTietData.FirstOrDefault(o => o.XuatKhoDuocPhamChiTietId == baoCaoTheKhoChiTietGridVo.XuatKhoDuocPhamChiTietId);
                    if (yeuCauDuocPhamXuatKhoChiTiet != null)
                    {
                        baoCaoTheKhoChiTietGridVo.HoTenBenhNhan = yeuCauDuocPhamXuatKhoChiTiet.HoTen;
                        baoCaoTheKhoChiTietGridVo.MaTiepNhan = yeuCauDuocPhamXuatKhoChiTiet.MaYeuCauTiepNhan;
                        if (yeuCauDuocPhamXuatKhoChiTiet.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                        {
                            baoCaoTheKhoChiTietGridVo.KhoaLinh = thongTinKhoaPhong.FirstOrDefault(o => o.Id == yeuCauDuocPhamXuatKhoChiTiet.KhoaPhongId)?.Ten;
                        }
                    }
                }

                var groupDataXuat = allDataXuat.GroupBy(o => new
                {
                    SCTNhap = o.SCTNhap,
                    SCTXuat = o.SCTXuat,
                    DonGia = o.DonGia,
                    TenKho = o.TenKho,
                    LoaiXuatKho = o.LoaiXuatKho,
                    XuatQuaKho = o.XuatQuaKho,
                    NgayThang = o.NgayThang,
                    MaTiepNhan = o.MaTiepNhan,
                    BenhNhanTraLai = o.BenhNhanTraLai,
                }, o => o,
                    (k, v) => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = v.First().Id,
                        SCTNhap = k.SCTNhap,
                        SCTXuat = k.SCTXuat,
                        DonGia = k.DonGia,
                        TenKho = k.TenKho,
                        LoaiXuatKho = k.LoaiXuatKho,
                        XuatQuaKho = k.XuatQuaKho,
                        NgayThang = k.NgayThang,
                        NgayThangCT = v.First().NgayThangCT,
                        SLNhap = v.Sum(x => x.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                        SLXuat = v.Sum(x => x.SLXuat.GetValueOrDefault()).MathRoundNumber(2),
                        HoTenBenhNhan = v.First().HoTenBenhNhan,
                        MaTiepNhan = v.First().MaTiepNhan,
                        KhoaLinh = v.First().KhoaLinh,
                        BenhNhanTraLai = k.BenhNhanTraLai
                    }).ToList();

                var allDataNhapXuat = groupDataNhap.Concat(groupDataXuat).OrderBy(o => o.NgayThang).ToList();
                var tonDauKy = allDataNhapXuat.Where(o => o.NgayThang < queryInfo.FromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var thanhTienTonDauKy = allDataNhapXuat.Where(o => o.NgayThang < queryInfo.FromDate)
                    .Select(o => ((decimal)o.SLNhap.GetValueOrDefault() * o.DonGia) - ((decimal)o.SLXuat.GetValueOrDefault() * o.DonGia)).DefaultIfEmpty(0).Sum();
                foreach (var item in allDataNhapXuat)
                {
                    if (item.NgayThang >= queryInfo.FromDate && item.NhapTuXuatKhoId != null)
                    {
                        var thongTinXuatKho = thongTinXuatKhos.FirstOrDefault(o => o.XuatKhoId == item.NhapTuXuatKhoId && o.HopDongThauId == item.HopDongThauId);
                        if (thongTinXuatKho != null)
                        {
                            item.KhoKhacHoanTra = thongTinXuatKho.KhoKhacHoanTra;
                            item.SoHoaDon = thongTinXuatKho.SoHoaDon;
                        }
                    }
                }

                var allDataNhapXuatTuNgay = allDataNhapXuat.Where(o => o.NgayThang >= queryInfo.FromDate).Select(o => new BaoCaoTheKhoSoChiTietVatTuHangHoaGridVo
                {
                    SoChungTu = o.SCTNhap + o.SCTXuat,
                    NgayXuatNhap = o.NgayThang,
                    NgayChungTu = o.NgayThangCT,
                    DienGiai = o.DienGiai,
                    DonGia = o.DonGia,
                    SoLuongNhap = o.SLNhap.GetValueOrDefault(),
                    SoLuongXuat = o.SLXuat.GetValueOrDefault()
                }).ToList();

                for (int i = 0; i < allDataNhapXuatTuNgay.Count; i++)
                {
                    if (i == 0)
                    {
                        allDataNhapXuatTuNgay[i].SoLuongTon = (tonDauKy + allDataNhapXuatTuNgay[i].SoLuongNhap - allDataNhapXuatTuNgay[i].SoLuongXuat).MathRoundNumber(2);
                        allDataNhapXuatTuNgay[i].ThanhTienTon = thanhTienTonDauKy + ((decimal)allDataNhapXuatTuNgay[i].SoLuongNhap * allDataNhapXuatTuNgay[i].DonGia) - ((decimal)allDataNhapXuatTuNgay[i].SoLuongXuat * allDataNhapXuatTuNgay[i].DonGia);
                    }
                    else
                    {
                        allDataNhapXuatTuNgay[i].SoLuongTon = (allDataNhapXuatTuNgay[i - 1].SoLuongTon + allDataNhapXuatTuNgay[i].SoLuongNhap - allDataNhapXuatTuNgay[i].SoLuongXuat).MathRoundNumber(2);
                        allDataNhapXuatTuNgay[i].ThanhTienTon = allDataNhapXuatTuNgay[i - 1].ThanhTienTon + ((decimal)allDataNhapXuatTuNgay[i].SoLuongNhap * allDataNhapXuatTuNgay[i].DonGia) - ((decimal)allDataNhapXuatTuNgay[i].SoLuongXuat * allDataNhapXuatTuNgay[i].DonGia);
                    }
                }
                allDataNhapXuatTuNgay.Insert(0, new BaoCaoTheKhoSoChiTietVatTuHangHoaGridVo
                {
                    NgayXuatNhap = queryInfo.FromDate,
                    DienGiai = "Đầu kỳ",
                    SoLuongTon = tonDauKy,
                    ThanhTienTon = thanhTienTonDauKy
                });
                return allDataNhapXuatTuNgay;
            }
            else if (queryInfo.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien)
            {
                var khoVatTuYTeId = (long) EnumKhoDuocPham.KhoVatTuYTe;
                IQueryable<NhapKhoVatTuChiTiet> allDataNhapQuery = null;

                if (queryInfo.KhoId == 0)
                {
                    allDataNhapQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(o =>
                            o.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                            (o.NhapKhoVatTu.KhoId != khoVatTuYTeId || o.KhoNhapSauKhiDuyetId == null) && 
                            o.NgayNhap <= queryInfo.ToDate);
                }
                else
                {
                    if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoVatTuYTe)
                    {
                        allDataNhapQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(o =>
                                o.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate);
                    }
                    else
                    {
                        allDataNhapQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(o =>
                                o.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId && o.KhoNhapSauKhiDuyetId == null &&
                                o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate);
                    }
                }

                    

                var allDataNhap = allDataNhapQuery
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        TenKho = o.NhapKhoVatTu.Kho.Ten,
                        NhapTuKho = o.NhapKhoVatTu.XuatKhoVatTuId != null ? o.NhapKhoVatTu.XuatKhoVatTu.KhoVatTuXuat.Ten : "",
                        SoHoaDon = o.NhapKhoVatTu.XuatKhoVatTuId == null ? o.NhapKhoVatTu.SoChungTu : "",
                        TenNhaThau = o.HopDongThauVatTu.NhaThau.Ten,
                        HopDongThauId = o.HopDongThauVatTuId,
                        SoHopDongThau = o.HopDongThauVatTu.SoHopDong,
                        //NhapTuNhaCungCap = o.HopDongThauVatTu.HeThongTuPhatSinh != null && o.HopDongThauVatTu.HeThongTuPhatSinh == true,
                        NgayThang = o.NgayNhap,
                        SCTNhap = o.NhapKhoVatTu.SoPhieu,
                        NhapTuXuatKhoId = o.NhapKhoVatTu.XuatKhoVatTuId,
                        DonGia = o.DonGiaTonKho,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

                var nhapTuXuatKhoIds = allDataNhap.Where(o => o.NhapTuXuatKhoId != null && o.NgayThang >= queryInfo.FromDate).Select(o => o.NhapTuXuatKhoId.Value).Distinct().ToList();
                var xuatKhoSauKhiDuyetIds = _xuatKhoVatTuRepository.TableNoTracking
                .Where(o => nhapTuXuatKhoIds.Contains(o.Id) && o.KhoXuatId == (long)EnumKhoDuocPham.KhoVatTuYTe && o.LyDoXuatKho == Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)
                .Select(o => o.Id).ToList();

                foreach (var baoCaoTheKhoChiTietData in allDataNhap)
                {
                    if (baoCaoTheKhoChiTietData.NhapTuXuatKhoId == null || xuatKhoSauKhiDuyetIds.Contains(baoCaoTheKhoChiTietData.NhapTuXuatKhoId.Value))
                    {
                        baoCaoTheKhoChiTietData.NhapTuNhaCungCap = true;
                    }
                }

                var groupDataNhap = allDataNhap.GroupBy(o => new
                {
                    SCTNhap = o.SCTNhap,
                    TenKho = o.TenKho,
                    NhapTuKho = o.NhapTuKho,
                    TenNhaThau = o.TenNhaThau,
                    HopDongThauId = o.HopDongThauId,
                    SoHopDongThau = o.SoHopDongThau,
                    NhapTuNhaCungCap = o.NhapTuNhaCungCap,
                    NgayThang = o.NgayThang,
                    DonGia = o.DonGia,
                }, o => o,
                    (k, v) => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = v.First().Id,
                        SCTNhap = k.SCTNhap,
                        NhapTuXuatKhoId = v.First().NhapTuXuatKhoId,
                        SoHoaDon = v.First().SoHoaDon,
                        TenKho = k.TenKho,
                        NhapTuKho = k.NhapTuKho,
                        TenNhaThau = k.TenNhaThau,
                        HopDongThauId = k.HopDongThauId,
                        SoHopDongThau = k.SoHopDongThau,
                        NhapTuNhaCungCap = k.NhapTuNhaCungCap,
                        NgayThang = k.NgayThang,
                        NgayThangCT = k.NgayThang,
                        SLNhap = v.Sum(x => x.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                        SLXuat = 0,
                        DonGia = k.DonGia
                    }).ToList();
                //var nhapTuXuatKhoIds = groupDataNhap.Where(o => o.NhapTuXuatKhoId != null && o.NgayThang >= queryInfo.FromDate).Select(o => o.NhapTuXuatKhoId.Value).ToList();
                var thongTinXuatKhos = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                    .Where(o => o.NhapKhoVatTuChiTiet.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                nhapTuXuatKhoIds.Contains(o.XuatKhoVatTuChiTiet.XuatKhoVatTuId.Value))
                    .Select(o => new BaoCaoTheKhoThongTinXuatKhoDeNhapVe
                    {
                        XuatKhoId = o.XuatKhoVatTuChiTiet.XuatKhoVatTuId.Value,
                        KhoXuatId = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId,
                        TenKhoXuat = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat.Ten,
                        HopDongThauId = o.NhapKhoVatTuChiTiet.HopDongThauVatTuId,
                        SoHoaDon = o.NhapKhoVatTuChiTiet.NhapKhoVatTu.SoChungTu,
                        KhoKhacHoanTra = o.YeuCauTraVatTuChiTiets.Any()
                    }).ToList();

                IQueryable<XuatKhoVatTuChiTietViTri> allDataXuatQuery = null;
                if (queryInfo.KhoId == 0)
                {
                    allDataXuatQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                        .Where(o => o.NhapKhoVatTuChiTiet.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                    o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                    (o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId != khoVatTuYTeId || o.XuatKhoVatTuChiTiet.XuatKhoVatTu.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)
                                    && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                        (o.NgayXuat == null &&
                                         o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)));
                }
                else
                {
                    if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoVatTuYTe)
                    {
                        allDataXuatQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTuChiTiet.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                        o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                        o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId
                                        && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                            (o.NgayXuat == null &&
                                             o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)));
                    }
                    else
                    {
                        allDataXuatQuery = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTuChiTiet.VatTuBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                        o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                                        o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet
                                        && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                            (o.NgayXuat == null &&
                                             o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)));
                    }
                }

                var allDataXuat = allDataXuatQuery
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        TenKho = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuXuat.Ten,
                        LoaiXuatKhoVatTu = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.LoaiXuatKho,
                        XuatQuaKho = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoNhapId != null
                            ? o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoVatTuNhap.Ten
                            : "",
                        NgayThang = o.NgayXuat != null
                            ? o.NgayXuat.Value
                            : o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                        NgayThangCT = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                        SCTNhap = o.SoLuongXuat < 0 ? o.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : string.Empty,
                        SCTXuat = o.SoLuongXuat > 0 ? o.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : string.Empty,
                        SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                        SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                        DonGia = o.NhapKhoVatTuChiTiet.DonGiaTonKho,
                        //HoTenBenhNhan = o.DonVTYTThanhToanChiTiets.Select(c => c.DonVTYTThanhToan.YeuCauTiepNhan.HoTen).FirstOrDefault(),
                        //MaTiepNhan = o.DonVTYTThanhToanChiTiets.Select(c => c.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan).FirstOrDefault(),
                        BenhNhanTraLai = o.SoLuongXuat < 0
                    }).ToList();

                var groupDataXuat = allDataXuat.GroupBy(o => new
                {
                    SCTNhap = o.SCTNhap,
                    SCTXuat = o.SCTXuat,
                    DonGia = o.DonGia,
                    TenKho = o.TenKho,
                    LoaiXuatKhoVatTu = o.LoaiXuatKhoVatTu,
                    XuatQuaKho = o.XuatQuaKho,
                    NgayThang = o.NgayThang,
                    BenhNhanTraLai = o.BenhNhanTraLai,
                }, o => o,
                    (k, v) => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = v.First().Id,
                        SCTNhap = k.SCTNhap,
                        SCTXuat = k.SCTXuat,
                        DonGia = k.DonGia,
                        TenKho = k.TenKho,
                        LoaiXuatKhoVatTu = k.LoaiXuatKhoVatTu,
                        XuatQuaKho = k.XuatQuaKho,
                        NgayThang = k.NgayThang,
                        NgayThangCT = v.First().NgayThangCT,
                        SLNhap = v.Sum(x => x.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                        SLXuat = v.Sum(x => x.SLXuat.GetValueOrDefault()).MathRoundNumber(2),
                        HoTenBenhNhan = v.First().HoTenBenhNhan,
                        MaTiepNhan = v.First().MaTiepNhan,
                        BenhNhanTraLai = k.BenhNhanTraLai
                    }).ToList();

                var allDataNhapXuat = groupDataNhap.Concat(groupDataXuat).OrderBy(o => o.NgayThang).ToList();
                var tonDauKy = allDataNhapXuat.Where(o => o.NgayThang < queryInfo.FromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var thanhTienTonDauKy = allDataNhapXuat.Where(o => o.NgayThang < queryInfo.FromDate)
                    .Select(o => ((decimal)o.SLNhap.GetValueOrDefault() * o.DonGia) - ((decimal)o.SLXuat.GetValueOrDefault() * o.DonGia)).DefaultIfEmpty(0).Sum();
                foreach (var item in allDataNhapXuat)
                {
                    if (item.NgayThang >= queryInfo.FromDate && item.NhapTuXuatKhoId != null)
                    {
                        var thongTinXuatKho = thongTinXuatKhos.FirstOrDefault(o => o.XuatKhoId == item.NhapTuXuatKhoId && o.HopDongThauId == item.HopDongThauId);
                        if (thongTinXuatKho != null)
                        {
                            item.KhoKhacHoanTra = thongTinXuatKho.KhoKhacHoanTra;
                            item.SoHoaDon = thongTinXuatKho.SoHoaDon;
                        }
                    }
                }

                var allDataNhapXuatTuNgay = allDataNhapXuat.Where(o => o.NgayThang >= queryInfo.FromDate).Select(o => new BaoCaoTheKhoSoChiTietVatTuHangHoaGridVo
                {
                    SoChungTu = o.SCTNhap + o.SCTXuat,
                    NgayXuatNhap = o.NgayThang,
                    NgayChungTu = o.NgayThangCT,
                    DienGiai = o.DienGiai,
                    DonGia = o.DonGia,
                    SoLuongNhap = o.SLNhap.GetValueOrDefault(),
                    SoLuongXuat = o.SLXuat.GetValueOrDefault()
                }).ToList();

                for (int i = 0; i < allDataNhapXuatTuNgay.Count; i++)
                {
                    if (i == 0)
                    {
                        allDataNhapXuatTuNgay[i].SoLuongTon = (tonDauKy + allDataNhapXuatTuNgay[i].SoLuongNhap - allDataNhapXuatTuNgay[i].SoLuongXuat).MathRoundNumber(2);
                        allDataNhapXuatTuNgay[i].ThanhTienTon = thanhTienTonDauKy + ((decimal)allDataNhapXuatTuNgay[i].SoLuongNhap * allDataNhapXuatTuNgay[i].DonGia) - ((decimal)allDataNhapXuatTuNgay[i].SoLuongXuat * allDataNhapXuatTuNgay[i].DonGia);
                    }
                    else
                    {
                        allDataNhapXuatTuNgay[i].SoLuongTon = (allDataNhapXuatTuNgay[i - 1].SoLuongTon + allDataNhapXuatTuNgay[i].SoLuongNhap - allDataNhapXuatTuNgay[i].SoLuongXuat).MathRoundNumber(2);
                        allDataNhapXuatTuNgay[i].ThanhTienTon = allDataNhapXuatTuNgay[i - 1].ThanhTienTon + ((decimal)allDataNhapXuatTuNgay[i].SoLuongNhap * allDataNhapXuatTuNgay[i].DonGia) - ((decimal)allDataNhapXuatTuNgay[i].SoLuongXuat * allDataNhapXuatTuNgay[i].DonGia);
                    }
                }
                allDataNhapXuatTuNgay.Insert(0, new BaoCaoTheKhoSoChiTietVatTuHangHoaGridVo
                {
                    NgayXuatNhap = queryInfo.FromDate,
                    DienGiai = "Đầu kỳ",
                    SoLuongTon = tonDauKy,
                    ThanhTienTon = thanhTienTonDauKy
                });
                return allDataNhapXuatTuNgay;
            }
            return null;
        }

        public async Task<GridDataSource> GetDataBaoCaoTheKhoSoChiTietVatTuHangHoaForGridAsync(BaoCaoTheKhoSoChiTietVatTuHangHoaQueryInfo queryInfo)
        {
            var allData = await GetAllDataForBaoCaoTheKhoSoChiTietVatTuHangHoa(queryInfo);
            return new GridDataSource { Data = allData.ToArray(), TotalRowCount = allData.Count() };
        }

        public virtual byte[] ExportBaoCaoBangTheKhoSoChiTietVatTuHangHoaGridVo(GridDataSource gridDataSource, BaoCaoTheKhoSoChiTietVatTuHangHoaQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTheKhoSoChiTietVatTuHangHoaGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTheKhoSoChiTietVatTuHangHoaGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("SỔ CHI TIẾT TIẾT VẬT TƯ HÀNG HÓA");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel                   
                    using (var range = worksheet.Cells["A3:L3"])
                    {
                        range.Worksheet.Cells["A3:L3"].Merge = true;
                        range.Worksheet.Cells["A3:L3"].Value = "SỔ CHI TIẾT VẬT TƯ HÀNG HÓA";
                        range.Worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:L3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:L3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:L4"])
                    {
                        range.Worksheet.Cells["A4:L4"].Merge = true;
                        range.Worksheet.Cells["A4:L4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                     + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:L4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                    }

                    //get vật tư - dược from filter
                    var maDuoc = string.Empty;
                    var tenDuoc = string.Empty;
                    var dvt = string.Empty;
                    if (query.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien)
                    {
                        var duocPhamBenhVien = _duocPhamBenhVienRepository.TableNoTracking.Where(c => c.Id == query.DuocPhamHoacVatTuBenhVienId)
                                                                            .Include(cv => cv.DuocPham).ThenInclude(c => c.DonViTinh)
                                                                            .FirstOrDefault();

                        maDuoc = duocPhamBenhVien.MaDuocPhamBenhVien;
                        tenDuoc = duocPhamBenhVien.DuocPham.Ten;
                        dvt = duocPhamBenhVien.DuocPham.DonViTinh.Ten;
                    }

                    if (query.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien)
                    {
                        var duocPhamBenhVien = _vatTuBenhVienRepository.TableNoTracking.Where(c => c.Id == query.DuocPhamHoacVatTuBenhVienId)
                                                                       .Include(c => c.VatTus).FirstOrDefault();

                        maDuoc = duocPhamBenhVien.MaVatTuBenhVien;
                        tenDuoc = duocPhamBenhVien.VatTus.Ten;
                        dvt = duocPhamBenhVien.VatTus.DonViTinh;
                    }



                    using (var range = worksheet.Cells["A6:D6"])
                    {
                        range.Worksheet.Cells["A6:D6"].Merge = true;
                        range.Worksheet.Cells["A6:D6"].Value = "Tên: " + maDuoc + " - " + tenDuoc;
                        range.Worksheet.Cells["A6:D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:D6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:D6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:D6"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["E6:F6"])
                    {
                        range.Worksheet.Cells["E6:F6"].Merge = true;
                        range.Worksheet.Cells["E6:F6"].Value = "Mã dược: " + maDuoc;
                        range.Worksheet.Cells["E6:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E6:F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E6:F6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["E6:F6"].Style.Font.Color.SetColor(Color.Black);
                    }

                    worksheet.Cells["H6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["H6"].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["H6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["H6"].Value = "ĐVT: " + dvt;


                    using (var range = worksheet.Cells["A8:L8"])
                    {
                        range.Worksheet.Cells["A8:L8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:L8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:L8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A8:L8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:L8"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A8:L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A9:L9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A9:L9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A9:L9"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A9:L9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A9:L9"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A9:L9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A8:A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A8:A9"].Merge = true;
                        range.Worksheet.Cells["A8:A9"].Value = "STT";

                        range.Worksheet.Cells["B8:B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B8:B9"].Merge = true;
                        range.Worksheet.Cells["B8:B9"].Value = "Ngày";

                        range.Worksheet.Cells["C8:D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C8:D8"].Merge = true;
                        range.Worksheet.Cells["C8:D8"].Value = "Chứng từ";

                        range.Worksheet.Cells["C9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C9"].Value = "Số hiệu";
                        range.Worksheet.Cells["D9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D9"].Value = "Ngày tháng";

                        range.Worksheet.Cells["E8:E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E8:E9"].Merge = true;
                        range.Worksheet.Cells["E8:E9"].Value = "Diễn giải";

                        range.Worksheet.Cells["F8:F9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F8:F9"].Merge = true;
                        range.Worksheet.Cells["F8:F9"].Value = "Đơn giá";

                        range.Worksheet.Cells["G8:H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G8:H8"].Merge = true;
                        range.Worksheet.Cells["G8:H8"].Value = "Nhập";

                        range.Worksheet.Cells["G9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G9"].Value = "Số Lượng";
                        range.Worksheet.Cells["H9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H9"].Value = "Thành Tiền";

                        range.Worksheet.Cells["I8:J8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I8:J8"].Merge = true;
                        range.Worksheet.Cells["I8:J8"].Value = "Xuất";

                        range.Worksheet.Cells["I9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I9"].Value = "Số Lượng";
                        range.Worksheet.Cells["J9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J9"].Value = "Thành Tiền";

                        range.Worksheet.Cells["K8:L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K8:L8"].Merge = true;
                        range.Worksheet.Cells["K8:L8"].Value = "Tồn";

                        range.Worksheet.Cells["K9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K9"].Value = "Số Lượng";
                        range.Worksheet.Cells["L9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L9"].Value = "Thành Tiền";
                    }

                    //write data from line 10
                    int index = 10;

                    //var listMonths = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    if (datas.Any())
                    {
                        foreach (var month in datas.GroupBy(o => new { o.NgayXuatNhap.Year, o.NgayXuatNhap.Month }).OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month))
                        {
                            var listDataTheoThang = month.ToList();
                            if (listDataTheoThang.Any())
                            {
                                var stt = 1;
                                foreach (var item in listDataTheoThang)
                                {
                                    // format border, font chữ,....
                                    worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                                    worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Row(index).Height = 20.5;



                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells["A" + index].Value = stt;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["B" + index].Value = item.NgayXuatNhapStr;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = item.SoChungTu;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = item.NgayChungTuStr;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Value = item.DienGiai;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["F" + index].Value = item.DonGia != 0 ? item.DonGia : (decimal?)null;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Value = item.SoLuongNhap != 0 ? item.SoLuongNhap : (double?)null; 

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Value = item.ThanhTienNhap != 0 ? item.ThanhTienNhap : (decimal?)null;

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Value = item.SoLuongXuat != 0 ? item.SoLuongXuat : (double?)null; 

                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Value = item.ThanhTienXuat != 0 ? item.ThanhTienXuat : (decimal?)null;

                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Value = item.SoLuongTon != 0 ? item.SoLuongTon : (double?)null; 

                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["L" + index].Value = item.ThanhTienTon != 0 ? item.ThanhTienTon : (decimal?)null;
                                    stt++;
                                    index++;
                                }

                                using (var range = worksheet.Cells["A" + index + ":F" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":F" + index].Merge = true;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Value = "Tổng cộng tháng: " + listDataTheoThang.FirstOrDefault().NgayXuatNhap.Month;
                                }

                                //sum nhập
                                worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["G" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["G" + index].Style.Font.Bold = true;
                                worksheet.Cells["G" + index].Value = listDataTheoThang.Sum(x => x.SoLuongNhap);

                                worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["H" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["H" + index].Style.Font.Bold = true;
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Value = listDataTheoThang.Sum(x => x.ThanhTienNhap);

                                //sum xuất
                                worksheet.Cells["I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["I" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["I" + index].Style.Font.Bold = true;
                                worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["I" + index].Value = listDataTheoThang.Sum(x => x.SoLuongXuat);

                                worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["J" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["J" + index].Style.Font.Bold = true;
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Value = listDataTheoThang.Sum(x => x.ThanhTienXuat);

                                //sum tồn
                                worksheet.Cells["K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["K" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["K" + index].Style.Font.Bold = true;
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Value = listDataTheoThang.LastOrDefault()?.SoLuongTon;
                               

                                worksheet.Cells["L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["L" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["L" + index].Style.Font.Bold = true;
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Value = listDataTheoThang.LastOrDefault()?.ThanhTienTon;     

                                index++;
                            }

                        }
                    }


                    using (var range = worksheet.Cells["A" + index + ":F" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":F" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + index + ":F" + index].Value = "Tổng cộng";
                    }

                    //sum nhập
                    worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["G" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["G" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["G" + index].Value = datas.Sum(x => x.SoLuongNhap);

                    worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["H" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["H" + index].Style.Font.Bold = true;
                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["H" + index].Value = datas.Sum(x => x.ThanhTienNhap);

                    //sum xuất
                    worksheet.Cells["I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["I" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["I" + index].Style.Font.Bold = true;
                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["I" + index].Value = datas.Sum(x => x.SoLuongXuat);

                    worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["J" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["J" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["J" + index].Value = datas.Sum(x => x.ThanhTienXuat);

                    //sum tồn
                    worksheet.Cells["K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["K" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["K" + index].Style.Font.Bold = true;
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Value = datas.Select(x => x.SoLuongTon).LastOrDefault(); 

                    worksheet.Cells["L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["L" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["L" + index].Style.Font.Bold = true;
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["L" + index].Value = datas.Select(x => x.ThanhTienTon).LastOrDefault(); 
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}