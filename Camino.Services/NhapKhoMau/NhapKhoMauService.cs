using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhapKhoMaus;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenMau;

namespace Camino.Services.NhapKhoMau
{
    [ScopedDependency(ServiceType = typeof(INhapKhoMauService))]
    public class NhapKhoMauService : MasterFileService<Core.Domain.Entities.MauVaChePhams.NhapKhoMau>, INhapKhoMauService
    {
        private IRepository<NhapKhoMauChiTiet> _nhapKhoMauChiTietRepository;
        private IRepository<XuatKhoMau> _xuatKhoMauRepository;
        private IRepository<YeuCauTruyenMau> _yeuCauTruyenMauRepository;
        private IRepository<Template> _templateRepository;
        private IUserAgentHelper _userAgentHelper;
        private ILocalizationService _localizationService;
        private IRepository<NoiTruHoSoKhac> _noiTruHoSoKhacRepository;
        public NhapKhoMauService(IRepository<Core.Domain.Entities.MauVaChePhams.NhapKhoMau> repository,
            IRepository<NhapKhoMauChiTiet> nhapKhoMauChiTietRepository,
            IRepository<YeuCauTruyenMau> yeuCauTruyenMauRepository,
            IRepository<Template> templateRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<NoiTruHoSoKhac> noiTruHoSoKhacRepository,
            IRepository<XuatKhoMau> xuatKhoMauRepository,
            ILocalizationService localizationService) : base(repository)
        {
            _nhapKhoMauChiTietRepository = nhapKhoMauChiTietRepository;
            _yeuCauTruyenMauRepository = yeuCauTruyenMauRepository;
            _templateRepository = templateRepository;
            _userAgentHelper = userAgentHelper;
            _localizationService = localizationService;
            _noiTruHoSoKhacRepository = noiTruHoSoKhacRepository;
            _xuatKhoMauRepository = xuatKhoMauRepository;
        }

        #region Grid
        public async Task<GridDataSource> GetDataForGridNhapKhoMauAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new NhapKhoMauTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<NhapKhoMauTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking
                .Select(item => new PhieuNhapKhoMauGridVo()
                {
                    Id = item.Id,
                    SoPhieu = item.SoPhieu,
                    SoHoaDon = item.SoHoaDon,
                    NgayHoaDon = item.NgayHoaDon,
                    NhaCungCap = item.NhaThau != null ? item.NhaThau.Ten : "",
                    GhiChu = item.GhiChu,
                    TinhTrang = item.DuocKeToanDuyet == true ? Enums.TrangThaiNhapKhoMau.DaNhapGia : Enums.TrangThaiNhapKhoMau.ChoNhapGia,
                    NguoiNhap = item.NguoiNhap.User.HoTen,
                    NgayNhap = item.NgayNhap,
                    NguoiDuyet = item.NhanVienDuyet.User.HoTen,
                    NgayDuyet = item.NgayDuyet
                })
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.SoPhieu, x => x.SoHoaDon, x => x.NhaCungCap);
            //x => x.GhiChu, x => x.NguoiNhap, x => x.NguoiDuyet, 

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || (p.NgayNhap != null && p.NgayNhap.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || (p.NgayNhap != null && p.NgayNhap.Value.Date <= denNgay.Date)));
            }

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChoNhapGia || timKiemNangCaoObj.TrangThai.DaNhapGia))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChoNhapGia && x.TinhTrang == Enums.TrangThaiNhapKhoMau.ChoNhapGia)
                    || (timKiemNangCaoObj.TrangThai.DaNhapGia && x.TinhTrang == Enums.TrangThaiNhapKhoMau.DaNhapGia));
            }

            #region //BVHD-3926
            if (timKiemNangCaoObj.TuNgayDenNgayHoaDon != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHoaDon.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHoaDon.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHoaDon.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHoaDon.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHoaDon.TuNgay) || (p.NgayHoaDon != null && p.NgayHoaDon.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHoaDon.DenNgay) || (p.NgayHoaDon != null && p.NgayHoaDon.Value.Date <= denNgay.Date)));
            }
            #endregion

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TinhTrang";
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).ThenBy(x => x.NgayNhap).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridNhapKhoMauAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new NhapKhoMauTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<NhapKhoMauTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking
                .Select(item => new PhieuNhapKhoMauGridVo()
                {
                    Id = item.Id,
                    SoPhieu = item.SoPhieu,
                    SoHoaDon = item.SoHoaDon,
                    NgayHoaDon = item.NgayHoaDon,
                    NhaCungCap = item.NhaThau != null ? item.NhaThau.Ten : "",
                    GhiChu = item.GhiChu,
                    TinhTrang = item.DuocKeToanDuyet == true ? Enums.TrangThaiNhapKhoMau.DaNhapGia : Enums.TrangThaiNhapKhoMau.ChoNhapGia,
                    NguoiNhap = item.NguoiNhap.User.HoTen,
                    NgayNhap = item.NgayNhap,
                    NguoiDuyet = item.NhanVienDuyet.User.HoTen,
                    NgayDuyet = item.NgayDuyet
                })
                .ApplyLike(timKiemNangCaoObj.SearchString, x => x.SoPhieu, x => x.SoHoaDon, x => x.NhaCungCap);
            //x => x.GhiChu, x => x.NguoiNhap, x => x.NguoiDuyet,

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || (p.NgayNhap != null && p.NgayNhap.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || (p.NgayNhap != null && p.NgayNhap.Value.Date <= denNgay.Date)));
            }

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChoNhapGia || timKiemNangCaoObj.TrangThai.DaNhapGia))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChoNhapGia && x.TinhTrang == Enums.TrangThaiNhapKhoMau.ChoNhapGia)
                    || (timKiemNangCaoObj.TrangThai.DaNhapGia && x.TinhTrang == Enums.TrangThaiNhapKhoMau.DaNhapGia));
            }

            #region //BVHD-3926
            if (timKiemNangCaoObj.TuNgayDenNgayHoaDon != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHoaDon.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHoaDon.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHoaDon.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHoaDon.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHoaDon.TuNgay) || (p.NgayHoaDon != null && p.NgayHoaDon.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHoaDon.DenNgay) || (p.NgayHoaDon != null && p.NgayHoaDon.Value.Date <= denNgay.Date)));
            }


            #endregion

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridNhapKhoMauChiTietAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var phieuNhapKhoMauId = long.Parse(queryInfo.AdditionalSearchString);

            var query = _nhapKhoMauChiTietRepository.TableNoTracking
                .Where(x => x.NhapKhoMauId == phieuNhapKhoMauId)
                .Select(item => new PhieuNhapKhoMauGridChiTietVo()
                {
                    NhomMauNguoiBenh = item.YeuCauTruyenMau.NhomMau != null ? (item.YeuCauTruyenMau.NhomMau.GetDescription() + (item.YeuCauTruyenMau.YeuToRh != null ? " Rh(" + item.YeuCauTruyenMau.YeuToRh.GetDescription() + ")" : "")) : "",
                    MaTuiMau = item.MaTuiMau,
                    ChePhamMau = item.YeuCauTruyenMau.TenDichVu,
                    NgaySanXuat = item.NgaySanXuat,
                    HanSuDung = item.HanSuDung,
                    DonGiaDichVu = item.DonGiaNhap,
                    DonGiaBaoHiem = item.DonGiaBaoHiem
                });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridNhapKhoMauChiTietAsync(QueryInfo queryInfo)
        {
            var phieuNhapKhoMauId = long.Parse(queryInfo.AdditionalSearchString);

            var query = _nhapKhoMauChiTietRepository.TableNoTracking
                .Where(x => x.NhapKhoMauId == phieuNhapKhoMauId)
                .Select(item => new PhieuNhapKhoMauGridChiTietVo()
                {
                    NhomMauNguoiBenh = item.YeuCauTruyenMau.NhomMau != null ? (item.YeuCauTruyenMau.NhomMau.GetDescription() + (item.YeuCauTruyenMau.YeuToRh != null ? " Rh(" + item.YeuCauTruyenMau.YeuToRh.GetDescription() + ")" : "")) : "",
                    MaTuiMau = item.MaTuiMau,
                    ChePhamMau = item.YeuCauTruyenMau.TenDichVu,
                    NgaySanXuat = item.NgaySanXuat,
                    HanSuDung = item.HanSuDung,
                    DonGiaDichVu = item.DonGiaNhap,
                    DonGiaBaoHiem = item.DonGiaBaoHiem
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region lookup
        public async Task<List<LookupItemYeuCauTruyenMauVo>> GetListYeuCauTruyenMauAsync(DropDownListRequestModel model)
        {
            //mấy bác khách hàng muốn chọn bệnh nhân trước sau đó load chế phẩm.  x.MauVaChePhamId == mauVaChePhamId x.MauVaChePhamId == mauVaChePhamId
            var mauVaChePhamId = CommonHelper.GetIdFromRequestDropDownList(model);
            var query = await _yeuCauTruyenMauRepository.TableNoTracking
                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy
                            && !x.NhapKhoMauChiTiets.Any()
                            && x.Id != model.Id)
                .Select(item => new LookupItemYeuCauTruyenMauVo()
                {
                    KeyId = item.Id,
                    DisplayName = item.YeuCauTiepNhan.HoTen,
                    MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = item.YeuCauTiepNhan.BenhNhan.MaBN,
                    MaBenhAn = item.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                    HoTen = item.YeuCauTiepNhan.HoTen,
                    MaChePhamMau = item.MaDichVu,
                    TenChePhamMau = item.TenDichVu,
                    ChePhamMauId = item.MauVaChePham.Id,
                    PhanLoaiMau = (int)item.PhanLoaiMau,
                    NhomMau = item.NhomMau != null ? "\"" + item.NhomMau.GetDescription() + "\"" + (item.YeuToRh != null ? " Rh(" + item.YeuToRh.GetDescription() + ")" : "") : null,
                        //item.NhomMau.GetDescription() + (item.YeuToRh == null ? "" : (item.YeuToRh == Enums.EnumYeuToRh.Amtinh ? "-" : "+")),
                    TheTich = item.TheTich,
                    TheTichDisplay = item.TheTich + "ml"
                })
                .Union(
                    _yeuCauTruyenMauRepository.TableNoTracking
                        .Where(x =>  x.Id == model.Id)
                        .Select(item => new LookupItemYeuCauTruyenMauVo()
                        {
                            KeyId = item.Id,
                            DisplayName = item.YeuCauTiepNhan.HoTen,
                            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            MaBenhNhan = item.YeuCauTiepNhan.BenhNhan.MaBN,
                            MaBenhAn = item.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                            HoTen = item.YeuCauTiepNhan.HoTen,
                            MaChePhamMau = item.MaDichVu,
                            TenChePhamMau = item.TenDichVu,
                            ChePhamMauId = item.MauVaChePham.Id,
                            PhanLoaiMau = (int)item.PhanLoaiMau,
                            NhomMau = item.NhomMau != null ? "\"" + item.NhomMau.GetDescription() + "\"" + (item.YeuToRh != null ? " Rh(" + item.YeuToRh.GetDescription() + ")" : "") : null,
                                //item.NhomMau.GetDescription() + (item.YeuToRh == null ? "" : (item.YeuToRh == Enums.EnumYeuToRh.Amtinh ? "-" : "+")),
                            TheTich = item.TheTich,
                            TheTichDisplay = item.TheTich + "ml"
                        })
                    ).ApplyLike(model.Query, x => x.MaYeuCauTiepNhan, x => x.MaBenhNhan, x => x.HoTen,
                    x => x.TenChePhamMau, x => x.NhomMau, x => x.TheTichDisplay)
                .Take(model.Take)
                .ToListAsync();
            return query;
        }

        public Task<List<LookupItemVo>> GetListNhomMauAsync(DropDownListRequestModel model)
        {
            var lstEnums = EnumHelper.GetListEnum<Enums.EnumNhomMau>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(model.Query))
            {
                lstEnums = lstEnums.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                   .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            return Task.FromResult(lstEnums);
        }

        public Task<List<LookupItemVo>> GetListYeuToRhAsync(DropDownListRequestModel model)
        {
            var lstEnums = EnumHelper.GetListEnum<Enums.EnumYeuToRh>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(model.Query))
            {
                lstEnums = lstEnums.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                   .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            return Task.FromResult(lstEnums);
        }
        public Task<List<LookupItemVo>> GetListLoaiXetNghiemMauNhapThemAsync(DropDownListRequestModel model)
        {
            var lstEnums = EnumHelper.GetListEnum<Enums.LoaiXetNghiemMauNhapThem>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(model.Query))
            {
                lstEnums = lstEnums.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                   .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            return Task.FromResult(lstEnums);
        }
        #endregion

        #region kiểm tra data

        public async Task<bool> KiemTraTrungMaTuiMauAsync(string maTuiMau, long nhapKhoMauChiTietId, List<string> maTuiMauDangNhaps)
        {
            if (string.IsNullOrEmpty(maTuiMau))
            {
                return false;
            }
            else
            {
                maTuiMau = maTuiMau.Trim().ToLower();
            }
            var isExists = await _nhapKhoMauChiTietRepository.TableNoTracking
                .Where(x => x.Id != nhapKhoMauChiTietId 
                            && x.NhapKhoMau.DuocKeToanDuyet != false
                            && x.MaTuiMau.Trim().ToLower() == maTuiMau)
                .AnyAsync();
            return isExists || maTuiMauDangNhaps.Any(x => x.Trim().ToLower() == maTuiMau);
        }

        public async Task<bool> KiemTraTrungYeuCauTruyenMauAsync(long? yeuCauTruyenMauId, long nhapKhoMauChiTietId, List<long> yeuCauTruyenMauIdDangChons)
        {
            if (yeuCauTruyenMauId == null)
            {
                return false;
            }

            var isExists = await _nhapKhoMauChiTietRepository.TableNoTracking
                .Where(x => x.Id != nhapKhoMauChiTietId
                            && x.NhapKhoMau.DuocKeToanDuyet != false
                            && x.YeuCauTruyenMauId == yeuCauTruyenMauId)
                .AnyAsync();
            return isExists || yeuCauTruyenMauIdDangChons.Any(x => x == yeuCauTruyenMauId);
        }

        public async Task<bool> KiemTraCapNhatGiaNhapKhoMauChiTietAsync(decimal? giaDV, decimal? giaBH, long nhapKhoMauChiTietId)
        {
            if (giaDV == null && giaBH == null)
            {
                return true;
            }

            var phieuNhapChiTiet = await _nhapKhoMauChiTietRepository.TableNoTracking
                .Where(x => x.Id == nhapKhoMauChiTietId
                            && x.NhapKhoMau.DuocKeToanDuyet == true
                            && x.YeuCauTruyenMau.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                .FirstOrDefaultAsync();
            if (phieuNhapChiTiet == null)
            {
                return true;
            }

            return phieuNhapChiTiet.DonGiaNhap == giaDV && phieuNhapChiTiet.DonGiaBaoHiem == giaBH;
        }
        #endregion

        #region get data

        public async Task<Core.Domain.Entities.MauVaChePhams.NhapKhoMau> GetPhieuNhapKhoMauAsync(long id)
        {
            var phieuNhap = await BaseRepository.TableNoTracking
                .Include(x => x.NhapKhoMauChiTiets).ThenInclude(y => y.MauVaChePham)
                .Include(x => x.NhapKhoMauChiTiets).ThenInclude(y => y.YeuCauTruyenMau).ThenInclude(z => z.YeuCauTiepNhan).ThenInclude(t => t.BenhNhan)
                .Include(x => x.NhapKhoMauChiTiets).ThenInclude(y => y.YeuCauTruyenMau).ThenInclude(z => z.YeuCauTiepNhan).ThenInclude(t => t.NoiTruBenhAn)
                .Include(x => x.NhaThau)
                .Include(x => x.NguoiGiao).ThenInclude(y => y.User)                
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            return phieuNhap;
        }


        #endregion

        #region In phiếu

        public async Task<string> XuLyInPhieuTruyenMauAsync(long phieuTruyenMauId)
        {
            var content = string.Empty;
            var hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                      "<th>PHIẾU TRUYỀN MÁU</th>" +
                      "</p>";

            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuTruyenMau"));
            var data = await _nhapKhoMauChiTietRepository.TableNoTracking
                .Where(x => x.NhapKhoMauId == phieuTruyenMauId)
                .Select(item => new PhieuTruyenMauVo()
                {
                    HeaderPhieuTruyenMau = hearder, // tạm thời k cần
                    SoVaoVien = item.YeuCauTruyenMau.YeuCauTiepNhan.MaYeuCauTiepNhan,

                    BenhNhanHoTen = item.YeuCauTruyenMau.YeuCauTiepNhan.HoTen,
                    BenhNhanMaSo = item.YeuCauTruyenMau.YeuCauTiepNhan.BenhNhan.MaBN,
                    NgaySinh = item.YeuCauTruyenMau.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTruyenMau.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTruyenMau.YeuCauTiepNhan.NamSinh,
                    BenhNhanGioiTinh = item.YeuCauTruyenMau.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    BenhNhanNhomMau = item.YeuCauTruyenMau.NhomMau != null ? "\"" + item.YeuCauTruyenMau.NhomMau.GetDescription() + "\"" : "",
                    BenhNhanYeuToRh = item.YeuCauTruyenMau.YeuToRh != null ? "("+ item.YeuCauTruyenMau.YeuToRh.GetDescription() + ")" : "", //item.YeuCauTruyenMau.YeuToRh == Enums.EnumYeuToRh.Amtinh ? "-" : "+",
                    ChanDoan = string.IsNullOrEmpty(item.YeuCauTruyenMau.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu) ? item.YeuCauTruyenMau.NoiTruPhieuDieuTri.ChanDoanChinhICD.TenTiengViet : item.YeuCauTruyenMau.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu,
                    KhoaPhong = item.YeuCauTruyenMau.NoiTruPhieuDieuTri.KhoaPhongDieuTri.Ten,
                    Giuong = item.YeuCauTruyenMau.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan) ? 
                        item.YeuCauTruyenMau.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.OrderByDescending(a => a.CreatedOn).FirstOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.Ten : "",

                    ChePhamMau = item.MauVaChePham.Ten,
                    SoLuong = item.TheTich.ToString(),
                    MaSoDonVi = item.MaTuiMau,
                    ChePhamMauNhomMau = item.NhomMau != null ? "\"" + item.NhomMau.GetDescription() + "\"" : "",//item.NhomMau.GetDescription(),
                    ChePhamMauYeuToRh = item.YeuToRh != null ? "(" + item.YeuToRh.GetDescription() + ")" : "",//item.YeuToRh == Enums.EnumYeuToRh.Amtinh ? "-" : "+",
                    NgayNhanMau = item.NgaySanXuat.ApplyFormatDate(),
                    HanSuDung = item.HanSuDung.ApplyFormatDate(),
                    NgayXetNghiemHoaHop = item.NgayLamXetNghiemHoaHop != null ? item.NgayLamXetNghiemHoaHop.Value.ApplyFormatDate() : null,
                    NoiXetNghiemHoaHop = item.NhapKhoMau.NhaThau.Ten,

                    //IsKetQuaMoiTruongMuoiAmTinh = item.KetQuaXetNghiemMoiTruongMuoi == Enums.EnumKetQuaXetNghiem.AmTinh,
                    //IsKetQua37oCKhangGlobulinAmTinh = item.KetQuaXetNghiem37oCKhangGlubulin == Enums.EnumKetQuaXetNghiem.AmTinh,
                    KetQuaXetNghiemKhacs = !string.IsNullOrEmpty(item.KetQuaXetNghiemKhac) ? JsonConvert.DeserializeObject<List<KetQuaXetNghiemKhacVo>>(item.KetQuaXetNghiemKhac) : new List<KetQuaXetNghiemKhacVo>(),

                    DiaDiem = "",
                    HoTenNguoiPhatMau = "",
                    NguoiLamXetNghiemHoaHop = item.NguoiLamXetNghiemHoaHop,
                    Gio = DateTime.Now.Hour.ToString(),
                    Phut = DateTime.Now.Minute.ToString(),
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).ToListAsync();

            //content +=
            //    "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:A4 portrait;} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
            //content += "</head><body>";
            foreach (var phieuChiTiet in data)
            {
                content += TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, phieuChiTiet) + "<div class='pagebreak'></div>";
            }
            //content += "</body></html>";

            return content;
        }

        #endregion

        #region Duyệt
        public void XuLyXuatKhoMau(Core.Domain.Entities.MauVaChePhams.NhapKhoMau phieuNhapKhoMau)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();

            phieuNhapKhoMau.NhanVienDuyetId = currentUserId;
            phieuNhapKhoMau.NgayDuyet = DateTime.Now;
            
            var newXuatKhoMau = new XuatKhoMau()
            {
                NguoiXuatId = currentUserId,
                NguoiNhanId = phieuNhapKhoMau.NguoiNhapId,
                NgayXuat = DateTime.Now
            };

            foreach (var phieuNhapChiTiet in phieuNhapKhoMau.NhapKhoMauChiTiets)
            {
                var newXuatChiTiet = new XuatKhoMauChiTiet()
                {
                    NhapKhoMauChiTietId = phieuNhapChiTiet.Id,
                    XuatKhoMau = newXuatKhoMau
                };
                phieuNhapChiTiet.YeuCauTruyenMau.XuatKhoMauChiTiet = newXuatChiTiet;
            }
        }


        #endregion

        #region update BVHD-3320: bỏ kế toán nhập giá máu

        public async Task XuLyTaoPhieuNhapKhoMauAsync(Core.Domain.Entities.MauVaChePhams.NhapKhoMau nhapKhoMau)
        {
            var nhanVienKeToanDuyetTuDongId = (long)Enums.NhanVienHeThong.NhanVienKeToanDuyetTuDong;

            nhapKhoMau.NhanVienDuyetId = nhanVienKeToanDuyetTuDongId;
            nhapKhoMau.NgayDuyet = DateTime.Now;
            nhapKhoMau.DuocKeToanDuyet = true;

            var newXuatKhoMau = new XuatKhoMau()
            {
                NguoiXuatId = nhanVienKeToanDuyetTuDongId,
                NguoiNhanId = nhapKhoMau.NguoiNhapId,
                NgayXuat = DateTime.Now
            };

            foreach (var phieuNhapChiTiet in nhapKhoMau.NhapKhoMauChiTiets)
            {
                var yeuCauTruyenMau = _yeuCauTruyenMauRepository.GetById(phieuNhapChiTiet.YeuCauTruyenMauId);
                phieuNhapChiTiet.DonGiaNhap = yeuCauTruyenMau.DonGiaNhap;
                phieuNhapChiTiet.DonGiaBan = yeuCauTruyenMau.DonGiaBan;
                phieuNhapChiTiet.DonGiaBaoHiem = yeuCauTruyenMau.DonGiaBaoHiem;
                var newXuatChiTiet = new XuatKhoMauChiTiet()
                {
                    NhapKhoMauChiTiet = phieuNhapChiTiet,
                    XuatKhoMau = newXuatKhoMau
                };
                yeuCauTruyenMau.XuatKhoMauChiTiet = newXuatChiTiet;
            }
            await BaseRepository.AddAsync(nhapKhoMau);
        }

        public async Task XuLyCapNhatPhieuNhapKhoMauAsync(Core.Domain.Entities.MauVaChePhams.NhapKhoMau nhapKhoMau)
        {
            nhapKhoMau.DuocKeToanDuyet = true;

            var yeuCauTruyenMauIds = nhapKhoMau.NhapKhoMauChiTiets.Where(o => o.Id != 0).Select(o => o.YeuCauTruyenMauId);
            var yeuCauTruyenMaus = _yeuCauTruyenMauRepository.Table.Where(o => yeuCauTruyenMauIds.Contains(o.Id)).Include(o => o.XuatKhoMauChiTiet).ToList();
            var yeuCauTiepNhanIds = yeuCauTruyenMaus.Select(o => o.YeuCauTiepNhanId).ToList();
            var phieuTheoDoiTruyenMaus = _noiTruHoSoKhacRepository.TableNoTracking.Where(o =>
                o.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenMau &&
                yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId));
            var checkCoPhieuTheoDoi = false;
            foreach (var phieuTheoDoiTruyenMau in phieuTheoDoiTruyenMaus)
            {
                var objectData = JsonConvert.DeserializeObject<InPhieuTheoDoiTruyenMau>(phieuTheoDoiTruyenMau.ThongTinHoSo);
                if (nhapKhoMau.NhapKhoMauChiTiets.Where(o => o.Id != 0).Select(o => o.Id).Contains(objectData.MaDonViMauTruyenId))
                {
                    checkCoPhieuTheoDoi = true;
                    break;
                }
            }
            if (checkCoPhieuTheoDoi)
                throw new Exception(_localizationService.GetResource("NhapKhoMau.DaCoPhieuTheoDoi"));

            if (nhapKhoMau.NhapKhoMauChiTiets.Any(o => o.WillDelete))
            {
                foreach (var nhapKhoMauChiTiet in nhapKhoMau.NhapKhoMauChiTiets.Where(o => o.WillDelete))
                {
                    var yeuCauTruyenMau = yeuCauTruyenMaus.First(o => o.Id == nhapKhoMauChiTiet.YeuCauTruyenMauId);
                    if (yeuCauTruyenMau.XuatKhoMauChiTietId != null)
                    {
                        yeuCauTruyenMau.XuatKhoMauChiTietId = null;
                        yeuCauTruyenMau.XuatKhoMauChiTiet.WillDelete = true;
                    }
                    nhapKhoMauChiTiet.WillDelete = true;
                }
            }

            if (nhapKhoMau.NhapKhoMauChiTiets.Any(o => o.Id == 0))
            {
                var nhapKhoMauChiTietIds = nhapKhoMau.NhapKhoMauChiTiets.Select(o => o.Id);
                var xuatKhoMau = _xuatKhoMauRepository.Table.LastOrDefault(o => o.XuatKhoMauChiTiets.Any(ct => nhapKhoMauChiTietIds.Contains(ct.NhapKhoMauChiTietId)));
                foreach (var phieuNhapChiTiet in nhapKhoMau.NhapKhoMauChiTiets.Where(o => o.Id == 0))
                {
                    var yeuCauTruyenMau = _yeuCauTruyenMauRepository.GetById(phieuNhapChiTiet.YeuCauTruyenMauId);
                    phieuNhapChiTiet.DonGiaNhap = yeuCauTruyenMau.DonGiaNhap;
                    phieuNhapChiTiet.DonGiaBan = yeuCauTruyenMau.DonGiaBan;
                    phieuNhapChiTiet.DonGiaBaoHiem = yeuCauTruyenMau.DonGiaBaoHiem;
                    var newXuatChiTiet = new XuatKhoMauChiTiet()
                    {
                        NhapKhoMauChiTiet = phieuNhapChiTiet,
                        XuatKhoMau = xuatKhoMau
                    };
                    yeuCauTruyenMau.XuatKhoMauChiTiet = newXuatChiTiet;
                }
            }
            await BaseRepository.Context.SaveChangesAsync();
        }

        public async Task XuLyXoaPhieuNhapKhoMauAsync(long id)
        {
            var phieuNhapKhoMau = BaseRepository.GetById(id,
                x => x.Include(a => a.NhapKhoMauChiTiets).ThenInclude(o => o.XuatKhoMauChiTiets).ThenInclude(o => o.YeuCauTruyenMaus)
                    .Include(a => a.NhapKhoMauChiTiets).ThenInclude(o => o.XuatKhoMauChiTiets).ThenInclude(o => o.XuatKhoMau));
            var yeuCauTiepNhanIds = phieuNhapKhoMau.NhapKhoMauChiTiets.SelectMany(o => o.XuatKhoMauChiTiets)
                .SelectMany(o => o.YeuCauTruyenMaus).Select(o => o.YeuCauTiepNhanId).ToList();
            var phieuTheoDoiTruyenMaus = _noiTruHoSoKhacRepository.TableNoTracking.Where(o =>
                o.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenMau &&
                yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId));

            var checkCoPhieuTheoDoi = false;
            foreach (var phieuTheoDoiTruyenMau in phieuTheoDoiTruyenMaus)
            {
                var objectData = JsonConvert.DeserializeObject<InPhieuTheoDoiTruyenMau>(phieuTheoDoiTruyenMau.ThongTinHoSo);
                if (phieuNhapKhoMau.NhapKhoMauChiTiets.Select(o => o.Id).Contains(objectData.MaDonViMauTruyenId))
                {
                    checkCoPhieuTheoDoi = true;
                    break;
                }
            }
            if(checkCoPhieuTheoDoi)
                throw new Exception(_localizationService.GetResource("NhapKhoMau.DaCoPhieuTheoDoi"));
            
            foreach (var nhapKhoMauChiTiet in phieuNhapKhoMau.NhapKhoMauChiTiets)
            {
                nhapKhoMauChiTiet.WillDelete = true;
                foreach (var xuatKhoMauChiTiet in nhapKhoMauChiTiet.XuatKhoMauChiTiets)
                {
                    xuatKhoMauChiTiet.WillDelete = true;
                    xuatKhoMauChiTiet.XuatKhoMau.WillDelete = true;
                    foreach (var yeuCauTruyenMau in xuatKhoMauChiTiet.YeuCauTruyenMaus)
                    {
                        yeuCauTruyenMau.XuatKhoMauChiTietId = null;
                    }
                }
            }

            phieuNhapKhoMau.WillDelete = true;
            await BaseRepository.Context.SaveChangesAsync();
        }

        #endregion
    }
}
