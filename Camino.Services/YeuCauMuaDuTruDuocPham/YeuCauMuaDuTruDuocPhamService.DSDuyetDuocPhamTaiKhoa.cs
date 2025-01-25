using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using Camino.Core.Helpers;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain;
using System.Globalization;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;

namespace Camino.Services.YeuCauMuaDuTruDuocPham
{
    public partial class YeuCauMuaDuTruDuocPhamService
    {

        #region Danh sách hàm đang chờ xử lý

        public async Task<GridDataSource> GetDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long duTruMuaDuocPhamId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var queryable = _duTruMuaDuocPhamChiTietRepository
                    .TableNoTracking.Where(cc => cc.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                    .Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                    {
                        Id = item.Id,
                        DuocPhamId = item.DuocPhamId,
                        TenDuocPham = item.DuocPham.Ten,
                        LaBHYT = item.LaDuocPhamBHYT,
                        HoatChat = item.DuocPham.HoatChat,
                        DuongDung = item.DuocPham.DuongDung.Ten,
                        NongDoHamLuong = item.DuocPham.HamLuong,
                        HangSanXuat = item.DuocPham.NhaSanXuat,
                        DonViTinh = item.DuocPham.DonViTinh.Ten,
                        NuocSanXuat = item.DuocPham.NuocSanXuat,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        NhomDuPhong = item.NhomDieuTriDuPhong.GetDescription(),
                        KhoId = item.DuTruMuaDuocPham.KhoId,
                        SoDangKy = item.DuocPham.SoDangKy,
                        SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet == null ? item.SoLuongDuTru : (int)item.SoLuongDuTruTruongKhoaDuyet,
                        KhoDuTruTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.DuocPhamBenhVienId == item.DuocPhamId && x.NhapKhoDuocPhams.KhoId == item.DuTruMuaDuocPham.KhoId && x.LaDuocPhamBHYT == item.LaDuocPhamBHYT && x.NhapKhoDuocPhams.DaHet != true
                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                    });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
            var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            foreach (var item in queryTask.Result)
            {
                var thongTinHDT = ThongTinDuTruMuaDuocPhamHDThau(item);

                item.KhoTongTon = thongTinHDT.SLTonKhoTong ?? 0;
                item.ThongTinChiTietTonKhoTongs = thongTinHDT.ThongTinChiTietTonKhoTongs;
                item.HDChuaNhap = thongTinHDT.SLChuaNhapVeHDT ?? 0;
                item.ThongTinChiTietTonHDTs = thongTinHDT.ThongTinChiTietTonHDTs;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            long duTruMuaDuocPhamId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var queryable = _duTruMuaDuocPhamChiTietRepository
                    .TableNoTracking.Where(cc => cc.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                     .Include(g => g.DuocPham).ThenInclude(c => c.DonViTinh)
                    .Include(g => g.DuocPham).ThenInclude(c => c.DuongDung)
                    .Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                    {
                        Id = item.Id,
                        DuocPhamId = item.DuocPhamId,
                        TenDuocPham = item.DuocPham.Ten,
                        LaBHYT = item.LaDuocPhamBHYT,
                        HoatChat = item.DuocPham.HoatChat,
                        DuongDung = item.DuocPham.DuongDung.Ten,
                        NongDoHamLuong = item.DuocPham.HamLuong,
                        HangSanXuat = item.DuocPham.NhaSanXuat,
                        DonViTinh = item.DuocPham.DonViTinh.Ten,
                        NuocSanXuat = item.DuocPham.NuocSanXuat,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoDangKy = item.DuocPham.SoDangKy,
                        SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet == null ? item.SoLuongDuTru : (int)item.SoLuongDuTruTruongKhoaDuyet
                    });
            var countTask = queryable.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public ThongTinDuTruMuaDuocPham ThongTinDuTruMuaDuocPhamHDThau(DuTruMuaDuocPhamTaiKhoaChiTietVo thongTinChiTietDuocPhamTonKho)
        {
            var thongTin = new ThongTinDuTruMuaDuocPham();
            var duocPhamBenhVien = _duocPhamBenhVienRepository.TableNoTracking
                        .Any(p => p.Id == thongTinChiTietDuocPhamTonKho.DuocPhamId && p.HieuLuc == true);
            var laDuocPhamBHYT = thongTinChiTietDuocPhamTonKho.LaBHYT;

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
                                SLTonKhoTong = s.NhapKhoDuocPhamChiTiets
                                            .Where(nkct => nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && nkct.LaDuocPhamBHYT == laDuocPhamBHYT)
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


        #endregion

        #region Common

        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new DuTruMuaDuocPhamTaiKhoaSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaSearch>(queryInfo.AdditionalSearchString);

            }

            var queryDangChoDuyet = GetDataYeuCauTraDuocPham(null, queryInfo, queryObject);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);

                if (queryString.RangeFromDate != null &&
                           (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                }
            }

            var queryTuDaDuyet = GetDataYeuCauTraDuocPham(true, queryInfo, queryObject);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);

                if (queryString.RangeFromDate != null &&
                           (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    queryTuDaDuyet = queryTuDaDuyet.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                }
            }
            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
            .Select(s => new DuTruMuaDuocPhamTaiKhoaGridVo())
            .AsQueryable();
            var isHaveQuery = false;
            if (queryObject.ChoDuyet)
            {
                query = queryDangChoDuyet;

                isHaveQuery = true;
            }

            if (queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaDuyet);
                }
                else
                {
                    query = queryTuDaDuyet;
                }
            }

            if (queryObject.ChoDuyet == false && queryObject.DaDuyet == false)
            {
                query = queryDangChoDuyet;
                query = query.Concat(queryTuDaDuyet);
            }

            if (queryInfo.SortString != null
                && !queryInfo.SortString.Equals("NgayYeuCau desc,Id asc")
                && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                //Sort KyDuTru (DateTime - DateTime)
                var sortString = queryInfo.SortString.Contains("KyDuTru") ? queryInfo.SortString.Replace("KyDuTru", "TuNgay") : queryInfo.SortString;

                query = query.OrderBy(sortString);
            }

            var countTask = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaDuocPhamTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new DuTruMuaDuocPhamTaiKhoaSearch();


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = GetDataYeuCauTraDuocPham(null, queryInfo, queryObject);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);

                if (queryString.RangeFromDate != null &&
                           (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                }
            }

            var queryTuDaDuyet = GetDataYeuCauTraDuocPham(true, queryInfo, queryObject);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);

                if (queryString.RangeFromDate != null &&
                           (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    queryTuDaDuyet = queryTuDaDuyet.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                }
            }
            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
             .Select(s => new DuTruMuaDuocPhamTaiKhoaGridVo())
             .AsQueryable();
            var isHaveQuery = false;

            if (queryObject.ChoDuyet)
            {
                query = queryDangChoDuyet;

                isHaveQuery = true;
            }

            if (queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaDuyet);
                }
                else
                {
                    query = queryTuDaDuyet;
                }
            }

            if (queryObject.ChoDuyet == false && queryObject.DaDuyet == false)
            {
                query = queryDangChoDuyet;
                query = query.Concat(queryTuDaDuyet);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        private IQueryable<DuTruMuaDuocPhamTaiKhoaGridVo> GetDataYeuCauTraDuocPham(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaDuocPhamTaiKhoaSearch queryObject)
        {
            var nhanVienThuocKhos = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == _userAgentHelper.GetCurrentUserId()).Select(cc => cc.KhoId).ToList();
            var result = BaseRepository.TableNoTracking;
            if (!string.IsNullOrEmpty(queryObject.SearchString))
            {
                result = result.ApplyLike(queryObject.SearchString.Replace("\t", "").Trim(),
                       q => q.SoPhieu,
                       q => q.Kho.Ten,
                       q => q.NhanVienYeuCau.User.HoTen)
                  .OrderBy(queryInfo.SortString);
            }
            var data = result.Where(p => p.TruongKhoaDuyet == duocDuyet && p.TruongKhoaDuyet != false
                 && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin) && p.DuTruMuaDuocPhamTheoKhoaId == null && nhanVienThuocKhos.Contains(p.KhoId))
                             .Select(o => new DuTruMuaDuocPhamTaiKhoaGridVo
                             {
                                 Id = o.Id,
                                 SoPhieu = o.SoPhieu,
                                 TrangThai = o.TruongKhoaDuyet == null ? EnumTrangThaiLoaiDuTru.ChoDuyet.GetDescription() : EnumTrangThaiLoaiDuTru.ChoGoi.GetDescription() + "." + o.TuNgay.ToString("dd/MM/yyyy") + " - " + o.DenNgay.ToString("dd/MM/yyyy"),
                                 TenKho = o.Kho.Ten,
                                 LoaiNhom = o.NhomDuocPhamDuTru.GetDescription(),
                                 NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                                 TuNgay = o.TuNgay,
                                 DenNgay = o.DenNgay,
                                 NgayYeuCau = o.NgayYeuCau,
                                 NgayTruongKhoaDuyet = o.NgayTruongKhoaDuyet,
                                 KyDuTruMuaDuocPhamVatTuId = o.KyDuTruMuaDuocPhamVatTuId
                             });
            if (queryInfo.SortString != null && queryInfo.SortString.Equals("NgayYeuCau desc, Id asc"))
            {
                data = data.OrderBy(queryInfo.SortString);
            }
            return data;

        }

        public async Task<bool> TuChoiDuyetTaiKhoa(ThongTinLyDoHuyDuyetTaiKhoa thongTinLyDoHuyNhapKhoDuocPham)
        {
            var duTruMuaDuocPhamEntity = await BaseRepository.Table.Where(cc => cc.Id == thongTinLyDoHuyNhapKhoDuocPham.Id).FirstOrDefaultAsync();
            if (duTruMuaDuocPhamEntity == null)
                return false;

            duTruMuaDuocPhamEntity.TruongKhoaDuyet = false;
            duTruMuaDuocPhamEntity.LyDoTruongKhoaTuChoi = thongTinLyDoHuyNhapKhoDuocPham.LyDoHuy;
            duTruMuaDuocPhamEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
            duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = DateTime.Now;

            BaseRepository.Update(duTruMuaDuocPhamEntity);
            return true;
        }

        public async Task<bool> DuyetTaiKhoa(DuyetDuTruMuaDuocPhamViewModel model)
        {
            var duTruMuaDuocPhamEntity = await BaseRepository.Table
                            .Where(cc => cc.Id == model.DuTruMuaDuocPhamId)
                            .Include(cc => cc.DuTruMuaDuocPhamChiTiets)
                            .FirstOrDefaultAsync();

            duTruMuaDuocPhamEntity.TruongKhoaDuyet = true;
            duTruMuaDuocPhamEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
            duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = DateTime.Now;

            //var duTruMuaDuocPhamChiTiets = BaseRepository.Table.Where(cc => cc.Id == model.DuTruMuaDuocPhamId).SelectMany(cc => cc.DuTruMuaDuocPhamChiTiets).ToList();
            var duTruMuaDuocPhamChiTiets = _duTruMuaDuocPhamChiTietRepository.Table.Where(cc => cc.DuTruMuaDuocPhamId == model.DuTruMuaDuocPhamId).ToList();
            foreach (var item in duTruMuaDuocPhamChiTiets)
            {
                if (model.DuTruMuaDuocPhamTaiKhoaChiTietVos != null && model.DuTruMuaDuocPhamTaiKhoaChiTietVos.Any())
                {
                    foreach (var itemModel in model.DuTruMuaDuocPhamTaiKhoaChiTietVos)
                    {
                        if (item.Id == itemModel.Id)
                        {
                            item.SoLuongDuTruTruongKhoaDuyet = (int)itemModel.SoLuongDuTruTruongKhoaDuyet;
                        }
                    }
                }
                else
                {
                    item.SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTru;
                }
            }
            BaseRepository.Update(duTruMuaDuocPhamEntity);
            return true;
        }

        public async Task<bool> HuyDuyetTaiKhoa(long id)
        {
            var duTruMuaDuocPhamEntity = await BaseRepository.Table.Where(cc => cc.Id == id).FirstOrDefaultAsync();

            duTruMuaDuocPhamEntity.TruongKhoaDuyet = null;
            duTruMuaDuocPhamEntity.TruongKhoaId = null;
            duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = null;

            var duTruMuaDuocPhamChiTiets = BaseRepository.Table.Where(cc => cc.Id == id).SelectMany(cc => cc.DuTruMuaDuocPhamChiTiets);
            foreach (var item in duTruMuaDuocPhamChiTiets)
            {
                item.SoLuongDuTruTruongKhoaDuyet = null;
            }
            BaseRepository.Update(duTruMuaDuocPhamEntity);
            return true;
        }

        public DuTruMuaDuocPhamViewModel GetThongTinDuTruDuocPhamTaiKhoa(long duTruDuocPhamId)
        {
            var duTruMuaDuocPham = BaseRepository.TableNoTracking
                                                 .Where(cc => cc.Id == duTruDuocPhamId)
                                                 .Select(s => new DuTruMuaDuocPhamViewModel
                                                 {
                                                     //trưởng khoa duyêt 0 va chưa duyệt 1
                                                     TinhTrang = s.TruongKhoaDuyet == true ? 0 : 1,
                                                     GhiChu = s.GhiChu,
                                                     TuNgay = s.TuNgay,
                                                     DenNgay = s.DenNgay,
                                                     KhoNhapId = s.KhoId,
                                                     LoaiDuTru = s.NhomDuocPhamDuTru,
                                                     TenLoaiDuTru = s.NhomDuocPhamDuTru.GetDescription(),
                                                     NgayYeuCau = s.NgayYeuCau,
                                                     SoPhieu = s.SoPhieu,
                                                     TenNhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                                                     TenKho = s.Kho.Ten,
                                                     LyDoTruongKhoaTuChoi = s.DuTruMuaDuocPhamTheoKhoa != null && s.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == false ? s.DuTruMuaDuocPhamTheoKhoa.LyDoKhoDuocTuChoi : s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null
                                                     && s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false ? s.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.LyDoGiamDocTuChoi : s.LyDoTruongKhoaTuChoi,
                                                 }).FirstOrDefault();
            return duTruMuaDuocPham;
        }

        public DuTruMuaDuocPhamViewModel GetThongTinDuTruDuocPhamTaiKhoaDaXuLy(long duTruDuocPhamId)
        {
            var duTruMuaDuocPham = _duTruMuaDuocPhamTheoKhoaRepository.TableNoTracking
                                                 .Where(cc => cc.Id == duTruDuocPhamId)
                                                 .Select(s => new DuTruMuaDuocPhamViewModel
                                                 {
                                                     TinhTrang = ((s.KhoDuocDuyet == true && s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true) ? 1 : (s.KhoDuocDuyet == false || s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false) ? 2 : 0),
                                                     SoPhieu = s.SoPhieu,
                                                     KhoNhapId = s.DuTruMuaDuocPhams.First().KhoId,
                                                     TenKho = s.DuTruMuaDuocPhams.First().Kho.Ten,
                                                     LoaiDuTru = s.DuTruMuaDuocPhams.First().NhomDuocPhamDuTru,
                                                     TenLoaiDuTru = s.DuTruMuaDuocPhams.First().NhomDuocPhamDuTru.GetDescription(),
                                                     TuNgay = s.TuNgay,
                                                     DenNgay = s.DenNgay,
                                                     TenNhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                                                     NgayYeuCau = s.NgayYeuCau,
                                                     GhiChu = s.GhiChu,

                                                     TenNhanVienKhoDuocDuyet = s.NhanVienKhoDuoc.User.HoTen,
                                                     NgayKhoDuocDuyet = s.NgayKhoDuocDuyet,
                                                     LyDoTruongKhoaTuChoi = s.LyDoKhoDuocTuChoi,

                                                     TenGiamDocDuyet = s.DuTruMuaDuocPhamKhoDuoc.GiamDoc.User.HoTen,
                                                     NgayGiamDocDuyet = s.DuTruMuaDuocPhamKhoDuoc.NgayGiamDocDuyet,
                                                     LyDoGiamDocTuChoi = s.DuTruMuaDuocPhamKhoDuoc.LyDoGiamDocTuChoi
                                                 })
                                                 .FirstOrDefault();
            return duTruMuaDuocPham;
        }

        public GetThongTinGoiTaiKhoa GetThongTinGoiTaiKhoa(long phongBenhVienId)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var hoTenNguoiYeuCau = _useRepository.GetById(currentUserId).HoTen;

            var khoaPhong = _phongBenhVienRepository.TableNoTracking.Where(cc => cc.Id == phongBenhVienId).FirstOrDefault();
            var duTruMuaDuocPham = BaseRepository.TableNoTracking.FirstOrDefault();

            var thongTinDuTru = new GetThongTinGoiTaiKhoa
            {
                KhoaPhongId = khoaPhong.KhoaPhongId,
                TenKhoaPhong = khoaPhong.Ten,
                NguoiyeuCauId = currentUserId,
                TenNhanVienYeuCau = hoTenNguoiYeuCau,
                NgayYeuCau = DateTime.Now
            };
            return thongTinDuTru;
        }

        #endregion


        #region Danh sách dự trù mua tại khoa 

        public async Task<GridDataSource> GetDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var nhanVienThuocKhos = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == _userAgentHelper.GetCurrentUserId()).Select(cc => cc.KhoId).ToList();
            long kyDuTruMuaDuocPhamVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var duTruDuocPhamIds = BaseRepository.TableNoTracking.Where(cc => cc.KyDuTruMuaDuocPhamVatTuId == kyDuTruMuaDuocPhamVatTuId).Where(cc => cc.TruongKhoaDuyet == true
              && (cc.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || cc.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin) && nhanVienThuocKhos.Contains(cc.KhoId) && cc.DuTruMuaDuocPhamTheoKhoaId == null).Select(cc => cc.Id);



            var queryable = _duTruMuaDuocPhamChiTietRepository
                    .TableNoTracking.Where(cc => duTruDuocPhamIds.Contains(cc.DuTruMuaDuocPhamId))
                    .Include(g => g.DuocPham).ThenInclude(c => c.DonViTinh)
                    .Include(g => g.DuocPham).ThenInclude(c => c.DuongDung)
                    .Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                    {
                        Id = item.Id,
                        DuocPhamId = item.DuocPhamId,
                        TenDuocPham = item.DuocPham.Ten,
                        LaBHYT = item.LaDuocPhamBHYT,
                        HoatChat = item.DuocPham.HoatChat,
                        DuongDung = item.DuocPham.DuongDung.Ten,
                        NongDoHamLuong = item.DuocPham.HamLuong,
                        HangSanXuat = item.DuocPham.NhaSanXuat,
                        DonViTinh = item.DuocPham.DonViTinh.Ten,
                        NuocSanXuat = item.DuocPham.NuocSanXuat,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        NhomDuPhong = item.NhomDieuTriDuPhong.GetDescription(),
                        SoLuongDuTruTruongKhoaDuyet = (int)item.SoLuongDuTruTruongKhoaDuyet,
                        SoDangKy = item.DuocPham.SoDangKy,
                        DuTruMuaDuocPhamId = item.DuTruMuaDuocPhamId,
                        DuTruMuaDuocPhamChiTietId = item.Id,
                        DenNgay = item.DuTruMuaDuocPham.DenNgay,
                        TuNgay = item.DuTruMuaDuocPham.TuNgay,
                    }).GroupBy(x => new
                    {
                        x.DuocPhamId,
                        x.Nhom
                    }).Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                    {
                        Id = item.First().Id,
                        DuocPhamId = item.First().DuocPhamId,
                        TenDuocPham = item.First().TenDuocPham,
                        LaBHYT = item.First().LaBHYT,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HangSanXuat = item.First().HangSanXuat,
                        SoDangKy = item.First().SoDangKy,
                        DonViTinh = item.First().DonViTinh,
                        NuocSanXuat = item.First().NuocSanXuat,
                        NhomDuPhong = item.First().NhomDuPhong,
                        DenNgay = item.First().DenNgay,
                        TuNgay = item.First().TuNgay,
                        SoLuongDuTru = item.Sum(cc => cc.SoLuongDuTru),
                        SoLuongDuKienSuDung = item.Sum(cc => cc.SoLuongDuKienSuDung),
                        SoLuongDuTruTruongKhoaDuyet = item.Sum(cc => cc.SoLuongDuTruTruongKhoaDuyet),
                        DuTruMuaDuocPhamIds = item.Select(c => c.DuTruMuaDuocPhamId).ToList(),
                        DuTruMuaDuocPhamChiTietIds = item.Select(c => c.DuTruMuaDuocPhamChiTietId).ToList()
                    });
            //.Skip(queryInfo.Skip).Take(queryInfo.Take)
            var data = queryable.ToArray();
            return new GridDataSource { Data = data, TotalRowCount = 0 };
        }

        public async Task<GridDataSource> GetTotalDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            var nhanVienThuocKhos = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == _userAgentHelper.GetCurrentUserId()).Select(cc => cc.KhoId).ToList();
            long kyDuTruMuaDuocPhamVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var duTruDuocPhamIds = BaseRepository.TableNoTracking.Where(cc => cc.KyDuTruMuaDuocPhamVatTuId == kyDuTruMuaDuocPhamVatTuId).Where(cc => cc.TruongKhoaDuyet == true
              && (cc.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || cc.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin) && nhanVienThuocKhos.Contains(cc.KhoId) && cc.DuTruMuaDuocPhamTheoKhoaId == null).Select(cc => cc.Id);

            var queryable = _duTruMuaDuocPhamChiTietRepository
                    .TableNoTracking.Where(cc => duTruDuocPhamIds.Contains(cc.DuTruMuaDuocPhamId))
                    .Include(g => g.DuocPham).ThenInclude(c => c.DonViTinh)
                    .Include(g => g.DuocPham).ThenInclude(c => c.DuongDung)
                    .Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                    {
                        Id = item.Id,
                        DuocPhamId = item.DuocPhamId,
                        TenDuocPham = item.DuocPham.Ten,
                        LaBHYT = item.LaDuocPhamBHYT,
                        HoatChat = item.DuocPham.HoatChat,
                        DuongDung = item.DuocPham.DuongDung.Ten,
                        NongDoHamLuong = item.DuocPham.HamLuong,
                        HangSanXuat = item.DuocPham.NhaSanXuat,
                        DonViTinh = item.DuocPham.DonViTinh.Ten,
                        NuocSanXuat = item.DuocPham.NuocSanXuat,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        NhomDuPhong = item.NhomDieuTriDuPhong.GetDescription(),
                        SoLuongDuTruTruongKhoaDuyet = (int)item.SoLuongDuTruTruongKhoaDuyet,
                        SoDangKy = item.DuocPham.SoDangKy,

                        DuTruMuaDuocPhamId = item.DuTruMuaDuocPhamId,
                        DuTruMuaDuocPhamChiTietId = item.Id
                    }).GroupBy(x => new
                    {
                        x.DuocPhamId,
                        x.Nhom
                    }).Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                    {
                        Id = item.First().Id,
                        DuocPhamId = item.First().DuocPhamId,
                        TenDuocPham = item.First().TenDuocPham,
                        LaBHYT = item.First().LaBHYT,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HangSanXuat = item.First().HangSanXuat,
                        SoDangKy = item.First().SoDangKy,
                        DonViTinh = item.First().DonViTinh,
                        NuocSanXuat = item.First().NuocSanXuat,
                        NhomDuPhong = item.First().NhomDuPhong,
                        SoLuongDuTru = item.Sum(cc => cc.SoLuongDuTru),
                        SoLuongDuKienSuDung = item.Sum(cc => cc.SoLuongDuKienSuDung),
                        SoLuongDuTruTruongKhoaDuyet = item.Sum(cc => cc.SoLuongDuTruTruongKhoaDuyet),
                        DuTruMuaDuocPhamIds = item.Select(c => c.DuTruMuaDuocPhamId).ToList(),
                        DuTruMuaDuocPhamChiTietIds = item.Select(c => c.DuTruMuaDuocPhamChiTietId).ToList()
                    });
            return new GridDataSource { TotalRowCount = queryable.Count() };
        }

        public async Task<GridDataSource> GetDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryString = queryInfo.AdditionalSearchString.Split('-');

            long kyDuTruMuaDuocPhamVatTuId = long.Parse(queryString[0]);
            bool laBHYT = bool.Parse(queryString[1]);
            long duocPhamId = long.Parse(queryString[2]);

            var nhanVienThuocKhos = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == _userAgentHelper.GetCurrentUserId()).Select(cc => cc.KhoId).ToList();

            var duTruDuocPhamIds = BaseRepository.TableNoTracking.Where(cc => cc.KyDuTruMuaDuocPhamVatTuId == kyDuTruMuaDuocPhamVatTuId && cc.TruongKhoaDuyet == true
              && cc.DuTruMuaDuocPhamTheoKhoaId == null && (cc.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || cc.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin) && nhanVienThuocKhos.Contains(cc.KhoId)).Select(cc => cc.Id);

            var queryable = _duTruMuaDuocPhamChiTietRepository
                    .TableNoTracking.Where(cc => duTruDuocPhamIds.Contains(cc.DuTruMuaDuocPhamId) && cc.LaDuocPhamBHYT == laBHYT && cc.DuocPhamId == duocPhamId)
                    .Select(item => new ThongTinDuTruMuaChiTietTaiKhoa()
                    {
                        Id = item.Id,
                        LoaiNhom = item.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                        TuNgay = item.DuTruMuaDuocPham.TuNgay,
                        TenKho = item.DuTruMuaDuocPham.Kho.Ten,
                        DenNgay = item.DuTruMuaDuocPham.DenNgay,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = (int)item.SoLuongDuTruTruongKhoaDuyet
                    });

            var data = queryable.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = data, TotalRowCount = 0 };
        }

        public async Task<GridDataSource> GetTotalDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryString = queryInfo.AdditionalSearchString.Split('-');

            long kyDuTruMuaDuocPhamVatTuId = long.Parse(queryString[0]);
            bool laBHYT = bool.Parse(queryString[1]);
            long duocPhamId = long.Parse(queryString[2]);

            var nhanVienThuocKhos = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == _userAgentHelper.GetCurrentUserId()).Select(cc => cc.KhoId).ToList();
            var duTruDuocPhamIds = BaseRepository.TableNoTracking.Where(cc => cc.KyDuTruMuaDuocPhamVatTuId == kyDuTruMuaDuocPhamVatTuId && cc.TruongKhoaDuyet == true
                && cc.DuTruMuaDuocPhamTheoKhoaId == null && (cc.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || cc.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin) && nhanVienThuocKhos.Contains(cc.KhoId)).Select(cc => cc.Id);

            var queryable = _duTruMuaDuocPhamChiTietRepository
                    .TableNoTracking.Where(cc => duTruDuocPhamIds.Contains(cc.DuTruMuaDuocPhamId) && cc.LaDuocPhamBHYT == laBHYT && cc.DuocPhamId == duocPhamId)
                    .Select(item => new ThongTinDuTruMuaChiTietTaiKhoa()
                    {
                        Id = item.Id,
                        LoaiNhom = item.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                        TuNgay = item.DuTruMuaDuocPham.TuNgay,
                        TenKho = item.DuTruMuaDuocPham.Kho.Ten,
                        DenNgay = item.DuTruMuaDuocPham.DenNgay,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = (int)item.SoLuongDuTruTruongKhoaDuyet
                    });

            var data = queryable.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { TotalRowCount = data.Count() };
        }

        public long GoiThongTinTaiKhoa(DuyetDuTruMuaDuocPhamViewModel model)
        {
            var kyDuTruMuaDuocPhamVatTu = _kyDuTruMuaDuocPhamVatTuRepository.TableNoTracking.Where(cc => cc.Id == model.DuTruMuaDuocPhamId).FirstOrDefault();
            var soPhieu = GetSoPhieuDuTruTheKhoa();
            var duTruMuaDuocPhamTheoKhoaModel = new DuTruMuaDuocPhamTheoKhoa()
            {
                SoPhieu = soPhieu,
                GhiChu = model.GhiChu,
                KhoaPhongId = model.KhoaPhongId,
                NhanVienYeuCauId = _userAgentHelper.GetCurrentUserId(),
                NgayYeuCau = model.NgayYeuCau,
                KyDuTruMuaDuocPhamVatTuId = kyDuTruMuaDuocPhamVatTu.Id,
                TuNgay = kyDuTruMuaDuocPhamVatTu.TuNgay,
                DenNgay = kyDuTruMuaDuocPhamVatTu.DenNgay
            };

            if (model.DuTruMuaDuocPhamTaiKhoaChiTietVos.Any())
            {
                foreach (var duTruMuaDuocPhamTaiKhoaChiTiet in model.DuTruMuaDuocPhamTaiKhoaChiTietVos)
                {
                    var duTruMuaDuocPhamTheoKhoaChiTiet = new DuTruMuaDuocPhamTheoKhoaChiTiet
                    {
                        LaDuocPhamBHYT = duTruMuaDuocPhamTaiKhoaChiTiet.LaBHYT,
                        DuocPhamId = duTruMuaDuocPhamTaiKhoaChiTiet.DuocPhamId,
                        SoLuongDuTru = (int)duTruMuaDuocPhamTaiKhoaChiTiet.SoLuongDuTru,
                        SoLuongDuKienSuDung = (int)duTruMuaDuocPhamTaiKhoaChiTiet.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = (int)duTruMuaDuocPhamTaiKhoaChiTiet.SoLuongDuTruTruongKhoaDuyet,
                    };
                    duTruMuaDuocPhamTheoKhoaModel.DuTruMuaDuocPhamTheoKhoaChiTiets.Add(duTruMuaDuocPhamTheoKhoaChiTiet);
                }
            }

            _duTruMuaDuocPhamTheoKhoaRepository.Add(duTruMuaDuocPhamTheoKhoaModel);

            //update lai dự trù mua và dự trù mua chi tiết
            if (duTruMuaDuocPhamTheoKhoaModel != null)
            {
                // var duTruMuaDuocPhamIds = model.DuTruMuaDuocPhamTaiKhoaChiTietVos.FirstOrDefault().DuTruMuaDuocPhamIds;
                var newListInt = new List<long>();

                foreach (var item in model.DuTruMuaDuocPhamTaiKhoaChiTietVos)
                {
                    foreach (var duTruMuaDuocPhamId in item.DuTruMuaDuocPhamIds)
                    {
                        newListInt.Add(duTruMuaDuocPhamId);
                    }

                }

                var duTruDuocPhamIds = newListInt.Select(c => c).Distinct().ToList();
                var duTruMuaDuocPhams = BaseRepository.TableNoTracking.Where(cc => duTruDuocPhamIds.Contains(cc.Id))
                                                                      .Include(c => c.DuTruMuaDuocPhamChiTiets).ToList();



                //var duTruMuaDuocPhamChiTietIds = model.DuTruMuaDuocPhamTaiKhoaChiTietVos.FirstOrDefault().DuTruMuaDuocPhamChiTietIds;
                //var duTruMuaDuocPhamChiTiets = _duTruMuaDuocPhamChiTietRepository.TableNoTracking.Where(cc => duTruMuaDuocPhamChiTietIds.Contains(cc.Id)).ToList();

                if (duTruMuaDuocPhams.Any())
                {
                    foreach (var duTruMuaDuocPham in duTruMuaDuocPhams)
                    {
                        duTruMuaDuocPham.DuTruMuaDuocPhamTheoKhoaId = duTruMuaDuocPhamTheoKhoaModel.Id;

                        BaseRepository.Update(duTruMuaDuocPham);

                        //update cho DuTruMuaDuocPhamChiTiets cập nhât DuTruMuaDuocPhamTheoKhoaChiTietId cho  
                        if (duTruMuaDuocPham.DuTruMuaDuocPhamChiTiets.Any())
                        {
                            foreach (var duTruMuaDuocPhamChiTiet in duTruMuaDuocPham.DuTruMuaDuocPhamChiTiets)
                            {
                                foreach (var listDuTruMuaDuocPhamTheoKhoaChiTiet in duTruMuaDuocPhamTheoKhoaModel.DuTruMuaDuocPhamTheoKhoaChiTiets)
                                {
                                    if (listDuTruMuaDuocPhamTheoKhoaChiTiet.DuocPhamId == duTruMuaDuocPhamChiTiet.DuocPhamId && listDuTruMuaDuocPhamTheoKhoaChiTiet.LaDuocPhamBHYT == duTruMuaDuocPhamChiTiet.LaDuocPhamBHYT)
                                    {
                                        duTruMuaDuocPhamChiTiet.DuTruMuaDuocPhamTheoKhoaChiTietId = listDuTruMuaDuocPhamTheoKhoaChiTiet.Id;
                                        _duTruMuaDuocPhamChiTietRepository.Update(duTruMuaDuocPhamChiTiet);
                                    }
                                }

                            }
                        }
                    }
                }
            }

            return duTruMuaDuocPhamTheoKhoaModel.Id;
        }

        public string GetSoPhieuDuTruTheKhoa()
        {
            var result = string.Empty;
            var soPhieu = "THDT";
            var lastYearMonthCurrent = DateTime.Now.ToString("yyMM");
            var lastSoPhieuStr = _duTruMuaDuocPhamTheoKhoaRepository.TableNoTracking.Select(p => p.SoPhieu).LastOrDefault();

            if (lastSoPhieuStr != null)
            {
                var lastSoPhieu = int.Parse(lastSoPhieuStr.Substring(lastSoPhieuStr.Length - 4));
                if (lastSoPhieu < 10)
                {
                    var lastDuTruId = "000" + (lastSoPhieu + 1).ToString();
                    result = soPhieu + lastYearMonthCurrent + lastDuTruId;
                }
                else if (lastSoPhieu < 100)
                {
                    var lastDuTruId = "00" + (lastSoPhieu + 1).ToString();
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

        public string InPhieuDuTruMuaTaiKhoa(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var contentThuoc = string.Empty;
            var contentHoaChat = string.Empty;
            var hearder = string.Empty;
            var templatePhieuInTongHopDuTruDuocPhamTaiKhoa = new Template();
            var duTruMuaDuocPhams = _duTruMuaDuocPhamTheoKhoaRepository.TableNoTracking.Where(c => c.Id == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId)
                                                                                     .Include(cc => cc.KhoaPhong)
                                                                                     .Include(cc => cc.NhanVienYeuCau).ThenInclude(c => c.User)
                                                                                     .Include(cc => cc.DuTruMuaDuocPhamTheoKhoaChiTiets)
                                                                                     .ThenInclude(cc => cc.DuocPham).ThenInclude(cc => cc.DonViTinh);

            //if (phieuInDuTruMuaTaiKhoa.Header)
            //{
            //    hearder = "<p style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                           "<th>PHIẾU TỔNG HỢP DỰ TRÙ THUỐC,VÁC XIN,HÓA CHẤT XÉT NGHIỆM</th>" +
            //                      "</p>";
            //}

            var groupBHYT = "BHYT";
            var groupKhongBHYT = "Không BHYT";
            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                     + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupBHYT.ToUpper()
                                     + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupKhongBHYT.ToUpper()
                                        + "</b></tr>";


            var tuNgay = string.Empty;
            var denNgay = string.Empty;

            var kyDuTruId = _duTruMuaDuocPhamTheoKhoaRepository.TableNoTracking
                                            .Where(p => p.Id == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId)
                                            .SelectMany(d => d.DuTruMuaDuocPhams)
                                            .Select(d => d.KyDuTruMuaDuocPhamVatTuId)
                                            .ToList();
            if (kyDuTruId.Count() > 0)
            {
                var kyDuTruInfo = _kyDuTruMuaDuocPhamVatTuRepository.TableNoTracking.Where(d => d.Id == kyDuTruId.First())
                    .Select(d => new {
                        TuNgay = d.TuNgay.ApplyFormatDate(),
                        DenNgay = d.DenNgay.ApplyFormatDate()
                    }).First();
                if (kyDuTruInfo != null)
                {
                    tuNgay = kyDuTruInfo.TuNgay;
                    denNgay = kyDuTruInfo.DenNgay;
                }
            }

            var duTruMuaDuocPhamChiTiet = string.Empty;
            templatePhieuInTongHopDuTruDuocPhamTaiKhoa = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuInTongHopDuTruDuocPhamTaiKhoa")).First();
            //var duTruMuaDuocPhamChiTiets = duTruMuaDuocPhams.SelectMany(cc => cc.DuTruMuaDuocPhamTheoKhoaChiTiets).Include(cc => cc.DuocPham).ThenInclude(cc => cc.DonViTinh).ToList();
            //PhieuMuaDuTruDuocPhamChiTietData
            var duTruMuaDuocPhamChiTiets = _duTruMuaDuocPhamTheoKhoaChiTietRepository.TableNoTracking
                                            .Where(p => p.DuTruMuaDuocPhamTheoKhoaId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId)
                                            .Select(s => new PhieuMuaDuTruDuocPhamChiTietData
                                            {
                                                MaHang = s.DuocPham.DuocPhamBenhVien.Ma, // todo: cần confirm hỏi lại
                                                Ten = s.DuocPham.Ten,
                                                HoatChat = s.DuocPham.HoatChat,
                                                HamLuong = s.DuocPham.HamLuong,
                                                DonVi = s.DuocPham.DonViTinh.Ten,
                                                SoLuong = s.SoLuongDuTruTruongKhoaDuyet,
                                                GhiChu = "",
                                                LaDuocPhamBHYT = s.LaDuocPhamBHYT
                                            }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT).ToList();
            var STT = 1;

            if (duTruMuaDuocPhamChiTiets.Any(p => p.LaDuocPhamBHYT))
            {
                duTruMuaDuocPhamChiTiet += headerBHYT;
                var queryBHYT = duTruMuaDuocPhamChiTiets.Where(x => x.LaDuocPhamBHYT).ToList();
                foreach (var item in queryBHYT)
                {
                    duTruMuaDuocPhamChiTiet = duTruMuaDuocPhamChiTiet
                                    + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                    + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                    + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.MaHang
                                    + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.Ten
                                            + (!string.IsNullOrEmpty(item.HamLuong) ? " (" + "<b>" + item.HamLuong + "</b>)" : "")
                                    + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.DonVi
                                    + "<td style = 'border: 1px solid #020000;text-align: right;  padding-right:3px'>" + item.SoLuong
                                    + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.GhiChu
                                    + "</tr>";
                    STT++;
                    groupBHYT = string.Empty;
                }
            }
            if (duTruMuaDuocPhamChiTiets.Any(p => !p.LaDuocPhamBHYT))
            {
                duTruMuaDuocPhamChiTiet += headerKhongBHYT;
                var queryKhongBHYT = duTruMuaDuocPhamChiTiets.Where(x => !x.LaDuocPhamBHYT).ToList();
                foreach (var item in queryKhongBHYT)
                {
                    duTruMuaDuocPhamChiTiet = duTruMuaDuocPhamChiTiet
                                     + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                     + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                     + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.MaHang
                                     + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.Ten
                                             + (!string.IsNullOrEmpty(item.HamLuong) ? " (" + "<b>" + item.HamLuong + "</b>)" : "")
                                     + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.DonVi
                                     + "<td style = 'border: 1px solid #020000;text-align: right;  padding-right:3px'>" + item.SoLuong
                                     + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.GhiChu
                                     + "</tr>";
                    STT++;
                    groupKhongBHYT = string.Empty;
                }
            }
            //foreach (var item in duTruMuaDuocPhamChiTiets)
            //{
            //    STT++;
            //    duTruMuaDuocPhamChiTiet = duTruMuaDuocPhamChiTiet
            //                        + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
            //                        + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
            //                        + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.MaHang
            //                        + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.Ten
            //                                + (!string.IsNullOrEmpty(item.HamLuong) ? " (" + "<b>" + item.HamLuong + "</b>)" : "")
            //                        + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.DonVi
            //                        + "<td style = 'border: 1px solid #020000;text-align: right;  padding-right:3px'>" + item.SoLuong
            //                        + "<td style = 'border: 1px solid #020000;text-align: left;  padding-left:3px'>" + item.GhiChu
            //                        + "</tr>";
            //}

            var data = new
            {
                Header = hearder,
                MaPhieuMuaDuTruHoaChat = "BMBH-KD02",
                DuTruMuaDuocPhamChiTiet = duTruMuaDuocPhamChiTiet,
                KhoaPhong = duTruMuaDuocPhams.FirstOrDefault().KhoaPhong?.Ten,
                NhanVienLap = duTruMuaDuocPhams.FirstOrDefault().NhanVienYeuCau.User.HoTen,

                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString(),
                TuNgay = tuNgay,
                DenNgay = denNgay
            };

            contentThuoc = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInTongHopDuTruDuocPhamTaiKhoa.Body, data);

            return contentThuoc;

        }

        #endregion

        #region DS THDT Da Xu Ly
        public async Task<GridDataSource> GetDataTHDTTaiKhoaDaXuLyForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var nhanVienLoginId = _userAgentHelper.GetCurrentUserId();
            var khoNhanVienQuanLys = _khoNhanVienQuanLyRepository.TableNoTracking.Where(p => p.NhanVienId == nhanVienLoginId && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin)).Select(p => p.KhoId).ToList();
            var query = _duTruMuaDuocPhamTheoKhoaRepository.TableNoTracking
                .Where(p => p.DuTruMuaDuocPhams.Any(dp => khoNhanVienQuanLys.Contains(dp.KhoId)))
                .Select(s => new DuTruMuaDuocPhamTaiKhoaGridVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    TenKhoa = s.KhoaPhong.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    TuNgay = s.TuNgay,
                    DenNgay = s.DenNgay,
                    // 0: đã gửi và chờ duyệt, 1: đã duyệt, 2: từ chối
                    TinhTrang = ((s.KhoDuocDuyet == true && s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true) ? 1 : (s.KhoDuocDuyet == false || s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false) ? 2 : 0),
                    NgayKhoDuocDuyet = s.NgayKhoDuocDuyet,
                    NgayGiamDocDuyet = s.DuTruMuaDuocPhamKhoDuoc.NgayGiamDocDuyet,
                    GhiChu = s.GhiChu
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
                // 0: đã gửi và chờ duyệt, 1: đã duyệt, 2: từ chối
                if (queryString.DaGuiChoDuyet == false && queryString.DaDuyetDaXuLy == true && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 1 || p.TinhTrang == 2);
                }
                else if (queryString.DaGuiChoDuyet == false && queryString.DaDuyetDaXuLy == false && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 2);
                }
                else if (queryString.DaGuiChoDuyet == true && queryString.DaDuyetDaXuLy == false && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 0 || p.TinhTrang == 2);
                }
                else if (queryString.DaGuiChoDuyet == false && queryString.DaDuyetDaXuLy == true && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.DaGuiChoDuyet == true && queryString.DaDuyetDaXuLy == true && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0 || p.TinhTrang == 1);
                }
                else if (queryString.DaGuiChoDuyet == true && queryString.DaDuyetDaXuLy == false && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0);
                }

                if (queryString.RangeFromDateDaXuLy != null &&
                            (!string.IsNullOrEmpty(queryString.RangeFromDateDaXuLy.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDateDaXuLy.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDateDaXuLy.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDateDaXuLy.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDateDaXuLy.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDateDaXuLy.DenNgay) || p.NgayYeuCau <= denNgay));
                }

                if (!string.IsNullOrEmpty(queryString.SearchStringDaXuLy))
                {
                    var searchTerms = queryString.SearchStringDaXuLy.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.TenKhoa,
                         g => g.NguoiYeuCau,
                         g => g.GhiChu


                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.SoPhieu,
                    g => g.TenKhoa,
                    g => g.NguoiYeuCau,
                         g => g.GhiChu

                    );
            }

            //Sort KyDuTru (DateTime - DateTime)
            var sortString = queryInfo.SortString.Contains("KyDuTru") ? queryInfo.SortString.Replace("KyDuTru", "TuNgay") : queryInfo.SortString;

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(sortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalTHDTTaiKhoaDaXuLyForGridAsync(QueryInfo queryInfo)
        {
            var nhanVienLoginId = _userAgentHelper.GetCurrentUserId();
            var khoNhanVienQuanLys = _khoNhanVienQuanLyRepository.TableNoTracking.Where(p => (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin)
                                                 && p.NhanVienId == nhanVienLoginId).Select(p => p.KhoId).ToList();
            var query = _duTruMuaDuocPhamTheoKhoaRepository.TableNoTracking
                .Where(p => p.DuTruMuaDuocPhams.Any(dp => khoNhanVienQuanLys.Contains(dp.KhoId)))
                .Select(s => new DuTruMuaDuocPhamTaiKhoaGridVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    TenKhoa = s.KhoaPhong.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TuNgay = s.TuNgay,
                    DenNgay = s.DenNgay,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    // 0: đã gửi và chờ duyệt, 1: đã duyệt, 2: từ chối
                    TinhTrang = ((s.KhoDuocDuyet == true && s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true) ? 1 : (s.KhoDuocDuyet == false || s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false) ? 2 : 0),
                    NgayKhoDuocDuyet = s.NgayKhoDuocDuyet,
                    NgayGiamDocDuyet = s.DuTruMuaDuocPhamKhoDuoc.NgayGiamDocDuyet,
                    GhiChu = s.GhiChu

                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
                // 0: đã gửi và chờ duyệt, 1: đã duyệt, 2: từ chối
                if (queryString.DaGuiChoDuyet == false && queryString.DaDuyetDaXuLy == true && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 1 || p.TinhTrang == 2);
                }
                else if (queryString.DaGuiChoDuyet == false && queryString.DaDuyetDaXuLy == false && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 2);
                }
                else if (queryString.DaGuiChoDuyet == true && queryString.DaDuyetDaXuLy == false && queryString.TuChoiDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 0 || p.TinhTrang == 2);
                }
                else if (queryString.DaGuiChoDuyet == false && queryString.DaDuyetDaXuLy == true && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.DaGuiChoDuyet == true && queryString.DaDuyetDaXuLy == true && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0 || p.TinhTrang == 1);
                }
                else if (queryString.DaGuiChoDuyet == true && queryString.DaDuyetDaXuLy == false && queryString.TuChoiDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0);
                }

                if (queryString.RangeFromDateDaXuLy != null &&
                            (!string.IsNullOrEmpty(queryString.RangeFromDateDaXuLy.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDateDaXuLy.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDateDaXuLy.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDateDaXuLy.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDateDaXuLy.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDateDaXuLy.DenNgay) || p.NgayYeuCau <= denNgay));
                }

                if (!string.IsNullOrEmpty(queryString.SearchStringDaXuLy))
                {
                    var searchTerms = queryString.SearchStringDaXuLy.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.TenKhoa,
                         g => g.NguoiYeuCau,
                          g => g.GhiChu
                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.SoPhieu,
                    g => g.TenKhoa,
                    g => g.NguoiYeuCau,
                    g => g.GhiChu

                    );
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                return null;
            }
            var duTruMuaDuocPhamTheoKhoaId = long.Parse(queryInfo.SearchTerms);
            var query = BaseRepository.TableNoTracking
                        .Where(p => p.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamTheoKhoaId)
                        .Select(s => new DuTruMuaDuocPhamTaiKhoaGridVoDetail
                        {
                            Id = s.Id,
                            SoPhieu = s.SoPhieu,
                            LoaiNhom = s.NhomDuocPhamDuTru.GetDescription(),
                            TenKho = s.Kho.Ten,
                            TuNgay = s.TuNgay,
                            DenNgay = s.DenNgay,
                            NgayYeuCau = s.NgayYeuCau,
                            NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                            NgayTruongKhoaDuyet = s.NgayTruongKhoaDuyet,
                            TinhTrang = (s.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == true && s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true) ? 1 : (s.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == false || s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false) ? 2 : 0,
                        });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                return null;
            }
            var duTruMuaDuocPhamTheoKhoaId = long.Parse(queryInfo.SearchTerms);
            var query = BaseRepository.TableNoTracking
                        .Where(p => p.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamTheoKhoaId)
                        .Select(s => new DuTruMuaDuocPhamTaiKhoaGridVoDetail
                        {
                            Id = s.Id,
                            SoPhieu = s.SoPhieu,
                            LoaiNhom = s.NhomDuocPhamDuTru.GetDescription(),
                            TenKho = s.Kho.Ten,
                            TuNgay = s.TuNgay,
                            DenNgay = s.DenNgay,
                            NgayYeuCau = s.NgayYeuCau,
                            NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                            NgayTruongKhoaDuyet = s.NgayTruongKhoaDuyet,
                            TinhTrang = (s.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == true && s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == true) ? 1 : (s.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == false || s.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false) ? 2 : 0,
                        });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region DS THDT đã xử lý chi tiết       
        public async Task<GridDataSource> GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            long duTruMuaDuocPhamTheoKhoaId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaDuocPhamTheoKhoaChiTietRepository.TableNoTracking
                .Where(x => x.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamTheoKhoaId)
                .Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                {
                    Id = item.Id,
                    DuTruMuaDuocPhamTheoKhoaId = item.DuTruMuaDuocPhamTheoKhoaId,
                    DuocPhamId = item.DuocPhamId,
                    TenDuocPham = item.DuocPham.Ten,
                    LaBHYT = item.LaDuocPhamBHYT,
                    HoatChat = item.DuocPham.HoatChat,
                    DuongDung = item.DuocPham.DuongDung.Ten,
                    NongDoHamLuong = item.DuocPham.HamLuong,
                    HangSanXuat = item.DuocPham.NhaSanXuat,
                    DonViTinh = item.DuocPham.DonViTinh.Ten,
                    NuocSanXuat = item.DuocPham.NuocSanXuat,
                    //NhomDuPhong = item.DuTruMuaDuocPhamChiTiets.First().NhomDieuTriDuPhong.GetDescription(),
                    SoDangKy = item.DuocPham.SoDangKy,
                    SoLuongDuTru = item.SoLuongDuTru,
                    SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                    SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet,
                })
                .GroupBy(x => new
                {
                    x.DuTruMuaDuocPhamTheoKhoaId,
                    x.DuocPhamId,
                    x.LaBHYT,
                    x.Nhom,
                    //x.NhomDuPhong,
                    x.SoLuongDuTru,
                    x.SoLuongDuKienSuDung,
                    x.SoLuongDuTruTruongKhoaDuyet,
                    x.NongDoHamLuong,
                    x.HoatChat,
                    x.DuongDung,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat
                })
                .Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                {
                    DuTruMuaDuocPhamTheoKhoaId = item.First().DuTruMuaDuocPhamTheoKhoaId,
                    DuocPhamId = item.First().DuocPhamId,
                    LaBHYT = item.First().LaBHYT,
                    TenDuocPham = item.First().TenDuocPham,
                    //NhomDuPhong = item.First().NhomDuPhong,
                    SoDangKy = item.First().SoDangKy,
                    NongDoHamLuong = item.First().NongDoHamLuong,
                    HoatChat = item.First().HoatChat,
                    DuongDung = item.First().DuongDung,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    NuocSanXuat = item.First().NuocSanXuat,
                    SoLuongDuTru = item.Sum(x => x.SoLuongDuTru),
                    SoLuongDuKienSuDung = item.Sum(x => x.SoLuongDuKienSuDung),
                    SoLuongDuTruTruongKhoaDuyet = item.Sum(x => x.SoLuongDuTruTruongKhoaDuyet),
                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            long duTruMuaDuocPhamTheoKhoaId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaDuocPhamTheoKhoaChiTietRepository.TableNoTracking
                .Where(x => x.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamTheoKhoaId)
                .Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                {
                    Id = item.Id,
                    DuTruMuaDuocPhamTheoKhoaId = item.DuTruMuaDuocPhamTheoKhoaId,
                    DuocPhamId = item.DuocPhamId,
                    TenDuocPham = item.DuocPham.Ten,
                    LaBHYT = item.LaDuocPhamBHYT,
                    HoatChat = item.DuocPham.HoatChat,
                    DuongDung = item.DuocPham.DuongDung.Ten,
                    NongDoHamLuong = item.DuocPham.HamLuong,
                    HangSanXuat = item.DuocPham.NhaSanXuat,
                    DonViTinh = item.DuocPham.DonViTinh.Ten,
                    NuocSanXuat = item.DuocPham.NuocSanXuat,
                    //NhomDuPhong = item.DuTruMuaDuocPhamChiTiets.First().NhomDieuTriDuPhong.GetDescription(),
                    SoDangKy = item.DuocPham.SoDangKy,
                    SoLuongDuTru = item.SoLuongDuTru,
                    SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                    SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet,
                })
                .GroupBy(x => new
                {
                    x.DuTruMuaDuocPhamTheoKhoaId,
                    x.DuocPhamId,
                    x.LaBHYT,
                    x.Nhom,
                    //x.NhomDuPhong,
                    x.SoLuongDuTru,
                    x.SoLuongDuKienSuDung,
                    x.SoLuongDuTruTruongKhoaDuyet,
                    x.NongDoHamLuong,
                    x.HoatChat,
                    x.DuongDung,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat
                })
                .Select(item => new DuTruMuaDuocPhamTaiKhoaChiTietVo()
                {
                    DuTruMuaDuocPhamTheoKhoaId = item.First().DuTruMuaDuocPhamTheoKhoaId,
                    DuocPhamId = item.First().DuocPhamId,
                    LaBHYT = item.First().LaBHYT,
                    TenDuocPham = item.First().TenDuocPham,
                    //NhomDuPhong = item.First().NhomDuPhong,
                    SoDangKy = item.First().SoDangKy,
                    NongDoHamLuong = item.First().NongDoHamLuong,
                    HoatChat = item.First().HoatChat,
                    DuongDung = item.First().DuongDung,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    NuocSanXuat = item.First().NuocSanXuat,
                    SoLuongDuTru = item.Sum(x => x.SoLuongDuTru),
                    SoLuongDuKienSuDung = item.Sum(x => x.SoLuongDuKienSuDung),
                    SoLuongDuTruTruongKhoaDuyet = item.Sum(x => x.SoLuongDuTruTruongKhoaDuyet),
                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion

        #region DS THDT đã xử lý chi tiết child

        public async Task<GridDataSource> GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            //var info = JsonConvert.DeserializeObject<ChiTietDaXuLyChild>(queryInfo.AdditionalSearchString);
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var duTruMuaDuocPhamTheoKhoaId = long.Parse(queryObj[0]);
            bool laBHYT = bool.Parse(queryObj[1]);
            var duocPhamId = long.Parse(queryObj[2]);


            var query = _duTruMuaDuocPhamChiTietRepository.TableNoTracking
                .Where(p => p.DuTruMuaDuocPhamTheoKhoaChiTiet.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamTheoKhoaId && p.LaDuocPhamBHYT == laBHYT && p.DuocPhamId == duocPhamId)
                .Select(s => new DuTruMuaDuocPhamTaiKhoaChiTietVo
                {
                    Id = s.Id,
                    LaBHYT = s.LaDuocPhamBHYT,
                    TenKho = s.DuTruMuaDuocPham.Kho.Ten,
                    LoaiThuoc = s.DuTruMuaDuocPham.NhomDuocPhamDuTru.GetDescription(),
                    KyDuTruDisplay = s.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + " - " + s.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienSuDung = s.SoLuongDuKienSuDung
                });
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var duTruMuaDuocPhamTheoKhoaId = long.Parse(queryObj[0]);
            bool laBHYT = bool.Parse(queryObj[1]);
            var duocPhamId = long.Parse(queryObj[2]);
            var query = _duTruMuaDuocPhamChiTietRepository.TableNoTracking
                .Where(p => p.DuTruMuaDuocPhamTheoKhoaChiTiet.DuTruMuaDuocPhamTheoKhoaId == duTruMuaDuocPhamTheoKhoaId && p.LaDuocPhamBHYT == laBHYT && p.DuocPhamId == duocPhamId)
                .Select(s => new DuTruMuaDuocPhamTaiKhoaChiTietVo
                {
                    Id = s.Id,
                    LaBHYT = s.LaDuocPhamBHYT,
                    TenKho = s.DuTruMuaDuocPham.Kho.Ten,
                    LoaiThuoc = s.NhomDieuTriDuPhong.GetDescription(),
                    KyDuTruDisplay = s.DuTruMuaDuocPham.TuNgay.ApplyFormatDate() + " - " + s.DuTruMuaDuocPham.DenNgay.ApplyFormatDate(),
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienSuDung = s.SoLuongDuKienSuDung
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region DS THDT Chi tiết phiếu mua dược phẩm dự trù
        public async Task<GridDataSource> GetDuTruMuaDuocPhamChiTietForGridAsyncChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var duTruMuaDuocPhamId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaDuocPhamChiTietRepository.TableNoTracking
                        .Where(p => p.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                        .Select(s => new DuTruMuaDuocPhamTaiKhoaChiTietVo
                        {
                            TenDuocPham = s.DuocPham.Ten,
                            LaBHYT = s.LaDuocPhamBHYT,
                            HoatChat = s.DuocPham.HoatChat,
                            NongDoHamLuong = s.DuocPham.HamLuong,
                            DonViTinh = s.DuocPham.DonViTinh.Ten,
                            SoDangKy = s.DuocPham.SoDangKy,
                            DuongDung = s.DuocPham.DuongDung.Ten,
                            HangSanXuat = s.DuocPham.NhaSanXuat,
                            NuocSanXuat = s.DuocPham.NuocSanXuat,
                            NhomDuPhong = s.NhomDieuTriDuPhong.GetDescription(),
                            SoLuongDuTru = s.SoLuongDuTru,
                            SoLuongDuKienSuDung = s.SoLuongDuKienSuDung,
                            SoLuongDuTruTruongKhoaDuyet = s.SoLuongDuTruTruongKhoaDuyet.GetValueOrDefault()
                        });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaDuocPhamChiTietForGridAsyncChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var duTruMuaDuocPhamId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaDuocPhamChiTietRepository.TableNoTracking
                        .Where(p => p.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                        .Select(s => new DuTruMuaDuocPhamTaiKhoaChiTietVo
                        {
                            TenDuocPham = s.DuocPham.Ten,
                            LaBHYT = s.LaDuocPhamBHYT,
                            HoatChat = s.DuocPham.HoatChat,
                            NongDoHamLuong = s.DuocPham.HamLuong,
                            DonViTinh = s.DuocPham.DonViTinh.Ten,
                            DuongDung = s.DuocPham.DuongDung.Ten,
                            HangSanXuat = s.DuocPham.NhaSanXuat,
                            NuocSanXuat = s.DuocPham.NuocSanXuat,
                            NhomDuPhong = s.NhomDieuTriDuPhong.GetDescription(),
                            SoLuongDuTru = s.SoLuongDuTru,
                            SoLuongDuKienSuDung = s.SoLuongDuKienSuDung,
                            SoLuongDuTruTruongKhoaDuyet = s.SoLuongDuTruTruongKhoaDuyet.GetValueOrDefault()
                        });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region Danh sách tư chối 

        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.SearchStringTuChoi))
                {
                    var searchTerms = queryString.SearchStringTuChoi.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.Kho.Ten,
                         g => g.NhanVienYeuCau.User.HoTen,
                         g => g.LyDoTruongKhoaTuChoi
                   );
                }
            }
            var data = query.Where(p => p.TruongKhoaDuyet == false)
                    .Select(o => new DuTruMuaDuocPhamTaiKhoaGridVo
                    {
                        Id = o.Id,
                        SoPhieu = o.SoPhieu,
                        LoaiNhom = o.NhomDuocPhamDuTru.GetDescription(),
                        TenKho = o.Kho.Ten,
                        TuNgay = o.TuNgay,
                        DenNgay = o.DenNgay,
                        NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                        NgayYeuCau = o.NgayYeuCau,
                        NgayTuChoi = o.NgayTruongKhoaDuyet,
                        NguoiTuChoi = o.TruongKhoa.User.HoTen,
                        LyDoTuChoi = o.LyDoTruongKhoaTuChoi
                    });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
                if (queryString.RangeFromDateTuChoi != null &&
                            (!string.IsNullOrEmpty(queryString.RangeFromDateTuChoi.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDateTuChoi.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDateTuChoi.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDateTuChoi.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    data = data.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDateTuChoi.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDateTuChoi.DenNgay) || p.NgayYeuCau <= denNgay));
                }

            }

            //Sort KyDuTru (DateTime - DateTime)
            var sortString = queryInfo.SortString.Contains("KyDuTru") ? queryInfo.SortString.Replace("KyDuTru", "TuNgay") : queryInfo.SortString;

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : data.CountAsync();
            var queryTask = data.OrderBy(sortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.SearchStringTuChoi))
                {
                    var searchTerms = queryString.SearchStringTuChoi.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.Kho.Ten,
                         g => g.NhanVienYeuCau.User.HoTen,
                         g => g.LyDoTruongKhoaTuChoi
                   );
                }
            }
            var data = query.Where(p => p.TruongKhoaDuyet == false)
                    .Select(o => new DuTruMuaDuocPhamTaiKhoaGridVo
                    {
                        Id = o.Id,
                        SoPhieu = o.SoPhieu,
                        LoaiNhom = o.NhomDuocPhamDuTru.GetDescription(),
                        TenKho = o.Kho.Ten,
                        TuNgay = o.TuNgay,
                        DenNgay = o.DenNgay,
                        NguoiYeuCau = o.NhanVienYeuCau.User.HoTen,
                        NgayYeuCau = o.NgayYeuCau,
                        NgayTuChoi = o.NgayTruongKhoaDuyet,
                        NguoiTuChoi = o.TruongKhoa.User.HoTen,
                        LyDoTuChoi = o.LyDoTruongKhoaTuChoi
                    });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
                if (queryString.RangeFromDateTuChoi != null &&
                            (!string.IsNullOrEmpty(queryString.RangeFromDateTuChoi.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDateTuChoi.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDateTuChoi.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDateTuChoi.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    data = data.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDateTuChoi.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDateTuChoi.DenNgay) || p.NgayYeuCau <= denNgay));
                }

            }

            var countTask = data.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion

        #region  DS THDT Từ Chối Chi tiết

        public async Task<GridDataSource> GetDataDuTruMuaDuocPhamTuChoiTaiKhoaChildForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var duTruMuaDuocPhamId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaDuocPhamChiTietRepository.TableNoTracking
                        .Where(p => p.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                        .Select(s => new DuTruMuaDuocPhamTaiKhoaChiTietVo
                        {
                            Id = s.Id,
                            DuocPhamId = s.DuocPhamId,
                            TenDuocPham = s.DuocPham.Ten,
                            LaBHYT = s.LaDuocPhamBHYT,
                            HoatChat = s.DuocPham.HoatChat,
                            DuongDung = s.DuocPham.DuongDung.Ten,
                            NongDoHamLuong = s.DuocPham.HamLuong,
                            DonViTinh = s.DuocPham.DonViTinh.Ten,
                            SoDangKy = s.DuocPham.SoDangKy,
                            HangSanXuat = s.DuocPham.NhaSanXuat,
                            NuocSanXuat = s.DuocPham.NuocSanXuat,
                            NhomDuPhong = s.NhomDieuTriDuPhong.GetDescription(),
                            SoLuongDuTru = s.SoLuongDuTru,
                            SoLuongDuKienSuDung = s.SoLuongDuKienSuDung,
                            SoLuongDuTruTruongKhoaDuyet = s.SoLuongDuTruTruongKhoaDuyet.GetValueOrDefault()
                        });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaDuocPhamTuChoiTaiKhoaChildForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var duTruMuaDuocPhamId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaDuocPhamChiTietRepository.TableNoTracking
                        .Where(p => p.DuTruMuaDuocPhamId == duTruMuaDuocPhamId)
                        .Select(s => new DuTruMuaDuocPhamTaiKhoaChiTietVo
                        {
                            Id = s.Id,
                            DuocPhamId = s.DuocPhamId,
                            TenDuocPham = s.DuocPham.Ten,
                            LaBHYT = s.LaDuocPhamBHYT,
                            HoatChat = s.DuocPham.HoatChat,
                            DuongDung = s.DuocPham.DuongDung.Ten,
                            NongDoHamLuong = s.DuocPham.HamLuong,
                            DonViTinh = s.DuocPham.DonViTinh.Ten,
                            SoDangKy = s.DuocPham.SoDangKy,
                            HangSanXuat = s.DuocPham.NhaSanXuat,
                            NuocSanXuat = s.DuocPham.NuocSanXuat,
                            NhomDuPhong = s.NhomDieuTriDuPhong.GetDescription(),
                            SoLuongDuTru = s.SoLuongDuTru,
                            SoLuongDuKienSuDung = s.SoLuongDuKienSuDung,
                            SoLuongDuTruTruongKhoaDuyet = s.SoLuongDuTruTruongKhoaDuyet.GetValueOrDefault()
                        });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion
    }
}
