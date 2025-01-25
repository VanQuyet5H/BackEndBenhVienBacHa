using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.GiuongBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.GiuongBenhs
{
    [ScopedDependency(ServiceType = typeof(IGiuongBenhService))]
    public class GiuongBenhService : MasterFileService<GiuongBenh>, IGiuongBenhService
    {
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IRepository<BenhNhan> _benhNhanRepository;
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<HoatDongGiuongBenh> _hoatDongGiuongBenhRepository;
        public GiuongBenhService(IRepository<GiuongBenh> repository
            , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
            , IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository
            , IRepository<BenhNhan> benhNhanRepository, IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
            IRepository<HoatDongGiuongBenh> hoatDongGiuongBenhRepository
            ) : base(repository)
        {
            _phongBenhVienRepository = phongBenhVienRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _benhNhanRepository = benhNhanRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _hoatDongGiuongBenhRepository = hoatDongGiuongBenhRepository;
        }

        public bool GiuongDangCoBenhNhan(long giuongBenhId)
        {
            return _hoatDongGiuongBenhRepository.TableNoTracking.Any(o =>
                o.GiuongBenhId == giuongBenhId && o.ThoiDiemKetThuc == null);
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
                .Select(s => new GiuongBenhGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    KhoaId = s.PhongBenhVien.KhoaPhongId,
                    Khoa = s.PhongBenhVien.KhoaPhong.Ten,
                    PhongId = s.PhongBenhVienId,
                    Phong = s.PhongBenhVien.Ten,
                    MoTa = s.MoTa,
                    CoHieuLuc=s.IsDisabled!=true,
                    LaGiuongNoi = s.LaGiuongNoi
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten, g => g.Khoa, g => g.Phong, g => g.MoTa);

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GiuongBenhSearchHeader>(queryInfo.AdditionalSearchString);
                if (queryString.KhoaId != null)
                {
                    query = query.Where(p => p.KhoaId == queryString.KhoaId);
                }
                if (queryString.PhongId != null)
                {
                    query = query.Where(p => p.PhongId == queryString.PhongId);
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking.Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
                .Select(s => new GiuongBenhGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    KhoaId = s.PhongBenhVien.KhoaPhongId,
                    Khoa = s.PhongBenhVien.KhoaPhong.Ten,
                    PhongId = s.PhongBenhVienId,
                    Phong = s.PhongBenhVien.Ten,
                    MoTa = s.MoTa,
                    CoHieuLuc = s.IsDisabled != true
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten, g => g.Khoa, g => g.Phong, g => g.MoTa);

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GiuongBenhSearchHeader>(queryInfo.AdditionalSearchString);
                if (queryString.KhoaId != null)
                {
                    query = query.Where(p => p.KhoaId == queryString.KhoaId);
                }
                if (queryString.PhongId != null)
                {
                    query = query.Where(p => p.PhongId == queryString.PhongId);
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridSoDoGiuongBenhKhoaAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = _khoaPhongRepository.TableNoTracking
                .Include(x => x.PhongBenhViens).ThenInclude(x => x.GiuongBenhs).ThenInclude(x => x.HoatDongGiuongBenhs)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SoDoGiuongBenhSearchHeader>(queryInfo.AdditionalSearchString);
                if (queryString.KhoaId != null && queryString.KhoaId != 0)
                {
                    query = query.Where(p => p.Id == queryString.KhoaId);
                }
            }

            //var query = BaseRepository.TableNoTracking.Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
            var dateTimeNow = DateTime.Now.Date;

            var result =  query.Select(s => new SoDoGiuongBenhKhoaGridVo()
             {
                 Id = s.Id,
                 Ten = s.Ten,
                 GiuongTrong = s.PhongBenhViens.SelectMany(p => p.GiuongBenhs).Where(p => p.IsDisabled != true)
                 .Count(p => !p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))),
                 GiuongCoBenhNhan = s.PhongBenhViens.SelectMany(p => p.GiuongBenhs).Where(p => p.IsDisabled != true)
                     .Count(p => p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))),
                 TongGiuongBenhCuaKhoa = s.PhongBenhViens.SelectMany(p => p.GiuongBenhs).Where(p => p.IsDisabled != true).Count(),
                 SoGiuongGhep = s.PhongBenhViens.SelectMany(p => p.GiuongBenhs).Where(p => p.IsDisabled != true 
                        && p.HoatDongGiuongBenhs.Any(m => m.NamGhep == true && m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))).Distinct()
                     .Count()

            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : result.CountAsync();
            var queryTask = result.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);



            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                item.STT = stt;
                item.TongSoGiuongTrong = queryTask.Result.Sum(p => p.GiuongTrong);
                item.TongSoGiuongCoBenhNhan = queryTask.Result.Sum(p => p.GiuongCoBenhNhan);
                item.TongSoTongGiuongBenhCuaKhoa = queryTask.Result.Sum(p => p.TongGiuongBenhCuaKhoa);
                item.TongSoGiuongGhep = queryTask.Result.Sum(p => p.SoGiuongGhep);

                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridSoDoGiuongBenhKhoaAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var query = _khoaPhongRepository.TableNoTracking
                .Include(x => x.PhongBenhViens).ThenInclude(x => x.GiuongBenhs).ThenInclude(x => x.HoatDongGiuongBenhs)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SoDoGiuongBenhSearchHeader>(queryInfo.AdditionalSearchString);
                if (queryString.KhoaId != null && queryString.KhoaId != 0)
                {
                    query = query.Where(p => p.Id == queryString.KhoaId);
                }
            }

            //var query = BaseRepository.TableNoTracking.Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
            var dateTimeNow = DateTime.Now.Date;

            var result = query.Select(s => new SoDoGiuongBenhKhoaGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                GiuongTrong = s.PhongBenhViens.SelectMany(p => p.GiuongBenhs).Where(p => p.IsDisabled != true)
                .Count(p => !p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))),
                GiuongCoBenhNhan = s.PhongBenhViens.SelectMany(p => p.GiuongBenhs).Where(p => p.IsDisabled != true)
                    .Count(p => p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))),
                TongGiuongBenhCuaKhoa = s.PhongBenhViens.SelectMany(p => p.GiuongBenhs).Where(p => p.IsDisabled != true).Count(),
                SoGiuongGhep = s.PhongBenhViens.SelectMany(p => p.GiuongBenhs).Where(p => p.IsDisabled != true 
                    && p.HoatDongGiuongBenhs.Any(m => m.NamGhep == true && m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))).Distinct()
                    .Count()

            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);

            var countTask = result.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridSoDoGiuongBenhPhongAsync(QueryInfo queryInfo, long khoaId = 0)
        {
            BuildDefaultSortExpression(queryInfo);

            var query = _phongBenhVienRepository.TableNoTracking
                .Include(x => x.GiuongBenhs).ThenInclude(x => x.HoatDongGiuongBenhs)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SoDoGiuongBenhSearchHeader>(queryInfo.AdditionalSearchString);
                if (queryString.KhoaId != null && queryString.KhoaId != 0)
                {
                    query = query.Where(p => p.Id == queryString.KhoaId);
                }
            }

            if (khoaId != 0)
            {
                query = query.Where(p => p.KhoaPhongId == khoaId);
            }

            //var query = BaseRepository.TableNoTracking.Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
            var dateTimeNow = DateTime.Now.Date;

            var result = query.Select(s => new SoDoGiuongBenhKhoaGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                GiuongTrong = s.GiuongBenhs.Where(p => p.IsDisabled != true)
                .Count(p => !p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))),
                GiuongCoBenhNhan = s.GiuongBenhs.Where(p => p.IsDisabled != true)
                    .Count(p => p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))),
                TongGiuongBenhCuaKhoa = s.GiuongBenhs.Where(p => p.IsDisabled != true).Count(),
                SoGiuongGhep = s.GiuongBenhs.Where(p => p.IsDisabled != true
                 && p.HoatDongGiuongBenhs.Any(m => m.NamGhep == true && m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))).Distinct()
                    .Count()

            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);

            var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
            var queryTask = result.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            //await Task.WhenAll(countTask, queryTask);



            var stt = 1;
            foreach (var item in queryTask)
            {
                item.STT = stt;
                item.TongSoGiuongTrong = queryTask.Sum(p => p.GiuongTrong);
                item.TongSoGiuongCoBenhNhan = queryTask.Sum(p => p.GiuongCoBenhNhan);
                item.TongSoTongGiuongBenhCuaKhoa = queryTask.Sum(p => p.TongGiuongBenhCuaKhoa);
                item.TongSoGiuongGhep = queryTask.Sum(p => p.SoGiuongGhep);

                stt++;
            }

            if (!string.IsNullOrEmpty(queryInfo.SortString))
            {
                queryTask = queryTask.AsQueryable().OrderBy(queryInfo.SortString).ToArray();
            }

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridSoDoGiuongBenhPhongAsync(QueryInfo queryInfo, long khoaId = 0)
        {
            BuildDefaultSortExpression(queryInfo);

            var query = _phongBenhVienRepository.TableNoTracking
                .Include(x => x.GiuongBenhs).ThenInclude(x => x.HoatDongGiuongBenhs)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SoDoGiuongBenhSearchHeader>(queryInfo.AdditionalSearchString);
                if (queryString.KhoaId != null && queryString.KhoaId != 0)
                {
                    query = query.Where(p => p.Id == queryString.KhoaId);
                }
            }

            if (khoaId != 0)
            {
                query = query.Where(p => p.KhoaPhongId == khoaId);
            }
            //var query = BaseRepository.TableNoTracking.Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
            var dateTimeNow = DateTime.Now.Date;

            var result = query.Select(s => new SoDoGiuongBenhKhoaGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                GiuongTrong = s.GiuongBenhs.Where(p => p.IsDisabled != true)
                .Count(p => !p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))),
                GiuongCoBenhNhan = s.GiuongBenhs.Where(p => p.IsDisabled != true)
                    .Count(p => p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))),
                TongGiuongBenhCuaKhoa = s.GiuongBenhs.Where(p => p.IsDisabled != true).Count(),
                SoGiuongGhep = s.GiuongBenhs.Where(p => p.IsDisabled != true 
                && p.HoatDongGiuongBenhs.Any(m => m.NamGhep == true && m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))).Distinct()
                    .Count()

            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);

            var countTask = result.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridSoDoGiuongBenhKhoaPhongAsync(QueryInfo queryInfo, long khoaId, long phongId)
        {
            BuildDefaultSortExpression(queryInfo);

            var dateTimeNow = DateTime.Now.Date;

            var query = BaseRepository.TableNoTracking
                .Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
                .Include(x => x.HoatDongGiuongBenhs)
                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.DichVuGiuongBenhVien)
                .Where(p => p.PhongBenhVienId == phongId && p.PhongBenhVien.KhoaPhongId == khoaId)
                .AsQueryable();

            var tenKhoa = query.FirstOrDefault()?.PhongBenhVien.Ten;
            var tenPhong = query.FirstOrDefault()?.PhongBenhVien.KhoaPhong.Ten;

            var totalGiuong = await query.CountAsync();
            var totalGiuongTrong = await query
                .Where(p => p.IsDisabled != true && !p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow)))
                .CountAsync();
            var TotalGiuongCoBenhNhan = await query
                .Where(p => p.IsDisabled != true &&  p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow)))
                .CountAsync();
            var TotalGiuongNamGhep = await query
                .Where(p => p.IsDisabled != true 
                    && p.HoatDongGiuongBenhs.Any(m => m.NamGhep == true && m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))).Distinct()
                .CountAsync();


            var result = query.Select(s => new SoDoGiuongBenhKhoaPhongGridVo()
            {
                Id = s.Id,
                TenGiuong = s.Ten,
                DonGiaDisplay = s.HoatDongGiuongBenhs.Any()
                    ? s.HoatDongGiuongBenhs.OrderBy(p => p.ThoiDiemBatDau).FirstOrDefault().YeuCauDichVuGiuongBenhVien
                          .Gia.ApplyFormatMoneyVND() + ""
                    : "",
                TongSoGiuong = totalGiuong,
                TongSoGiuongTrong = totalGiuongTrong,
                TongSoGiuongCoBenhNhan = TotalGiuongCoBenhNhan,
                TongSoGiuongNamGhep = TotalGiuongNamGhep,
                //lstYCTN = s.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))
                //    ? s.HoatDongGiuongBenhs.Where(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))
                //        .Select(p => p.YeuCauTiepNhanId): new List<long>(),

            });
            //.ApplyLike(queryInfo.SearchTerms, g => g.TenGiuong);

            var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
            var queryTask = result.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            //await Task.WhenAll(countTask, queryTask);



            var stt = 1;
            foreach (var item in queryTask)
            {
                //
                var lst = await BaseRepository.TableNoTracking
                    .Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
                    .Include(x => x.HoatDongGiuongBenhs)
                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.DichVuGiuongBenhVien)
                    .FirstOrDefaultAsync(p => p.Id == item.Id);
                item.lstYCTN = lst.HoatDongGiuongBenhs.Any(m =>
                    m.ThoiDiemBatDau.Date <= dateTimeNow &&
                    (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))
                    ? lst.HoatDongGiuongBenhs.Where(m =>
                            m.ThoiDiemBatDau.Date <= dateTimeNow &&
                            (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))
                        .Select(p => p.YeuCauTiepNhanId).ToList()
                    : new List<long>();
                //
                item.STT = stt;

                if (item.lstYCTN != null)
                {
                    foreach (var yctn in item.lstYCTN)
                    {
                        var entity = await _yeuCauTiepNhanRepository.TableNoTracking
                            .Include(p => p.BenhNhan)
                            .Include(p => p.HoatDongGiuongBenhs).ThenInclude(p => p.GiuongBenh)
                            .FirstOrDefaultAsync(p => p.Id == yctn);
                        var benhNhanGiuong = new LstBenhNhanGiuong
                        {
                            HoVaTen = entity?.BenhNhan.HoTen,
                            MaBenhNhan = entity?.BenhNhan.MaBN,
                            KhoaDieuTri = tenKhoa,
                            Phong = tenPhong,
                            NamSinh = entity?.BenhNhan.NamSinh + "",
                            SoDienThoai = entity?.BenhNhan.SoDienThoai,
                            TinhTrangBenh = entity.HoatDongGiuongBenhs.Any(m =>
                                m.ThoiDiemBatDau.Date <= dateTimeNow &&
                                (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow) && m.GiuongBenhId == item.Id) 
                                ? entity.HoatDongGiuongBenhs.First(m =>
                                    m.ThoiDiemBatDau.Date <= dateTimeNow &&
                                    (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow) && m.GiuongBenhId == item.Id).TinhTrangBenhNhan.GetDescription() : "",
                            NgayVaoVien = entity?.ThoiDiemTiepNhan.ApplyFormatDate(),
                            NgayRaVien = "",
                        };
                        item.lstBenhNhanGiuong.Add(benhNhanGiuong);
                    }
                }
               

                stt++;
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                queryTask = queryTask.AsQueryable().Where(p => p.lstBenhNhanGiuong.Any(m => m.HoVaTen.Trim().ToLower().ConvertUnicodeString().ConvertToUnSign()
                .Contains(queryInfo.SearchTerms.Trim().ToLower().ConvertUnicodeString().ConvertToUnSign())) 
                    || p.TenGiuong.Trim().ToLower().ConvertUnicodeString().ConvertToUnSign().Contains(queryInfo.SearchTerms.Trim().ToLower().ConvertUnicodeString().ConvertToUnSign()) )
                .OrderBy(queryInfo.SortString).ToArray();
            }

            if (!string.IsNullOrEmpty(queryInfo.SortString))
            {
                queryTask = queryTask.AsQueryable().OrderBy(queryInfo.SortString).ToArray();
            }

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };

        }

        public async Task<GridDataSource> GetTotalPageForGridSoDoGiuongBenhKhoaPhongAsync(QueryInfo queryInfo, long khoaId, long phongId)
        {
            BuildDefaultSortExpression(queryInfo);

            var dateTimeNow = DateTime.Now.Date;

            var query = BaseRepository.TableNoTracking
                .Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
                .Include(x => x.HoatDongGiuongBenhs)
                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.DichVuGiuongBenhVien)
                .Where(p => p.PhongBenhVienId == phongId && p.PhongBenhVien.KhoaPhongId == khoaId)
                .AsQueryable();

            var tenKhoa = query.FirstOrDefault()?.PhongBenhVien.Ten;
            var tenPhong = query.FirstOrDefault()?.PhongBenhVien.KhoaPhong.Ten;

            var totalGiuong = await query.CountAsync();
            var totalGiuongTrong = await query
                .Where(p => p.IsDisabled != true && !p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow)))
                .CountAsync();
            var TotalGiuongCoBenhNhan = await query
                .Where(p => p.IsDisabled != true && p.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow)))
                .CountAsync();
            var TotalGiuongNamGhep = await query
                .Where(p => p.IsDisabled != true
                    && p.HoatDongGiuongBenhs.Any(m => m.NamGhep == true && m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))).Distinct()
                .CountAsync();


            var result = query.Select(s => new SoDoGiuongBenhKhoaPhongGridVo()
            {
                Id = s.Id,
                TenGiuong = s.Ten,
                DonGiaDisplay = s.HoatDongGiuongBenhs.Any() ? s.HoatDongGiuongBenhs.OrderBy(p => p.ThoiDiemBatDau).FirstOrDefault().YeuCauDichVuGiuongBenhVien.Gia.ApplyFormatMoneyVND() + ""
                    : "",
                TongSoGiuong = totalGiuong,
                TongSoGiuongTrong = totalGiuongTrong,
                TongSoGiuongCoBenhNhan = TotalGiuongCoBenhNhan,
                TongSoGiuongNamGhep = TotalGiuongNamGhep,
                //lstYCTN = s.HoatDongGiuongBenhs.Any(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))
                //    ? s.HoatDongGiuongBenhs.Where(m => m.ThoiDiemBatDau.Date <= dateTimeNow && (m.ThoiDiemKetThuc == null || m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow))
                //        .Select(p => p.YeuCauTiepNhanId): new List<long>(),

            }).ApplyLike(queryInfo.SearchTerms, g => g.TenGiuong);
            
            var countTask = result.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<string> GetTenKhoa(long id)
        {
            var khoaPhong = await _khoaPhongRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return khoaPhong?.Ten;
        }

        public async Task<string> GetMaTenPhong(long id)
        {
            var phong = await _phongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return phong != null ? phong.Ma + " - " + phong.Ten : "";
        }

        public async Task<ResultSoDoPopup> getLstPhongForKhoaPopup(ResultSoDoPopup model)
        {
            var result = new ResultSoDoPopup
            {
                PhongId = model.PhongId,
                KhoaId = model.KhoaId,
                TenKhoa = model.TenKhoa,
            };

            var dateTimeNow = DateTime.Now.Date;

            if (model.PhongId == null || model.PhongId == 0)
            {
                var lstGiuong = await BaseRepository.TableNoTracking
                    .Include(p => p.PhongBenhVien).ThenInclude(p => p.KhoaPhong)
                    .Include(p => p.HoatDongGiuongBenhs)
                    .Where(p => p.IsDisabled != true && p.PhongBenhVien.KhoaPhongId == model.KhoaId).ToListAsync();
                foreach (var item in lstGiuong)
                {
                    var soNguoiNamGiuong = item.HoatDongGiuongBenhs
                        .Count(m => m.ThoiDiemBatDau.Date <= dateTimeNow &&
                                    (m.ThoiDiemKetThuc == null ||
                                     m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow));
                    var giuong = new LstGiuong
                    {
                        TenGiuong = item.Ten,
                        IsGiuongTrong = soNguoiNamGiuong == 0,
                        SoBenhNhan = soNguoiNamGiuong < 10 ? "0" + soNguoiNamGiuong : soNguoiNamGiuong + "",

                    };

                    if (result.lstPhong.Any(p => p.PhongId == item.PhongBenhVienId))
                    {
                        result.lstPhong.FirstOrDefault(p => p.PhongId == item.PhongBenhVienId)?.lstGiuong.Add(giuong);
                    }
                    else
                    {
                        var phong = new LstPhong
                        {
                            PhongId = item.PhongBenhVienId,
                            DisplayName = item.PhongBenhVien.Ma + " - " + item.PhongBenhVien.Ten,
                        };

                        phong.lstGiuong.Add(giuong);
                        result.lstPhong. Add(phong);
                    }
                }
            }
            else
            {
                var lstGiuong = await BaseRepository.TableNoTracking
                    .Include(p => p.PhongBenhVien).ThenInclude(p => p.KhoaPhong)
                    .Include(p => p.HoatDongGiuongBenhs)
                    .Where(p => p.IsDisabled != true && p.PhongBenhVienId == model.PhongId).ToListAsync();
                foreach (var item in lstGiuong)
                {
                    var soNguoiNamGiuong = item.HoatDongGiuongBenhs
                        .Count(m => m.ThoiDiemBatDau.Date <= dateTimeNow &&
                                    (m.ThoiDiemKetThuc == null ||
                                     m.ThoiDiemKetThuc.GetValueOrDefault().Date > dateTimeNow));
                    var giuong = new LstGiuong
                    {
                        TenGiuong = item.Ten,
                        IsGiuongTrong = soNguoiNamGiuong == 0,
                        SoBenhNhan = soNguoiNamGiuong < 10 ? "0" + soNguoiNamGiuong : soNguoiNamGiuong + "",

                    };
                    if (result.lstPhong.Any(p => p.PhongId == item.PhongBenhVienId))
                    {
                        result.lstPhong.FirstOrDefault(p => p.PhongId == item.PhongBenhVienId)?.lstGiuong.Add(giuong);
                    }
                    else
                    {
                        var phong = new LstPhong
                        {
                            PhongId = item.PhongBenhVienId,
                            DisplayName = item.PhongBenhVien.Ma + " - " + item.PhongBenhVien.Ten,
                        };

                        phong.lstGiuong.Add(giuong);
                        result.lstPhong.Add(phong);
                    }
                }
            }

            return result;
        }

        public async Task<bool> KiemTraMaGiuongBenhAsync(long id, string ma)
        {
            if (string.IsNullOrEmpty(ma))
            {
                return true;
            }

            var kiemTra = await BaseRepository.TableNoTracking.AnyAsync(x =>
                (id == 0 || x.Id != id)
                && x.Ma.ToLower().Trim() == ma.ToLower().Trim());
            return !kiemTra;
        }
    }
}