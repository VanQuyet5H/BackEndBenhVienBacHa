using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.ValueObject.DanhMucMarketing;

namespace Camino.Services.YeuCauTiepNhans
{
    [ScopedDependency(ServiceType = typeof(IThuNganMarketingService))]
    public class ThuNganMarketingService : MasterFileService<BenhNhan>, IThuNganMarketingService
    {
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuRepository;
        private readonly IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<User> _useRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;

        private readonly IRepository<ChuongTrinhGoiDichVuDichVuGiuong> _chuongTrinhGoiDichVuDichVuGiuongRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> _chuongTrinhGoiDichVuDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> _chuongTrinhGoiDichVuDichVuKyThuatRepository;
        private readonly IRepository<ChuongTrinhGoiDichVu> _chuongTrinhGoiDichVuRepository;

        public ThuNganMarketingService(IRepository<BenhNhan> repository, ICauHinhService cauHinhService, IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository, IRepository<User> useRepository, IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository
            , IUserAgentHelper userAgentHelper, IRepository<User> userRepository, IRepository<TaiKhoanBenhNhanThu> taiKhoanBenhNhanThuRepository,
            IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChiRepository, IRepository<Template> templateRepository,
            IRepository<ChuongTrinhGoiDichVuDichVuGiuong> chuongTrinhGoiDichVuDichVuGiuongRepository,
            IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> chuongTrinhGoiDichVuDichVuKhamBenhRepository,
            IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> chuongTrinhGoiDichVuDichVuKyThuatRepository,
            IRepository<ChuongTrinhGoiDichVu> chuongTrinhGoiDichVuRepository
            )
            : base(repository)
        {
            _cauHinhService = cauHinhService;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _userAgentHelper = userAgentHelper;
            _userRepository = userRepository;
            _taiKhoanBenhNhanThuRepository = taiKhoanBenhNhanThuRepository;
            _templateRepository = templateRepository;
            _useRepository = useRepository;
            _taiKhoanBenhNhanChiRepository = taiKhoanBenhNhanChiRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;

            _chuongTrinhGoiDichVuDichVuGiuongRepository = chuongTrinhGoiDichVuDichVuGiuongRepository;
            _chuongTrinhGoiDichVuDichVuKhamBenhRepository = chuongTrinhGoiDichVuDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuDichVuKyThuatRepository = chuongTrinhGoiDichVuDichVuKyThuatRepository;
            _chuongTrinhGoiDichVuRepository = chuongTrinhGoiDichVuRepository;
        }

        #region Chuyển gói dịch vụ      

        public ICollection<LookupItemTemplateVo> GetListGoiMarketingChuyenGoi(DropDownListRequestModel queryInfo, long yeuCauGoiDichVuId)
        {
            var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.GetById(yeuCauGoiDichVuId);
            var dateTimeNow = DateTime.Now.Date;
            var getMarrketings = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => p.BenhNhan == null && p.TamNgung != true && p.GoiSoSinh == yeuCauGoiDichVu.GoiSoSinh && p.TuNgay.Date <= dateTimeNow
                && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                && p.TamNgung != true)
                    .Select(s => new LookupItemTemplateVo
                    {
                        KeyId = s.Id,
                        DisplayName = s.Ten + " - " + s.TenGoiDichVu,

                        Ma = s.Ma,
                        Ten = s.Ten
                    });

            getMarrketings = getMarrketings.ApplyLike(queryInfo.Query, g => g.DisplayName);
            return getMarrketings.ToList();
        }

        public List<DichVuTrongGoiMarketingModel> GetThongTinGoiDichVuMoiMuonChuyen(long chonGoiMarketing)
        {
            var dateTimeNow = DateTime.Now.Date;

            var chuongTrinhGoiDichVuId = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == chonGoiMarketing && p.TuNgay.Date <= dateTimeNow
            && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
            && p.TamNgung != true).Select(x => x.Id).FirstOrDefault();

            var dichVuTrongGoiMarketingGridVos = _chuongTrinhGoiDichVuDichVuKhamBenhRepository.TableNoTracking
                       .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                       .Select(kb => new DichVuTrongGoiMarketingModel
                       {
                           DichVuBenhVienId = kb.DichVuKhamBenhBenhVienId,
                           Ma = kb.DichVuKhamBenhBenhVien.Ma,
                           Ten = kb.DichVuKhamBenhBenhVien.Ten,
                           Nhom = EnumDichVuTongHop.KhamBenh,
                           SoLuong = kb.SoLan,
                           LoaiGia = kb.NhomGiaDichVuKhamBenhBenhVien.Ten,
                           NhomGiaDichVuId = kb.NhomGiaDichVuKhamBenhBenhVien.Id,
                           DonGiaBenhVien = kb.DonGia,
                           DonGiaTruocChietKhau = kb.DonGiaTruocChietKhau,
                           DonGiaSauChietKhau = kb.DonGiaSauChietKhau,
                       }).Union(
                        _chuongTrinhGoiDichVuDichVuKyThuatRepository.TableNoTracking
                       .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                       .Select(kt => new DichVuTrongGoiMarketingModel
                       {
                           DichVuBenhVienId = kt.DichVuKyThuatBenhVienId,
                           Ma = kt.DichVuKyThuatBenhVien.Ma,
                           Ten = kt.DichVuKyThuatBenhVien.Ten,
                           Nhom = EnumDichVuTongHop.KyThuat,
                           SoLuong = kt.SoLan,
                           DonGiaBenhVien = kt.DonGia,
                           LoaiGia = kt.NhomGiaDichVuKyThuatBenhVien.Ten,
                           NhomGiaDichVuId = kt.NhomGiaDichVuKyThuatBenhVien.Id,
                           DonGiaTruocChietKhau = kt.DonGiaTruocChietKhau,
                           DonGiaSauChietKhau = kt.DonGiaSauChietKhau,
                       }
                       )).Union(
                               _chuongTrinhGoiDichVuDichVuGiuongRepository.TableNoTracking
                               .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                               .Select(g => new DichVuTrongGoiMarketingModel
                               {
                                   DichVuBenhVienId = g.DichVuGiuongBenhVienId,
                                   Ma = g.DichVuGiuongBenhVien.Ma,
                                   Ten = g.DichVuGiuongBenhVien.Ten,
                                   Nhom = EnumDichVuTongHop.GiuongBenh,
                                   SoLuong = g.SoLan,
                                   DonGiaBenhVien = g.DonGia,
                                   LoaiGia = g.NhomGiaDichVuGiuongBenhVien.Ten,
                                   NhomGiaDichVuId = g.NhomGiaDichVuGiuongBenhVien.Id,
                                   DonGiaTruocChietKhau = g.DonGiaTruocChietKhau,
                                   DonGiaSauChietKhau = g.DonGiaSauChietKhau,
                               }
                           )).ToList();


            return dichVuTrongGoiMarketingGridVos;
        }

        public List<DichVuTrongGoiMarketingModel> GetThongTinGoiDichVuCuaBenhNhan(long yeuCauGoiDichVuId)
        {
            var thongTinGoiDichVu = GetThongTinGoiDichVu(yeuCauGoiDichVuId).Result;

            var thongTinGoiCuaBenhNhans = new List<DichVuTrongGoiMarketingModel>();

            foreach (var dichVu in thongTinGoiDichVu.DanhSachDichVuTrongGois.Where(o=> o.SoLuongChuaDung > 0))
            {
                thongTinGoiCuaBenhNhans.Add(new DichVuTrongGoiMarketingModel
                {
                    DichVuBenhVienId = dichVu.DichVuBenhVienId,
                    Ma = dichVu.MaDichVu,
                    Nhom = dichVu.LoaiNhom,
                    Ten = dichVu.TenDichVu,
                    LoaiGia = dichVu.LoaiGia,
                    NhomGiaDichVuId = dichVu.NhomGiaDichVuId,
                    SoLuong = (int) Math.Round(dichVu.SoLuongChuaDung),
                    DonGiaBenhVien = dichVu.DGBV,
                    DonGiaTruocChietKhau = dichVu.DGTruocCK,
                    DonGiaSauChietKhau = dichVu.DGSauCK,
                });
            }

            return thongTinGoiCuaBenhNhans;
        }

        #endregion

        public async Task<GridDataSource> GetDanhSachChuaQuyetToanAsync(ThuNganMarketingQueryInfo queryInfo, bool isAllData)
        {
            var cauHinhMarketing = _cauHinhService.LoadSetting<CauHinhMarketing>();
            ReplaceDisplayValueSortExpression(queryInfo);

            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> { new Sort { Field = "ChuaThanhToan", Dir = "desc" },
                                                  new Sort { Field = "Id", Dir = "asc" } };
            }

            var query = BaseRepository.TableNoTracking
                .Where(o => o.YeuCauGoiDichVus.Any(g => g.DaQuyetToan != true && g.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy))
                .Select(s => new DanhSachChuaQuyetToanMarketingGridVo
                {
                    Id = s.Id,
                    MaBN = s.MaBN,
                    HoTen = s.HoTen,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                    SoDienThoai = s.SoDienThoai,
                    GioiTinh = s.GioiTinh,
                    NamSinh = s.NamSinh,
                    CMND = s.SoChungMinhThu,
                    DiaChi = s.DiaChiDayDu,
                    //ChuaThanhToan = !s.YeuCauGoiDichVus.Any(g => g.TaiKhoanBenhNhanChis.Any(c => c.DaHuy != true))
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.MaBN, g => g.HoTen);
            }

            var queryResult = isAllData ? await query.OrderBy(queryInfo.SortString).ToArrayAsync()
                : await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            if (queryResult.Length > 0)
            {
                var benhNhanIds = queryResult.Select(o => o.Id).ToList();

                var yeuCauGoiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Where(o => benhNhanIds.Contains(o.BenhNhanId) && o.DaQuyetToan != true && o.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                    .Select(o => new
                    {
                        Id = o.Id,
                        BenhNhanId = o.BenhNhanId,
                        TaiKhoanBenhNhanChis = o.TaiKhoanBenhNhanChis.Select(chi => new { chi.TienChiPhi, chi.DaHuy }).ToList()
                    }).ToList();

                //var danhSachBenhNhan = BaseRepository.TableNoTracking.Where(bn => benhNhanIds.Contains(bn.Id)).Select(bn =>
                //    new
                //    {
                //        BenhNhanId = bn.Id,
                //        YeuCauGoiDichVuIds = bn.YeuCauGoiDichVus.Where(g => g.DaQuyetToan != true && g.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy).Select(o => o.Id).ToList(),
                //        YeuCauDichVuKyThuats = bn.YeuCauGoiDichVus.Where(g => g.DaQuyetToan != true && g.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy).SelectMany(o => o.YeuCauDichVuKyThuats).ToList(),
                //        YeuCauKhamBenhs = bn.YeuCauGoiDichVus.Where(g => g.DaQuyetToan != true && g.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy).SelectMany(o => o.YeuCauKhamBenhs).ToList(),
                //        YeuCauDichVuGiuongBenhVienChiPhiBenhViens = bn.YeuCauGoiDichVus.Where(g => g.DaQuyetToan != true && g.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy).SelectMany(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ToList(),
                //        TaiKhoanBenhNhanChis = bn.YeuCauGoiDichVus.Where(g => g.DaQuyetToan != true && g.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy).SelectMany(o => o.TaiKhoanBenhNhanChis).ToList(),
                //    }
                //).ToList();

                foreach (var danhSachChuaQuyetToanMarketingGridVo in queryResult)
                {
                    danhSachChuaQuyetToanMarketingGridVo.ChuaThanhToan = !yeuCauGoiDichVus.Any(o => o.BenhNhanId == danhSachChuaQuyetToanMarketingGridVo.Id && o.TaiKhoanBenhNhanChis.Any(c => c.DaHuy != true));
                    //var benhNhan = danhSachBenhNhan.FirstOrDefault(o => o.BenhNhanId == danhSachChuaQuyetToanMarketingGridVo.Id);
                    //if (benhNhan != null)
                    //{
                    //    foreach (var yeuCauGoiDichVuId in benhNhan.YeuCauGoiDichVuIds)
                    //    {
                    //        if ((benhNhan.TaiKhoanBenhNhanChis.Where(o => o.DaHuy != true && o.YeuCauGoiDichVuId == yeuCauGoiDichVuId).Select(o => o.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum()
                    //             - benhNhan.YeuCauKhamBenhs.Where(o => o.YeuCauGoiDichVuId == yeuCauGoiDichVuId && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(o => o.DonGiaSauChietKhau.GetValueOrDefault()).DefaultIfEmpty().Sum()
                    //             - benhNhan.YeuCauDichVuKyThuats.Where(o => o.YeuCauGoiDichVuId == yeuCauGoiDichVuId && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(o => o.DonGiaSauChietKhau.GetValueOrDefault() * o.SoLan).DefaultIfEmpty().Sum()
                    //             - benhNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Where(o => o.YeuCauGoiDichVuId == yeuCauGoiDichVuId).Select(o => o.DonGiaSauChietKhau.GetValueOrDefault() * (decimal)o.SoLuong).DefaultIfEmpty().Sum()) < cauHinhMarketing.SoTienCanhBaoThuThemGoiMarketing)
                    //        {
                    //            danhSachChuaQuyetToanMarketingGridVo.Highlight = true;
                    //            break;
                    //        }
                    //    }

                    //}
                }
            }

            return new GridDataSource { Data = queryResult, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageDanhSachChuaQuyetToanAsync(ThuNganMarketingQueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Where(o => o.YeuCauGoiDichVus.Any(g => g.DaQuyetToan != true && g.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy))
                .Select(s => new DanhSachChuaQuyetToanMarketingGridVo
                {
                    Id = s.Id,
                    MaBN = s.MaBN,
                    HoTen = s.HoTen,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                    SoDienThoai = s.SoDienThoai,
                    GioiTinh = s.GioiTinh,
                    NamSinh = s.NamSinh,
                    CMND = s.SoChungMinhThu,
                    DiaChi = s.DiaChiDayDu
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.MaBN, g => g.HoTen);
            }

            var totalRowCount = await query.CountAsync();
            return new GridDataSource { TotalRowCount = totalRowCount };
        }

        public async Task<GridDataSource> GetDanhSachDaQuyetToanAsync(ThuNganMarketingQueryInfo queryInfo, bool isAllData)
        {
            var cauHinhMarketing = _cauHinhService.LoadSetting<CauHinhMarketing>();
            ReplaceDisplayValueSortExpression(queryInfo);

            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> { new Sort { Field = "DaThanhToan", Dir = "desc" },
                                                  new Sort { Field = "Id", Dir = "asc" } };
            }

            var query = BaseRepository.TableNoTracking
                .Where(o => o.YeuCauGoiDichVus.Any(g => g.DaQuyetToan == true))
                .Select(s => new DanhSachChuaQuyetToanMarketingGridVo
                {
                    Id = s.Id,
                    MaBN = s.MaBN,
                    HoTen = s.HoTen,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                    SoDienThoai = s.SoDienThoai,
                    GioiTinh = s.GioiTinh,
                    NamSinh = s.NamSinh,
                    CMND = s.SoChungMinhThu,
                    DiaChi = s.DiaChiDayDu,
                    //ChuaThanhToan = !s.YeuCauGoiDichVus.Any(g => g.TaiKhoanBenhNhanChis.Any(c => c.DaHuy != true))
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.MaBN, g => g.HoTen);
            }

            var queryResult = isAllData ? await query.OrderBy(queryInfo.SortString).ToArrayAsync()
                : await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            return new GridDataSource { Data = queryResult, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageDanhSachDaQuyetToanAsync(ThuNganMarketingQueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Where(o => o.YeuCauGoiDichVus.Any(g => g.DaQuyetToan == true))
                .Select(s => new DanhSachChuaQuyetToanMarketingGridVo
                {
                    Id = s.Id,
                    MaBN = s.MaBN,
                    HoTen = s.HoTen,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                    SoDienThoai = s.SoDienThoai,
                    GioiTinh = s.GioiTinh,
                    NamSinh = s.NamSinh,
                    CMND = s.SoChungMinhThu,
                    DiaChi = s.DiaChiDayDu
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.MaBN, g => g.HoTen);
            }

            var totalRowCount = await query.CountAsync();
            return new GridDataSource { TotalRowCount = totalRowCount };
        }

        public async Task<GridDataSource> GetGoiChuaQuyetToanTheoBenhNhanAsync(long benhNhanId)
        {
            var yeuCauGoiDichVus = await _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(o => o.BenhNhanId == benhNhanId && o.DaQuyetToan != true && o.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.TaiKhoanBenhNhanThu)
                .Include(o => o.YeuCauKhamBenhs)
                .Include(o => o.YeuCauDichVuKyThuats)
                .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ToListAsync();

            var cauHinhMarketing = _cauHinhService.LoadSetting<CauHinhMarketing>();

            var queryResult = yeuCauGoiDichVus
                .Select(o => new GoiChuaQuyetToanMarketingGridVo
                {
                    Id = o.Id,
                    MaGoi = o.MaChuongTrinh,
                    TenGoi = o.TenChuongTrinh,
                    ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    GiaGoi = o.GiaSauChietKhau,
                    DaThu = o.TaiKhoanBenhNhanChis.Where(c => c.DaHuy != true && c.TaiKhoanBenhNhanThu != null && c.TaiKhoanBenhNhanThu.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng).Select(c => c.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                    TongDVDaDung = o.YeuCauKhamBenhs.Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(y => y.DonGiaSauChietKhau.GetValueOrDefault()).DefaultIfEmpty().Sum()
                                   + o.YeuCauDichVuKyThuats.Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(y => y.DonGiaSauChietKhau.GetValueOrDefault() * y.SoLan).DefaultIfEmpty().Sum()
                                   + o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Select(y => y.DonGiaSauChietKhau.GetValueOrDefault() * (decimal)y.SoLuong).DefaultIfEmpty().Sum(),
                    NgungSuDung = o.NgungSuDung
                }).ToArray();

            foreach (var goiChuaQuyetToanMarketingGridVo in queryResult)
            {
                if (goiChuaQuyetToanMarketingGridVo.DaThu - goiChuaQuyetToanMarketingGridVo.TongDVDaDung < cauHinhMarketing.SoTienCanhBaoThuThemGoiMarketing)
                {
                    goiChuaQuyetToanMarketingGridVo.Highlight = true;
                }
            }

            return new GridDataSource { Data = queryResult };
        }

        public async Task<GridDataSource> GetGoiDaQuyetToanTheoBenhNhanAsync(long benhNhanId)
        {
            var yeuCauGoiDichVus = await _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(o => o.BenhNhanId == benhNhanId && o.DaQuyetToan == true)
                .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.TaiKhoanBenhNhanThu)
                .Include(o => o.YeuCauKhamBenhs)
                .Include(o => o.YeuCauDichVuKyThuats)
                .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ToListAsync();

            var queryResult = yeuCauGoiDichVus
                .Select(o => new GoiDaQuyetToanMarketingGridVo
                {
                    Id = o.Id,
                    MaGoi = o.MaChuongTrinh,
                    TenGoi = o.TenChuongTrinh,
                    ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    GiaGoi = o.GiaSauChietKhau,
                    DaThu = o.TaiKhoanBenhNhanChis.Where(c => c.DaHuy != true && c.TaiKhoanBenhNhanThu != null && c.TaiKhoanBenhNhanThu.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng).Select(c => c.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                    TongDVDaDung = o.YeuCauKhamBenhs.Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(y => y.DonGiaSauChietKhau.GetValueOrDefault()).DefaultIfEmpty().Sum()
                                   + o.YeuCauDichVuKyThuats.Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(y => y.DonGiaSauChietKhau.GetValueOrDefault() * y.SoLan).DefaultIfEmpty().Sum()
                                   + o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Select(y => y.DonGiaSauChietKhau.GetValueOrDefault() * (decimal)y.SoLuong).DefaultIfEmpty().Sum(),
                    TraLaiBN = o.SoTienTraLai.GetValueOrDefault(),
                    TrangThai = o.TrangThai,
                }).ToArray();

            return new GridDataSource { Data = queryResult };
        }

        public async Task<(long, string)> ThuTienGoiDichVu(ThongTinThuTienGoi thongTinThuTienGoi)
        {
            var benhNhan = await BaseRepository.GetByIdAsync(thongTinThuTienGoi.BenhNhanId,
                x => x.Include(o => o.YeuCauTiepNhans)
                .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.YeuCauKhamBenhs)
                .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.YeuCauDichVuKyThuats)
                .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .Include(o => o.TaiKhoanBenhNhan));

            var tongThu = thongTinThuTienGoi.TienMat + thongTinThuTienGoi.ChuyenKhoan + thongTinThuTienGoi.POS;

            if (!thongTinThuTienGoi.GoiChuaQuyetToanMarketing.Sum(o => o.SoTienThuLanNay).Equals(tongThu))
                return (0, "Số tiền thanh toán không hợp lệ");
            foreach (var goiChuaQuyetToanMarketingGridVo in thongTinThuTienGoi.GoiChuaQuyetToanMarketing)
            {
                var yeuCauGoiDV = benhNhan.YeuCauGoiDichVus.FirstOrDefault(o =>
                    o.Id == goiChuaQuyetToanMarketingGridVo.Id && o.DaQuyetToan != true &&
                    o.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy);
                if (yeuCauGoiDV == null || yeuCauGoiDV.TaiKhoanBenhNhanChis.Where(o => o.DaHuy != true).Select(o => o.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum() + goiChuaQuyetToanMarketingGridVo.SoTienThuLanNay > yeuCauGoiDV.GiaSauChietKhau)
                {
                    return (0, "Số tiền thanh toán không hợp lệ");
                }
            }

            List<ThongTinThanhToanGoiDichVuVo> thongTinThanhToanGoiDichVuVos = benhNhan.YeuCauGoiDichVus
                .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy && o.DaQuyetToan != true)
                .Select(o => new ThongTinThanhToanGoiDichVuVo
                {
                    YeuCauGoiDichVuId = o.Id,
                    SoTienBenhNhanDaChi = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    DanhSachDichVuDaBaoLanhSuDung = o.YeuCauKhamBenhs
                        .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                                    y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                        .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)1))
                        .Concat(o.YeuCauDichVuKyThuats
                            .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                            .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLan)))
                        .Concat(o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                            .Where(y => y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                            .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLuong)))
                        .ToList()
                }).ToList();

            var tk = benhNhan.TaiKhoanBenhNhan ?? new TaiKhoanBenhNhan
            {
                BenhNhan = benhNhan,
                SoDuTaiKhoan = 0
            };

            var userId = _userAgentHelper.GetCurrentUserId();
            var userThuNgan = _userRepository.GetById(_userAgentHelper.GetCurrentUserId());
            var maTaiKhoan = userId.ToString();
            if (userThuNgan.Email != null && userThuNgan.Email.IndexOf("@") > 0)
            {
                maTaiKhoan = userThuNgan.Email.Substring(0, userThuNgan.Email.IndexOf("@") > 10 ? 10 : userThuNgan.Email.IndexOf("@"));
            }

            var thuPhi = new TaiKhoanBenhNhanThu
            {
                TaiKhoanBenhNhan = tk,
                LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.ThuTamUng,
                LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                ThuTienGoiDichVu = true,
                TienMat = thongTinThuTienGoi.TienMat,
                ChuyenKhoan = thongTinThuTienGoi.ChuyenKhoan,
                POS = thongTinThuTienGoi.POS,
                NoiDungThu = thongTinThuTienGoi.NoiDungThu,
                NgayThu = thongTinThuTienGoi.NgayThu,
                SoPhieuHienThi = ResourceHelper.CreateSoTamUng(),
                NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
            };
            var ycTiepNhan = benhNhan.YeuCauTiepNhans.OrderByDescending(o => o.Id).FirstOrDefault(o =>
                o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien);
            if (ycTiepNhan == null)
            {
                ycTiepNhan = new YeuCauTiepNhan
                {
                    LoaiYeuCauTiepNhan = Enums.EnumLoaiYeuCauTiepNhan.DangKyGoiMarketing,
                    TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien,
                    MaYeuCauTiepNhan = ResourceHelper.CreateMaYeuCauTiepNhan(),
                    BenhNhanId = benhNhan.Id,
                    ThoiDiemTiepNhan = DateTime.Now,
                    ThoiDiemCapNhatTrangThai = DateTime.Now,
                    HoTen = benhNhan.HoTen,
                    NgaySinh = benhNhan.NgaySinh,
                    ThangSinh = benhNhan.ThangSinh,
                    NamSinh = benhNhan.NamSinh,
                    SoChungMinhThu = benhNhan.SoChungMinhThu,
                    GioiTinh = benhNhan.GioiTinh,
                    NhomMau = benhNhan.NhomMau,
                    NgheNghiepId = benhNhan.NgheNghiepId,
                    NoiLamViec = benhNhan.NoiLamViec,
                    QuocTichId = benhNhan.QuocTichId,
                    DanTocId = benhNhan.DanTocId,
                    DiaChi = benhNhan.DiaChi,
                    PhuongXaId = benhNhan.PhuongXaId,
                    QuanHuyenId = benhNhan.QuanHuyenId,
                    TinhThanhId = benhNhan.TinhThanhId,
                    SoDienThoai = benhNhan.SoDienThoai,
                    Email = benhNhan.Email,
                };
                benhNhan.YeuCauTiepNhans.Add(ycTiepNhan);
            }

            foreach (var goiChuaQuyetToanMarketingGridVo in thongTinThuTienGoi.GoiChuaQuyetToanMarketing)
            {
                var yc = benhNhan.YeuCauGoiDichVus.First(o => o.Id == goiChuaQuyetToanMarketingGridVo.Id);
                yc.SoTienBenhNhanDaChi = yc.SoTienBenhNhanDaChi.GetValueOrDefault() + goiChuaQuyetToanMarketingGridVo.SoTienThuLanNay;
                yc.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;
                if (yc.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien)
                {
                    yc.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien;
                }
                thuPhi.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhan = tk,
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                    TienChiPhi = goiChuaQuyetToanMarketingGridVo.SoTienThuLanNay,
                    NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                    NgayChi = DateTime.Now,
                    SoPhieuHienThi = thuPhi.SoPhieuHienThi,
                    YeuCauGoiDichVu = yc,
                    Gia = yc.GiaSauChietKhau,
                    SoLuong = 1,
                    SoTienMienGiam = yc.SoTienMienGiam,
                    SoTienBaoHiemTuNhanChiTra = yc.SoTienBaoHiemTuNhanChiTra,
                    YeuCauTiepNhan = ycTiepNhan,
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                });
                var thongTinThanhToanGoiDichVuVo = thongTinThanhToanGoiDichVuVos.FirstOrDefault(o => o.YeuCauGoiDichVuId == goiChuaQuyetToanMarketingGridVo.Id);
                if (thongTinThanhToanGoiDichVuVo != null)
                {
                    thongTinThanhToanGoiDichVuVo.SoTienBenhNhanDaChi += goiChuaQuyetToanMarketingGridVo.SoTienThuLanNay;

                    foreach (var yeuCauKhamBenh in yc.YeuCauKhamBenhs.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan))
                    {
                        if (thongTinThanhToanGoiDichVuVo.SoTienConBaoLanh >= yeuCauKhamBenh.DonGiaSauChietKhau.GetValueOrDefault())
                        {
                            yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                            if (yeuCauKhamBenh.NoiDangKyId != null)
                            {
                                yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauKhamBenh.YeuCauTiepNhanId, yeuCauKhamBenh.NoiDangKyId.Value));
                            }
                            thongTinThanhToanGoiDichVuVo.DanhSachDichVuDaBaoLanhSuDung.Add((yeuCauKhamBenh.DonGiaSauChietKhau.GetValueOrDefault(), 1));
                        }
                    }
                    foreach (var yeuCauDichVuKyThuat in yc.YeuCauDichVuKyThuats.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan))
                    {
                        if (thongTinThanhToanGoiDichVuVo.SoTienConBaoLanh >= yeuCauDichVuKyThuat.DonGiaSauChietKhau.GetValueOrDefault())
                        {
                            yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                            if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                            {
                                yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauDichVuKyThuat.YeuCauTiepNhanId, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                            }
                            thongTinThanhToanGoiDichVuVo.DanhSachDichVuDaBaoLanhSuDung.Add((yeuCauDichVuKyThuat.DonGiaSauChietKhau.GetValueOrDefault(), yeuCauDichVuKyThuat.SoLan));
                        }
                    }
                }
            }

            ycTiepNhan.TaiKhoanBenhNhanThus.Add(thuPhi);

            await BaseRepository.UpdateAsync(benhNhan);
            return (thuPhi.Id, string.Empty);
        }

        private List<PhongBenhVienHangDoi> _phongBenhVienHangDoi;
        private List<PhongBenhVienHangDoi> GetPhongBenhVienHangDois()
        {
            return _phongBenhVienHangDoi ?? (_phongBenhVienHangDoi = BaseRepository.Context.Set<PhongBenhVienHangDoi>().AsNoTracking().ToList());
        }
        private PhongBenhVienHangDoi XepVaoHangDoi(long yeuCauTiepNhanId, long phongBenhVienId)
        {
            var lastStt = GetPhongBenhVienHangDois().Where(o => o.PhongBenhVienId == phongBenhVienId).OrderBy(o => o.SoThuTu).LastOrDefault();
            var stt = lastStt?.SoThuTu + 1 ?? 1;
            var phongBenhVienHangDoi = new PhongBenhVienHangDoi
            {
                PhongBenhVienId = phongBenhVienId,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham,
                SoThuTu = stt
            };
            _phongBenhVienHangDoi.Add(phongBenhVienHangDoi);
            try
            {
                LogManager.GetCurrentClassLogger().Info($"ThuNganMarketing XepVaoHangDoi phongBenhVienId{phongBenhVienId}, yeuCauTiepNhanId{yeuCauTiepNhanId}");
            }
            catch (Exception e)
            {

            }
            return phongBenhVienHangDoi;
        }

        public async Task<GridDataSource> GetDanhSachPhieuThuMarketingAsync(ThongTinPhieuMarketingQueryInfo queryInfo)
        {
            var fromDate = queryInfo.ThoiDiemTuFormat;
            var toDate = queryInfo.ThoiDiemDenFormat;

            var query = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(o => o.TaiKhoanBenhNhanId == queryInfo.BenhNhanId
                    && ((o.ThuTienGoiDichVu == true && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng) || ((o.ThuTienGoiDichVu == true && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && o.TaiKhoanBenhNhanChis.Any(c => c.YeuCauGoiDichVuId != null && c.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi))))
                    && (queryInfo.GoiDichDichVuId == null || o.TaiKhoanBenhNhanChis.Any(c => c.YeuCauGoiDichVuId == queryInfo.GoiDichDichVuId))
                    && (fromDate == null || o.NgayThu >= fromDate)
                    && (toDate == null || o.NgayThu <= toDate))
                .Select(s => new ThongTinPhieuThuGoiDichVuGridVo
                {
                    Id = s.Id,
                    SoPhieuThu = s.SoPhieuHienThi,
                    LoaiPhieuThuChiThuNgan = s.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? LoaiPhieuThuChiThuNgan.ThuTheoChiPhi : LoaiPhieuThuChiThuNgan.ThuTamUng,
                    ThuCuaGoi = s.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? s.TaiKhoanBenhNhanChis.Where(o => o.YeuCauGoiDichVuId != null && o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(o => o.YeuCauGoiDichVu.TenChuongTrinh) : s.TaiKhoanBenhNhanChis.Where(o => o.YeuCauGoiDichVuId != null).Select(o => o.YeuCauGoiDichVu.TenChuongTrinh),
                    NguoiThu = s.NhanVienThucHien.User.HoTen,
                    NgayThu = s.NgayThu,
                    SoTienThu = s.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? s.TamUng.GetValueOrDefault() : (s.TienMat.GetValueOrDefault() + s.ChuyenKhoan.GetValueOrDefault() + s.POS.GetValueOrDefault()),
                    DaHuy = s.DaHuy.GetValueOrDefault(),
                    LyDo = s.LyDoHuy,
                    ThuHoiPhieu = s.DaThuHoi,
                    NguoiThuHoiId = s.NhanVienThuHoiId,
                    TenNguoiThuHoi = s.NhanVienThuHoi != null ? s.NhanVienThuHoi.User.HoTen : "",
                    ThoiGianThuHoi = s.NgayThuHoi
                });
            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                query = query.ApplyLike(queryInfo.SearchString, g => g.SoPhieuThu);
            }

            var queryPhieuChi = _taiKhoanBenhNhanChiRepository.TableNoTracking
                .Where(o => o.TaiKhoanBenhNhanId == queryInfo.BenhNhanId
                            && o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng
                            && (queryInfo.GoiDichDichVuId == null || o.YeuCauGoiDichVuId == queryInfo.GoiDichDichVuId)
                            && (fromDate == null || o.NgayChi >= fromDate)
                            && (toDate == null || o.NgayChi <= toDate))
                .Select(s => new ThongTinPhieuThuGoiDichVuGridVo
                {
                    Id = s.Id,
                    SoPhieuThu = s.SoPhieuHienThi,
                    LoaiPhieuThuChiThuNgan = LoaiPhieuThuChiThuNgan.HoanUng,
                    ThuCuaGoi = new[] { s.YeuCauGoiDichVu.TenChuongTrinh },
                    NguoiThu = s.NhanVienThucHien.User.HoTen,
                    NgayThu = s.NgayChi,
                    SoTienThu = s.TienMat.GetValueOrDefault(),
                    DaHuy = s.DaHuy.GetValueOrDefault(),
                    LyDo = s.LyDoHuy,
                    ThuHoiPhieu = s.DaThuHoi,
                    NguoiThuHoiId = s.NhanVienThuHoiId,
                    TenNguoiThuHoi = s.NhanVienThuHoi != null ? s.NhanVienThuHoi.User.HoTen : "",
                    ThoiGianThuHoi = s.NgayThuHoi
                });
            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                queryPhieuChi = queryPhieuChi.ApplyLike(queryInfo.SearchString, g => g.SoPhieuThu);
            }

            var queryResult = (await query.ToArrayAsync()).Concat(await queryPhieuChi.ToArrayAsync());

            return new GridDataSource { Data = queryResult.OrderByDescending(o => o.NgayThu).ToArray() };
        }

        public async Task<ThongTinPhieuThu> GetThongTinPhieuThu(long phieuThuId, LoaiPhieuThuChiThuNgan loaiPhieuThuChiThuNgan)
        {
            ThongTinPhieuThu thongTinPhieuThu = null;
            if (loaiPhieuThuChiThuNgan == LoaiPhieuThuChiThuNgan.ThuTamUng)
            {
                var taiKhoanBenhNhanThu = await _taiKhoanBenhNhanThuRepository.TableNoTracking
                                                                        .Include(o => o.NhanVienHuy).ThenInclude(o => o.User)
                                                                        .Include(o => o.NhanVienThuHoi).ThenInclude(o => o.User)
                                                                        .FirstOrDefaultAsync(o => o.Id == phieuThuId);
                if (taiKhoanBenhNhanThu != null)
                {
                    var hinhThucThanhToans = new List<string>();
                    if (taiKhoanBenhNhanThu.TienMat.GetValueOrDefault(0) != 0)
                    {
                        hinhThucThanhToans.Add("Tiền mặt");
                    }
                    if (taiKhoanBenhNhanThu.ChuyenKhoan.GetValueOrDefault(0) != 0)
                    {
                        hinhThucThanhToans.Add("Chuyển khoản");
                    }
                    if (taiKhoanBenhNhanThu.POS.GetValueOrDefault(0) != 0)
                    {
                        hinhThucThanhToans.Add("POS");
                    }
                    thongTinPhieuThu = new ThongTinPhieuThu
                    {
                        Id = taiKhoanBenhNhanThu.Id,
                        SoPhieu = taiKhoanBenhNhanThu.SoPhieuHienThi,
                        LoaiPhieuThuChiThuNgan = LoaiPhieuThuChiThuNgan.ThuTamUng,
                        DaHuy = taiKhoanBenhNhanThu.DaHuy,
                        TienMat = taiKhoanBenhNhanThu.TienMat,
                        ChuyenKhoan = taiKhoanBenhNhanThu.ChuyenKhoan,
                        Pos = taiKhoanBenhNhanThu.POS,
                        HinhThucThanhToan = string.Join(", ", hinhThucThanhToans),
                        NgayThu = taiKhoanBenhNhanThu.NgayThu,
                        NoiDungThu = taiKhoanBenhNhanThu.NoiDungThu,
                        DaThuHoi = taiKhoanBenhNhanThu.DaThuHoi.GetValueOrDefault(),
                        NgayThuHoi = taiKhoanBenhNhanThu.NgayThuHoi,
                        NguoiThuHoiId = taiKhoanBenhNhanThu.NhanVienThuHoiId,
                        NguoiThuHoi = taiKhoanBenhNhanThu.NhanVienThuHoi?.User.HoTen,
                        NhanVienHuyPhieu = taiKhoanBenhNhanThu.NhanVienHuy?.User.HoTen,
                        NgayHuy = taiKhoanBenhNhanThu.NgayHuy
                    };
                }
            }
            else if (loaiPhieuThuChiThuNgan == LoaiPhieuThuChiThuNgan.HoanUng)
            {
                var taiKhoanBenhNhanChi = _taiKhoanBenhNhanChiRepository.TableNoTracking.Include(o => o.NhanVienHuy).ThenInclude(o => o.User).Include(o => o.NhanVienThuHoi).ThenInclude(o => o.User).FirstOrDefault(o => o.Id == phieuThuId);
                if (taiKhoanBenhNhanChi != null)
                {
                    thongTinPhieuThu = new ThongTinPhieuThu
                    {
                        Id = taiKhoanBenhNhanChi.Id,
                        SoPhieu = taiKhoanBenhNhanChi.SoPhieuHienThi,
                        LoaiPhieuThuChiThuNgan = LoaiPhieuThuChiThuNgan.HoanUng,
                        DaHuy = taiKhoanBenhNhanChi.DaHuy,
                        TienMat = taiKhoanBenhNhanChi.TienMat,
                        HinhThucThanhToan = "Tiền mặt",
                        NgayThu = taiKhoanBenhNhanChi.NgayChi,
                        NoiDungThu = taiKhoanBenhNhanChi.NoiDungChi,
                        DaThuHoi = taiKhoanBenhNhanChi.DaThuHoi.GetValueOrDefault(),
                        NgayThuHoi = taiKhoanBenhNhanChi.NgayThuHoi,
                        NguoiThuHoiId = taiKhoanBenhNhanChi.NhanVienThuHoiId,
                        NguoiThuHoi = taiKhoanBenhNhanChi.NhanVienThuHoi?.User.HoTen,
                        NhanVienHuyPhieu = taiKhoanBenhNhanChi.NhanVienHuy?.User.HoTen,
                        NgayHuy = taiKhoanBenhNhanChi.NgayHuy
                    };
                }
            }
            else if (loaiPhieuThuChiThuNgan == LoaiPhieuThuChiThuNgan.ThuTheoChiPhi)
            {
                var taiKhoanBenhNhanThu = await _taiKhoanBenhNhanThuRepository.TableNoTracking
                                                                        .Include(o => o.NhanVienHuy).ThenInclude(o => o.User)
                                                                        .Include(o => o.NhanVienThuHoi).ThenInclude(o => o.User)
                                                                        .FirstOrDefaultAsync(o => o.Id == phieuThuId);
                if (taiKhoanBenhNhanThu != null)
                {
                    thongTinPhieuThu = new ThongTinPhieuThu
                    {
                        Id = taiKhoanBenhNhanThu.Id,
                        SoPhieu = taiKhoanBenhNhanThu.SoPhieuHienThi,
                        LoaiPhieuThuChiThuNgan = LoaiPhieuThuChiThuNgan.ThuTheoChiPhi,
                        DaHuy = taiKhoanBenhNhanThu.DaHuy,
                        TongChiPhi = taiKhoanBenhNhanThu.TamUng.GetValueOrDefault(),
                        BHYTThanhToan = 0,
                        MienGiam = 0,
                        BenhNhanThanhToan = 0,
                        TienMat = taiKhoanBenhNhanThu.TamUng.GetValueOrDefault(),
                        ChuyenKhoan = 0,
                        Pos = 0,
                        CongNo = 0,
                        TamUng = taiKhoanBenhNhanThu.TamUng.GetValueOrDefault(),
                        SoTienPhaiThuHoacChi = taiKhoanBenhNhanThu.TamUng.GetValueOrDefault(),
                        LaPhieuChi = false,
                        NgayThu = taiKhoanBenhNhanThu.NgayThu,
                        NoiDungThu = taiKhoanBenhNhanThu.NoiDungThu,
                        DaThuHoi = taiKhoanBenhNhanThu.DaThuHoi.GetValueOrDefault(),
                        NgayThuHoi = taiKhoanBenhNhanThu.NgayThuHoi,
                        NguoiThuHoiId = taiKhoanBenhNhanThu.NhanVienThuHoiId,
                        NguoiThuHoi = taiKhoanBenhNhanThu.NhanVienThuHoi?.User.HoTen,
                        NhanVienHuyPhieu = taiKhoanBenhNhanThu.NhanVienHuy?.User.HoTen,
                        NgayHuy = taiKhoanBenhNhanThu.NgayHuy,
                        ThuTienGoiDichVu = taiKhoanBenhNhanThu.ThuTienGoiDichVu
                    };
                }
            }
            return thongTinPhieuThu;
        }

        public async Task HuyPhieuThuGoiDichVu(ThongTinHuyPhieuVo thongTinHuyPhieuVo)
        {
            if (thongTinHuyPhieuVo.LoaiPhieuThuChiThuNgan == LoaiPhieuThuChiThuNgan.ThuTamUng)
            {
                var phieuThu = await _taiKhoanBenhNhanThuRepository.GetByIdAsync(thongTinHuyPhieuVo.SoPhieu,
                    o => o.Include(x => x.TaiKhoanBenhNhan)
                        .Include(x => x.TaiKhoanBenhNhanChis)
                        .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.YeuCauGoiDichVu));
                if (phieuThu.DaHuy == true)
                {
                    throw new Exception("Phiếu thu đã được hủy");
                }
                if (phieuThu.TaiKhoanBenhNhanChis.Any(o => o.YeuCauGoiDichVu.DaQuyetToan == true))
                {
                    throw new Exception("Gói dịch vụ đã quyết toán");
                }
                foreach (var chiTamUng in phieuThu.TaiKhoanBenhNhanChis)
                {
                    if (_taiKhoanBenhNhanChiRepository.TableNoTracking.Any(o =>
                        o.YeuCauGoiDichVuId == chiTamUng.YeuCauGoiDichVuId && o.Id > chiTamUng.Id &&
                        o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng))
                    {
                        throw new Exception("Đã phát sinh hoàn ứng từ phiếu thu này");
                    }
                }

                foreach (var phieuThuTaiKhoanBenhNhanChi in phieuThu.TaiKhoanBenhNhanChis.Where(o =>
                    o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi))
                {
                    phieuThuTaiKhoanBenhNhanChi.DaHuy = true;
                    phieuThuTaiKhoanBenhNhanChi.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
                    phieuThuTaiKhoanBenhNhanChi.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    phieuThuTaiKhoanBenhNhanChi.LyDoHuy = thongTinHuyPhieuVo.LyDo;
                    if (phieuThuTaiKhoanBenhNhanChi.YeuCauGoiDichVu != null)
                    {
                        phieuThuTaiKhoanBenhNhanChi.YeuCauGoiDichVu.SoTienBenhNhanDaChi -=
                            phieuThuTaiKhoanBenhNhanChi.TienChiPhi.GetValueOrDefault();
                        if (phieuThuTaiKhoanBenhNhanChi.YeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault()
                            .Equals(0))
                            phieuThuTaiKhoanBenhNhanChi.YeuCauGoiDichVu.TrangThaiThanhToan =
                                Enums.TrangThaiThanhToan.ChuaThanhToan;
                    }
                }

                phieuThu.DaHuy = true;
                phieuThu.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
                phieuThu.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
                phieuThu.LyDoHuy = thongTinHuyPhieuVo.LyDo;
                phieuThu.NgayHuy = DateTime.Now;
                phieuThu.DaThuHoi = thongTinHuyPhieuVo.ThoiGianThuHoi != null;
                phieuThu.NhanVienThuHoiId = thongTinHuyPhieuVo.NguoiThuHoiId;
                phieuThu.NgayThuHoi = thongTinHuyPhieuVo.ThoiGianThuHoi;
                await _taiKhoanBenhNhanThuRepository.UpdateAsync(phieuThu);

            }
            else
            {
                throw new Exception("Phiếu này không thể hủy");
            }
        }

        public async Task CapnhatNguoiThuHoiPhieuThu(ThongTinHuyPhieuVo thongTinHuyPhieuVo)
        {
            var phieuThu = await _taiKhoanBenhNhanThuRepository.GetByIdAsync(thongTinHuyPhieuVo.SoPhieu);
            phieuThu.LyDoHuy = thongTinHuyPhieuVo.LyDo;
            phieuThu.DaThuHoi = thongTinHuyPhieuVo.ThoiGianThuHoi != null;
            phieuThu.NhanVienThuHoiId = thongTinHuyPhieuVo.NguoiThuHoiId;
            phieuThu.NgayThuHoi = thongTinHuyPhieuVo.ThoiGianThuHoi;

            await _taiKhoanBenhNhanThuRepository.UpdateAsync(phieuThu);
        }

        public async Task<long?> QuyetToanGoiDichVu(ThongTinYeuCauGoiDichVu thongTinYeuCauGoiDichVu)
        {
            var yeuCauGoiDichVu = await _yeuCauGoiDichVuRepository.GetByIdAsync(
                thongTinYeuCauGoiDichVu.YeuCauGoiDichVuId,
                x => x.Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan)
                .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.TaiKhoanBenhNhanThu)
                .Include(o => o.YeuCauKhamBenhs)
                .Include(o => o.YeuCauDichVuKyThuats)
                .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens));
            if (yeuCauGoiDichVu.DaQuyetToan == true)
            {
                throw new Exception("Gói dịch vụ đã quyết toán");
            }

            var tongTienDaHoan = yeuCauGoiDichVu.TaiKhoanBenhNhanChis
                .Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng && o.DaHuy != true)
                .Select(o => o.TienMat.GetValueOrDefault()).DefaultIfEmpty().Sum();
            if (thongTinYeuCauGoiDichVu.SoTienTraLai > 0 && yeuCauGoiDichVu.TaiKhoanBenhNhanChis.Count(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && o.DaHuy != true) == 0)
            {
                throw new Exception("Bệnh nhân chưa đóng tiền.");
            }
            if (thongTinYeuCauGoiDichVu.SoTienTraLai > 0 && !yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault().SoTienTuongDuong(thongTinYeuCauGoiDichVu.SoTienTraLai + tongTienDaHoan) &&
                thongTinYeuCauGoiDichVu.SoTienTraLai + tongTienDaHoan > yeuCauGoiDichVu.SoTienBenhNhanDaChi)
            {
                throw new Exception("Số tiền trả lại không đúng.");
            }
            if (yeuCauGoiDichVu.YeuCauKhamBenhs.Any(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan)
               || yeuCauGoiDichVu.YeuCauDichVuKyThuats.Any(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan)
               || yeuCauGoiDichVu.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Any(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan))
            {
                throw new Exception("Dịch vụ trong gói chưa quyết toán");
            }

            long? yeuCauTiepNhanId = null;
            TaiKhoanBenhNhanChi phieuHoanUng = null;
            if (thongTinYeuCauGoiDichVu.SoTienTraLai > 0)
            {
                yeuCauTiepNhanId = YeuCauTiepNhanCuoiBenhNhan(yeuCauGoiDichVu.BenhNhanId)?.Id;
                phieuHoanUng = new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhan = yeuCauGoiDichVu.BenhNhan.TaiKhoanBenhNhan,
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.HoanUng,
                    YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                    TienMat = thongTinYeuCauGoiDichVu.SoTienTraLai,
                    NoiDungChi = Enums.LoaiChiTienBenhNhan.HoanUng.GetDescription(),
                    NgayChi = DateTime.Now,
                    YeuCauTiepNhanId = yeuCauTiepNhanId,
                    SoPhieuHienThi = ResourceHelper.CreateSoHoanUng(),
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                yeuCauGoiDichVu.TaiKhoanBenhNhanChis.Add(phieuHoanUng);
            }
            //tạo phiếu thu BVHD-3646
            if (!yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault()
                    .SoTienTuongDuong(thongTinYeuCauGoiDichVu.SoTienTraLai + tongTienDaHoan) &&
                yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault() >
                thongTinYeuCauGoiDichVu.SoTienTraLai + tongTienDaHoan)
            {
                var userId = _userAgentHelper.GetCurrentUserId();
                var userThuNgan = _userRepository.GetById(_userAgentHelper.GetCurrentUserId());
                var maTaiKhoan = userId.ToString();
                if (userThuNgan.Email != null && userThuNgan.Email.IndexOf("@") > 0)
                {
                    maTaiKhoan = userThuNgan.Email.Substring(0, userThuNgan.Email.IndexOf("@") > 10 ? 10 : userThuNgan.Email.IndexOf("@"));
                }
                yeuCauTiepNhanId = yeuCauTiepNhanId ?? YeuCauTiepNhanCuoiBenhNhan(yeuCauGoiDichVu.BenhNhanId).Id;
                var tienThuKhac = yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault() - (thongTinYeuCauGoiDichVu.SoTienTraLai + tongTienDaHoan);

                var phieuHoanUngKhac = new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhan = yeuCauGoiDichVu.BenhNhan.TaiKhoanBenhNhan,
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.HoanUng,
                    YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                    TienMat = tienThuKhac,
                    NoiDungChi = "Hoàn ứng khác",
                    NgayChi = DateTime.Now,
                    YeuCauTiepNhanId = yeuCauTiepNhanId,
                    SoPhieuHienThi = ResourceHelper.CreateSoHoanUng(),
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };

                var thuPhi = new TaiKhoanBenhNhanThu
                {
                    TaiKhoanBenhNhan = yeuCauGoiDichVu.BenhNhan.TaiKhoanBenhNhan,
                    LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi,
                    ThuTienGoiDichVu = true,
                    LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                    NoiDungThu = "Thu khác",
                    NgayThu = DateTime.Now,
                    TamUng = tienThuKhac,
                    SoPhieuHienThi = ResourceHelper.CreateSoPhieuThu(userId, maTaiKhoan),
                    YeuCauTiepNhanId = yeuCauTiepNhanId.Value,
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };

                var taiKhoanBenhNhanChiQuyetToan = new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhan = yeuCauGoiDichVu.BenhNhan.TaiKhoanBenhNhan,
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                    TienChiPhi = tienThuKhac,
                    NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                    NgayChi = DateTime.Now,
                    SoPhieuHienThi = thuPhi.SoPhieuHienThi,
                    YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                    Gia = tienThuKhac,
                    SoLuong = 1,
                    YeuCauTiepNhanId = yeuCauTiepNhanId,
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                List<NoiDungQuyetToanGoiMarketing> noiDungQuyetToanGoiMarketings = new List<NoiDungQuyetToanGoiMarketing>();
                if (yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault().SoTienTuongDuong(yeuCauGoiDichVu.GiaSauChietKhau) &&
                    thongTinYeuCauGoiDichVu.SoTienTraLai.AlmostEqual(0))
                {
                    //lay ds dv chua su dung
                    var thongTinGoiDichVu = await GetThongTinGoiDichVu(thongTinYeuCauGoiDichVu.YeuCauGoiDichVuId);
                    var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
                    foreach (var chiPhi in thongTinGoiDichVu.DanhSachDichVuTrongGois)
                    {
                        if (!chiPhi.SoLuongChuaDung.AlmostEqual(0))
                        {
                            var loaiDichVuYeuCau = chiPhi.Nhom == NhomKhamBenh
                                ? LoaiDichVuYeuCau.YeuCauKhamBenh
                                : (chiPhi.Nhom == NhomGiuongBenh ? LoaiDichVuYeuCau.YeuCauDichVuGiuong : LoaiDichVuYeuCau.YeuCauDichVuKyThuat);
                            noiDungQuyetToanGoiMarketings.Add(new NoiDungQuyetToanGoiMarketing
                            {
                                LoaiDichVuYeuCau = loaiDichVuYeuCau,
                                NhomDichVuBenhVienId = chiPhi.NhomDichVuBenhVienId,
                                LoaiDichVuKyThuat = loaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuKyThuat ? CalculateHelper.GetLoaiDichVuKyThuat(chiPhi.NhomDichVuBenhVienId.GetValueOrDefault(), lstNhomDichVuBenhVien) : (Enums.LoaiDichVuKyThuat?)null,
                                NoiDung = chiPhi.TenDichVu,
                                ThanhTien = chiPhi.TTSauCKChuaDung,
                                DonGia = chiPhi.DGSauCK,
                                DichVuBenhVienId = chiPhi.DichVuBenhVienId,
                                NhomGiaDichVuId = chiPhi.NhomGiaDichVuId,
                                SoLuong = chiPhi.SoLuongChuaDung
                            });
                        }
                    }

                    var tongTTSauCKChuaDung = noiDungQuyetToanGoiMarketings.Sum(o => o.ThanhTien);
                    taiKhoanBenhNhanChiQuyetToan.TienChiPhi = tongTTSauCKChuaDung;
                    taiKhoanBenhNhanChiQuyetToan.Gia = tongTTSauCKChuaDung;
                    phieuHoanUngKhac.TienMat = tongTTSauCKChuaDung;
                    thuPhi.TamUng = tongTTSauCKChuaDung;
                }
                else
                {
                    noiDungQuyetToanGoiMarketings.Add(new NoiDungQuyetToanGoiMarketing
                    {
                        LoaiDichVuYeuCau = LoaiDichVuYeuCau.YeuCauDichVuKyThuat,
                        NhomDichVuBenhVienId = 0,
                        LoaiDichVuKyThuat = Enums.LoaiDichVuKyThuat.Khac,
                        NoiDung = "Doanh thu khác",
                        ThanhTien = tienThuKhac
                    });
                }

                if (noiDungQuyetToanGoiMarketings.Count > 0)
                {
                    taiKhoanBenhNhanChiQuyetToan.TaiKhoanBenhNhanChiThongTin = new TaiKhoanBenhNhanChiThongTin
                    {
                        LoaiNoiDung = Enums.LoaiNoiDungChiTien.QuyetToanGoiMarketing,
                        NoiDung = JsonConvert.SerializeObject(noiDungQuyetToanGoiMarketings)
                    };

                    thuPhi.TaiKhoanBenhNhanChis.Add(taiKhoanBenhNhanChiQuyetToan);
                    phieuHoanUngKhac.TaiKhoanBenhNhanThu = thuPhi;
                    yeuCauGoiDichVu.TaiKhoanBenhNhanChis.Add(phieuHoanUngKhac);
                }
            }

            yeuCauGoiDichVu.DaQuyetToan = true;
            yeuCauGoiDichVu.TrangThai = thongTinYeuCauGoiDichVu.HuyGoi == true ? Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy : Enums.EnumTrangThaiYeuCauGoiDichVu.DaThucHien;
            yeuCauGoiDichVu.GhiChu = thongTinYeuCauGoiDichVu.LyDoHuyGoi;
            yeuCauGoiDichVu.ThoiDiemQuyetToan = DateTime.Now;
            yeuCauGoiDichVu.NhanVienQuyetToanId = _userAgentHelper.GetCurrentUserId();
            yeuCauGoiDichVu.NoiQuyetToanId = _userAgentHelper.GetCurrentNoiLLamViecId();
            yeuCauGoiDichVu.SoTienTraLai = thongTinYeuCauGoiDichVu.SoTienTraLai;
            yeuCauGoiDichVu.ThoiDiemHuyQuyetToan = null;
            yeuCauGoiDichVu.NhanVienHuyQuyetToanId = null;
            await _yeuCauGoiDichVuRepository.UpdateAsync(yeuCauGoiDichVu);
            return phieuHoanUng?.Id;
        }

        public async Task HuyQuyetToanGoiDichVu(HuyQuyetToanGoi huyQuyetToanGoi)
        {
            var yeuCauGoiDichVu = await _yeuCauGoiDichVuRepository.GetByIdAsync(huyQuyetToanGoi.YeuCauGoiDichVuId, x => x.Include(o => o.TaiKhoanBenhNhanThus).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.TaiKhoanBenhNhanThu));
            if (yeuCauGoiDichVu.ThoiDiemQuyetToan != null && yeuCauGoiDichVu.ThoiDiemQuyetToan.Value.Date != DateTime.Now.Date)
            {
                throw new Exception("Không thể hủy quyết toán gói dịch vụ đã qua ngày");
            }
            if (yeuCauGoiDichVu.SoTienTraLai.GetValueOrDefault() > 0)
            {
                var phieuHoanUng = yeuCauGoiDichVu.TaiKhoanBenhNhanChis
                    .Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng && o.TaiKhoanBenhNhanThuId == null && o.DaHuy != true).OrderBy(o => o.Id).LastOrDefault();
                if (phieuHoanUng != null)
                {
                    phieuHoanUng.DaHuy = true;
                    phieuHoanUng.NgayHuy = DateTime.Now;
                    phieuHoanUng.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
                    phieuHoanUng.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    phieuHoanUng.LyDoHuy = "Hủy quyết toán gói dịch vụ";
                }
            }
            //hủy phiếu thu khác BVHD-3646
            var phieuThuKhacCuaGoi = yeuCauGoiDichVu.TaiKhoanBenhNhanChis.Where(c => c.YeuCauGoiDichVuId != null && c.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && c.DaHuy != true)
                .Select(o => o.TaiKhoanBenhNhanThu).FirstOrDefault(o => o.DaHuy != true && o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi);

            if (phieuThuKhacCuaGoi != null)
            {
                var phieuHoanUngs = phieuThuKhacCuaGoi.TaiKhoanBenhNhanChis.Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng).ToList();
                foreach (var phieuHoanUng in phieuHoanUngs)
                {
                    phieuHoanUng.DaHuy = true;
                    phieuHoanUng.NgayHuy = DateTime.Now;
                    phieuHoanUng.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
                    phieuHoanUng.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    phieuHoanUng.LyDoHuy = "Hủy quyết toán gói dịch vụ";
                }
                foreach (var phieuThuTaiKhoanBenhNhanChi in phieuThuKhacCuaGoi.TaiKhoanBenhNhanChis.Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi))
                {
                    phieuThuTaiKhoanBenhNhanChi.DaHuy = true;
                    phieuThuTaiKhoanBenhNhanChi.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
                    phieuThuTaiKhoanBenhNhanChi.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    phieuThuTaiKhoanBenhNhanChi.LyDoHuy = "Hủy quyết toán gói dịch vụ";

                }

                phieuThuKhacCuaGoi.DaHuy = true;
                phieuThuKhacCuaGoi.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
                phieuThuKhacCuaGoi.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
                phieuThuKhacCuaGoi.NgayHuy = DateTime.Now;
                phieuThuKhacCuaGoi.LyDoHuy = "Hủy quyết toán gói dịch vụ";
            }



            //if (!yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault()
            //        .SoTienTuongDuong(thongTinYeuCauGoiDichVu.SoTienTraLai + tongTienDaHoan) &&
            //    yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault() >
            //    thongTinYeuCauGoiDichVu.SoTienTraLai + tongTienDaHoan)
            //{
            //    var userId = _userAgentHelper.GetCurrentUserId();
            //    var userThuNgan = _userRepository.GetById(_userAgentHelper.GetCurrentUserId());
            //    var maTaiKhoan = userId.ToString();
            //    if (userThuNgan.Email != null && userThuNgan.Email.IndexOf("@") > 0)
            //    {
            //        maTaiKhoan = userThuNgan.Email.Substring(0, userThuNgan.Email.IndexOf("@") > 10 ? 10 : userThuNgan.Email.IndexOf("@"));
            //    }
            //    yeuCauTiepNhanId = yeuCauTiepNhanId ?? YeuCauTiepNhanCuoiBenhNhan(yeuCauGoiDichVu.BenhNhanId).Id;
            //    var tienThuKhac = yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault() - (thongTinYeuCauGoiDichVu.SoTienTraLai + tongTienDaHoan);

            //    var phieuHoanUngKhac = new TaiKhoanBenhNhanChi
            //    {
            //        TaiKhoanBenhNhan = yeuCauGoiDichVu.BenhNhan.TaiKhoanBenhNhan,
            //        LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.HoanUng,
            //        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
            //        TienMat = tienThuKhac,
            //        NoiDungChi = "Hoàn ứng khác",
            //        NgayChi = DateTime.Now,
            //        YeuCauTiepNhanId = yeuCauTiepNhanId,
            //        SoPhieuHienThi = ResourceHelper.CreateSoHoanUng(),
            //        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
            //        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
            //    };

            //    var thuPhi = new TaiKhoanBenhNhanThu
            //    {
            //        TaiKhoanBenhNhan = yeuCauGoiDichVu.BenhNhan.TaiKhoanBenhNhan,
            //        LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi,
            //        ThuTienGoiDichVu = true,
            //        LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
            //        NoiDungThu = "Thu khác",
            //        NgayThu = DateTime.Now,
            //        TamUng = tienThuKhac,
            //        SoPhieuHienThi = ResourceHelper.CreateSoPhieuThu(userId, maTaiKhoan),
            //        YeuCauTiepNhanId = yeuCauTiepNhanId.Value,
            //        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
            //        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
            //    };
            //    thuPhi.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
            //    {
            //        TaiKhoanBenhNhan = yeuCauGoiDichVu.BenhNhan.TaiKhoanBenhNhan,
            //        LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
            //        TienChiPhi = tienThuKhac,
            //        NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
            //        NgayChi = DateTime.Now,
            //        SoPhieuHienThi = thuPhi.SoPhieuHienThi,
            //        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
            //        Gia = tienThuKhac,
            //        SoLuong = 1,
            //        YeuCauTiepNhanId = yeuCauTiepNhanId,
            //        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
            //        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
            //    });
            //    phieuHoanUngKhac.TaiKhoanBenhNhanThu = thuPhi;
            //    yeuCauGoiDichVu.TaiKhoanBenhNhanChis.Add(phieuHoanUngKhac);
            //    //thuPhi.TaiKhoanBenhNhanChis.Add(phieuHoanUngKhac);
            //}

            yeuCauGoiDichVu.DaQuyetToan = false;
            yeuCauGoiDichVu.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien;
            yeuCauGoiDichVu.ThoiDiemQuyetToan = null;
            yeuCauGoiDichVu.NhanVienQuyetToanId = null;
            yeuCauGoiDichVu.NoiQuyetToanId = null;
            yeuCauGoiDichVu.SoTienTraLai = null;
            yeuCauGoiDichVu.ThoiDiemHuyQuyetToan = DateTime.Now;
            yeuCauGoiDichVu.LyDoHuyQuyetToan = huyQuyetToanGoi.LyDoHuyQuyetToan;
            yeuCauGoiDichVu.NhanVienHuyQuyetToanId = _userAgentHelper.GetCurrentUserId();
            await _yeuCauGoiDichVuRepository.UpdateAsync(yeuCauGoiDichVu);
        }

        public YeuCauTiepNhan YeuCauTiepNhanCuoiBenhNhan(long benhNhanId)
        {
            return BaseRepository.TableNoTracking.Where(o => o.Id == benhNhanId)
                                                      .SelectMany(cc => cc.YeuCauTiepNhans)
                                                      .Where(o => o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy && (o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                                                                                                                        || o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                                                                                                                        || o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.DangKyGoiMarketing))
                                                      .OrderByDescending(c => c.Id)
                                                      .FirstOrDefault();
        }

        public string GetHtmlPhieuThuMarketing(long taiKhoanThuId, string hostingName)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiLapPhieu = _useRepository.GetById(currentUserId).HoTen;

            var cauHinhPhieuThu = _cauHinhService.LoadSetting<CauHinhPhieuThu>();
            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuThuMarketing"));

            var taiKhoanBenhNhanThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(c =>
                    c.Id == taiKhoanThuId).Include(cc => cc.YeuCauTiepNhan).Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.QuanHuyen)
                                        .Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.TinhThanh)
                                        .Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.BenhNhan)
                                        .Include(ccc => ccc.TaiKhoanBenhNhanChis)
                                        .Include(dt => dt.CongTyBaoHiemTuNhanCongNos).FirstOrDefault();

            var dsChiPhi = GetThongTinPhieuThu(taiKhoanThuId, LoaiPhieuThuChiThuNgan.ThuTamUng).Result;


            var tongTienThu = dsChiPhi.TienMat + dsChiPhi.Pos + dsChiPhi.ChuyenKhoan;

            var diaChi = taiKhoanBenhNhanThu?.YeuCauTiepNhan.DiaChiDayDu;

            var ngayThangNam = DateHelper.DOBFormat(taiKhoanBenhNhanThu?.YeuCauTiepNhan.NgaySinh, taiKhoanBenhNhanThu?.YeuCauTiepNhan.ThangSinh, taiKhoanBenhNhanThu?.YeuCauTiepNhan.NamSinh);

            var data = new
            {
                cauHinhPhieuThu.TaiKhoanCo,
                cauHinhPhieuThu.TaiKhoanNo,
                LoaiPhieu = LoaiPhieuThuChiThuNgan.ThuTheoChiPhi.GetDescription().ToUpper(),
                LoaiNguoiNopTien = "Người nộp tiền",
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                NguoiNopTien = taiKhoanBenhNhanThu?.YeuCauTiepNhan.HoTen,

                NamSinh = ngayThangNam,

                taiKhoanBenhNhanThu?.YeuCauTiepNhan.BenhNhan.MaBN,
                GioiTinh = taiKhoanBenhNhanThu?.YeuCauTiepNhan.GioiTinh.GetDescription(),
                DiaChi = diaChi,
                NoiDung = taiKhoanBenhNhanThu?.NoiDungThu,
                taiKhoanBenhNhanThu?.SoQuyen,

                SoPhieu = taiKhoanBenhNhanThu?.SoPhieuHienThi,
                //update 21/06/2022 làm tròn tiền trên phiếu thu ApplyFormatMoneyToDouble -> RoundAndApplyFormatMoney
                TongChiPhi = Convert.ToDouble(taiKhoanBenhNhanThu.TamUng).RoundAndApplyFormatMoney(),
                BHYTT = Convert.ToDouble(dsChiPhi.BHYTThanhToan).RoundAndApplyFormatMoney(),
                MiemGiam = Convert.ToDouble(dsChiPhi.MienGiam).RoundAndApplyFormatMoney(),
                nguoiLapPhieu,
                BenhNhanTT = Convert.ToDouble(dsChiPhi.BenhNhanThanhToan).RoundAndApplyFormatMoney(),
                ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year,
                TienMat = Convert.ToDouble(taiKhoanBenhNhanThu.TamUng).RoundAndApplyFormatMoney(),
                ChuyenKhoan = Convert.ToDouble(dsChiPhi.ChuyenKhoan).RoundAndApplyFormatMoney(),
                Pos = Convert.ToDouble(dsChiPhi.Pos).RoundAndApplyFormatMoney(),
                tongCongNo = Convert.ToDouble(dsChiPhi.CongNo).RoundAndApplyFormatMoney(),
                GoiHienTai = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second,
                SoTienThuChi = "Số tiền phải thu của BN",

                ThanhTien = Convert.ToDouble(tongTienThu).RoundAndApplyFormatMoney(),
                TamUng = Convert.ToDouble(dsChiPhi.TamUng).RoundAndApplyFormatMoney(),

                SoTienBangChu = NumberHelper.ChuyenSoRaText(Math.Round(Convert.ToDouble(taiKhoanBenhNhanThu.TamUng), MidpointRounding.AwayFromZero)),


                KemTheo = "Chứng từ gốc",
                ChungTu = "Chứng từ gốc"
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }

        public string GetHtmlPhieuThuTamUngMarketing(long id, string hostingName)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiLapPhieu = _useRepository.GetById(currentUserId).HoTen;

            var result = _templateRepository.TableNoTracking
                .FirstOrDefault(x => x.Name.Equals("TamUngGoiMarketing"));

            var taiKhoanBenhNhanThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(c =>
                    c.Id == id && c.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng)
                .Include(cc => cc.YeuCauTiepNhan)
                .ThenInclude(cc => cc.PhuongXa)
                .Include(cc => cc.YeuCauTiepNhan)
                .ThenInclude(cc => cc.QuanHuyen)
                .Include(cc => cc.YeuCauTiepNhan)
                .Include(cc => cc.YeuCauTiepNhan)
                .ThenInclude(cc => cc.TinhThanh)
                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan)
                .Include(ccc => ccc.TaiKhoanBenhNhanChis).FirstOrDefault();
            var soTienTamUng = taiKhoanBenhNhanThu?.ChuyenKhoan + taiKhoanBenhNhanThu?.POS +
                               taiKhoanBenhNhanThu?.TienMat;

            var yeuCauGoiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                 .Where(o => o.BenhNhanId == taiKhoanBenhNhanThu.YeuCauTiepNhan.BenhNhanId && o.DaQuyetToan != true && o.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                 .ToList();

            decimal tongChiPhi = 0;
            foreach (var yeuCauGoiDichVu in yeuCauGoiDichVus)
            {
                if (yeuCauGoiDichVu.NgungSuDung == true)
                {
                    var thongTinGoiDichVu = GetThongTinGoiDichVu(yeuCauGoiDichVu.Id).Result;
                    tongChiPhi += thongTinGoiDichVu.TongTienDaDung;
                }
                else
                {
                    tongChiPhi += yeuCauGoiDichVu.GiaSauChietKhau;
                }
            }

            var diaChi = taiKhoanBenhNhanThu?.YeuCauTiepNhan.DiaChiDayDu;
            var noiDungStr = taiKhoanBenhNhanThu?.NoiDungThu.Replace("Thu tiền gói :", "").Split(",");
            var noiDung = string.Empty;

            if (noiDungStr.Length > 0)
            {
                foreach (var item in noiDungStr)
                {
                    noiDung += $"<tr><td> -{item}</td></tr>";
                }
            }

            var ngayThangNam = DateHelper.DOBFormat(taiKhoanBenhNhanThu?.YeuCauTiepNhan.NgaySinh, taiKhoanBenhNhanThu?.YeuCauTiepNhan.ThangSinh, taiKhoanBenhNhanThu?.YeuCauTiepNhan.NamSinh);

            var data = new
            {
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                NguoiNopTien = taiKhoanBenhNhanThu?.YeuCauTiepNhan.HoTen,

                NamSinh = ngayThangNam,

                GioiTinh = taiKhoanBenhNhanThu?.YeuCauTiepNhan.GioiTinh.GetDescription(),
                DiaChi = diaChi,
                DienDai = taiKhoanBenhNhanThu?.NoiDungThu,
                SoQuyen = taiKhoanBenhNhanThu?.SoQuyen,
                SoPhieu = taiKhoanBenhNhanThu?.SoPhieuHienThi,

                ngayThangHientai = "Ngày " + taiKhoanBenhNhanThu.NgayThu.Day + " tháng " + taiKhoanBenhNhanThu.NgayThu.Month + " năm " +
                                  taiKhoanBenhNhanThu.NgayThu.Year,

                TienMat = Convert.ToDouble(taiKhoanBenhNhanThu?.TienMat).ApplyFormatMoneyToDouble(),
                ChuyenKhoan = Convert.ToDouble(taiKhoanBenhNhanThu?.ChuyenKhoan).ApplyFormatMoneyToDouble(),
                Pos = Convert.ToDouble(taiKhoanBenhNhanThu?.POS).ApplyFormatMoneyToDouble(),

                TongChiPhi = Convert.ToDouble(tongChiPhi).ApplyFormatMoneyToDouble(),
                noiDung,


                MaNB = taiKhoanBenhNhanThu?.YeuCauTiepNhan.BenhNhan.MaBN,
                SoTien = Convert.ToDouble(soTienTamUng).ApplyFormatMoneyToDouble(),
                KemTheo = "Chứng từ gốc",
                ChungTu = "Chứng từ gốc",

                NguoiLapPhieu = nguoiLapPhieu,
                GoiHienTai = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" +
                             DateTime.Now.Second,
                SoTienBangChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(soTienTamUng))
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }

        public string GetHtmBangKeDichVu(long yeuCauTiepNhanId, string hostingName)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiLapPhieu = _useRepository.GetById(currentUserId).HoTen;

            var cauHinhPhieuThu = _cauHinhService.LoadSetting<CauHinhPhieuThu>();
            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("BangKeKhamBenhBHYTVaBenhVien"));



            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, null);
            return content;
        }

        public string GetHtmBangKeSuDung(long yeuCauGoiDichVuId, string hostingName)
        {
            var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.GetById(yeuCauGoiDichVuId,
                x => x.Include(o => o.BenhNhan)
                    .Include(o => o.BenhNhanSoSinh)
                    .Include(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
                    .Include(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                    .Include(o => o.NhanVienTuVan).ThenInclude(o => o.User)
                    .Include(o => o.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhomDichVuBenhVien));

            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiLapPhieu = _useRepository.GetById(currentUserId).HoTen;

            var cauHinhPhieuThu = _cauHinhService.LoadSetting<CauHinhPhieuThu>();
            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("BangKeSuDungDVTrongGoi"));


            var dateItemChiPhis = string.Empty;
            int sttItem = 1;
            decimal tongGiaDichVu = 0M;

            if (yeuCauGoiDichVu.YeuCauKhamBenhs.Any(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
            {
                dateItemChiPhis += "<tr class='border'>" +
                                   "<td class='border' colspan='5'><strong>Khám bệnh</strong></td>" +
                                   "</tr>";

                foreach (var yeuCauKhamBenhGroup in yeuCauGoiDichVu.YeuCauKhamBenhs.GroupBy(o => new { o.DichVuKhamBenhBenhVienId, o.NhomGiaDichVuKhamBenhBenhVienId }))
                {
                    dateItemChiPhis += "<tr class='border'>" +
                                       "<td class='border1'>" + sttItem + "</td>" +
                                       "<td class='border2'>" + yeuCauKhamBenhGroup.First().TenDichVu + "</td>" +
                                       "<td class='border3' style='text-align: right'>" + yeuCauKhamBenhGroup.First().DonGiaSauChietKhau.GetValueOrDefault().ApplyFormatMoneyVND() + "</td>" +
                                       "<td class='border4' style='text-align: center;'>" + yeuCauKhamBenhGroup.Count() + "</td>" +
                                       "<td class='border5' style='text-align: right'>" + (yeuCauKhamBenhGroup.First().DonGiaSauChietKhau.GetValueOrDefault() * yeuCauKhamBenhGroup.Count()).ApplyFormatMoneyVND() + "</td>" +
                                       "</tr>";
                    sttItem++;
                    tongGiaDichVu += yeuCauKhamBenhGroup.First().DonGiaSauChietKhau.GetValueOrDefault() * yeuCauKhamBenhGroup.Count();
                }
            }
            if (yeuCauGoiDichVu.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Any())
            {
                dateItemChiPhis += "<tr class='border'>" +
                                   "<td class='border' colspan='5'><strong>Giường bệnh</strong></td>" +
                                   "</tr>";

                foreach (var yeuCauDichVuGiuongGroup in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.GroupBy(o => new { o.DichVuGiuongBenhVienId, o.NhomGiaDichVuGiuongBenhVienId }))
                {
                    dateItemChiPhis += "<tr class='border'>" +
                                       "<td class='border1'>" + sttItem + "</td>" +
                                       "<td class='border2'>" + yeuCauDichVuGiuongGroup.First().Ten + "</td>" +
                                       "<td class='border3' style='text-align: right'>" + yeuCauDichVuGiuongGroup.First().DonGiaSauChietKhau.GetValueOrDefault().ApplyFormatMoneyVND() + "</td>" +
                                       "<td class='border4' style='text-align: center;'>" + yeuCauDichVuGiuongGroup.Sum(o => o.SoLuong) + "</td>" +
                                       "<td class='border5' style='text-align: right'>" + (yeuCauDichVuGiuongGroup.First().DonGiaSauChietKhau.GetValueOrDefault() * (decimal)yeuCauDichVuGiuongGroup.Sum(o => o.SoLuong)).ApplyFormatMoneyVND() + "</td>" +
                                       "</tr>";
                    sttItem++;
                    tongGiaDichVu += yeuCauDichVuGiuongGroup.First().DonGiaSauChietKhau.GetValueOrDefault() * (decimal)yeuCauDichVuGiuongGroup.Sum(o => o.SoLuong);
                }

            }
            if (yeuCauGoiDichVu.YeuCauDichVuKyThuats.Any(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            {
                foreach (var nhomDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.GroupBy(o => o.NhomDichVuBenhVienId))
                {
                    dateItemChiPhis += "<tr class='border'>" +
                                       "<td class='border' colspan='5'><strong>" + nhomDichVuKyThuat.First().NhomDichVuBenhVien.Ten + "</strong></td>" +
                                       "</tr>";

                    foreach (var yeuCauDichVuKyThuatGroup in nhomDichVuKyThuat.GroupBy(o => new { o.DichVuKyThuatBenhVienId, o.NhomGiaDichVuKyThuatBenhVienId }))
                    {
                        dateItemChiPhis += "<tr class='border'>" +
                                           "<td class='border1'>" + sttItem + "</td>" +
                                           "<td class='border2'>" + yeuCauDichVuKyThuatGroup.First().TenDichVu + "</td>" +
                                           "<td class='border3' style='text-align: right'>" + (yeuCauDichVuKyThuatGroup.First().DonGiaSauChietKhau.GetValueOrDefault()).ApplyFormatMoneyVND() + "</td>" +
                                           "<td class='border4' style='text-align: center;'>" + yeuCauDichVuKyThuatGroup.Sum(o => o.SoLan) + "</td>" +
                                           "<td class='border5' style='text-align: right'>" + (yeuCauDichVuKyThuatGroup.First().DonGiaSauChietKhau.GetValueOrDefault() * (decimal)yeuCauDichVuKyThuatGroup.Sum(o => o.SoLan)).ApplyFormatMoneyVND() + "</td>" +
                                           "</tr>";
                        sttItem++;
                        tongGiaDichVu += yeuCauDichVuKyThuatGroup.First().DonGiaSauChietKhau.GetValueOrDefault() * (decimal)yeuCauDichVuKyThuatGroup.Sum(o => o.SoLan);
                    }
                }

            }
            var benhNhan = yeuCauGoiDichVu.BenhNhanSoSinh ?? yeuCauGoiDichVu.BenhNhan;
            var ngayThangNam = DateHelper.DOBFormat(benhNhan?.NgaySinh, benhNhan?.ThangSinh, benhNhan?.NamSinh);

            var data = new
            {
                TenBenhNhan = benhNhan.HoTen,
                NamSinh = ngayThangNam,

                GioiTinh = benhNhan.GioiTinh?.GetDescription(),
                DiaChi = benhNhan.DiaChiDayDu,
                TenGoi = yeuCauGoiDichVu.TenChuongTrinh,
                NgayDangKy = yeuCauGoiDichVu.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),
                NhanVienDangKy = yeuCauGoiDichVu.NhanVienChiDinh.User.HoTen,
                NoiDangKy = $"{yeuCauGoiDichVu.NoiChiDinh.KhoaPhong.Ten} - {yeuCauGoiDichVu.NoiChiDinh.Ten}",
                NhanVienTuVan = yeuCauGoiDichVu.NhanVienTuVan?.User.HoTen,
                DateItemChiPhis = dateItemChiPhis,
                NgayHienTai = DateTime.Now.ApplyFormatNgayThangNam(),

                Ngay = DateTime.Now.Day,
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                TongGiaDichVu = tongGiaDichVu.ApplyFormatMoneyVND(),
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                MaYTe = yeuCauGoiDichVu.BenhNhan.MaBN,
                BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(yeuCauGoiDichVu.BenhNhan.Id.ToString()),
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }

        private const string NhomKhamBenh = "Khám bệnh";
        private const string NhomGiuongBenh = "Giường bệnh";
        public async Task<ThongTinYeuCauGoiDichVu> GetThongTinGoiDichVu(long yeuCauGoiDichVuId)
        {
            var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.GetById(yeuCauGoiDichVuId,
                x => x.Include(o => o.BenhNhan)
                    .Include(o => o.BenhNhanSoSinh)
                    .Include(o => o.TaiKhoanBenhNhanChis)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(o => o.NhomGiaDichVuKhamBenhBenhVien)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(o => o.DichVuGiuongBenhVien)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(o => o.NhomGiaDichVuGiuongBenhVien)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(o => o.DichVuKyThuatBenhVien).ThenInclude(o => o.NhomDichVuBenhVien)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(o => o.NhomGiaDichVuKyThuatBenhVien)
                    .Include(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
                    .Include(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                    .Include(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
                    .Include(o => o.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhomDichVuBenhVien));

            ThongTinYeuCauGoiDichVu thongTinYeuCauGoiDichVu = new ThongTinYeuCauGoiDichVu
            {
                BenhNhanId = yeuCauGoiDichVu.BenhNhanId,
                YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                TenChuongTrinhGoiDichVu = yeuCauGoiDichVu.TenChuongTrinh,
                TrangThaiQuyetToanDichVu = yeuCauGoiDichVu.DaQuyetToan == true ? TrangThaiQuyetToanDichVu.DaQuyetToan : TrangThaiQuyetToanDichVu.ChuaQuyetToan,
                TongTienGoi = yeuCauGoiDichVu.GiaSauChietKhau,
                SoTienTraLai = yeuCauGoiDichVu.SoTienTraLai.GetValueOrDefault(),
                DanhSachDichVuTrongGois = new List<DanhSachDichVuTrongGoi>(),

                HuyGoi = yeuCauGoiDichVu.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy,
                LyDoHuyGoi = yeuCauGoiDichVu.GhiChu
            };
            foreach (var dichVuKhamBenh in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs)
            {
                thongTinYeuCauGoiDichVu.DanhSachDichVuTrongGois.Add(new DanhSachDichVuTrongGoi
                {
                    MaDichVu = dichVuKhamBenh.DichVuKhamBenhBenhVien.Ma,
                    LoaiNhom = EnumDichVuTongHop.KhamBenh,
                    DichVuBenhVienId = dichVuKhamBenh.DichVuKhamBenhBenhVienId,
                    NhomGiaDichVuId = dichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVienId,
                    Nhom = NhomKhamBenh,
                    TenDichVu = dichVuKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    LoaiGia = dichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    SoLuong = dichVuKhamBenh.SoLan,
                    DGBV = dichVuKhamBenh.DonGia,
                    DGTruocCK = dichVuKhamBenh.DonGiaTruocChietKhau,
                    DGSauCK = dichVuKhamBenh.DonGiaSauChietKhau,
                    SoLuongDaDung = yeuCauGoiDichVu.YeuCauKhamBenhs.Count(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DichVuKhamBenhBenhVienId == dichVuKhamBenh.DichVuKhamBenhBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == dichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVienId)
                });
            }
            foreach (var dichVuGiuong in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs)
            {
                thongTinYeuCauGoiDichVu.DanhSachDichVuTrongGois.Add(new DanhSachDichVuTrongGoi
                {
                    MaDichVu = dichVuGiuong.DichVuGiuongBenhVien.Ma,
                    Nhom = NhomGiuongBenh,
                    LoaiNhom = EnumDichVuTongHop.GiuongBenh,
                    DichVuBenhVienId = dichVuGiuong.DichVuGiuongBenhVienId,
                    NhomGiaDichVuId = dichVuGiuong.NhomGiaDichVuGiuongBenhVienId,
                    TenDichVu = dichVuGiuong.DichVuGiuongBenhVien.Ten,
                    LoaiGia = dichVuGiuong.NhomGiaDichVuGiuongBenhVien.Ten,
                    SoLuong = dichVuGiuong.SoLan,
                    DGBV = dichVuGiuong.DonGia,
                    DGTruocCK = dichVuGiuong.DonGiaTruocChietKhau,
                    DGSauCK = dichVuGiuong.DonGiaSauChietKhau,
                    SoLuongDaDung = yeuCauGoiDichVu.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Where(o => o.DichVuGiuongBenhVienId == dichVuGiuong.DichVuGiuongBenhVienId && o.NhomGiaDichVuGiuongBenhVienId == dichVuGiuong.NhomGiaDichVuGiuongBenhVienId).Select(o => o.SoLuong).DefaultIfEmpty().Sum()
                });
            }
            foreach (var nhomDichVuKyThuat in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.GroupBy(o => o.DichVuKyThuatBenhVien.NhomDichVuBenhVienId))
            {

                foreach (var dichVuKyThuat in nhomDichVuKyThuat)
                {
                    thongTinYeuCauGoiDichVu.DanhSachDichVuTrongGois.Add(new DanhSachDichVuTrongGoi
                    {
                        MaDichVu = dichVuKyThuat.DichVuKyThuatBenhVien.Ma,
                        NhomDichVuBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                        Nhom = dichVuKyThuat.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten,
                        LoaiNhom = EnumDichVuTongHop.KyThuat,
                        DichVuBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVienId,
                        NhomGiaDichVuId = dichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId,
                        TenDichVu = dichVuKyThuat.DichVuKyThuatBenhVien.Ten,
                        LoaiGia = dichVuKyThuat.NhomGiaDichVuKyThuatBenhVien.Ten,
                        SoLuong = dichVuKyThuat.SoLan,
                        DGBV = dichVuKyThuat.DonGia,
                        DGTruocCK = dichVuKyThuat.DonGiaTruocChietKhau,
                        DGSauCK = dichVuKyThuat.DonGiaSauChietKhau,
                        SoLuongDaDung = yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.DichVuKyThuatBenhVienId == dichVuKyThuat.DichVuKyThuatBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == dichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId).Select(o => o.SoLan).DefaultIfEmpty().Sum()
                    });
                }
            }
            thongTinYeuCauGoiDichVu.TongTienDaThu = yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault();

            thongTinYeuCauGoiDichVu.TongTienDaDung = thongTinYeuCauGoiDichVu.DanhSachDichVuTrongGois.Sum(o => o.TTSauCKDaDung);

            return thongTinYeuCauGoiDichVu;
        }

        public async Task LuuThongTinMuonChuyenGoiMoi(LuuThongTinChuyenGoiMoi thongTinChuyenGois)
        {
            if (thongTinChuyenGois.DichVuTrongGoiMarketingModels == null ||
                thongTinChuyenGois.DichVuTrongGoiMarketingModels.Count == 0)
            {
                throw new Exception("Chưa có dịch vụ trong gói");
            }
            var nhomDvTrung = thongTinChuyenGois.DichVuTrongGoiMarketingModels
                .GroupBy(o => new {o.Nhom, o.DichVuBenhVienId, o.NhomGiaDichVuId}).FirstOrDefault(o => o.Count() > 1);
            if (nhomDvTrung != null)
            {
                throw new Exception($"Dịch vụ {nhomDvTrung.First().Ten} bị trùng");
            }
            var yeuCauGoiDichVuTruoc = _yeuCauGoiDichVuRepository.GetById(thongTinChuyenGois.YeuCauGoiDichVuId,
                x => x.Include(o => o.ChuongTrinhGoiDichVu).Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan));

            if (yeuCauGoiDichVuTruoc.DaQuyetToan == true)
            {
                throw new Exception("Gói dịch vụ đã quyết toán");
            }

            //them chuongTrinhGoiDichVuMoi
            var chuongTrinhGoiDichVuMoi = new ChuongTrinhGoiDichVu
            {
                Ten = thongTinChuyenGois.TenGoiMoi,
                Ma = thongTinChuyenGois.MaGoiMoi,
                GoiDichVuId = yeuCauGoiDichVuTruoc.ChuongTrinhGoiDichVu.GoiDichVuId,
                TenGoiDichVu = yeuCauGoiDichVuTruoc.ChuongTrinhGoiDichVu.TenGoiDichVu,
                MoTaGoiDichVu = yeuCauGoiDichVuTruoc.ChuongTrinhGoiDichVu.MoTaGoiDichVu,
                TuNgay = DateTime.Today,
                GoiSoSinh = yeuCauGoiDichVuTruoc.GoiSoSinh,
                BenhNhanId = yeuCauGoiDichVuTruoc.BenhNhanId
            };

            foreach (var goiDvKhamBenh in thongTinChuyenGois.DichVuTrongGoiMarketingModels.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KhamBenh))
            {
                var chuongTrinhDvKhamBenh = new ChuongTrinhGoiDichVuDichVuKhamBenh
                {
                    DichVuKhamBenhBenhVienId = goiDvKhamBenh.DichVuBenhVienId,
                    NhomGiaDichVuKhamBenhBenhVienId = goiDvKhamBenh.NhomGiaDichVuId,
                    DonGia = goiDvKhamBenh.DonGiaBenhVien,
                    DonGiaSauChietKhau = goiDvKhamBenh.DonGiaSauChietKhau,
                    DonGiaTruocChietKhau = goiDvKhamBenh.DonGiaTruocChietKhau,
                    SoLan = goiDvKhamBenh.SoLuong
                };
                chuongTrinhGoiDichVuMoi.ChuongTrinhGoiDichVuDichKhamBenhs.Add(chuongTrinhDvKhamBenh);
            }

            foreach (var goiDvKyThuat in thongTinChuyenGois.DichVuTrongGoiMarketingModels.Where(o => o.Nhom == Enums.EnumDichVuTongHop.KyThuat))
            {
                var chuongTrinhDvKyThuat = new ChuongTrinhGoiDichVuDichVuKyThuat
                {
                    DichVuKyThuatBenhVienId = goiDvKyThuat.DichVuBenhVienId,
                    NhomGiaDichVuKyThuatBenhVienId = goiDvKyThuat.NhomGiaDichVuId,
                    DonGia = goiDvKyThuat.DonGiaBenhVien,
                    DonGiaSauChietKhau = goiDvKyThuat.DonGiaSauChietKhau,
                    DonGiaTruocChietKhau = goiDvKyThuat.DonGiaTruocChietKhau,
                    SoLan = goiDvKyThuat.SoLuong
                };
                chuongTrinhGoiDichVuMoi.ChuongTrinhGoiDichVuDichVuKyThuats.Add(chuongTrinhDvKyThuat);
            }
        
            foreach (var goiDvGiuongBenh in thongTinChuyenGois.DichVuTrongGoiMarketingModels.Where(o => o.Nhom == Enums.EnumDichVuTongHop.GiuongBenh))
            {
                var chuongTrinhDvGiuongBenh = new ChuongTrinhGoiDichVuDichVuGiuong
                {
                    DichVuGiuongBenhVienId = goiDvGiuongBenh.DichVuBenhVienId,
                    NhomGiaDichVuGiuongBenhVienId = goiDvGiuongBenh.NhomGiaDichVuId,
                    DonGia = goiDvGiuongBenh.DonGiaBenhVien,
                    DonGiaSauChietKhau = goiDvGiuongBenh.DonGiaSauChietKhau,
                    DonGiaTruocChietKhau = goiDvGiuongBenh.DonGiaTruocChietKhau,
                    SoLan = goiDvGiuongBenh.SoLuong
                };
                chuongTrinhGoiDichVuMoi.ChuongTrinhGoiDichVuDichVuGiuongs.Add(chuongTrinhDvGiuongBenh);
            }

            chuongTrinhGoiDichVuMoi.GiaTruocChietKhau =
                chuongTrinhGoiDichVuMoi.ChuongTrinhGoiDichVuDichKhamBenhs.Sum(o => o.DonGiaTruocChietKhau * o.SoLan) +
                chuongTrinhGoiDichVuMoi.ChuongTrinhGoiDichVuDichVuKyThuats.Sum(o => o.DonGiaTruocChietKhau * o.SoLan) +
                chuongTrinhGoiDichVuMoi.ChuongTrinhGoiDichVuDichVuGiuongs.Sum(o => o.DonGiaTruocChietKhau * o.SoLan);

            chuongTrinhGoiDichVuMoi.GiaSauChietKhau =
                chuongTrinhGoiDichVuMoi.ChuongTrinhGoiDichVuDichKhamBenhs.Sum(o => o.DonGiaSauChietKhau * o.SoLan) +
                chuongTrinhGoiDichVuMoi.ChuongTrinhGoiDichVuDichVuKyThuats.Sum(o => o.DonGiaSauChietKhau * o.SoLan) +
                chuongTrinhGoiDichVuMoi.ChuongTrinhGoiDichVuDichVuGiuongs.Sum(o => o.DonGiaSauChietKhau * o.SoLan);

            //them yeu cau cho goi moi
            var yeuCauGoiDichVuMoi = new YeuCauGoiDichVu
            {
                BenhNhanId = yeuCauGoiDichVuTruoc.BenhNhanId,
                BenhNhanSoSinhId = yeuCauGoiDichVuTruoc.BenhNhanSoSinhId,
                ChuongTrinhGoiDichVu = chuongTrinhGoiDichVuMoi,
                MaChuongTrinh = chuongTrinhGoiDichVuMoi.Ma,
                TenChuongTrinh = chuongTrinhGoiDichVuMoi.Ten,
                GiaTruocChietKhau = chuongTrinhGoiDichVuMoi.GiaTruocChietKhau,
                GiaSauChietKhau = chuongTrinhGoiDichVuMoi.GiaSauChietKhau,
                TenGoiDichVu = chuongTrinhGoiDichVuMoi.TenGoiDichVu,
                MoTaGoiDichVu = chuongTrinhGoiDichVuMoi.MoTaGoiDichVu,
                GoiSoSinh = chuongTrinhGoiDichVuMoi.GoiSoSinh,
                NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                ThoiDiemChiDinh = DateTime.Now,
                BoPhanMarketingDangKy = true,
                TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien,
                TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
            };
            //update goi truoc
            yeuCauGoiDichVuTruoc.NgungSuDung = true;
            //chuyen tam ung
            if (yeuCauGoiDichVuTruoc.SoTienBenhNhanDaChi.GetValueOrDefault() > 0)
            {
                var thongTinGoiDichVu = await GetThongTinGoiDichVu(thongTinChuyenGois.YeuCauGoiDichVuId);
                var tongTTSauCKDaDung = thongTinGoiDichVu.DanhSachDichVuTrongGois.Sum(o => o.TTSauCKDaDung);
                if (yeuCauGoiDichVuTruoc.SoTienBenhNhanDaChi.GetValueOrDefault() > tongTTSauCKDaDung)
                {
                    //chuyen so du qua goi moi
                    var yeuCauTiepNhanId = YeuCauTiepNhanCuoiBenhNhan(yeuCauGoiDichVuTruoc.BenhNhanId)?.Id;
                    var phieuHoanUng = new TaiKhoanBenhNhanChi
                    {
                        TaiKhoanBenhNhan = yeuCauGoiDichVuTruoc.BenhNhan.TaiKhoanBenhNhan,
                        LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.HoanUng,
                        YeuCauGoiDichVuId = yeuCauGoiDichVuTruoc.Id,
                        TienMat = yeuCauGoiDichVuTruoc.SoTienBenhNhanDaChi.GetValueOrDefault() - tongTTSauCKDaDung,
                        NoiDungChi = Enums.LoaiChiTienBenhNhan.HoanUng.GetDescription(),
                        NgayChi = DateTime.Now,
                        YeuCauTiepNhanId = yeuCauTiepNhanId,
                        SoPhieuHienThi = ResourceHelper.CreateSoHoanUng(),
                        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                    };
                    yeuCauGoiDichVuTruoc.TaiKhoanBenhNhanChis.Add(phieuHoanUng);
                    var thuPhiTamUng = new TaiKhoanBenhNhanThu
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhanId.Value,
                        TaiKhoanBenhNhan = yeuCauGoiDichVuTruoc.BenhNhan.TaiKhoanBenhNhan,
                        LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.ThuTamUng,
                        LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                        ThuTienGoiDichVu = true,
                        TienMat = phieuHoanUng.TienMat,
                        NoiDungThu = Enums.LoaiThuTienBenhNhan.ThuTamUng.GetDescription(),
                        NgayThu = DateTime.Now,
                        SoPhieuHienThi = ResourceHelper.CreateSoTamUng(),
                        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                    };

                    yeuCauGoiDichVuTruoc.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                    {
                        TaiKhoanBenhNhan = yeuCauGoiDichVuTruoc.BenhNhan.TaiKhoanBenhNhan,
                        LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                        TienChiPhi = thuPhiTamUng.TienMat * (-1),
                        NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                        NgayChi = DateTime.Now,
                        SoPhieuHienThi = thuPhiTamUng.SoPhieuHienThi,
                        TaiKhoanBenhNhanThu = thuPhiTamUng,
                        Gia = yeuCauGoiDichVuTruoc.GiaSauChietKhau,
                        SoLuong = 1,
                        YeuCauTiepNhanId = yeuCauTiepNhanId,
                        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                    });
                    yeuCauGoiDichVuTruoc.SoTienBenhNhanDaChi = yeuCauGoiDichVuTruoc.SoTienBenhNhanDaChi - thuPhiTamUng.TienMat;

                    yeuCauGoiDichVuMoi.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                    {
                        TaiKhoanBenhNhan = yeuCauGoiDichVuTruoc.BenhNhan.TaiKhoanBenhNhan,
                        LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                        TienChiPhi = thuPhiTamUng.TienMat,
                        NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                        NgayChi = DateTime.Now,
                        SoPhieuHienThi = thuPhiTamUng.SoPhieuHienThi,
                        TaiKhoanBenhNhanThu = thuPhiTamUng,
                        Gia = yeuCauGoiDichVuMoi.GiaSauChietKhau,
                        SoLuong = 1,
                        YeuCauTiepNhanId = yeuCauTiepNhanId,
                        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                    });
                    yeuCauGoiDichVuMoi.SoTienBenhNhanDaChi = thuPhiTamUng.TienMat;
                    yeuCauGoiDichVuMoi.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
                }
            }
            _yeuCauGoiDichVuRepository.Add(yeuCauGoiDichVuMoi);
        }
    }
}
