using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
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
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain;

namespace Camino.Services.YeuCauMuaDuTruDuocPham
{
    public partial class YeuCauMuaDuTruDuocPhamService
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
                .Select(s => new YeuCauDuTruDuocPhamGridVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    NhomDuocPhamDuTru = s.NhomDuocPhamDuTru,
                    TenNhomDuTru = s.NhomDuocPhamDuTru.GetDescription(),
                    TenKho = s.Kho.Ten,
                    TuNgay = s.TuNgay,
                    KyDuTru = s.TuNgay.ApplyFormatDate() + " - " + s.DenNgay.ApplyFormatDate(),
                    NgayYeuCau = s.NgayYeuCau,
                    NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                    IsKhoaDuyet = s.TruongKhoaDuyet,
                    // 0: chờ duyệt, 1: đã duyệt, 2: từ chối
                    TuChoiDuyet = s.TruongKhoaDuyet == false || (s.DuTruMuaDuocPhamTheoKhoa != null ? s.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == false : false)
                                                             || (s.DuTruMuaDuocPhamKhoDuoc != null ? s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false : false)
                                                             || (s.DuTruMuaDuocPhamTheoKhoa != null && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null
                                                                && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false),
                    DaDuyet = (s.DuTruMuaDuocPhamKhoDuoc != null && s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true)
                           || (s.DuTruMuaDuocPhamTheoKhoa != null && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null
                                && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true),


                    NgayTaiKhoa = s.NgayTruongKhoaDuyet,

                    NgayTaiKhoDuoc = (s.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || s.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin) ? (s.DuTruMuaDuocPhamTheoKhoa != null ? s.DuTruMuaDuocPhamTheoKhoa.NgayKhoDuocDuyet : null) : s.NgayTruongKhoaDuyet,

                    NgayTaiGiamDoc = (s.DuTruMuaDuocPhamTheoKhoa != null && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null) ? s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.NgayGiamDocDuyet : (s.DuTruMuaDuocPhamKhoDuoc != null ? s.DuTruMuaDuocPhamKhoDuoc.NgayGiamDocDuyet : null),

                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<YeuCauDuTruDuocPhamGridVo>(queryInfo.AdditionalSearchString);
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
                .Select(s => new YeuCauDuTruDuocPhamGridVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    NhomDuocPhamDuTru = s.NhomDuocPhamDuTru,
                    TenNhomDuTru = s.NhomDuocPhamDuTru.GetDescription(),
                    TenKho = s.Kho.Ten,
                    KyDuTru = s.TuNgay.ApplyFormatDate() + " - " + s.DenNgay.ApplyFormatDate(),
                    NgayYeuCau = s.NgayYeuCau,
                    NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                    IsKhoaDuyet = s.TruongKhoaDuyet,
                    TuChoiDuyet = s.TruongKhoaDuyet == false || (s.DuTruMuaDuocPhamTheoKhoa != null ? s.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == false : false)
                                                             || (s.DuTruMuaDuocPhamKhoDuoc != null ? s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false : false)
                                                             || (s.DuTruMuaDuocPhamTheoKhoa != null && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null
                                                                && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false),
                    DaDuyet = (s.DuTruMuaDuocPhamKhoDuoc != null && s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true)
                           || (s.DuTruMuaDuocPhamTheoKhoa != null && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null
                                && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true),
                    NgayTaiKhoa = s.NgayTruongKhoaDuyet,
                    NgayTaiKhoDuoc = (s.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || s.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin) ? (s.DuTruMuaDuocPhamTheoKhoa != null ? s.DuTruMuaDuocPhamTheoKhoa.NgayKhoDuocDuyet : null) : s.NgayTruongKhoaDuyet,

                    NgayTaiGiamDoc = (s.DuTruMuaDuocPhamTheoKhoa != null && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null) ? s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.NgayGiamDocDuyet : (s.DuTruMuaDuocPhamKhoDuoc != null ? s.DuTruMuaDuocPhamKhoDuoc.NgayGiamDocDuyet : null),
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<YeuCauDuTruDuocPhamGridVo>(queryInfo.AdditionalSearchString);
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

        public List<LookupItemVo> NhomDuocPhamDuTru(DropDownListRequestModel queryInfo)
        {
            var enums = Enum.GetValues(typeof(EnumNhomDuocPhamDuTru)).Cast<Enum>();
            var result = enums.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            });
            return result.ToList();
        }
        public List<LookupItemVo> NhomDuocPhamDieuTriDuPhong(DropDownListRequestModel queryInfo)
        {
            var enums = Enum.GetValues(typeof(EnumNhomDieuTriDuPhong)).Cast<Enum>();
            var result = enums.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower()
                    .Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower()));
            return result.ToList();
        }


        public async Task<List<LookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo, bool? laDuocPham = null)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                        .Where(p => p.NhanVienId == userCurrentId && (laDuocPham == true ? p.Kho.LoaiDuocPham == true : p.Kho.LoaiVatTu == true))
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.KhoId,
                            DisplayName = s.Kho.Ten
                        })
                        .OrderByDescending(x => x.KeyId == noiLamViecCurrentId).ThenBy(x => x.DisplayName)
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetKyDuTru(DropDownListRequestModel queryInfo)
        {
            var lstKyDuTru = _kyDuTruMuaDuocPhamVatTuRepository
                                 .TableNoTracking.Where(p => p.HieuLuc == true && p.MuaDuocPham == true
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

        public async Task<List<DuocPhamTemplateLookupItem>> GetDuocPhamMuaDuTrus(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
                {
                    nameof(DuocPham.Ten),
                    nameof(DuocPham.HoatChat)
                };
            if (string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                return new List<DuocPhamTemplateLookupItem>();
            }
            var info = JsonConvert.DeserializeObject<DuocPhamVaHoaChat>(queryInfo.ParameterDependencies); // Hiện tại chưa dùng đến
            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                var duocPhams = _duocPhamRepository
                                 .TableNoTracking
                                 .Select(s => new DuocPhamTemplateLookupItem
                                 {
                                     KeyId = s.Id,
                                     DisplayName = s.Ten,
                                     Ten = s.Ten,
                                     HoatChat = s.HoatChat,
                                     DVT = s.DonViTinh.Ten,
                                     TenDuongDung = s.DuongDung.Ten,
                                     HamLuong = s.HamLuong,
                                     SoDangKy = s.SoDangKy,
                                     NhaSX = s.NhaSanXuat,
                                     NuocSX = s.NuocSanXuat
                                 })
                                  .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.HoatChat, o => o.SoDangKy)
                                  .Take(queryInfo.Take);
                return await duocPhams.ToListAsync();
            }
            else
            {
                var lstDuocPhamId = await _duocPhamRepository
                     .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                     .Select(s => s.Id).ToListAsync();


                var duocPhams = new List<DuocPhamTemplateLookupItem>();
                duocPhams = await _duocPhamRepository.TableNoTracking
                    .Where(x => lstDuocPhamId.Contains(x.Id))
                    //.OrderByDescending(x => x.Id == duocPhamId)
                    //.ThenBy(p => lstDuocPhamId.IndexOf(p.Id) != -1 ? lstDuocPhamId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .ApplyLike(queryInfo.Query, o => o.Ten, o => o.HoatChat, o => o.SoDangKy)
                    .Take(queryInfo.Take)
                    .Select(item => new DuocPhamTemplateLookupItem
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        DVT = item.DonViTinh.Ten,
                        HamLuong = item.HamLuong,
                        SoDangKy = item.SoDangKy,
                        TenDuongDung = item.DuongDung.Ten,
                        NhaSX = item.NhaSanXuat,
                        NuocSX = item.NuocSanXuat
                    }).ToListAsync();

                return duocPhams;
            }
        }

        public ThongTinDuTruMuaDuocPham ThongTinDuTruMuaDuocPham(ThongTinChiTietDuocPhamTonKho thongTinChiTietDuocPhamTonKho)
        {
            var thongTin = new ThongTinDuTruMuaDuocPham();
            var duocPhamBenhVien = _duocPhamBenhVienRepository.TableNoTracking
                        .Any(p => p.Id == thongTinChiTietDuocPhamTonKho.DuocPhamId && p.HieuLuc == true);
            var laDuocPhamBHYT = false;
            if (thongTinChiTietDuocPhamTonKho.LoaiDuocPham == 2)
            {
                laDuocPhamBHYT = true;
            }
            var hopDongThauDuocPhamIds = _hopDongThauDuocPhamRepository
                                          .TableNoTracking
                                          .Where(p => p.HopDongThauDuocPhamChiTiets.Any(ct => ct.DuocPhamId == thongTinChiTietDuocPhamTonKho.DuocPhamId))
                                          .SelectMany(p => p.HopDongThauDuocPhamChiTiets.Select(o => o.HopDongThauDuocPhamId)).Distinct().ToList();

            var slTonHDT = _hopDongThauDuocPhamChiTietRepository
                                .TableNoTracking
                                .Where(p => hopDongThauDuocPhamIds.Contains(p.HopDongThauDuocPhamId)
                                         && p.DuocPhamId == thongTinChiTietDuocPhamTonKho.DuocPhamId
                                         && p.HopDongThauDuocPham.NgayHetHan >= DateTime.Now)
                                .Sum(p => p.SoLuong - p.SoLuongDaCap);
            if (duocPhamBenhVien)
            {
                thongTin = _duocPhamBenhVienRepository
                            .TableNoTracking
                            .Where(p => p.Id == thongTinChiTietDuocPhamTonKho.DuocPhamId)
                            .Select(s => new ThongTinDuTruMuaDuocPham
                            {
                                HoatChat = s.DuocPham.HoatChat,
                                SoDangKy = s.DuocPham.SoDangKy,
                                HamLuong = s.DuocPham.HamLuong,
                                DVT = s.DuocPham.DonViTinh.Ten,
                                TenDuongDung = s.DuocPham.DuongDung.Ten,
                                NhaSX = s.DuocPham.NhaSanXuat,
                                NuocSX = s.DuocPham.NuocSanXuat,
                                SLTonDuTru = s.NhapKhoDuocPhamChiTiets
                                            .Where(nkct => nkct.NhapKhoDuocPhams.KhoId == thongTinChiTietDuocPhamTonKho.KhoId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT
                                            && nkct.HanSuDung >= DateTime.Now)
                                            .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                SLTonKhoTong = s.NhapKhoDuocPhamChiTiets
                                            .Where(nkct => nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && nkct.LaDuocPhamBHYT == laDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now)
                                            .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                SLChuaNhapVeHDT = slTonHDT,
                                ThongTinChiTietTonKhoTongs = s.NhapKhoDuocPhamChiTiets
                                                              .Where(p => p.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                                                                         && p.LaDuocPhamBHYT == laDuocPhamBHYT
                                                                         && p.DuocPhamBenhVienId == thongTinChiTietDuocPhamTonKho.DuocPhamId
                                                                         && p.SoLuongNhap - p.SoLuongDaXuat > 0
                                                                         )
                                                               .Select(item => new ThongTinChiTietTonKho
                                                               {
                                                                   Ten = item.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                                                                   SLTon = (item.SoLuongNhap - item.SoLuongDaXuat)
                                                               }).GroupBy(x => new { x.Ten })
                                                               .Select(o => new ThongTinChiTietTonKho
                                                               {
                                                                   Ten = o.First().Ten,
                                                                   SLTon = o.Sum(x => x.SLTon),
                                                               }).OrderBy(x => x.Ten).Distinct().ToList(),
                                ThongTinChiTietTonHDTs = _hopDongThauDuocPhamChiTietRepository.TableNoTracking
                                                           .Where(o => hopDongThauDuocPhamIds.Contains(o.HopDongThauDuocPhamId)
                                                                    && o.SoLuong - o.SoLuongDaCap > 0 && o.DuocPhamId == thongTinChiTietDuocPhamTonKho.DuocPhamId)
                                                           .Select(item => new ThongTinChiTietTonKho
                                                           {
                                                               Ten = item.HopDongThauDuocPham.SoHopDong,
                                                               SLTon = item.SoLuong - item.SoLuongDaCap
                                                           }).GroupBy(x => new { x.Ten })
                                                           .Select(o => new ThongTinChiTietTonKho
                                                           {
                                                               Ten = o.First().Ten,
                                                               SLTon = o.Sum(x => x.SLTon)
                                                           }).OrderBy(x => x.Ten).Distinct().ToList()
                            }).FirstOrDefault();
            }
            else
            {
                thongTin = _duocPhamRepository
                            .TableNoTracking
                            .Where(p => p.Id == thongTinChiTietDuocPhamTonKho.DuocPhamId)
                            .Select(s => new ThongTinDuTruMuaDuocPham
                            {
                                HoatChat = s.HoatChat,
                                SoDangKy = s.SoDangKy,
                                HamLuong = s.HamLuong,
                                DVT = s.DonViTinh.Ten,
                                TenDuongDung = s.DuongDung.Ten,
                                NhaSX = s.NhaSanXuat,
                                NuocSX = s.NuocSanXuat,
                                SLTonDuTru = 0,
                                SLTonKhoTong = 0,
                                SLChuaNhapVeHDT = slTonHDT,
                                ThongTinChiTietTonKhoTongs = new List<ThongTinChiTietTonKho>(),
                                ThongTinChiTietTonHDTs = _hopDongThauDuocPhamChiTietRepository.TableNoTracking
                                                           .Where(o => hopDongThauDuocPhamIds.Contains(o.HopDongThauDuocPhamId)
                                                                    && o.SoLuong - o.SoLuongDaCap > 0 && o.DuocPhamId == thongTinChiTietDuocPhamTonKho.DuocPhamId)
                                                           .Select(item => new ThongTinChiTietTonKho
                                                           {
                                                               Ten = item.HopDongThauDuocPham.SoHopDong,
                                                               SLTon = slTonHDT
                                                           }).GroupBy(x => new { x.Ten })
                                                           .Select(o => new ThongTinChiTietTonKho
                                                           {
                                                               Ten = o.First().Ten,
                                                               SLTon = o.Sum(x => x.SLTon)
                                                           }).OrderBy(x => x.Ten).Distinct().ToList()
                            }).FirstOrDefault();
            }
            return thongTin;
        }


        public async Task<TrangThaiDuyetDuTruMuaVo> GetTrangThaiPhieuMua(long phieuMuaId)
        {
            var duocDuyet = await BaseRepository.TableNoTracking
                                   .Include(p => p.DuTruMuaDuocPhamTheoKhoa).ThenInclude(p => p.DuTruMuaDuocPhamKhoDuoc)
                                   .Include(p => p.DuTruMuaDuocPhamKhoDuoc)
                                  .Where(p => p.Id == phieuMuaId).FirstOrDefaultAsync();
            var trangThaiVo = new TrangThaiDuyetDuTruMuaVo();
            if ((duocDuyet.DuTruMuaDuocPhamKhoDuoc != null && duocDuyet.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true)
                           || (duocDuyet.DuTruMuaDuocPhamTheoKhoa != null && duocDuyet.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null
                                && duocDuyet.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true))
            {
                trangThaiVo.TrangThai = true;
                trangThaiVo.Ten = "Đã duyệt";
            }
            else
            if (duocDuyet.TruongKhoaDuyet == false || (duocDuyet.DuTruMuaDuocPhamTheoKhoa != null ? duocDuyet.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == false : false)
                                                             || (duocDuyet.DuTruMuaDuocPhamKhoDuoc != null ? duocDuyet.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false : false)
                                                             || (duocDuyet.DuTruMuaDuocPhamTheoKhoa != null && duocDuyet.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null
                                                                && duocDuyet.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false))
            {
                if (duocDuyet.DuTruMuaDuocPhamTheoKhoa == null && duocDuyet.DuTruMuaDuocPhamKhoDuoc == null && duocDuyet.TruongKhoaDuyet == false)
                {
                    trangThaiVo.IsKhoaDuyet = false;
                }
                if (
                     //(duocDuyet.DuTruMuaDuocPhamKhoDuoc != null && duocDuyet.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false)
                     //|| (duocDuyet.DuTruMuaDuocPhamTheoKhoa != null && duocDuyet.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == false)
                     //|| (duocDuyet.DuTruMuaDuocPhamTheoKhoa != null && duocDuyet.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null && duocDuyet.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false)
                     duocDuyet.DuTruMuaDuocPhamTheoKhoa != null && duocDuyet.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet != null
                 || (duocDuyet.DuTruMuaDuocPhamKhoDuoc != null && duocDuyet.TruongKhoaDuyet == false)
                 || (duocDuyet.DuTruMuaDuocPhamTheoKhoa == null && duocDuyet.DuTruMuaDuocPhamKhoDuoc == null && duocDuyet.TruongKhoaDuyet == false)
                    )
                {
                    trangThaiVo.IsKhoDuocDuyet = false;
                }
                if ((duocDuyet.DuTruMuaDuocPhamTheoKhoa != null && duocDuyet.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null
                        && duocDuyet.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false)
                   || (duocDuyet.DuTruMuaDuocPhamKhoDuoc != null && duocDuyet.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false))
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
                if ((duocDuyet.DuTruMuaDuocPhamTheoKhoa != null && duocDuyet.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == true) || (duocDuyet.DuTruMuaDuocPhamKhoDuoc != null && duocDuyet.TruongKhoaDuyet == true))
                {
                    trangThaiVo.IsKhoDuocDuyet = true;
                }
                trangThaiVo.TrangThai = null;
                trangThaiVo.Ten = "Đang chờ duyệt";
            }
            return trangThaiVo;

        }
        public async Task<bool> CheckDuocPhamExists(long? duocPhamId, bool? laDuocPhamBHYT, List<DuocPhamDuTruViewModelValidator> duocPhams)
        {
            if (duocPhamId == null)
            {
                return true;
            }
            if (duocPhams != null)
            {
                foreach (var item in duocPhams)
                {
                    if (duocPhams.Any(p => p.DuocPhamId == duocPhamId && p.LaDuocPhamBHYT == laDuocPhamBHYT))
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
        public async Task<KyDuTruMuaDuocPhamVatTuVo> GetKyDuTruMuaDuocPhamVatTu(long kyDuTruMuaDuocPhamVatTuId)
        {
            var result = _kyDuTruMuaDuocPhamVatTuRepository.TableNoTracking.Where(p => p.Id == kyDuTruMuaDuocPhamVatTuId)
                         .Select(s => new KyDuTruMuaDuocPhamVatTuVo
                         {
                             TuNgay = s.TuNgay,
                             DenNgay = s.DenNgay
                         });
            return await result.FirstAsync();
        }

        public async Task<string> GetSoPhieuDuTru()
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
                    var lastDuTruId = string.Empty;
                    lastDuTruId = "0" + (lastSoPhieu + 1).ToString();
                    result = soPhieu + lastYearMonthCurrent + lastDuTruId;
                }
            }
            else
            {
                result = soPhieu + lastYearMonthCurrent + "0001";
            }
            return result;
        }

        public string InPhieuMuaDuTruDuocPham(PhieuMuaDuTruDuocPham phieuMuaDuTruDuocPham)
        {
            var contentThuoc = string.Empty;
            var contentHoaChat = string.Empty;

            var hearder = string.Empty;
            var templatePhieuMuaDuTruThuocVacXin = new Template();
            var templatePhieuMuaDuTruHoaChat = new Template();
            var thuocChiTiet = string.Empty;

            var phieuMuaThuoc = BaseRepository.GetById(phieuMuaDuTruDuocPham.DuTruMuaDuocPhamId, s =>
                                                                    s.Include(r => r.DuTruMuaDuocPhamChiTiets).ThenInclude(ct => ct.DuocPham).ThenInclude(dp => dp.DuongDung)
                                                                     .Include(r => r.DuTruMuaDuocPhamChiTiets).ThenInclude(ct => ct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                                                                     .Include(r => r.KyDuTruMuaDuocPhamVatTu)
                                                                     .Include(r => r.Kho).ThenInclude(p => p.PhongBenhVien)
                                                                     .Include(r => r.NhanVienYeuCau).ThenInclude(nv => nv.User)
                                                                     .Include(r => r.Kho).ThenInclude(k => k.KhoaPhong)
                                                                   );


            if (phieuMuaDuTruDuocPham.Header == true)
            {
                if (phieuMuaThuoc.NhomDuocPhamDuTru == EnumNhomDuocPhamDuTru.ThuocVacxin)
                {
                    hearder = "<p style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                            "<th>PHIẾU DỰ TRÙ THUỐC, VẮC XIN</th>" +
                       "</p>";
                }
                else
                {
                    hearder = "<p style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                            "<th>PHIẾU DỰ TRÙ HÓA CHẤT, HÓA CHẤT XÉT NGHIỆM</th>" +
                       "</p>";
                }

            }
            var groupBHYT = "BHYT";
            var groupKhongBHYT = "Không BHYT";

            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupBHYT.ToUpper()
                                        + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupKhongBHYT.ToUpper()
                                        + "</b></tr>";

            var query = _duTruMuaDuocPhamChiTietRepository
                           .TableNoTracking
                           .Where(p => p.DuTruMuaDuocPhamId == phieuMuaDuTruDuocPham.DuTruMuaDuocPhamId)
                           .Select(s => new PhieuMuaDuTruDuocPhamChiTietData
                           {
                               Ten = s.DuocPham.Ten,
                               HamLuong = s.DuocPham.HamLuong,
                               HoatChat = s.DuocPham.HoatChat,
                               DonVi = s.DuocPham.DonViTinh.Ten,
                               SoLuong = s.SoLuongDuTru,
                               Nhom = s.NhomDieuTriDuPhong != null ? s.NhomDieuTriDuPhong.GetDescription() : "",
                               GhiChu = s.GhiChu,
                               LaDuocPhamBHYT = s.LaDuocPhamBHYT
                           }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT).ToList();

            if (phieuMuaThuoc.NhomDuocPhamDuTru == EnumNhomDuocPhamDuTru.ThuocVacxin)
            {
                thuocChiTiet = string.Empty;
                templatePhieuMuaDuTruThuocVacXin = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuMuaDuTruThuocVacXin")).First();
                var STT = 1;
                if (query.Any(p => p.LaDuocPhamBHYT))
                {
                    thuocChiTiet += headerBHYT;
                    var queryBHYT = query.Where(x => x.LaDuocPhamBHYT).ToList();
                    foreach (var item in queryBHYT)
                    {
                        thuocChiTiet = thuocChiTiet
                                            + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                            + (!string.IsNullOrEmpty(item.HamLuong) ? " (" + "<b>" + item.HamLuong + "</b>)" : "")
                                            //+ "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.HoatChat
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.DonVi
                                            + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Nhom
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                            + "</tr>";
                        STT++;
                        groupBHYT = string.Empty;
                    }
                }
                if (query.Any(p => !p.LaDuocPhamBHYT))
                {
                    thuocChiTiet += headerKhongBHYT;
                    var queryKhongBHYT = query.Where(x => !x.LaDuocPhamBHYT).ToList();
                    foreach (var item in queryKhongBHYT)
                    {
                        thuocChiTiet = thuocChiTiet
                                            + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                            + (!string.IsNullOrEmpty(item.HamLuong) ? " (" + "<b>" + item.HamLuong + "</b>)" : "")
                                            //+ "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.HoatChat
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.DonVi
                                            + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Nhom
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                            + "</tr>";
                        STT++;
                        groupKhongBHYT = string.Empty;
                    }
                }

                var data = new PhieuMuaDuTruDuocPhamData
                {
                    Header = hearder,
                    MaPhieuMuaDuTruDuocPham = "BMBH-KD01.01",
                    Thuoc = thuocChiTiet,
                    TenKho = phieuMuaThuoc.Kho.Ten,
                    KhoaPhong = phieuMuaThuoc.Kho.KhoaPhong?.Ten,
                    //LogoUrl = phieuLinhThuongDuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    NhanVienLap = phieuMuaThuoc.NhanVienYeuCau.User.HoTen,
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };
                contentThuoc = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuMuaDuTruThuocVacXin.Body, data);
                return contentThuoc;

            }
            else
            {
                templatePhieuMuaDuTruHoaChat = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuMuaDuTruHoaChatXetNghiem")).First();
                thuocChiTiet = string.Empty;
                var STT = 1;
                if (query.Any(p => p.LaDuocPhamBHYT))
                {
                    thuocChiTiet += headerBHYT;
                    var queryBHYT = query.Where(x => x.LaDuocPhamBHYT).ToList();
                    foreach (var item in queryBHYT)
                    {
                        thuocChiTiet = thuocChiTiet
                                            + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                            + (!string.IsNullOrEmpty(item.HamLuong) ? " (" + "<b>" + item.HamLuong + "</b>)" : "")
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.DonVi
                                            + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                            + "</tr>";
                        STT++;
                        groupBHYT = string.Empty;
                    }
                }
                if (query.Any(p => !p.LaDuocPhamBHYT))
                {
                    thuocChiTiet += headerKhongBHYT;
                    var queryKhongBHYT = query.Where(x => !x.LaDuocPhamBHYT).ToList();
                    foreach (var item in queryKhongBHYT)
                    {
                        thuocChiTiet = thuocChiTiet
                                            + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                            + (!string.IsNullOrEmpty(item.HamLuong) ? " (" + "<b>" + item.HamLuong + "</b>)" : "")
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.DonVi
                                            + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong
                                            + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                            + "</tr>";
                        STT++;
                        groupKhongBHYT = string.Empty;
                    }
                }

                var data = new PhieuMuaDuTruDuocPhamData
                {
                    Header = hearder,
                    MaPhieuMuaDuTruHoaChat = "BMBH-KD01.02",
                    Thuoc = thuocChiTiet,
                    TenKho = phieuMuaThuoc.Kho.Ten,
                    KhoaPhong = phieuMuaThuoc.Kho.KhoaPhong?.Ten + (phieuMuaThuoc.Kho.PhongBenhVien != null ? " - " + phieuMuaThuoc.Kho.PhongBenhVien.Ten : ""),
                    NhanVienLap = phieuMuaThuoc.NhanVienYeuCau.User.HoTen,
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };
                contentHoaChat = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuMuaDuTruHoaChat.Body, data);
                return contentHoaChat;

            }
        }
    }
}
