using Camino.Core.Data;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DanhSachYeuCauHoanTra.DuocPham;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauHoanTra.DuocPham
{
    public partial class YeuCauHoanTraDuocPhamService
    {
        public async Task<GridDataSource> GetDataForGridAsyncDuocPhamTuTrucDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauTraDuocPhamTuTrucVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dp => dp.DonViTinh)
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                    .Include(nkct => nkct.NhapKhoDuocPhams)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoDuocPhams.KhoDuocPhams.Id == info.KhoXuatId);
            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTiets.ApplyLike(searchTerms,
                    g => g.DuocPhamBenhViens.DuocPham.Ten,
                    g => g.DuocPhamBenhViens.Ma,
                    g => g.DuocPhamBenhViens.DuocPham.SoDangKy,
                    g => g.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    g => g.DuocPhamBenhViens.DuocPham.HamLuong,
                    g => g.Solo
               );
            }
            var yeuCauTraDuocPhamChiTiets = new List<YeuCauTraDuocPhamGridVo>();
            var nhapKhoDuocPhamChiTietGroup = nhapKhoDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhViens.Ma, x.DuocPhamBenhViens.DuocPham.Ten, x.DuocPhamBenhViens.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten, x.DonGiaNhap })
                                          .Select(g => new { nhapKhoDuocPhamChiTiets = g.FirstOrDefault() });

            foreach (var item in nhapKhoDuocPhamChiTietGroup)
            {
                var yeuCauXuatKhoDuocPhamGridVo = new YeuCauTraDuocPhamGridVo
                {
                    Id = item.nhapKhoDuocPhamChiTiets.Id,
                    DuocPhamBenhVienId = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhVienId,
                    Ten = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.Ten,
                    DVT = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    LaDuocPhamBHYT = item.nhapKhoDuocPhamChiTiets.LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    TenNhom = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom?.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.Ma,
                    SoLo = item.nhapKhoDuocPhamChiTiets.Solo,
                    HanSuDung = item.nhapKhoDuocPhamChiTiets.HanSuDung,
                    DonGiaNhap = item.nhapKhoDuocPhamChiTiets.DonGiaNhap,
                    KhoXuatId = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.KhoId,
                    SoDangKy = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.SoDangKy,
                    NgayNhap = item.nhapKhoDuocPhamChiTiets.NgayNhap,
                };
                yeuCauTraDuocPhamChiTiets.Add(yeuCauXuatKhoDuocPhamGridVo);
            }

            var result = yeuCauTraDuocPhamChiTiets.Select(o =>
            {
                o.SoLuongTon = nhapKhoDuocPhamChiTiets.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaDuocPhamBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo && t.DonGiaNhap == o.DonGiaNhap).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                o.SoLuongTra = nhapKhoDuocPhamChiTiets.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaDuocPhamBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo && t.DonGiaNhap == o.DonGiaNhap).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            }).ToList();
            var yeuCauHoanTraDuocPhamGridVos = new List<YeuCauTraDuocPhamGridVo>();
            if (!info.IsCreate)
            {
                yeuCauHoanTraDuocPhamGridVos = _ycTraDpChiTiet.TableNoTracking
                                            .Where(ct => ct.YeuCauTraDuocPhamId == info.YeuCauTraDuocPhamId && ct.YeuCauTraDuocPham.KhoXuatId == info.KhoXuatId)
                                            .Select(s => new YeuCauTraDuocPhamGridVo
                                            {
                                                Id = s.Id,
                                                DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                                                Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                                DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                                                DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVienPhanNhomId,
                                                TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                                                Ma = s.DuocPhamBenhVien.Ma,
                                                SoLo = s.Solo,
                                                HanSuDung = s.HanSuDung,
                                                DonGiaNhap = s.DonGiaNhap,
                                                KhoXuatId = s.YeuCauTraDuocPham.KhoXuatId,
                                                SoLuongTra = s.SoLuongTra,
                                                NgayNhap = s.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NgayNhap,
                                                SoDangKy = s.DuocPhamBenhVien.DuocPham.SoDangKy
                                            })
                                            .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                          .Select(g => new YeuCauTraDuocPhamGridVo
                                          {
                                              Id = g.First().Id,
                                              DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaDuocPhamBHYT = g.First().LaDuocPhamBHYT,
                                              DuocPhamBenhVienPhanNhomId = g.First().DuocPhamBenhVienPhanNhomId,
                                              TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                                              Ma = g.First().Ma,
                                              SoDangKy = g.First().SoDangKy,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                                              NgayNhap = g.First().NgayNhap,
                                          }).ToList();
                result.AddRange(yeuCauHoanTraDuocPhamGridVos);
                result = result.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                          .Select(g => new YeuCauTraDuocPhamGridVo
                                          {
                                              Id = g.First().Id,
                                              DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaDuocPhamBHYT = g.First().LaDuocPhamBHYT,
                                              DuocPhamBenhVienPhanNhomId = g.First().DuocPhamBenhVienPhanNhomId,
                                              TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                                              Ma = g.First().Ma,
                                              SoDangKy = g.First().SoDangKy,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.Sum(z => z.SoLuongTon),
                                              NgayNhap = g.First().NgayNhap,
                                          }).ToList();
            }

            if (info.DuocPhamBenhVienVos.Any())
            {
                result = result.Where(x => !info.DuocPhamBenhVienVos.Any(z => z.DuocPhamBenhVienId == x.DuocPhamBenhVienId && z.LaDuocPhamBHYT == x.LaDuocPhamBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung && z.DonGiaNhap == x.DonGiaNhap)).ToList();
            }
            var query = result.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamTuTrucDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauTraDuocPhamTuTrucVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dp => dp.DonViTinh)
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                    .Include(nkct => nkct.NhapKhoDuocPhams)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoDuocPhams.KhoDuocPhams.Id == info.KhoXuatId);

            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTiets.ApplyLike(searchTerms,
                    g => g.DuocPhamBenhViens.DuocPham.Ten,
                    g => g.DuocPhamBenhViens.Ma,
                    g => g.DuocPhamBenhViens.DuocPham.SoDangKy,
                    g => g.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    g => g.DuocPhamBenhViens.DuocPham.HamLuong,
                    g => g.Solo
               );
            }
            var yeuCauTraDuocPhamChiTiets = new List<YeuCauTraDuocPhamGridVo>();
            var nhapKhoDuocPhamChiTietGroup = nhapKhoDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhViens.Ma, x.DuocPhamBenhViens.DuocPham.Ten, x.DuocPhamBenhViens.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten, x.DonGiaNhap })
                                          .Select(g => new { nhapKhoDuocPhamChiTiets = g.FirstOrDefault() });

            foreach (var item in nhapKhoDuocPhamChiTietGroup)
            {
                var yeuCauXuatKhoDuocPhamGridVo = new YeuCauTraDuocPhamGridVo
                {
                    Id = item.nhapKhoDuocPhamChiTiets.Id,
                    DuocPhamBenhVienId = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhVienId,
                    Ten = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.Ten,
                    DVT = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    LaDuocPhamBHYT = item.nhapKhoDuocPhamChiTiets.LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    TenNhom = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom?.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.Ma,
                    SoLo = item.nhapKhoDuocPhamChiTiets.Solo,
                    HanSuDung = item.nhapKhoDuocPhamChiTiets.HanSuDung,
                    DonGiaNhap = item.nhapKhoDuocPhamChiTiets.DonGiaNhap,
                    KhoXuatId = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.KhoId,
                };
                yeuCauTraDuocPhamChiTiets.Add(yeuCauXuatKhoDuocPhamGridVo);
            }
            var yeuCauHoanTraDuocPhamGridVos = new List<YeuCauTraDuocPhamGridVo>();
            if (!info.IsCreate)
            {
                yeuCauHoanTraDuocPhamGridVos = _ycTraDpChiTiet.TableNoTracking
                                            .Where(ct => ct.YeuCauTraDuocPhamId == info.YeuCauTraDuocPhamId && ct.YeuCauTraDuocPham.KhoXuatId == info.KhoXuatId)
                                            .Select(s => new YeuCauTraDuocPhamGridVo
                                            {
                                                Id = s.Id,
                                                DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                                                Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                                DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                                                DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVienPhanNhomId,
                                                TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                                                Ma = s.DuocPhamBenhVien.Ma,
                                                SoLo = s.Solo,
                                                HanSuDung = s.HanSuDung,
                                                DonGiaNhap = s.DonGiaNhap,
                                                KhoXuatId = s.YeuCauTraDuocPham.KhoXuatId,
                                                SoLuongTra = s.SoLuongTra
                                            })
                                            .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                          .Select(g => new YeuCauTraDuocPhamGridVo
                                          {
                                              Id = g.First().Id,
                                              DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaDuocPhamBHYT = g.First().LaDuocPhamBHYT,
                                              DuocPhamBenhVienPhanNhomId = g.First().DuocPhamBenhVienPhanNhomId,
                                              TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                                              Ma = g.First().Ma,
                                              SoDangKy = g.First().SoDangKy,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                                          }).ToList();
                yeuCauTraDuocPhamChiTiets.AddRange(yeuCauHoanTraDuocPhamGridVos);
                yeuCauTraDuocPhamChiTiets = yeuCauTraDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                         .Select(g => new YeuCauTraDuocPhamGridVo
                                         {
                                             Id = g.First().Id,
                                             DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                                             Ten = g.First().Ten,
                                             DVT = g.First().DVT,
                                             LaDuocPhamBHYT = g.First().LaDuocPhamBHYT,
                                             DuocPhamBenhVienPhanNhomId = g.First().DuocPhamBenhVienPhanNhomId,
                                             TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                                             Ma = g.First().Ma,
                                             SoDangKy = g.First().SoDangKy,
                                             SoLo = g.First().SoLo,
                                             HanSuDung = g.First().HanSuDung,
                                             DonGiaNhap = g.First().DonGiaNhap,
                                             KhoXuatId = g.First().KhoXuatId,
                                             SoLuongTra = g.Sum(z => z.SoLuongTra),
                                             SoLuongTon = g.Sum(z => z.SoLuongTon),
                                             NgayNhap = g.First().NgayNhap,
                                         }).ToList();
            }

            if (info.DuocPhamBenhVienVos.Any())
            {
                yeuCauTraDuocPhamChiTiets = yeuCauTraDuocPhamChiTiets.Where(x => !info.DuocPhamBenhVienVos.Any(z => z.DuocPhamBenhVienId == x.DuocPhamBenhVienId && z.LaDuocPhamBHYT == x.LaDuocPhamBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung && z.DonGiaNhap ==  x.DonGiaNhap)).ToList();
            }
            var countTask = yeuCauTraDuocPhamChiTiets.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetDataForGridChildAsyncDaDuyet(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            var yeuCauTraDuocPhamVo = JsonConvert.DeserializeObject<DanhSachDaDuyetChiTietVo>(queryInfo.AdditionalSearchString);
            var query = _ycTraDpChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamId == yeuCauTraDuocPhamVo.YeuCauTraDuocPhamId)
                .Select(s => new DanhSachYeuCauHoanTraDuocPhamChiTietGridVo
                {
                    Id = s.Id,
                    DuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                    Ma = s.DuocPhamBenhVien.Ma,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    HopDong = s.HopDongThauDuocPham.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    LaDuocPhamBhyt = s.LaDuocPhamBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                }).ApplyLike(yeuCauTraDuocPhamVo.SearchString?.Replace("\t", "").Trim(),
                    q => q.Ma,
                    q => q.DuocPham,
                    q => q.SoLo,
                    q => q.HopDong,
                    q => q.Ma);

            var groupQuery = query.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBhyt, x.Ma, x.DuocPham, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new DanhSachYeuCauHoanTraDuocPhamChiTietGridVo
                {
                    DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                    DuocPham = g.First().DuocPham,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaDuocPhamBhyt = g.First().LaDuocPhamBhyt,
                    TenNhom = g.First().TenNhom,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    MaVach = g.First().MaVach,
                    Vat = g.First().Vat,
                    TiLeThapGia = g.First().TiLeThapGia,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });
            var result = await groupQuery.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            return new GridDataSource { Data = result, TotalRowCount = result.Count() };
        }
        public async Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyet(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { TotalRowCount = 0 };
            }
            var yeuCauTraDuocPhamVo = JsonConvert.DeserializeObject<DanhSachDaDuyetChiTietVo>(queryInfo.AdditionalSearchString);
            var query = _ycTraDpChiTiet.TableNoTracking
               .Where(p => p.YeuCauTraDuocPhamId == yeuCauTraDuocPhamVo.YeuCauTraDuocPhamId)
               .Select(s => new DanhSachYeuCauHoanTraDuocPhamChiTietGridVo
               {
                   Id = s.Id,
                   DuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                   Ma = s.DuocPhamBenhVien.Ma,
                   DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                   DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                   HopDong = s.HopDongThauDuocPham.SoHopDong,
                   SoLuongTra = s.SoLuongTra,
                   HanSuDung = s.HanSuDung,
                   MaVach = s.MaVach,
                   DonGiaNhap = s.DonGiaNhap,
                   Vat = s.VAT,
                   TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   LaDuocPhamBhyt = s.LaDuocPhamBHYT,
                   SoLo = s.Solo,
                   TiLeThapGia = s.TiLeTheoThapGia,
                   NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
               }).ApplyLike(yeuCauTraDuocPhamVo.SearchString?.Replace("\t", "").Trim(),
                    q => q.Ma,
                    q => q.DuocPham,
                    q => q.SoLo,
                    q => q.HopDong,
                    q => q.Ma);
            var groupQuery = query.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBhyt, x.Ma, x.DuocPham, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new DanhSachYeuCauHoanTraDuocPhamChiTietGridVo
                {
                    DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                    DuocPham = g.First().DuocPham,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaDuocPhamBhyt = g.First().LaDuocPhamBhyt,
                    TenNhom = g.First().TenNhom,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    MaVach = g.First().MaVach,
                    Vat = g.First().Vat,
                    TiLeThapGia = g.First().TiLeThapGia,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    NgayNhapVaoBenhVien = g.First().NgayNhapVaoBenhVien,
                });
            var countTask = groupQuery.CountAsync();
            return new GridDataSource { TotalRowCount = await countTask };
        }

        public async Task<TrangThaiDuyetVo> GetTrangThaiYeuCauHoanTraDuocPham(long phieuLinhId)
        {
            var yeuCauLinhDuocPham = await BaseRepository.TableNoTracking.Where(p => p.Id == phieuLinhId).FirstAsync();
            var trangThaiVo = new TrangThaiDuyetVo();
            //if (yeuCauLinhDuocPham.DaGui != true)
            //{
            //    trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoGui;
            //    trangThaiVo.Ten = EnumTrangThaiPhieuLinh.DangChoGui.GetDescription();
            //    trangThaiVo.TrangThai = null;
            //    return trangThaiVo;
            //}
            //else
            //{
            if (yeuCauLinhDuocPham.DuocDuyet == true)
            {
                trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                trangThaiVo.TrangThai = true;
                trangThaiVo.Ten = EnumTrangThaiPhieuLinh.DaDuyet.GetDescription();
                return trangThaiVo;
            }
            else if (yeuCauLinhDuocPham.DuocDuyet == false)
            {
                trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.TuChoiDuyet;
                trangThaiVo.TrangThai = false;
                trangThaiVo.Ten = EnumTrangThaiPhieuLinh.TuChoiDuyet.GetDescription();
                return trangThaiVo;
            }
            else
            {
                trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoDuyet;
                trangThaiVo.TrangThai = null;
                trangThaiVo.Ten = EnumTrangThaiPhieuLinh.DangChoDuyet.GetDescription();
                return trangThaiVo;
            }
            //}

        }

        public async Task<List<YeuCauTraDuocPhamGridVo>> YeuCauTraDuocPhamTuTrucChiTiets(long yeuCauTraDuocPhamId)
        {
            var query = _ycTraDpChiTiet.TableNoTracking.Where(z => z.YeuCauTraDuocPhamId == yeuCauTraDuocPhamId)
                .Select(s => new YeuCauTraDuocPhamGridVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                    TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = s.DuocPhamBenhVien.Ma,
                    SoDangKy = s.DuocPhamBenhVien.DuocPham.SoDangKy,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    DonGiaNhap = s.DonGiaNhap,
                    KhoXuatId = s.YeuCauTraDuocPham.KhoXuatId,
                    SoLuongTra = s.SoLuongTra,
                    SoLuongTon = s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(z => z.DuocPhamBenhVienId == s.DuocPhamBenhVienId && z.LaDuocPhamBHYT == s.LaDuocPhamBHYT && z.NhapKhoDuocPhams.KhoId == s.YeuCauTraDuocPham.KhoXuatId && z.HanSuDung == s.HanSuDung && z.Solo == s.Solo && z.DonGiaNhap == s.DonGiaNhap).Sum(z => z.SoLuongNhap - z.SoLuongDaXuat),
                    NgayNhap = s.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NgayNhap
                })
                .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new YeuCauTraDuocPhamGridVo
                {
                    Id = g.First().Id,
                    DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                    Ten = g.First().Ten,
                    DVT = g.First().DVT,
                    LaDuocPhamBHYT = g.First().LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = g.First().DuocPhamBenhVienPhanNhomId,
                    TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                    Ma = g.First().Ma,
                    SoDangKy = g.First().SoDangKy,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    KhoXuatId = g.First().KhoXuatId,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                    NgayNhap = g.First().NgayNhap,
                }).ToList();
            return query;
        }

        private async Task XoaChiTietHoanTra(YeuCauTraDuocPham yeuCauTraDuocPham)
        {
            foreach (var chiTiet in yeuCauTraDuocPham.YeuCauTraDuocPhamChiTiets)
            {
                chiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat =
                                                                    (chiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - chiTiet.SoLuongTra).MathRoundNumber(2);
                chiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                chiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                chiTiet.WillDelete = true;
            }
            await BaseRepository.UpdateAsync(yeuCauTraDuocPham);
        }
        public async Task XuLyThemHoacCapNhatHoanTraThuocAsync(YeuCauTraDuocPham yeuCauTraDuocPham, List<YeuCauTraDuocPhamTuTrucChiTietVo> yeuCauTraDuocPhamTuTrucChiTiets, bool isCreate)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            // xóa các chi tiết đã tạo yêu cầu chưa duyệt
            if (!isCreate)
            {
                await XoaChiTietHoanTra(yeuCauTraDuocPham);
            }
            // thêm các chi tiết mới
            var khoXuatIds = yeuCauTraDuocPhamTuTrucChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
            var nhapKhoDuocPhamChiTietAlls = _nhapKhoDuocPhamChiTietRepository.Table
                    .Include(nk => nk.DuocPhamBenhViens)
                    .Include(nk => nk.NhapKhoDuocPhams)
                  .Where(o =>
                  khoXuatIds.Contains(o.NhapKhoDuocPhams.KhoId)
                  && o.SoLuongNhap > o.SoLuongDaXuat)
                  .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();

            foreach (var chiTiet in yeuCauTraDuocPhamTuTrucChiTiets)
            {
                var nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTietAlls.Where(o => o.NhapKhoDuocPhams.KhoId == chiTiet.KhoXuatId
                     && o.LaDuocPhamBHYT == chiTiet.LaDuocPhamBHYT
                     && o.DuocPhamBenhVienId == chiTiet.DuocPhamBenhVienId
                     && o.Solo == chiTiet.SoLo
                     && o.HanSuDung == chiTiet.HanSuDung
                     && o.DonGiaNhap == chiTiet.DonGiaNhap).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();

                var SLTon = nhapKhoDuocPhamChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat);

                if (SLTon < chiTiet.SoLuongTra)
                {
                    throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                }

                var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                {
                    DuocPhamBenhVienId = chiTiet.DuocPhamBenhVienId,
                };
                var yeuCauTraDuocPhamChiTietNews = new List<YeuCauTraDuocPhamChiTiet>();
                var soLuongTra = chiTiet.SoLuongTra ?? 0;// số lượng trả
                foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                {
                    if (soLuongTra == 0)
                    {
                        break;
                    }
                    var yeuCauTraDuocPhamChiTietNew = new YeuCauTraDuocPhamChiTiet
                    {
                        DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                        LaDuocPhamBHYT = nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                        Solo = nhapKhoDuocPhamChiTiet.Solo,
                        HanSuDung = nhapKhoDuocPhamChiTiet.HanSuDung,
                        NgayNhapVaoBenhVien = nhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien,
                        DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                        TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                        VAT = nhapKhoDuocPhamChiTiet.VAT,
                        MaVach = nhapKhoDuocPhamChiTiet.MaVach,
                        MaRef = nhapKhoDuocPhamChiTiet.MaRef,
                        DuocPhamBenhVienPhanNhomId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId ?? nhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                        KhoViTriId = nhapKhoDuocPhamChiTiet.KhoViTriId
                    };
                    var SLTonHienTai = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                    if (SLTonHienTai > soLuongTra || SLTonHienTai.AlmostEqual(soLuongTra))
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat = (nhapKhoDuocPhamChiTiet.SoLuongDaXuat + soLuongTra).MathRoundNumber(2);
                        yeuCauTraDuocPhamChiTietNew.SoLuongTra = soLuongTra;
                        soLuongTra = 0;
                    }
                    else
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat = nhapKhoDuocPhamChiTiet.SoLuongNhap;
                        yeuCauTraDuocPhamChiTietNew.SoLuongTra = SLTonHienTai;
                        soLuongTra -= SLTonHienTai;
                    }
                    var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                    {
                        SoLuongXuat = yeuCauTraDuocPhamChiTietNew.SoLuongTra,
                        GhiChu = "Hoàn trả thuốc tủ trực.",
                        XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet,
                    };
                    yeuCauTraDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri = xuatKhoDuocPhamChiTietViTri;
                    yeuCauTraDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet;
                    yeuCauTraDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet;
                    yeuCauTraDuocPham.YeuCauTraDuocPhamChiTiets.Add(yeuCauTraDuocPhamChiTietNew);
                }
            }
            await BaseRepository.UpdateAsync(yeuCauTraDuocPham);
        }
    }
}
