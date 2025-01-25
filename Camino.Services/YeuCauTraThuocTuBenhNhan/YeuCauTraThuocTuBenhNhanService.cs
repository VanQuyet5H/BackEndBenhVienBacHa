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
using Camino.Core.Domain.Entities.KhoDuocPhams;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Services.CauHinh;
using Camino.Services.YeuCauKhamBenh;

namespace Camino.Services.YeuCauTraThuocTuBenhNhan
{
    [ScopedDependency(ServiceType = typeof(IYeuCauTraThuocTuBenhNhanService))]

    public class YeuCauTraThuocTuBenhNhanService : MasterFileService<YeuCauTraDuocPhamTuBenhNhan>, IYeuCauTraThuocTuBenhNhanService
    {
        public IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        public IRepository<Core.Domain.Entities.Users.User> _userRepository;
        public IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        public IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        public IRepository<Kho> _khoRepository;
        public IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        public IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> _yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Template> _templateRepository;
        private readonly ICauHinhService _cauHinhService;

        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;

        public YeuCauTraThuocTuBenhNhanService(IRepository<YeuCauTraDuocPhamTuBenhNhan> repository,
           IRepository<Core.Domain.Entities.Users.User> userRepository,
           IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
           IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
           IUserAgentHelper userAgentHelper,
           IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
           IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> yeuCauTraDuocPhamTuBenhNhanChiTietRepository,
           IRepository<Kho> khoRepository,
           IRepository<Template> templateRepository,
           ICauHinhService cauHinhService,
           IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
           IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
           IYeuCauKhamBenhService yeuCauKhamBenhService) : base(repository)
        {
            _nhanVienRepository = nhanVienRepository;
            _userRepository = userRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _yeuCauTraDuocPhamTuBenhNhanChiTietRepository = yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
            _userAgentHelper = userAgentHelper;
            _phongBenhVienRepository = phongBenhVienRepository;
            _khoRepository = khoRepository;
            _templateRepository = templateRepository;
            _cauHinhService = cauHinhService;
            _cauHinhRepository = cauHinhRepository;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
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
                .Select(s => new YeuCauTraThuocTuBenhNhanGridVo
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

                var queryString = JsonConvert.DeserializeObject<YeuCauTraThuocTuBenhNhanGridVo>(queryInfo.AdditionalSearchString);
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
                 .Select(s => new YeuCauTraThuocTuBenhNhanGridVo
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

                var queryString = JsonConvert.DeserializeObject<YeuCauTraThuocTuBenhNhanGridVo>(queryInfo.AdditionalSearchString);
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

        //Duoc Pham
        public async Task<GridDataSource> GetDataForGridAsyncDuocPhamChild(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var yeuCauTraDuocPhamTuBenhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                        .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == yeuCauTraDuocPhamTuBenhNhanId)
                        .Select(s => new TraDuocPhamChiTietGridVo
                        {
                            YeuCauTraDuocPhamTuBenhNhanId = s.YeuCauTraDuocPhamTuBenhNhanId,
                            DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                            Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                            HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                            LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                            DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            SLDaTraLanNay = s.SoLuongTra,
                            SLChiDinh = s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0),
                            SLDaTra = s.YeuCauDuocPhamBenhVien.SoLuongDaTra.GetValueOrDefault(),
                            DuocDuyet = s.YeuCauTraDuocPhamTuBenhNhan.DuocDuyet
                        }).GroupBy(x => new { x.YeuCauTraDuocPhamTuBenhNhanId, x.DuocPhamBenhVienId, x.Ten, x.LaDuocPhamBHYT, x.HoatChat })
                        .Select(item => new TraDuocPhamChiTietGridVo()
                        {
                            YeuCauTraDuocPhamTuBenhNhanId = item.First().YeuCauTraDuocPhamTuBenhNhanId,
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                            Ten = item.First().Ten,
                            HoatChat = item.First().HoatChat,
                            DVT = item.First().DVT,
                            DuocDuyet = item.First().DuocDuyet,
                            TongSoLuongTraLanNay = item.Sum(x => x.SLDaTraLanNay),
                            TongSoLuongDaTra = item.Sum(x => x.SLDaTra),
                            TongSoLuongChiDinh = item.Sum(x => x.SLChiDinh),
                        })
                    .OrderBy(x => x.LaDuocPhamBHYT).ThenBy(x => x.Ten).Distinct();

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
        public async Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamChild(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var yeuCauTraDuocPhamTuBenhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                        .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == yeuCauTraDuocPhamTuBenhNhanId)
                        .Select(s => new TraDuocPhamChiTietGridVo
                        {
                            DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                            Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                            HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                            LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                            DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            SLDaTraLanNay = s.SoLuongTra,
                            SLChiDinh = s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0),
                            SLDaTra = s.YeuCauDuocPhamBenhVien.SoLuongDaTra.GetValueOrDefault()
                        }).GroupBy(x => new { x.DuocPhamBenhVienId, x.Ten, x.LaDuocPhamBHYT, x.HoatChat })
                        .Select(item => new TraDuocPhamChiTietGridVo()
                        {
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                            Ten = item.First().Ten,
                            HoatChat = item.First().HoatChat,
                            DVT = item.First().DVT,
                            TongSoLuongTraLanNay = item.Sum(x => x.SLDaTraLanNay),
                            TongSoLuongDaTra = item.First().SLDaTra,
                            TongSoLuongChiDinh = item.First().SLChiDinh,
                        })
                    .OrderBy(x => x.LaDuocPhamBHYT).ThenBy(x => x.Ten).Distinct();
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataForGridAsyncDuocPhamTheoKho(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryString = JsonConvert.DeserializeObject<TraDuocPhamChiTietGridVo>(queryInfo.AdditionalSearchString);
            if (queryString.IsCreate)
            {
                var queryRange = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking;
                if (queryString.RangeFromDate != null &&
                    (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    queryRange = queryRange.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                }

                var query = queryRange
                        .Where(p => p.KhoTraId == queryString.KhoTraId
                                   && (p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.KhoaPhongDieuTriId == queryString.KhoaHoanTraId || p.YeuCauDuocPhamBenhVien.NoiChiDinh.KhoaPhongId == queryString.KhoaHoanTraId)
                                   && p.YeuCauTraDuocPhamTuBenhNhanId == null)
                        .Select(s => new TraDuocPhamChiTietGridVo
                        {
                            YeuCauDuocPhamBenhVienId = s.YeuCauDuocPhamBenhVienId,
                            YeuCauTraDuocPhamTuBenhNhanId = s.YeuCauTraDuocPhamTuBenhNhanId,
                            DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                            Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                            HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                            KhoTraId = s.KhoTraId,
                            NgayYeuCau = s.NgayYeuCau,
                            LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                            DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            SLDaTraLanNay = s.SoLuongTra,
                            SLChiDinh = s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0),
                            SLDaTra = s.YeuCauDuocPhamBenhVien.SoLuongDaTra.GetValueOrDefault()
                        }).GroupBy(x => new { x.DuocPhamBenhVienId, x.Ten, x.LaDuocPhamBHYT, x.HoatChat })
                        .Select(item => new TraDuocPhamChiTietGridVo()
                        {
                            YeuCauDuocPhamBenhVienIds = string.Join(",", item.Select(x => x.YeuCauDuocPhamBenhVienId)),
                            YeuCauTraDuocPhamTuBenhNhanId = item.First().YeuCauTraDuocPhamTuBenhNhanId,
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                            Ten = item.First().Ten,
                            HoatChat = item.First().HoatChat,
                            DVT = item.First().DVT,
                            KhoTraId = item.First().KhoTraId,
                            NgayYeuCau = item.First().NgayYeuCau,
                            TongSoLuongTraLanNay = item.Sum(x => x.SLDaTraLanNay),
                            TongSoLuongDaTra = item.Sum(x => x.SLDaTra),
                            TongSoLuongChiDinh = item.Sum(x => x.SLChiDinh),
                        })
                    .OrderBy(x => x.LaDuocPhamBHYT).ThenBy(x => x.Ten).Distinct();

                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                var dataReturn = queryTask.Result;
                foreach(var dataItem in dataReturn)
                {
                    dataItem.TongSoLuongDaTra = dataItem.TongSoLuongDaTra - dataItem.TongSoLuongTraLanNay;
                }
                //end update BVHD-3411

                return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };
            }
            else
            {
                var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                        .Where(p => p.KhoTraId == queryString.KhoTraId
                        && (p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.KhoaPhongDieuTriId == queryString.KhoaHoanTraId || p.YeuCauDuocPhamBenhVien.NoiChiDinh.KhoaPhongId == queryString.KhoaHoanTraId)
                        && p.YeuCauTraDuocPhamTuBenhNhanId == queryString.Id
                               )
                        .Select(s => new TraDuocPhamChiTietGridVo
                        {
                            YeuCauDuocPhamBenhVienId = s.YeuCauDuocPhamBenhVienId,
                            YeuCauTraDuocPhamTuBenhNhanId = s.YeuCauTraDuocPhamTuBenhNhanId,
                            DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                            Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                            HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                            KhoTraId = s.KhoTraId,
                            NgayYeuCau = s.NgayYeuCau,
                            LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                            DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            SLDaTraLanNay = s.SoLuongTra,
                            SLChiDinh = s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0),
                            SLDaTra = s.YeuCauDuocPhamBenhVien.SoLuongDaTra.GetValueOrDefault(),
                            DuocDuyet = s.YeuCauTraDuocPhamTuBenhNhan.DuocDuyet
                        }).GroupBy(x => new { x.DuocPhamBenhVienId, x.Ten, x.LaDuocPhamBHYT, x.HoatChat })
                        .Select(item => new TraDuocPhamChiTietGridVo()
                        {
                            YeuCauDuocPhamBenhVienIds = string.Join(",", item.Select(x => x.YeuCauDuocPhamBenhVienId)),
                            YeuCauTraDuocPhamTuBenhNhanId = item.First().YeuCauTraDuocPhamTuBenhNhanId,
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                            Ten = item.First().Ten,
                            HoatChat = item.First().HoatChat,
                            DVT = item.First().DVT,
                            KhoTraId = item.First().KhoTraId,
                            NgayYeuCau = item.First().NgayYeuCau,
                            DuocDuyet = item.First().DuocDuyet,
                            TongSoLuongTraLanNay = item.Sum(x => x.SLDaTraLanNay),
                            TongSoLuongDaTra = item.Sum(x => x.SLDaTra),
                            TongSoLuongChiDinh = item.Sum(x => x.SLChiDinh),
                        })
                    .OrderBy(x => x.LaDuocPhamBHYT).ThenBy(x => x.Ten).Distinct();
                //da lay theo YeuCauTraDuocPhamTuBenhNhanId
                //if (queryString.RangeFromDate != null &&
                //                (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                //{
                //    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                //    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                //    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                //    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayYeuCau >= tuNgay)
                //                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayYeuCau <= denNgay));
                //}

                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);

                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                var dataReturn = queryTask.Result;
                foreach (var dataItem in dataReturn)
                {
                    if(dataItem.DuocDuyet != true)
                    {
                        dataItem.TongSoLuongDaTra = dataItem.TongSoLuongDaTra - dataItem.TongSoLuongTraLanNay;
                    }                    
                }
                //end update BVHD-3411

                return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };
            }


        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamTheoKho(QueryInfo queryInfo)
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
            var yeuCauTraDuocPhamTuBenhNhanId = long.Parse(queryObj[0]);
            var duocPhamBenhVienId = long.Parse(queryObj[1]);
            var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == yeuCauTraDuocPhamTuBenhNhanId && p.DuocPhamBenhVienId == duocPhamBenhVienId)
                      .Select(s => new TraDuocPhamBenhNhanChiTietGridVo
                      {
                          Id = s.Id,
                          YeuCauTiepNhanId = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                          NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0),
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0,
                          TiLeTheoThapGia = s.YeuCauDuocPhamBenhVien.TiLeTheoThapGia,
                          VAT = s.YeuCauDuocPhamBenhVien.VAT,
                          DonGiaNhap = s.YeuCauDuocPhamBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauDuocPhamBenhVien.DonGiaBan,
                          BenhNhan = "MÃ TN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan + "</b> - " + "MÃ BN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN + "</b> - " + " HỌ TÊN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen + "</b>" ,
                          KhongTinhPhi = s.YeuCauDuocPhamBenhVien.KhongTinhPhi,
                          DuocDuyet = s.YeuCauTraDuocPhamTuBenhNhan.DuocDuyet
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
            var yeuCauTraDuocPhamTuBenhNhanId = long.Parse(queryObj[0]);
            var duocPhamBenhVienId = long.Parse(queryObj[1]);
            var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == yeuCauTraDuocPhamTuBenhNhanId && p.DuocPhamBenhVienId == duocPhamBenhVienId)
                      .Select(s => new TraDuocPhamBenhNhanChiTietGridVo
                      {
                          Id = s.Id,
                          YeuCauTiepNhanId = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                          NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0),
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauDuocPhamBenhVien.SoLuongDaTra,
                          TiLeTheoThapGia = s.YeuCauDuocPhamBenhVien.TiLeTheoThapGia,
                          VAT = s.YeuCauDuocPhamBenhVien.VAT,
                          DonGiaNhap = s.YeuCauDuocPhamBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauDuocPhamBenhVien.DonGiaBan,
                          BenhNhan = "MÃ TN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan + "</b> - " + "MÃ BN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN + "</b> - " + " HỌ TÊN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen + "</b>" + " - SL CHỈ ĐỊNH: <b>" + (s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0)) + "</b>" + " - SL ĐÃ TRẢ: <b>" + s.YeuCauDuocPhamBenhVien.SoLuongDaTra.GetValueOrDefault() + "</b>",
                          KhongTinhPhi = s.YeuCauDuocPhamBenhVien.KhongTinhPhi
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
            var yeuCauTraDuocPhamTuBenhNhanId = long.Parse(queryObj[0]);
            var duocPhamBenhVienId = long.Parse(queryObj[1]);
            var isCreate = bool.Parse(queryObj[2]);
            var khoTraId = long.Parse(queryObj[3]);
            var laDuocPhamBHYT = bool.Parse(queryObj[4]);
            var khoaHoanTraId = long.Parse(queryObj[5]);

            if (isCreate)
            {
                var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == null
                            && p.DuocPhamBenhVienId == duocPhamBenhVienId
                            && p.KhoTraId == khoTraId
                            && p.LaDuocPhamBHYT == laDuocPhamBHYT
                            && (p.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.KhoaPhongDieuTriId == khoaHoanTraId || p.YeuCauDuocPhamBenhVien.NoiChiDinh.KhoaPhongId == khoaHoanTraId))
                      .Select(s => new TraDuocPhamBenhNhanChiTietGridVo
                      {
                          Id = s.Id,
                          YeuCauTiepNhanId = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                          NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0),
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0,
                          TiLeTheoThapGia = s.YeuCauDuocPhamBenhVien.TiLeTheoThapGia,
                          VAT = s.YeuCauDuocPhamBenhVien.VAT,
                          DonGiaNhap = s.YeuCauDuocPhamBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauDuocPhamBenhVien.DonGiaBan,
                          BenhNhan = "MÃ TN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan + "</b> - " + "MÃ BN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN + "</b> - " + " HỌ TÊN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen + "</b>" ,
                          KhongTinhPhi = s.YeuCauDuocPhamBenhVien.KhongTinhPhi
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
                    dataItem.BenhNhan = dataItem.BenhNhan +" - SL CHỈ ĐỊNH: <b>" + dataReturn.Where(o=>o.YeuCauTiepNhanId == dataItem.YeuCauTiepNhanId).Select(o=>o.SoLuongChiDinh.GetValueOrDefault()).Sum() + "</b>";                    
                    dataItem.BenhNhan = dataItem.BenhNhan + " - SL ĐÃ TRẢ: <b>" + dataReturn.Where(o => o.YeuCauTiepNhanId == dataItem.YeuCauTiepNhanId).Select(o => o.SoLuongDaTra.GetValueOrDefault()).Sum() + "</b>";
                }
                //end update BVHD-3411

                return new GridDataSource { Data = dataReturn, TotalRowCount = countTask.Result };
            }
            else
            {
                var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == yeuCauTraDuocPhamTuBenhNhanId
                            && p.DuocPhamBenhVienId == duocPhamBenhVienId
                            && p.KhoTraId == khoTraId
                            && p.LaDuocPhamBHYT == laDuocPhamBHYT)
                      .Select(s => new TraDuocPhamBenhNhanChiTietGridVo
                      {
                          Id = s.Id,
                          YeuCauTiepNhanId = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                          NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri,
                          NgayTra = s.NgayYeuCau,
                          NhanVienYeuCau = s.NhanVienYeuCau.User.HoTen,
                          SoLuongChiDinh = s.YeuCauDuocPhamBenhVien.SoLuong + (s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0),
                          SoLuongTraLanNay = s.SoLuongTra,
                          SoLuongDaTra = s.YeuCauDuocPhamBenhVien.SoLuongDaTra ?? 0,
                          TiLeTheoThapGia = s.YeuCauDuocPhamBenhVien.TiLeTheoThapGia,
                          VAT = s.YeuCauDuocPhamBenhVien.VAT,
                          DonGiaNhap = s.YeuCauDuocPhamBenhVien.DonGiaNhap,
                          DonGia = s.YeuCauDuocPhamBenhVien.DonGiaBan,
                          BenhNhan = "MÃ TN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan + "</b> - " + "MÃ BN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN + "</b> - " + " HỌ TÊN: <b>" + s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen + "</b>" ,
                          KhongTinhPhi = s.YeuCauDuocPhamBenhVien.KhongTinhPhi,
                          DuocDuyet = s.YeuCauTraDuocPhamTuBenhNhan.DuocDuyet
                      });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(p => p.YeuCauTiepNhanId).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);

                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                var dataReturn = queryTask.Result;
                foreach (var dataItem in dataReturn)
                {
                    if(dataItem.DuocDuyet != true)
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

        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncBenhNhanTheoKhoChild(QueryInfo queryInfo)
        {
            return null;
        }


        public async Task<List<YeuCauTraDuocPhamTuBenhNhanChiTiet>> YeuCauTraDuocPhamTuBenhNhanChiTiet(long yeuCauDuocPhamBenhVienId, long duocPhamBenhVienId)
        {
            var yeuCauTraDuocPhamTuBenhNhanChiTiet = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.Table.Where(p => p.YeuCauDuocPhamBenhVienId == yeuCauDuocPhamBenhVienId && p.DuocPhamBenhVienId == duocPhamBenhVienId && p.YeuCauTraDuocPhamTuBenhNhanId == null);
            return await yeuCauTraDuocPhamTuBenhNhanChiTiet.ToListAsync();
        }

        public async Task<ThongTinKhoaHoanTra> ThongTinKhoaHoanTra()
        {
            var nhanVienYeuCauId = _userAgentHelper.GetCurrentUserId();
            var tenNhanVienYeuCau = _userRepository.TableNoTracking.Where(p => p.Id == nhanVienYeuCauId).Select(p => p.HoTen).FirstOrDefault();
            var noiLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoaPhongId = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == noiLamViecId).Select(p => p.KhoaPhongId).First();
            var tenKhoaPhong = await _khoaPhongRepository.TableNoTracking.Where(p => p.Id == khoaPhongId).Select(p => p.Ten).FirstAsync();
            var thongTinKhoaHoanTra = new ThongTinKhoaHoanTra
            {
                KhoaHoanTraId = khoaPhongId,
                TenKhoaTra = tenKhoaPhong,
                NhanVienYeuCauId = nhanVienYeuCauId,
                TenNhanVienYeuCau = tenNhanVienYeuCau
            };
            return thongTinKhoaHoanTra;
        }
        public async Task<List<LookupItemVo>> GetKhoDuocPhamCap2(DropDownListRequestModel queryInfo)
        {
            var result = _khoRepository.TableNoTracking
                        .Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && p.LoaiDuocPham == true)
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.Id,
                            DisplayName = s.Ten
                        })
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetKhoaPhong(DropDownListRequestModel queryInfo)
        {
            var result = _khoaPhongRepository.TableNoTracking
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.Id,
                            DisplayName = s.Ten
                        })
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<TrangThaiDuyetVo> GetTrangThaiPhieuTraDuocPham(long phieuTraId)
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

        public async Task XoaPhieuTraThuoc(YeuCauTraDuocPhamTuBenhNhan entity)
        {
            foreach (var item in entity.YeuCauTraDuocPhamTuBenhNhanChiTiets)
            {
                item.YeuCauTraDuocPhamTuBenhNhanId = null;
            }
            entity.WillDelete = true;
            await BaseRepository.UpdateAsync(entity);
        }

        public string InPhieuYeuCauTraThuocTuBenhNhan(PhieuTraThuoc phieuTraThuoc)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuHoanTraDuocPhamBenhNhan"));
            var templateGayNghien = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuHoanTraThuocGayNghien"));
            var headerTitile = "<div style=\'width: 100%; height: 40px; background: #005dab;color:#fff;text-align: center;font-size: 23px\'> PHIẾU HOÀN TRẢ THUỐC</div>";

            var hoanTraDuocPhamChiTiets = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                                          .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == phieuTraThuoc.YeuCauTraDuocPhamTuBenhNhanId &&
                                          (p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan))
                                          .Select(s => new PhieuHoanTraThuocTuBenhNhanChiTietData
                                          {
                                              SoPhieu = s.YeuCauTraDuocPhamTuBenhNhan.SoPhieu,
                                              KhoaTraLai = s.YeuCauTraDuocPhamTuBenhNhan.KhoaHoanTra.Ten,
                                              KhoNhan = s.KhoTra.Ten,
                                              HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                                              Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                              HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                                              SoLo = s.YeuCauDuocPhamBenhVien.SoHopDongThau,
                                              NuocSX = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                              HanSuDung = s.YeuCauDuocPhamBenhVien.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                                                    .Where(nkct => nkct.NhapKhoDuocPhams.KhoId == s.KhoTraId
                                                        && nkct.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                        && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT)
                                                   .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                                                .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                .Select(o => o.HanSuDung).FirstOrDefault(),
                                              DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                              SoLuong = s.SoLuongTra,
                                              DonGiaNhap = s.YeuCauDuocPhamBenhVien.DonGiaNhap,
                                              DonGia = s.YeuCauDuocPhamBenhVien.DonGiaBan,
                                              VAT = s.YeuCauDuocPhamBenhVien.VAT,
                                              TiLeTheoThapGia = s.YeuCauDuocPhamBenhVien.TiLeTheoThapGia,
                                              NgayYeuCau = s.NgayYeuCau,
                                              CreatedOn = s.YeuCauTraDuocPhamTuBenhNhan.CreatedOn,
                                              KhongTinhPhi = s.YeuCauDuocPhamBenhVien.KhongTinhPhi
                                          }).OrderBy(p => p.HoTen).ToList();
            var hoanTraDuocPhamChiTietsGayNghien = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                                                  .Where(p => p.YeuCauTraDuocPhamTuBenhNhanId == phieuTraThuoc.YeuCauTraDuocPhamTuBenhNhanId &&
                                                  (p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                                                  .Select(s => new PhieuHoanTraThuocTuBenhNhanChiTietData
                                                  {
                                                      SoPhieu = s.YeuCauTraDuocPhamTuBenhNhan.SoPhieu,
                                                      KhoaTraLai = s.YeuCauTraDuocPhamTuBenhNhan.KhoaHoanTra.Ten,
                                                      KhoNhan = s.KhoTra.Ten,
                                                      HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                                                      Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                                      HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                                                      SoLo = s.YeuCauDuocPhamBenhVien.SoHopDongThau,
                                                      NuocSX = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                                      HanSuDung = s.YeuCauDuocPhamBenhVien.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                                                            .Where(nkct => nkct.NhapKhoDuocPhams.KhoId == s.KhoTraId
                                                                && nkct.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                                && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT)
                                                           .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                                                        .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(o => o.HanSuDung).FirstOrDefault(),
                                                      DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                      SoLuong = s.SoLuongTra,
                                                      DonGiaNhap = s.YeuCauDuocPhamBenhVien.DonGiaNhap,
                                                      DonGia = s.YeuCauDuocPhamBenhVien.DonGiaBan,
                                                      VAT = s.YeuCauDuocPhamBenhVien.VAT,
                                                      TiLeTheoThapGia = s.YeuCauDuocPhamBenhVien.TiLeTheoThapGia,
                                                      NgayYeuCau = s.NgayYeuCau,
                                                      CreatedOn = s.YeuCauTraDuocPhamTuBenhNhan.CreatedOn,
                                                      KhongTinhPhi = s.YeuCauDuocPhamBenhVien.KhongTinhPhi
                                                  }).OrderBy(p => p.HoTen).ToList();

            var content = string.Empty;
            var contentGayNghien = string.Empty;

            if (hoanTraDuocPhamChiTiets.Any())
            {
                var thuocChiTiet = string.Empty;
                var STT = 1;
                decimal thanhTienKhong = 0;
                foreach (var item in hoanTraDuocPhamChiTiets)
                {
                    thuocChiTiet +=
                                               "<tr style = 'border: 1px solid #020000;'>"
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT 
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HoTen
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TenThuoc
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SoLuong
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + (item.KhongTinhPhi != true ? item.ThanhTien.ApplyFormatMoneyVND() : thanhTienKhong.ApplyFormatMoneyVND())
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                               + "</tr>";
                    STT++;
                }
                var tongThanhTien = hoanTraDuocPhamChiTiets.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
                var tuNgay = hoanTraDuocPhamChiTiets.OrderBy(p => p.NgayYeuCau).Select(p => p.NgayYeuCau).First();
                var denNgay = hoanTraDuocPhamChiTiets.OrderByDescending(p => p.NgayYeuCau).Select(p => p.NgayYeuCau).First();

                thuocChiTiet += "<tr>"
                                    + "<td colspan='3'><b>Tổng cộng: </b></td>"
                                    + "<td colspan='4' style = 'text-align: right; padding-right:3px'>" + tongThanhTien + "</td>"
                                    + "<td>&nbsp;</td>"
                               + "</tr>";
                var data = new PhieuHoanTraVatTuTuBenhNhanData
                {
                    SoPhieu = hoanTraDuocPhamChiTiets.First().SoPhieu,
                    KhoaTraLai = hoanTraDuocPhamChiTiets.First().KhoaTraLai,
                    KhoNhan = hoanTraDuocPhamChiTiets.First().KhoNhan,
                    BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraDuocPhamChiTiets.First().SoPhieu),
                    NgayLapPhieu = hoanTraDuocPhamChiTiets.First().CreatedOn?.ApplyFormatDate(),
                    ThuocVatTu = thuocChiTiet,
                    LogoUrl = phieuTraThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    TuNgay = tuNgay.ApplyFormatDate(),
                    DenNgay = denNgay.ApplyFormatDate(),
                    //Ngay = DateTime.Now.Day.ConvertDateToString(),
                    //Thang = DateTime.Now.Month.ConvertMonthToString(),
                    //Nam = DateTime.Now.Year.ConvertYearToString()
                    NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            }
            if (hoanTraDuocPhamChiTietsGayNghien.Any())
            {
                var thuocChiTiet = string.Empty;
                var STT = 1;
                decimal thanhTienKhong = 0;
                foreach (var item in hoanTraDuocPhamChiTietsGayNghien)
                {
                    thuocChiTiet +=
                                               "<tr style = 'border: 1px solid #020000;'>"
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HoTen
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TenThuoc
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND()
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + (item.KhongTinhPhi != true ? item.ThanhTien.ApplyFormatMoneyVND() : thanhTienKhong.ApplyFormatMoneyVND())
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GhiChu
                                               + "</tr>";
                    STT++;
                }
                var tongThanhTien = hoanTraDuocPhamChiTietsGayNghien.Sum(p => p.ThanhTien).ApplyFormatMoneyVND();
                var tuNgay = hoanTraDuocPhamChiTietsGayNghien.OrderBy(p => p.NgayYeuCau).Select(p => p.NgayYeuCau).First();
                var denNgay = hoanTraDuocPhamChiTietsGayNghien.OrderByDescending(p => p.NgayYeuCau).Select(p => p.NgayYeuCau).First();

                thuocChiTiet += "<tr>"
                                    + "<td colspan='3'><b>Tổng cộng: </b></td>"
                                    + "<td colspan='4' style = 'text-align: right; padding-right:3px'>" + tongThanhTien + "</td>"
                                    + "<td>&nbsp;</td>"
                               + "</tr>";
                var data = new PhieuHoanTraVatTuTuBenhNhanData
                {
                    SoPhieu = hoanTraDuocPhamChiTietsGayNghien.First().SoPhieu,
                    KhoaTraLai = hoanTraDuocPhamChiTietsGayNghien.First().KhoaTraLai,
                    KhoNhan = hoanTraDuocPhamChiTietsGayNghien.First().KhoNhan,
                    BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(hoanTraDuocPhamChiTietsGayNghien.First().SoPhieu),
                    NgayLapPhieu = hoanTraDuocPhamChiTietsGayNghien.First().CreatedOn?.ApplyFormatDate(),
                    ThuocVatTu = thuocChiTiet,
                    LogoUrl = phieuTraThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    TuNgay = tuNgay.ApplyFormatDate(),
                    DenNgay = denNgay.ApplyFormatDate(),
                    //Ngay = DateTime.Now.Day.ConvertDateToString(),
                    //Thang = DateTime.Now.Month.ConvertMonthToString(),
                    //Nam = DateTime.Now.Year.ConvertYearToString()
                    NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()
                };
                contentGayNghien = TemplateHelpper.FormatTemplateWithContentTemplate(templateGayNghien.Body, data);
            }
            if (!string.IsNullOrEmpty(content))
            {
                content = headerTitile + content;
            }
            if (!string.IsNullOrEmpty(contentGayNghien))
            {
                content = content + headerTitile + "<div style='break-after:page'></div>" + contentGayNghien;
            }
            return content;
        }

    }
}
