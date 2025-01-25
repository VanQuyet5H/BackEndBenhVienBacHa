using System.Collections.Generic;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Services.CauHinh;
using Camino.Core.Domain;
using Camino.Services.Helpers;
using System;
using System.Globalization;
using Camino.Core.Domain.ValueObject.YeuCauDieuChuyenDuocPhams;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.CauHinh;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.Localization;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Services.YeuCauKhamBenh;

namespace Camino.Services.XuatKhoDieuChuyenKhoNoiBoDuocPhams
{
    [ScopedDependency(ServiceType = typeof(IXuatKhoDieuChuyenKhoNoiBoDuocPhamService))]

    public class XuatKhoDieuChuyenKhoNoiBoDuocPhamService : MasterFileService<YeuCauDieuChuyenDuocPham>, IXuatKhoDieuChuyenKhoNoiBoDuocPhamService
    {
        private readonly IRepository<YeuCauDieuChuyenDuocPhamChiTiet> _yeuCauDieuChuyenDuocPhamChiTietRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Template> _templateRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;

        public XuatKhoDieuChuyenKhoNoiBoDuocPhamService(
            IRepository<YeuCauDieuChuyenDuocPham> repository,
            ICauHinhService cauHinhService,
            IRepository<Template> templateRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<YeuCauDieuChuyenDuocPhamChiTiet> yeuCauDieuChuyenDuocPhamChiTietRepository,
            IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
            ILocalizationService localizationService,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
            IRepository<LocaleStringResource> localeStringResourceRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IYeuCauKhamBenhService yeuCauKhamBenhService
            ) : base(repository)
        {
            _cauHinhService = cauHinhService;
            _yeuCauDieuChuyenDuocPhamChiTietRepository = yeuCauDieuChuyenDuocPhamChiTietRepository;
            _templateRepository = templateRepository;
            _userAgentHelper = userAgentHelper;
            _nhanVienRepository = nhanVienRepository;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _localizationService = localizationService;
            _localeStringResourceRepository = localeStringResourceRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Select(s => new YeuCauDieuChuyenDuocPhamVo
                {
                    Id = s.Id,
                    SoPhieu = s.YeuCauDieuChuyenDuocPhamChiTiets.Select(z => z.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu).FirstOrDefault(),
                    TenKhoNhap = s.KhoNhap.Ten,
                    TenKhoXuat = s.KhoXuat.Ten,
                    TenNhanVienYeuCau = s.NguoiXuat.User.HoTen,
                    NgayYeuCau = s.CreatedOn,
                    DuocKeToanDuyet = s.DuocKeToanDuyet,
                    TenNhanVienDuyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<YeuCauDieuChuyenDuocPhamVo>(queryInfo.AdditionalSearchString);
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
                         g => g.TenKhoNhap,
                         g => g.TenKhoXuat,
                         g => g.TenNhanVienYeuCau,
                         g => g.TenNhanVienDuyet
                   );
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
            var query = BaseRepository.TableNoTracking
                .Select(s => new YeuCauDieuChuyenDuocPhamVo
                {
                    Id = s.Id,
                    SoPhieu = s.YeuCauDieuChuyenDuocPhamChiTiets.Select(z => z.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu).FirstOrDefault(),
                    TenKhoNhap = s.KhoNhap.Ten,
                    TenKhoXuat = s.KhoXuat.Ten,
                    TenNhanVienYeuCau = s.NguoiXuat.User.HoTen,
                    NgayYeuCau = s.CreatedOn,
                    DuocKeToanDuyet = s.DuocKeToanDuyet,
                    TenNhanVienDuyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<YeuCauDieuChuyenDuocPhamVo>(queryInfo.AdditionalSearchString);
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
                         g => g.TenKhoNhap,
                         g => g.TenKhoXuat,
                         g => g.TenNhanVienYeuCau,
                         g => g.TenNhanVienDuyet
                   );
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
                queryInfo.Sort = new List<Sort> { new Sort { Field = "Ten", Dir = "desc" } };
            }
            var yeuCauDieuChuyenDuocPhamId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauDieuChuyenDuocPhamChiTietRepository.TableNoTracking
                .Where(z => z.YeuCauDieuChuyenDuocPhamId == yeuCauDieuChuyenDuocPhamId)
                .Select(s => new YeuCauDieuChuyenDuocPhamChiTietVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                    HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    SoLuongDieuChuyen = s.SoLuongDieuChuyen,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM"
                })
                 .GroupBy(x => new { x.DuocPhamBenhVienId, x.Ten, x.HoatChat, x.Nhom, x.DVT, x.SoLo, x.LaDuocPhamBHYT, x.HanSuDung })
                    .Select(item => new YeuCauDieuChuyenDuocPhamChiTietVo()
                    {
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                        Ten = item.First().Ten,
                        Nhom = item.First().Nhom,
                        HoatChat = item.First().HoatChat,
                        DVT = item.First().DVT,
                        SoLuongDieuChuyen = item.Sum(x => x.SoLuongDieuChuyen),
                        SoLo = item.First().SoLo,
                        HanSuDung = item.First().HanSuDung,
                    })
                    .OrderBy(x => x.LaDuocPhamBHYT).ThenBy(x => x.Ten).Distinct();
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var yeuCauDieuChuyenDuocPhamId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauDieuChuyenDuocPhamChiTietRepository.TableNoTracking
                .Where(z => z.YeuCauDieuChuyenDuocPhamId == yeuCauDieuChuyenDuocPhamId)
                .Select(s => new YeuCauDieuChuyenDuocPhamChiTietVo
                {
                    Id = s.Id,
                    Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                    HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    SoLuongDieuChuyen = s.SoLuongDieuChuyen,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM"
                }).GroupBy(x => new { x.DuocPhamBenhVienId, x.Ten, x.HoatChat, x.Nhom, x.DVT, x.SoLo, x.LaDuocPhamBHYT, x.HanSuDung })
                    .Select(item => new YeuCauDieuChuyenDuocPhamChiTietVo()
                    {
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                        Ten = item.First().Ten,
                        Nhom = item.First().Nhom,
                        HoatChat = item.First().HoatChat,
                        DVT = item.First().DVT,
                        SoLuongDieuChuyen = item.Sum(x => x.SoLuongDieuChuyen),
                        SoLo = item.First().SoLo,
                        HanSuDung = item.First().HanSuDung,
                    })
                    .OrderBy(x => x.LaDuocPhamBHYT).ThenBy(x => x.Ten).Distinct();
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridAsyncDuocPhamDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            BuildDefaultSortExpression(queryInfo);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dp => dp.DonViTinh)
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat && (info.HienThiCaThuocHetHan == true || (info.HienThiCaThuocHetHan != true && x.HanSuDung >= DateTime.Now)) && x.NhapKhoDuocPhams.KhoDuocPhams.Id == info.KhoXuatId);

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
            var yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats = new List<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo>();
            var nhapKhoDuocPhamChiTietGroup = nhapKhoDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhViens.Ma, x.DuocPhamBenhViens.DuocPham.Ten, x.DuocPhamBenhViens.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten, x.DonGiaTonKho })
                                                .Select(g => new { nhapKhoDuocPhamChiTiets = g.FirstOrDefault() });

            foreach (var item in nhapKhoDuocPhamChiTietGroup)
            {
                var yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuat = new YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo
                {
                    Id = item.nhapKhoDuocPhamChiTiets.Id,
                    DuocPhamBenhVienId = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhVienId,
                    Ten = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.Ten,
                    DVT = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    LaDuocPhamBHYT = item.nhapKhoDuocPhamChiTiets.LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    TenNhom = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom?.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.Ma,
                    SoDangKy = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.SoDangKy,
                    HamLuong = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.HamLuong,
                    SoLo = item.nhapKhoDuocPhamChiTiets.Solo,
                    HanSuDung = item.nhapKhoDuocPhamChiTiets.HanSuDung,
                    DonGia = item.nhapKhoDuocPhamChiTiets.DonGiaTonKho,
                    DonGiaNhap = item.nhapKhoDuocPhamChiTiets.DonGiaNhap
                };
                yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Add(yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuat);
            }
            var duocPhamBenhViens = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == info.KhoXuatId
                         && x.SoLuongDaXuat < x.SoLuongNhap && (info.HienThiCaThuocHetHan == true || (info.HienThiCaThuocHetHan != true && x.HanSuDung >= DateTime.Now))).ToList();

            var result = yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Select(o =>
            {
                o.SoLuongTon = duocPhamBenhViens.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaDuocPhamBHYT && t.Solo == o.SoLo && t.HanSuDung == o.HanSuDung && t.DonGiaTonKho == o.DonGia).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                o.SoLuongDieuChuyen = duocPhamBenhViens.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaDuocPhamBHYT && t.Solo == o.SoLo && t.HanSuDung == o.HanSuDung && t.DonGiaTonKho == o.DonGia).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            });
            if (info.DuocPhamBenhViens.Any())
            {
                result = result.Where(x => !info.DuocPhamBenhViens.Any(z => z.DuocPhamBenhVienId == x.DuocPhamBenhVienId && z.LaDuocPhamBHYT == x.LaDuocPhamBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.DonGia == x.DonGia));
            }
            var query = result.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamDaChon(QueryInfo queryInfo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dp => dp.DonViTinh)
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat && (info.HienThiCaThuocHetHan == true || (info.HienThiCaThuocHetHan != true && x.HanSuDung >= DateTime.Now)) && x.NhapKhoDuocPhams.KhoDuocPhams.Id == info.KhoXuatId);

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
            var yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats = new List<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo>();
            var nhapKhoDuocPhamChiTietGroup = nhapKhoDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhViens.Ma, x.DuocPhamBenhViens.DuocPham.Ten, x.DuocPhamBenhViens.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten, x.DonGiaTonKho })
                                                .Select(g => new { nhapKhoDuocPhamChiTiets = g.FirstOrDefault() });

            foreach (var item in nhapKhoDuocPhamChiTietGroup)
            {
                var yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuat = new YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo
                {
                    DuocPhamBenhVienId = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhVienId,
                    Ten = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.Ten,
                    DVT = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    LaDuocPhamBHYT = item.nhapKhoDuocPhamChiTiets.LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    TenNhom = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom?.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.Ma,
                    SoDangKy = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.SoDangKy,
                    HamLuong = item.nhapKhoDuocPhamChiTiets.DuocPhamBenhViens.DuocPham.HamLuong,
                    SoLo = item.nhapKhoDuocPhamChiTiets.Solo,
                    HanSuDung = item.nhapKhoDuocPhamChiTiets.HanSuDung,
                    DonGia = item.nhapKhoDuocPhamChiTiets.DonGiaTonKho,
                    DonGiaNhap = item.nhapKhoDuocPhamChiTiets.DonGiaNhap
                };
                yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Add(yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuat);
            }
            if (info.DuocPhamBenhViens.Any())
            {
                yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats = yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Where(x => !info.DuocPhamBenhViens.Any(z => z.DuocPhamBenhVienId == x.DuocPhamBenhVienId && z.LaDuocPhamBHYT == x.LaDuocPhamBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.DonGia == x.DonGia)).ToList();
            }
            var query = yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.AsQueryable();
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<List<LookupItemVo>> GetKhoTongCap2VaNhaThuocs(DropDownListRequestModel queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                        .Where(p => p.NhanVienId == userCurrentId && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoDuocPhamChoXuLy || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) && p.Kho.LoaiDuocPham == true)
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.KhoId,
                            DisplayName = s.Kho.Ten
                        })
                        .OrderByDescending(x => x.KeyId == khoId).ThenBy(x => x.DisplayName)
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);

            return await result.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetNguoiNhap(DropDownListRequestModel model)
        {
            var nhanVienId = CommonHelper.GetIdFromRequestDropDownList(model);
            var nhanViens = _nhanVienRepository.TableNoTracking
                            .Select(item => new LookupItemVo
                            {
                                DisplayName = item.User.HoTen,
                                KeyId = item.Id,
                            })
                            .OrderByDescending(x => x.KeyId == nhanVienId).ThenBy(x => x.DisplayName)
                            .ApplyLike(model.Query, s => s.DisplayName).Take(model.Take);
            return await nhanViens.ToListAsync();
        }

        public async Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long yeuCauDieuChuyenDuocPhamId)
        {
            var duocDuyet = await BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauDieuChuyenDuocPhamId).Select(p => p.DuocKeToanDuyet).FirstOrDefaultAsync();
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

        public async Task XuLyThemDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, List<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo> yeuCauDieuChuyenDuocPhamChiTiets)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            var khoXuatIds = yeuCauDieuChuyenDuocPhamChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
            var duocPhamBenhVienIds = yeuCauDieuChuyenDuocPhamChiTiets.Select(c => c.DuocPhamBenhVienId).Distinct().ToList();

            var nhapKhoDuocPhamChiTietAlls = _nhapKhoDuocPhamChiTietRepository.Table
                    .Include(nk => nk.NhapKhoDuocPhams)
                    .Where(o => khoXuatIds.Contains(o.NhapKhoDuocPhams.KhoId) && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && o.SoLuongNhap > o.SoLuongDaXuat)
                    .ToList();

            foreach (var yeuCauDieuChuyenDuocPhamChiTiet in yeuCauDieuChuyenDuocPhamChiTiets)
            {
                var nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTietAlls
                  .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauDieuChuyenDuocPhamChiTiet.KhoXuatId
                  && o.LaDuocPhamBHYT == yeuCauDieuChuyenDuocPhamChiTiet.LaDuocPhamBHYT
                  && o.DuocPhamBenhVienId == yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId
                  && (yeuCauDieuChuyenDuocPham.HienThiCaThuocHetHan == true || (yeuCauDieuChuyenDuocPham.HienThiCaThuocHetHan != true && o.HanSuDung >= DateTime.Now))
                  && o.Solo == yeuCauDieuChuyenDuocPhamChiTiet.SoLo
                  && o.HanSuDung == yeuCauDieuChuyenDuocPhamChiTiet.HanSuDung
                  && o.DonGiaTonKho == yeuCauDieuChuyenDuocPhamChiTiet.DonGia
                  && o.SoLuongNhap > o.SoLuongDaXuat)
                  .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                  .ToList();

                var SLTon = nhapKhoDuocPhamChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat);

                if (SLTon < yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen)
                {
                    throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                }
                var soLuongDieuChuyen = yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen;// số lượng điều chuyển

                var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                {
                    DuocPhamBenhVienId = yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId,
                };
                var yeuCauDieuChuyenDuocPhamChiTietNews = new List<YeuCauDieuChuyenDuocPhamChiTiet>();

                foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                {
                    if (soLuongDieuChuyen == 0)
                    {
                        break;
                    }
                    var yeuCauDieuChuyenDuocPhamChiTietNew = new YeuCauDieuChuyenDuocPhamChiTiet
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
                        PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                        MaVach = nhapKhoDuocPhamChiTiet.MaVach,
                        TiLeBHYTThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan,
                        MaRef = nhapKhoDuocPhamChiTiet.MaRef,
                    };
                    var SLTonHienTai = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                    if (SLTonHienTai > soLuongDieuChuyen || SLTonHienTai.AlmostEqual(soLuongDieuChuyen.Value))
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongDieuChuyen.Value;
                        yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen = soLuongDieuChuyen.Value;
                        soLuongDieuChuyen = 0;
                    }
                    else
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat = nhapKhoDuocPhamChiTiet.SoLuongNhap;
                        yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen = SLTonHienTai;
                        soLuongDieuChuyen -= SLTonHienTai;
                    }
                    var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                    {
                        SoLuongXuat = yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen,
                        GhiChu = "Xuất kho điều chuyển thuốc.",
                        XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet,
                        NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet
                    };
                    yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri = xuatKhoDuocPhamChiTietViTri;
                    //yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet;
                    //yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet;
                    yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Add(yeuCauDieuChuyenDuocPhamChiTietNew);
                }
            }
            await BaseRepository.AddAsync(yeuCauDieuChuyenDuocPham);
        }
        public async Task XuLyThemDieuChuyenThuocAsyncOld(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, List<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo> yeuCauDieuChuyenDuocPhamChiTiets)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            foreach (var yeuCauDieuChuyenDuocPhamChiTiet in yeuCauDieuChuyenDuocPhamChiTiets)
            {
                var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                  .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauDieuChuyenDuocPhamChiTiet.KhoXuatId
                  && o.LaDuocPhamBHYT == yeuCauDieuChuyenDuocPhamChiTiet.LaDuocPhamBHYT
                  && o.DuocPhamBenhVienId == yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId
                  && (yeuCauDieuChuyenDuocPham.HienThiCaThuocHetHan == true || (yeuCauDieuChuyenDuocPham.HienThiCaThuocHetHan != true && o.HanSuDung >= DateTime.Now))
                  && o.Solo == yeuCauDieuChuyenDuocPhamChiTiet.SoLo
                  && o.HanSuDung == yeuCauDieuChuyenDuocPhamChiTiet.HanSuDung
                  && o.DonGiaNhap == yeuCauDieuChuyenDuocPhamChiTiet.DonGiaNhap
                  && o.SoLuongNhap > o.SoLuongDaXuat)
                  .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();

                var SLTon = nhapKhoDuocPhamChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat);

                if (SLTon < yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen)
                {
                    throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                }
                var soLuongDieuChuyen = yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen;// số lượng điều chuyển

                var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                {
                    DuocPhamBenhVienId = yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId,
                };
                var yeuCauDieuChuyenDuocPhamChiTietNews = new List<YeuCauDieuChuyenDuocPhamChiTiet>();

                foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                {
                    if (soLuongDieuChuyen == 0)
                    {
                        break;
                    }
                    var yeuCauDieuChuyenDuocPhamChiTietNew = new YeuCauDieuChuyenDuocPhamChiTiet
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
                        PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                        MaVach = nhapKhoDuocPhamChiTiet.MaVach,
                        TiLeBHYTThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan,
                        MaRef = nhapKhoDuocPhamChiTiet.MaRef,
                    };
                    var SLTonHienTai = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                    if (SLTonHienTai > soLuongDieuChuyen || SLTonHienTai.AlmostEqual(soLuongDieuChuyen.Value))
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongDieuChuyen.Value;
                        yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen = soLuongDieuChuyen.Value;
                        soLuongDieuChuyen = 0;
                    }
                    else
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat = nhapKhoDuocPhamChiTiet.SoLuongNhap;
                        yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen = SLTonHienTai;
                        soLuongDieuChuyen -= SLTonHienTai;
                    }
                    var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                    {
                        SoLuongXuat = yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen,
                        GhiChu = "Xuất kho điều chuyển thuốc.",
                        XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet,
                    };
                    yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri = xuatKhoDuocPhamChiTietViTri;
                    yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet;
                    yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet;
                    yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Add(yeuCauDieuChuyenDuocPhamChiTietNew);
                }
            }
            await BaseRepository.UpdateAsync(yeuCauDieuChuyenDuocPham);
        }

        public async Task XuLyXoaDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, bool isUpdate)
        {
            foreach (var yeuCauDieuChuyenDuocPhamChiTiet in yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets)
            {
                if (!isUpdate)
                {
                    //hoàn trả số lượng đã xuất
                    yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen;
                }
                //xóa hết các xuất kho vị trí
                yeuCauDieuChuyenDuocPhamChiTiet.WillDelete = true;
                yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
            }
            if (!isUpdate)
            {
                yeuCauDieuChuyenDuocPham.WillDelete = true;
                await BaseRepository.UpdateAsync(yeuCauDieuChuyenDuocPham);
            }
        }
        public async Task XuLyCapNhatDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, List<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo> yeuCauDieuChuyenDuocPhamChiTiets)
        {
            foreach (var yeuCauDieuChuyenDuocPhamChiTiet in yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets)
            {                
                //hoàn trả số lượng đã xuất
                yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen;
                
                //xóa hết các xuất kho vị trí
                yeuCauDieuChuyenDuocPhamChiTiet.WillDelete = true;
                yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
            }
            await BaseRepository.UpdateAsync(yeuCauDieuChuyenDuocPham);

            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            var khoXuatIds = yeuCauDieuChuyenDuocPhamChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
            var duocPhamBenhVienIds = yeuCauDieuChuyenDuocPhamChiTiets.Select(c => c.DuocPhamBenhVienId).Distinct().ToList();

            var nhapKhoDuocPhamChiTietAlls = _nhapKhoDuocPhamChiTietRepository.Table
                    .Include(nk => nk.NhapKhoDuocPhams)
                    .Where(o => khoXuatIds.Contains(o.NhapKhoDuocPhams.KhoId) && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && o.SoLuongNhap > o.SoLuongDaXuat)
                    .ToList();

            foreach (var yeuCauDieuChuyenDuocPhamChiTiet in yeuCauDieuChuyenDuocPhamChiTiets)
            {
                var nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTietAlls
                  .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauDieuChuyenDuocPhamChiTiet.KhoXuatId
                  && o.LaDuocPhamBHYT == yeuCauDieuChuyenDuocPhamChiTiet.LaDuocPhamBHYT
                  && o.DuocPhamBenhVienId == yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId
                  && (yeuCauDieuChuyenDuocPham.HienThiCaThuocHetHan == true || (yeuCauDieuChuyenDuocPham.HienThiCaThuocHetHan != true && o.HanSuDung >= DateTime.Now))
                  && o.Solo == yeuCauDieuChuyenDuocPhamChiTiet.SoLo
                  && o.HanSuDung == yeuCauDieuChuyenDuocPhamChiTiet.HanSuDung
                  && o.DonGiaTonKho == yeuCauDieuChuyenDuocPhamChiTiet.DonGia
                  && o.SoLuongNhap > o.SoLuongDaXuat)
                  .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                  .ToList();

                var SLTon = nhapKhoDuocPhamChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat);

                if (SLTon < yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen)
                {
                    throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                }
                var soLuongDieuChuyen = yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen;// số lượng điều chuyển

                var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                {
                    DuocPhamBenhVienId = yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId,
                };
                var yeuCauDieuChuyenDuocPhamChiTietNews = new List<YeuCauDieuChuyenDuocPhamChiTiet>();

                foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                {
                    if (soLuongDieuChuyen == 0)
                    {
                        break;
                    }
                    var yeuCauDieuChuyenDuocPhamChiTietNew = new YeuCauDieuChuyenDuocPhamChiTiet
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
                        PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                        MaVach = nhapKhoDuocPhamChiTiet.MaVach,
                        TiLeBHYTThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan,
                        MaRef = nhapKhoDuocPhamChiTiet.MaRef,
                    };
                    var SLTonHienTai = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                    if (SLTonHienTai > soLuongDieuChuyen || SLTonHienTai.AlmostEqual(soLuongDieuChuyen.Value))
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongDieuChuyen.Value;
                        yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen = soLuongDieuChuyen.Value;
                        soLuongDieuChuyen = 0;
                    }
                    else
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat = nhapKhoDuocPhamChiTiet.SoLuongNhap;
                        yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen = SLTonHienTai;
                        soLuongDieuChuyen -= SLTonHienTai;
                    }
                    var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                    {
                        SoLuongXuat = yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen,
                        GhiChu = "Xuất kho điều chuyển thuốc.",
                        XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet,
                        NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet
                    };
                    yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri = xuatKhoDuocPhamChiTietViTri;
                    yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Add(yeuCauDieuChuyenDuocPhamChiTietNew);
                }
            }
            await BaseRepository.UpdateAsync(yeuCauDieuChuyenDuocPham);
        }
        public async Task XuLyCapNhatDieuChuyenThuocAsyncOld(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, List<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo> yeuCauDieuChuyenDuocPhamChiTiets)
        {
            await XuLyXoaDieuChuyenThuocAsync(yeuCauDieuChuyenDuocPham, true);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            foreach (var yeuCauDieuChuyenDuocPhamChiTiet in yeuCauDieuChuyenDuocPhamChiTiets)
            {
                if (yeuCauDieuChuyenDuocPhamChiTiet.WillDelete == true)
                {
                    var yeuCauDieuChuyenDuocPhamChiTietWillDeletes = yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Where(z => z.DuocPhamBenhVienId == yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId && z.LaDuocPhamBHYT == yeuCauDieuChuyenDuocPhamChiTiet.LaDuocPhamBHYT).ToList();
                    foreach (var item in yeuCauDieuChuyenDuocPhamChiTietWillDeletes)
                    {
                        item.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= item.SoLuongDieuChuyen;
                        item.WillDelete = true;
                        item.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                        item.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                    }
                }
                else
                {

                    var SLDaDieuChuyen = yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Where(z => z.DuocPhamBenhVienId == yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId && z.LaDuocPhamBHYT == yeuCauDieuChuyenDuocPhamChiTiet.LaDuocPhamBHYT).Sum(z => z.SoLuongDieuChuyen);

                    var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                      .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauDieuChuyenDuocPhamChiTiet.KhoXuatId
                      && o.LaDuocPhamBHYT == yeuCauDieuChuyenDuocPhamChiTiet.LaDuocPhamBHYT
                      && o.DuocPhamBenhVienId == yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId
                      && (yeuCauDieuChuyenDuocPham.HienThiCaThuocHetHan == true || (yeuCauDieuChuyenDuocPham.HienThiCaThuocHetHan != true && o.HanSuDung >= DateTime.Now))
                      && o.Solo == yeuCauDieuChuyenDuocPhamChiTiet.SoLo
                      && o.HanSuDung == yeuCauDieuChuyenDuocPhamChiTiet.HanSuDung
                      && o.SoLuongNhap + SLDaDieuChuyen > o.SoLuongDaXuat)
                      .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();

                    var SLTon = nhapKhoDuocPhamChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat) + SLDaDieuChuyen;

                    if (SLTon < yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen)
                    {
                        throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                    }
                    //var soLuongDieuChuyen = yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen;// số lượng điều chuyển

                    var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                    {
                        DuocPhamBenhVienId = yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId,
                    };
                    var yeuCauDieuChuyenDuocPhamChiTietNews = new List<YeuCauDieuChuyenDuocPhamChiTiet>();

                    foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                    {
                        if (yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen == 0)
                        {
                            break;
                        }
                        var yeuCauDieuChuyenDuocPhamChiTietDaXuats = yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Where(z => z.DuocPhamBenhVienId == yeuCauDieuChuyenDuocPhamChiTiet.DuocPhamBenhVienId && z.LaDuocPhamBHYT == yeuCauDieuChuyenDuocPhamChiTiet.LaDuocPhamBHYT && z.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId == nhapKhoDuocPhamChiTiet.Id).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                        if (!yeuCauDieuChuyenDuocPhamChiTietDaXuats.Any())
                        {
                            continue;
                        }
                        var tongSLDaDieuChuyen = yeuCauDieuChuyenDuocPhamChiTietDaXuats.Sum(z => z.SoLuongDieuChuyen);

                        var yeuCauDieuChuyenDuocPhamChiTietNew = new YeuCauDieuChuyenDuocPhamChiTiet
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
                            PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                            MaVach = nhapKhoDuocPhamChiTiet.MaVach,
                            TiLeBHYTThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan,
                            MaRef = nhapKhoDuocPhamChiTiet.MaRef,
                        };
                        var SLTonHienTai = (nhapKhoDuocPhamChiTiet.SoLuongNhap + tongSLDaDieuChuyen) - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                        if (SLTonHienTai > yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen || SLTonHienTai.AlmostEqual(yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen.Value))
                        {
                            nhapKhoDuocPhamChiTiet.SoLuongDaXuat -= tongSLDaDieuChuyen - yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen.Value; // trừ lại số lượng đã xuất
                            yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen = yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen.Value;
                            yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen = 0;
                        }
                        else
                        {
                            nhapKhoDuocPhamChiTiet.SoLuongDaXuat = nhapKhoDuocPhamChiTiet.SoLuongNhap;
                            yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen = SLTonHienTai;
                            yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen -= SLTonHienTai;
                        }
                        var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                        {
                            SoLuongXuat = yeuCauDieuChuyenDuocPhamChiTietNew.SoLuongDieuChuyen,
                            GhiChu = "Xuất kho điều chuyển thuốc.",
                            XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet,
                        };
                        yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri = xuatKhoDuocPhamChiTietViTri;
                        yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet;
                        foreach (var item in yeuCauDieuChuyenDuocPhamChiTietDaXuats)
                        {
                            if (item.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId == nhapKhoDuocPhamChiTiet.Id)
                            {
                                item.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                                break;
                            }
                        }
                        yeuCauDieuChuyenDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet;
                        yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Add(yeuCauDieuChuyenDuocPhamChiTietNew);
                    }
                }
            }
            await BaseRepository.UpdateAsync(yeuCauDieuChuyenDuocPham);
        }

        public async Task<GridDataSource> GetDataForGridAsyncDaTaoYeuCau(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVoSearch>(queryInfo.AdditionalSearchString);

            var yeuCauDieuChuyenDuocPhamChiTiets = _yeuCauDieuChuyenDuocPhamChiTietRepository.TableNoTracking
                   .Include(nkct => nkct.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dp => dp.DonViTinh)
                   .Include(nkct => nkct.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                   .Include(nkct => nkct.YeuCauDieuChuyenDuocPham)
                   .Where(x => x.YeuCauDieuChuyenDuocPhamId == info.YeuCauDieuChuyenDuocPhamId);

            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                yeuCauDieuChuyenDuocPhamChiTiets = yeuCauDieuChuyenDuocPhamChiTiets.ApplyLike(searchTerms,
                    g => g.DuocPhamBenhVien.DuocPham.Ten,
                    g => g.DuocPhamBenhVien.Ma,
                    g => g.DuocPhamBenhVien.DuocPham.SoDangKy,
                    g => g.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    g => g.DuocPhamBenhVien.DuocPham.HamLuong,
                    g => g.Solo
               );
            }
            var yeuCauDieuChuyenDuocPhamChiTietGroup = yeuCauDieuChuyenDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhVien.Ma, x.DuocPhamBenhVien.DuocPham.Ten, x.DuocPhamBenhVien.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhVien.DuocPham.DonViTinh.Ten, x.DonGiaTonKho })
                                                .Select(item => new YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo
                                                {
                                                    KhoXuatId = item.First().YeuCauDieuChuyenDuocPham.KhoXuatId,
                                                    DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                                    Ten = item.First().DuocPhamBenhVien.DuocPham.Ten,
                                                    DVT = item.First().DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                    LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                                                    DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                                    TenNhom = item.First().DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? item.First().DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM",
                                                    Ma = item.First().DuocPhamBenhVien.Ma,
                                                    SoDangKy = item.First().DuocPhamBenhVien.DuocPham.SoDangKy,
                                                    HamLuong = item.First().DuocPhamBenhVien.DuocPham.HamLuong,
                                                    SoLo = item.First().Solo,
                                                    HanSuDung = item.First().HanSuDung,
                                                    DonGia = item.First().DonGiaTonKho.GetValueOrDefault(),
                                                    SoLuongDieuChuyen = item.Sum(z => z.SoLuongDieuChuyen)
                                                }).OrderBy(x => x.Ma).ThenBy(x => x.Ten).ToList();

            var result = yeuCauDieuChuyenDuocPhamChiTietGroup.Select(o =>
            {
                var soLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.DuocPhamBenhVienId == o.DuocPhamBenhVienId && x.LaDuocPhamBHYT == o.LaDuocPhamBHYT && x.HanSuDung >= DateTime.Now && x.SoLuongDaXuat < x.SoLuongNhap && x.NhapKhoDuocPhams.KhoId == o.KhoXuatId && x.Solo == o.SoLo && x.DonGiaTonKho == o.DonGia && x.HanSuDung == o.HanSuDung).Sum(z => z.SoLuongNhap - z.SoLuongDaXuat);
                o.SoLuongTon = soLuongTon + o.SoLuongDieuChuyen;
                return o;
            });
            if (info.DuocPhamBenhViens.Any())
            {
                result = result.Where(x => !info.DuocPhamBenhViens.Any(z => z.DuocPhamBenhVienId == x.DuocPhamBenhVienId && z.LaDuocPhamBHYT == x.LaDuocPhamBHYT && z.Ten == x.Ten && z.Ma == x.Ma && z.SoLo == x.SoLo && z.DonGia == x.DonGia));
            }
            var query = result.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncDaTaoYeuCau(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVoSearch>(queryInfo.AdditionalSearchString);

            var yeuCauDieuChuyenDuocPhamChiTiets = _yeuCauDieuChuyenDuocPhamChiTietRepository.TableNoTracking
                   .Include(nkct => nkct.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dp => dp.DonViTinh)
                   .Include(nkct => nkct.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                   .Where(x => x.YeuCauDieuChuyenDuocPhamId == info.YeuCauDieuChuyenDuocPhamId);

            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                yeuCauDieuChuyenDuocPhamChiTiets = yeuCauDieuChuyenDuocPhamChiTiets.ApplyLike(searchTerms,
                    g => g.DuocPhamBenhVien.DuocPham.Ten,
                    g => g.DuocPhamBenhVien.Ma,
                    g => g.DuocPhamBenhVien.DuocPham.SoDangKy,
                    g => g.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    g => g.DuocPhamBenhVien.DuocPham.HamLuong,
                    g => g.Solo
               );
            }
            var yeuCauDieuChuyenDuocPhamChiTietGroup = yeuCauDieuChuyenDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhVien.Ma, x.DuocPhamBenhVien.DuocPham.Ten, x.DuocPhamBenhVien.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhVien.DuocPham.DonViTinh.Ten, x.DonGiaTonKho })
                                                .Select(item => new YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo
                                                {
                                                    DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                                    Ten = item.First().DuocPhamBenhVien.DuocPham.Ten,
                                                    DVT = item.First().DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                    LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                                                    DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                                    TenNhom = item.First().DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? item.First().DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM",
                                                    Ma = item.First().DuocPhamBenhVien.Ma,
                                                    SoDangKy = item.First().DuocPhamBenhVien.DuocPham.SoDangKy,
                                                    HamLuong = item.First().DuocPhamBenhVien.DuocPham.HamLuong,
                                                    SoLo = item.First().Solo,
                                                    HanSuDung = item.First().HanSuDung,
                                                    DonGia = item.First().DonGiaTonKho.GetValueOrDefault(),
                                                    SoLuongDieuChuyen = item.Sum(z => z.SoLuongDieuChuyen)
                                                }).OrderBy(x => x.Ma).ThenBy(x => x.Ten).ToList();
            if (info.DuocPhamBenhViens.Any())
            {
                yeuCauDieuChuyenDuocPhamChiTietGroup = yeuCauDieuChuyenDuocPhamChiTietGroup.Where(x => !info.DuocPhamBenhViens.Any(z => z.DuocPhamBenhVienId == x.DuocPhamBenhVienId && z.LaDuocPhamBHYT == x.LaDuocPhamBHYT && z.Ten == x.Ten && z.Ma == x.Ma && z.SoLo == x.SoLo && z.DonGia == x.DonGia)).ToList();
            }
            var countTask = yeuCauDieuChuyenDuocPhamChiTietGroup.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task CheckPhieuYeuCauDieuChuyenDaDuyetHoacDaHuy(long yeuCauDieuChuyenDuocPhamId)
        {
            var result = await BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauDieuChuyenDuocPhamId).Select(p => p).FirstOrDefaultAsync();
            var resourceName = string.Empty;
            if (result == null)
            {
                resourceName = "YeuCauDieuChuyenDuocPham.PhieuYeuCau.NotExists";
            }
            else
            {
                if (result.DuocKeToanDuyet != null)
                {
                    resourceName = "YeuCauDieuChuyenDuocPham.PhieuYeuCau.DaDuyet";
                }
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                var currentUserLanguge = _userAgentHelper.GetUserLanguage();
                var mess = await _localeStringResourceRepository.TableNoTracking
                    .Where(x => x.ResourceName == resourceName && x.Language == (int)currentUserLanguge)
                    .Select(x => x.ResourceValue).FirstOrDefaultAsync();
                throw new Exception(mess ?? resourceName);
            }
        }

        public async Task<List<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo>> YeuCauDieuChuyenDuocPhamChiTietHienThis(long yeuCauDieuChuyenDuocPhamId)
        {
            var yeuCauDieuChuyenDuocPhamChiTietHienThis = _yeuCauDieuChuyenDuocPhamChiTietRepository.TableNoTracking.Where(z => z.YeuCauDieuChuyenDuocPhamId == yeuCauDieuChuyenDuocPhamId)
                .Select(s => new YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    SoLuongDieuChuyen = s.SoLuongDieuChuyen,
                    XuatKhoDuocPhamChiTietViTriId = s.XuatKhoDuocPhamChiTietViTriId
                });
            return await yeuCauDieuChuyenDuocPhamChiTietHienThis.ToListAsync();
        }

        #region DUYỆT
        public async Task XuLyDuyetDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham)
        {
            if (yeuCauDieuChuyenDuocPham.DuocKeToanDuyet == null)
            {
                var soChungTu = yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.First().XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.SoChungTu;
                var nhapKhoDuocPham = new NhapKhoDuocPham
                {
                    SoChungTu = soChungTu,
                    TenNguoiGiao = yeuCauDieuChuyenDuocPham.NguoiXuat.User.HoTen,
                    NguoiGiaoId = yeuCauDieuChuyenDuocPham.NguoiXuatId,
                    NguoiNhapId = yeuCauDieuChuyenDuocPham.NguoiNhapId,
                    LoaiNguoiGiao = LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayNhap = DateTime.Now,
                    KhoId = yeuCauDieuChuyenDuocPham.KhoNhapId,
                };
                var xuatKhoDuocPham = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham
                {
                    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                    LyDoXuatKho = yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Select(z => z.XuatKhoDuocPhamChiTietViTri.GhiChu).FirstOrDefault(),
                    TenNguoiNhan = yeuCauDieuChuyenDuocPham.NguoiNhap.User.HoTen,
                    NguoiNhanId = yeuCauDieuChuyenDuocPham.NguoiNhapId,
                    NguoiXuatId = yeuCauDieuChuyenDuocPham.NguoiXuatId,
                    NgayXuat = DateTime.Now,
                    KhoXuatId = yeuCauDieuChuyenDuocPham.KhoXuatId,
                    KhoNhapId = yeuCauDieuChuyenDuocPham.KhoNhapId,
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.TrongHeThong
                };
                xuatKhoDuocPham.NhapKhoDuocPhams.Add(nhapKhoDuocPham);
                //update BVHD-3576
                //Thuốc từ kho BHYT chỉ được chuyển về kho Thuốc Bệnh viện và Kho Nhà thuốc
                // Khi bán thuốc/ sử dụng thuốc cho NB sau khi điều chuyển nội bộ  từ kho BHYT về vẫn áp dụng THÁP GIÁ bình thường, lúc này VAT = 0
                bool dieuChuyenTuKhoBHYT = yeuCauDieuChuyenDuocPham.KhoXuatId == (long)EnumKhoDuocPham.KhoThuocBHYT &&
                                           (yeuCauDieuChuyenDuocPham.KhoNhapId == (long)EnumKhoDuocPham.KhoNhaThuoc ||
                                            yeuCauDieuChuyenDuocPham.KhoNhapId == (long)EnumKhoDuocPham.KhoThuocBenhVien);
                foreach (var chiTiet in yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets)
                {
                    var tiLeTheoThapGiaMoi = chiTiet.TiLeTheoThapGia;
                    var laDuocPhamBHYTMoi = chiTiet.LaDuocPhamBHYT;
                    if (dieuChuyenTuKhoBHYT && chiTiet.LaDuocPhamBHYT)
                    {
                        laDuocPhamBHYTMoi = false;
                        tiLeTheoThapGiaMoi = _cauHinhService.GetTiLeTheoThapGia(LoaiThapGia.ThuocKhongBaoHiem, chiTiet.DonGiaNhap);
                    }
                    var nhapKhoDuocPhamChiTiet = new NhapKhoDuocPhamChiTiet
                    {
                        DuocPhamBenhVienId = chiTiet.DuocPhamBenhVienId,
                        HopDongThauDuocPhamId = chiTiet.HopDongThauDuocPhamId,
                        Solo = chiTiet.Solo,
                        HanSuDung = chiTiet.HanSuDung,
                        SoLuongNhap = chiTiet.SoLuongDieuChuyen,
                        DonGiaNhap = chiTiet.DonGiaNhap,
                        VAT = chiTiet.VAT,
                        MaVach = chiTiet.MaVach,
                        SoLuongDaXuat = 0,
                        NgayNhap = DateTime.Now,
                        LaDuocPhamBHYT = laDuocPhamBHYTMoi,
                        NgayNhapVaoBenhVien = chiTiet.NgayNhapVaoBenhVien,
                        TiLeTheoThapGia = tiLeTheoThapGiaMoi,
                        TiLeBHYTThanhToan = chiTiet.TiLeBHYTThanhToan,
                        MaRef = chiTiet.MaRef,
                        PhuongPhapTinhGiaTriTonKho = chiTiet.PhuongPhapTinhGiaTriTonKho,
                        KhoNhapSauKhiDuyetId = yeuCauDieuChuyenDuocPham.KhoXuatId,
                        NguoiNhapSauKhiDuyetId = nhapKhoDuocPham.NguoiNhapId
                    };
                    nhapKhoDuocPham.NhapKhoDuocPhamChiTiets.Add(nhapKhoDuocPhamChiTiet);
                }
                yeuCauDieuChuyenDuocPham.DuocKeToanDuyet = true;
                yeuCauDieuChuyenDuocPham.NgayDuyet = DateTime.Now;
                yeuCauDieuChuyenDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                foreach (var yeuCauDieuChuyenDuocPhamChiTiet in yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets)
                {
                    yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.NgayXuat = DateTime.Now;
                    yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = xuatKhoDuocPham;
                }

                await BaseRepository.UpdateAsync(yeuCauDieuChuyenDuocPham);
            }
                
        }

        public async Task XuLyKhongDuyetDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, string lyDoKhongDuyet)
        {
            if(yeuCauDieuChuyenDuocPham.DuocKeToanDuyet == null)
            {
                foreach (var yeuCauDieuChuyenDuocPhamChiTiet in yeuCauDieuChuyenDuocPham.YeuCauDieuChuyenDuocPhamChiTiets)
                {
                    yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= yeuCauDieuChuyenDuocPhamChiTiet.SoLuongDieuChuyen;
                    yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                    yeuCauDieuChuyenDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                }
                yeuCauDieuChuyenDuocPham.DuocKeToanDuyet = false;
                yeuCauDieuChuyenDuocPham.NgayDuyet = DateTime.Now;
                yeuCauDieuChuyenDuocPham.LyDoKhongDuyet = lyDoKhongDuyet;
                yeuCauDieuChuyenDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                await BaseRepository.UpdateAsync(yeuCauDieuChuyenDuocPham);
            }
        }
        #endregion


        public string InPhieuXuatKhoDuocPhamDieuChuyen(YeuCauDieuChuyenDuocPhamDataVo yeuCauDieuChuyenDuocPhamDataVo)
        {
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatThuocDieuChuyenNoiBo"));
            var yeuCauDieuChuyenDuocPhamChiTietDatas = _yeuCauDieuChuyenDuocPhamChiTietRepository.TableNoTracking
                                          .Where(p => p.YeuCauDieuChuyenDuocPhamId == yeuCauDieuChuyenDuocPhamDataVo.YeuCauDieuChuyenDuocPhamId)
                                          .Select(s => new YeuCauDieuChuyenDuocPhamChiTietData
                                          {
                                              NguoiNhan = s.YeuCauDieuChuyenDuocPham.NguoiNhap.User.HoTen,
                                              KhoNhap = s.YeuCauDieuChuyenDuocPham.KhoNhap.Ten,
                                              LyDoXuat = "Xuất nội bộ",
                                              KhoXuat = s.YeuCauDieuChuyenDuocPham.KhoXuat.Ten,
                                              Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                                              Ma = s.DuocPhamBenhVien.Ma,
                                              SoLo = s.Solo,
                                              HanSuDung = s.HanSuDung,
                                              DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                              LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                                              SoLuongDieuChuyen = s.SoLuongDieuChuyen,
                                              DonGia = s.DonGiaTonKho.GetValueOrDefault(),
                                              DonGiaNhap = s.DonGiaNhap,
                                              NguoiLap = s.YeuCauDieuChuyenDuocPham.NguoiXuat.User.HoTen,
                                              CreateOn = s.YeuCauDieuChuyenDuocPham.CreatedOn,
                                          })
                                          .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.Ten, x.HamLuong, x.SoLo, x.HanSuDung, x.DVT, x.DonGia })
                                                .Select(g => new YeuCauDieuChuyenDuocPhamChiTietData
                                                {
                                                    NguoiNhan = g.First().NguoiNhan,
                                                    KhoNhap = g.First().KhoNhap,
                                                    LyDoXuat = g.First().LyDoXuat,
                                                    KhoXuat = g.First().KhoXuat,
                                                    Ten = g.First().Ten,
                                                    Ma = g.First().Ma,
                                                    SoLo = g.First().SoLo,
                                                    HanSuDung = g.First().HanSuDung,
                                                    DVT = g.First().DVT,
                                                    SoLuongDieuChuyen = g.Sum(z => z.SoLuongDieuChuyen).MathRoundNumber(2),
                                                    DonGia = g.First().DonGia,
                                                    NguoiLap = g.First().NguoiLap,
                                                    CreateOn = g.First().CreateOn,
                                                })
                                          .ToList();

            var congKhoan = yeuCauDieuChuyenDuocPhamChiTietDatas.Count();
            var tongCong = yeuCauDieuChuyenDuocPhamChiTietDatas.Sum(z => z.ThanhTien);
            var STT = 1;
            var duocPhamHoacVatTus = string.Empty;
            foreach (var item in yeuCauDieuChuyenDuocPhamChiTietDatas)
            {
                duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                           + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                           + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLo
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HanSuDungDisplay
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuongDieuChuyen.ApplyNumber()
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuongDieuChuyen.ApplyNumber()
                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                           + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND().Replace(" ₫", "")
                                           + "</tr>";
                STT++;
            }
            var data = new YeuCauDieuChuyenDuocPhamData
            {
                LogoUrl = yeuCauDieuChuyenDuocPhamDataVo.HostingName + "/assets/img/logo-bacha-full.png",
                NgayTaoPhieu = yeuCauDieuChuyenDuocPhamChiTietDatas.First().NgayTaoPhieu,
                NguoiNhan = yeuCauDieuChuyenDuocPhamChiTietDatas.First().NguoiNhan,
                KhoNhap = yeuCauDieuChuyenDuocPhamChiTietDatas.First().KhoNhap,
                LyDoXuat = yeuCauDieuChuyenDuocPhamChiTietDatas.First().LyDoXuat,
                KhoXuat = yeuCauDieuChuyenDuocPhamChiTietDatas.First().KhoXuat,
                DuocPhamHoacVatTus = duocPhamHoacVatTus,
                CongKhoan = congKhoan,
                TongCongDecimal = tongCong,
                NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                NguoiLap = yeuCauDieuChuyenDuocPhamChiTietDatas.First().NguoiLap,
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
    }
}
