using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Newtonsoft.Json;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.Grid;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using System.Globalization;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial class YeuCauLinhVatTuService
    {
        public async Task<GridDataSource> GetYeuCauVatTuBenhVienDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<VatTuBenhVienJsonVo>(queryInfo.AdditionalSearchString);
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
                var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(x => x.KhoLinhId == info.LinhVeKhoId
                                && x.YeuCauLinhVatTuId == null
                                && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                                && x.KhongLinhBu != true
                                && x.SoLuong > 0
                                && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                                && ((x.NoiTruPhieuDieuTri != null
                                    && x.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay
                                    && x.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay)
                                    || (x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                )
                    .Select(item => new YeuCauLinhVatTuBuGridParentVo()
                    {
                        Id = item.Id,
                        VatTuBenhVienId = item.VatTuBenhVienId,
                        LaBHYT = item.LaVatTuBHYT,
                        TenVatTu = item.Ten,
                        Nhom = item.LaVatTuBHYT ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                        DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                        HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                        SoLuongCanBu = item.SoLuong,
                        SoLuongDaBu = item.SoLuongDaLinhBu
                    })
                    .GroupBy(x => new { x.YeuCauLinhVatTuId, x.VatTuBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                    .Select(item => new YeuCauLinhVatTuBuGridParentVo()
                    {
                        YeuCauLinhVatTuIdstring = string.Join(",", item.Select(x => x.Id)),
                        VatTuBenhVienId = item.First().VatTuBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenVatTu = item.First().TenVatTu,
                        Nhom = item.First().Nhom,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                        SoLuongDaBu = item.Sum(x => x.SoLuongDaBu)

                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                var yeuCauLinhVatTuBuGridParentVos = query.ToList();

                var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == info.LinhTuKhoId
                         && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                var result = yeuCauLinhVatTuBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));

                result = result.Select(o =>
                {
                    o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
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
                var vatTuBuGridParentVos = new List<YeuCauLinhVatTuBuGridParentVo>();
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                        .Where(p => p.YeuCauLinhVatTuId == info.YeuCauLinhVatTuId
                            && p.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                             && (p.YeuCauVatTuBenhVien != null
                                && ((p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null
                                    && p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay
                                    && p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay)
                                    || (p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri == null && p.YeuCauVatTuBenhVien.ThoiDiemChiDinh >= tuNgay && p.YeuCauVatTuBenhVien.ThoiDiemChiDinh <= denNgay))
                                )
                        )
                        .Select(s => new YeuCauLinhVatTuBuGridParentVo
                        {
                            Id = s.Id,
                            YeuCauLinhVatTuId = s.YeuCauLinhVatTuId,
                            VatTuBenhVienId = s.VatTuBenhVienId,
                            TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                            DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                            HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                            NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                            LaBHYT = s.LaVatTuBHYT,
                            Nhom = s.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                            SoLuongCanBu = s.SoLuongCanBu,
                            SoLuongDaBu = s.SoLuongDaLinhBu,
                            SoLuongDuocDuyet = s.SoLuong,
                            SLYeuCauLinhThucTe = s.SoLuong

                        }).GroupBy(x => new { x.YeuCauLinhVatTuId, x.VatTuBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                          .Select(item => new YeuCauLinhVatTuBuGridParentVo()
                          {
                              VatTuBenhVienId = item.First().VatTuBenhVienId,
                              LaBHYT = item.First().LaBHYT,
                              TenVatTu = item.First().TenVatTu,
                              Nhom = item.First().Nhom,
                              DonViTinh = item.First().DonViTinh,
                              HangSanXuat = item.First().HangSanXuat,
                              NuocSanXuat = item.First().NuocSanXuat,
                              SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                              //SoLuongCanBu = item.First().SoLuongCanBu,
                              SoLuongDaBu = item.Sum(x => x.SoLuongDaBu),
                              SoLuongDuocDuyet = item.Sum(x => x.SoLuongDuocDuyet),
                              SLYeuCauLinhThucTe = Convert.ToInt32(item.Sum(x => x.SLYeuCauLinhThucTe).MathCelling()),
                          })
                          .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                vatTuBuGridParentVos = query.ToList();

                var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                            .Where(x => x.NhapKhoVatTu.KhoId == info.LinhTuKhoId)
                                            //&& x.SoLuongDaXuat < x.SoLuongNhap
                                            .ToList();

                var result = vatTuBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));

                result = result.Select(o =>
                {
                    o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                    o.SoLuongYeuCau = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                    : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SoLuongDaBu).MathRoundNumber(2));
                    return o;
                });
                var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                var queryTask = result.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }
        }

        public async Task<GridDataSource> GetYeuCauVatTuBenhVienTotalPageForGridAsync(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<GridDataSource> GetBenhNhanTheoVatTuDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }

            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauLinhId = long.Parse(queryObj[0]);
            var vatTuBenhVienId = long.Parse(queryObj[1]);
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
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                   .Where(o => o.VatTuBenhVienId == vatTuBenhVienId
                          && o.YeuCauVatTuBenhVien.LaVatTuBHYT == laBHYT
                          && o.YeuCauVatTuBenhVien.KhoLinhId == khoLinhId
                          && o.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                          && o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                          && o.YeuCauLinhVatTuId == yeuCauLinhId
                          && (o.YeuCauVatTuBenhVien != null && o.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null && o.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.YeuCauVatTuBenhVien.ThoiDiemChiDinh >= tuNgay && o.YeuCauVatTuBenhVien.ThoiDiemChiDinh <= denNgay)
                          )
                  .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                  .Select(s => new VatTuLinhBuCuaBNGridVos
                  {
                      Id = s.Id,
                      MaTN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                      MaBN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                      HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                      SL = s.YeuCauVatTuBenhVien.SoLuong.MathRoundNumber(2),
                      SLDaBu = s.YeuCauVatTuBenhVien.SoLuongDaLinhBu,
                      SLYeuCau = s.SoLuongCanBu.MathRoundNumber(2),
                      DVKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                      BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                      NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                      NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh
                  });
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
                else
                {
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                   .Where(o => o.VatTuBenhVienId == vatTuBenhVienId
                          && o.YeuCauVatTuBenhVien.LaVatTuBHYT == laBHYT
                          && o.YeuCauVatTuBenhVien.KhoLinhId == khoLinhId
                          && o.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                          && o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                          && o.YeuCauLinhVatTuId == yeuCauLinhId
                          )
                  .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                  .Select(s => new VatTuLinhBuCuaBNGridVos
                  {
                      Id = s.Id,
                      MaTN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                      MaBN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                      HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                      SL = s.YeuCauVatTuBenhVien.SoLuong.MathRoundNumber(2),
                      SLDaBu = s.SoLuongDaLinhBu.MathRoundNumber(2),
                      SLYeuCau = s.SoLuongCanBu.MathRoundNumber(2),
                      DVKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                      BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                      NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                      NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh
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
                var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.VatTuBenhVienId == vatTuBenhVienId
                            && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            && o.LaVatTuBHYT == laBHYT
                            && o.KhoLinhId == khoLinhId
                            && o.YeuCauLinhVatTuId == null
                            && o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                            && o.KhongLinhBu != true
                            && o.SoLuong > 0
                            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                               && (o.NoiTruPhieuDieuTri != null && o.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.ThoiDiemChiDinh >= tuNgay && o.ThoiDiemChiDinh <= denNgay)
                            )
                .OrderBy(x => x.ThoiDiemChiDinh)
                .Select(s => new VatTuLinhBuCuaBNGridVos
                {
                    Id = s.Id,
                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    SL = s.SoLuong.MathRoundNumber(2), // SoLuongCanBu
                    SLDaBu = s.SoLuongDaLinhBu.MathRoundNumber(2),
                    DVKham = s.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                    BSKeToa = s.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    NgayDieuTri = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh
                });
                var vatTuLinhBuCuaBNGridVos = query.ToList();
                foreach (var item in vatTuLinhBuCuaBNGridVos)
                {
                    if (sLTon == 0)
                    {
                        break;
                    }
                    var soLuongConLai = (item.SL - item.SLDaBu.GetValueOrDefault()).MathRoundNumber(2);
                    if (soLuongConLai >= sLTon)
                    {
                        item.SLYeuCau = sLTon.MathRoundNumber(2);
                        sLTon = 0;
                    }
                    else
                    {
                        item.SLYeuCau = soLuongConLai.MathRoundNumber(2);
                        sLTon -= soLuongConLai.Value.MathRoundNumber(2);
                    }
                }
                var result = vatTuLinhBuCuaBNGridVos.Where(p => p.SLYeuCau > 0);
                var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                var queryTask = result.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }
        }
        public async Task<GridDataSource> GetBenhNhanTheoVatTuTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }

            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauLinhId = long.Parse(queryObj[0]);
            var vatTuBenhVienId = long.Parse(queryObj[1]);
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
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                   .Where(o => o.VatTuBenhVienId == vatTuBenhVienId
                          && o.YeuCauVatTuBenhVien.LaVatTuBHYT == laBHYT
                          && o.YeuCauVatTuBenhVien.KhoLinhId == khoLinhId
                          && o.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                          && o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                          && o.YeuCauLinhVatTuId == yeuCauLinhId
                          && (o.YeuCauVatTuBenhVien != null && o.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null && o.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.YeuCauVatTuBenhVien.ThoiDiemChiDinh >= tuNgay && o.YeuCauVatTuBenhVien.ThoiDiemChiDinh <= denNgay))
                  .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                  .Select(s => new VatTuLinhBuCuaBNGridVos
                  {
                      Id = s.Id,
                      MaTN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                      MaBN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                      HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                      SL = s.YeuCauVatTuBenhVien.SoLuong.MathRoundNumber(2),
                      SLDaBu = s.YeuCauVatTuBenhVien.SoLuongDaLinhBu,
                      SLYeuCau = s.SoLuongCanBu.MathRoundNumber(2),
                      DVKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                      BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                      NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                      NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh
                  });
                    var countTask = query.CountAsync();
                    await Task.WhenAll(countTask);
                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
                else
                {
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                   .Where(o => o.VatTuBenhVienId == vatTuBenhVienId
                          && o.YeuCauVatTuBenhVien.LaVatTuBHYT == laBHYT
                          && o.YeuCauVatTuBenhVien.KhoLinhId == khoLinhId
                          && o.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                          && o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                          && o.YeuCauLinhVatTuId == yeuCauLinhId
                          )
                  .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                  .Select(s => new VatTuLinhBuCuaBNGridVos
                  {
                      Id = s.Id,
                      MaTN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                      MaBN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                      HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                      SL = s.YeuCauVatTuBenhVien.SoLuong.MathRoundNumber(2),
                      SLDaBu = s.SoLuongDaLinhBu.MathRoundNumber(2),
                      SLYeuCau = s.SoLuongCanBu.MathRoundNumber(2),
                      DVKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                      BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                      NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                      NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh
                  });
                    var countTask = query.CountAsync();
                    await Task.WhenAll(countTask);
                    return new GridDataSource { TotalRowCount = countTask.Result };
                }

            }
            else
            {
                var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.VatTuBenhVienId == vatTuBenhVienId
                            && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            && o.LaVatTuBHYT == laBHYT
                            && o.KhoLinhId == khoLinhId
                            && o.YeuCauLinhVatTuId == null
                            && o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                            && o.KhongLinhBu != true
                            && o.SoLuong > 0
                            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                               && (o.NoiTruPhieuDieuTri != null && o.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.ThoiDiemChiDinh >= tuNgay && o.ThoiDiemChiDinh <= denNgay)
                            )
                .OrderBy(x => x.ThoiDiemChiDinh)
                .Select(s => new VatTuLinhBuCuaBNGridVos
                {
                    Id = s.Id,
                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    SL = s.SoLuong.MathRoundNumber(2), // SoLuongCanBu
                    SLDaBu = s.SoLuongDaLinhBu.MathRoundNumber(2),
                    DVKham = s.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                    BSKeToa = s.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    NgayDieuTri = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh
                });
                var vatTuLinhBuCuaBNGridVos = query.ToList();
                foreach (var item in vatTuLinhBuCuaBNGridVos)
                {
                    if (sLTon == 0)
                    {
                        break;
                    }
                    var soLuongConLai = item.SL - (item.SLDaBu ?? 0);
                    if (soLuongConLai >= sLTon)
                    {
                        item.SLYeuCau = sLTon;
                        sLTon = 0;
                    }
                    else
                    {
                        item.SLYeuCau = soLuongConLai;
                        sLTon -= soLuongConLai.Value;
                    }
                }
                var result = vatTuLinhBuCuaBNGridVos.Where(p => p.SLYeuCau > 0);
                var countTask = result.Count();
                return new GridDataSource { TotalRowCount = countTask };
            }
        }
        public bool KiemTraSoLuongTon(long khoLinhTuId, long vatTuBenhVienId, bool laVatTuBHYT, double? soLuongBu)
        {
            var soLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                 .Where(x => x.VatTuBenhVienId == vatTuBenhVienId
                                     && x.NhapKhoVatTu.KhoId == khoLinhTuId
                                     && x.LaVatTuBHYT == laVatTuBHYT
                                     && x.NhapKhoVatTu.DaHet != true
                                     && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat).MathRoundNumber();
            if (soLuongTon < soLuongBu)
            {
                return false;
            }
            return true;
        }

        public async Task XuLyThemYeuCauLinhVatTuBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<ThongTinVatTuTietItem> thongTinVatTuChiTietItems)
        {
            var yeuCauVatTuBenhViens = await _yeuCauVatTuBenhVienRepository.TableNoTracking.Include(yc => yc.NoiTruPhieuDieuTri).Where(x => x.YeuCauLinhVatTuId == null
                                && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                                && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && x.KhongLinhBu != true
                                && x.SoLuong > 0
                                && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                                && x.KhoLinhId == thongTinVatTuChiTietItems.First().KhoLinhVeId
                                && thongTinVatTuChiTietItems.Any(p => p.VatTuBenhVienId == x.VatTuBenhVienId)).OrderBy(z => z.CreatedOn).ToListAsync();

            yeuCauVatTuBenhViens = yeuCauVatTuBenhViens.Where(x => x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri >= thongTinVatTuChiTietItems.First().ThoiDiemChiDinhTu
                                 && x.NoiTruPhieuDieuTri.NgayDieuTri <= thongTinVatTuChiTietItems.First().ThoiDiemChiDinhDen
                            || x.ThoiDiemChiDinh >= thongTinVatTuChiTietItems.First().ThoiDiemChiDinhTu && x.ThoiDiemChiDinh <= thongTinVatTuChiTietItems.First().ThoiDiemChiDinhDen).ToList();
            var nhapKhoVatTuChiTiets = await _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == thongTinVatTuChiTietItems.First().KhoLinhTuId
                                                            && thongTinVatTuChiTietItems.Any(x => x.VatTuBenhVienId == p.VatTuBenhVienId)
                                                            && p.SoLuongNhap > p.SoLuongDaXuat).ToListAsync();

            foreach (var yc in thongTinVatTuChiTietItems)
            {
                var sLYeuCauLinhThucTe = yc.SLYeuCauLinhThucTe;
                var sLDaBu = yc.SoLuongDaBu;

                var vatTuBvs = yeuCauVatTuBenhViens.Where(p => p.LaVatTuBHYT == yc.LaVatTuBHYT && p.VatTuBenhVienId == yc.VatTuBenhVienId).OrderBy(z => z.CreatedOn).ToList();
                var soLuongTonHienTai = nhapKhoVatTuChiTiets
                                        .Where(p => p.VatTuBenhVienId == yc.VatTuBenhVienId && p.LaVatTuBHYT == yc.LaVatTuBHYT).Sum(p => p.SoLuongNhap - p.SoLuongDaXuat);
                if (soLuongTonHienTai < yc.SLYeuCauLinhThucTe)
                {
                    // throw message error
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.SoLuongTon.KhongDu"));
                }
                foreach (var vt in vatTuBvs)
                {
                    if (sLYeuCauLinhThucTe == 0)
                    {
                        break;
                    }
                    var yeuCauLinhVatTuChiTiet = new YeuCauLinhVatTuChiTiet
                    {

                        VatTuBenhVienId = yc.VatTuBenhVienId.Value,
                        LaVatTuBHYT = yc.LaVatTuBHYT.Value,
                        SoLuongCanBu = vt.SoLuong,
                        //SoLuongDaLinhBu = yc.SoLuongDaBu
                    };
                    var soLuongConLai = vt.SoLuong - (vt.SoLuongDaLinhBu ?? 0);
                    var soLuongDaBuConLai = sLDaBu ?? 0;
                    if (soLuongDaBuConLai > sLDaBu || soLuongDaBuConLai.AlmostEqual(sLDaBu.GetValueOrDefault()))
                    {
                        yeuCauLinhVatTuChiTiet.SoLuongDaLinhBu = sLDaBu;
                        sLDaBu = 0;
                    }
                    else
                    {
                        yeuCauLinhVatTuChiTiet.SoLuongDaLinhBu = soLuongDaBuConLai;
                        sLDaBu -= soLuongDaBuConLai;
                    }
                    //SoluongConlai > SL Yeu cau
                    if (soLuongConLai > sLYeuCauLinhThucTe || soLuongConLai.AlmostEqual(sLYeuCauLinhThucTe.GetValueOrDefault()))
                    {
                        yeuCauLinhVatTuChiTiet.SoLuong = sLYeuCauLinhThucTe.Value;
                        sLYeuCauLinhThucTe = 0;
                    }
                    else
                    {
                        yeuCauLinhVatTuChiTiet.SoLuong = soLuongConLai;
                        sLYeuCauLinhThucTe -= soLuongConLai;
                    }
                    yeuCauLinhVatTuChiTiet.YeuCauVatTuBenhVienId = vt.Id;
                    yeuCauLinhBuVatTu.YeuCauLinhVatTuChiTiets.Add(yeuCauLinhVatTuChiTiet);
                }
                yeuCauLinhBuVatTu.YeuCauLinhVatTuChiTiets.Last().SoLuong += sLYeuCauLinhThucTe.GetValueOrDefault(0);
            }
            //await _yeuCauLinhDuocPhamRepository.AddAsync(yeuCauLinhBuDuocPham);
        }

        public async Task XuLyCapNhatYeuCauLinhVatTuBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<ThongTinVatTuTietItem> thongTinVatTuChiTietItems)
        {
            var nhapKhoVatTuChiTiets = await _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == thongTinVatTuChiTietItems.First().KhoLinhTuId
                                                            && thongTinVatTuChiTietItems.Any(x => x.VatTuBenhVienId == p.VatTuBenhVienId)
                                                            && p.SoLuongNhap > p.SoLuongDaXuat).ToListAsync();
            foreach (var item in yeuCauLinhBuVatTu.YeuCauLinhVatTuChiTiets.Where(z => !thongTinVatTuChiTietItems.Any(c => c.VatTuBenhVienId == z.VatTuBenhVienId)))
            {
                item.WillDelete = true;
            }
            foreach (var yc in thongTinVatTuChiTietItems)
            {
                var sLYeuCauLinhThucTe = yc.SLYeuCauLinhThucTe;
                var sLDaBu = yc.SoLuongDaBu.MathRoundNumber(2);
                var yeuCauLinhVatTuChiTiets = yeuCauLinhBuVatTu.YeuCauLinhVatTuChiTiets.Where(p => p.LaVatTuBHYT == yc.LaVatTuBHYT && p.VatTuBenhVienId == yc.VatTuBenhVienId).ToList();
                var soLuongTonHienTai = nhapKhoVatTuChiTiets
                                        .Where(p => p.VatTuBenhVienId == yc.VatTuBenhVienId && p.LaVatTuBHYT == yc.LaVatTuBHYT).Sum(p => p.SoLuongNhap - p.SoLuongDaXuat);
                if (soLuongTonHienTai < yc.SLYeuCauLinhThucTe)
                {
                    // throw message error
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.SoLuongTon.KhongDu"));
                }
                foreach (var yeuCauLinhVatTuChiTiet in yeuCauLinhVatTuChiTiets)
                {
                    if (sLYeuCauLinhThucTe == 0)
                    {
                        yeuCauLinhVatTuChiTiet.WillDelete = true;
                    }
                    else
                    {
                        var soLuongConLai = (yeuCauLinhVatTuChiTiet.SoLuongCanBu.GetValueOrDefault() - yeuCauLinhVatTuChiTiet.SoLuongDaLinhBu.GetValueOrDefault()).MathRoundNumber(2);
                        var soLuongDaBuConLai = sLDaBu.GetValueOrDefault().MathRoundNumber(2);
                        if (soLuongDaBuConLai > sLDaBu || soLuongDaBuConLai.AlmostEqual(sLDaBu.GetValueOrDefault()))
                        {
                            yeuCauLinhVatTuChiTiet.SoLuongDaLinhBu = sLDaBu.MathRoundNumber(2);
                            sLDaBu = 0;
                        }
                        else
                        {
                            yeuCauLinhVatTuChiTiet.SoLuongDaLinhBu = soLuongDaBuConLai.MathRoundNumber(2);
                            sLDaBu = (sLDaBu - soLuongDaBuConLai).MathRoundNumber(2);

                        }
                        //SoluongConlai > SL Yeu cau
                        if (soLuongConLai > sLYeuCauLinhThucTe || soLuongConLai.AlmostEqual(sLYeuCauLinhThucTe.GetValueOrDefault()))
                        {
                            yeuCauLinhVatTuChiTiet.SoLuong = sLYeuCauLinhThucTe.Value.MathRoundNumber(2);
                            sLYeuCauLinhThucTe = 0;
                        }
                        else
                        {
                            yeuCauLinhVatTuChiTiet.SoLuong = soLuongConLai.MathRoundNumber(2);
                            sLYeuCauLinhThucTe = (sLYeuCauLinhThucTe - soLuongConLai).MathRoundNumber(2);
                        }
                    }
                }
                yeuCauLinhBuVatTu.YeuCauLinhVatTuChiTiets.Last().SoLuong += sLYeuCauLinhThucTe.GetValueOrDefault(0);
            }
        }

        public async Task<List<LookupItemVo>> GetKhoCurrentUserLinhBu(DropDownListRequestModel queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var result = await _khoNhanVienQuanLyRepository.TableNoTracking
                        .Where(p => p.NhanVienId == userCurrentId && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe))
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.KhoId,
                            DisplayName = s.Kho.Ten
                        })
                        .OrderByDescending(x => x.KeyId == khoId).ThenBy(x => x.DisplayName)
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take)
                        .ToListAsync();
            return result;
        }

        public List<long> GetIdsYeuCauVatTu(long KhoLinhTuId, long KhoLinhVeId)
        {
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(x => x.KhoLinhId == KhoLinhVeId
                                && x.YeuCauLinhVatTuId == null
                                && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                                && x.KhongLinhBu != true
                                && x.SoLuong > 0
                                && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                                )
                    .Select(item => new YeuCauLinhVatTuBuGridParentVo()
                    {
                        Id = item.Id,
                        VatTuBenhVienId = item.VatTuBenhVienId,
                        LaBHYT = item.LaVatTuBHYT,
                        TenVatTu = item.Ten,
                        Nhom = item.LaVatTuBHYT ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                        DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                        HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                        SoLuongCanBu = item.SoLuong,
                        SoLuongDaBu = item.SoLuongDaLinhBu
                    });

            var yeuCauLinhVatTuBuGridParentVos = query.ToList();

            var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == KhoLinhTuId
                     && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var result = yeuCauLinhVatTuBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));



            return result.Select(s => s.Id).ToList();
        }
        public DateTime GetDateTime(long YeuCauVatTuBenhVienId)
        {
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(s => s.Id == YeuCauVatTuBenhVienId).Select(s => (s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh));
            return query.First();
        }
        public List<long> GetIdsYeuCauVT(long KhoLinhTuId, long KhoLinhVeId,long vatTuBenhVienId)
        {
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(x => x.KhoLinhId == KhoLinhVeId
                                && x.YeuCauLinhVatTuId == null
                                && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                                && x.KhongLinhBu != true
                                && x.SoLuong > 0
                                && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                                && x.VatTuBenhVienId == vatTuBenhVienId)
                    .Select(item => new YeuCauLinhVatTuBuGridParentVo()
                    {
                        Id = item.Id,
                        VatTuBenhVienId = item.VatTuBenhVienId,
                        LaBHYT = item.LaVatTuBHYT,
                        TenVatTu = item.Ten,
                        Nhom = item.LaVatTuBHYT ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                        DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                        HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                        SoLuongCanBu = item.SoLuong,
                        SoLuongDaBu = item.SoLuongDaLinhBu
                    });

            var yeuCauLinhVatTuBuGridParentVos = query.ToList();

            var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == KhoLinhTuId
                     && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var result = yeuCauLinhVatTuBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));



            return result.Select(s => s.Id).ToList();
        }
    }
}
