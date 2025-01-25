using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using System.Linq.Dynamic.Core;
using static Camino.Core.Domain.Enums;
using System.Globalization;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Microsoft.EntityFrameworkCore.Internal;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial class YeuCauLinhKSNKService
    {
        public async Task<GridDataSource> GetYeuCauKSNKBenhVienDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<KSNKBenhVienJsonVo>(queryInfo.AdditionalSearchString);
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
                // vật tư
                var queryVT = _yeuCauVatTuBenhVienRepository.TableNoTracking
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
                               && x.KhoLinh.LaKhoKSNK == true // ksnk
                                )
                    .Select(item => new YeuCauLinhKSNKBuGridParentVo()
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
                    .Select(item => new YeuCauLinhKSNKBuGridParentVo()
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
                        SoLuongDaBu = item.Sum(x => x.SoLuongDaBu),
                        LoaiDuocPhamHayVatTu = false

                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();

                var yeuCauLinhVatTuBuGridParentVos = queryVT.ToList();

                var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == info.LinhTuKhoId
                         && x.SoLuongDaXuat < x.SoLuongNhap).ToList();




                var resultVT = yeuCauLinhVatTuBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));

                resultVT = resultVT.Select(o =>
                {
                    o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                    o.SoLuongYeuCau = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SoLuongDaBu).MathRoundNumber(2));

                    o.SLYeuCauLinhThucTe = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathCelling() : o.SoLuongCanBu.MathCelling())
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathCelling() : (o.SoLuongCanBu - o.SoLuongDaBu).MathCelling());
                    return o;
                });

                //BVHD_3909
                var yeuCauVTBenhVienIds = resultVT.Select(d => d.VatTuBenhVienId).ToList();
                var khoCap2Ids =
                    _khoRepository.TableNoTracking.Where(p => p.Id == info.LinhTuKhoId).Select(d => d.Id).ToList();

                var infoVatTus = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                 .Where(x => yeuCauVTBenhVienIds.Contains(x.VatTuBenhVienId)
                                             && khoCap2Ids.Contains(x.NhapKhoVatTu.KhoId)
                                             && x.NhapKhoVatTu.DaHet != true
                                             //&& x.LaVatTuBHYT == item.LaDuocPhamVatTuBHYT
                                             && x.SoLuongDaXuat < x.SoLuongNhap)
                                  .Select(d => new
                                  {
                                      VatTuBenhVienId = d.VatTuBenhVienId,
                                      VatTuLoaiKhoHC = d.NhapKhoVatTu.Kho.LoaiKho,
                                      VatTuNhomHC = d.VatTuBenhVien.VatTus.NhomVatTuId
                                  }).ToList();

                var inFoVatTus = new List<long>();
                foreach (var item in infoVatTus)
                {
                    if (item.VatTuLoaiKhoHC == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh && item.VatTuNhomHC == (long)Enums.EnumNhomVatTu.NhomHanhChinh)
                    {
                        inFoVatTus.Add(item.VatTuBenhVienId);
                    }
                    if (item.VatTuLoaiKhoHC != Enums.EnumLoaiKhoDuocPham.KhoHanhChinh)
                    {
                        inFoVatTus.Add(item.VatTuBenhVienId);
                    }
                }

                resultVT = resultVT.Where(d => inFoVatTus.Contains(d.VatTuBenhVienId));
                // end BVHD_3909

                // DP
                var queryDP = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
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
                               && x.KhoLinh.LaKhoKSNK == true // ksnk
                               )
                   .Select(item => new YeuCauLinhKSNKBuGridParentVo()
                   {
                       Id = item.Id,
                       VatTuBenhVienId = item.DuocPhamBenhVienId,
                       LaBHYT = item.LaDuocPhamBHYT,
                       TenVatTu = item.Ten,
                       NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                       HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                       DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                       DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                       HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                       NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                       SoLuongCanBu = item.SoLuong,
                       SoLuongDaBu = item.SoLuongDaLinhBu
                   })
                   .GroupBy(x => new { x.YeuCauLinhVatTuId, x.VatTuBenhVienId, x.LaBHYT, x.Nhom, x.NongDoHamLuong, x.HoatChat, x.DuongDung, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                   .Select(item => new YeuCauLinhKSNKBuGridParentVo()
                   {
                       YeuCauLinhVatTuIdstring = string.Join(",", item.Select(x => x.Id)),
                       VatTuBenhVienId = item.First().VatTuBenhVienId,
                       LaBHYT = item.First().LaBHYT,
                       TenVatTu = item.First().TenVatTu,
                       NongDoHamLuong = item.First().NongDoHamLuong,
                       HoatChat = item.First().HoatChat,
                       DuongDung = item.First().DuongDung,
                       DonViTinh = item.First().DonViTinh,
                       HangSanXuat = item.First().HangSanXuat,
                       NuocSanXuat = item.First().NuocSanXuat,
                       SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                       SoLuongDaBu = item.Sum(x => x.SoLuongDaBu),
                       LoaiDuocPhamHayVatTu = true
                   })
                   .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                var yeuCauLinhDuocPhamBuGridParentVos = queryDP.ToList();

                var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == info.LinhTuKhoId
                         && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                var resultDP = yeuCauLinhDuocPhamBuGridParentVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.VatTuBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                resultDP = resultDP.Select(o =>
                {
                    o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.VatTuBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                    o.SoLuongYeuCau = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SoLuongDaBu).MathRoundNumber(2));

                    o.SLYeuCauLinhThucTe = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathCelling() : o.SoLuongCanBu.MathCelling())
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathCelling() : (o.SoLuongCanBu - o.SoLuongDaBu).MathCelling());
                    return o;
                });


                var result = resultVT.Union(resultDP);

                var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                var queryTask = result.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }
            else
            {
                BuildDefaultSortExpression(queryInfo);
                var vatTuBuGridParentVos = new List<YeuCauLinhKSNKBuGridParentVo>();
                var queryVT = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                        .Where(p => p.YeuCauLinhVatTuId == info.YeuCauLinhVatTuId
                            && p.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                             && (p.YeuCauVatTuBenhVien != null
                                && ((p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null
                                    && p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay
                                    && p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay)
                                    || (p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri == null && p.YeuCauVatTuBenhVien.ThoiDiemChiDinh >= tuNgay && p.YeuCauVatTuBenhVien.ThoiDiemChiDinh <= denNgay))
                                )
                        && p.YeuCauVatTuBenhVien.KhoLinh.LaKhoKSNK == true )
                        .Select(s => new YeuCauLinhKSNKBuGridParentVo
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
                          .Select(item => new YeuCauLinhKSNKBuGridParentVo()
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
                              LoaiDuocPhamHayVatTu = false
                          })
                          .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                vatTuBuGridParentVos = queryVT.ToList();

                var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                            .Where(x => x.NhapKhoVatTu.KhoId == info.LinhTuKhoId)
                                            //&& x.SoLuongDaXuat < x.SoLuongNhap
                                            .ToList();

                var resultVT = vatTuBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));

                resultVT = resultVT.Select(o =>
                {
                    o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                    o.SoLuongYeuCau = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                    : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SoLuongDaBu).MathRoundNumber(2));
                    return o;
                });

                //BVHD_3909
                var yeuCauVTBenhVienIds = resultVT.Select(d => d.VatTuBenhVienId).ToList();
                var khoCap2Ids =
                     _khoRepository.TableNoTracking.Where(p => p.Id == info.LinhTuKhoId).Select(d => d.Id).ToList();

                var infoVatTus = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                 .Where(x => yeuCauVTBenhVienIds.Contains(x.VatTuBenhVienId)
                                             && khoCap2Ids.Contains(x.NhapKhoVatTu.KhoId)
                                             && x.NhapKhoVatTu.DaHet != true
                                             //&& x.LaVatTuBHYT == item.LaDuocPhamVatTuBHYT
                                             && x.SoLuongDaXuat < x.SoLuongNhap)
                                  .Select(d => new
                                  {
                                      VatTuBenhVienId = d.VatTuBenhVienId,
                                      VatTuLoaiKhoHC = d.NhapKhoVatTu.Kho.LoaiKho,
                                      VatTuNhomHC = d.VatTuBenhVien.VatTus.NhomVatTuId
                                  }).ToList();

                var inFoVatTus = new List<long>();
                foreach (var item in infoVatTus)
                {
                    if (item.VatTuLoaiKhoHC == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh && item.VatTuNhomHC == (long)Enums.EnumNhomVatTu.NhomHanhChinh)
                    {
                        inFoVatTus.Add(item.VatTuBenhVienId);
                    }
                    if (item.VatTuLoaiKhoHC != Enums.EnumLoaiKhoDuocPham.KhoHanhChinh)
                    {
                        inFoVatTus.Add(item.VatTuBenhVienId);
                    }
                }

                resultVT = resultVT.Where(d => inFoVatTus.Contains(d.VatTuBenhVienId));
                // end BVHD_3909



                var queryDP = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                         .Where(p => p.YeuCauLinhDuocPhamId == info.YeuCauLinhVatTuId
                             && p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                              && (p.YeuCauDuocPhamBenhVien != null
                               && ((p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null
                                   && p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay
                                   && p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay)
                                   || (p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri == null && p.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh >= tuNgay && p.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh <= denNgay))
                               )
                             && p.YeuCauDuocPhamBenhVien.KhoLinh.LaKhoKSNK == true)
                         .Select(s => new YeuCauLinhKSNKBuGridParentVo
                         {
                             Id = s.Id,
                             YeuCauLinhVatTuId = s.YeuCauLinhDuocPhamId,
                             VatTuBenhVienId = s.DuocPhamBenhVienId,
                             TenVatTu = s.DuocPhamBenhVien.DuocPham.Ten,
                             HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                             //HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
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
                         }).GroupBy(x => new { x.YeuCauLinhVatTuId, x.VatTuBenhVienId, x.LaBHYT, x.Nhom, x.HoatChat, x.DuongDung, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                           .Select(item => new YeuCauLinhKSNKBuGridParentVo()
                           {
                               VatTuBenhVienId = item.First().VatTuBenhVienId,
                               LaBHYT = item.First().LaBHYT,
                               TenVatTu = item.First().TenVatTu,
                               Nhom = item.First().Nhom,
                               //HamLuong = item.First().HamLuong,
                               HoatChat = item.First().HoatChat,
                               DuongDung = item.First().DuongDung,
                               DonViTinh = item.First().DonViTinh,
                               HangSanXuat = item.First().HangSanXuat,
                               NuocSanXuat = item.First().NuocSanXuat,
                               SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                               SoLuongDaBu = item.Sum(x => x.SoLuongDaBu),
                               SoLuongDuocDuyet = item.Sum(x => x.SoLuongDuocDuyet),
                               SLYeuCauLinhThucTe = Convert.ToInt32(item.Sum(x => x.SLYeuCauLinhThucTe).MathCelling()),
                               LoaiDuocPhamHayVatTu = true
                           })
                           .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                var duocPhamLinhBuGridVos = queryDP.ToList();

                var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                    .Where(p => p.NhapKhoDuocPhams.KhoId == info.LinhTuKhoId)
                                    //&& p.SoLuongDaXuat < p.SoLuongNhap)
                                    .ToList();

                var resultDP = duocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.VatTuBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));
                resultDP = resultDP.Select(o =>
                {
                    o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.VatTuBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                    o.SoLuongYeuCau = (o.SoLuongDaBu == 0 || o.SoLuongDaBu == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                    : (o.SoLuongTon < (o.SoLuongCanBu - o.SoLuongDaBu) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SoLuongDaBu).MathRoundNumber(2));
                    return o;
                });

                var result = resultVT.Union(resultDP);



                var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                var queryTask = result.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }
        }

        public async Task<GridDataSource> GetYeuCauKSNKBenhVienTotalPageForGridAsync(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<GridDataSource> GetBenhNhanTheoKSNKDataForGridAsync(QueryInfo queryInfo)
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

            var loaiDuocPhamHayVatTu = bool.Parse(queryObj[9]);

            if(loaiDuocPhamHayVatTu == true)
            {
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
                               .Where(o => o.DuocPhamBenhVienId == vatTuBenhVienId
                                      && o.LaDuocPhamBHYT == laBHYT
                                      && o.YeuCauDuocPhamBenhVien.KhoLinhId == khoLinhId
                                      && o.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                      && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                      && o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                         && (o.YeuCauDuocPhamBenhVien != null && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh >= tuNgay && o.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh <= denNgay)
                                         && o.YeuCauDuocPhamBenhVien.KhoLinh.LaKhoKSNK == true)
                                      .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                                      .Select(s => new KSNKLinhBuCuaNBGridVos
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
                            .Where(o => o.DuocPhamBenhVienId == vatTuBenhVienId
                                      && o.LaDuocPhamBHYT == laBHYT
                                      && o.YeuCauDuocPhamBenhVien.KhoLinhId == khoLinhId
                                      && o.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                      && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                      && o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                      && o.YeuCauDuocPhamBenhVien.KhoLinh.LaKhoKSNK == true)
                                      .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                                      .Select(s => new KSNKLinhBuCuaNBGridVos
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
                    .Where(o => o.DuocPhamBenhVienId == vatTuBenhVienId // dpbvien id
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
                                
                                && o.KhoLinh.LaKhoKSNK == true)
                    .OrderBy(x => x.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaNBGridVos
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
            else
            {
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
                              && o.YeuCauVatTuBenhVien.KhoLinh.LaKhoKSNK == true)
                      .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                      .Select(s => new KSNKLinhBuCuaNBGridVos
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
                             && o.YeuCauVatTuBenhVien.KhoLinh.LaKhoKSNK == true)
                      .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                      .Select(s => new KSNKLinhBuCuaNBGridVos
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
                                && o.KhoLinh.LaKhoKSNK == true)
                    .OrderBy(x => x.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaNBGridVos
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
                    var KSNKLinhBuCuaNBGridVos = query.ToList();
                    foreach (var item in KSNKLinhBuCuaNBGridVos)
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
                    var result = KSNKLinhBuCuaNBGridVos.Where(p => p.SLYeuCau > 0);
                    var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                    var queryTask = result.ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
            }

          
        }
        public async Task<GridDataSource> GetBenhNhanTheoKSNKTotalPageForGridAsync(QueryInfo queryInfo)
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

            var loaiDuocPhamHayVatTu = bool.Parse(queryObj[9]);

          

            if (loaiDuocPhamHayVatTu == true)
            {
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
                               .Where(o => o.DuocPhamBenhVienId == vatTuBenhVienId
                                      && o.LaDuocPhamBHYT == laBHYT
                                      && o.YeuCauDuocPhamBenhVien.KhoLinhId == khoLinhId
                                      && o.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                      && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                      && o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                         && (o.YeuCauDuocPhamBenhVien != null && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay && o.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay || o.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh >= tuNgay && o.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh <= denNgay)
                                         && o.YeuCauDuocPhamBenhVien.KhoLinh.LaKhoKSNK == true)
                                      .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                                      .Select(s => new KSNKLinhBuCuaNBGridVos
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
                        
                        var result = query;
                        var countTask = result.Count();
                        return new GridDataSource { TotalRowCount = countTask };
                    }
                    else
                    {
                        var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(o => o.DuocPhamBenhVienId == vatTuBenhVienId
                                      && o.LaDuocPhamBHYT == laBHYT
                                      && o.YeuCauDuocPhamBenhVien.KhoLinhId == khoLinhId
                                      && o.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                      && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                      && o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                      && o.YeuCauDuocPhamBenhVien.KhoLinh.LaKhoKSNK == true)
                                      .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                                      .Select(s => new KSNKLinhBuCuaNBGridVos
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
                        var result = query;
                        var countTask = result.Count();
                        return new GridDataSource { TotalRowCount = countTask };
                    }

                }
                else
                {
                    var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.DuocPhamBenhVienId == vatTuBenhVienId // dpbvien id
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

                                && o.KhoLinh.LaKhoKSNK == true)
                    .OrderBy(x => x.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaNBGridVos
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
                    var countTask = result.Count();
                    return new GridDataSource { TotalRowCount = countTask };
                }
            }
            else
            {
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
                              && o.YeuCauVatTuBenhVien.KhoLinh.LaKhoKSNK == true)
                      .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                      .Select(s => new KSNKLinhBuCuaNBGridVos
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
                        var result = query.Where(p => p.SLYeuCau > 0);
                        var countTask = result.Count();
                        return new GridDataSource { TotalRowCount = countTask };
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
                             && o.YeuCauVatTuBenhVien.KhoLinh.LaKhoKSNK == true)
                      .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                      .Select(s => new KSNKLinhBuCuaNBGridVos
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
                                && o.KhoLinh.LaKhoKSNK == true)
                    .OrderBy(x => x.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaNBGridVos
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
                    var kSNKLinhBuCuaNBGridVos = query.ToList();
                    foreach (var item in kSNKLinhBuCuaNBGridVos)
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
                    var result = kSNKLinhBuCuaNBGridVos.Where(p => p.SLYeuCau > 0);
                    var countTask = result.Count();
                    return new GridDataSource { TotalRowCount = countTask };
                }
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

        public async Task XuLyThemYeuCauLinhKSNKBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<ThongTinKSNKTietItem> thongTinVatTuChiTietItems)
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

        public async Task XuLyCapNhatYeuCauLinhKSNKBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<ThongTinKSNKTietItem> thongTinVatTuChiTietItems)
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

        public List<long> GetIdsYeuCauKSNK(long KhoLinhTuId, long KhoLinhVeId)
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
                    .Select(item => new YeuCauLinhKSNKBuGridParentVo()
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
        //public List<long> GetIdsYeuCauVT(long KhoLinhTuId, long KhoLinhVeId, long vatTuBenhVienId)
        //{
        //    var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
        //            .Where(x => x.KhoLinhId == KhoLinhVeId
        //                        && x.YeuCauLinhVatTuId == null
        //                        && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
        //                        && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
        //                        && x.KhongLinhBu != true
        //                        && x.SoLuong > 0
        //                        && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
        //                        && x.VatTuBenhVienId == vatTuBenhVienId)
        //            .Select(item => new YeuCauLinhKSNKBuGridParentVo()
        //            {
        //                Id = item.Id,
        //                VatTuBenhVienId = item.VatTuBenhVienId,
        //                LaBHYT = item.LaVatTuBHYT,
        //                TenVatTu = item.Ten,
        //                Nhom = item.LaVatTuBHYT ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
        //                DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
        //                HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
        //                NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
        //                SoLuongCanBu = item.SoLuong,
        //                SoLuongDaBu = item.SoLuongDaLinhBu
        //            });

        //    var yeuCauLinhVatTuBuGridParentVos = query.ToList();

        //    var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == KhoLinhTuId
        //             && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

        //    var result = yeuCauLinhVatTuBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));



        //    return result.Select(s => s.Id).ToList();
        //}
        public string InPhieuLinhBuKSNK(PhieuLinhThuongDPVTModel phieuLinhThuongKSNK)
        {
            var listHTML = new List<string>();
            var headerTitile = "<div class=\'wrap\'><div class=\'content\'>PHIẾU LĨNH </div></div>";
            if (phieuLinhThuongKSNK.YeuCauLinhVatTuIds.Where(d => d.LoaiDuocPhamHayVatTu == true).Count() != 0)
            {

                var hearder = string.Empty;

                var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuongThuocDuocPham")).First();
                var templateLinhThuongGayNghien = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuocGayNghien")).First();

                int? nhomVatTu = 0;
                string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
                if (string.IsNullOrEmpty(nhomVatTuString))
                {
                    nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
                }

                var yeuCauLinhDuocPhamIds = phieuLinhThuongKSNK.YeuCauLinhVatTuIds
                                        .Where(d => d.LoaiDuocPhamHayVatTu == true)
                                        .Select(d => d.YeuCauLinhVatTuId).Distinct().ToList();


                var infoYeuCauLinhDuocPhams = _yeuCauLinhDuocPhamRepository.TableNoTracking
                                                         .Where(d => yeuCauLinhDuocPhamIds.Contains(d.Id))
                                                                          .Select(d => new {
                                                                              MaVachPhieuLinh = d.SoPhieu,
                                                                              BarCodeImgBase64 = d.SoPhieu,
                                                                              TuNgay = d.ThoiDiemLinhTongHopTuNgay != null ? d.ThoiDiemLinhTongHopTuNgay.GetValueOrDefault().ApplyFormatDate() : "&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                              DenNgay = d.ThoiDiemLinhTongHopDenNgay != null ? d.ThoiDiemLinhTongHopTuNgay.GetValueOrDefault().ApplyFormatDate() : "&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                              DienGiai = d.GhiChu,
                                                                              NoiGiao = d.KhoXuat.Ten,
                                                                              NhapVeKho = d.KhoNhap.Ten,
                                                                              YeuCauLinhChiTietIds = d.YeuCauLinhDuocPhamChiTiets.Select(g => g.Id).ToList(),
                                                                              YeuCauLinhId = d.Id
                                                                          }).ToList();

                foreach (var item in infoYeuCauLinhDuocPhams.ToList())
                {
                    var content = string.Empty;
                    var contentGayNghien = string.Empty;

                    var groupThuocBHYT = "Thuốc BHYT";
                    var groupThuocKhongBHYT = "Không BHYT";

                    if (phieuLinhThuongKSNK.Header == true)
                    {
                        hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                                      "<th>PHIẾU LĨNH KNSK</th>" +
                                 "</p>";
                    }

                    var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                                + "</b></tr>";
                    var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                                + "</b></tr>";
                    var query = new List<ThongTinLinhDuocPhamChiTiet>();
                    var queryGayNghien = new List<ThongTinLinhDuocPhamChiTiet>();

                    query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(x => x.YeuCauLinhDuocPhamId == item.YeuCauLinhId
                                    && x.YeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien
                                    && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan))
                     .Select(s => new ThongTinLinhDuocPhamChiTiet
                     {
                         DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                         Ma = s.DuocPhamBenhVien.Ma,
                         TenThuocHoacVatTu = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == nhomVatTu ?
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NhaSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                       (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NuocSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.HamLuong) ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                         DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                         SLYeuCau = s.SoLuong,
                         LaDuocPhamBHYT = s.LaDuocPhamBHYT
                     }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
                     .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.DVT })
                     .Select(items => new ThongTinLinhDuocPhamChiTiet()
                     {
                         Ma = items.First().Ma,
                         TenThuocHoacVatTu = items.First().TenThuocHoacVatTu,
                         DVT = items.First().DVT,
                         SLYeuCau = items.Sum(x => x.SLYeuCau).MathRoundNumber(2),
                         LaDuocPhamBHYT = items.First().LaDuocPhamBHYT
                     }).OrderByDescending(z => z.LaDuocPhamBHYT).ThenBy(z => z.TenThuocHoacVatTu).ToList();

                    queryGayNghien = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                               .Where(x => x.YeuCauLinhDuocPhamId == item.YeuCauLinhId
                                       && x.YeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien
                                       && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                        .Select(s => new ThongTinLinhDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                            Ma = s.DuocPhamBenhVien.Ma,
                            TenThuocHoacVatTu = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == nhomVatTu ?
                                                                        s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NhaSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                          (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NuocSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                        s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.HamLuong) ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                            DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                            SLYeuCau = s.SoLuong,
                            LaDuocPhamBHYT = s.LaDuocPhamBHYT
                        }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
                        .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.DVT })
                        .Select(items => new ThongTinLinhDuocPhamChiTiet()
                        {
                            Ma = items.First().Ma,
                            TenThuocHoacVatTu = items.First().TenThuocHoacVatTu,
                            DVT = items.First().DVT,
                            SLYeuCau = items.Sum(x => x.SLYeuCau).MathRoundNumber(2),
                            LaDuocPhamBHYT = items.First().LaDuocPhamBHYT
                        }).OrderByDescending(z => z.LaDuocPhamBHYT).ThenBy(z => z.TenThuocHoacVatTu).ToList();

                    if (query.Any())
                    {
                        var objData = GetHTMLLinhBuKSNK(query, false);

                        var data = new PhieuLinhThuongDuocPhamData
                        {
                            HeaderPhieuLinhThuoc = hearder,
                            MaVachPhieuLinh = item.MaVachPhieuLinh,
                            ThuocHoacVatTu = objData.html,
                            LogoUrl = phieuLinhThuongKSNK.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(item.BarCodeImgBase64),
                            TuNgay = item.TuNgay,
                            DenNgay = item.DenNgay,
                            DienGiai = item.DienGiai,
                            NoiGiao = item.NoiGiao,
                            NhapVeKho = item.NhapVeKho,
                            CongKhoan = "Cộng khoản: " + (objData.Index - 1).ToString() + " khoản",
                            Ngay = DateTime.Now.Day.ConvertDateToString(),
                            Thang = DateTime.Now.Month.ConvertMonthToString(),
                            Nam = DateTime.Now.Year.ConvertYearToString()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
                        listHTML.Add(content);
                    }
                    if (queryGayNghien.Any())
                    {
                        var objData = GetHTMLLinhBuKSNK(queryGayNghien, true);

                        var data = new PhieuLinhThuongDuocPhamData
                        {
                            HeaderPhieuLinhThuoc = hearder,
                            MaVachPhieuLinh = item.MaVachPhieuLinh,
                            ThuocHoacVatTu = objData.html,
                            LogoUrl = phieuLinhThuongKSNK.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(item.BarCodeImgBase64),
                            TuNgay = item.TuNgay,
                            DenNgay = item.DenNgay,
                            DienGiai = item.DienGiai,
                            NoiGiao = item.NoiGiao,
                            NhapVeKho = item.NhapVeKho,
                            CongKhoan = "Cộng khoản: " + (objData.Index - 1).ToString() + " khoản",
                            Ngay = DateTime.Now.Day.ConvertDateToString(),
                            Thang = DateTime.Now.Month.ConvertMonthToString(),
                            Nam = DateTime.Now.Year.ConvertYearToString()
                        };
                        contentGayNghien = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongGayNghien.Body, data);
                        listHTML.Add(content);
                    }
                }

            }
            if (phieuLinhThuongKSNK.YeuCauLinhVatTuIds.Where(d => d.LoaiDuocPhamHayVatTu == false).Count() != 0)
            {
                var yeuCauLinhVatTuIds = phieuLinhThuongKSNK.YeuCauLinhVatTuIds
                                        .Where(d => d.LoaiDuocPhamHayVatTu == false)
                                        .Select(d => d.YeuCauLinhVatTuId).Distinct().ToList();

                var templateLinhThuongVT = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuongVatTu")).First();


                // get tat ca cac phieu can  vua tao theo yeuCauLinhVatTuIds

                var infoYeuCauLinhVatTus = BaseRepository.TableNoTracking
                                                         .Where(d => yeuCauLinhVatTuIds.Contains(d.Id))
                                                                          .Select(d => new {
                                                                              MaVachPhieuLinh = d.SoPhieu,
                                                                              BarCodeImgBase64 = d.SoPhieu,
                                                                              TuNgay = d.ThoiDiemLinhTongHopTuNgay != null ? d.ThoiDiemLinhTongHopTuNgay.GetValueOrDefault().ApplyFormatDate() : "&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                              DenNgay = d.ThoiDiemLinhTongHopDenNgay != null ? d.ThoiDiemLinhTongHopTuNgay.GetValueOrDefault().ApplyFormatDate() : "&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                              DienGiai = d.GhiChu,
                                                                              NoiGiao = d.KhoXuat.Ten,
                                                                              NhapVeKho = d.KhoNhap.Ten,
                                                                              YeuCauLinhChiTietIds = d.YeuCauLinhVatTuChiTiets.Select(g => g.Id).ToList(),
                                                                              YeuCauLinhId = d.Id
                                                                          }).ToList();


                foreach (var item in infoYeuCauLinhVatTus.ToList())
                {
                    var content = string.Empty;
                    var hearder = string.Empty;

                    if (phieuLinhThuongKSNK.Header == true)
                    {
                        hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                                      "<th>PHIẾU LĨNH KNSK</th>" +
                                 "</p>";
                    }

                    var infoLinhDuocChiTiet = string.Empty;
                    var groupThuocBHYT = "Vật Tư BHYT";
                    var groupThuocKhongBHYT = "Vật Tư Không BHYT";

                    var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                                + "</b></tr>";
                    var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                                + "</b></tr>";
                    var STT = 1;
                    var query = new List<ThongTinLinhVatTuChiTiet>();
                     query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.YeuCauLinhVatTuId == item.YeuCauLinhId
                                    && x.YeuCauVatTuBenhVien.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien
                                    )
                     .Select(s => new ThongTinLinhVatTuChiTiet
                     {
                         VatTuBenhVienId = s.VatTuBenhVienId,
                         Ma = s.VatTuBenhVien.Ma,
                         TenThuocHoacVatTu = s.VatTuBenhVien.VatTus.Ten
                                             + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NhaSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "")
                                             + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NuocSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                         DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                         SLYeuCau = s.SoLuong,
                         LaVatTuBHYT = s.LaVatTuBHYT
                     }).OrderByDescending(p => p.LaVatTuBHYT).ThenBy(p => !p.LaVatTuBHYT)
                     .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.DVT })
                     .Select(s => new ThongTinLinhVatTuChiTiet()
                     {
                         Ma = s.First().Ma,
                         TenThuocHoacVatTu = s.First().TenThuocHoacVatTu,
                         DVT = s.First().DVT,
                         SLYeuCau = s.Sum(x => x.SLYeuCau).Value.MathRoundNumber(2),
                         LaVatTuBHYT = s.First().LaVatTuBHYT
                     }).OrderByDescending(z => z.LaVatTuBHYT).ThenBy(z => z.TenThuocHoacVatTu).ToList();

                    foreach (var itemChiTiet in query)
                    {
                        infoLinhDuocChiTiet +=  "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.Ma
                                                + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.TenThuocHoacVatTu
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.DVT
                                                + "<td style = 'border: 1px solid #020000;text-align: right;'>" + itemChiTiet.SLYeuCau
                                                + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"/*item.SLThucPhat*/
                                                + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.GhiChu
                                                + "</tr>";
                        STT++;
                    }
                    var data = new PhieuLinhThuongKSNKData
                    {
                        HeaderPhieuLinhThuoc = hearder,
                        MaVachPhieuLinh = "Số: " + item.MaVachPhieuLinh,
                        ThuocHoacVatTu = infoLinhDuocChiTiet,
                        LogoUrl = phieuLinhThuongKSNK.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(item.BarCodeImgBase64),
                        TuNgay = item.TuNgay,
                        DenNgay = item.DenNgay,
                        DienGiai = item.DienGiai,
                        NoiGiao = item.NoiGiao,
                        NhapVeKho = item.NhapVeKho,
                        CongKhoan = "Cộng khoản: " + (STT - 1).ToString() + " khoản",
                        Ngay = DateTime.Now.Day.ConvertDateToString(),
                        Thang = DateTime.Now.Month.ConvertMonthToString(),
                        Nam = DateTime.Now.Year.ConvertYearToString()
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongVT.Body, data);
                    listHTML.Add(content);
                }
            }

            return listHTML.Join("<div class=\"pagebreak\"> </div>");
        }
        private OBJKSNKList GetHTMLLinhBuKSNK(List<ThongTinLinhDuocPhamChiTiet> gridVos, bool loaiThuoc)
        {
            int STT = 1;
            var infoLinhDuocChiTiet = string.Empty;
            foreach (var item in gridVos)
            {
                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                         + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                         + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                         + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                         + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                         + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                         + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLYeuCau), false) : item.SLYeuCau.MathRoundNumber(2) + "")
                                         + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"/* (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLThucPhat), false) : item.SLThucPhat.MathRoundNumber(2) + "")*/
                                         + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                         + "</tr>";
                STT++;
            }
            var data = new OBJKSNKList
            {
                html = infoLinhDuocChiTiet,
                Index = STT
            };
            return data;
        }
    }
}
