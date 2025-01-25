using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauTraThuocTuBenhNhan;
using Newtonsoft.Json;
using System;
using System.Globalization;
using Camino.Services.Helpers;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain;
using Camino.Services.CauHinh;

namespace Camino.Services.YeuCauTraVatTuTuBenhNhan
{
    [ScopedDependency(ServiceType = typeof(IYeuCauTraVatTuTuBenhNhanService))]

    public class YeuCauTraVatTuTuBenhNhanService : MasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan>, IYeuCauTraVatTuTuBenhNhanService

    {
        public IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        public IRepository<Core.Domain.Entities.Users.User> _userRepository;
        public IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        public IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        public IRepository<Kho> _khoRepository;
        public IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        public IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTietRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Template> _templateRepository;
        private readonly ICauHinhService _cauHinhService;

        public YeuCauTraVatTuTuBenhNhanService(IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan> repository,
           IRepository<Core.Domain.Entities.Users.User> userRepository,
           IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
           IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
           IUserAgentHelper userAgentHelper,
           IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
           IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> yeuCauTraVatTuTuBenhNhanChiTietRepository,
           IRepository<Kho> khoRepository,
           IRepository<Template> templateRepository,
           ICauHinhService cauHinhService,
        IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository) : base(repository)
        {
            _nhanVienRepository = nhanVienRepository;
            _userRepository = userRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _yeuCauTraVatTuTuBenhNhanChiTietRepository = yeuCauTraVatTuTuBenhNhanChiTietRepository;
            _userAgentHelper = userAgentHelper;
            _phongBenhVienRepository = phongBenhVienRepository;
            _khoRepository = khoRepository;
            _templateRepository = templateRepository;
            _cauHinhService = cauHinhService;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                .Where(p => phongBenhVien != null && p.KhoaHoanTraId == phongBenhVien.KhoaPhongId)
                .Select(s => new YeuCauTraVatTuTuBenhNhanGridVo
                {
                    Id = s.Id,
                    SoPhieu = s.SoPhieu,
                    KhoaHoanTraId = s.KhoaHoanTraId,
                    TenKhoa = s.KhoaHoanTra.Ten,
                    KhoTraId = s.KhoTraId,
                    TenKho = s.KhoTra.Ten,
                    NhanVienYeuCauId = s.NhanVienYeuCauId,
                    NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                    NgayYeuCau = s.NgayYeuCau,
                    DuocDuyet = s.DuocDuyet,
                    NhanVienDuyetId = s.NhanVienDuyetId,
                    NhanVienDuyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<YeuCauTraVatTuTuBenhNhanGridVo>(queryInfo.AdditionalSearchString);
                // 0: chờ duyệt, 1: đã duyệt, 2: từ chối
                if (queryString.ChoDuyet == false && queryString.DaDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == false)
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
                         g => g.NhanVienYeuCau,
                         g => g.NhanVienDuyet,
                         g => g.TenKhoa
                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                         g => g.SoPhieu,
                         g => g.TenKho,
                         g => g.NhanVienYeuCau,
                         g => g.NhanVienDuyet,
                         g => g.TenKhoa
                    );
            }

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TinhTrang";
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).ThenByDescending(p => p.NgayYeuCau).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                .Where(p => phongBenhVien != null && p.KhoaHoanTraId == phongBenhVien.KhoaPhongId)
                 .Select(s => new YeuCauTraVatTuTuBenhNhanGridVo
                 {
                     Id = s.Id,
                     SoPhieu = s.SoPhieu,
                     KhoaHoanTraId = s.KhoaHoanTraId,
                     TenKhoa = s.KhoaHoanTra.Ten,
                     KhoTraId = s.KhoTraId,
                     TenKho = s.KhoTra.Ten,
                     NhanVienYeuCauId = s.NhanVienYeuCauId,
                     NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                     NgayYeuCau = s.NgayYeuCau,
                     DuocDuyet = s.DuocDuyet,
                     NhanVienDuyetId = s.NhanVienDuyetId,
                     NhanVienDuyet = s.NhanVienDuyet.User.HoTen,
                     NgayDuyet = s.NgayDuyet

                 });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<YeuCauTraVatTuTuBenhNhanGridVo>(queryInfo.AdditionalSearchString);
                // 0: chờ duyệt, 1: đã duyệt, 2: từ chối
                if (queryString.ChoDuyet == false && queryString.DaDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == false)
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
                         g => g.NhanVienYeuCau,
                         g => g.NhanVienDuyet,
                         g => g.TenKhoa
                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                         g => g.SoPhieu,
                         g => g.TenKho,
                         g => g.NhanVienYeuCau,
                         g => g.NhanVienDuyet,
                         g => g.TenKhoa
                    );
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };

        }

        //Vat Tu
        public async Task<GridDataSource> GetDataForGridAsyncVatTuChild(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var yeuCauTraVatTuTuBenhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                        .Where(p => p.YeuCauTraVatTuTuBenhNhanId == yeuCauTraVatTuTuBenhNhanId)
                        .Select(s => new TraVatTuChiTietGridVo
                        {
                            YeuCauTraVatTuTuBenhNhanId = s.YeuCauTraVatTuTuBenhNhanId,
                            VatTuBenhVienId = s.VatTuBenhVienId,
                            Ten = s.VatTuBenhVien.VatTus.Ten,
                            LaVatTuBHYT = s.LaVatTuBHYT,
                            DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                            SLDaTraLanNay = s.SoLuongTra,
                            SLChiDinh = s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0),
                            SLDaTra = s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0,
                            DuocDuyet = s.YeuCauTraVatTuTuBenhNhan.DuocDuyet
                        }).GroupBy(x => new { x.YeuCauTraVatTuTuBenhNhanId, x.VatTuBenhVienId, x.Ten, x.LaVatTuBHYT, })
                        .Select(item => new TraVatTuChiTietGridVo()
                        {
                            YeuCauTraVatTuTuBenhNhanId = item.First().YeuCauTraVatTuTuBenhNhanId,
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaVatTuBHYT = item.First().LaVatTuBHYT,
                            Ten = item.First().Ten,
                            DVT = item.First().DVT,
                            DuocDuyet = item.First().DuocDuyet,
                            TongSoLuongTraLanNay = item.Sum(x => x.SLDaTraLanNay),
                            TongSoLuongDaTra = item.Sum(x => x.SLDaTra),
                            TongSoLuongChiDinh = item.Sum(x => x.SLChiDinh),
                        })
                    .OrderBy(x => x.LaVatTuBHYT).ThenBy(x => x.Ten).Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
            var dataReturn = queryTask.Result;
            foreach (var dataItem in dataReturn)
            {
                if (dataItem.DuocDuyet != true)
                {
                    dataItem.TongSoLuongDaTra = dataItem.TongSoLuongDaTra - dataItem.TongSoLuongTraLanNay;
                }
            }
            //end update BVHD-3411

            return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncVatTuChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var yeuCauTraVatTuTuBenhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                        .Where(p => p.YeuCauTraVatTuTuBenhNhanId == yeuCauTraVatTuTuBenhNhanId)
                         .Select(s => new TraVatTuChiTietGridVo
                         {
                             YeuCauTraVatTuTuBenhNhanId = s.YeuCauTraVatTuTuBenhNhanId,
                             VatTuBenhVienId = s.VatTuBenhVienId,
                             Ten = s.VatTuBenhVien.VatTus.Ten,
                             LaVatTuBHYT = s.LaVatTuBHYT,
                             DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                             SLDaTraLanNay = s.SoLuongTra,
                             SLChiDinh = s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0),
                             SLDaTra = s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0,
                             YeuCauVatTuBenhVienId = s.YeuCauVatTuBenhVienId
                         }).GroupBy(x => new { x.YeuCauTraVatTuTuBenhNhanId, x.YeuCauVatTuBenhVienId, x.VatTuBenhVienId, x.Ten, x.LaVatTuBHYT, x.SLChiDinh })
                        .Select(item => new TraVatTuChiTietGridVo()
                        {
                            YeuCauVatTuBenhVienId = item.First().YeuCauVatTuBenhVienId,
                            YeuCauTraVatTuTuBenhNhanId = item.First().YeuCauTraVatTuTuBenhNhanId,
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaVatTuBHYT = item.First().LaVatTuBHYT,
                            Ten = item.First().Ten,
                            DVT = item.First().DVT,
                            TongSoLuongTraLanNay = item.Sum(x => x.SLDaTraLanNay),
                            TongSoLuongDaTra = item.First().SLDaTra,
                            TongSoLuongChiDinh = item.First().SLChiDinh,
                        })
                    .OrderBy(x => x.LaVatTuBHYT).ThenBy(x => x.Ten).Distinct();
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataForGridAsyncVatTuTheoKho(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryString = JsonConvert.DeserializeObject<TraVatTuChiTietGridVo>(queryInfo.AdditionalSearchString);
            if (queryString.IsCreate)
            {
                var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                        .Where(p => p.KhoTraId == queryString.KhoTraId
                                   && (p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.KhoaPhongDieuTriId == queryString.KhoaHoanTraId || p.YeuCauVatTuBenhVien.NoiChiDinh.KhoaPhongId == queryString.KhoaHoanTraId)
                                   && p.YeuCauTraVatTuTuBenhNhanId == null)
                        .Select(s => new TraVatTuChiTietGridVo
                        {
                            YeuCauVatTuBenhVienId = s.YeuCauVatTuBenhVienId,
                            YeuCauTraVatTuTuBenhNhanId = s.YeuCauTraVatTuTuBenhNhanId,
                            VatTuBenhVienId = s.VatTuBenhVienId,
                            Ten = s.VatTuBenhVien.VatTus.Ten,
                            KhoTraId = s.KhoTraId,
                            NgayYeuCau = s.NgayYeuCau,
                            LaVatTuBHYT = s.LaVatTuBHYT,
                            DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                            SLDaTraLanNay = s.SoLuongTra,
                            SLChiDinh = s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0),
                            SLDaTra = s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0,
                        }).GroupBy(x => new { x.VatTuBenhVienId, x.Ten, x.LaVatTuBHYT, x.DVT })
                        .Select(item => new TraVatTuChiTietGridVo()
                        {
                            YeuCauVatTuBenhVienIds = string.Join(",", item.Select(x => x.YeuCauVatTuBenhVienId)),
                            YeuCauTraVatTuTuBenhNhanId = item.First().YeuCauTraVatTuTuBenhNhanId,
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaVatTuBHYT = item.First().LaVatTuBHYT,
                            Ten = item.First().Ten,
                            DVT = item.First().DVT,
                            KhoTraId = item.First().KhoTraId,
                            NgayYeuCau = item.First().NgayYeuCau,
                            TongSoLuongTraLanNay = item.Sum(x => x.SLDaTraLanNay),
                            TongSoLuongDaTra = item.Sum(x => x.SLDaTra),
                            TongSoLuongChiDinh = item.Sum(x => x.SLChiDinh),
                        })
                    .OrderBy(x => x.LaVatTuBHYT).ThenBy(x => x.Ten).Distinct();

                if (queryString.RangeFromDate != null &&
                                (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                }

                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);

                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                var dataReturn = queryTask.Result;
                foreach (var dataItem in dataReturn)
                {
                    dataItem.TongSoLuongDaTra = dataItem.TongSoLuongDaTra - dataItem.TongSoLuongTraLanNay;
                }
                //end update BVHD-3411

                return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };
            }
            else
            {
                var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                        .Where(p => p.KhoTraId == queryString.KhoTraId
                        && (p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.KhoaPhongDieuTriId == queryString.KhoaHoanTraId || p.YeuCauVatTuBenhVien.NoiChiDinh.KhoaPhongId == queryString.KhoaHoanTraId)
                        && p.YeuCauTraVatTuTuBenhNhanId == queryString.Id
                               )
                        .Select(s => new TraVatTuChiTietGridVo
                        {
                            YeuCauVatTuBenhVienId = s.YeuCauVatTuBenhVienId,
                            YeuCauTraVatTuTuBenhNhanId = s.YeuCauTraVatTuTuBenhNhanId,
                            VatTuBenhVienId = s.VatTuBenhVienId,
                            Ten = s.VatTuBenhVien.VatTus.Ten,
                            KhoTraId = s.KhoTraId,
                            NgayYeuCau = s.NgayYeuCau,
                            LaVatTuBHYT = s.LaVatTuBHYT,
                            DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                            SLDaTraLanNay = s.SoLuongTra,
                            SLChiDinh = s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0),
                            SLDaTra = s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0,
                            DuocDuyet = s.YeuCauTraVatTuTuBenhNhan.DuocDuyet
                        }).GroupBy(x => new { x.VatTuBenhVienId, x.Ten, x.LaVatTuBHYT, x.DVT })
                        .Select(item => new TraVatTuChiTietGridVo()
                        {
                            YeuCauVatTuBenhVienIds = string.Join(",", item.Select(x => x.YeuCauVatTuBenhVienId)),
                            YeuCauTraVatTuTuBenhNhanId = item.First().YeuCauTraVatTuTuBenhNhanId,
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaVatTuBHYT = item.First().LaVatTuBHYT,
                            Ten = item.First().Ten,
                            DVT = item.First().DVT,
                            DuocDuyet = item.First().DuocDuyet,
                            KhoTraId = item.First().KhoTraId,
                            NgayYeuCau = item.First().NgayYeuCau,
                            TongSoLuongTraLanNay = item.Sum(x => x.SLDaTraLanNay),
                            TongSoLuongDaTra = item.Sum(x => x.SLDaTra),
                            TongSoLuongChiDinh = item.Sum(x => x.SLChiDinh),
                        })
                    .OrderBy(x => x.LaVatTuBHYT).ThenBy(x => x.Ten).Distinct();

                if (queryString.RangeFromDate != null &&
                                (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                }

                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);

                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                var dataReturn = queryTask.Result;
                foreach (var dataItem in dataReturn)
                {
                    if (dataItem.DuocDuyet != true)
                    {
                        dataItem.TongSoLuongDaTra = dataItem.TongSoLuongDaTra - dataItem.TongSoLuongTraLanNay;
                    }
                }
                //end update BVHD-3411

                return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };
            }


        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncVatTuTheoKho(QueryInfo queryInfo)
        {
            return null;
        }

        //Benh Nhan
        public async Task<GridDataSource> GetDataForGridAsyncBenhNhanChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTraVatTuTuBenhNhanId = long.Parse(queryObj[0]);
            var vatTuBenhVienId = long.Parse(queryObj[1]);
            var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraVatTuTuBenhNhanId == yeuCauTraVatTuTuBenhNhanId && p.VatTuBenhVienId == vatTuBenhVienId)
                      .Select(s => new TraVatTuBenhNhanChiTietGridVo
                      {
                          Id = s.Id,
                          YeuCauTiepNhanId = s.YeuCauVatTuBenhVien.YeuCauTiepNhanId,
                          NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0),
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0,
                          TiLeTheoThapGia = s.YeuCauVatTuBenhVien.TiLeTheoThapGia,
                          VAT = s.YeuCauVatTuBenhVien.VAT,
                          DonGiaNhap = s.YeuCauVatTuBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauVatTuBenhVien.DonGiaBan,
                          BenhNhan = "MÃ TN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan + "</b> - " + "MÃ BN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN + "</b> - " + " HỌ TÊN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen + "</b>" ,
                          KhongTinhPhi = s.YeuCauVatTuBenhVien.KhongTinhPhi,
                          DuocDuyet = s.YeuCauTraVatTuTuBenhNhan.DuocDuyet
                      });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(p => p.YeuCauTiepNhanId).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
            var dataReturn = queryTask.Result;
            foreach (var dataItem in dataReturn)
            {
                if (dataItem.DuocDuyet != true)
                {
                    dataItem.SoLuongDaTra = dataItem.SoLuongDaTra - dataItem.SoLuongTraLanNay;
                }
            }
            foreach (var dataItem in dataReturn)
            {
                dataItem.BenhNhan = dataItem.BenhNhan + " - SL CHỈ ĐỊNH: <b>" + dataReturn.Where(o => o.YeuCauTiepNhanId == dataItem.YeuCauTiepNhanId).Select(o => o.SoLuongChiDinh.GetValueOrDefault()).Sum() + "</b>";
                dataItem.BenhNhan = dataItem.BenhNhan + " - SL ĐÃ TRẢ: <b>" + dataReturn.Where(o => o.YeuCauTiepNhanId == dataItem.YeuCauTiepNhanId).Select(o => o.SoLuongDaTra.GetValueOrDefault()).Sum() + "</b>";
            }
            //end update BVHD-3411

            return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncBenhNhanChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTraVatTuTuBenhNhanId = long.Parse(queryObj[0]);
            var VatTuBenhVienId = long.Parse(queryObj[1]);
            var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraVatTuTuBenhNhanId == yeuCauTraVatTuTuBenhNhanId && p.VatTuBenhVienId == VatTuBenhVienId)
                      .Select(s => new TraVatTuBenhNhanChiTietGridVo
                      {
                          Id = s.Id,
                          YeuCauTiepNhanId = s.YeuCauVatTuBenhVien.YeuCauTiepNhanId,
                          NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0),
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauVatTuBenhVien.SoLuongDaTra,
                          TiLeTheoThapGia = s.YeuCauVatTuBenhVien.TiLeTheoThapGia,
                          VAT = s.YeuCauVatTuBenhVien.VAT,
                          DonGiaNhap = s.YeuCauVatTuBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauVatTuBenhVien.DonGiaBan,
                          BenhNhan = "MÃ TN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan + "</b> - " + "MÃ BN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN + "</b> - " + " HỌ TÊN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen + "</b>" + " - SL CHỈ ĐỊNH: <b>" + (s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0)) + "</b>" + " - SL ĐÃ TRẢ: <b>" + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0) + "</b>",
                          KhongTinhPhi = s.YeuCauVatTuBenhVien.KhongTinhPhi
                      });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataForGridAsyncBenhNhanTheoKhoChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTraVatTuTuBenhNhanId = long.Parse(queryObj[0]);
            var vatTuBenhVienId = long.Parse(queryObj[1]);
            var isCreate = bool.Parse(queryObj[2]);
            var khoTraId = long.Parse(queryObj[3]);
            var laVatTuBHYT = bool.Parse(queryObj[4]);
            var khoaHoanTraId = long.Parse(queryObj[5]);
            if (isCreate)
            {
                var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraVatTuTuBenhNhanId == null
                && p.VatTuBenhVienId == vatTuBenhVienId
                && p.KhoTraId == khoTraId
                && p.LaVatTuBHYT == laVatTuBHYT
                && (p.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.KhoaPhongDieuTriId == khoaHoanTraId || p.YeuCauVatTuBenhVien.NoiChiDinh.KhoaPhongId == khoaHoanTraId))
                      .Select(s => new TraVatTuBenhNhanChiTietGridVo
                      {
                          Id = s.Id,
                          YeuCauTiepNhanId = s.YeuCauVatTuBenhVien.YeuCauTiepNhanId,
                          NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0),
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0,
                          TiLeTheoThapGia = s.YeuCauVatTuBenhVien.TiLeTheoThapGia,
                          VAT = s.YeuCauVatTuBenhVien.VAT,
                          DonGiaNhap = s.YeuCauVatTuBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauVatTuBenhVien.DonGiaBan,
                          BenhNhan = "MÃ TN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan + "</b> - " + "MÃ BN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN + "</b> - " + " HỌ TÊN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen + "</b>" ,
                          KhongTinhPhi = s.YeuCauVatTuBenhVien.KhongTinhPhi
                      });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(p => p.YeuCauTiepNhanId).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);

                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                var dataReturn = queryTask.Result;
                foreach (var dataItem in dataReturn)
                {
                    dataItem.SoLuongDaTra = dataItem.SoLuongDaTra - dataItem.SoLuongTraLanNay;
                }
                foreach (var dataItem in dataReturn)
                {
                    dataItem.BenhNhan = dataItem.BenhNhan + " - SL CHỈ ĐỊNH: <b>" + dataReturn.Where(o => o.YeuCauTiepNhanId == dataItem.YeuCauTiepNhanId).Select(o => o.SoLuongChiDinh.GetValueOrDefault()).Sum() + "</b>";
                    dataItem.BenhNhan = dataItem.BenhNhan + " - SL ĐÃ TRẢ: <b>" + dataReturn.Where(o => o.YeuCauTiepNhanId == dataItem.YeuCauTiepNhanId).Select(o => o.SoLuongDaTra.GetValueOrDefault()).Sum() + "</b>";
                }
                //end update BVHD-3411

                return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };
            }
            else
            {
                var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraVatTuTuBenhNhanId == yeuCauTraVatTuTuBenhNhanId
                && p.VatTuBenhVienId == vatTuBenhVienId
                && p.KhoTraId == khoTraId
                && p.LaVatTuBHYT == laVatTuBHYT)
                      .Select(s => new TraVatTuBenhNhanChiTietGridVo
                      {
                          Id = s.Id,
                          YeuCauTiepNhanId = s.YeuCauVatTuBenhVien.YeuCauTiepNhanId,
                          NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauVatTuBenhVien.SoLuong + (s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0),
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauVatTuBenhVien.SoLuongDaTra ?? 0,
                          TiLeTheoThapGia = s.YeuCauVatTuBenhVien.TiLeTheoThapGia,
                          VAT = s.YeuCauVatTuBenhVien.VAT,
                          DonGiaNhap = s.YeuCauVatTuBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauVatTuBenhVien.DonGiaBan,
                          BenhNhan = "MÃ TN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan + "</b> - " + "MÃ BN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN + "</b> - " + " HỌ TÊN: <b>" + s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen + "</b>" ,
                          KhongTinhPhi = s.YeuCauVatTuBenhVien.KhongTinhPhi,
                          DuocDuyet = s.YeuCauTraVatTuTuBenhNhan.DuocDuyet
                      });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(p => p.YeuCauTiepNhanId).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);

                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                var dataReturn = queryTask.Result;
                foreach (var dataItem in dataReturn)
                {
                    if (dataItem.DuocDuyet != true)
                    {
                        dataItem.SoLuongDaTra = dataItem.SoLuongDaTra - dataItem.SoLuongTraLanNay;
                    }
                }
                foreach (var dataItem in dataReturn)
                {
                    dataItem.BenhNhan = dataItem.BenhNhan + " - SL CHỈ ĐỊNH: <b>" + dataReturn.Where(o => o.YeuCauTiepNhanId == dataItem.YeuCauTiepNhanId).Select(o => o.SoLuongChiDinh.GetValueOrDefault()).Sum() + "</b>";
                    dataItem.BenhNhan = dataItem.BenhNhan + " - SL ĐÃ TRẢ: <b>" + dataReturn.Where(o => o.YeuCauTiepNhanId == dataItem.YeuCauTiepNhanId).Select(o => o.SoLuongDaTra.GetValueOrDefault()).Sum() + "</b>";
                }
                //end update BVHD-3411

                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }

        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncBenhNhanTheoKhoChild(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<List<LookupItemVo>> GetKhoVatTuCap2(DropDownListRequestModel queryInfo)
        {
            var result = _khoRepository.TableNoTracking
                        .Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && p.LoaiVatTu == true)
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.Id,
                            DisplayName = s.Ten
                        })
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<TrangThaiDuyetVo> GetTrangThaiPhieuTraVatTu(long phieuTraId)
        {
            var duocDuyet = await BaseRepository.TableNoTracking.Where(p => p.Id == phieuTraId).Select(p => p.DuocDuyet).FirstAsync();
            var trangThaiVo = new TrangThaiDuyetVo();
            if (duocDuyet == true)
            {
                trangThaiVo.TrangThai = true;
                trangThaiVo.Ten = "Đã duyệt";
                return trangThaiVo;
            }
            else if (duocDuyet == false)
            {
                trangThaiVo.TrangThai = false;
                trangThaiVo.Ten = "Từ chối duyệt";
                return trangThaiVo;
            }
            else
            {
                trangThaiVo.TrangThai = null;
                trangThaiVo.Ten = "Đang chờ duyệt";
                return trangThaiVo;
            }
        }

        public async Task<List<YeuCauTraVatTuTuBenhNhanChiTiet>> YeuCauTraVatTuTuBenhNhanChiTiets(long yeuCauVatTuBenhVienId, long vatTuBenhVienId)
        {
            var yeuCauTraVatTuTuBenhNhanChiTiet = _yeuCauTraVatTuTuBenhNhanChiTietRepository.Table.Where(p => p.YeuCauVatTuBenhVienId == yeuCauVatTuBenhVienId && p.VatTuBenhVienId == vatTuBenhVienId && p.YeuCauTraVatTuTuBenhNhanId == null);
            return await yeuCauTraVatTuTuBenhNhanChiTiet.ToListAsync();
        }

        public async Task XoaPhieuTraVatTu(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan entity)
        {
            foreach (var item in entity.YeuCauTraVatTuTuBenhNhanChiTiets)
            {
                item.YeuCauTraVatTuTuBenhNhanId = null;
            }
            entity.WillDelete = true;
            await BaseRepository.UpdateAsync(entity);
        }

        public string InPhieuYeuCauTraVatTuTuBenhNhan(PhieuTraVatTu phieuTraVatTu)
        {
            {
                var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
                var content = string.Empty;
                var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuHoanTraDuocPhamBenhNhan"));

                var hoanTraVatTuChiTiets = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
                                              .Where(p => p.YeuCauTraVatTuTuBenhNhanId == phieuTraVatTu.YeuCauTraVatTuTuBenhNhanId)
                                              .Select(s => new PhieuHoanTraVatTuTuBenhNhanChiTietData
                                              {
                                                  SoPhieu = s.YeuCauTraVatTuTuBenhNhan.SoPhieu,
                                                  KhoaTraLai = s.YeuCauTraVatTuTuBenhNhan.KhoaHoanTra.Ten,
                                                  KhoNhan = s.KhoTra.Ten,
                                                  HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                                                  Ten = s.VatTuBenhVien.VatTus.Ten,
                                                  SoLo = s.YeuCauVatTuBenhVien.SoHopDongThau,
                                                  NuocSX = s.VatTuBenhVien.VatTus.NuocSanXuat,
                                                  HanSuDung = s.YeuCauVatTuBenhVien.VatTuBenhVien.NhapKhoVatTuChiTiets
                                                        .Where(nkct => nkct.NhapKhoVatTu.KhoId == s.KhoTraId
                                                            && nkct.VatTuBenhVienId == s.VatTuBenhVienId
                                                            && nkct.LaVatTuBHYT == s.LaVatTuBHYT)
                                                       .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                                                    .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                    .Select(o => o.HanSuDung).FirstOrDefault(),
                                                  DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                  SoLuong = s.SoLuongTra,
                                                  DonGiaNhap = s.YeuCauVatTuBenhVien.DonGiaNhap,
                                                  DonGia = s.YeuCauVatTuBenhVien.DonGiaBan,
                                                  VAT = s.YeuCauVatTuBenhVien.VAT,
                                                  TiLeTheoThapGia = s.YeuCauVatTuBenhVien.TiLeTheoThapGia,
                                                  NgayYeuCau = s.NgayYeuCau,
                                                  CreatedOn = s.YeuCauTraVatTuTuBenhNhan.CreatedOn,
                                                  KhongTinhPhi = s.YeuCauVatTuBenhVien.KhongTinhPhi
                                              }).OrderBy(p => p.HoTen).ToList();
                var thuocChiTiet = string.Empty;
                var STT = 1;
                decimal thanhTienKhong = 0;
                foreach (var item in hoanTraVatTuChiTiets)
                {
                    thuocChiTiet +=
                                               "<tr style = 'border: 1px solid #020000;'>"
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HoTen
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TenVatTu
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + (item.KhongTinhPhi != true ? item.ThanhTien.ApplyFormatMoneyVND() : thanhTienKhong.ApplyFormatMoneyVND())
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                               + "</tr>";
                    STT++;
                }
                var tongThanhTien = hoanTraVatTuChiTiets.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
                var tuNgay = hoanTraVatTuChiTiets.OrderBy(p => p.NgayYeuCau).Select(p => p.NgayYeuCau).First();
                var denNgay = hoanTraVatTuChiTiets.OrderByDescending(p => p.NgayYeuCau).Select(p => p.NgayYeuCau).First();

                thuocChiTiet += "<tr>"
                                    + "<td colspan='3'><b>Tổng cộng: </b></td>"
                                    + "<td colspan='4' style = 'text-align: right; padding-right:3px'>" + tongThanhTien + "</td>"
                                    + "<td>&nbsp;</td>"
                               + "</tr>";
                var data = new PhieuHoanTraVatTuTuBenhNhanData
                {
                    SoPhieu = hoanTraVatTuChiTiets.First().SoPhieu,
                    KhoaTraLai = hoanTraVatTuChiTiets.First().KhoaTraLai,
                    KhoNhan = hoanTraVatTuChiTiets.First().KhoNhan,
                    BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraVatTuChiTiets.First().SoPhieu),
                    NgayLapPhieu = hoanTraVatTuChiTiets.First().CreatedOn?.ApplyFormatDate(),
                    ThuocVatTu = thuocChiTiet,
                    LogoUrl = phieuTraVatTu.HostingName + "/assets/img/logo-bacha-full.png",
                    TuNgay = tuNgay.ApplyFormatDate(),
                    DenNgay = denNgay.ApplyFormatDate(),
                    //Ngay = DateTime.Now.Day.ConvertDateToString(),
                    //Thang = DateTime.Now.Month.ConvertMonthToString(),
                    //Nam = DateTime.Now.Year.ConvertYearToString()
                    NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                return content;
            }
        }
    }
}
