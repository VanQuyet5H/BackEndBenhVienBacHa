using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaKSNKTaiKhoa;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauMuaVatTu;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using static Camino.Core.Domain.Enums;


namespace Camino.Services.YeuCauMuaDuTruKiemSoatNhiemKhuan
{
    public partial class YeuCauMuaDuTruKiemSoatNhiemKhuanService
    {
        #region Danh sách hàm đang chờ xử lý

        public async Task<GridDataSource> GetDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long duTruMuaVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;

            var queryable = _duTruMuaVatTuChiTietRepository
                    .TableNoTracking.Where(cc => cc.DuTruMuaVatTuId == duTruMuaVatTuId)
                    .Include(g => g.VatTu).ThenInclude(c => c.DonViTinh)
                    .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo()
                    {
                        Id = item.Id,
                        VatTuId = item.VatTuId,
                        VatTu = item.VatTu.Ten,
                        TenVatTu = item.VatTu.Ten,
                        LaBHYT = item.LaVatTuBHYT,
                        QuyCach = item.VatTu.QuyCach,
                        HangSanXuat = item.VatTu.NhaSanXuat,
                        DonViTinh = item.VatTu.DonViTinh,
                        NuocSanXuat = item.VatTu.NuocSanXuat,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet != null ? (int)item.SoLuongDuTruTruongKhoaDuyet : 0,
                    });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
            var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            long duTruMuaVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var queryable = _duTruMuaVatTuChiTietRepository
                   .TableNoTracking.Where(cc => cc.DuTruMuaVatTuId == duTruMuaVatTuId)
                   .Include(g => g.VatTu).ThenInclude(c => c.DonViTinh)
                   .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo()
                   {
                       Id = item.Id,
                       VatTuId = item.VatTuId,
                       VatTu = item.VatTu.Ten,
                       TenVatTu = item.VatTu.Ten,
                       LaBHYT = item.LaVatTuBHYT,
                       QuyCach = item.VatTu.QuyCach,
                       HangSanXuat = item.VatTu.NhaSanXuat,
                       DonViTinh = item.VatTu.DonViTinh,
                       NuocSanXuat = item.VatTu.NuocSanXuat,
                       SoLuongDuTru = item.SoLuongDuTru,
                       SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                       SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet != null ? (int)item.SoLuongDuTruTruongKhoaDuyet : 0,
                   });

            var countTask = queryable.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion

        public async Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new DuTruMuaKSNKTaiKhoaSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaKSNKTaiKhoaSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = GetDataYeuCauTraVatTu(null, queryInfo, queryObject);
            var queryTuDaDuyet = GetDataYeuCauTraVatTu(true, queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0)
                .Select(s => new DuTruMuaKSNKTaiKhoaGridVo())
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

        public async Task<GridDataSource> GetTotalDuTruMuaKSNKTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new DuTruMuaKSNKTaiKhoaSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DuTruMuaKSNKTaiKhoaSearch>(queryInfo.AdditionalSearchString);
            }

            var queryDangChoDuyet = GetDataYeuCauTraVatTu(null, queryInfo, queryObject);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaKSNKTaiKhoaGridVo>(queryInfo.AdditionalSearchString);

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
            var queryTuDaDuyet = GetDataYeuCauTraVatTu(true, queryInfo, queryObject);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaKSNKTaiKhoaGridVo>(queryInfo.AdditionalSearchString);

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
                .Select(s => new DuTruMuaKSNKTaiKhoaGridVo())
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



        private IQueryable<DuTruMuaKSNKTaiKhoaGridVo> GetDataYeuCauTraVatTu(bool? duocDuyet, QueryInfo queryInfo, DuTruMuaKSNKTaiKhoaSearch queryObject)
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
                                                                                 && p.Kho.LoaiKho ==
                                                                                 Enums.EnumLoaiKhoDuocPham.KhoLe &&
                                                                                 p.DuTruMuaVatTuTheoKhoaId == null &&
                                                                                 nhanVienThuocKhos.Contains(p.KhoId))
                .Select(o => new DuTruMuaKSNKTaiKhoaGridVo
                {
                    Id = o.Id,
                    SoPhieu = o.SoPhieu,
                    TrangThai = o.TruongKhoaDuyet == null ? EnumTrangThaiLoaiDuTru.ChoDuyet.GetDescription() : EnumTrangThaiLoaiDuTru.ChoGoi.GetDescription() + "." + o.TuNgay.ToString("dd/MM/yyyy") + " - " + o.DenNgay.ToString("dd/MM/yyyy"),
                    TenKho = o.Kho.Ten,
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

            if (queryObject != null)
            {
                if (queryObject.RangeYeuCau != null && queryObject.RangeYeuCau.startDate != null)
                {
                    var tuNgay = queryObject.RangeYeuCau.startDate.GetValueOrDefault().Date;

                    data = data.Where(p => tuNgay <= p.NgayYeuCau.Date);
                }
                if (queryObject.RangeYeuCau != null && queryObject.RangeYeuCau.endDate != null)
                {
                    var denNgay = queryObject.RangeYeuCau.endDate.GetValueOrDefault().Date;
                    data = data.Where(p => denNgay >= p.NgayYeuCau.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    data = data.Where(p => tuNgay <= p.NgayTruongKhoaDuyet.Value.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    data = data.Where(p => denNgay >= p.NgayTruongKhoaDuyet.Value.Date);
                }
            }

            return data;
        }

        public async Task<bool> TuChoiDuyetTaiKhoa(ThongTinLyDoHuyDuyetKSNKTaiKhoa thongTinLyDoHuyNhapKhoDuocPham)
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

        public async Task<bool> DuyetTaiKhoa(DuyetDuTruMuaKSNKViewModel model)
        {
            var duTruMuaVatTuEntity = await BaseRepository.Table
                            .Where(cc => cc.Id == model.DuTruMuaVatTuId)
                            .Include(cc => cc.DuTruMuaVatTuChiTiets)
                            .FirstOrDefaultAsync();

            duTruMuaVatTuEntity.TruongKhoaDuyet = true;
            duTruMuaVatTuEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
            duTruMuaVatTuEntity.NgayTruongKhoaDuyet = DateTime.Now;

            var duTruMuaDuocPhamChiTiets = BaseRepository.Table.Where(cc => cc.Id == model.DuTruMuaVatTuId).SelectMany(cc => cc.DuTruMuaVatTuChiTiets);
            foreach (var item in duTruMuaDuocPhamChiTiets)
            {
                if (model.DuTruMuaVatTuTaiKhoaChiTietVos != null && model.DuTruMuaVatTuTaiKhoaChiTietVos.Any())
                {
                    foreach (var itemModel in model.DuTruMuaVatTuTaiKhoaChiTietVos)
                    {
                        if (item.Id == itemModel.Id)
                        {
                            item.SoLuongDuTruTruongKhoaDuyet = itemModel.SoLuongDuTruTruongKhoaDuyet;
                        }
                    }
                }
                else
                {
                    item.SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTru;
                }
            }
            BaseRepository.Update(duTruMuaVatTuEntity);
            return true;
        }

        public async Task<bool> HuyDuyetTaiKhoa(long id)
        {
            var duTruMuaDuocPhamEntity = await BaseRepository.Table.Where(cc => cc.Id == id).FirstOrDefaultAsync();

            duTruMuaDuocPhamEntity.TruongKhoaDuyet = null;
            duTruMuaDuocPhamEntity.TruongKhoaId = null;
            duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = null;

            var duTruMuaDuocPhamChiTiets = BaseRepository.Table.Where(cc => cc.Id == id).SelectMany(cc => cc.DuTruMuaVatTuChiTiets);
            foreach (var item in duTruMuaDuocPhamChiTiets)
            {
                item.SoLuongDuTruTruongKhoaDuyet = null;
            }
            BaseRepository.Update(duTruMuaDuocPhamEntity);
            return true;
        }

        public GetThongTinGoiKSNKTaiKhoa GetThongTinGoiTaiKhoa(long phongBenhVienId)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var hoTenNguoiYeuCau = _userRepository.GetById(currentUserId).HoTen;

            var khoaPhong = _phongBenhVienRepository.TableNoTracking.FirstOrDefault(cc => cc.Id == phongBenhVienId);

            if (khoaPhong != null)
            {
                var thongTinDuTru = new GetThongTinGoiKSNKTaiKhoa
                {
                    KhoaPhongId = khoaPhong.KhoaPhongId,
                    TenKhoaPhong = khoaPhong.Ten,
                    NguoiyeuCauId = currentUserId,
                    TenNhanVienYeuCau = hoTenNguoiYeuCau,
                    NgayYeuCau = DateTime.Now
                };
                return thongTinDuTru;
            }
            return new GetThongTinGoiKSNKTaiKhoa();
        }

        #region Danh sách con dự trù chi tiết       

        public async Task<GridDataSource> GetDuTruMuaKSNKTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long duTruMuaDuocPhamId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;

            var queryable = _duTruMuaVatTuChiTietRepository
                    .TableNoTracking.Where(cc => cc.DuTruMuaVatTuId == duTruMuaDuocPhamId)
                    .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo
                    {
                        Id = item.Id,
                        VatTuId = item.VatTuId,
                        VatTu = item.VatTu.Ten,
                        LaBHYT = item.LaVatTuBHYT,
                        QuyCach = item.VatTu.QuyCach,
                        HangSanXuat = item.VatTu.NhaSanXuat,
                        DonViTinh = item.VatTu.DonViTinh,
                        NuocSanXuat = item.VatTu.NuocSanXuat,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet == null ? item.SoLuongDuTru : (int)item.SoLuongDuTruTruongKhoaDuyet,
                        KhoDuTruTon = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.VatTuBenhVienId == item.VatTuId && x.NhapKhoVatTu.KhoId == item.DuTruMuaVatTu.KhoId && x.LaVatTuBHYT == item.LaVatTuBHYT && x.NhapKhoVatTu.DaHet != true
                                                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                    });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
            var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            foreach (var item in queryTask.Result)
            {
                var thongTinHDT = ThongTinDuTruMuaVatTuHopDongThau(item);

                item.KhoTongTon = thongTinHDT.SLChuaNhapVeHDT ?? 0;
                item.ThongTinChiTietTonKhoTongs = thongTinHDT.ThongTinChiTietTonKhoTongs;

                item.HDChuaNhap = thongTinHDT.SLTonKhoTong ?? 0;
                item.ThongTinChiTietTonHDTs = thongTinHDT.ThongTinChiTietTonHDTs;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            long duTruMuaVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var queryable = _duTruMuaVatTuChiTietRepository
                    .TableNoTracking.Where(cc => cc.DuTruMuaVatTuId == duTruMuaVatTuId)
                    .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo
                    {
                        Id = item.Id,
                        VatTuId = item.VatTuId,
                        VatTu = item.VatTu.Ten,
                        LaBHYT = item.LaVatTuBHYT,
                        QuyCach = item.VatTu.QuyCach,
                        HangSanXuat = item.VatTu.NhaSanXuat,
                        DonViTinh = item.VatTu.DonViTinh,
                        NuocSanXuat = item.VatTu.NuocSanXuat,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet ?? item.SoLuongDuTru
                    });

            var countTask = queryable.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public ThongTinDuTruMuaVatTu ThongTinDuTruMuaVatTuHopDongThau(DuTruMuaKSNKTaiKhoaChiTietVo thongTinChiTietVatTuTonKho)
        {
            var thongTin = new ThongTinDuTruMuaVatTu();
            var vatTuBenhVien = _vatTuBenhVienRepository.TableNoTracking
                        .Any(p => p.Id == thongTinChiTietVatTuTonKho.VatTuId && p.HieuLuc == true);
            var laVatTuBHYT = thongTinChiTietVatTuTonKho.LaBHYT;
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
                                SLTonKhoTong = s.NhapKhoVatTuChiTiets
                                            .Where(nkct => nkct.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && nkct.LaVatTuBHYT == laVatTuBHYT)
                                            .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                SLChuaNhapVeHDT = slTonHDT,
                                ThongTinChiTietTonHDTs = _hopDongThauVatTuChiTietRepository.TableNoTracking
                                                           .Where(o => hopDongThauVatTuIds.Contains(o.HopDongThauVatTuId)
                                                                    && o.VatTuId == thongTinChiTietVatTuTonKho.VatTuId
                                                                    && o.SoLuong - o.SoLuongDaCap > 0
                                                                    && o.HopDongThauVatTu.NhapKhoVatTuChiTiets.Any(p => p.LaVatTuBHYT == laVatTuBHYT)
                                                                    )
                                                           .Select(item => new Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho
                                                           {
                                                               Ten = item.HopDongThauVatTu.SoHopDong,
                                                               SLTon = (item.SoLuong - item.SoLuongDaCap)
                                                           })
                                                           .GroupBy(x => new { x.Ten })
                                                           .Select(o => new Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho
                                                           {
                                                               Ten = o.First().Ten,
                                                               SLTon = o.Sum(x => x.SLTon)
                                                           }).OrderBy(x => x.Ten).Distinct().ToList().ToList(),

                                ThongTinChiTietTonKhoTongs = s.NhapKhoVatTuChiTiets
                                                            .Where(p => p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                         && p.LaVatTuBHYT == laVatTuBHYT
                                                                         && p.VatTuBenhVienId == thongTinChiTietVatTuTonKho.VatTuId
                                                                         && p.SoLuongNhap - p.SoLuongDaXuat > 0
                                                                         )
                                                               .Select(item => new Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho
                                                               {
                                                                   Ten = item.NhapKhoVatTu.Kho.Ten,
                                                                   SLTon = (item.SoLuongNhap - item.SoLuongDaXuat)
                                                               }).GroupBy(x => new { x.Ten })
                                                               .Select(o => new Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho
                                                               {
                                                                   Ten = o.First().Ten,
                                                                   SLTon = o.Sum(x => x.SLTon),
                                                               }).OrderBy(x => x.Ten).Distinct().ToList(),

                            }).FirstOrDefault();
            }
            else
            {

                thongTin = _vatTuRepository
                              .TableNoTracking
                              .Where(p => p.Id == thongTinChiTietVatTuTonKho.VatTuId)
                              .Select(s => new ThongTinDuTruMuaVatTu
                              {
                                  SLTonKhoTong = 0,
                                  SLChuaNhapVeHDT = slTonHDT,
                                  ThongTinChiTietTonKhoTongs = new List<Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho>(),
                                  ThongTinChiTietTonHDTs = _hopDongThauVatTuChiTietRepository.TableNoTracking
                                                             .Where(o => hopDongThauVatTuIds.Contains(o.HopDongThauVatTuId) && o.SoLuong - o.SoLuongDaCap > 0
                                                             && o.VatTuId == thongTinChiTietVatTuTonKho.VatTuId)
                                                             .Select(item => new Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho
                                                             {
                                                                 Ten = item.HopDongThauVatTu.SoHopDong,
                                                                 SLTon = (item.SoLuong - item.SoLuongDaCap)
                                                             })
                                                             .GroupBy(x => new { x.Ten })
                                                             .Select(o => new Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho
                                                             {
                                                                 Ten = o.First().Ten,
                                                                 SLTon = o.Sum(x => x.SLTon)
                                                             }).OrderBy(x => x.Ten).Distinct().ToList().ToList()
                              }).FirstOrDefault();
            }
            return thongTin;

        }

        #endregion

        #region Danh sách dự trù mua tại khoa 

        public GridDataSource GetDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var nhanVienThuocKhos = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == _userAgentHelper.GetCurrentUserId()).Select(cc => cc.KhoId).ToList();
            long kyDuTruMuaDuocPhamVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var duTruVatTuIds = BaseRepository.TableNoTracking.Where(cc => cc.KyDuTruMuaDuocPhamVatTuId == kyDuTruMuaDuocPhamVatTuId).Where(cc => cc.TruongKhoaDuyet == true
              && cc.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && nhanVienThuocKhos.Contains(cc.KhoId) && cc.DuTruMuaVatTuTheoKhoaId == null).Select(cc => cc.Id);

            var queryable = _duTruMuaVatTuChiTietRepository
                    .TableNoTracking.Where(cc => duTruVatTuIds.Contains(cc.DuTruMuaVatTuId))
                    .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo
                    {
                        Id = item.Id,
                        VatTuId = item.VatTuId,
                        VatTu = item.VatTu.Ten,
                        LaBHYT = item.LaVatTuBHYT,
                        QuyCach = item.VatTu.QuyCach,
                        HangSanXuat = item.VatTu.NhaSanXuat,
                        DonViTinh = item.VatTu.DonViTinh,
                        NuocSanXuat = item.VatTu.NuocSanXuat,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = (int)item.SoLuongDuTruTruongKhoaDuyet,
                        DuTruMuaVatTuId = item.DuTruMuaVatTuId,
                        DuTruMuaVatTuChiTietId = item.Id
                    }).GroupBy(x => new
                    {
                        x.VatTuId,
                        x.Nhom
                    }).Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo()
                    {
                        Id = item.First().Id,
                        VatTuId = item.First().VatTuId,
                        VatTu = item.First().VatTu,
                        LaBHYT = item.First().LaBHYT,
                        QuyCach = item.First().QuyCach,
                        HangSanXuat = item.First().HangSanXuat,
                        DonViTinh = item.First().DonViTinh,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongDuTru = item.Sum(cc => cc.SoLuongDuTru),
                        SoLuongDuKienSuDung = item.Sum(cc => cc.SoLuongDuKienSuDung),
                        SoLuongDuTruTruongKhoaDuyet = item.Sum(cc => cc.SoLuongDuTruTruongKhoaDuyet),
                        DuTruMuaVatTuIds = item.Select(c => c.DuTruMuaVatTuId).ToList(),
                        DuTruMuaVatTuChiTietIds = item.Select(c => c.DuTruMuaVatTuChiTietId).ToList()
                    });
            //.Skip(queryInfo.Skip).Take(queryInfo.Take)
            var data = queryable.ToArray();
            return new GridDataSource { Data = data, TotalRowCount = 0 };
        }

        public GridDataSource GetTotalDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            var nhanVienThuocKhos = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == _userAgentHelper.GetCurrentUserId()).Select(cc => cc.KhoId).ToList();
            long kyDuTruMuaDuocPhamVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var duTruVatTuIds = BaseRepository.TableNoTracking.Where(cc => cc.KyDuTruMuaDuocPhamVatTuId == kyDuTruMuaDuocPhamVatTuId).Where(cc => cc.TruongKhoaDuyet == true
              && cc.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && nhanVienThuocKhos.Contains(cc.KhoId) && cc.DuTruMuaVatTuTheoKhoaId == null).Select(cc => cc.Id);

            var queryable = _duTruMuaVatTuChiTietRepository
                    .TableNoTracking.Where(cc => duTruVatTuIds.Contains(cc.DuTruMuaVatTuId))
                    .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo
                    {
                        Id = item.Id,
                        VatTuId = item.VatTuId,
                        VatTu = item.VatTu.Ten,
                        LaBHYT = item.LaVatTuBHYT,
                        QuyCach = item.VatTu.QuyCach,
                        HangSanXuat = item.VatTu.NhaSanXuat,
                        DonViTinh = item.VatTu.DonViTinh,
                        NuocSanXuat = item.VatTu.NuocSanXuat,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = (int)item.SoLuongDuTruTruongKhoaDuyet,
                        DuTruMuaVatTuId = item.DuTruMuaVatTuId,
                        DuTruMuaVatTuChiTietId = item.Id
                    }).GroupBy(x => new
                    {
                        x.VatTuId,
                        x.Nhom
                    }).Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo
                    {
                        Id = item.First().Id,
                        VatTuId = item.First().VatTuId,
                        VatTu = item.First().VatTu,
                        LaBHYT = item.First().LaBHYT,
                        QuyCach = item.First().QuyCach,
                        HangSanXuat = item.First().HangSanXuat,
                        DonViTinh = item.First().DonViTinh,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongDuTru = item.Sum(cc => cc.SoLuongDuTru),
                        SoLuongDuKienSuDung = item.Sum(cc => cc.SoLuongDuKienSuDung),
                        SoLuongDuTruTruongKhoaDuyet = item.Sum(cc => cc.SoLuongDuTruTruongKhoaDuyet),
                        DuTruMuaVatTuIds = item.Select(c => c.DuTruMuaVatTuId).ToList(),
                        DuTruMuaVatTuChiTietIds = item.Select(c => c.DuTruMuaVatTuChiTietId).ToList()
                    });
            return new GridDataSource { TotalRowCount = queryable.Count() };
        }

        public GridDataSource GetDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryString = queryInfo.AdditionalSearchString.Split('-');

            long kyDuTruMuaDuocPhamVatTuId = long.Parse(queryString[0]);
            bool laBhyt = bool.Parse(queryString[1]);
            long vatTuId = long.Parse(queryString[2]);

            var nhanVienThuocKhos = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == _userAgentHelper.GetCurrentUserId()).Select(cc => cc.KhoId).ToList();
            var duTruVatTuIds = BaseRepository.TableNoTracking.Where(cc => cc.KyDuTruMuaDuocPhamVatTuId == kyDuTruMuaDuocPhamVatTuId && cc.TruongKhoaDuyet == true
              && cc.DuTruMuaVatTuTheoKhoaId == null
              && cc.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && nhanVienThuocKhos.Contains(cc.KhoId)).Select(cc => cc.Id);

            var queryable = _duTruMuaVatTuChiTietRepository
                    .TableNoTracking.Where(cc => duTruVatTuIds.Contains(cc.DuTruMuaVatTuId) && cc.LaVatTuBHYT == laBhyt && cc.VatTuId == vatTuId)
                    .Select(item => new ThongTinDuTruMuaKSNKChiTietTaiKhoa
                    {
                        Id = item.Id,
                        TuNgay = item.DuTruMuaVatTu.TuNgay,
                        TenKho = item.DuTruMuaVatTu.Kho.Ten,
                        DenNgay = item.DuTruMuaVatTu.DenNgay,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = (int)item.SoLuongDuTruTruongKhoaDuyet
                    });

            var data = queryable.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = data, TotalRowCount = 0 };
        }

        public GridDataSource GetTotalDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var queryString = queryInfo.AdditionalSearchString.Split('-');

            long kyDuTruMuaDuocPhamVatTuId = long.Parse(queryString[0]);
            bool laBhyt = bool.Parse(queryString[1]);
            long vatTuId = long.Parse(queryString[2]);

            var nhanVienThuocKhos = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == _userAgentHelper.GetCurrentUserId()).Select(cc => cc.KhoId).ToList();
            var duTruVatTuIds = BaseRepository.TableNoTracking.Where(cc => cc.KyDuTruMuaDuocPhamVatTuId == kyDuTruMuaDuocPhamVatTuId && cc.TruongKhoaDuyet == true
                                         && cc.DuTruMuaVatTuTheoKhoaId == null
                                         && cc.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && nhanVienThuocKhos.Contains(cc.KhoId)).Select(cc => cc.Id);

            var queryable = _duTruMuaVatTuChiTietRepository
                    .TableNoTracking.Where(cc => duTruVatTuIds.Contains(cc.DuTruMuaVatTuId) && cc.LaVatTuBHYT == laBhyt && cc.VatTuId == vatTuId)
                    .Select(item => new ThongTinDuTruMuaKSNKChiTietTaiKhoa
                    {
                        Id = item.Id,
                        TuNgay = item.DuTruMuaVatTu.TuNgay,
                        TenKho = item.DuTruMuaVatTu.Kho.Ten,
                        DenNgay = item.DuTruMuaVatTu.DenNgay,
                        SoLuongDuTru = item.SoLuongDuTru,
                        SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                        SoLuongDuTruTruongKhoaDuyet = (int)item.SoLuongDuTruTruongKhoaDuyet
                    });

            var data = queryable.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { TotalRowCount = data.Count() };
        }

        public long GoiThongTinTaiKhoa(DuyetDuTruMuaKSNKViewModel model)
        {
            var kyDuTruMuaDuocPhamVatTu = _kyDuTruMuaDuocPhamVatTuRepository.TableNoTracking.FirstOrDefault(cc => cc.Id == model.DuTruMuaVatTuId);
            var soPhieu = GetSoPhieuDuTruTheKhoa();
            if (kyDuTruMuaDuocPhamVatTu != null)
            {
                var duTruMuaVatTuTheoKhoaModel = new DuTruMuaVatTuTheoKhoa
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

                if (model.DuTruMuaVatTuTaiKhoaChiTietVos.Any())
                {
                    foreach (var duTruMuaDuocPhamTaiKhoaChiTiet in model.DuTruMuaVatTuTaiKhoaChiTietVos)
                    {
                        var duTruMuaDuocPhamTheoKhoaChiTiet = new DuTruMuaVatTuTheoKhoaChiTiet
                        {
                            LaVatTuBHYT = duTruMuaDuocPhamTaiKhoaChiTiet.LaBHYT,
                            VatTuId = duTruMuaDuocPhamTaiKhoaChiTiet.VatTuId,
                            SoLuongDuTru = (int)duTruMuaDuocPhamTaiKhoaChiTiet.SoLuongDuTru,
                            SoLuongDuKienSuDung = (int)duTruMuaDuocPhamTaiKhoaChiTiet.SoLuongDuKienSuDung,
                            SoLuongDuTruTruongKhoaDuyet = duTruMuaDuocPhamTaiKhoaChiTiet.SoLuongDuTruTruongKhoaDuyet,
                        };
                        duTruMuaVatTuTheoKhoaModel.DuTruMuaVatTuTheoKhoaChiTiets.Add(duTruMuaDuocPhamTheoKhoaChiTiet);
                    }
                }

                _duTruMuaVatTuTheoKhoaRepository.Add(duTruMuaVatTuTheoKhoaModel);

                //update lai dự trù mua và dự trù mua chi tiết             
                var newListInt = new List<long>();
                foreach (var item in model.DuTruMuaVatTuTaiKhoaChiTietVos)
                {
                    foreach (var duTruMuaDuocPhamId in item.DuTruMuaVatTuIds)
                    {
                        newListInt.Add(duTruMuaDuocPhamId);
                    }

                }
                var listduTruMuaVatTuId = newListInt.Select(c => c).Distinct().ToList();
                var duTruMuaVatTus = BaseRepository.TableNoTracking.Where(cc => listduTruMuaVatTuId.Contains(cc.Id))
                                                   .Include(c => c.DuTruMuaVatTuChiTiets).ToList();

                //var duTruMuaVatTuChiTietIds = model.DuTruMuaVatTuTaiKhoaChiTietVos.FirstOrDefault()?.DuTruMuaVatTuChiTietIds;
                //var duTruMuaVatTuChiTiets = _duTruMuaVatTuChiTietRepository.TableNoTracking.Where(cc => duTruMuaVatTuChiTietIds.Contains(cc.Id)).ToList();

                if (duTruMuaVatTus.Any())
                {
                    foreach (var duTruMuaVatTu in duTruMuaVatTus)
                    {
                        duTruMuaVatTu.DuTruMuaVatTuTheoKhoaId = duTruMuaVatTuTheoKhoaModel.Id;
                        BaseRepository.Update(duTruMuaVatTu);

                        if (duTruMuaVatTu.DuTruMuaVatTuChiTiets.Any())
                        {
                            foreach (var duTruMuaVatTuChiTiet in duTruMuaVatTu.DuTruMuaVatTuChiTiets)
                            {
                                foreach (var listDuTruMuaVatTuTheoKhoaChiTiet in duTruMuaVatTuTheoKhoaModel.DuTruMuaVatTuTheoKhoaChiTiets)
                                {
                                    if (listDuTruMuaVatTuTheoKhoaChiTiet.VatTuId == duTruMuaVatTuChiTiet.VatTuId && listDuTruMuaVatTuTheoKhoaChiTiet.LaVatTuBHYT == duTruMuaVatTuChiTiet.LaVatTuBHYT)
                                    {
                                        duTruMuaVatTuChiTiet.DuTruMuaVatTuTheoKhoaChiTietId = listDuTruMuaVatTuTheoKhoaChiTiet.Id;
                                        _duTruMuaVatTuChiTietRepository.Update(duTruMuaVatTuChiTiet);
                                    }
                                }

                            }
                        }
                    }
                }

                return duTruMuaVatTuTheoKhoaModel.Id;
            }

            return 0;
        }

        public string GetSoPhieuDuTruTheKhoa()
        {
            string result;
            var soPhieu = "THDT";
            var lastYearMonthCurrent = DateTime.Now.ToString("yyMM");
            var lastSoPhieuStr = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Select(p => p.SoPhieu).LastOrDefault();

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

        public string InPhieuDuTruMuaTaiKhoa(PhieuInDuTruMuaKSNKTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var contentThuoc = string.Empty;
            var contentHoaChat = string.Empty;
            var hearder = string.Empty;
            var templatePhieuInTongHopDuTruDuocPhamTaiKhoa = new Template();
            var duTruMuaDuocPhams = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking.Where(c => c.Id == phieuInDuTruMuaTaiKhoa.DuTruMuaVatTuTheoKhoaId)
                                                                                     .Include(cc => cc.KhoaPhong)
                                                                                     .Include(cc => cc.NhanVienYeuCau).ThenInclude(c => c.User)
                                                                                     .Include(cc => cc.DuTruMuaVatTuTheoKhoaChiTiets)
                                                                                     .ThenInclude(cc => cc.VatTu);

            //if (phieuInDuTruMuaTaiKhoa.Header)
            //{
            //    hearder = "<p style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                           "<th>PHIẾU TỔNG HỢP DỰ TRÙ VẬT TƯ</th>" +
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

            var kyDuTruId = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
                                            .Where(p => p.Id == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId)
                                            .SelectMany(d => d.DuTruMuaVatTus)
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
            templatePhieuInTongHopDuTruDuocPhamTaiKhoa = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuInTongHopDuTruKSNKTaiKhoa")).First();
         
            var duTruMuaDuocPhamChiTiets = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking
                                            .Where(p => p.DuTruMuaVatTuTheoKhoaId == phieuInDuTruMuaTaiKhoa.DuTruMuaVatTuTheoKhoaId)
                                            .Select(s => new Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.PhieuMuaDuTruDuocPhamChiTietData
                                            {
                                                MaHang = s.VatTu.VatTuBenhVien.Ma, // todo: cần confirm hỏi lại
                                                Ten = s.VatTu.Ten,                                             
                                                DonVi = s.VatTu.DonViTinh,
                                                SoLuong = s.SoLuongDuTruTruongKhoaDuyet,
                                                GhiChu = "",
                                                LaDuocPhamBHYT = s.LaVatTuBHYT
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
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.MaHang + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ten + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonVi + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SoLuong + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                    + "</td></tr>";
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
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.MaHang + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ten + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonVi + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SoLuong + "</td>"
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                    + "</td></tr>";
                    STT++;
                    groupKhongBHYT = string.Empty;
                }
            }

            var data = new
            {
                Header = hearder,
                MaPhieuMuaDuTruVatTu = "BMBH-KD02",
                DuTruMuaVatTuChiTiet = duTruMuaDuocPhamChiTiet,
                KhoaPhong = duTruMuaDuocPhams.FirstOrDefault().KhoaPhong?.Ten,
                NhanVienLap = duTruMuaDuocPhams.FirstOrDefault().NhanVienYeuCau.User.HoTen,

                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString(),
                TuNgay = tuNgay,
                DenNgay =denNgay
            };

            contentThuoc = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInTongHopDuTruDuocPhamTaiKhoa.Body, data);
            return contentThuoc;
        }

        public string InPhieuDuTruMuaTaiKhoaDuocPham(PhieuInDuTruMuaKSNKTaiKhoa phieuInDuTruMuaTaiKhoa)
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
                                            .Where(p => p.DuTruMuaDuocPhamKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId)
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

            var duTruMuaDuocPhamChiTiets = _duTruMuaDuocPhamTheoKhoaChiTietRepository.TableNoTracking
                                            .Where(p => p.DuTruMuaDuocPhamKhoDuocChiTiet.DuTruMuaDuocPhamKhoDuocId == phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId)
                                            .Select(s => new Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.PhieuMuaDuTruDuocPhamChiTietData
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
                TuNgay =tuNgay,
                DenNgay = denNgay
            };

            contentThuoc = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInTongHopDuTruDuocPhamTaiKhoa.Body, data);

            return contentThuoc;

        }

        #endregion

        #region Thông tin dự trù vật tư tại khoa

        public DuTruMuaKSNKViewModel GetThongTinDuTruKSNKTaiKhoa(long duTruVatTuId)
        {
            var duTruMuaVatTu = BaseRepository.TableNoTracking
                                                 .Where(cc => cc.Id == duTruVatTuId)
                                                  .Select(c => new DuTruMuaKSNKViewModel
                                                  {
                                                      TinhTrang = c.TruongKhoaDuyet == true ? 0 : 1,
                                                      GhiChu = c.GhiChu,
                                                      TuNgay = c.TuNgay,
                                                      DenNgay = c.DenNgay,
                                                      KhoNhapId = c.KhoId,
                                                      NgayYeuCau = c.NgayYeuCau,
                                                      SoPhieu = c.SoPhieu,
                                                      TenNhanVienYeuCau = c.NhanVienYeuCau.User.HoTen,
                                                      TenKho = c.Kho.Ten,
                                                      LyDoTruongKhoaTuChoi = c.DuTruMuaVatTuTheoKhoa != null && c.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == false ? c.DuTruMuaVatTuTheoKhoa.LyDoKhoDuocTuChoi : c.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null
                                                     && c.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false ? c.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.LyDoGiamDocTuChoi
                                                     : c.LyDoTruongKhoaTuChoi
                                                  }).FirstOrDefault();

            return duTruMuaVatTu;
        }

        public DuTruMuaKSNKViewModel GetThongTinDuTruKSNKTaiKhoaDaXuLy(long duTruVatTuId)
        {
            var duTruMuaVatTu = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking
                                                 .Where(cc => cc.Id == duTruVatTuId)
                                                 .Select(s => new DuTruMuaKSNKViewModel
                                                 {
                                                     SoPhieu = s.SoPhieu,
                                                     KhoNhapId = s.DuTruMuaVatTus.First().KhoId,
                                                     TenKho = s.DuTruMuaVatTus.First().Kho.Ten,
                                                     TuNgay = s.TuNgay,
                                                     DenNgay = s.DenNgay,
                                                     TenNhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                                                     NgayYeuCau = s.NgayYeuCau,
                                                     GhiChu = s.GhiChu,
                                                     LyDoTruongKhoaTuChoi = s.LyDoKhoDuocTuChoi,
                                                     TinhTrang = ((s.KhoDuocDuyet == true && s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true) ? 1 : (s.KhoDuocDuyet == false || s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false) ? 2 : 0),
                                                     TenNhanVienKhoDuocDuyet = s.NhanVienKhoDuoc.User.HoTen,
                                                     NgayKhoDuocDuyet = s.NgayKhoDuocDuyet,
                                                     TenGiamDocDuyet = s.DuTruMuaVatTuKhoDuoc.GiamDoc.User.HoTen,
                                                     NgayGiamDocDuyet = s.DuTruMuaVatTuKhoDuoc.NgayGiamDocDuyet,
                                                     LyDoGiamDocTuChoi = s.DuTruMuaVatTuKhoDuoc.LyDoGiamDocTuChoi
                                                 })
                                                 .FirstOrDefault();
            return duTruMuaVatTu;
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
            var khoNhanVienQuanLys = _khoNhanVienQuanLyRepository.TableNoTracking.Where(p => p.NhanVienId == nhanVienLoginId && p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe).Select(p => p.KhoId).ToList();
            var data = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.SearchStringDaXuLy))
                {
                    var searchTerms = queryString.SearchStringDaXuLy.Replace("\t", "").Trim();
                    data = data.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.KhoaPhong.Ten,
                         g => g.NhanVienYeuCau.User.HoTen,
                         g => g.GhiChu


                   );
                }
            }
            var query = data
            .Where(p => p.DuTruMuaVatTus.Any(dp => khoNhanVienQuanLys.Contains(dp.KhoId)))
            .Select(s => new DuTruMuaKSNKTaiKhoaGridVo
            {
                Id = s.Id,
                SoPhieu = s.SoPhieu,
                TenKhoa = s.KhoaPhong.Ten,
                NgayYeuCau = s.NgayYeuCau,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                TuNgay = s.TuNgay,
                DenNgay = s.DenNgay,
                // 0: đã gửi và chờ duyệt, 1: đã duyệt, 2: từ chối
                TinhTrang = ((s.KhoDuocDuyet == true && s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true) ? 1 : (s.KhoDuocDuyet == false || s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false) ? 2 : 0),
                NgayKhoDuocDuyet = s.NgayKhoDuocDuyet,
                NgayGiamDocDuyet = s.DuTruMuaVatTuKhoDuoc.NgayGiamDocDuyet,
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


            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.SoPhieu,
                    g => g.TenKho,
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
            var khoNhanVienQuanLys = _khoNhanVienQuanLyRepository.TableNoTracking.Where(p => p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
                                                 && p.NhanVienId == nhanVienLoginId).Select(p => p.KhoId).ToList();
            var data = _duTruMuaVatTuTheoKhoaRepository.TableNoTracking;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuTruMuaDuocPhamTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.SearchStringDaXuLy))
                {
                    var searchTerms = queryString.SearchStringDaXuLy.Replace("\t", "").Trim();
                    data = data.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.KhoaPhong.Ten,
                         g => g.NhanVienYeuCau.User.HoTen,
                         g => g.GhiChu


                   );
                }
            }
            var query = data
            .Where(p => p.DuTruMuaVatTus.Any(dp => khoNhanVienQuanLys.Contains(dp.KhoId)))
            .Select(s => new DuTruMuaKSNKTaiKhoaGridVo
            {
                Id = s.Id,
                SoPhieu = s.SoPhieu,
                TenKhoa = s.KhoaPhong.Ten,
                NgayYeuCau = s.NgayYeuCau,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                TuNgay = s.TuNgay,
                DenNgay = s.DenNgay,
                // 0: đã gửi và chờ duyệt, 1: đã duyệt, 2: từ chối
                TinhTrang = ((s.KhoDuocDuyet == true && s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true) ? 1 : (s.KhoDuocDuyet == false || s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false) ? 2 : 0),
                NgayKhoDuocDuyet = s.NgayKhoDuocDuyet,
                NgayGiamDocDuyet = s.DuTruMuaVatTuKhoDuoc.NgayGiamDocDuyet,
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


            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.SoPhieu,
                    g => g.TenKho,
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
            var duTruMuaVatTuTheoKhoaId = long.Parse(queryInfo.SearchTerms);
            var query = BaseRepository.TableNoTracking
                        .Where(p => p.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuTheoKhoaId)
                        .Select(s => new DuTruMuaKSNKTaiKhoaGridVoDetail
                        {
                            Id = s.Id,
                            SoPhieu = s.SoPhieu,
                            TenKho = s.Kho.Ten,
                            TuNgay = s.TuNgay,
                            DenNgay = s.DenNgay,
                            NgayYeuCau = s.NgayYeuCau,
                            NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                            NgayTruongKhoaDuyet = s.NgayTruongKhoaDuyet,
                            TinhTrang = (s.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == true && s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true) ? 1
                                      : (s.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == false || s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false) ? 2 : 0,
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
            var duTruMuaVatTuTheoKhoaId = long.Parse(queryInfo.SearchTerms);
            var query = BaseRepository.TableNoTracking
                        .Where(p => p.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuTheoKhoaId)
                        .Select(s => new DuTruMuaKSNKTaiKhoaGridVoDetail
                        {
                            Id = s.Id,
                            SoPhieu = s.SoPhieu,
                            TenKho = s.Kho.Ten,
                            TuNgay = s.TuNgay,
                            DenNgay = s.DenNgay,
                            NgayYeuCau = s.NgayYeuCau,
                            NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                            NgayTruongKhoaDuyet = s.NgayTruongKhoaDuyet,
                            TinhTrang = (s.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == true && s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == true) ? 1
                                      : (s.DuTruMuaVatTuTheoKhoa.KhoDuocDuyet == false || s.DuTruMuaVatTuKhoDuoc.GiamDocDuyet == false) ? 2 : 0,
                        });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region DS THDT đã xử lý chi tiết       
        public async Task<GridDataSource> GetDuTruMuaKSNKTHDTChiTietForGridAsync(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            long duTruMuaVatTuTheoKhoaId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking
                .Where(x => x.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuTheoKhoaId)
                .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo()
                {
                    Id = item.Id,
                    DuTruMuaVatTuTheoKhoaId = item.DuTruMuaVatTuTheoKhoaId,
                    VatTuId = item.VatTuId,
                    TenVatTu = item.VatTu.Ten,
                    LaBHYT = item.LaVatTuBHYT,
                    HangSanXuat = item.VatTu.NhaSanXuat,
                    DonViTinh = item.VatTu.DonViTinh,
                    NuocSanXuat = item.VatTu.NuocSanXuat,
                    SoLuongDuTru = item.SoLuongDuTru,
                    SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                    SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet,
                })
                .GroupBy(x => new
                {
                    x.DuTruMuaVatTuTheoKhoaId,
                    x.VatTuId,
                    x.TenVatTu,
                    x.LaBHYT,
                    x.Nhom,
                    x.SoLuongDuTru,
                    x.SoLuongDuKienSuDung,
                    x.SoLuongDuTruTruongKhoaDuyet,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat
                })
                .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo()
                {
                    DuTruMuaVatTuTheoKhoaId = item.First().DuTruMuaVatTuTheoKhoaId,
                    VatTuId = item.First().VatTuId,
                    LaBHYT = item.First().LaBHYT,
                    TenVatTu = item.First().TenVatTu,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    NuocSanXuat = item.First().NuocSanXuat,
                    SoLuongDuTru = item.Sum(x => x.SoLuongDuTru),
                    SoLuongDuKienSuDung = item.Sum(x => x.SoLuongDuKienSuDung),
                    SoLuongDuTruTruongKhoaDuyet = item.Sum(x => x.SoLuongDuTruTruongKhoaDuyet),
                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaKSNKTHDTChiTietForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            long duTruMuaVatTuTheoKhoaId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaVatTuTheoKhoaChiTietRepository.TableNoTracking
                .Where(x => x.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuTheoKhoaId)
                .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo()
                {
                    Id = item.Id,
                    DuTruMuaVatTuTheoKhoaId = item.DuTruMuaVatTuTheoKhoaId,
                    VatTuId = item.VatTuId,
                    TenVatTu = item.VatTu.Ten,
                    LaBHYT = item.LaVatTuBHYT,
                    HangSanXuat = item.VatTu.NhaSanXuat,
                    DonViTinh = item.VatTu.DonViTinh,
                    NuocSanXuat = item.VatTu.NuocSanXuat,
                    SoLuongDuTru = item.SoLuongDuTru,
                    SoLuongDuKienSuDung = item.SoLuongDuKienSuDung,
                    SoLuongDuTruTruongKhoaDuyet = item.SoLuongDuTruTruongKhoaDuyet,
                })
                .GroupBy(x => new
                {
                    x.DuTruMuaVatTuTheoKhoaId,
                    x.VatTuId,
                    x.TenVatTu,
                    x.LaBHYT,
                    x.Nhom,
                    x.SoLuongDuTru,
                    x.SoLuongDuKienSuDung,
                    x.SoLuongDuTruTruongKhoaDuyet,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat
                })
                .Select(item => new DuTruMuaKSNKTaiKhoaChiTietVo()
                {
                    DuTruMuaVatTuTheoKhoaId = item.First().DuTruMuaVatTuTheoKhoaId,
                    VatTuId = item.First().VatTuId,
                    LaBHYT = item.First().LaBHYT,
                    TenVatTu = item.First().TenVatTu,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    NuocSanXuat = item.First().NuocSanXuat,
                    SoLuongDuTru = item.Sum(x => x.SoLuongDuTru),
                    SoLuongDuKienSuDung = item.Sum(x => x.SoLuongDuKienSuDung),
                    SoLuongDuTruTruongKhoaDuyet = item.Sum(x => x.SoLuongDuTruTruongKhoaDuyet),
                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion

        #region DS THDT đã xử lý chi tiết child
        public async Task<GridDataSource> GetDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var duTruMuaVatTuTheoKhoaId = long.Parse(queryObj[0]);
            bool laBHYT = bool.Parse(queryObj[1]);
            var vatTuId = long.Parse(queryObj[2]);

            var query = _duTruMuaVatTuChiTietRepository.TableNoTracking
                .Where(p => p.DuTruMuaVatTuTheoKhoaChiTiet.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuTheoKhoaId && p.LaVatTuBHYT == laBHYT && p.VatTuId == vatTuId)
                .Select(s => new DuTruMuaKSNKTaiKhoaChiTietVo
                {
                    Id = s.Id,
                    LaBHYT = s.LaVatTuBHYT,
                    TenKho = s.DuTruMuaVatTu.Kho.Ten,
                    KyDuTruDisplay = s.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + " - " + s.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienSuDung = s.SoLuongDuKienSuDung
                });
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var duTruMuaVatTuTheoKhoaId = long.Parse(queryObj[0]);
            bool laBHYT = bool.Parse(queryObj[1]);
            var vatTuId = long.Parse(queryObj[2]);

            var query = _duTruMuaVatTuChiTietRepository.TableNoTracking
                .Where(p => p.DuTruMuaVatTuTheoKhoaChiTiet.DuTruMuaVatTuTheoKhoaId == duTruMuaVatTuTheoKhoaId && p.LaVatTuBHYT == laBHYT && p.VatTuId == vatTuId)
                .Select(s => new DuTruMuaKSNKTaiKhoaChiTietVo
                {
                    Id = s.Id,
                    LaBHYT = s.LaVatTuBHYT,
                    TenKho = s.DuTruMuaVatTu.Kho.Ten,
                    KyDuTruDisplay = s.DuTruMuaVatTu.TuNgay.ApplyFormatDate() + " - " + s.DuTruMuaVatTu.DenNgay.ApplyFormatDate(),
                    SoLuongDuTru = s.SoLuongDuTru,
                    SoLuongDuKienSuDung = s.SoLuongDuKienSuDung
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region DS THDT Chi tiết phiếu mua vật tư dự trù

        public async Task<GridDataSource> GetDuTruMuaKSNKChiTietForGridAsyncChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var duTruMuaVatTuId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaVatTuChiTietRepository.TableNoTracking
                        .Where(p => p.DuTruMuaVatTuId == duTruMuaVatTuId)
                        .Select(s => new DuTruMuaKSNKTaiKhoaChiTietVo
                        {
                            TenVatTu = s.VatTu.Ten,
                            LaBHYT = s.LaVatTuBHYT,
                            DonViTinh = s.VatTu.DonViTinh,
                            HangSanXuat = s.VatTu.NhaSanXuat,
                            NuocSanXuat = s.VatTu.NuocSanXuat,
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

        public async Task<GridDataSource> GetTotalDuTruMuaKSNKChiTietForGridAsyncChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var duTruMuaVatTuId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaVatTuChiTietRepository.TableNoTracking
                        .Where(p => p.DuTruMuaVatTuId == duTruMuaVatTuId)
                        .Select(s => new DuTruMuaKSNKTaiKhoaChiTietVo
                        {
                            TenVatTu = s.VatTu.Ten,
                            LaBHYT = s.LaVatTuBHYT,
                            DonViTinh = s.VatTu.DonViTinh,
                            HangSanXuat = s.VatTu.NhaSanXuat,
                            NuocSanXuat = s.VatTu.NuocSanXuat,
                            SoLuongDuTru = s.SoLuongDuTru,
                            SoLuongDuKienSuDung = s.SoLuongDuKienSuDung,
                            SoLuongDuTruTruongKhoaDuyet = s.SoLuongDuTruTruongKhoaDuyet.GetValueOrDefault()
                        });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion

        #region DS THDT Từ chối 
        public async Task<GridDataSource> GetDataDuTruMuaKSNKTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
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
            var data = query
                       .Where(p => p.TruongKhoaDuyet == false)
                        .Select(o => new DuTruMuaKSNKTaiKhoaGridVo
                        {
                            Id = o.Id,
                            SoPhieu = o.SoPhieu,
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
                var queryString = JsonConvert.DeserializeObject<DuTruMuaKSNKTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
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

            var sortString = queryInfo.SortString.Contains("KyDuTru") ? queryInfo.SortString.Replace("KyDuTru", "TuNgay") : queryInfo.SortString;

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : data.CountAsync();
            var queryTask = data.OrderBy(sortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDuTruMuaKSNKTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
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
            var data = query
                       .Where(p => p.TruongKhoaDuyet == false)
                        .Select(o => new DuTruMuaKSNKTaiKhoaGridVo
                        {
                            Id = o.Id,
                            SoPhieu = o.SoPhieu,
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
                var queryString = JsonConvert.DeserializeObject<DuTruMuaKSNKTaiKhoaGridVo>(queryInfo.AdditionalSearchString);
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

        public async Task<GridDataSource> GetDSTHDTTuChoiChildForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var duTruMuaVatTuId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaVatTuChiTietRepository.TableNoTracking
                        .Where(p => p.DuTruMuaVatTuId == duTruMuaVatTuId)
                        .Select(s => new DuTruMuaKSNKTaiKhoaChiTietVo
                        {
                            Id = s.Id,
                            VatTuId = s.VatTuId,
                            TenVatTu = s.VatTu.Ten,
                            LaBHYT = s.LaVatTuBHYT,
                            DonViTinh = s.VatTu.DonViTinh,
                            HangSanXuat = s.VatTu.NhaSanXuat,
                            NuocSanXuat = s.VatTu.NuocSanXuat,
                            SoLuongDuTru = s.SoLuongDuTru,
                            SoLuongDuKienSuDung = s.SoLuongDuKienSuDung,
                            SoLuongDuTruTruongKhoaDuyet = s.SoLuongDuTruTruongKhoaDuyet.GetValueOrDefault()
                        });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDSTHDTTuChoiChildForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var duTruMuaVatTuId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _duTruMuaVatTuChiTietRepository.TableNoTracking
                        .Where(p => p.DuTruMuaVatTuId == duTruMuaVatTuId)
                        .Select(s => new DuTruMuaKSNKTaiKhoaChiTietVo
                        {
                            Id = s.Id,
                            VatTuId = s.VatTuId,
                            TenVatTu = s.VatTu.Ten,
                            LaBHYT = s.LaVatTuBHYT,
                            DonViTinh = s.VatTu.DonViTinh,
                            HangSanXuat = s.VatTu.NhaSanXuat,
                            NuocSanXuat = s.VatTu.NuocSanXuat,
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
