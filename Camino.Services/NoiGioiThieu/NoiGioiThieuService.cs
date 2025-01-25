using System;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuBenhVienTongHops;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.NoiGioiThieu;
using Camino.Core.Domain.ValueObject.NguoiGioiThieu;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.DonViMaus;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Services.CauHinh;
using Camino.Services.Localization;
using OfficeOpenXml;

namespace Camino.Services.NoiGioiThieu
{
    [ScopedDependency(ServiceType = typeof(INoiGioiThieuService))]


    public class NoiGioiThieuService : MasterFileService<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu>, INoiGioiThieuService
    {
        private IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private IRepository<Core.Domain.Entities.Users.User> _userRepository;
        private IRepository<DonViMau> _donViMauRepository;
        private IRepository<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private IRepository<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;
        private IRepository<DichVuGiuongBenhVienGiaBenhVien> _dichVuGiuongBenhVienGiaBenhVienRepository;
        private IRepository<NoiGioiThieuChiTietMienGiam> _noiGioiThieuChiTietMienGiamRepository;
        private IRepository<NhomGiaDichVuKhamBenhBenhVien> _nhomGiaDichVuKhamBenhBenhVienRepository;
        private IRepository<NhomGiaDichVuKyThuatBenhVien> _nhomGiaDichVuKyThuatBenhVienRepository;
        private IRepository<NhomGiaDichVuGiuongBenhVien> _nhomGiaDichVuGiuongBenhVienRepository;

        private IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> _dichVuGiuongBenhVienRepository;
        private IRepository<DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;
        private IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private IRepository<DichVuBenhVienTongHop> _dichVuBenhVienTongHopRepository;

        private readonly ILocalizationService _localizationService;
        private readonly ICauHinhService _cauHinhService;


        public NoiGioiThieuService(IRepository<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu> repository,
            IRepository<Core.Domain.Entities.Users.User> userRepository,
            IRepository<DonViMau> donViMauRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository,
            IRepository<DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository,
            IRepository<DichVuGiuongBenhVienGiaBenhVien> dichVuGiuongBenhVienGiaBenhVienRepository,
            IRepository<NoiGioiThieuChiTietMienGiam> noiGioiThieuChiTietMienGiamRepository,
            IRepository<NhomGiaDichVuKhamBenhBenhVien> nhomGiaDichVuKhamBenhBenhVienRepository,
            IRepository<NhomGiaDichVuKyThuatBenhVien> nhomGiaDichVuKyThuatBenhVienRepository,
            IRepository<NhomGiaDichVuGiuongBenhVien> nhomGiaDichVuGiuongBenhVienRepository,

            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> dichVuGiuongBenhVienRepository,
            IRepository<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<DichVuBenhVienTongHop> dichVuBenhVienTongHopRepository,

            ILocalizationService localizationService,
            ICauHinhService cauHinhService
            ) : base(repository)
        {
            _nhanVienRepository = nhanVienRepository;
            _userRepository = userRepository;
            _donViMauRepository = donViMauRepository;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
            _dichVuGiuongBenhVienGiaBenhVienRepository = dichVuGiuongBenhVienGiaBenhVienRepository;
            _noiGioiThieuChiTietMienGiamRepository = noiGioiThieuChiTietMienGiamRepository;
            _localizationService = localizationService;
            _nhomGiaDichVuKhamBenhBenhVienRepository = nhomGiaDichVuKhamBenhBenhVienRepository;
            _nhomGiaDichVuKyThuatBenhVienRepository = nhomGiaDichVuKyThuatBenhVienRepository;
            _nhomGiaDichVuGiuongBenhVienRepository = nhomGiaDichVuGiuongBenhVienRepository;

            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _dichVuGiuongBenhVienRepository = dichVuGiuongBenhVienRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _dichVuBenhVienTongHopRepository = dichVuBenhVienTongHopRepository;
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

            var query = BaseRepository.TableNoTracking
                .Select(s => new NoiGioiThieuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                    DonVi = s.DonVi,
                    IsDisabled = s.IsDisabled,
                    MoTa = s.MoTa,
                    HoTenNguoiQuanLy = s.NhanVienQuanLy.User.HoTen + (s.NhanVienQuanLy.User.SoDienThoaiDisplay == null ? null : (" - " + s.NhanVienQuanLy.User.SoDienThoaiDisplay))
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.SoDienThoai, g => g.HoTenNguoiQuanLy, g => g.DonVi, g => g.MoTa, g => g.SoDienThoaiDisplay);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Select(s => new NoiGioiThieuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                    DonVi = s.DonVi,
                    IsDisabled = s.IsDisabled,
                    MoTa = s.MoTa,
                    HoTenNguoiQuanLy = s.NhanVienQuanLy.User.HoTen + (s.NhanVienQuanLy.User.SoDienThoaiDisplay == null ? null : (" - " + s.NhanVienQuanLy.User.SoDienThoaiDisplay))
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.SoDienThoai, g => g.HoTenNguoiQuanLy, g => g.DonVi, g => g.MoTa, g => g.SoDienThoaiDisplay);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<NguoiQuanLyTemplateVo>> GetNguoiQuanLyListAsync(DropDownListRequestModel queryInfo)
        {
            var result = await _nhanVienRepository.TableNoTracking
                .Select(item => new NguoiQuanLyTemplateVo
                {
                    DisplayName = item.User.HoTen + "  -  " + item.User.SoDienThoai.ApplyFormatPhone(),
                    KeyId = item.Id,
                    HoTen = item.User.HoTen,
                    SoDienThoai = item.User.SoDienThoaiDisplay,
                }).ApplyLike(queryInfo.Query, o => o.HoTen, o => o.SoDienThoai).Take(queryInfo.Take).ToListAsync();
            return result;
        }

        public async Task<List<LookupItemVo>> GetDonViMaus(DropDownListRequestModel queryInfo)
        {
            var result = _donViMauRepository.TableNoTracking.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<bool> IsTenExists(string ten = null, long Id = 0)
        {
            var result = false;
            if (ten == null)
            {
                return result;
            }
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != Id);
            }
            return result;
        }

        public async Task<bool> IsPhoneNumberExists(string ten = null, string soDienThoai = null, long Id = 0)
        {
            var result = false;
            if (ten == null && soDienThoai == null || soDienThoai == "" || soDienThoai == null)
            {
                return result;
            }
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoDienThoai == soDienThoai);
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Id != Id && p.SoDienThoai == soDienThoai);
            }
            return result;
        }

        public async Task<GridDataSource> GetDataForGridDonViMauAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = _donViMauRepository.TableNoTracking
                .Select(s => new DonViMauGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    IsDefault = s.IsDefault,
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridDonViMauAsync(QueryInfo queryInfo)
        {
            var query = _donViMauRepository.TableNoTracking
                .Select(s => new DonViMauGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    IsDefault = s.IsDefault,
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<bool> IsMaTenExists(string maHoacTen = null, long Id = 0, bool flag = false)
        {
            var result = false;
            if (Id == 0)
            {
                result = !flag ? await _donViMauRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maHoacTen))
                    : await _donViMauRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(maHoacTen));
            }
            else
            {
                result = !flag ? await _donViMauRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maHoacTen) && p.Id != Id) :
                     await _donViMauRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(maHoacTen) && p.Id != Id);
            }
            if (result)
                return false;
            return true;
        }

        #region BVHD-3882
        public async Task GetDonGia(ThongTinGiaVo thongTinDichVu)
        {
            if (thongTinDichVu.NhomGiaId != null)
            {
                if (thongTinDichVu.DichVuKhamBenhBenhVienId != null)
                {
                    var thongTinGia = _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                        .FirstOrDefault(x => x.DichVuKhamBenhBenhVienId == thongTinDichVu.DichVuKhamBenhBenhVienId
                                    && x.NhomGiaDichVuKhamBenhBenhVienId == thongTinDichVu.NhomGiaId
                                    && x.TuNgay.Date <= DateTime.Now.Date
                                    && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    if (thongTinGia != null)
                    {
                        thongTinDichVu.DonGia = thongTinGia.Gia;
                    }
                }
                else if (thongTinDichVu.DichVuKyThuatBenhVienId != null)
                {
                    var thongTinGia = _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                        .FirstOrDefault(x => x.DichVuKyThuatBenhVienId == thongTinDichVu.DichVuKyThuatBenhVienId
                                             && x.NhomGiaDichVuKyThuatBenhVienId == thongTinDichVu.NhomGiaId
                                             && x.TuNgay.Date <= DateTime.Now.Date
                                             && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    if (thongTinGia != null)
                    {
                        thongTinDichVu.DonGia = thongTinGia.Gia;
                    }
                }
                else if (thongTinDichVu.DichVuGiuongBenhVienId != null)
                {
                    var thongTinGia = _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking
                        .FirstOrDefault(x => x.DichVuGiuongBenhVienId == thongTinDichVu.DichVuGiuongBenhVienId
                                             && x.NhomGiaDichVuGiuongBenhVienId == thongTinDichVu.NhomGiaId
                                             && x.TuNgay.Date <= DateTime.Now.Date
                                             && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    if (thongTinGia != null)
                    {
                        thongTinDichVu.DonGia = thongTinGia.Gia;
                    }
                }
            }
        }
        public async Task<string> GetTenNhomGiaTheoLoaiDichVuAsync(NoiGioiThieuChiTietMienGiam noiGioiThieuChiTietMienGiam)
        {
            var tenNhomGia = string.Empty;
            if (noiGioiThieuChiTietMienGiam.NhomGiaDichVuKhamBenhBenhVienId != null)
            {
                tenNhomGia = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
                    .Where(x => x.Id == noiGioiThieuChiTietMienGiam.NhomGiaDichVuKhamBenhBenhVienId)
                    .Select(x => x.Ten)
                    .FirstOrDefault();
            }
            else if (noiGioiThieuChiTietMienGiam.NhomGiaDichVuKhamBenhBenhVienId != null)
            {
                tenNhomGia = _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(x => x.Id == noiGioiThieuChiTietMienGiam.NhomGiaDichVuKhamBenhBenhVienId)
                    .Select(x => x.Ten)
                    .FirstOrDefault();
            }
            else if (noiGioiThieuChiTietMienGiam.NhomGiaDichVuKhamBenhBenhVienId != null)
            {
                tenNhomGia = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking
                    .Where(x => x.Id == noiGioiThieuChiTietMienGiam.NhomGiaDichVuKhamBenhBenhVienId)
                    .Select(x => x.Ten)
                    .FirstOrDefault();
            }

            return tenNhomGia;
        }
        public async Task<NoiGioiThieuChiTietMienGiam> XuLyGetMienGiamDichVuAsync(long id)
        {
            var mienGiamDichVu = _noiGioiThieuChiTietMienGiamRepository.GetById(id);
            return mienGiamDichVu;
        }
        public async Task<GridDataSource> GetDataForGridChiTietMienGiamAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long noiGioiThieuId = 0;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                noiGioiThieuId = long.Parse(queryInfo.AdditionalSearchString);
            }

            var query = _noiGioiThieuChiTietMienGiamRepository.TableNoTracking
                .Where(x => x.NoiGioiThieuId == noiGioiThieuId)
                .Select(s => new NoiGioiThieuChiTietMienGiamGridVo
                {
                    Id = s.Id,
                    NoiGioiThieuId = s.NoiGioiThieuId,
                    MaDichVu = s.DichVuKhamBenhBenhVien.Ma ?? s.DichVuKyThuatBenhVien.Ma ?? s.DichVuGiuongBenhVien.Ma ?? s.DuocPhamBenhVien.MaDuocPhamBenhVien ?? s.VatTuBenhVien.MaVatTuBenhVien,
                    TenDichVu = s.DichVuKhamBenhBenhVien.Ten ?? s.DichVuKyThuatBenhVien.Ten ?? s.DichVuGiuongBenhVien.Ten ?? s.DuocPhamBenhVien.DuocPham.Ten ?? s.VatTuBenhVien.VatTus.Ten,
                    DichVuKhamBenhBenhVienId = s.DichVuKhamBenhBenhVienId,
                    DichVuKyThuatBenhVienId = s.DichVuKyThuatBenhVienId,
                    DichVuGiuongBenhVienId = s.DichVuGiuongBenhVienId,

                    TenNhomGia = s.NhomGiaDichVuKhamBenhBenhVien.Ten ?? s.NhomGiaDichVuKyThuatBenhVien.Ten ?? s.NhomGiaDichVuGiuongBenhVien.Ten,
                    NhomGiaDichVuKhamBenhBenhVienId = s.NhomGiaDichVuKhamBenhBenhVienId,
                    NhomGiaDichVuKyThuatBenhVienId = s.NhomGiaDichVuKyThuatBenhVienId,
                    NhomGiaDichVuGiuongBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    LoaiChietKhau = s.LoaiChietKhau,
                    TiLeChietKhau = s.TiLeChietKhau,
                    SoTienChietKhau = s.SoTienChietKhau,
                    GhiChu = s.GhiChu
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString)
                //.Skip(queryInfo.Skip).Take(queryInfo.Take)
                .ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            if (queryTask.Result.Length > 0)
            {
                var lstDichVuKhamId = queryTask.Result.Where(x => x.DichVuKhamBenhBenhVienId != null)
                    .Select(x => x.DichVuKhamBenhBenhVienId.Value).Distinct().ToList();
                var lstDichVuKyThuatId = queryTask.Result.Where(x => x.DichVuKyThuatBenhVienId != null)
                    .Select(x => x.DichVuKyThuatBenhVienId.Value).Distinct().ToList();
                var lstDichVuGiuongId = queryTask.Result.Where(x => x.DichVuGiuongBenhVienId != null)
                    .Select(x => x.DichVuGiuongBenhVienId.Value).Distinct().ToList();

                var lstGiaKham = new List<DichVuKhamBenhBenhVienGiaBenhVien>();
                if (lstDichVuKhamId.Any())
                {
                    lstGiaKham = _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                        .Where(x => lstDichVuKhamId.Contains(x.DichVuKhamBenhBenhVienId)
                                    && x.TuNgay.Date <= DateTime.Now.Date
                                    && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date))
                        .ToList();
                }

                var lstGiaKyThuat = new List<DichVuKyThuatBenhVienGiaBenhVien>();
                if (lstDichVuKyThuatId.Any())
                {
                    lstGiaKyThuat = _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                        .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId)
                                    && x.TuNgay.Date <= DateTime.Now.Date
                                    && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date))
                        .ToList();
                }

                var lstGiaGiuong = new List<DichVuGiuongBenhVienGiaBenhVien>();
                if (lstDichVuGiuongId.Any())
                {
                    lstGiaGiuong = _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking
                        .Where(x => lstDichVuGiuongId.Contains(x.DichVuGiuongBenhVienId)
                                    && x.TuNgay.Date <= DateTime.Now.Date
                                    && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date))
                        .ToList();
                }

                foreach (var dichVu in queryTask.Result)
                {
                    if (dichVu.DichVuKhamBenhBenhVienId != null)
                    {
                        var gia = lstGiaKham.FirstOrDefault(x =>
                            x.DichVuKhamBenhBenhVienId == dichVu.DichVuKhamBenhBenhVienId &&
                            x.NhomGiaDichVuKhamBenhBenhVienId == dichVu.NhomGiaDichVuKhamBenhBenhVienId);
                        if (gia != null)
                        {
                            dichVu.DonGia = gia.Gia;
                        }
                    }
                    else if (dichVu.DichVuKyThuatBenhVienId != null)
                    {
                        var gia = lstGiaKyThuat.FirstOrDefault(x =>
                            x.DichVuKyThuatBenhVienId == dichVu.DichVuKyThuatBenhVienId &&
                            x.NhomGiaDichVuKyThuatBenhVienId == dichVu.NhomGiaDichVuKyThuatBenhVienId);
                        if (gia != null)
                        {
                            dichVu.DonGia = gia.Gia;
                        }
                    }
                    else if (dichVu.DichVuGiuongBenhVienId != null)
                    {
                        var gia = lstGiaGiuong.FirstOrDefault(x =>
                            x.DichVuGiuongBenhVienId == dichVu.DichVuGiuongBenhVienId &&
                            x.NhomGiaDichVuGiuongBenhVienId == dichVu.NhomGiaDichVuGiuongBenhVienId);
                        if (gia != null)
                        {
                            dichVu.DonGia = gia.Gia;
                        }
                    }
                }
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridChiTietMienGiamAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long noiGioiThieuId = 0;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                noiGioiThieuId = long.Parse(queryInfo.AdditionalSearchString);
            }
            var query = _noiGioiThieuChiTietMienGiamRepository.TableNoTracking
                .Where(x => x.NoiGioiThieuId == noiGioiThieuId)
                .Select(s => new NoiGioiThieuChiTietMienGiamGridVo
                {
                    Id = s.Id
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                return await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(x => x.HieuLuc
                                && x.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                    .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DichVu = item.Ten,
                        Ma = item.Ma
                    }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                    .Take(model.Take)
                    .ToListAsync();
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");

                var lstDichVuKyThuatBenhVienId = await _dichVuBenhVienTongHopRepository
                    .ApplyFulltext(model.Query, nameof(DichVuBenhVienTongHop), lstColumnNameSearch)
                    .Where(p => p.LoaiDichVuBenhVien == Enums.EnumDichVuTongHop.KyThuat
                                && p.HieuLuc)
                    .Select(x => x.DichVuKyThuatBenhVienId)
                    .Take(model.Take).ToListAsync();

                return await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(x => lstDichVuKyThuatBenhVienId.Contains(x.Id)
                                && x.HieuLuc
                                && x.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                    .OrderBy(x => lstDichVuKyThuatBenhVienId.IndexOf(x.Id))
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DichVu = item.Ten,
                        Ma = item.Ma
                    })
                    .Take(model.Take)
                    .ToListAsync();
            }
        }
        public async Task<List<DichVuKyThuatTemplateVo>> GetDichVuGiuong(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                return await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .Where(x => x.HieuLuc
                                && x.DichVuGiuongBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                    .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DichVu = item.Ten,
                        Ma = item.Ma
                    }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                    .Take(model.Take)
                    .ToListAsync();
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");

                var lstDichVuGiuongBenhVienId = await _dichVuBenhVienTongHopRepository
                    .ApplyFulltext(model.Query, nameof(DichVuBenhVienTongHop), lstColumnNameSearch)
                    .Where(p => p.LoaiDichVuBenhVien == Enums.EnumDichVuTongHop.GiuongBenh
                                && p.HieuLuc)
                    .Select(x => x.DichVuGiuongBenhVienId)
                    .Take(model.Take).ToListAsync();

                return await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .Where(x => lstDichVuGiuongBenhVienId.Contains(x.Id)
                                && x.HieuLuc
                                && x.DichVuGiuongBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                    .OrderBy(x => lstDichVuGiuongBenhVienId.IndexOf(x.Id))
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DichVu = item.Ten,
                        Ma = item.Ma
                    })
                    .Take(model.Take)
                    .ToListAsync();
            }
        }
        public async Task<List<DichVuKyThuatTemplateVo>> GetDichVuKham(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                return await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                    .Where(x => x.HieuLuc
                                && x.DichVuKhamBenhBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                    .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DichVu = item.Ten,
                        Ma = item.Ma
                    }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                    .Take(model.Take)
                    .ToListAsync();
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");

                var lstDichVuKhamBenhVienId = await _dichVuBenhVienTongHopRepository
                    .ApplyFulltext(model.Query, nameof(DichVuBenhVienTongHop), lstColumnNameSearch)
                    .Where(p => p.LoaiDichVuBenhVien == Enums.EnumDichVuTongHop.KhamBenh
                                && p.HieuLuc)
                    .Select(x => x.DichVuKhamBenhBenhVienId)
                    .Take(model.Take).ToListAsync();

                return await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                    .Where(x => lstDichVuKhamBenhVienId.Contains(x.Id)
                                && x.HieuLuc
                                && x.DichVuKhamBenhBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                    .OrderBy(x => lstDichVuKhamBenhVienId.IndexOf(x.Id))
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DichVu = item.Ten,
                        Ma = item.Ma
                    })
                    .ToListAsync();
            }
        }
        public async Task<List<DichVuKyThuatTemplateVo>> GetDuocPham(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                return await _duocPhamBenhVienRepository.TableNoTracking
                    .Where(x => x.HieuLuc)
                    .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.DuocPham.Ten)
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.DuocPham.Ten,
                        KeyId = item.Id,
                        DichVu = item.DuocPham.Ten,
                        Ma = item.MaDuocPhamBenhVien
                    }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                    .Take(model.Take)
                    .ToListAsync();
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");

                var lstDuocPhamBenhVienId = await _duocPhamBenhVienRepository
                    .ApplyFulltext(model.Query, nameof(DichVuBenhVienTongHop), lstColumnNameSearch)
                    .Where(p => p.HieuLuc)
                    .Select(x => x.Id)
                    .Take(model.Take).ToListAsync();

                return await _duocPhamBenhVienRepository.TableNoTracking
                    .Where(x => lstDuocPhamBenhVienId.Contains(x.Id)
                                && x.HieuLuc)
                    .OrderBy(x => lstDuocPhamBenhVienId.IndexOf(x.Id))
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.DuocPham.Ten,
                        KeyId = item.Id,
                        DichVu = item.DuocPham.Ten,
                        Ma = item.MaDuocPhamBenhVien
                    })
                    .ToListAsync();
            }
        }
        public async Task<List<DichVuKyThuatTemplateVo>> GetVatTu(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                return await _vatTuBenhVienRepository.TableNoTracking
                    .Where(x => x.HieuLuc)
                    .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.VatTus.Ten)
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.VatTus.Ten,
                        KeyId = item.Id,
                        DichVu = item.VatTus.Ten,
                        Ma = item.MaVatTuBenhVien
                    }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                    .Take(model.Take)
                    .ToListAsync();
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");

                var lstVatTuBenhVienId = await _vatTuBenhVienRepository
                    .ApplyFulltext(model.Query, nameof(DichVuBenhVienTongHop), lstColumnNameSearch)
                    .Where(p => p.HieuLuc)
                    .Select(x => x.Id)
                    .Take(model.Take).ToListAsync();

                return await _vatTuBenhVienRepository.TableNoTracking
                    .Where(x => lstVatTuBenhVienId.Contains(x.Id)
                                && x.HieuLuc)
                    .OrderBy(x => lstVatTuBenhVienId.IndexOf(x.Id))
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        DisplayName = item.VatTus.Ten,
                        KeyId = item.Id,
                        DichVu = item.VatTus.Ten,
                        Ma = item.Ma
                    })
                    .ToListAsync();
            }
        }

        public async Task XuLyThemMienGiamDichVuAsync(NoiGioiThieuChiTietMienGiam noiGioiThieuChiTietMienGiam)
        {
            var noiGioiThieu = BaseRepository.TableNoTracking
                .FirstOrDefault(x => x.Id == noiGioiThieuChiTietMienGiam.NoiGioiThieuId);
            if (noiGioiThieu == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var checkExists = _noiGioiThieuChiTietMienGiamRepository.TableNoTracking
                .Any(x => x.NoiGioiThieuId == noiGioiThieu.Id
                            && x.DichVuKhamBenhBenhVienId == noiGioiThieuChiTietMienGiam.DichVuKhamBenhBenhVienId
                            && x.NhomGiaDichVuKhamBenhBenhVienId == noiGioiThieuChiTietMienGiam.NhomGiaDichVuKhamBenhBenhVienId
                            && x.DichVuKyThuatBenhVienId == noiGioiThieuChiTietMienGiam.DichVuKyThuatBenhVienId
                            && x.NhomGiaDichVuKyThuatBenhVienId == noiGioiThieuChiTietMienGiam.NhomGiaDichVuKyThuatBenhVienId
                            && x.DichVuGiuongBenhVienId == noiGioiThieuChiTietMienGiam.DichVuGiuongBenhVienId
                            && x.NhomGiaDichVuGiuongBenhVienId == noiGioiThieuChiTietMienGiam.NhomGiaDichVuGiuongBenhVienId
                            && x.DuocPhamBenhVienId == noiGioiThieuChiTietMienGiam.DuocPhamBenhVienId
                            && x.VatTuBenhVienId == noiGioiThieuChiTietMienGiam.VatTuBenhVienId
                            );
            if (checkExists)
            {
                throw new Exception(_localizationService.GetResource("NoiGioiThieuChiTietMienGiam.DichVuKhuyenMai.IsExists"));
            }

            _noiGioiThieuChiTietMienGiamRepository.Add(noiGioiThieuChiTietMienGiam);
        }
        public async Task XuLyCapNhatMienGiamDichVuAsync(NoiGioiThieuChiTietMienGiam noiGioiThieuChiTietMienGiam)
        {
            _noiGioiThieuChiTietMienGiamRepository.Context.SaveChanges();
        }
        public async Task XuLyXoaMienGiamDichVuAsync(long id)
        {
            var mienGiamDichVu = _noiGioiThieuChiTietMienGiamRepository.GetById(id);
            _noiGioiThieuChiTietMienGiamRepository.Delete(mienGiamDichVu);
        }
        #endregion

        #region BVHD-3936
        public async Task<NoiGioiThieuDataImportVo> XuLyKiemTraDataDichVuMienGiamImportAsync(NoiGioiThieuFileImportVo info)
        {
            var result = new NoiGioiThieuDataImportVo();
            var datas = new List<ThongTinDichVuMienGiamTuFileExcelVo>();

            #region get data từ file excel
            using (ExcelPackage package = new ExcelPackage(info.Path))
            {
                ExcelWorksheet workSheetDichVuKham = package.Workbook.Worksheets["Dịch vụ khám"];
                ExcelWorksheet workSheetDichVuKyThuat = package.Workbook.Worksheets["Dịch vụ kỹ thuật"];
                ExcelWorksheet workSheetDichVuGiuong = package.Workbook.Worksheets["Dịch vụ giường"];
                ExcelWorksheet workSheetThuoc = package.Workbook.Worksheets["Thuốc"];
                ExcelWorksheet workSheetVatTu = package.Workbook.Worksheets["Vật tư"];
                if (workSheetDichVuKham == null || workSheetDichVuKyThuat == null || workSheetDichVuGiuong == null || workSheetThuoc == null || workSheetVatTu == null)
                {
                    throw new Exception("Thông tin file không đúng");
                }

                #region workSheetDichVuKham
                int totalRowKham = workSheetDichVuKham.Dimension.Rows; // count row có data
                if (totalRowKham >= 2) // row 1 là title, data bắt đầu từ row 2
                {
                    for (int i = 2; i <= totalRowKham; i++)
                    {
                        var dichVu = GanDataTheoRow(workSheetDichVuKham, i, Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
                        if (dichVu == null)
                        {
                            break;
                        }
                        datas.Add(dichVu);
                    }
                }
                #endregion
                #region workSheetDichVuKyThuat
                int totalRowKyThuat = workSheetDichVuKyThuat.Dimension.Rows; // count row có data
                if (totalRowKyThuat >= 2) // row 1 là title, data bắt đầu từ row 2
                {
                    for (int i = 2; i <= totalRowKyThuat; i++)
                    {
                        var dichVu = GanDataTheoRow(workSheetDichVuKyThuat, i, Enums.EnumNhomGoiDichVu.DichVuKyThuat);
                        if (dichVu == null)
                        {
                            break;
                        }
                        datas.Add(dichVu);
                    }
                }
                #endregion
                #region workSheetDichVuGiuong
                int totalRowGiuong = workSheetDichVuGiuong.Dimension.Rows; // count row có data
                if (totalRowGiuong >= 2) // row 1 là title, data bắt đầu từ row 2
                {
                    for (int i = 2; i <= totalRowGiuong; i++)
                    {
                        var dichVu = GanDataTheoRow(workSheetDichVuGiuong, i, Enums.EnumNhomGoiDichVu.DichVuGiuongBenh);
                        if (dichVu == null)
                        {
                            break;
                        }
                        datas.Add(dichVu);
                    }
                }
                #endregion
                #region workSheetThuoc
                int totalRowThuoc = workSheetThuoc.Dimension.Rows; // count row có data
                if (totalRowThuoc >= 2) // row 1 là title, data bắt đầu từ row 2
                {
                    for (int i = 2; i <= totalRowThuoc; i++)
                    {
                        var dichVu = GanDataTheoRow(workSheetThuoc, i, Enums.EnumNhomGoiDichVu.DuocPham);
                        if (dichVu == null)
                        {
                            break;
                        }
                        datas.Add(dichVu);
                    }
                }
                #endregion
                #region workSheetVatTu
                int totalRowVatTu = workSheetVatTu.Dimension.Rows; // count row có data
                if (totalRowVatTu >= 2) // row 1 là title, data bắt đầu từ row 2
                {
                    for (int i = 2; i <= totalRowVatTu; i++)
                    {
                        var dichVu = GanDataTheoRow(workSheetVatTu, i, Enums.EnumNhomGoiDichVu.VatTuTieuHao);
                        if (dichVu == null)
                        {
                            break;
                        }
                        datas.Add(dichVu);
                    }
                }
                #endregion
            }
            #endregion

            #region Kiểm tra data
            if (datas.Any())
            {
                await KiemTraDataDichVuMienGiamImportAsync(datas, result, info.NoiGioiThieuId);
            }
            #endregion

            return result;
        }

        private ThongTinDichVuMienGiamTuFileExcelVo GanDataTheoRow(ExcelWorksheet workSheet, int index, Enums.EnumNhomGoiDichVu nhomDichVu)
        {
            var dichVu = new ThongTinDichVuMienGiamTuFileExcelVo()
            {
                NhomDichVu = nhomDichVu,
                MaDichVuBenhVien = workSheet.Cells[index, 1].Text?.Trim(),
                TenDichVuBenhVien = workSheet.Cells[index, 2].Text?.Trim(),
                //LoaiGia = workSheet.Cells[index, 3].Text,
                //TiLeChietKhau = workSheet.Cells[index, 4].Text,
                //SoTienChietKhau = workSheet.Cells[index, 5].Text,
                //GhiChu = workSheet.Cells[index, 6].Text
            };

            if (nhomDichVu == Enums.EnumNhomGoiDichVu.DuocPham || nhomDichVu == Enums.EnumNhomGoiDichVu.VatTuTieuHao)
            {
                dichVu.TiLeChietKhau = workSheet.Cells[index, 3].Text?.Trim();
                dichVu.GhiChu = workSheet.Cells[index, 4].Text.Trim();
            }
            else
            {
                dichVu.LoaiGia = workSheet.Cells[index, 3].Text.Trim();
                dichVu.TiLeChietKhau = workSheet.Cells[index, 4].Text.Trim();
                dichVu.SoTienChietKhau = workSheet.Cells[index, 5].Text.Trim();
                dichVu.GhiChu = workSheet.Cells[index, 6].Text.Trim();
            }

            if (string.IsNullOrEmpty(dichVu.MaDichVuBenhVien)
                && string.IsNullOrEmpty(dichVu.TenDichVuBenhVien)
                && string.IsNullOrEmpty(dichVu.LoaiGia)
                && string.IsNullOrEmpty(dichVu.TiLeChietKhau)
                && string.IsNullOrEmpty(dichVu.SoTienChietKhau)
                && string.IsNullOrEmpty(dichVu.GhiChu))
            {
                dichVu = null;
            }
            return dichVu;
        }

        public async Task KiemTraDataDichVuMienGiamImportAsync(List<ThongTinDichVuMienGiamTuFileExcelVo> datas, NoiGioiThieuDataImportVo result, long? noiGioiThieuId)
        {
            if (datas.Any())
            {
                var lstLookupDichVu = datas
                    .Select(x => new Core.Domain.ValueObject.NoiGioiThieu.LookupDichVuBenhVienVo()
                    {
                        NhomDichVu = x.NhomDichVu,
                        MaDichVu = x.MaDichVuBenhVien?.Trim().ToLower(),
                        TenDichVu = x.TenDichVuBenhVien?.Trim().ToLower()
                    }).Distinct().ToList();

                var lstLookupKham = lstLookupDichVu.Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh).ToList();
                var lstLookupKyThuat = lstLookupDichVu.Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat).ToList();
                var lstLookupGiuong = lstLookupDichVu.Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh).ToList();
                var lstLookupThuoc = lstLookupDichVu.Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DuocPham).ToList();
                var lstLookupVatTu = lstLookupDichVu.Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.VatTuTieuHao).ToList();

                var lstDichVuBenhVien = new List<Core.Domain.ValueObject.NoiGioiThieu.ThongTinDichVuBenhVienVo>();
                if (lstLookupKham.Any())
                {
                    var lstDichVuKhamBenhVien = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                        .Select(x => new Core.Domain.ValueObject.NoiGioiThieu.ThongTinDichVuBenhVienVo()
                        {
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                            DichVuBenhVienId = x.Id,
                            MaDichVu = x.Ma,
                            TenDichVu = x.Ten
                        }).ToList();
                    lstDichVuKhamBenhVien = lstDichVuKhamBenhVien
                        .Where(x => lstLookupKham.Any(a => a.MaDichVu.Equals(x.MaDichVu.Trim().ToLower())
                                                              && a.TenDichVu.Equals(x.TenDichVu.Trim().ToLower())))
                        .ToList();
                    if (lstDichVuKhamBenhVien.Any())
                    {
                        var lstDichVuId = lstDichVuKhamBenhVien.Select(x => x.DichVuBenhVienId).Distinct().ToList();
                        var lstGiaBenhVien = _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                            .Where(x => lstDichVuId.Contains(x.DichVuKhamBenhBenhVienId))
                            .Select(a => new Core.Domain.ValueObject.NoiGioiThieu.ThongTinGiaBenhVienVo()
                            {
                                DichVuBenhVienId = a.DichVuKhamBenhBenhVienId,
                                NhomGiaId = a.NhomGiaDichVuKhamBenhBenhVienId,
                                TenLoaiGia = a.NhomGiaDichVuKhamBenhBenhVien.Ten,
                                Gia = a.Gia,
                                TuNgay = a.TuNgay,
                                DenNgay = a.DenNgay
                            })
                            .ToList();
                        foreach (var dichVuBenhVien in lstDichVuKhamBenhVien)
                        {
                            var lstGia = lstGiaBenhVien.Where(x => x.DichVuBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            dichVuBenhVien.ThongTinGias = lstGia;
                        }
                        lstDichVuBenhVien = lstDichVuBenhVien.Concat(lstDichVuKhamBenhVien).ToList();
                    }
                }
                if (lstLookupKyThuat.Any())
                {
                    var lstDichVuKyThuatBenhVien = _dichVuKyThuatBenhVienRepository.TableNoTracking
                        .Select(x => new Core.Domain.ValueObject.NoiGioiThieu.ThongTinDichVuBenhVienVo()
                        {
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                            DichVuBenhVienId = x.Id,
                            MaDichVu = x.Ma,
                            TenDichVu = x.Ten
                        }).ToList();

                    lstDichVuKyThuatBenhVien = lstDichVuKyThuatBenhVien
                        .Where(x => lstLookupKyThuat.Any(a => a.MaDichVu.Equals(x.MaDichVu.Trim().ToLower())
                                                              && a.TenDichVu.Equals(x.TenDichVu.Trim().ToLower())))
                        .ToList();
                    if (lstDichVuKyThuatBenhVien.Any())
                    {
                        var lstDichVuId = lstDichVuKyThuatBenhVien.Select(x => x.DichVuBenhVienId).Distinct().ToList();
                        var lstGiaBenhVien = _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                            .Where(x => lstDichVuId.Contains(x.DichVuKyThuatBenhVienId))
                            .Select(a => new Core.Domain.ValueObject.NoiGioiThieu.ThongTinGiaBenhVienVo()
                            {
                                DichVuBenhVienId = a.DichVuKyThuatBenhVienId,
                                NhomGiaId = a.NhomGiaDichVuKyThuatBenhVienId,
                                TenLoaiGia = a.NhomGiaDichVuKyThuatBenhVien.Ten,
                                Gia = a.Gia,
                                TuNgay = a.TuNgay,
                                DenNgay = a.DenNgay
                            })
                            .ToList();
                        foreach (var dichVuBenhVien in lstDichVuKyThuatBenhVien)
                        {
                            var lstGia = lstGiaBenhVien.Where(x => x.DichVuBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            dichVuBenhVien.ThongTinGias = lstGia;
                        }
                        lstDichVuBenhVien = lstDichVuBenhVien.Concat(lstDichVuKyThuatBenhVien).ToList();
                    }
                }
                if (lstLookupGiuong.Any())
                {
                    var lstDichVuGiuongBenhVien = _dichVuGiuongBenhVienRepository.TableNoTracking
                        .Select(x => new Core.Domain.ValueObject.NoiGioiThieu.ThongTinDichVuBenhVienVo()
                        {
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                            DichVuBenhVienId = x.Id,
                            MaDichVu = x.Ma,
                            TenDichVu = x.Ten
                        }).ToList();
                    lstDichVuGiuongBenhVien = lstDichVuGiuongBenhVien
                        .Where(x => lstLookupGiuong.Any(a => a.MaDichVu.Equals(x.MaDichVu.Trim().ToLower())
                                                              && a.TenDichVu.Equals(x.TenDichVu.Trim().ToLower())))
                        .ToList();
                    if (lstDichVuGiuongBenhVien.Any())
                    {
                        var lstDichVuId = lstDichVuGiuongBenhVien.Select(x => x.DichVuBenhVienId).Distinct().ToList();
                        var lstGiaBenhVien = _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking
                            .Where(x => lstDichVuId.Contains(x.DichVuGiuongBenhVienId))
                            .Select(a => new Core.Domain.ValueObject.NoiGioiThieu.ThongTinGiaBenhVienVo()
                            {
                                DichVuBenhVienId = a.DichVuGiuongBenhVienId,
                                NhomGiaId = a.NhomGiaDichVuGiuongBenhVienId,
                                TenLoaiGia = a.NhomGiaDichVuGiuongBenhVien.Ten,
                                Gia = a.Gia,
                                TuNgay = a.TuNgay,
                                DenNgay = a.DenNgay
                            })
                            .ToList();
                        foreach (var dichVuBenhVien in lstDichVuGiuongBenhVien)
                        {
                            var lstGia = lstGiaBenhVien.Where(x => x.DichVuBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            dichVuBenhVien.ThongTinGias = lstGia;
                        }
                        lstDichVuBenhVien = lstDichVuBenhVien.Concat(lstDichVuGiuongBenhVien).ToList();
                    }
                }
                if (lstLookupThuoc.Any())
                {
                    var lstThuocBenhVien = _duocPhamBenhVienRepository.TableNoTracking
                        .Select(x => new Core.Domain.ValueObject.NoiGioiThieu.ThongTinDichVuBenhVienVo()
                        {
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DuocPham,
                            DichVuBenhVienId = x.Id,
                            MaDichVu = x.Ma,
                            TenDichVu = x.DuocPham.Ten
                        }).ToList();
                    lstThuocBenhVien = lstThuocBenhVien
                        .Where(x => lstLookupThuoc.Any(a => a.MaDichVu.Equals(x.MaDichVu.Trim().ToLower())
                                                              && a.TenDichVu.Equals(x.TenDichVu.Trim().ToLower())))
                        .ToList();
                    if (lstThuocBenhVien.Any())
                    {
                        lstDichVuBenhVien = lstDichVuBenhVien.Concat(lstThuocBenhVien).ToList();
                    }
                }
                if (lstLookupVatTu.Any())
                {
                    var lstVatTuBenhVien = _vatTuBenhVienRepository.TableNoTracking
                        .Select(x => new Core.Domain.ValueObject.NoiGioiThieu.ThongTinDichVuBenhVienVo()
                        {
                            NhomDichVu = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                            DichVuBenhVienId = x.Id,
                            MaDichVu = x.Ma,
                            TenDichVu = x.VatTus.Ten
                        }).ToList();
                    lstVatTuBenhVien = lstVatTuBenhVien
                        .Where(x => lstLookupVatTu.Any(a => a.MaDichVu.Equals(x.MaDichVu.Trim().ToLower())
                                                              && a.TenDichVu.Equals(x.TenDichVu.Trim().ToLower())))
                        .ToList();
                    if (lstVatTuBenhVien.Any())
                    {
                        lstDichVuBenhVien = lstDichVuBenhVien.Concat(lstVatTuBenhVien).ToList();
                    }
                }

                var lstDichVuMienGiamDaChiDinh = new List<NoiGioiThieuChiTietMienGiamVo>();
                if (noiGioiThieuId != null)
                {
                    lstDichVuMienGiamDaChiDinh = _noiGioiThieuChiTietMienGiamRepository.TableNoTracking
                        .Where(x => x.NoiGioiThieuId == noiGioiThieuId.Value)
                        .Select(x => new NoiGioiThieuChiTietMienGiamVo()
                        {
                            DichVuKhamBenhBenhVienId = x.DichVuKhamBenhBenhVienId,
                            DichVuKyThuatBenhVienId = x.DichVuKyThuatBenhVienId,
                            DichVuGiuongBenhVienId = x.DichVuGiuongBenhVienId,
                            DuocPhamBenhVienId = x.DuocPhamBenhVienId,
                            VatTuBenhVienId = x.VatTuBenhVienId,
                            NhomGiaDichVuKhamBenhBenhVienId = x.NhomGiaDichVuKhamBenhBenhVienId,
                            NhomGiaDichVuKyThuatBenhVienId = x.NhomGiaDichVuKyThuatBenhVienId,
                            NhomGiaDichVuGiuongBenhVienId = x.NhomGiaDichVuGiuongBenhVienId
                        }).ToList();
                }

                var cauHinhNhomGiaThuongKhamBenh = _cauHinhService.GetSetting("CauHinhDichVuKhamBenh.NhomGiaThuong");
                long.TryParse(cauHinhNhomGiaThuongKhamBenh?.Value, out long nhomGiaThuongKhamBenhId);
                var cauHinhNhomGiaThuongKyThuat = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
                long.TryParse(cauHinhNhomGiaThuongKyThuat?.Value, out long nhomGiaThuongKyThuatId);
                var cauHinhNhomGiaThuongGiuong = _cauHinhService.GetSetting("CauHinhDichVuGiuong.NhomGiaThuong");
                long.TryParse(cauHinhNhomGiaThuongGiuong?.Value, out long nhomGiaThuongGiuongId);
                foreach (var dichVu in datas)
                {
                    dichVu.ValidationErrors = new List<ValidationErrorDichVuKhuyenMaiVo>();

                    #region Gán lại thông tin giá cho data dịch vụ từ file excel

                    var thongTinDichVu = lstDichVuBenhVien.FirstOrDefault(x => x.NhomDichVu == dichVu.NhomDichVu 
                                                                               && x.MaDichVu.Trim().Equals(dichVu.MaDichVuBenhVien)
                                                                               && x.TenDichVu.Trim().Equals(dichVu.TenDichVuBenhVien));
                    if (thongTinDichVu != null)
                    {
                        dichVu.DichVuBenhVienId = thongTinDichVu.DichVuBenhVienId;

                        if (dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh
                            || dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat
                            || dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                        {
                            var thongTinGias = thongTinDichVu.ThongTinGias
                                .Where(x => !string.IsNullOrEmpty(dichVu.LoaiGia)
                                            && x.TenLoaiGia.ToLower().Equals(dichVu.LoaiGia.ToLower())
                                            && x.TuNgay.Date <= DateTime.Now.Date
                                            && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date))
                                .ToList();
                            if (thongTinGias.Any())
                            {
                                Core.Domain.ValueObject.NoiGioiThieu.ThongTinGiaBenhVienVo gia = null;
                                var nhomGiaThuongId = nhomGiaThuongKhamBenhId;
                                if (dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                                {
                                    nhomGiaThuongId = nhomGiaThuongKyThuatId;
                                }
                                else if (dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                                {
                                    nhomGiaThuongId = nhomGiaThuongGiuongId;
                                }

                                gia = thongTinGias.OrderByDescending(x => x.NhomGiaId == nhomGiaThuongId).FirstOrDefault();
                                if (gia != null)
                                {
                                    dichVu.GiaBenhVien = gia.Gia;
                                    dichVu.LoaiGiaId = gia.NhomGiaId;
                                    dichVu.LoaiGia = gia.TenLoaiGia;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(dichVu.LoaiGia))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                                    {
                                        Field = nameof(dichVu.LoaiGia),
                                        Message = "Loại giá không có hiệu lực"
                                    });
                                }
                            }
                        }
                    }


                    #endregion
                }
                foreach (var dichVu in datas)
                {
                    #region Kiểm tra yêu cầu nhập và format dữ liệu
                    var laDichVuCoLoaiGia = (dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh
                                             || dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat
                                             || dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh);

                    if (string.IsNullOrEmpty(dichVu.MaDichVuBenhVien))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                        {
                            Field = nameof(dichVu.MaDichVuBenhVien),
                            Message = "Mã dịch vụ bệnh viện yêu cầu nhập"
                        });
                    }

                    if (string.IsNullOrEmpty(dichVu.TenDichVuBenhVien))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                        {
                            Field = nameof(dichVu.TenDichVuBenhVien),
                            Message = "Tên dịch vụ bệnh viện yêu cầu nhập"
                        });
                    }

                    // chỉ có dv khám, kỹ thuật, giường mới có loại giá
                    if (string.IsNullOrEmpty(dichVu.LoaiGia) && laDichVuCoLoaiGia)
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                        {
                            Field = nameof(dichVu.LoaiGia),
                            Message = "Loại giá yêu cầu nhập"
                        });
                    }

                    if (string.IsNullOrEmpty(dichVu.TiLeChietKhau) && string.IsNullOrEmpty(dichVu.SoTienChietKhau))
                    {
                        var mess = "Tỉ lệ chiết khâu yêu cầu nhập";
                        if (laDichVuCoLoaiGia)
                        {
                            mess = "Yêu cầu nhập tỉ lệ chiết khấu hoặc số tiền chiết khấu";
                        }

                        dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                        {
                            Field = nameof(dichVu.TiLeChietKhau),
                            Message = mess
                        });
                    }

                    if (!string.IsNullOrEmpty(dichVu.TiLeChietKhau))
                    {
                        var isNumeric = int.TryParse(dichVu.TiLeChietKhau, out int tiLeChietKhau);
                        if (isNumeric)
                        {
                            dichVu.TiLeChietKhauValue = tiLeChietKhau;

                            if (tiLeChietKhau > 100 && !KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TiLeChietKhau)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                                {
                                    Field = nameof(dichVu.TiLeChietKhau),
                                    Message = "Tỉ lệ chiết khấu không được lớn hơn 100"
                                });
                            }
                        }
                        else
                        {
                            if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TiLeChietKhau)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                                {
                                    Field = nameof(dichVu.TiLeChietKhau),
                                    Message = "Tỉ lệ chiết khấu phải nhập số nguyên"
                                });
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(dichVu.SoTienChietKhau))
                    {
                        if (laDichVuCoLoaiGia)
                        {
                            var isNumeric = decimal.TryParse(dichVu.SoTienChietKhau, out decimal soTienChietKhau);
                            if (isNumeric)
                            {
                                dichVu.SoTienChietKhauValue = soTienChietKhau;
                            }
                            else
                            {
                                if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.SoTienChietKhau)))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                                    {
                                        Field = nameof(dichVu.SoTienChietKhau),
                                        Message = "Số tiền chiết khấu nhập sai định dạng"
                                    });
                                }
                            }

                            if (dichVu.GiaBenhVien != null
                                && dichVu.SoTienChietKhauValue != null
                                && dichVu.GiaBenhVien < dichVu.SoTienChietKhauValue
                                && !KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.SoTienChietKhau)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                                {
                                    Field = nameof(dichVu.SoTienChietKhau),
                                    Message = "Số tiền chiết khấu Không được lớn hơn giá bệnh viện"
                                });
                            }
                        }
                        else
                        {
                            if(!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.SoTienChietKhau)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                                {
                                    Field = nameof(dichVu.SoTienChietKhau),
                                    Message = "Thuốc và vật tư chỉ được nhập tỉ lệ chiết khấu"
                                });
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(dichVu.GhiChu) && dichVu.GhiChu.Length > 1000)
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                        {
                            Field = nameof(dichVu.GhiChu),
                            Message = "Ghi chú không được nhập quá 1000 ký tự"
                        });
                    }
                    #endregion

                    #region Kiểm tra trùng dữ liệu

                    //Cập nhật 10/06/2022: fix bug dịch vụ không tồn tại
                    if (dichVu.DichVuBenhVienId == null && !KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TenDichVuBenhVien)))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                        {
                            Field = nameof(dichVu.TenDichVuBenhVien),
                            Message = "Mã hoặc tên dịch vụ không đúng"
                        });
                    }
                    //====================================


                    var trungDichVu = datas.Count(x => x.NhomDichVu == dichVu.NhomDichVu 
                                                     && x.DichVuBenhVienId == dichVu.DichVuBenhVienId 
                                                     && x.LoaiGiaId == dichVu.LoaiGiaId) > 1;
                    if (!trungDichVu)
                    {
                        trungDichVu = lstDichVuMienGiamDaChiDinh.Count(x => x.NhomDichVu == dichVu.NhomDichVu 
                                                                          && x.DichVuBenhVienId == dichVu.DichVuBenhVienId
                                                                          && (dichVu.LoaiGiaId == null 
                                                                              || x.NhomGiaDichVuKhamBenhBenhVienId == dichVu.LoaiGiaId 
                                                                              || x.NhomGiaDichVuKyThuatBenhVienId == dichVu.LoaiGiaId
                                                                              || x.NhomGiaDichVuGiuongBenhVienId == dichVu.LoaiGiaId)) > 0;
                    }

                    if (trungDichVu && !KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TenDichVuBenhVien)))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorDichVuKhuyenMaiVo()
                        {
                            Field = nameof(dichVu.TenDichVuBenhVien),
                            Message = "Dịch vụ đã tồn tại"
                        });
                    }
                    #endregion
                }

                result.DuLieuDungs = datas.Where(x => !x.ValidationErrors.Any()).OrderBy(x => x.NhomDichVu).ToList();
                result.DuLieuSais = datas.Where(x => x.ValidationErrors.Any()).OrderBy(x => x.NhomDichVu).ToList();
            }
        }

        private bool KiemTraDaCoValidationErrors(List<ValidationErrorDichVuKhuyenMaiVo> validationErrors, string filed)
        {
            return validationErrors.Any(a => a.Field.Equals(filed));
        }

        public async Task<List<NoiGioiThieuChiTietMienGiamGridVo>> XuLyLuuDichVuMienGiamImportAsync(List<ThongTinDichVuMienGiamTuFileExcelVo> datas, long? noiGioiThieuId)
        {
            var gridDataDichVuMienGiam = new List<NoiGioiThieuChiTietMienGiamGridVo>();

            Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu noiGioiThieu = null;
            if (noiGioiThieuId != null)
            {
                noiGioiThieu = BaseRepository.GetById(noiGioiThieuId.Value,
                    x => x.Include(a => a.NoiGioiThieuChiTietMienGiams));
            }

            foreach (var dichVu in datas)
            {
                var thongTinMienGiam = new NoiGioiThieuChiTietMienGiam()
                {
                    TiLeChietKhau = dichVu.TiLeChietKhauValue,
                    SoTienChietKhau = dichVu.SoTienChietKhauValue,
                    GhiChu = dichVu.GhiChu
                };

                if (thongTinMienGiam.TiLeChietKhau != null)
                {
                    thongTinMienGiam.LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe;
                }
                else if (thongTinMienGiam.SoTienChietKhau != null)
                {
                    thongTinMienGiam.LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien;
                }

                if (dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    thongTinMienGiam.DichVuKhamBenhBenhVienId = dichVu.DichVuBenhVienId;
                    thongTinMienGiam.NhomGiaDichVuKhamBenhBenhVienId = dichVu.LoaiGiaId;
                }
                else if (dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                {
                    thongTinMienGiam.DichVuKyThuatBenhVienId = dichVu.DichVuBenhVienId;
                    thongTinMienGiam.NhomGiaDichVuKyThuatBenhVienId = dichVu.LoaiGiaId;
                }
                else if (dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                {
                    thongTinMienGiam.DichVuGiuongBenhVienId = dichVu.DichVuBenhVienId;
                    thongTinMienGiam.NhomGiaDichVuGiuongBenhVienId = dichVu.LoaiGiaId;
                }
                else if (dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.DuocPham)
                {
                    thongTinMienGiam.DuocPhamBenhVienId = dichVu.DichVuBenhVienId;
                }
                else if (dichVu.NhomDichVu == Enums.EnumNhomGoiDichVu.VatTuTieuHao)
                {
                    thongTinMienGiam.VatTuBenhVienId = dichVu.DichVuBenhVienId;
                }

                if (noiGioiThieu != null)
                {
                    var checkExists = noiGioiThieu.NoiGioiThieuChiTietMienGiams
                        .Any(x => x.NoiGioiThieuId == noiGioiThieu.Id
                                  && x.DichVuKhamBenhBenhVienId == thongTinMienGiam.DichVuKhamBenhBenhVienId
                                  && x.NhomGiaDichVuKhamBenhBenhVienId == thongTinMienGiam.NhomGiaDichVuKhamBenhBenhVienId
                                  && x.DichVuKyThuatBenhVienId == thongTinMienGiam.DichVuKyThuatBenhVienId
                                  && x.NhomGiaDichVuKyThuatBenhVienId == thongTinMienGiam.NhomGiaDichVuKyThuatBenhVienId
                                  && x.DichVuGiuongBenhVienId == thongTinMienGiam.DichVuGiuongBenhVienId
                                  && x.NhomGiaDichVuGiuongBenhVienId == thongTinMienGiam.NhomGiaDichVuGiuongBenhVienId
                                  && x.DuocPhamBenhVienId == thongTinMienGiam.DuocPhamBenhVienId
                                  && x.VatTuBenhVienId == thongTinMienGiam.VatTuBenhVienId
                        );
                    if (checkExists)
                    {
                        throw new Exception(_localizationService.GetResource("NoiGioiThieuChiTietMienGiam.DichVuKhuyenMai.IsExists"));
                    }

                    noiGioiThieu.NoiGioiThieuChiTietMienGiams.Add(thongTinMienGiam);
                }

                #region gán data vào grid đối với trường hợp tạo mới nơi giới thiệu

                var gridItem = new NoiGioiThieuChiTietMienGiamGridVo
                {
                    NoiGioiThieuId = noiGioiThieuId,
                    MaDichVu = dichVu.MaDichVuBenhVien,
                    TenDichVu = dichVu.TenDichVuBenhVien,
                    DichVuKhamBenhBenhVienId = thongTinMienGiam.DichVuKhamBenhBenhVienId,
                    DichVuKyThuatBenhVienId = thongTinMienGiam.DichVuKyThuatBenhVienId,
                    DichVuGiuongBenhVienId = thongTinMienGiam.DichVuGiuongBenhVienId,
                    TenNhomGia = dichVu.LoaiGia,
                    NhomGiaDichVuKhamBenhBenhVienId = thongTinMienGiam.NhomGiaDichVuKhamBenhBenhVienId,
                    NhomGiaDichVuKyThuatBenhVienId = thongTinMienGiam.NhomGiaDichVuKyThuatBenhVienId,
                    NhomGiaDichVuGiuongBenhVienId = thongTinMienGiam.NhomGiaDichVuGiuongBenhVienId,
                    DuocPhamBenhVienId = thongTinMienGiam.DuocPhamBenhVienId,
                    VatTuBenhVienId = thongTinMienGiam.VatTuBenhVienId,
                    LoaiChietKhau = thongTinMienGiam.LoaiChietKhau,
                    TiLeChietKhau = thongTinMienGiam.TiLeChietKhau,
                    SoTienChietKhau = thongTinMienGiam.SoTienChietKhau,
                    GhiChu = thongTinMienGiam.GhiChu,
                    DonGia = dichVu.GiaBenhVien
                };
                gridDataDichVuMienGiam.Add(gridItem);
                #endregion
            }

            if (noiGioiThieu != null)
            {
                BaseRepository.Context.SaveChanges();
            }

            return gridDataDichVuMienGiam;
        }
        #endregion
    }
}
