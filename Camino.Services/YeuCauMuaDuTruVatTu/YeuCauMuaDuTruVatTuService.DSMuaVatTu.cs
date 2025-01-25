using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
using System.Linq.Dynamic.Core;
using System.Globalization;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.YeuCauMuaVatTu;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoaDuoc;

namespace Camino.Services.YeuCauMuaDuTruVatTu
{
    public partial class YeuCauMuaDuTruVatTuService
    {
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var nhanVienLoginId = _userAgentHelper.GetCurrentUserId();
            var khoNhanVienQuanLys = _khoNhanVienQuanLyRepository.TableNoTracking.Where(p => p.NhanVienId == nhanVienLoginId).Select(p => p.KhoId).ToList();
            var query = BaseRepository.TableNoTracking
                .Where(p => khoNhanVienQuanLys.Contains(p.KhoId))
                .Select(s => new YeuCauMuaVatTuGridVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    TenKho = s.Kho.Ten,
                    TuNgay = s.TuNgay,
                    KyDuTru = s.TuNgay.ApplyFormatDate() + " - " + s.DenNgay.ApplyFormatDate(),
                    NgayYeuCau = s.NgayYeuCau,
                    NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                    IsKhoaDuyet = s.TruongKhoaDuyet,
                    // 0: chờ duyệt, 1: đã duyệt, 2: từ chối
                    TuChoiDuyet = s.TruongKhoaDuyet == false || (s.DuTruMuaVatTuTheoKhoa != null ? s.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == false : false)
                                                             || (s.DuTruMuaVatTuKhoDuoc != null ? s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false : false)
                                                             || (s.DuTruMuaVatTuTheoKhoa != null && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null
                                                                && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false),
                    DaDuyet = (s.DuTruMuaVatTuKhoDuoc != null && s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true)
                           || (s.DuTruMuaVatTuTheoKhoa != null && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null
                                && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true),
                    NgayTaiKhoa = s.NgayTruongKhoaDuyet,
                    NgayTaiKhoDuoc = s.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe ? (s.DuTruMuaVatTuTheoKhoa != null ? s.DuTruMuaVatTuTheoKhoa.NgayKhoDuocDuyet : null) : s.NgayTruongKhoaDuyet,
                    NgayTaiGiamDoc = (s.DuTruMuaVatTuTheoKhoa != null && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null) ? s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.NgayGiamDocDuyet : (s.DuTruMuaVatTuKhoDuoc != null ? s.DuTruMuaVatTuKhoDuoc.NgayGiamDocDuyet : null),
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<YeuCauMuaVatTuGridVo>(queryInfo.AdditionalSearchString);
                // 0: chờ duyệt, 1: đã duyệt, 2: từ chối
                if (queryString.ChoDuyet == false && queryString.DaDuyet == true && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 1 || p.TinhTrang == 2);
                }
                else if (queryString.ChoDuyet == false && queryString.DaDuyet == false && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 2);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == false && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 0 || p.TinhTrang == 2);
                }
                else if (queryString.ChoDuyet == false && queryString.DaDuyet == true && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == true && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0 || p.TinhTrang == 1);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == false && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0);
                }

                if (queryString.RangeFromDate != null &&
                            (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                }

                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.TenKho,
                         g => g.NhanVienYeuCau
                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.SoPhieu,
                    g => g.TenKho,
                    g => g.NhanVienYeuCau
                    );
            }
            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TinhTrang";
            }

            //Sort KyDuTru (DateTime - DateTime)
            var sortString = queryInfo.SortString.Contains("KyDuTru") ? queryInfo.SortString.Replace("KyDuTru", "TuNgay") : queryInfo.SortString;

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(sortString).ThenByDescending(p => p.NgayYeuCau).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var nhanVienLoginId = _userAgentHelper.GetCurrentUserId();
            var khoNhanVienQuanLys = _khoNhanVienQuanLyRepository.TableNoTracking.Where(p => p.NhanVienId == nhanVienLoginId).Select(p => p.KhoId).ToList();
            var query = BaseRepository.TableNoTracking
                .Where(p => khoNhanVienQuanLys.Contains(p.KhoId))
                .Select(s => new YeuCauMuaVatTuGridVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    TenKho = s.Kho.Ten,
                    KyDuTru = s.TuNgay.ApplyFormatDate() + " - " + s.DenNgay.ApplyFormatDate(),
                    NgayYeuCau = s.NgayYeuCau,
                    NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                    IsKhoaDuyet = s.TruongKhoaDuyet,
                    // 0: chờ duyệt, 1: đã duyệt, 2: từ chối
                    TuChoiDuyet = s.TruongKhoaDuyet == false || (s.DuTruMuaVatTuTheoKhoa != null ? s.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == false : false)
                                                             || (s.DuTruMuaVatTuKhoDuoc != null ? s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false : false)
                                                             || (s.DuTruMuaVatTuTheoKhoa != null && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null
                                                                && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false),
                    DaDuyet = (s.DuTruMuaVatTuKhoDuoc != null && s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true)
                           || (s.DuTruMuaVatTuTheoKhoa != null && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null
                                && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true),
                    NgayTaiKhoa = s.NgayTruongKhoaDuyet,
                    NgayTaiKhoDuoc = s.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe ? (s.DuTruMuaVatTuTheoKhoa != null ? s.DuTruMuaVatTuTheoKhoa.NgayKhoDuocDuyet : null) : s.NgayTruongKhoaDuyet,

                    NgayTaiGiamDoc = (s.DuTruMuaVatTuTheoKhoa != null && s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null) ? s.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.NgayGiamDocDuyet : (s.DuTruMuaVatTuKhoDuoc != null ? s.DuTruMuaVatTuKhoDuoc.NgayGiamDocDuyet : null),
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<YeuCauMuaVatTuGridVo>(queryInfo.AdditionalSearchString);
                // 0: chờ duyệt, 1: đã duyệt, 2: từ chối
                if (queryString.ChoDuyet == false && queryString.DaDuyet == true && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 1 || p.TinhTrang == 2);
                }
                else if (queryString.ChoDuyet == false && queryString.DaDuyet == false && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 2);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == false && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 0 || p.TinhTrang == 2);
                }
                else if (queryString.ChoDuyet == false && queryString.DaDuyet == true && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == true && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0 || p.TinhTrang == 1);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == false && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0);
                }

                if (queryString.RangeFromDate != null &&
                            (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                }

                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.TenKho,
                         g => g.NhanVienYeuCau
                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.SoPhieu,
                    g => g.TenKho,
                    g => g.NhanVienYeuCau
                    );
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupTreeItemVo>> GetNhomVatTuTreeView(DropDownListRequestModel model)
        {
            var query = await _nhomVatTuRepository.TableNoTracking
                .Select(s => new LookupTreeItemVo
                {
                    KeyId = s.Id,
                    DisplayName = s.Ten,
                    Level = s.CapNhom,
                    ParentId = s.NhomVatTuChaId
                }).ToListAsync();

            var list = query.Select(s => new LookupTreeItemVo
            {
                KeyId = s.KeyId,
                DisplayName = s.DisplayName,
                Level = s.Level + 1,
                ParentId = s.ParentId,
                Items = GetChildrenTree(query, s.KeyId, s.Level, model.Query.RemoveDiacritics(), s.DisplayName)
            }).Where(x =>
                x.ParentId == null && (string.IsNullOrEmpty(model.Query) ||
                                       !string.IsNullOrEmpty(model.Query) && (x.Items.Any()
                                       || x.DisplayName.RemoveDiacritics().Trim().ToLower().Contains(model.Query.RemoveDiacritics().Trim().ToLower()))))
                .Take(model.Take)
                .ToList();

            return list;
        }
        private static List<LookupTreeItemVo> GetChildrenTree(List<LookupTreeItemVo> comments, long Id, long level, string queryString, string parentDisplay)
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id && c.Level == level + 1)
                .Select(c => new LookupTreeItemVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Items = GetChildrenTree(comments, c.KeyId, c.Level, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString) || !string.IsNullOrEmpty(queryString) && (parentDisplay.RemoveDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.RemoveDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any()))
                .ToList();
            return query;
        }


        public async Task<List<LookupItemVo>> GetKyDuTruVatTu(DropDownListRequestModel queryInfo)
        {
            var lstKyDuTru = _kyDuTruMuaDuocPhamVatTuRepository
                                 .TableNoTracking.Where(p => p.HieuLuc == true && p.MuaVatTu == true
                                                    && (p.NgayBatDauLap.Date <= DateTime.Now.Date && DateTime.Now.Date <= p.NgayKetThucLap.Date))
                                 .Select(s => new LookupItemVo
                                 {
                                     KeyId = s.Id,
                                     DisplayName = s.TuNgay.ApplyFormatDate() + " - " + s.DenNgay.ApplyFormatDate()
                                 })
                                .ApplyLike(queryInfo.Query, o => o.DisplayName)
                                .Take(queryInfo.Take);
            return await lstKyDuTru.ToListAsync();
        }

        public async Task<List<VatTuTemplateLookupItem>> GetVatTuMuaDuTrus(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
                {
                    nameof(Core.Domain.Entities.VatTus.VatTu.Ten)
                };
            if (string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                return new List<VatTuTemplateLookupItem>();
            }
            //var info = JsonConvert.DeserializeObject<DuocPhamVaHoaChat>(queryInfo.ParameterDependencies); // Hiện tại chưa dùng đến
            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                var vatTus = _vatTuRepository
                                 .TableNoTracking.Where(p => p.IsDisabled != true)
                                 .Select(s => new VatTuTemplateLookupItem
                                 {
                                     KeyId = s.Id,
                                     DisplayName = s.Ten,
                                     Ma = s.Ma,
                                     Ten = s.Ten,
                                     DVT = s.DonViTinh,
                                     QuyCach = s.QuyCach,
                                     NhaSX = s.NhaSanXuat,
                                     NuocSX = s.NuocSanXuat
                                 })
                                  .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.Ma)
                                  .Take(queryInfo.Take);
                return await vatTus.ToListAsync();
            }
            else
            {
                var lstVatTuId = await _vatTuRepository
                     .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                     .Select(s => s.Id).ToListAsync();
                var vatTus = new List<VatTuTemplateLookupItem>();

                vatTus = await _vatTuRepository.TableNoTracking
                    .Where(x => lstVatTuId.Contains(x.Id) && x.IsDisabled != true)
                    //.OrderByDescending(x => x.Id == vatTuId)
                    //.ThenBy(p => lstVatTuId.IndexOf(p.Id) != -1 ? lstVatTuId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .ApplyLike(queryInfo.Query, o => o.Ma, o => o.Ten)
                    .Take(queryInfo.Take)
                    .Select(item => new VatTuTemplateLookupItem
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ma = item.Ma,
                        Ten = item.Ten,
                        DVT = item.DonViTinh,
                        QuyCach = item.QuyCach,
                        NhaSX = item.NhaSanXuat,
                        NuocSX = item.NuocSanXuat
                    }).ToListAsync();

                return vatTus;
            }
        }
        public ThongTinDuTruMuaVatTu ThongTinDuTruMuaVatTu(ThongTinChiTietVatTuTonKho thongTinChiTietVatTuTonKho)
        {
            var thongTin = new ThongTinDuTruMuaVatTu();
            var vatTuBenhVien = _vatTuBenhVienRepository.TableNoTracking
                        .Any(p => p.Id == thongTinChiTietVatTuTonKho.VatTuId && p.HieuLuc == true);
            var laVatTuBHYT = false;
            if (thongTinChiTietVatTuTonKho.LoaiVatTu == 2)
            {
                laVatTuBHYT = true;
            }
            var hopDongThauVatTuIds = _hopDongThauVatTuRepository
                                           .TableNoTracking
                                           .Where(p => p.HopDongThauVatTuChiTiets.Any(ct => ct.VatTuId == thongTinChiTietVatTuTonKho.VatTuId))
                                         .SelectMany(p => p.HopDongThauVatTuChiTiets.Select(o => o.HopDongThauVatTuId)).Distinct().ToList();

            var slTonHDT = _hopDongThauVatTuChiTietRepository
                           .TableNoTracking
                           .Where(p => hopDongThauVatTuIds.Contains(p.HopDongThauVatTuId)
                                    && p.VatTuId == thongTinChiTietVatTuTonKho.VatTuId
                                    && p.HopDongThauVatTu.NgayHetHan >= DateTime.Now)
                           .Sum(p => p.SoLuong - p.SoLuongDaCap);
            if (vatTuBenhVien)
            {
                thongTin = _vatTuBenhVienRepository
                            .TableNoTracking
                            .Where(p => p.Id == thongTinChiTietVatTuTonKho.VatTuId)
                            .Select(s => new ThongTinDuTruMuaVatTu
                            {
                                DVT = s.VatTus.DonViTinh,
                                NhaSX = s.VatTus.NhaSanXuat,
                                NuocSX = s.VatTus.NuocSanXuat,
                                SLTonDuTru = s.NhapKhoVatTuChiTiets
                                            .Where(nkct => nkct.NhapKhoVatTu.KhoId == thongTinChiTietVatTuTonKho.KhoId && nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                            .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                SLTonKhoTong = s.NhapKhoVatTuChiTiets
                                            .Where(nkct => nkct.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                            .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                ThongTinChiTietTonKhoTongs = s.NhapKhoVatTuChiTiets
                                                            .Where(p => p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                         && p.LaVatTuBHYT == laVatTuBHYT
                                                                         && p.VatTuBenhVienId == thongTinChiTietVatTuTonKho.VatTuId
                                                                         && p.SoLuongNhap - p.SoLuongDaXuat > 0
                                                                         && p.HanSuDung >= DateTime.Now
                                                                         )
                                                               .Select(item => new ThongTinChiTietTonKho
                                                               {
                                                                   Ten = item.NhapKhoVatTu.Kho.Ten,
                                                                   SLTon = (item.SoLuongNhap - item.SoLuongDaXuat)
                                                               }).GroupBy(x => new { x.Ten })
                                                               .Select(o => new ThongTinChiTietTonKho
                                                               {
                                                                   Ten = o.First().Ten,
                                                                   SLTon = o.Sum(x => x.SLTon),
                                                               }).OrderBy(x => x.Ten).Distinct().ToList(),
                                SLChuaNhapVeHDT = slTonHDT,
                                ThongTinChiTietTonHDTs = _hopDongThauVatTuChiTietRepository.TableNoTracking
                                                           .Where(o => hopDongThauVatTuIds.Contains(o.HopDongThauVatTuId)
                                                                    && o.VatTuId == thongTinChiTietVatTuTonKho.VatTuId
                                                                    && o.SoLuong - o.SoLuongDaCap > 0)

                                                           .Select(item => new ThongTinChiTietTonKho
                                                           {
                                                               Ten = item.HopDongThauVatTu.SoHopDong,
                                                               SLTon = (item.SoLuong - item.SoLuongDaCap)
                                                           })
                                                           .GroupBy(x => new { x.Ten })
                                                           .Select(o => new ThongTinChiTietTonKho
                                                           {
                                                               Ten = o.First().Ten,
                                                               SLTon = o.Sum(x => x.SLTon)
                                                           }).OrderBy(x => x.Ten).Distinct().ToList().ToList(),

                            }).FirstOrDefault();
            }
            else
            {
                thongTin = _vatTuRepository
                            .TableNoTracking
                            .Where(p => p.Id == thongTinChiTietVatTuTonKho.VatTuId)
                            .Select(s => new ThongTinDuTruMuaVatTu
                            {
                                DVT = s.DonViTinh,
                                NhaSX = s.NhaSanXuat,
                                NuocSX = s.NuocSanXuat,
                                SLTonDuTru = 0,
                                SLTonKhoTong = 0,
                                SLChuaNhapVeHDT = slTonHDT,
                                ThongTinChiTietTonKhoTongs = new List<ThongTinChiTietTonKho>(),
                                ThongTinChiTietTonHDTs = _hopDongThauVatTuChiTietRepository.TableNoTracking
                                                           .Where(o => hopDongThauVatTuIds.Contains(o.HopDongThauVatTuId) && o.SoLuong - o.SoLuongDaCap > 0
                                                           && o.VatTuId == thongTinChiTietVatTuTonKho.VatTuId)
                                                           .Select(item => new ThongTinChiTietTonKho
                                                           {
                                                               Ten = item.HopDongThauVatTu.SoHopDong,
                                                               SLTon = (item.SoLuong - item.SoLuongDaCap)
                                                           })
                                                           .GroupBy(x => new { x.Ten })
                                                           .Select(o => new ThongTinChiTietTonKho
                                                           {
                                                               Ten = o.First().Ten,
                                                               SLTon = o.Sum(x => x.SLTon)
                                                           }).OrderBy(x => x.Ten).Distinct().ToList().ToList()
                            }).FirstOrDefault();
            }
            return thongTin;

        }

        public async Task<TrangThaiDuyetDuTruMuaVo> GetTrangThaiPhieuMuaVatTu(long phieuMuaId)
        {
            var duocDuyet = await BaseRepository.TableNoTracking
                                   .Include(p => p.DuTruMuaVatTuTheoKhoa).ThenInclude(p => p.DuTruMuaVatTuKhoDuoc)
                                   .Include(p => p.DuTruMuaVatTuKhoDuoc)
                                  .Where(p => p.Id == phieuMuaId).FirstOrDefaultAsync();
            var trangThaiVo = new TrangThaiDuyetDuTruMuaVo();

            if ((duocDuyet.DuTruMuaVatTuKhoDuoc != null && duocDuyet.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true)
                           || (duocDuyet.DuTruMuaVatTuTheoKhoa != null && duocDuyet.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null
                                && duocDuyet.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true))
            {
                trangThaiVo.TrangThai = true;
                trangThaiVo.Ten = "Đã duyệt";
            }
            else if (duocDuyet.TruongKhoaDuyet == false
                        || (duocDuyet.DuTruMuaVatTuTheoKhoa != null ? duocDuyet.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == false : false)
                        || (duocDuyet.DuTruMuaVatTuKhoDuoc != null ? duocDuyet.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false : false)
                        || (duocDuyet.DuTruMuaVatTuTheoKhoa != null && duocDuyet.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null
                                      && duocDuyet.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false))
            {
                if (duocDuyet.DuTruMuaVatTuTheoKhoa == null && duocDuyet.DuTruMuaVatTuKhoDuoc == null && duocDuyet.TruongKhoaDuyet == false)
                {
                    trangThaiVo.IsKhoaDuyet = false;
                }
                if (duocDuyet.DuTruMuaVatTuTheoKhoa != null && duocDuyet.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet != null
                 || (duocDuyet.DuTruMuaVatTuKhoDuoc != null && duocDuyet.TruongKhoaDuyet == false)
                 || (duocDuyet.DuTruMuaVatTuTheoKhoa == null && duocDuyet.DuTruMuaVatTuKhoDuoc == null && duocDuyet.TruongKhoaDuyet == false))
                {
                    trangThaiVo.IsKhoDuocDuyet = false;
                }
                if ((duocDuyet.DuTruMuaVatTuTheoKhoa != null && duocDuyet.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null
                        && duocDuyet.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false)
                   || (duocDuyet.DuTruMuaVatTuKhoDuoc != null && duocDuyet.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false))
                {
                    trangThaiVo.IsGiamDocDuyet = false;
                }
                trangThaiVo.TrangThai = false;
                trangThaiVo.Ten = "Từ chối";
            }
            else
            {
                if (duocDuyet.TruongKhoaDuyet == true)
                {
                    trangThaiVo.IsKhoaDuyet = true;
                }
                if ((duocDuyet.DuTruMuaVatTuTheoKhoa != null && duocDuyet.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == true) || (duocDuyet.DuTruMuaVatTuKhoDuoc != null && duocDuyet.TruongKhoaDuyet == true))
                {
                    trangThaiVo.IsKhoDuocDuyet = true;
                }
                trangThaiVo.TrangThai = null;
                trangThaiVo.Ten = "Đang chờ duyệt";
            }
            return trangThaiVo;
        }

        public async Task<bool> CheckVatTuExists(long? vatTuId, bool? laVatTuBHYT, List<VatTuDuTruViewModelValidator> vatTus)
        {
            if (vatTuId == null)
            {
                return true;
            }
            if (vatTuId != null)
            {
                foreach (var item in vatTus)
                {
                    if (vatTus.Any(p => p.VatTuId == vatTuId && p.LaVatTuBHYT == laVatTuBHYT))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        public string InPhieuMuaDuTruVatTu(PhieuMuaDuTruVatTu phieuMuaDuTruVatTu)
        {
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuMuaDuTruVatTu")).First();
            var vatTuChiTiet = string.Empty;
            var hearder = string.Empty;
            var phieuMuaVatTu = BaseRepository.GetById(phieuMuaDuTruVatTu.DuTruMuaVatTuId, s =>
                                                                 s.Include(r => r.DuTruMuaVatTuChiTiets).ThenInclude(ct => ct.VatTu)
                                                                  .Include(r => r.NhanVienYeuCau).ThenInclude(nv => nv.User)
                                                                  .Include(r => r.Kho).ThenInclude(k => k.KhoaPhong)
                                                                  .Include(r => r.Kho).ThenInclude(p => p.PhongBenhVien)
                                                                );
            if (phieuMuaDuTruVatTu.Header == true)
            {
                hearder = "<p style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                           "<th>PHIẾU DỰ TRÙ VẬT TƯ</th>" +
                      "</p>";
            }
            var groupBHYT = "BHYT";
            var groupKhongBHYT = "Không BHYT";

            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupBHYT.ToUpper()
                                        + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupKhongBHYT.ToUpper()
                                        + "</b></tr>";

            var query = _duTruMuaVatTuChiTietRepository
                          .TableNoTracking
                          .Where(p => p.DuTruMuaVatTuId == phieuMuaDuTruVatTu.DuTruMuaVatTuId)
                          .Select(s => new PhieuMuaDuTruVatTuChiTietData
                          {
                              Ten = s.VatTu.Ten,
                              DonVi = s.VatTu.DonViTinh,
                              SoLuong = s.SoLuongDuTru,
                              GhiChu = s.GhiChu,
                              LaVatTuBHYT = s.LaVatTuBHYT
                          }).OrderByDescending(p => p.LaVatTuBHYT).ThenBy(p => !p.LaVatTuBHYT).ToList();
            var STT = 1;
            if (query.Any(p => p.LaVatTuBHYT))
            {
                vatTuChiTiet += headerBHYT;
                var queryBHYT = query.Where(x => x.LaVatTuBHYT).ToList();
                foreach (var item in queryBHYT)
                {
                    vatTuChiTiet = vatTuChiTiet
                                        + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                        + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                        + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                        + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.DonVi
                                        + "<td style = 'border: 1px solid #020000;text-align: right;  padding-right:3px'>" + item.SoLuong
                                        + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                        + "</tr>";
                    STT++;
                    groupBHYT = string.Empty;
                }
            }
            if (query.Any(p => !p.LaVatTuBHYT))
            {
                vatTuChiTiet += headerKhongBHYT;
                var queryKhongBHYT = query.Where(x => !x.LaVatTuBHYT).ToList();
                foreach (var item in queryKhongBHYT)
                {
                    vatTuChiTiet = vatTuChiTiet
                                        + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                        + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                        + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                        + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.DonVi
                                        + "<td style = 'border: 1px solid #020000;text-align: right;  padding-right:3px'>" + item.SoLuong
                                        + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                        + "</tr>";
                    STT++;
                    groupKhongBHYT = string.Empty;
                }
            }
            var data = new PhieuMuaDuTruVatTuData
            {
                Header = hearder,
                MaPhieuMuaDuTruVatTu = "BMBH-KD01.02",
                VatTu = vatTuChiTiet,
                TenKho = phieuMuaVatTu.Kho.Ten,
                KhoaPhong = phieuMuaVatTu.Kho.KhoaPhong?.Ten + (phieuMuaVatTu.Kho.PhongBenhVien != null ? " - " + phieuMuaVatTu.Kho.PhongBenhVien.Ten : ""),
                NhanVienLap = phieuMuaVatTu.NhanVienYeuCau.User.HoTen,
                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString()
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
        public async Task<string> GetSoPhieuDuTruVatTu()
        {
            var result = string.Empty;
            var soPhieu = "DT";
            var lastYearMonthCurrent = DateTime.Now.ToString("yyMM");
            var lastSoPhieuStr = BaseRepository.TableNoTracking.Select(p => p.SoPhieu).LastOrDefault();
            if (lastSoPhieuStr != null)
            {
                var lastSoPhieu = int.Parse(lastSoPhieuStr.Substring(lastSoPhieuStr.Length - 4));
                if (lastSoPhieu < 10)
                {
                    var lastDuTruId = string.Empty;
                    if (lastSoPhieu == 9)
                    {
                        lastDuTruId = "00" + (lastSoPhieu + 1).ToString();
                    }
                    else
                    {
                        lastDuTruId = "000" + (lastSoPhieu + 1).ToString();
                    }
                    result = soPhieu + lastYearMonthCurrent + lastDuTruId;
                }
                else if (lastSoPhieu < 100)
                {
                    var lastDuTruId = string.Empty;
                    if (lastSoPhieu == 99)
                    {
                        lastDuTruId = "0" + (lastSoPhieu + 1).ToString();
                    }
                    else
                    {
                        lastDuTruId = "00" + (lastSoPhieu + 1).ToString();
                    }
                    result = soPhieu + lastYearMonthCurrent + lastDuTruId;
                }
                else
                {
                    var lastDuTruId = "0" + (lastSoPhieu + 1).ToString();
                    result = soPhieu + lastYearMonthCurrent + lastDuTruId;
                }
            }
            else
            {
                result = soPhieu + lastYearMonthCurrent + "0001";
            }
            return result;
        }
    }
}
