using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Newtonsoft.Json;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.Grid;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using System.Globalization;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial class YeuCauLinhDuocPhamService
    {
        public async Task<GridDataSource> GetYeuCauDuocPhamBenhVienDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<DuocPhamBenhVienJsonVo>(queryInfo.AdditionalSearchString);
            DateTime tuNgay = new DateTime(1970, 1, 1);
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(info.ThoiDiemChiDinhTu) || !string.IsNullOrEmpty(info.ThoiDiemChiDinhDen))
            {
                info.ThoiDiemChiDinhTu.TryParseExactCustom(out tuNgay);
                if (!string.IsNullOrEmpty(info.ThoiDiemChiDinhDen))
                {
                    info.ThoiDiemChiDinhDen.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            }
            if (info.IsCreate)
            {
                BuildDefaultSortExpression(queryInfo);
                var queryData = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(x => x.KhoLinhId == info.LinhVeKhoId
                                && x.YeuCauLinhDuocPhamId == null
                                && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                                && x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                && x.KhongLinhBu != true
                                && x.SoLuong > 0
                                && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                                && ((x.NoiTruPhieuDieuTri != null
                                    && x.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay
                                    && x.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay)
                                    || (x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                )
                    .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                    {
                        Id = item.Id,
                        DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                        LaBHYT = item.LaDuocPhamBHYT,
                        TenDuocPham = item.Ten,
                        NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                        HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                        DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        SoLuongCanBu = item.SoLuong,
                        SoLuongDaBu = item.SoLuongDaLinhBu,
                    }).ToList();

                var yeuCauLinhDuocPhamBuGridParentVos = queryData.GroupBy(x => new { x.YeuCauLinhDuocPhamId, x.DuocPhamBenhVienId, x.LaBHYT, x.Nhom, x.NongDoHamLuong, x.HoatChat, x.DuongDung, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                    .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                    {
                        YeuCauLinhDuocPhamIdstring = string.Join(",", item.Select(x => x.Id)),
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenDuocPham = item.First().TenDuocPham,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                        SoLuongDaBu = item.Sum(x => x.SoLuongDaBu),
                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct().ToList();

                var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == info.LinhTuKhoId
                         && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                var result = yeuCauLinhDuocPhamBuGridParentVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                result = result.Select(o =>
                {
                    o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                    o.SoLuongYeuCau = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SoLuongDaBu).MathRoundNumber(2));

                    o.SLYeuCauLinhThucTe = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathCelling() : o.SoLuongCanBu.MathCelling())
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathCelling() : (o.SoLuongCanBu - o.SoLuongDaBu).MathCelling());
                    return o;
                });

                var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                var queryTask = result.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }
            else
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                          .Where(p => p.YeuCauLinhDuocPhamId == info.YeuCauLinhDuocPhamId
                              && p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                               && (p.YeuCauDuocPhamBenhVien != null
                                && ((p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null
                                    && p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay
                                    && p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay)
                                    || (p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri == null && p.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh >= tuNgay && p.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh <= denNgay))
                                )
                              )
                          .Select(s => new DuocPhamLinhBuGridVo
                          {
                              Id = s.Id,
                              YeuCauLinhDuocPhamId = s.YeuCauLinhDuocPhamId,
                              DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                              TenDuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                              HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                              HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                              DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                              DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                              HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                              NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                              LaBHYT = s.LaDuocPhamBHYT,
                              Nhom = s.LaDuocPhamBHYT == true ? "Thuốc BHYT" : "Thuốc Không BHYT",
                              SoLuongCanBu = s.SoLuongCanBu,
                              SoLuongDaBu = s.SoLuongDaLinhBu,
                              SoLuongDuocDuyet = s.SoLuong,
                              SLYeuCauLinhThucTe = s.SoLuong
                          }).GroupBy(x => new { x.YeuCauLinhDuocPhamId, x.DuocPhamBenhVienId, x.LaBHYT, x.Nhom, x.HamLuong, x.HoatChat, x.DuongDung, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                            .Select(item => new DuocPhamLinhBuGridVo()
                            {
                                DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                LaBHYT = item.First().LaBHYT,
                                TenDuocPham = item.First().TenDuocPham,
                                Nhom = item.First().Nhom,
                                HamLuong = item.First().HamLuong,
                                HoatChat = item.First().HoatChat,
                                DuongDung = item.First().DuongDung,
                                DonViTinh = item.First().DonViTinh,
                                HangSanXuat = item.First().HangSanXuat,
                                NuocSanXuat = item.First().NuocSanXuat,
                                SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                                SoLuongDaBu = item.Sum(x => x.SoLuongDaBu),
                                SoLuongDuocDuyet = item.Sum(x => x.SoLuongDuocDuyet),
                                SLYeuCauLinhThucTe = Convert.ToInt32(item.Sum(x => x.SLYeuCauLinhThucTe).MathCelling()),
                            })
                            .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
                var duocPhamLinhBuGridVos = query.ToList();

                var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                    .Where(p => p.NhapKhoDuocPhams.KhoId == info.LinhTuKhoId)
                                    //&& p.SoLuongDaXuat < p.SoLuongNhap)
                                    .ToList();

                var result = duocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));
                result = result.Select(o =>
                {
                    o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                    o.SoLuongYeuCau = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                    : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SoLuongDaBu).MathRoundNumber(2));
                    return o;
                });
                var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                var queryTask = result.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }

        }
        public async Task<GridDataSource> GetYeuCauDuocPhamBenhVienDataForGridAsyncOld(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<DuocPhamBenhVienJsonVo>(queryInfo.AdditionalSearchString);
            DateTime tuNgay = new DateTime(1970, 1, 1);
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(info.ThoiDiemChiDinhTu) || !string.IsNullOrEmpty(info.ThoiDiemChiDinhDen))
            {
                info.ThoiDiemChiDinhTu.TryParseExactCustom(out tuNgay);
                if (!string.IsNullOrEmpty(info.ThoiDiemChiDinhDen))
                {
                    info.ThoiDiemChiDinhDen.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            }
            if (info.IsCreate)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(x => x.KhoLinhId == info.LinhVeKhoId
                                && x.YeuCauLinhDuocPhamId == null
                                && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                                && x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                && x.KhongLinhBu != true
                                && x.SoLuong > 0
                                && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                                && ((x.NoiTruPhieuDieuTri != null
                                    && x.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay
                                    && x.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay)
                                    || (x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                )
                    .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                    {
                        Id = item.Id,
                        DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                        LaBHYT = item.LaDuocPhamBHYT,
                        TenDuocPham = item.Ten,
                        NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                        HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                        DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        SoLuongCanBu = item.SoLuong,
                        SoLuongDaBu = item.SoLuongDaLinhBu
                    })
                    .GroupBy(x => new { x.YeuCauLinhDuocPhamId, x.DuocPhamBenhVienId, x.LaBHYT, x.Nhom, x.NongDoHamLuong, x.HoatChat, x.DuongDung, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                    .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                    {
                        YeuCauLinhDuocPhamIdstring = string.Join(",", item.Select(x => x.Id)),
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenDuocPham = item.First().TenDuocPham,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                        SoLuongDaBu = item.Sum(x => x.SoLuongDaBu),
                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
                var yeuCauLinhDuocPhamBuGridParentVos = query.ToList();

                var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == info.LinhTuKhoId
                         && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                var result = yeuCauLinhDuocPhamBuGridParentVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                result = result.Select(o =>
                {
                    o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                    o.SoLuongYeuCau = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SoLuongDaBu).MathRoundNumber(2));

                    o.SLYeuCauLinhThucTe = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathCelling() : o.SoLuongCanBu.MathCelling())
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathCelling() : (o.SoLuongCanBu - o.SoLuongDaBu).MathCelling());
                    return o;
                });

                var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                var queryTask = result.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }
            else
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                          .Where(p => p.YeuCauLinhDuocPhamId == info.YeuCauLinhDuocPhamId
                              && p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                               && (p.YeuCauDuocPhamBenhVien != null
                                && ((p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null
                                    && p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay
                                    && p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay)
                                    || (p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri == null && p.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh >= tuNgay && p.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh <= denNgay))
                                )
                              )
                          .Select(s => new DuocPhamLinhBuGridVo
                          {
                              Id = s.Id,
                              YeuCauLinhDuocPhamId = s.YeuCauLinhDuocPhamId,
                              DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                              TenDuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                              HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                              HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                              DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                              DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                              HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                              NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                              LaBHYT = s.LaDuocPhamBHYT,
                              Nhom = s.LaDuocPhamBHYT == true ? "Thuốc BHYT" : "Thuốc Không BHYT",
                              SoLuongCanBu = s.SoLuongCanBu,
                              SoLuongDaBu = s.SoLuongDaLinhBu,
                              SoLuongDuocDuyet = s.SoLuong,
                              SLYeuCauLinhThucTe = s.SoLuong
                          }).GroupBy(x => new { x.YeuCauLinhDuocPhamId, x.DuocPhamBenhVienId, x.LaBHYT, x.Nhom, x.HamLuong, x.HoatChat, x.DuongDung, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                            .Select(item => new DuocPhamLinhBuGridVo()
                            {
                                DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                LaBHYT = item.First().LaBHYT,
                                TenDuocPham = item.First().TenDuocPham,
                                Nhom = item.First().Nhom,
                                HamLuong = item.First().HamLuong,
                                HoatChat = item.First().HoatChat,
                                DuongDung = item.First().DuongDung,
                                DonViTinh = item.First().DonViTinh,
                                HangSanXuat = item.First().HangSanXuat,
                                NuocSanXuat = item.First().NuocSanXuat,
                                SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                                SoLuongDaBu = item.Sum(x => x.SoLuongDaBu),
                                SoLuongDuocDuyet = item.Sum(x => x.SoLuongDuocDuyet),
                                SLYeuCauLinhThucTe = Convert.ToInt32(item.Sum(x => x.SLYeuCauLinhThucTe).MathCelling()),
                            })
                            .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
                var duocPhamLinhBuGridVos = query.ToList();

                var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                    .Where(p => p.NhapKhoDuocPhams.KhoId == info.LinhTuKhoId)
                                    //&& p.SoLuongDaXuat < p.SoLuongNhap)
                                    .ToList();

                var result = duocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));
                result = result.Select(o =>
                {
                    o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                    o.SoLuongYeuCau = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                    : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SoLuongDaBu).MathRoundNumber(2));
                    return o;
                });
                var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                var queryTask = result.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }

        }

        public async Task<GridDataSource> GetYeuCauDuocPhamBenhVienTotalPageForGridAsync(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<GridDataSource> GetBenhNhanTheoDuocPhamDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }

            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauLinhId = long.Parse(queryObj[0]);
            var duocPhamBenhVienId = long.Parse(queryObj[1]);
            bool laBHYT = bool.Parse(queryObj[2]);
            bool isCreate = bool.Parse(queryObj[3]);
            var khoLinhId = long.Parse(queryObj[4]);
            var trangThai = 2; // 0: Chưa duyệt , 1: Được duyêt, 2: Từ chối duyệt
            var tuNgay = new DateTime(1970, 1, 1);
            var denNgay = DateTime.Now;
            if (queryObj[5] == "true")
            {
                trangThai = 1;
            }
            else if (queryObj[5] == "false")
            {
                trangThai = 2;
            }
            else
            {
                trangThai = 0;
            }
            var sLTon = 0.0;
            if (!string.IsNullOrEmpty(queryObj[6]))
            {
                sLTon = double.Parse(queryObj[6]);
            }
            if (!string.IsNullOrEmpty(queryObj[7]) && queryObj[7] != "null")
            {
                DateTime.TryParseExact(queryObj[7], "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
            }
            if (!string.IsNullOrEmpty(queryObj[8]) && queryObj[8] != "null")
            {
                DateTime.TryParseExact(queryObj[8], "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            }
            if (!isCreate)
            {
                if (trangThai == 0)
                {
                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                           .Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId
                                  && o.LaDuocPhamBHYT == laBHYT
                                  && o.YeuCauDuocPhamBenhVien.KhoLinhId == khoLinhId
                                  && o.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                  && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                  && o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                     && (o.YeuCauDuocPhamBenhVien != null && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh >= tuNgay && o.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh <= denNgay))
                                  .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                                  .Select(s => new DuocPhamLinhBuCuaBNGridVo
                                  {
                                      Id = s.Id,
                                      MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                      MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                                      HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                                      SL = s.YeuCauDuocPhamBenhVien.SoLuong.MathRoundNumber(2),
                                      SLDaBu = s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu.MathRoundNumber(2),
                                      SLYeuCau = s.SoLuongCanBu.MathRoundNumber(2),
                                      DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                                      BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                                      NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                      NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                                  });
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
                else
                {
                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId
                                  && o.LaDuocPhamBHYT == laBHYT
                                  && o.YeuCauDuocPhamBenhVien.KhoLinhId == khoLinhId
                                  && o.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                  && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                  && o.YeuCauLinhDuocPhamId == yeuCauLinhId)
                                  .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                                  .Select(s => new DuocPhamLinhBuCuaBNGridVo
                                  {
                                      Id = s.Id,
                                      MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                      MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                                      HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                                      SL = s.YeuCauDuocPhamBenhVien.SoLuong.MathRoundNumber(2),
                                      SLDaBu = s.SoLuongDaLinhBu.MathRoundNumber(2),
                                      SLYeuCau = s.SoLuong.MathRoundNumber(2),
                                      DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                                      BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                                      NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                      NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                                  });
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }

            }
            else
            {
                var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId
                            && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            && o.LaDuocPhamBHYT == laBHYT
                            && o.KhoLinhId == khoLinhId
                            && o.YeuCauLinhDuocPhamId == null
                            && o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                            && (!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.Any(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                            && o.KhongLinhBu != true
                            && o.SoLuong > 0
                            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                            && (o.NoiTruPhieuDieuTri != null && o.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.ThoiDiemChiDinh >= tuNgay && o.ThoiDiemChiDinh <= denNgay)
                            )
                .OrderBy(x => x.ThoiDiemChiDinh)
                .Select(s => new DuocPhamLinhBuCuaBNGridVo
                {
                    Id = s.Id,
                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    SL = s.SoLuong.MathRoundNumber(2),
                    SLDaBu = s.SoLuongDaLinhBu.MathRoundNumber(2),
                    DVKham = s.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                    BSKeToa = s.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    NgayDieuTri = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh
                });
                var duocPhamLinhBuCuaBNGridVos = query.ToList();
                foreach (var item in duocPhamLinhBuCuaBNGridVos)
                {
                    if (sLTon == 0)
                    {
                        break;
                    }
                    var soLuongConLai = (item.SL - (item.SLDaBu ?? 0)).MathRoundNumber(2);
                    if (soLuongConLai >= sLTon)
                    {
                        item.SLYeuCau = sLTon;
                        sLTon = 0;
                    }
                    else
                    {
                        item.SLYeuCau = soLuongConLai.MathRoundNumber(2);
                        sLTon -= soLuongConLai.Value.MathRoundNumber(2);
                    }
                }
                var result = duocPhamLinhBuCuaBNGridVos.Where(p => p.SLYeuCau > 0);
                var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                var queryTask = result.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }

        }

        public async Task<GridDataSource> GetBenhNhanTheoDuocPhamTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }

            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauLinhId = long.Parse(queryObj[0]);
            var duocPhamBenhVienId = long.Parse(queryObj[1]);
            bool laBHYT = bool.Parse(queryObj[2]);
            bool isCreate = bool.Parse(queryObj[3]);
            var khoLinhId = long.Parse(queryObj[4]);
            var trangThai = 2; // 0: Chưa duyệt , 1: Được duyêt, 2: Từ chối duyệt
            var tuNgay = new DateTime(1970, 1, 1);
            var denNgay = DateTime.Now;
            if (queryObj[5] == "true")
            {
                trangThai = 1;
            }
            else if (queryObj[5] == "false")
            {
                trangThai = 2;
            }
            else
            {
                trangThai = 0;
            }
            var sLTon = 0.0;
            if (!string.IsNullOrEmpty(queryObj[6]))
            {
                sLTon = double.Parse(queryObj[6]);
            }

            if (!string.IsNullOrEmpty(queryObj[7]) && queryObj[7] != "null")
            {
                DateTime.TryParseExact(queryObj[7], "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
            }
            if (!string.IsNullOrEmpty(queryObj[8]) && queryObj[8] != "null")
            {
                DateTime.TryParseExact(queryObj[8], "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            }
            if (!isCreate)
            {
                if (trangThai == 0)
                {
                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                           .Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId
                                  && o.LaDuocPhamBHYT == laBHYT
                                  && o.YeuCauDuocPhamBenhVien.KhoLinhId == khoLinhId
                                  && o.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                  && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                  && o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                  && (o.YeuCauDuocPhamBenhVien != null && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh >= tuNgay && o.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh <= denNgay))
                                  .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                                  .Select(s => new DuocPhamLinhBuCuaBNGridVo
                                  {
                                      Id = s.Id,
                                      MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                      MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                                      HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                                      SL = s.YeuCauDuocPhamBenhVien.SoLuong,
                                      SLDaBu = s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu,
                                      SLYeuCau = s.SoLuong,
                                      DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                                      BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                                      NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                      NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                                  });
                    var countTask = query.CountAsync();
                    await Task.WhenAll(countTask);
                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
                else
                {
                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId
                                  && o.LaDuocPhamBHYT == laBHYT
                                  && o.YeuCauDuocPhamBenhVien.KhoLinhId == khoLinhId
                                  && o.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                  && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                  && o.YeuCauLinhDuocPhamId == yeuCauLinhId)
                                  .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                                  .Select(s => new DuocPhamLinhBuCuaBNGridVo
                                  {
                                      Id = s.Id,
                                      MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                      MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                                      HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                                      SL = s.YeuCauDuocPhamBenhVien.SoLuong,
                                      SLDaBu = s.SoLuongDaLinhBu,
                                      SLYeuCau = s.SoLuong,
                                      DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                                      BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                                      NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                      NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                                  });
                    var countTask = query.CountAsync();
                    await Task.WhenAll(countTask);
                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
            }
            else
            {
                var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                   .Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId
                               && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                               && o.KhoLinhId == khoLinhId
                               && o.YeuCauLinhDuocPhamId == null
                               && o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                               && o.KhongLinhBu != true
                               && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                               && (!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.Any(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                               && o.LaDuocPhamBHYT == laBHYT
                               && (o.NoiTruPhieuDieuTri != null && o.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.ThoiDiemChiDinh >= tuNgay && o.ThoiDiemChiDinh <= denNgay)
                               )
                   .OrderBy(x => x.ThoiDiemChiDinh)
                   .Select(s => new DuocPhamLinhBuCuaBNGridVo
                   {
                       Id = s.Id,
                       MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                       MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                       HoTen = s.YeuCauTiepNhan.HoTen,
                       SL = s.SoLuong,
                       SLDaBu = s.SoLuongDaLinhBu,
                       DVKham = s.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                       BSKeToa = s.NhanVienChiDinh.User.HoTen,
                       NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                       NgayDieuTri = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh
                   });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }
        }

        public async Task<List<LookupItemVo>> GetKhoCurrentUserLinhBu(DropDownListRequestModel queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                        .Where(p => p.NhanVienId == userCurrentId && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin))
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.KhoId,
                            DisplayName = s.Kho.Ten
                        })
                        .OrderByDescending(x => x.KeyId == khoId).ThenBy(x => x.DisplayName)
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);

            return await result.ToListAsync();
        }

        public bool KiemTraSoLuongTon(long khoLinhTuId, long duocPhamBenhVienId, bool laDuocPhamBHYT, double? soLuongBu)
        {
            var soLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                   .Where(x => x.DuocPhamBenhVienId == duocPhamBenhVienId
                                       && x.NhapKhoDuocPhams.KhoId == khoLinhTuId
                                       && x.LaDuocPhamBHYT == laDuocPhamBHYT
                                       && x.NhapKhoDuocPhams.DaHet != true
                                       && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat).MathRoundNumber();

            if (soLuongTon < soLuongBu)
            {
                return false;
            }
            return true;
        }

        public async Task XuLyThemYeuCauLinhDuocPhamBuAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhBuDuocPham, List<ThongTinDuocPhamChiTietItem> thongTinDuocPhamChiTietItems)
        {
            var duocPhamBenhVienIds = thongTinDuocPhamChiTietItems.Select(o => o.DuocPhamBenhVienId.GetValueOrDefault()).ToList();
            var khoKhoLinhVeId = thongTinDuocPhamChiTietItems.First().KhoLinhVeId;
            var khoLinhTuId = thongTinDuocPhamChiTietItems.First().KhoLinhTuId;
            var yeuCauDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Include(yc => yc.NoiTruPhieuDieuTri)
                                .Where(x => x.YeuCauLinhDuocPhamId == null
                                && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.Any(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                                && x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                && x.KhongLinhBu != true
                                && x.SoLuong > 0
                                && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                                //&& x.KhoLinhId == thongTinDuocPhamChiTietItems.First().KhoLinhVeId
                                //&& thongTinDuocPhamChiTietItems.Any(p => p.DuocPhamBenhVienId == x.DuocPhamBenhVienId)
                                && x.KhoLinhId == khoKhoLinhVeId
                                && duocPhamBenhVienIds.Contains(x.DuocPhamBenhVienId)
                                ).ToList();
            yeuCauDuocPhamBenhViens = yeuCauDuocPhamBenhViens.Where(x => x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri >= thongTinDuocPhamChiTietItems.First().ThoiDiemChiDinhTu
                                     && x.NoiTruPhieuDieuTri.NgayDieuTri <= thongTinDuocPhamChiTietItems.First().ThoiDiemChiDinhDen
                                || x.ThoiDiemChiDinh >= thongTinDuocPhamChiTietItems.First().ThoiDiemChiDinhTu && x.ThoiDiemChiDinh <= thongTinDuocPhamChiTietItems.First().ThoiDiemChiDinhDen).ToList();
            
            var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(p =>
                                                            //p.NhapKhoDuocPhams.KhoId == thongTinDuocPhamChiTietItems.First().KhoLinhTuId
                                                            //&& thongTinDuocPhamChiTietItems.Any(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId)
                                                            p.NhapKhoDuocPhams.KhoId == khoLinhTuId
                                                            && duocPhamBenhVienIds.Contains(p.DuocPhamBenhVienId)
                                                            && p.SoLuongNhap > p.SoLuongDaXuat).ToList();

            foreach (var yc in thongTinDuocPhamChiTietItems)
            {
                var sLYeuCauLinhThucTe = yc.SLYeuCauLinhThucTe;
                var sLDaBu = yc.SoLuongDaBu.MathRoundNumber(2);

                var duocPhamBvs = yeuCauDuocPhamBenhViens.Where(p => p.LaDuocPhamBHYT == yc.LaDuocPhamBHYT && p.DuocPhamBenhVienId == yc.DuocPhamBenhVienId).ToList();
                var soLuongTonHienTai = nhapKhoDuocPhamChiTiets
                                        .Where(p => p.DuocPhamBenhVienId == yc.DuocPhamBenhVienId && p.LaDuocPhamBHYT == yc.LaDuocPhamBHYT).Sum(p => p.SoLuongNhap - p.SoLuongDaXuat);
                if (soLuongTonHienTai < yc.SLYeuCauLinhThucTe)
                {
                    // throw message error
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.SoLuongTon.KhongDu"));
                }
                foreach (var dp in duocPhamBvs)
                {
                    if (sLYeuCauLinhThucTe == 0)
                    {
                        break;
                    }
                    var yeuCauLinhDuocPhamChiTiet = new YeuCauLinhDuocPhamChiTiet
                    {

                        DuocPhamBenhVienId = yc.DuocPhamBenhVienId.Value,
                        LaDuocPhamBHYT = yc.LaDuocPhamBHYT.Value,
                        SoLuongCanBu = dp.SoLuong.MathRoundNumber(2),
                        //SoLuongDaLinhBu = yc.SoLuongDaBu
                    };
                    var soLuongConLai = (dp.SoLuong - dp.SoLuongDaLinhBu.GetValueOrDefault()).MathRoundNumber(2);
                    var soLuongDaBuConLai = sLDaBu.GetValueOrDefault().MathRoundNumber(2);
                    if (soLuongDaBuConLai > sLDaBu || soLuongDaBuConLai.AlmostEqual(sLDaBu.GetValueOrDefault()))
                    {
                        yeuCauLinhDuocPhamChiTiet.SoLuongDaLinhBu = sLDaBu.MathRoundNumber(2);
                        sLDaBu = 0;
                    }
                    else
                    {
                        yeuCauLinhDuocPhamChiTiet.SoLuongDaLinhBu = soLuongDaBuConLai.MathRoundNumber(2);
                        sLDaBu = (sLDaBu - soLuongDaBuConLai).MathRoundNumber(2);
                    }
                    //SoluongConlai > SL Yeu cau
                    if (soLuongConLai > sLYeuCauLinhThucTe || soLuongConLai.AlmostEqual(sLYeuCauLinhThucTe.GetValueOrDefault()))
                    {
                        yeuCauLinhDuocPhamChiTiet.SoLuong = sLYeuCauLinhThucTe.Value.MathRoundNumber(2);
                        sLYeuCauLinhThucTe = 0;
                    }
                    else
                    {
                        yeuCauLinhDuocPhamChiTiet.SoLuong = soLuongConLai.MathRoundNumber(2);
                        sLYeuCauLinhThucTe = (sLYeuCauLinhThucTe - soLuongConLai).MathRoundNumber(2);
                    }
                    yeuCauLinhDuocPhamChiTiet.YeuCauDuocPhamBenhVienId = dp.Id;
                    yeuCauLinhBuDuocPham.YeuCauLinhDuocPhamChiTiets.Add(yeuCauLinhDuocPhamChiTiet);
                }
                yeuCauLinhBuDuocPham.YeuCauLinhDuocPhamChiTiets.Last().SoLuong = (yeuCauLinhBuDuocPham.YeuCauLinhDuocPhamChiTiets.Last().SoLuong + sLYeuCauLinhThucTe.GetValueOrDefault(0)).MathRoundNumber(2);
            }
            //await _yeuCauLinhDuocPhamRepository.AddAsync(yeuCauLinhBuDuocPham);
        }

        public async Task XuLyCapNhatYeuCauLinhDuocPhamBuAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhBuDuocPham, List<ThongTinDuocPhamChiTietItem> thongTinDuocPhamChiTietItems)
        {
            var nhapKhoDuocPhamChiTiets = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(p => p.NhapKhoDuocPhams.KhoId == thongTinDuocPhamChiTietItems.First().KhoLinhTuId
                                                            && thongTinDuocPhamChiTietItems.Any(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId)
                                                            && p.SoLuongNhap > p.SoLuongDaXuat).ToListAsync();
            foreach (var item in yeuCauLinhBuDuocPham.YeuCauLinhDuocPhamChiTiets.Where(z => !thongTinDuocPhamChiTietItems.Any(c => c.DuocPhamBenhVienId == z.DuocPhamBenhVienId)))
            {
                item.WillDelete = true;
            }
            foreach (var yc in thongTinDuocPhamChiTietItems)
            {
                var sLYeuCauLinhThucTe = yc.SLYeuCauLinhThucTe;
                var sLDaBu = yc.SoLuongDaBu.MathRoundNumber(2);

                var yeuCauLinhDuocPhamChiTiets = yeuCauLinhBuDuocPham.YeuCauLinhDuocPhamChiTiets.Where(p => p.LaDuocPhamBHYT == yc.LaDuocPhamBHYT && p.DuocPhamBenhVienId == yc.DuocPhamBenhVienId).ToList();
                //yeuCauLinhDuocPhamChiTiets = yeuCauLinhDuocPhamChiTiets.Where(p => p.YeuCauDuocPhamBenhVien != null && p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null
                //    && p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= thongTinDuocPhamChiTietItems.First().ThoiDiemChiDinhTu
                //    && p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= thongTinDuocPhamChiTietItems.First().ThoiDiemChiDinhDen
                //    || p.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh >= thongTinDuocPhamChiTietItems.First().ThoiDiemChiDinhTu && p.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh <= thongTinDuocPhamChiTietItems.First().ThoiDiemChiDinhDen).ToList();
                var soLuongTonHienTai = nhapKhoDuocPhamChiTiets
                                        .Where(p => p.DuocPhamBenhVienId == yc.DuocPhamBenhVienId && p.LaDuocPhamBHYT == yc.LaDuocPhamBHYT).Sum(p => p.SoLuongNhap - p.SoLuongDaXuat);
                if (soLuongTonHienTai < yc.SLYeuCauLinhThucTe)
                {
                    // throw message error
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.SoLuongTon.KhongDu"));
                }
                foreach (var yeuCauLinhDuocPhamChiTiet in yeuCauLinhDuocPhamChiTiets)
                {
                    if (sLYeuCauLinhThucTe == 0)
                    {
                        yeuCauLinhDuocPhamChiTiet.WillDelete = true;
                    }
                    else
                    {
                        //var soLuongConLai = (yeuCauLinhDuocPhamChiTiet.SoLuong - (yeuCauLinhDuocPhamChiTiet.SoLuongDaLinhBu ?? 0)).MathRoundNumber(2);
                        var soLuongConLai = (yeuCauLinhDuocPhamChiTiet.SoLuongCanBu.GetValueOrDefault() - yeuCauLinhDuocPhamChiTiet.SoLuongDaLinhBu.GetValueOrDefault()).MathRoundNumber(2);
                        var soLuongDaBuConLai = sLDaBu.GetValueOrDefault().MathRoundNumber(2);
                        if (soLuongDaBuConLai > sLDaBu || soLuongDaBuConLai.AlmostEqual(sLDaBu.GetValueOrDefault()))
                        {
                            yeuCauLinhDuocPhamChiTiet.SoLuongDaLinhBu = sLDaBu.MathRoundNumber(2);
                            sLDaBu = 0;
                        }
                        else
                        {
                            yeuCauLinhDuocPhamChiTiet.SoLuongDaLinhBu = soLuongDaBuConLai.MathRoundNumber(2);
                            sLDaBu = (sLDaBu - soLuongDaBuConLai).MathRoundNumber(2);
                        }
                        //SoluongConlai > SL Yeu cau
                        if (soLuongConLai > sLYeuCauLinhThucTe || soLuongConLai.AlmostEqual(sLYeuCauLinhThucTe.GetValueOrDefault()))
                        {
                            yeuCauLinhDuocPhamChiTiet.SoLuong = sLYeuCauLinhThucTe.Value.MathRoundNumber(2);
                            sLYeuCauLinhThucTe = 0;
                        }
                        else
                        {
                            yeuCauLinhDuocPhamChiTiet.SoLuong = soLuongConLai.MathRoundNumber(2);
                            sLYeuCauLinhThucTe = (sLYeuCauLinhThucTe - soLuongConLai).MathRoundNumber(2);
                        }
                    }
                }
                yeuCauLinhBuDuocPham.YeuCauLinhDuocPhamChiTiets.Last().SoLuong = (yeuCauLinhBuDuocPham.YeuCauLinhDuocPhamChiTiets.Last().SoLuong + sLYeuCauLinhThucTe.GetValueOrDefault(0)).MathRoundNumber(2);
            }
        }

        public List<long> GetIdsYeuCauKhamBenh(long KhoLinhTuId, long KhoLinhVeId)
        {
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                  .Where(x => x.KhoLinhId == KhoLinhVeId
                              && x.YeuCauLinhDuocPhamId == null
                              && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                              && x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                              && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                              && x.KhongLinhBu != true
                              && x.SoLuong > 0
                              && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                              )
                  .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                  {
                      Id = item.Id,
                      DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                      LaBHYT = item.LaDuocPhamBHYT,
                      TenDuocPham = item.Ten,
                      NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                      HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                      DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                      DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                      HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                      NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                      SoLuongCanBu = item.SoLuong,
                      SoLuongDaBu = item.SoLuongDaLinhBu
                  });

            var yeuCauLinhDuocPhamBuGridParentVos = query.ToList();

            var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == KhoLinhTuId
                     && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var result = yeuCauLinhDuocPhamBuGridParentVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));



            return result.Select(s => s.Id).ToList();
        }
        public DateTime GetDateTime(long YeuCauDuocPhamBenhVienId)
        {
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(s => s.Id == YeuCauDuocPhamBenhVienId).Select(s => (s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh));
            return query.First();
        }
        public List<long> GetIdsYeuCauDuocPhamBenhVien(long KhoLinhTuId, long KhoLinhVeId,long duocPhamId)
        {
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                  .Where(x => x.KhoLinhId == KhoLinhVeId
                              && x.YeuCauLinhDuocPhamId == null
                              && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                              && x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                              && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                              && x.KhongLinhBu != true
                              && x.SoLuong > 0
                              && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                              && x.DuocPhamBenhVienId == duocPhamId)
                  .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                  {
                      Id = item.Id,
                      DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                      LaBHYT = item.LaDuocPhamBHYT,
                      TenDuocPham = item.Ten,
                      NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                      HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                      DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                      DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                      HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                      NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                      SoLuongCanBu = item.SoLuong,
                      SoLuongDaBu = item.SoLuongDaLinhBu
                  });

            var yeuCauLinhDuocPhamBuGridParentVos = query.ToList();

            var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == KhoLinhTuId
                     && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var result = yeuCauLinhDuocPhamBuGridParentVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));



            return result.Select(s => s.Id).ToList();
        }

        public List<YeuCauLinhDuocPhamBuGridParentVo> GetChiTietYeuCauDuocPhamBenhVienCanBu(long KhoLinhTuId, long KhoLinhVeId, List<long> duocPhamIds)
        {
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                  .Where(x => x.KhoLinhId == KhoLinhVeId
                              && x.YeuCauLinhDuocPhamId == null
                              && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                              && x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                              && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                              && x.KhongLinhBu != true
                              && x.SoLuong > 0
                              && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                              && duocPhamIds.Contains(x.DuocPhamBenhVienId))
                  .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                  {
                      Id = item.Id,
                      DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                      LaBHYT = item.LaDuocPhamBHYT,
                      TenDuocPham = item.Ten,
                      NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                      HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                      DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                      DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                      HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                      NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                      SoLuongCanBu = item.SoLuong,
                      SoLuongDaBu = item.SoLuongDaLinhBu,
                      NgayChiDinh = item.NoiTruPhieuDieuTri != null ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                  });

            var yeuCauLinhDuocPhamBuGridParentVos = query.ToList();

            var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == KhoLinhTuId
                     && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var result = yeuCauLinhDuocPhamBuGridParentVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

            return result.ToList();
        }
    }
}
