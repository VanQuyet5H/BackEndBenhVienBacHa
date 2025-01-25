using Camino.Core.Data;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
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
namespace Camino.Services.YeuCauHoanTra.VatTu
{
    public partial class YeuCauHoanTraVatTuService
    {
        public async Task<GridDataSource> GetDataForGridAsyncVatTuTuTrucDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauTraVatTuTuTrucVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(nkct => nkct.VatTus)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoVatTu.Kho.Id == info.KhoXuatId);
            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoVatTuChiTiets = nhapKhoVatTuChiTiets.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten,
                    g => g.VatTuBenhVien.Ma,
                    g => g.VatTuBenhVien.VatTus.DonViTinh,
                    g => g.Solo
               );
            }
            var yeuCauTraVatTuChiTiets = new List<YeuCauTraVatTuGridVo>();
            var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh, x.DonGiaNhap }).Select(g => new { nhapKhoVatTuChiTiet = g.FirstOrDefault() });

            foreach (var item in nhapKhoVatTuChiTietGroup)
            {
                var yeuCauTraVatTuChiTiet = new YeuCauTraVatTuGridVo
                {
                    Id = item.nhapKhoVatTuChiTiet.Id,
                    VatTuBenhVienId = item.nhapKhoVatTuChiTiet.VatTuBenhVienId,
                    Ten = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    Ma = item.nhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    DVT = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    LaVatTuBHYT = item.nhapKhoVatTuChiTiet.LaVatTuBHYT,
                    LoaiSuDung = item.nhapKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung,
                    SoLo = item.nhapKhoVatTuChiTiet.Solo,
                    HanSuDung = item.nhapKhoVatTuChiTiet.HanSuDung,
                    DonGiaNhap = item.nhapKhoVatTuChiTiet.DonGiaNhap,
                    KhoXuatId = item.nhapKhoVatTuChiTiet.NhapKhoVatTu.KhoId,
                    NgayNhap = item.nhapKhoVatTuChiTiet.NgayNhap,
                    VAT = item.nhapKhoVatTuChiTiet.VAT,
                    TiLeThapGia = item.nhapKhoVatTuChiTiet.TiLeTheoThapGia
                };
                yeuCauTraVatTuChiTiets.Add(yeuCauTraVatTuChiTiet);
            }

            var result = yeuCauTraVatTuChiTiets.Select(o =>
            {
                o.SoLuongTon = nhapKhoVatTuChiTiets.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo && t.DonGiaNhap == o.DonGiaNhap).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                o.SoLuongTra = nhapKhoVatTuChiTiets.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo && t.DonGiaNhap == o.DonGiaNhap).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            }).ToList();
            var yeuCauTraVatTuGridVos = new List<YeuCauTraVatTuGridVo>();
            if (!info.IsCreate)
            {
                yeuCauTraVatTuGridVos = _ycTraVtChiTiet.TableNoTracking
                                            .Where(ct => ct.YeuCauTraVatTuId == info.YeuCauTraVatTuId && ct.YeuCauTraVatTu.KhoXuatId == info.KhoXuatId)
                                            .Select(s => new YeuCauTraVatTuGridVo
                                            {
                                                Id = s.Id,
                                                VatTuBenhVienId = s.VatTuBenhVienId,
                                                Ten = s.VatTuBenhVien.VatTus.Ten,
                                                DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                LaVatTuBHYT = s.LaVatTuBHYT,
                                                LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                                                Ma = s.VatTuBenhVien.VatTus.Ma,
                                                SoLo = s.Solo,
                                                HanSuDung = s.HanSuDung,
                                                DonGiaNhap = s.DonGiaNhap,
                                                KhoXuatId = s.YeuCauTraVatTu.KhoXuatId,
                                                SoLuongTra = s.SoLuongTra,
                                                NgayNhap = s.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhap,
                                                VAT = s.VAT,
                                                TiLeThapGia = s.TiLeTheoThapGia
                                            })
                                            .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                          .Select(g => new YeuCauTraVatTuGridVo
                                          {
                                              Id = g.First().Id,
                                              VatTuBenhVienId = g.First().VatTuBenhVienId,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaVatTuBHYT = g.First().LaVatTuBHYT,
                                              LoaiSuDung = g.First().LoaiSuDung,
                                              Ma = g.First().Ma,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                                              NgayNhap = g.First().NgayNhap,
                                              VAT = g.First().VAT,
                                              TiLeThapGia = g.First().TiLeThapGia,
                                          }).ToList();
                result.AddRange(yeuCauTraVatTuGridVos);
                result = result.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                          .Select(g => new YeuCauTraVatTuGridVo
                                          {
                                              Id = g.First().Id,
                                              VatTuBenhVienId = g.First().VatTuBenhVienId,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaVatTuBHYT = g.First().LaVatTuBHYT,
                                              LoaiSuDung = g.First().LoaiSuDung,
                                              Ma = g.First().Ma,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.Sum(z => z.SoLuongTon),
                                              NgayNhap = g.First().NgayNhap,
                                              VAT = g.First().VAT,
                                              TiLeThapGia = g.First().TiLeThapGia,
                                          }).ToList();
            }

            if (info.VatTuBenhVienVos.Any())
            {
                result = result.Where(x => !info.VatTuBenhVienVos.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT
                                                                        && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim()
                                                                        && z.HanSuDung == x.HanSuDung
                                                                        && z.DonGiaNhap == x.DonGiaNhap)).ToList();
            }
            var query = result.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncVatTuTuTrucDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauTraVatTuTuTrucVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(nkct => nkct.VatTus)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoVatTu.Kho.Id == info.KhoXuatId);
            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoVatTuChiTiets = nhapKhoVatTuChiTiets.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten,
                    g => g.VatTuBenhVien.Ma,
                    g => g.VatTuBenhVien.VatTus.DonViTinh,
                    g => g.Solo
               );
            }
            var yeuCauTraVatTuChiTiets = new List<YeuCauTraVatTuGridVo>();
            var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh, x.DonGiaNhap }).Select(g => new { nhapKhoVatTuChiTiet = g.FirstOrDefault() });

            foreach (var item in nhapKhoVatTuChiTietGroup)
            {
                var yeuCauTraVatTuChiTiet = new YeuCauTraVatTuGridVo
                {
                    Id = item.nhapKhoVatTuChiTiet.Id,
                    VatTuBenhVienId = item.nhapKhoVatTuChiTiet.VatTuBenhVienId,
                    Ten = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    Ma = item.nhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    DVT = item.nhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    LaVatTuBHYT = item.nhapKhoVatTuChiTiet.LaVatTuBHYT,
                    LoaiSuDung = item.nhapKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung,
                    SoLo = item.nhapKhoVatTuChiTiet.Solo,
                    HanSuDung = item.nhapKhoVatTuChiTiet.HanSuDung,
                    DonGiaNhap = item.nhapKhoVatTuChiTiet.DonGiaNhap,
                    KhoXuatId = item.nhapKhoVatTuChiTiet.NhapKhoVatTu.KhoId,
                    NgayNhap = item.nhapKhoVatTuChiTiet.NgayNhap,
                    VAT = item.nhapKhoVatTuChiTiet.VAT,
                    TiLeThapGia = item.nhapKhoVatTuChiTiet.TiLeTheoThapGia
                };
                yeuCauTraVatTuChiTiets.Add(yeuCauTraVatTuChiTiet);
            }

            var yeuCauTraVatTuGridVos = new List<YeuCauTraVatTuGridVo>();
            if (!info.IsCreate)
            {
                yeuCauTraVatTuGridVos = _ycTraVtChiTiet.TableNoTracking
                                            .Where(ct => ct.YeuCauTraVatTuId == info.YeuCauTraVatTuId && ct.YeuCauTraVatTu.KhoXuatId == info.KhoXuatId)
                                            .Select(s => new YeuCauTraVatTuGridVo
                                            {
                                                Id = s.Id,
                                                VatTuBenhVienId = s.VatTuBenhVienId,
                                                Ten = s.VatTuBenhVien.VatTus.Ten,
                                                DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                LaVatTuBHYT = s.LaVatTuBHYT,
                                                LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                                                Ma = s.VatTuBenhVien.VatTus.Ma,
                                                SoLo = s.Solo,
                                                HanSuDung = s.HanSuDung,
                                                DonGiaNhap = s.DonGiaNhap,
                                                KhoXuatId = s.YeuCauTraVatTu.KhoXuatId,
                                                SoLuongTra = s.SoLuongTra,
                                                NgayNhap = s.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhap,
                                                VAT = s.VAT,
                                                TiLeThapGia = s.TiLeTheoThapGia
                                            })
                                            .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                          .Select(g => new YeuCauTraVatTuGridVo
                                          {
                                              Id = g.First().Id,
                                              VatTuBenhVienId = g.First().VatTuBenhVienId,
                                              Ten = g.First().Ten,
                                              DVT = g.First().DVT,
                                              LaVatTuBHYT = g.First().LaVatTuBHYT,
                                              LoaiSuDung = g.First().LoaiSuDung,
                                              Ma = g.First().Ma,
                                              SoLo = g.First().SoLo,
                                              HanSuDung = g.First().HanSuDung,
                                              DonGiaNhap = g.First().DonGiaNhap,
                                              KhoXuatId = g.First().KhoXuatId,
                                              SoLuongTra = g.Sum(z => z.SoLuongTra),
                                              SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                                              NgayNhap = g.First().NgayNhap,
                                              VAT = g.First().VAT,
                                              TiLeThapGia = g.First().TiLeThapGia,
                                          }).ToList();
                yeuCauTraVatTuChiTiets.AddRange(yeuCauTraVatTuGridVos);
                yeuCauTraVatTuChiTiets = yeuCauTraVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                                         .Select(g => new YeuCauTraVatTuGridVo
                                         {
                                             Id = g.First().Id,
                                             VatTuBenhVienId = g.First().VatTuBenhVienId,
                                             Ten = g.First().Ten,
                                             DVT = g.First().DVT,
                                             LaVatTuBHYT = g.First().LaVatTuBHYT,
                                             LoaiSuDung = g.First().LoaiSuDung,
                                             Ma = g.First().Ma,
                                             SoLo = g.First().SoLo,
                                             HanSuDung = g.First().HanSuDung,
                                             DonGiaNhap = g.First().DonGiaNhap,
                                             KhoXuatId = g.First().KhoXuatId,
                                             SoLuongTra = g.First().SoLuongTra,
                                             SoLuongTon = g.First().SoLuongTon,
                                             NgayNhap = g.First().NgayNhap,
                                             VAT = g.First().VAT,
                                             TiLeThapGia = g.First().TiLeThapGia,
                                         }).ToList();
            }

            if (info.VatTuBenhVienVos.Any())
            {
                yeuCauTraVatTuChiTiets = yeuCauTraVatTuChiTiets.Where(x => !info.VatTuBenhVienVos.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT
                                                                       && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim()
                                                                       && z.HanSuDung == x.HanSuDung
                                                                       && z.DonGiaNhap == x.DonGiaNhap)).ToList();
            }
            var countTask = yeuCauTraVatTuChiTiets.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetDataForGridChildAsyncDaDuyetVatTu(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            var yeuCauTraVTVo = JsonConvert.DeserializeObject<DanhSachDaDuyetChiTietVTVo>(queryInfo.AdditionalSearchString);
            var query = _ycTraVtChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraVatTuId == yeuCauTraVTVo.YeuCauTraVatTuId)
                .Select(s => new DanhSachYeuCauHoanTraVTChiTietGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ten = s.VatTuBenhVien.VatTus.Ten,
                    Ma = s.VatTuBenhVien.Ma,
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                    HopDong = s.HopDongThauVatTu.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                }).ApplyLike(yeuCauTraVTVo.SearchString?.Replace("\t", "").Trim(),
                    q => q.Ma,
                    q => q.Ten,
                    q => q.SoLo,
                    q => q.HopDong,
                    q => q.Ma);

            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new DanhSachYeuCauHoanTraVTChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    LoaiSuDung = g.First().LoaiSuDung,
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
        public async Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyetVatTu(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { TotalRowCount = 0 };
            }
            var yeuCauTraVTVo = JsonConvert.DeserializeObject<DanhSachDaDuyetChiTietVTVo>(queryInfo.AdditionalSearchString);
            var query = _ycTraVtChiTiet.TableNoTracking
                .Where(p => p.YeuCauTraVatTuId == yeuCauTraVTVo.YeuCauTraVatTuId)
                .Select(s => new DanhSachYeuCauHoanTraVTChiTietGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ten = s.VatTuBenhVien.VatTus.Ten,
                    Ma = s.VatTuBenhVien.Ma,
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                    HopDong = s.HopDongThauVatTu.SoHopDong,
                    SoLuongTra = s.SoLuongTra,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    DonGiaNhap = s.DonGiaNhap,
                    Vat = s.VAT,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    SoLo = s.Solo,
                    TiLeThapGia = s.TiLeTheoThapGia,
                    NgayNhapVaoBenhVien = s.NgayNhapVaoBenhVien
                }).ApplyLike(yeuCauTraVTVo.SearchString?.Replace("\t", "").Trim(),
                    q => q.Ma,
                    q => q.Ten,
                    q => q.SoLo,
                    q => q.HopDong,
                    q => q.Ma);

            var groupQuery = query.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new DanhSachYeuCauHoanTraVTChiTietGridVo
                {
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    HopDong = g.First().HopDong,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    LoaiSuDung = g.First().LoaiSuDung,
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
        public async Task<TrangThaiDuyetVo> GetTrangThaiYeuCauHoanTraVT(long phieuLinhId)
        {
            var yeuCauLinhDuocPham = await BaseRepository.TableNoTracking.Where(p => p.Id == phieuLinhId).FirstAsync();
            var trangThaiVo = new TrangThaiDuyetVo();
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
        }
        public async Task XuLyThemHoacCapNhatHoanTraVTAsync(YeuCauTraVatTu yeuCauTraVatTu, List<YeuCauTraVatTuTuTrucChiTietVo> yeuCauTraVTTuTrucChiTiets, bool isCreate)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            if (!isCreate)
            {
                await XoaChiTietHoanTra(yeuCauTraVatTu);
            }
            var khoXuatIds = yeuCauTraVTTuTrucChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
            var nhapKhoVatTuChiTietAlls = _nhapKhoVatTuChiTietRepository.Table
                                         .Include(nk => nk.NhapKhoVatTu)
                                         .Where(o => khoXuatIds.Contains(o.NhapKhoVatTu.KhoId) && o.SoLuongNhap > o.SoLuongDaXuat)
                                         .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                          .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
            foreach (var chiTiet in yeuCauTraVTTuTrucChiTiets)
            {
                var nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietAlls.Where(o =>
                                      o.NhapKhoVatTu.KhoId == chiTiet.KhoXuatId
                                      && o.LaVatTuBHYT == chiTiet.LaVatTuBHYT
                                      && o.VatTuBenhVienId == chiTiet.VatTuBenhVienId
                                      && o.Solo == chiTiet.SoLo
                                      && o.HanSuDung == chiTiet.HanSuDung
                                      && o.DonGiaNhap == chiTiet.DonGiaNhap)
                                      .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();

                var SLTon = nhapKhoVatTuChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat);

                if (SLTon < chiTiet.SoLuongTra)
                {
                    throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                }
                var soLuongTra = chiTiet.SoLuongTra;// số lượng trả

                var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                {
                    VatTuBenhVienId = chiTiet.VatTuBenhVienId,
                };
                var yeuCauTraVatTuChiTietNews = new List<YeuCauTraVatTuChiTiet>();
                foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                {
                    if (soLuongTra == 0)
                    {
                        break;
                    }
                    var yeuCauTraVatTuChiTietNew = new YeuCauTraVatTuChiTiet
                    {
                        VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                        HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                        LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT,
                        Solo = nhapKhoVatTuChiTiet.Solo,
                        HanSuDung = nhapKhoVatTuChiTiet.HanSuDung,
                        NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                        DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                        TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                        VAT = nhapKhoVatTuChiTiet.VAT,
                        MaVach = nhapKhoVatTuChiTiet.MaVach,
                        MaRef = nhapKhoVatTuChiTiet.MaRef,
                        KhoViTriId = nhapKhoVatTuChiTiet.KhoViTriId
                    };
                    var SLTonHienTai = (nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                    if (SLTonHienTai > soLuongTra || SLTonHienTai.AlmostEqual(soLuongTra.Value))
                    {
                        nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongTra.Value).MathRoundNumber(2);
                        yeuCauTraVatTuChiTietNew.SoLuongTra = soLuongTra.Value;
                        soLuongTra = 0;
                    }
                    else
                    {
                        nhapKhoVatTuChiTiet.SoLuongDaXuat = nhapKhoVatTuChiTiet.SoLuongNhap;
                        yeuCauTraVatTuChiTietNew.SoLuongTra = SLTonHienTai;
                        soLuongTra -= SLTonHienTai;
                    }
                    var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                    {
                        SoLuongXuat = yeuCauTraVatTuChiTietNew.SoLuongTra,
                        GhiChu = "Hoàn trả vật tư tủ trực.",
                        XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet,
                    };
                    yeuCauTraVatTuChiTietNew.XuatKhoVatTuChiTietViTri = xuatKhoVatTuChiTietViTri;
                    yeuCauTraVatTuChiTietNew.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet;
                    yeuCauTraVatTuChiTietNew.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet;
                    yeuCauTraVatTu.YeuCauTraVatTuChiTiets.Add(yeuCauTraVatTuChiTietNew);
                }
            }
            await BaseRepository.UpdateAsync(yeuCauTraVatTu);
        }
        public async Task<List<YeuCauTraVatTuGridVo>> YeuCauHoanTraVatTuChiTiets(long yeuCauTraVatTuId)
        {
            var query = _ycTraVtChiTiet.TableNoTracking.Where(z => z.YeuCauTraVatTuId == yeuCauTraVatTuId)
                .Select(s => new YeuCauTraVatTuGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ten = s.VatTuBenhVien.VatTus.Ten,
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                    Ma = s.VatTuBenhVien.Ma,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    DonGiaNhap = s.DonGiaNhap,
                    KhoXuatId = s.YeuCauTraVatTu.KhoXuatId,
                    SoLuongTra = s.SoLuongTra,
                    SoLuongTon = s.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(z => z.VatTuBenhVienId == s.VatTuBenhVienId && z.LaVatTuBHYT == s.LaVatTuBHYT && z.NhapKhoVatTu.KhoId == s.YeuCauTraVatTu.KhoXuatId && z.HanSuDung == s.HanSuDung && z.Solo == s.Solo && z.DonGiaNhap == s.DonGiaNhap).Sum(z => z.SoLuongNhap - z.SoLuongDaXuat),
                    NgayNhap = s.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NgayNhap,
                    VAT = s.VAT,
                    TiLeThapGia = s.TiLeTheoThapGia,
                })
                .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT, x.DonGiaNhap })
                .Select(g => new YeuCauTraVatTuGridVo
                {
                    Id = g.First().Id,
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    LoaiSuDung = g.First().LoaiSuDung,
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    KhoXuatId = g.First().KhoXuatId,
                    SoLuongTra = g.Sum(z => z.SoLuongTra),
                    SoLuongTon = g.First().SoLuongTon + g.Sum(z => z.SoLuongTra),
                    NgayNhap = g.First().NgayNhap,
                    TiLeThapGia = g.First().TiLeThapGia,
                    VAT = g.First().VAT,
                }).ToList();
            return query;
        }
        private async Task XoaChiTietHoanTra(YeuCauTraVatTu yeuCauTraVatTu)
        {
            foreach (var chiTiet in yeuCauTraVatTu.YeuCauTraVatTuChiTiets)
            {
                chiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat =
                                                                    (chiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat - chiTiet.SoLuongTra).MathRoundNumber(2);
                chiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                chiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                chiTiet.WillDelete = true;
            }
            await BaseRepository.UpdateAsync(yeuCauTraVatTu);
        }

    }
}
