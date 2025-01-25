using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Data;
using Camino.Services.CauHinh;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using System.Globalization;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.Localization;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.MayXetNghiems;

namespace Camino.Services.XuatKhoKhacs
{
    [ScopedDependency(ServiceType = typeof(IXuatKhoKhacService))]
    public class XuatKhoKhacService : MasterFileService<XuatKhoDuocPham>, IXuatKhoKhacService
    {
        private readonly IRepository<XuatKhoDuocPhamChiTiet> _xuatKhoDuocPhamChiTietRepository;
        private readonly IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        private readonly IRepository<Kho> _khoDuocPhamRepository;
        private readonly IRepository<KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<DuocPham> _duocPhamRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.NhaThaus.NhaThau> _nhaThauRepository;
        private readonly IRepository<NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhom;
        private readonly IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Template> _templateRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<YeuCauXuatKhoDuocPham> _yeuCauXuatKhoDuocPhamRepository;
        private readonly IRepository<YeuCauXuatKhoDuocPhamChiTiet> _yeuCauXuatKhoDuocPhamChiTietRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<YeuCauNhapKhoVatTuChiTiet> _yeuCauNhapKhoVatTuChiTietRepository;
        private readonly IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;
        private readonly IRepository<YeuCauXuatKhoVatTu> _yeuCauXuatKhoVatTuRepository;
        private readonly IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;
        private readonly IRepository<YeuCauXuatKhoVatTuChiTiet> _yeuCauXuatKhoVatTuChiTietRepository;
        private readonly IRepository<DuocPhamBenhVienMayXetNghiem> _duocPhamBenhVienMayXetNghiemRepository;

        public XuatKhoKhacService(IRepository<XuatKhoDuocPham> repository, IRepository<XuatKhoDuocPhamChiTiet> xuatKhoDuocPhamChiTietRepository
            , IRepository<Kho> khoDuocPhamRepository
            , IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository
            , IRepository<DuocPham> duocPhamRepository
            , IRepository<Template> templateRepository
            , IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository
            , IRepository<NhapKhoDuocPham> nhapKhoDuocPhamRepository
            , IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhom
            , IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository
            , ICauHinhService cauHinhService, IUserAgentHelper userAgentHelper
            , IRepository<YeuCauXuatKhoDuocPham> yeuCauXuatKhoDuocPhamRepository
            , IRepository<YeuCauXuatKhoDuocPhamChiTiet> yeuCauXuatKhoDuocPhamChiTietRepository
            , ILocalizationService localizationService
            , IRepository<LocaleStringResource> localeStringResourceRepository
            , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
            , IRepository<Core.Domain.Entities.NhaThaus.NhaThau> nhaThauRepository
            , IRepository<KhoNhanVienQuanLy> khoNhanVienQuanLyRepository
            , IRepository<YeuCauNhapKhoDuocPhamChiTiet> yeuCauNhapKhoDuocPhamChiTietRepository
            , IRepository<YeuCauNhapKhoVatTuChiTiet> yeuCauNhapKhoVatTuChiTietRepository
            , IRepository<XuatKhoVatTu> xuatKhoVatTuRepository
            , IRepository<YeuCauXuatKhoVatTu> yeuCauXuatKhoVatTuRepository
            , IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository
            , IRepository<YeuCauXuatKhoVatTuChiTiet> yeuCauXuatKhoVatTuChiTietRepository
            , IRepository<DuocPhamBenhVienMayXetNghiem> duocPhamBenhVienMayXetNghiemRepository
            , IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository) : base(repository)
        {
            _xuatKhoDuocPhamChiTietRepository = xuatKhoDuocPhamChiTietRepository;
            _khoDuocPhamRepository = khoDuocPhamRepository;
            _nhanVienRepository = nhanVienRepository;
            _duocPhamRepository = duocPhamRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _duocPhamBenhVienPhanNhom = duocPhamBenhVienPhanNhom;
            _templateRepository = templateRepository;
            _cauHinhService = cauHinhService;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
            _userAgentHelper = userAgentHelper;
            _yeuCauXuatKhoDuocPhamRepository = yeuCauXuatKhoDuocPhamRepository;
            _yeuCauXuatKhoDuocPhamChiTietRepository = yeuCauXuatKhoDuocPhamChiTietRepository;
            _localizationService = localizationService;
            _localeStringResourceRepository = localeStringResourceRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _nhaThauRepository = nhaThauRepository;
            _yeuCauNhapKhoDuocPhamChiTietRepository = yeuCauNhapKhoDuocPhamChiTietRepository;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _yeuCauXuatKhoVatTuRepository = yeuCauXuatKhoVatTuRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _yeuCauXuatKhoVatTuChiTietRepository = yeuCauXuatKhoVatTuChiTietRepository;
            _yeuCauNhapKhoVatTuChiTietRepository = yeuCauNhapKhoVatTuChiTietRepository;
            _duocPhamBenhVienMayXetNghiemRepository = duocPhamBenhVienMayXetNghiemRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                 .Where(p => p.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatKhac && (p.KhoDuocPhamXuat.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoDuocPhamXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2))
                .Select(s => new XuatKhoDuocPhamKhacGridVo
                {
                    Id = s.Id,
                    KhoDuocPhamXuat = s.KhoDuocPhamXuat.Ten,
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    TenNguoiNhan = s.NguoiNhan.User.HoTen,
                    TenNguoiXuat = s.NguoiXuat.User.HoTen,
                    NgayXuat = s.NgayXuat,
                    DuocDuyet = true,
                    CoNCC = s.TraNCC
                }).Union(
                _yeuCauXuatKhoDuocPhamRepository
                .TableNoTracking
                 .Where(p => p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
                .Select(z => new XuatKhoDuocPhamKhacGridVo
                {
                    Id = z.Id,
                    KhoDuocPhamXuat = z.KhoXuat.Ten,
                    LyDoXuatKho = z.LyDoXuatKho,
                    TenNguoiNhan = z.NguoiNhan.User.HoTen,
                    TenNguoiXuat = z.NguoiXuat.User.HoTen,
                    NgayXuat = z.NgayXuat,
                    DuocDuyet = null,
                    CoNCC = z.TraNCC
                }));

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<XuatKhoDuocPhamKhacGridVo>(queryInfo.AdditionalSearchString);

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
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayXuat <= denNgay));
                }

                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.KhoDuocPhamXuat,
                         g => g.TenNguoiXuat,
                         g => g.TenNguoiNhan,
                         g => g.LyDoXuatKho
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
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                 .Where(p => p.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatKhac && (p.KhoDuocPhamXuat.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoDuocPhamXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2))
                .Select(s => new XuatKhoDuocPhamKhacGridVo
                {
                    Id = s.Id,
                    KhoDuocPhamXuat = s.KhoDuocPhamXuat.Ten,
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    TenNguoiNhan = s.NguoiNhan.User.HoTen,
                    TenNguoiXuat = s.NguoiXuat.User.HoTen,
                    NgayXuat = s.NgayXuat,
                    DuocDuyet = true
                }).Union(
                _yeuCauXuatKhoDuocPhamRepository
                .TableNoTracking
                 .Where(p => p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
                .Select(z => new XuatKhoDuocPhamKhacGridVo
                {
                    Id = z.Id,
                    KhoDuocPhamXuat = z.KhoXuat.Ten,
                    LyDoXuatKho = z.LyDoXuatKho,
                    TenNguoiNhan = z.NguoiNhan.User.HoTen,
                    TenNguoiXuat = z.NguoiXuat.User.HoTen,
                    NgayXuat = z.NgayXuat,
                    DuocDuyet = null
                }));

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<XuatKhoDuocPhamKhacGridVo>(queryInfo.AdditionalSearchString);

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
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayXuat <= denNgay));
                }

                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.KhoDuocPhamXuat,
                         g => g.TenNguoiXuat,
                         g => g.TenNguoiNhan,
                         g => g.LyDoXuatKho
                   );
                }
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var xuatKhoDuocPhamId = long.Parse(queryObj[0]);
            var tinhTrang = long.Parse(queryObj[1]);
            IQueryable<YeuCauXuatKhoDuocPhamGridVo> query;
            if (tinhTrang == 0)
            {
                query = _yeuCauXuatKhoDuocPhamChiTietRepository.TableNoTracking
               .Where(x => x.YeuCauXuatKhoDuocPhamId == xuatKhoDuocPhamId)
               .Select(s => new YeuCauXuatKhoDuocPhamGridVo()
               {
                   Id = s.Id,
                   Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                   DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                   TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.DuocPhamBenhVien.Ma,
                   SoDangKy = s.DuocPhamBenhVien.DuocPham.SoDangKy,
                   HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                   SoLo = s.Solo,
                   SoLuongXuat = s.SoLuongXuat,
                   HanSuDung = s.HanSuDung,
               });
            }
            else
            {
                query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
               .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == xuatKhoDuocPhamId)
               .Select(s => new YeuCauXuatKhoDuocPhamGridVo()
               {
                   Id = s.Id,
                   Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                   DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                   TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                   SoDangKy = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.SoDangKy,
                   HamLuong = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                   SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
                   HanSuDung = s.NhapKhoDuocPhamChiTiet.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,
                   SoPhieu = s.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
               });
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var xuatKhoDuocPhamId = long.Parse(queryObj[0]);
            var tinhTrang = long.Parse(queryObj[1]);
            IQueryable<YeuCauXuatKhoDuocPhamGridVo> query;
            if (tinhTrang == 0)
            {
                query = _yeuCauXuatKhoDuocPhamChiTietRepository.TableNoTracking
               .Where(x => x.YeuCauXuatKhoDuocPhamId == xuatKhoDuocPhamId)
               .Select(s => new YeuCauXuatKhoDuocPhamGridVo()
               {
                   Id = s.Id,
                   Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                   DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                   TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.DuocPhamBenhVien.Ma,
                   SoDangKy = s.DuocPhamBenhVien.DuocPham.SoDangKy,
                   HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                   SoLo = s.Solo,
                   HanSuDung = s.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,

               });
            }
            else
            {
                query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
               .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == xuatKhoDuocPhamId)
               .Select(s => new YeuCauXuatKhoDuocPhamGridVo()
               {
                   Id = s.Id,
                   Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                   DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                   TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                   SoDangKy = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.SoDangKy,
                   HamLuong = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                   SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
                   HanSuDung = s.NhapKhoDuocPhamChiTiet.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,
                   SoPhieu = s.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu
               });
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsyncDaDuyet(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<YeuCauXuatKhoDuocPhamGridVo>(queryInfo.AdditionalSearchString);
            var query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
           .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == queryString.XuatKhoDuocPhamId)
           .Select(s => new YeuCauXuatKhoDuocPhamGridVo()
           {
               Id = s.Id,
               Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
               DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
               TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
               Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
               SoDangKy = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.SoDangKy,
               HamLuong = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
               SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
               HanSuDung = s.NhapKhoDuocPhamChiTiet.HanSuDung,
               SoLuongXuat = s.SoLuongXuat,
               SoPhieu = s.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu

           });
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.DVT,
                     g => g.Ten,
                     g => g.Ma,
                     g => g.SoDangKy,
                     g => g.SoLo,
                     g => g.HamLuong,
                     g => g.SoPhieu
               );
            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyet(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<YeuCauXuatKhoDuocPhamGridVo>(queryInfo.AdditionalSearchString);
            var query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
           .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == queryString.XuatKhoDuocPhamId)
           .Select(s => new YeuCauXuatKhoDuocPhamGridVo()
           {
               Id = s.Id,
               Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
               DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
               TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
               Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
               SoDangKy = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.SoDangKy,
               HamLuong = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
               SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
               HanSuDung = s.NhapKhoDuocPhamChiTiet.HanSuDung,
               SoLuongXuat = s.SoLuongXuat,
               SoPhieu = s.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu

           });
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.DVT,
                     g => g.Ten,
                     g => g.Ma,
                     g => g.SoDangKy,
                     g => g.SoLo,
                     g => g.HamLuong,
                     g => g.SoPhieu
               );
            }
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
            var info = JsonConvert.DeserializeObject<YeuCauXuatKhoDuocPhamChiTietVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dp => dp.DonViTinh)
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                    .Include(nkct => nkct.NhapKhoDuocPhams)
                    .Include(nkct => nkct.HopDongThauDuocPhams)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoDuocPhams.KhoDuocPhams.Id == info.KhoXuatId);
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
            var yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats = new List<YeuCauXuatKhoDuocPhamGridVo>();

            if (info.NhaThauId != null && !string.IsNullOrEmpty(info.SoChungTu))
            {
                var yeuCauNhapKhoDuocPhamChiTiets = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(z => z.YeuCauNhapKhoDuocPham.SoChungTu.Equals(info.SoChungTu)).ToList();
                var nhapKhoDPCTs = new List<NhapKhoDuocPhamChiTiet>();
                foreach (var item in yeuCauNhapKhoDuocPhamChiTiets)
                {
                    foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                    {
                        if (item.DuocPhamBenhVienId == nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId
                         && item.LaDuocPhamBHYT == nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT
                         && item.HopDongThauDuocPhamId == nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId
                         && item.Solo == nhapKhoDuocPhamChiTiet.Solo
                         && item.HanSuDung == nhapKhoDuocPhamChiTiet.HanSuDung
                         && item.TiLeTheoThapGia == nhapKhoDuocPhamChiTiet.TiLeTheoThapGia
                         && item.VAT == nhapKhoDuocPhamChiTiet.VAT
                         && item.MaVach == nhapKhoDuocPhamChiTiet.MaVach
                         && item.MaRef == nhapKhoDuocPhamChiTiet.MaRef)
                        {
                            nhapKhoDPCTs.Add(nhapKhoDuocPhamChiTiet);
                        }
                    }
                }

                var nhapKhoDuocPhamChiTietGroup = nhapKhoDPCTs.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhViens.Ma, x.DuocPhamBenhViens.DuocPham.Ten, x.DuocPhamBenhViens.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten })
                                               .Select(g => new { nhapKhoDuocPhamChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoDuocPhamChiTietGroup)
                {

                    var yeuCauXuatKhoDuocPhamGridVo = new YeuCauXuatKhoDuocPhamGridVo
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
                        DonGiaNhap = item.nhapKhoDuocPhamChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.KhoId,
                        NhaThauId = item.nhapKhoDuocPhamChiTiets.HopDongThauDuocPhams.NhaThauId,
                        SoChungTu = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.SoChungTu
                    };
                    yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Add(yeuCauXuatKhoDuocPhamGridVo);
                }

            }
            else
            {
                var nhapKhoDuocPhamChiTietGroup = nhapKhoDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhViens.Ma, x.DuocPhamBenhViens.DuocPham.Ten, x.DuocPhamBenhViens.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten })
                                               .Select(g => new { nhapKhoDuocPhamChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoDuocPhamChiTietGroup)
                {

                    var yeuCauXuatKhoDuocPhamGridVo = new YeuCauXuatKhoDuocPhamGridVo
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
                        DonGiaNhap = item.nhapKhoDuocPhamChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.KhoId,
                        NhaThauId = item.nhapKhoDuocPhamChiTiets.HopDongThauDuocPhams.NhaThauId,
                        SoChungTu = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.SoChungTu
                    };
                    yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Add(yeuCauXuatKhoDuocPhamGridVo);
                }
            }

            var duocPhamBenhViens = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == info.KhoXuatId
                         && x.SoLuongDaXuat < x.SoLuongNhap
                         //&& x.HanSuDung >= DateTime.Now
                         ).ToList();

            var result = yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Select(o =>
            {
                o.SoLuongTon = duocPhamBenhViens.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaDuocPhamBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                o.SoLuongXuat = duocPhamBenhViens.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaDuocPhamBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            });
            if (info.DuocPhamBenhViens.Any())
            {
                result = result.Where(x => !info.DuocPhamBenhViens.Any(z => z.DuocPhamBenhVienId == x.DuocPhamBenhVienId && z.LaDuocPhamBHYT == x.LaDuocPhamBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung));
            }


            var query = result.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            var duocPhamBVMayXetNghiems = _duocPhamBenhVienMayXetNghiemRepository.TableNoTracking.Select(c => new { c.MayXetNghiemId, c.DuocPhamBenhVienId, c.MayXetNghiem.Ten }).OrderBy(c => c.Ten).ToList();
            foreach (var item in queryTask)
            {
                var duocPhamMayXetNghiem = duocPhamBVMayXetNghiems.Where(c => c.DuocPhamBenhVienId == item.DuocPhamBenhVienId).FirstOrDefault();
                item.XetNghiemIdDauTienMayXetNghiem = duocPhamMayXetNghiem?.MayXetNghiemId;
                item.TenXetNghiemDauTienMayXetNghiem = duocPhamMayXetNghiem?.Ten;
            }

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamDaChon(QueryInfo queryInfo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauXuatKhoDuocPhamChiTietVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dp => dp.DonViTinh)
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                    .Include(nkct => nkct.NhapKhoDuocPhams)
                    .Include(nkct => nkct.HopDongThauDuocPhams)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoDuocPhams.KhoDuocPhams.Id == info.KhoXuatId)
                    ;

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
            var yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats = new List<YeuCauXuatKhoDuocPhamGridVo>();

            if (info.NhaThauId != null && !string.IsNullOrEmpty(info.SoChungTu))
            {
                var yeuCauNhapKhoDuocPhamChiTiets = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(z => z.YeuCauNhapKhoDuocPham.SoChungTu.Equals(info.SoChungTu)).ToList();
                var nhapKhoDPCTs = new List<NhapKhoDuocPhamChiTiet>();
                foreach (var item in yeuCauNhapKhoDuocPhamChiTiets)
                {
                    foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                    {
                        if (item.DuocPhamBenhVienId == nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId
                         && item.LaDuocPhamBHYT == nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT
                         && item.HopDongThauDuocPhamId == nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId
                         && item.Solo == nhapKhoDuocPhamChiTiet.Solo
                         && item.HanSuDung == nhapKhoDuocPhamChiTiet.HanSuDung
                         && item.TiLeTheoThapGia == nhapKhoDuocPhamChiTiet.TiLeTheoThapGia
                         && item.VAT == nhapKhoDuocPhamChiTiet.VAT
                         && item.MaVach == nhapKhoDuocPhamChiTiet.MaVach
                         && item.MaRef == nhapKhoDuocPhamChiTiet.MaRef)
                        {
                            nhapKhoDPCTs.Add(nhapKhoDuocPhamChiTiet);
                        }
                    }
                }

                var nhapKhoDuocPhamChiTietGroup = nhapKhoDPCTs.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhViens.Ma, x.DuocPhamBenhViens.DuocPham.Ten, x.DuocPhamBenhViens.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten })
                                               .Select(g => new { nhapKhoDuocPhamChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoDuocPhamChiTietGroup)
                {
                    var yeuCauXuatKhoDuocPhamGridVo = new YeuCauXuatKhoDuocPhamGridVo
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
                        DonGiaNhap = item.nhapKhoDuocPhamChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.KhoId,
                        NhaThauId = item.nhapKhoDuocPhamChiTiets.HopDongThauDuocPhams.NhaThauId,
                        SoChungTu = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.SoChungTu
                    };
                    yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Add(yeuCauXuatKhoDuocPhamGridVo);
                }

            }
            else
            {
                var nhapKhoDuocPhamChiTietGroup = nhapKhoDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DuocPhamBenhViens.Ma, x.DuocPhamBenhViens.DuocPham.Ten, x.DuocPhamBenhViens.DuocPham.HamLuong, x.Solo, x.HanSuDung, DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten })
                                               .Select(g => new { nhapKhoDuocPhamChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoDuocPhamChiTietGroup)
                {
                    var yeuCauXuatKhoDuocPhamGridVo = new YeuCauXuatKhoDuocPhamGridVo
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
                        DonGiaNhap = item.nhapKhoDuocPhamChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.KhoId,
                        NhaThauId = item.nhapKhoDuocPhamChiTiets.HopDongThauDuocPhams.NhaThauId,
                        SoChungTu = item.nhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.SoChungTu
                    };
                    yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Add(yeuCauXuatKhoDuocPhamGridVo);
                }
            }
            if (info.DuocPhamBenhViens.Any())
            {
                yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats = yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.Where(x => !info.DuocPhamBenhViens.Any(z => z.DuocPhamBenhVienId == x.DuocPhamBenhVienId && z.LaDuocPhamBHYT == x.LaDuocPhamBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung)).ToList();
            }

            var query = yeuCauDieuChuyenDuocPhamChiTietTheoKhoXuats.AsQueryable();
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<List<YeuCauXuatKhoDuocPhamGridVo>> YeuCauXuatDuocPhamChiTiets(long yeuCauXuatKhoDuocPhamId)
        {
            var query = _yeuCauXuatKhoDuocPhamChiTietRepository.TableNoTracking.Where(z => z.YeuCauXuatKhoDuocPhamId == yeuCauXuatKhoDuocPhamId)
                .Select(s => new YeuCauXuatKhoDuocPhamGridVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                    TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = s.DuocPhamBenhVien.Ma,
                    SoDangKy = s.DuocPhamBenhVien.DuocPham.SoDangKy,
                    HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    DonGiaNhap = s.DonGiaNhap,
                    KhoXuatId = s.YeuCauXuatKhoDuocPham.KhoXuatId,
                    SoLuongXuat = s.SoLuongXuat,

                    XetNghiemIdDauTienMayXetNghiem = s.MayXetNghiem.Id,
                    TenXetNghiemDauTienMayXetNghiem = s.MayXetNghiem.Ten
                })
                .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.Ten, x.HamLuong, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new YeuCauXuatKhoDuocPhamGridVo
                {
                    Id = g.First().Id,
                    DuocPhamBenhVienId = g.First().DuocPhamBenhVienId,
                    Ten = g.First().Ten,
                    DVT = g.First().DVT,
                    LaDuocPhamBHYT = g.First().LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = g.First().DuocPhamBenhVienPhanNhomId,
                    TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                    Ma = g.First().Ma,
                    SoDangKy = g.First().SoDangKy,
                    HamLuong = g.First().HamLuong,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    KhoXuatId = g.First().KhoXuatId,
                    SoLuongXuat = g.Sum(z => z.SoLuongXuat),
                    XetNghiemIdDauTienMayXetNghiem = g.First().XetNghiemIdDauTienMayXetNghiem,
                    TenXetNghiemDauTienMayXetNghiem = g.First().TenXetNghiemDauTienMayXetNghiem,
                });

            var yeuCauXuatPhamBuGridParentVos = query.ToList();

            var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => yeuCauXuatPhamBuGridParentVos.Any(z => z.DuocPhamBenhVienId == x.DuocPhamBenhVienId && z.LaDuocPhamBHYT == x.LaDuocPhamBHYT && x.NhapKhoDuocPhams.KhoId == z.KhoXuatId && x.HanSuDung == z.HanSuDung && x.Solo == z.SoLo) && x.SoLuongNhap > x.SoLuongDaXuat
            //&& x.HanSuDung >= DateTime.Now
            ).ToList();

            var result = yeuCauXuatPhamBuGridParentVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaDuocPhamBHYT));
            result = result.Select(o =>
            {
                o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaDuocPhamBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            });
            return result.ToList();
        }

        public async Task XuLyThemYeuCauXuatKhoKSNKAsync(YeuCauXuatKhoDuocPham yeuCauXuatKhoDuocPham, List<XuatKhoKhacDuocPhamChiTietVo> yeuCauXuatDuocPhamChiTiets, bool laLuuDuyet)
        {
            if (!laLuuDuyet) // chỉ lưu
            {
                if (yeuCauXuatDuocPhamChiTiets.Any())
                {
                    var duocPhamBenhVienIds = yeuCauXuatDuocPhamChiTiets.Select(o => o.DuocPhamBenhVienId).ToList();
                    var soLos = yeuCauXuatDuocPhamChiTiets.Select(o => o.SoLo).ToList();

                    var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauXuatKhoDuocPham.KhoXuatId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();

                    foreach (var chiTietVo in yeuCauXuatDuocPhamChiTiets)
                    {
                        var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                        .Where(o => o.DuocPhamBenhVienId == chiTietVo.DuocPhamBenhVienId && o.LaDuocPhamBHYT == chiTietVo.LaDuocPhamBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (nhapKhoDuocPhamChiTietXuats.Count() == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat))
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        var nhapKhoDuocPhamChiTietXuat = nhapKhoDuocPhamChiTietXuats.First();
                        var yeuCauXuatKhoDuocPhamChiTietNew = new YeuCauXuatKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = nhapKhoDuocPhamChiTietXuat.DuocPhamBenhVienId,
                            HopDongThauDuocPhamId = nhapKhoDuocPhamChiTietXuat.HopDongThauDuocPhamId,
                            LaDuocPhamBHYT = nhapKhoDuocPhamChiTietXuat.LaDuocPhamBHYT,
                            Solo = nhapKhoDuocPhamChiTietXuat.Solo,
                            HanSuDung = nhapKhoDuocPhamChiTietXuat.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoDuocPhamChiTietXuat.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoDuocPhamChiTietXuat.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoDuocPhamChiTietXuat.TiLeTheoThapGia,
                            VAT = nhapKhoDuocPhamChiTietXuat.VAT,
                            MaVach = nhapKhoDuocPhamChiTietXuat.MaVach,
                            TiLeBHYTThanhToan = nhapKhoDuocPhamChiTietXuat.TiLeBHYTThanhToan,
                            MaRef = nhapKhoDuocPhamChiTietXuat.MaRef,
                            SoLuongXuat = chiTietVo.SoLuongXuat.Value,
                            MayXetNghiemId = chiTietVo.MayXetNghiemId
                        };
                        yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets.Add(yeuCauXuatKhoDuocPhamChiTietNew);
                    }
                    _yeuCauXuatKhoDuocPhamRepository.Add(yeuCauXuatKhoDuocPham);
                }
            }
            else
            {
                XuatKhoDuocPham xuatKhoDuocPham = null;
                if (yeuCauXuatDuocPhamChiTiets.Any())
                {
                    var duocPhamBenhVienIds = yeuCauXuatDuocPhamChiTiets.Select(o => o.DuocPhamBenhVienId).ToList();
                    var soLos = yeuCauXuatDuocPhamChiTiets.Select(o => o.SoLo).ToList();

                    var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.Table
                        .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauXuatKhoDuocPham.KhoXuatId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();
                    //xuat kho
                    xuatKhoDuocPham = new XuatKhoDuocPham
                    {
                        LoaiXuatKho = Enums.XuatKhoDuocPham.XuatKhac,
                        KhoXuatId = yeuCauXuatKhoDuocPham.KhoXuatId,
                        LyDoXuatKho = yeuCauXuatKhoDuocPham.LyDoXuatKho,
                        NguoiXuatId = yeuCauXuatKhoDuocPham.NguoiXuatId,
                        NguoiNhanId = yeuCauXuatKhoDuocPham.NguoiNhanId,
                        LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                        NgayXuat = yeuCauXuatKhoDuocPham.NgayXuat,
                        TraNCC = yeuCauXuatKhoDuocPham.TraNCC,
                        NhaThauId = yeuCauXuatKhoDuocPham.NhaThauId,
                        SoChungTu = yeuCauXuatKhoDuocPham.SoChungTu
                    };


                    foreach (var chiTietVo in yeuCauXuatDuocPhamChiTiets)
                    {
                        var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                            .Where(o => o.DuocPhamBenhVienId == chiTietVo.DuocPhamBenhVienId && o.LaDuocPhamBHYT == chiTietVo.LaDuocPhamBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat)
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        double soLuongCanXuat = chiTietVo.SoLuongXuat.Value;
                        while (!soLuongCanXuat.AlmostEqual(0))
                        {
                            // tinh so luong xuat
                            var nhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTietXuats
                                .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                            var soLuongTon = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                            var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                            nhapKhoDuocPhamChiTiet.SoLuongDaXuat = (nhapKhoDuocPhamChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                            var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                            {
                                SoLuongXuat = soLuongXuat,
                                NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                                NgayXuat = yeuCauXuatKhoDuocPham.NgayXuat,
                                MayXetNghiemId = chiTietVo.MayXetNghiemId
                            };
                            var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                            {
                                DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                                NgayXuat = yeuCauXuatKhoDuocPham.NgayXuat
                            };
                            xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                            xuatKhoDuocPham.XuatKhoDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);

                            soLuongCanXuat = (soLuongCanXuat - soLuongXuat).MathRoundNumber(2);
                        }
                    }
                    BaseRepository.Add(xuatKhoDuocPham);
                }
            }
        }

        public async Task<CapNhatXuatKhoKhacResultVo> XuLyCapNhatYeuCauXuatKhoKSNKAsync(YeuCauXuatKhoDuocPham yeuCauXuatKhoDuocPham, List<XuatKhoKhacDuocPhamChiTietVo> yeuCauXuatDuocPhamChiTiets, bool laLuuDuyet)
        {
            foreach (var chiTiet in yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets)
            {
                if (chiTiet.Id != 0)
                {
                    chiTiet.WillDelete = true;
                }
            }

            if (!laLuuDuyet) // chỉ lưu
            {
                if (yeuCauXuatDuocPhamChiTiets.Any())
                {
                    var duocPhamBenhVienIds = yeuCauXuatDuocPhamChiTiets.Select(o => o.DuocPhamBenhVienId).ToList();
                    var soLos = yeuCauXuatDuocPhamChiTiets.Select(o => o.SoLo).ToList();

                    var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauXuatKhoDuocPham.KhoXuatId &&
                                    duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) &&
                                    o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();

                    foreach (var chiTietVo in yeuCauXuatDuocPhamChiTiets)
                    {
                        var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                            .Where(o => o.DuocPhamBenhVienId == chiTietVo.DuocPhamBenhVienId &&
                                        o.LaDuocPhamBHYT == chiTietVo.LaDuocPhamBHYT && o.Solo == chiTietVo.SoLo &&
                                        o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (nhapKhoDuocPhamChiTietXuats.Count() == 0 ||
                            (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat))
                        {
                            throw new Exception(
                                _localizationService.GetResource(
                                    "XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        var nhapKhoDuocPhamChiTietXuat = nhapKhoDuocPhamChiTietXuats.First();
                        var yeuCauXuatKhoDuocPhamChiTietNew = new YeuCauXuatKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = nhapKhoDuocPhamChiTietXuat.DuocPhamBenhVienId,
                            HopDongThauDuocPhamId = nhapKhoDuocPhamChiTietXuat.HopDongThauDuocPhamId,
                            LaDuocPhamBHYT = nhapKhoDuocPhamChiTietXuat.LaDuocPhamBHYT,
                            Solo = nhapKhoDuocPhamChiTietXuat.Solo,
                            HanSuDung = nhapKhoDuocPhamChiTietXuat.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoDuocPhamChiTietXuat.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoDuocPhamChiTietXuat.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoDuocPhamChiTietXuat.TiLeTheoThapGia,
                            VAT = nhapKhoDuocPhamChiTietXuat.VAT,
                            MaVach = nhapKhoDuocPhamChiTietXuat.MaVach,
                            TiLeBHYTThanhToan = nhapKhoDuocPhamChiTietXuat.TiLeBHYTThanhToan,
                            MaRef = nhapKhoDuocPhamChiTietXuat.MaRef,
                            SoLuongXuat = chiTietVo.SoLuongXuat.Value,
                            MayXetNghiemId = chiTietVo.MayXetNghiemId
                        };
                        yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets.Add(yeuCauXuatKhoDuocPhamChiTietNew);
                    }
                }
                _yeuCauXuatKhoDuocPhamRepository.Update(yeuCauXuatKhoDuocPham);
                return new CapNhatXuatKhoKhacResultVo { Id = yeuCauXuatKhoDuocPham.Id, LastModified = yeuCauXuatKhoDuocPham.LastModified };
            }
            else
            {
                if (yeuCauXuatDuocPhamChiTiets.Any())
                {
                    var duocPhamBenhVienIds = yeuCauXuatDuocPhamChiTiets.Select(o => o.DuocPhamBenhVienId).ToList();
                    var soLos = yeuCauXuatDuocPhamChiTiets.Select(o => o.SoLo).ToList();

                    var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.Table
                        .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauXuatKhoDuocPham.KhoXuatId &&
                                    duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) &&
                                    o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();
                    //xuat kho
                    var xuatKhoDuocPham = new XuatKhoDuocPham
                    {
                        LoaiXuatKho = Enums.XuatKhoDuocPham.XuatKhac,
                        KhoXuatId = yeuCauXuatKhoDuocPham.KhoXuatId,
                        LyDoXuatKho = yeuCauXuatKhoDuocPham.LyDoXuatKho,
                        NguoiXuatId = yeuCauXuatKhoDuocPham.NguoiXuatId,
                        NguoiNhanId = yeuCauXuatKhoDuocPham.NguoiNhanId,
                        LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                        NgayXuat = yeuCauXuatKhoDuocPham.NgayXuat,
                        TraNCC = yeuCauXuatKhoDuocPham.TraNCC,
                        NhaThauId = yeuCauXuatKhoDuocPham.NhaThauId,
                        SoChungTu = yeuCauXuatKhoDuocPham.SoChungTu,
                    };


                    foreach (var chiTietVo in yeuCauXuatDuocPhamChiTiets)
                    {
                        var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                            .Where(o => o.DuocPhamBenhVienId == chiTietVo.DuocPhamBenhVienId &&
                                        o.LaDuocPhamBHYT == chiTietVo.LaDuocPhamBHYT && o.Solo == chiTietVo.SoLo &&
                                        o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat)
                        {
                            throw new Exception(
                                _localizationService.GetResource(
                                    "XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        double soLuongCanXuat = chiTietVo.SoLuongXuat.Value;
                        while (!soLuongCanXuat.AlmostEqual(0))
                        {
                            // tinh so luong xuat
                            var nhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTietXuats
                                .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien)
                                .First();
                            var soLuongTon = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                            var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                            nhapKhoDuocPhamChiTiet.SoLuongDaXuat = (nhapKhoDuocPhamChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                            var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                            {
                                SoLuongXuat = soLuongXuat,
                                NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                                NgayXuat = yeuCauXuatKhoDuocPham.NgayXuat,
                                MayXetNghiemId = chiTietVo.MayXetNghiemId
                            };
                            var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                            {
                                DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                                NgayXuat = yeuCauXuatKhoDuocPham.NgayXuat
                            };
                            xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                            xuatKhoDuocPham.XuatKhoDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);

                            soLuongCanXuat = (soLuongCanXuat - soLuongXuat).MathRoundNumber(2);
                        }
                    }
                    yeuCauXuatKhoDuocPham.WillDelete = true;
                    BaseRepository.Add(xuatKhoDuocPham);
                    return new CapNhatXuatKhoKhacResultVo { Id = xuatKhoDuocPham.Id, LastModified = xuatKhoDuocPham.LastModified };
                }
                return null;
            }
        }

        public List<string> KiemTraXuatHoaChatTheoMayXetNghiem(List<XuatKhoKhacDuocPhamChiTietVo> yeuCauXuatDuocPhamChiTiets)
        {
            var duocPhamChuaChonMayXetNghiems = new List<string>();
            if (yeuCauXuatDuocPhamChiTiets != null && yeuCauXuatDuocPhamChiTiets.Any())
            {
                var ycChuaChonMayXetNghiems = yeuCauXuatDuocPhamChiTiets.Where(o => o.MayXetNghiemId == null).ToList();
                var duocPhamBenhVienIds = ycChuaChonMayXetNghiems.Select(o => o.DuocPhamBenhVienId).ToList();
                if (duocPhamBenhVienIds.Any())
                {
                    duocPhamChuaChonMayXetNghiems = _duocPhamBenhVienRepository.TableNoTracking
                        .Where(o => duocPhamBenhVienIds.Contains(o.Id) && o.DuocPhamBenhVienMayXetNghiems.Any())
                        .Select(o => o.DuocPham.Ten).ToList();
                }
            }
            return duocPhamChuaChonMayXetNghiems;
        }

        public async Task XuLyThemHoacCapNhatVaDuyetYeuCauThuocAsync(YeuCauXuatKhoDuocPham yeuCauXuatKhoDuocPham, List<XuatKhoKhacDuocPhamChiTietVo> yeuCauXuatDuocPhamChiTiets, bool laLuuDuyet, bool isCreate)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            if (!laLuuDuyet) // chỉ lưu
            {
                if (!isCreate)
                {
                    foreach (var chiTiet in yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets)
                    {
                        if (chiTiet.Id != 0)
                        {
                            chiTiet.WillDelete = true;
                        }
                    }
                }
                var khoXuatIds = yeuCauXuatDuocPhamChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
                var nhapKhoDuocPhamChiTietAlls = _nhapKhoDuocPhamChiTietRepository.Table
                    .Include(nl => nl.NhapKhoDuocPhams)
                   .Where(o => khoXuatIds.Contains(o.NhapKhoDuocPhams.KhoId)
                   && o.SoLuongNhap > o.SoLuongDaXuat)
                   .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var yeuCauXuatDuocPhamChiTiet in yeuCauXuatDuocPhamChiTiets)
                {
                    var nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTietAlls
                     .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauXuatDuocPhamChiTiet.KhoXuatId
                     && o.LaDuocPhamBHYT == yeuCauXuatDuocPhamChiTiet.LaDuocPhamBHYT
                     && o.DuocPhamBenhVienId == yeuCauXuatDuocPhamChiTiet.DuocPhamBenhVienId
                     && o.HanSuDung == yeuCauXuatDuocPhamChiTiet.HanSuDung
                     && o.Solo == yeuCauXuatDuocPhamChiTiet.SoLo
                     && o.SoLuongNhap > o.SoLuongDaXuat)
                     .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();

                    var SLTon = nhapKhoDuocPhamChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                    if (SLTon < yeuCauXuatDuocPhamChiTiet.SoLuongXuat)
                    {
                        throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                    }

                    var soLuongXuat = yeuCauXuatDuocPhamChiTiet.SoLuongXuat.MathRoundNumber(2);// số lượng xuất

                    foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                    {
                        if (soLuongXuat == 0)
                        {
                            break;
                        }
                        var yeuCauXuatKhoDuocPhamChiTietNew = new YeuCauXuatKhoDuocPhamChiTiet
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
                            MaVach = nhapKhoDuocPhamChiTiet.MaVach,
                            TiLeBHYTThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan,
                            MaRef = nhapKhoDuocPhamChiTiet.MaRef,
                        };
                        var SLTonHienTai = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                        if (SLTonHienTai > soLuongXuat || SLTonHienTai.AlmostEqual(soLuongXuat.Value))
                        {
                            yeuCauXuatKhoDuocPhamChiTietNew.SoLuongXuat = soLuongXuat.Value.MathRoundNumber(2);
                            soLuongXuat = 0;
                        }
                        else
                        {
                            yeuCauXuatKhoDuocPhamChiTietNew.SoLuongXuat = SLTonHienTai.MathRoundNumber(2);
                            soLuongXuat = (soLuongXuat - SLTonHienTai).MathRoundNumber(2);
                        }
                        yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets.Add(yeuCauXuatKhoDuocPhamChiTietNew);
                    }
                }
            }
            else
            {
                if (!isCreate)
                {
                    foreach (var chiTiet in yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets)
                    {
                        if (chiTiet.Id != 0)
                        {
                            chiTiet.WillDelete = true;
                        }
                    }
                }
                var tenNguoiNhan = _nhanVienRepository.TableNoTracking.Where(x => x.Id == yeuCauXuatKhoDuocPham.NguoiNhanId).Select(z => z.User.HoTen).FirstOrDefault();
                var xuatKhoDuocPham = new XuatKhoDuocPham
                {
                    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatKhac,
                    LyDoXuatKho = yeuCauXuatKhoDuocPham.LyDoXuatKho,
                    NguoiNhanId = yeuCauXuatKhoDuocPham.NguoiNhanId,
                    TenNguoiNhan = tenNguoiNhan,
                    NguoiXuatId = yeuCauXuatKhoDuocPham.NguoiXuatId,
                    LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayXuat = DateTime.Now,
                    KhoXuatId = yeuCauXuatKhoDuocPham.KhoXuatId,
                    TraNCC = yeuCauXuatKhoDuocPham.TraNCC,
                    NhaThauId = yeuCauXuatKhoDuocPham.NhaThauId,
                    SoChungTu = yeuCauXuatKhoDuocPham.SoChungTu
                };
                var khoXuatIds = yeuCauXuatDuocPhamChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
                var nhapKhoDuocPhamChiTietAlls = _nhapKhoDuocPhamChiTietRepository.Table
                    .Include(nl => nl.NhapKhoDuocPhams)
                   .Where(o => khoXuatIds.Contains(o.NhapKhoDuocPhams.KhoId)
                   && o.SoLuongNhap > o.SoLuongDaXuat)
                   .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var yeuCauXuatDuocPhamChiTiet in yeuCauXuatDuocPhamChiTiets)
                {
                    var nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTietAlls
                    .Where(o => o.NhapKhoDuocPhams.KhoId == yeuCauXuatDuocPhamChiTiet.KhoXuatId
                    && o.LaDuocPhamBHYT == yeuCauXuatDuocPhamChiTiet.LaDuocPhamBHYT
                    && o.DuocPhamBenhVienId == yeuCauXuatDuocPhamChiTiet.DuocPhamBenhVienId
                    && o.HanSuDung == yeuCauXuatDuocPhamChiTiet.HanSuDung
                    && o.Solo == yeuCauXuatDuocPhamChiTiet.SoLo
                    && o.SoLuongNhap > o.SoLuongDaXuat)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();

                    var SLTon = nhapKhoDuocPhamChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                    if (SLTon < yeuCauXuatDuocPhamChiTiet.SoLuongXuat)
                    {
                        throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                    }

                    var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                    {
                        DuocPhamBenhVienId = yeuCauXuatDuocPhamChiTiet.DuocPhamBenhVienId,
                        XuatKhoDuocPham = xuatKhoDuocPham
                    };

                    var soLuongXuat = yeuCauXuatDuocPhamChiTiet.SoLuongXuat.MathRoundNumber(2);// số lượng xuất

                    foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                    {
                        if (soLuongXuat == 0)
                        {
                            break;
                        }
                        var yeuCauXuatKhoDuocPhamChiTietNew = new YeuCauXuatKhoDuocPhamChiTiet
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
                            MaVach = nhapKhoDuocPhamChiTiet.MaVach,
                            TiLeBHYTThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan,
                            MaRef = nhapKhoDuocPhamChiTiet.MaRef,
                        };
                        var SLTonHienTai = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                        if (SLTonHienTai > soLuongXuat || SLTonHienTai.AlmostEqual(soLuongXuat.Value))
                        {
                            nhapKhoDuocPhamChiTiet.SoLuongDaXuat = (nhapKhoDuocPhamChiTiet.SoLuongDaXuat + soLuongXuat.Value).MathRoundNumber(2);
                            yeuCauXuatKhoDuocPhamChiTietNew.SoLuongXuat = soLuongXuat.Value.MathRoundNumber(2);
                            soLuongXuat = 0;
                        }
                        else
                        {
                            nhapKhoDuocPhamChiTiet.SoLuongDaXuat = nhapKhoDuocPhamChiTiet.SoLuongNhap.MathRoundNumber(2);
                            yeuCauXuatKhoDuocPhamChiTietNew.SoLuongXuat = SLTonHienTai.MathRoundNumber(2);
                            soLuongXuat = (soLuongXuat - SLTonHienTai).MathRoundNumber(2);
                        }
                        var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                        {
                            SoLuongXuat = yeuCauXuatKhoDuocPhamChiTietNew.SoLuongXuat,
                            GhiChu = yeuCauXuatKhoDuocPham.TraNCC == true ? "Trả nhà cung cấp" : "Xuất khác.",
                            XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet,
                            NgayXuat = DateTime.Now,
                            NhapKhoDuocPhamChiTietId = nhapKhoDuocPhamChiTiet.Id
                        };
                        yeuCauXuatKhoDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri = xuatKhoDuocPhamChiTietViTri;
                        yeuCauXuatKhoDuocPhamChiTietNew.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet;
                        yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets.Add(yeuCauXuatKhoDuocPhamChiTietNew);
                        await XuLySoLuongTonAsync(nhapKhoDuocPhamChiTiet);
                    }
                }
            }
            await _yeuCauXuatKhoDuocPhamRepository.UpdateAsync(yeuCauXuatKhoDuocPham);
        }

        private async Task XuLySoLuongTonAsync(NhapKhoDuocPhamChiTiet nhapKhoDuocPhamChiTiet)
        {
            await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(nhapKhoDuocPhamChiTiet);
        }

        public async Task<XuatKhoKhacDuocPhamRsVo> XuLyXoaYeuCauThuocAsync(YeuCauXuatKhoDuocPham yeuCauXuatKhoDuocPham)
        {
            foreach (var item in yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets)
            {
                item.WillDelete = true;
            }
            yeuCauXuatKhoDuocPham.WillDelete = true;
            await _yeuCauXuatKhoDuocPhamRepository.UpdateAsync(yeuCauXuatKhoDuocPham);
            var xuatKhoKhacDuocPhamRsVo = new XuatKhoKhacDuocPhamRsVo
            {
                Id = yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets.First().XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId.GetValueOrDefault(),
                LastModified = yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets.First().XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LastModified,
            };
            return xuatKhoKhacDuocPhamRsVo;
        }

        public async Task CheckPhieuYeuCauXuatThuocKhacDaDuyetHoacDaHuy(long yeuCauXuatKhoDuocPhamId)
        {
            var result = await _yeuCauXuatKhoDuocPhamRepository.TableNoTracking.Where(p => p.Id == yeuCauXuatKhoDuocPhamId).Select(p => p).FirstOrDefaultAsync();
            var resourceName = string.Empty;
            if (result == null)
            {
                resourceName = "YeuCauDieuChuyenDuocPham.PhieuYeuCau.NotExists";
            }
            else
            {
                if (result.DuocDuyet == true)
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

        public async Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long yeuCauXuatKhoDuocPhamId)
        {
            var yeuCau = await _yeuCauXuatKhoDuocPhamRepository.TableNoTracking.Where(p => p.Id == yeuCauXuatKhoDuocPhamId).FirstOrDefaultAsync();
            var trangThaiVo = new TrangThaiDuyetVo();
            if (yeuCau != null)
            {
                trangThaiVo.TrangThai = null;
                trangThaiVo.Ten = "Đang chờ duyệt";
                return trangThaiVo;
            }
            else
            {
                trangThaiVo.TrangThai = true;
                trangThaiVo.Ten = "Đã duyệt xuất";
                return trangThaiVo;
            }
        }

        public async Task<List<LookupItemVo>> GetKhoDuocPham(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var lstEntity = await _khoDuocPhamRepository.TableNoTracking.Where(p =>
            (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe
            || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc
            || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoVacXin
            || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
            && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
            && EF.Functions.Like(p.Ten, $"%{model.Query}%"))
                .Take(1000)
                .ToListAsync();
            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<XuatKhoKhacLookupItem>> GetKhoTheoLoaiDuocPham(DropDownListRequestModel model)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var khoId = CommonHelper.GetIdFromRequestDropDownList(model);
            var khos = await _khoDuocPhamRepository.TableNoTracking
                 .Where(p =>
                         ((p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                         || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc
                         || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoVacXin)
                         && p.KhoaPhongId == phongBenhVien.KhoaPhongId
                         && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId))
                         && p.LoaiDuocPham == true)
                         .ApplyLike(model.Query, p => p.Ten)
                         .Select(item => new XuatKhoKhacLookupItem
                         {
                             DisplayName = item.Ten,
                             KeyId = item.Id,
                             LoaiKho = item.LoaiKho
                         })
                         .OrderByDescending(x => x.KeyId == khoId).ThenBy(x => x.DisplayName)
                         .Take(model.Take).ToListAsync();
            return khos;
        }

        public async Task<List<LookupItemVo>> GetNguoiNhanXuatKhac(DropDownListRequestModel model)
        {
            var nhanVienId = CommonHelper.GetIdFromRequestDropDownList(model);
            var nhanViens = _nhanVienRepository.TableNoTracking
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.User.HoTen,
                    KeyId = item.Id,
                })
                 .OrderByDescending(x => x.KeyId == nhanVienId).ThenBy(x => x.DisplayName)
                 .ApplyLike(model.Query, p => p.DisplayName)
                .Take(model.Take);

            return await nhanViens.ToListAsync();
        }


        public async Task<bool> CheckSoLuongTonDuocPham(long duocPhamBenhVienId, bool? laDuocPhamBHYT, long? khoXuatId, double? soLuongXuat, string soLo, DateTime? hanSuDung)
        {
            if (soLuongXuat == null)
            {
                return true;
            }
            var soLuongTonDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId && o.LaDuocPhamBHYT == laDuocPhamBHYT && o.NhapKhoDuocPhams.KhoId == khoXuatId && o.HanSuDung == hanSuDung && o.Solo == soLo && o.SoLuongNhap > o.SoLuongDaXuat
            //&& o.HanSuDung >= DateTime.Now
            ).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);

            soLuongTonDuocPham = soLuongTonDuocPham.MathRoundNumber(2);
            if (soLuongXuat > soLuongTonDuocPham)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<double> GetSoLuongTonThucTe(YeuCauXuatKhoDuocPhamGridVo yeuCauXuatKhoDuocPhamGridVo)
        {
            return await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(z => z.DuocPhamBenhVienId == yeuCauXuatKhoDuocPhamGridVo.DuocPhamBenhVienId && z.LaDuocPhamBHYT == yeuCauXuatKhoDuocPhamGridVo.LaDuocPhamBHYT && z.Solo == yeuCauXuatKhoDuocPhamGridVo.SoLo && z.HanSuDung == yeuCauXuatKhoDuocPhamGridVo.HanSuDung && z.NhapKhoDuocPhams.KhoId == yeuCauXuatKhoDuocPhamGridVo.KhoXuatId
            //&& z.HanSuDung >= DateTime.Now
            && z.SoLuongNhap > z.SoLuongDaXuat).SumAsync(x => x.SoLuongNhap - x.SoLuongDaXuat);
        }

        public async Task<Enums.EnumLoaiKhoDuocPham> GetLoaiKho(long khoId)
        {
            return await _khoDuocPhamRepository.TableNoTracking.Where(z => z.Id == khoId).Select(x => x.LoaiKho).FirstAsync();
        }

        public async Task<List<LookupItemVo>> GetNhaCungCapTheoKhoDuocPhams(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.ParameterDependencies))
            {
                return null;
            }

            var info = JsonConvert.DeserializeObject<NhaCCTheoKhoDuocPhamJsonVo>(model.ParameterDependencies.Replace("undefined", "null"));
            var query = _nhaThauRepository.TableNoTracking
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.Id,
                            DisplayName = s.Ten
                        }).ApplyLike(model.Query, q => q.DisplayName)
                        .OrderBy(z => z.KeyId == info.Id)
                        .Take(model.Take)
                        ;
            return await query.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetSoHoaDonTheoKhoDuocPhams(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.ParameterDependencies))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<SoCTTheoKhoDuocPhamJsonVo>(model.ParameterDependencies);
            var query = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Include(cc => cc.NhapKhoDuocPhams)
                        .Where(z => z.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 && z.HopDongThauDuocPhams.NhaThauId == info.NhaThauId
                        && !string.IsNullOrEmpty(z.NhapKhoDuocPhams.SoChungTu))
                        .GroupBy(x => new { x.NhapKhoDuocPhams.SoChungTu }).Select(g => new { NhapKhoDuocPhamChiTiets = g.FirstOrDefault() });
            var results = new List<LookupItemVo>();
            foreach (var item in query)
            {
                var result = new LookupItemVo
                {
                    KeyId = item.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamId,
                    DisplayName = item.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhams.SoChungTu
                };
                results.Add(result);
            }
            if (!string.IsNullOrEmpty(model.Query))
            {
                results = results.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return results.Take(model.Take).ToList();
        }

        public async Task<long?> GetNhapKhoDuocPhamIdBy(string soChungTu)
        {
            return await _nhapKhoDuocPhamRepository.TableNoTracking.Where(z => z.SoChungTu.Equals(soChungTu)).Select(z => z.Id).FirstOrDefaultAsync();
        }

        public string InPhieuXuatKhoKhac(PhieuXuatKhoKhacVo phieuXuatKhoKhac)
        {
            var content = string.Empty;
            var template = new Template();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId, s => s.Include(c => c.KhoaPhong));
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var xuatKhoDuocPhamKhacInVo = new XuatKhoDuocPhamKhacInVo();
            var xuatKhoDuocPhamKhacChiTietInVos = new List<XuatKhoDuocPhamKhacChiTietInVo>();
            var duocPhamHoacVatTus = string.Empty;
            if (phieuXuatKhoKhac.LaDuocPham)
            {
                if (phieuXuatKhoKhac.DuocDuyet == true)
                {
                    xuatKhoDuocPhamKhacInVo = BaseRepository.TableNoTracking.Where(c => c.Id == phieuXuatKhoKhac.Id)
                                            .Select(s => new XuatKhoDuocPhamKhacInVo
                                            {
                                                SoPhieu = s.SoPhieu,
                                                KhoaPhong = phongBenhVien.KhoaPhong.Ten,
                                                CreatedOn = s.CreatedOn,
                                                KhoTraLai = s.KhoDuocPhamXuat.Ten,
                                                NguoiXuat = s.NguoiXuat.User.HoTen,
                                                //SoPhieuNhap = s.NhapKhoDuocPhams.Any() ? s.NhapKhoDuocPhams.FirstOrDefault().SoPhieu : "",
                                                //NgayNhap = s.NhapKhoDuocPhams.Any() ? s.NhapKhoDuocPhams.FirstOrDefault().NgayNhap : (DateTime?)null,
                                                SoHoaDon = s.SoChungTu,
                                                NhaThauId = s.NhaThauId,
                                                //NgayHoaDon = s.NhapKhoDuocPhams.Any() ? s.NhapKhoDuocPhams.FirstOrDefault().NgayHoaDon : null,
                                                NCC = s.NhaThau != null ? s.NhaThau.Ten : "",
                                                DienGiai = s.LyDoXuatKho
                                            }).First();
                    xuatKhoDuocPhamKhacChiTietInVos = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                       .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == phieuXuatKhoKhac.Id)
                       .Select(s => new XuatKhoDuocPhamKhacChiTietInVo()
                       {
                           Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                           Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                           DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                           SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
                           HanSuDungDisplay = s.NhapKhoDuocPhamChiTiet.HanSuDung.ApplyFormatDate(),
                           SoLuong = s.SoLuongXuat,
                           DonGia = s.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                           GhiChu = s.GhiChu
                       }).OrderBy(z => z.Ten).ToList();
                    decimal tongCong = 0;
                    tongCong = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.ThanhTien);
                    xuatKhoDuocPhamKhacInVo.TongCong = tongCong.ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.VAT = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.VAT).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.GiaTriThanhToan = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongTienBangChu = ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan));
                }
                else
                {
                    xuatKhoDuocPhamKhacInVo = _yeuCauXuatKhoDuocPhamRepository.TableNoTracking.Where(c => c.Id == phieuXuatKhoKhac.Id)
                                           .Select(s => new XuatKhoDuocPhamKhacInVo
                                           {
                                               SoPhieu = "",
                                               KhoaPhong = phongBenhVien.KhoaPhong.Ten,
                                               CreatedOn = s.CreatedOn,
                                               KhoTraLai = s.KhoXuat.Ten,
                                               NguoiXuat = s.NguoiXuat.User.HoTen,
                                               //SoPhieuNhap = s.YeuCauXuatKhoDuocPhamChiTiets.Any() && s.YeuCauXuatKhoDuocPhamChiTiets.First().XuatKhoDuocPhamChiTietViTri != null ? s.YeuCauXuatKhoDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.SoPhieu : "",
                                               //NgayNhap = s.YeuCauXuatKhoDuocPhamChiTiets.Any() && s.YeuCauXuatKhoDuocPhamChiTiets.First().XuatKhoDuocPhamChiTietViTri != null ? s.YeuCauXuatKhoDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.NgayNhap : (DateTime?)null,
                                               SoHoaDon = s.SoChungTu,
                                               NhaThauId = s.NhaThauId,
                                               //NgayHoaDon = s.YeuCauXuatKhoDuocPhamChiTiets.Any() && s.YeuCauXuatKhoDuocPhamChiTiets.First().XuatKhoDuocPhamChiTietViTri != null ? s.YeuCauXuatKhoDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.NgayHoaDon : null,
                                               NCC = s.NhaThau != null ? s.NhaThau.Ten : "",
                                               DienGiai = s.LyDoXuatKho,
                                               ChietKhau = 0
                                           }).First();
                    xuatKhoDuocPhamKhacChiTietInVos = _yeuCauXuatKhoDuocPhamChiTietRepository.TableNoTracking
                       .Where(x => x.YeuCauXuatKhoDuocPhamId == phieuXuatKhoKhac.Id)
                       .Select(s => new XuatKhoDuocPhamKhacChiTietInVo()
                       {
                           Ma = s.DuocPhamBenhVien.Ma,
                           Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                           DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           SoLo = s.Solo,
                           HanSuDungDisplay = s.HanSuDung.ApplyFormatDate(),
                           SoLuong = s.SoLuongXuat,
                           DonGia = s.DonGiaNhap,
                           VATCal = s.VAT,
                           GhiChu = ""
                       }).OrderBy(z => z.Ten).ToList();
                    decimal tongCong = 0;
                    tongCong = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.ThanhTien);
                    xuatKhoDuocPhamKhacInVo.VAT = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.VAT).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.GiaTriThanhToan = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongCong = tongCong.ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongTienBangChu = ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan));
                }
                if (phieuXuatKhoKhac.CoNCC != true)
                {
                    var STT = 1;
                    template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatThuocKhac"));
                    foreach (var item in xuatKhoDuocPhamKhacChiTietInVos)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLo
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HanSuDungDisplay
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.SoLuong != (int)item.SoLuong ? item.SoLuong.ToString() : item.SoLuong.ApplyNumber())
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                   + "</tr>";
                        STT++;
                    }
                    xuatKhoDuocPhamKhacInVo.ThuocVatTu = duocPhamHoacVatTus;
                    xuatKhoDuocPhamKhacInVo.CongKhoan = STT - 1;
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, xuatKhoDuocPhamKhacInVo);
                }
                else
                {
                    //lay SoPhieuNhap,NgayNhap, NgayHoaDon
                    if (xuatKhoDuocPhamKhacInVo.NhaThauId != null && !string.IsNullOrEmpty(xuatKhoDuocPhamKhacInVo.SoHoaDon))
                    {
                        var thongTinNhap = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(o => o.YeuCauNhapKhoDuocPham.DuocKeToanDuyet == true && o.YeuCauNhapKhoDuocPham.SoChungTu == xuatKhoDuocPhamKhacInVo.SoHoaDon && o.HopDongThauDuocPham.NhaThauId == xuatKhoDuocPhamKhacInVo.NhaThauId)
                            .Select(o => new { o.Id, o.YeuCauNhapKhoDuocPham.SoPhieu, o.YeuCauNhapKhoDuocPham.NgayNhap, o.YeuCauNhapKhoDuocPham.NgayHoaDon })
                            .OrderBy(o => o.Id).LastOrDefault();
                        if (thongTinNhap != null)
                        {
                            xuatKhoDuocPhamKhacInVo.SoPhieuNhap = thongTinNhap.SoPhieu;
                            xuatKhoDuocPhamKhacInVo.DTNgayNhap = thongTinNhap.NgayNhap;
                            xuatKhoDuocPhamKhacInVo.DTNgayHoaDon = thongTinNhap.NgayHoaDon;
                        }
                    }

                    template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatThuocKhacNCC"));
                    var STT = 1;
                    foreach (var item in xuatKhoDuocPhamKhacChiTietInVos)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLo
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HanSuDungDisplay
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.SoLuong != (int)item.SoLuong ? item.SoLuong.ToString() : item.SoLuong.ApplyNumber())
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "</tr>";
                        STT++;
                    }
                    xuatKhoDuocPhamKhacInVo.ThuocVatTu = duocPhamHoacVatTus;
                    xuatKhoDuocPhamKhacInVo.CongKhoan = STT - 1;
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, xuatKhoDuocPhamKhacInVo);
                }
            }
            else // VẬT TƯ
            {
                if (phieuXuatKhoKhac.DuocDuyet == true)
                {
                    xuatKhoDuocPhamKhacInVo = _xuatKhoVatTuRepository.TableNoTracking.Where(c => c.Id == phieuXuatKhoKhac.Id)
                                            .Select(s => new XuatKhoDuocPhamKhacInVo
                                            {
                                                SoPhieu = s.SoPhieu,
                                                KhoaPhong = phongBenhVien.KhoaPhong.Ten,
                                                CreatedOn = s.CreatedOn,
                                                KhoTraLai = s.KhoVatTuXuat.Ten,
                                                NguoiXuat = s.NguoiXuat.User.HoTen,
                                                //SoPhieuNhap = s.NhapKhoVatTus.Any() ? s.NhapKhoVatTus.FirstOrDefault().SoPhieu : "",
                                                //NgayNhap = s.NhapKhoVatTus.Any() ? s.NhapKhoVatTus.FirstOrDefault().NgayNhap : (DateTime?)null,
                                                SoHoaDon = s.SoChungTu,
                                                NhaThauId = s.NhaThauId,
                                                //NgayHoaDon = s.NhapKhoVatTus.Any() ? s.NhapKhoVatTus.FirstOrDefault().NgayHoaDon : null,
                                                NCC = s.NhaThau != null ? s.NhaThau.Ten : "",
                                                DienGiai = s.LyDoXuatKho
                                            }).First();
                    xuatKhoDuocPhamKhacChiTietInVos = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                       .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == phieuXuatKhoKhac.Id)
                       .Select(s => new XuatKhoDuocPhamKhacChiTietInVo()
                       {
                           Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                           Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                           DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                           SoLo = s.NhapKhoVatTuChiTiet.Solo,
                           HanSuDungDisplay = s.NhapKhoVatTuChiTiet.HanSuDung.ApplyFormatDate(),
                           SoLuong = s.SoLuongXuat,
                           DonGia = s.NhapKhoVatTuChiTiet.DonGiaNhap,
                           GhiChu = s.GhiChu
                       }).OrderBy(z => z.Ten).ToList();
                    decimal tongCong = 0;
                    tongCong = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.ThanhTien);
                    xuatKhoDuocPhamKhacInVo.TongCong = tongCong.ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.VAT = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.VAT).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.GiaTriThanhToan = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongTienBangChu = ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan));
                }
                else
                {
                    xuatKhoDuocPhamKhacInVo = _yeuCauXuatKhoVatTuRepository.TableNoTracking.Where(c => c.Id == phieuXuatKhoKhac.Id)
                                           .Select(s => new XuatKhoDuocPhamKhacInVo
                                           {
                                               SoPhieu = "",
                                               KhoaPhong = phongBenhVien.KhoaPhong.Ten,
                                               CreatedOn = s.CreatedOn,
                                               KhoTraLai = s.KhoXuat.Ten,
                                               NguoiXuat = s.NguoiXuat.User.HoTen,
                                               //SoPhieuNhap = s.YeuCauXuatKhoVatTuChiTiets.Any() && s.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri != null ? s.YeuCauXuatKhoVatTuChiTiets.FirstOrDefault().XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NhapKhoVatTu.SoPhieu : "",
                                               //NgayNhap = s.YeuCauXuatKhoVatTuChiTiets.Any() && s.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri != null ? s.YeuCauXuatKhoVatTuChiTiets.FirstOrDefault().XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NhapKhoVatTu.NgayNhap : (DateTime?)null,
                                               SoHoaDon = s.SoChungTu,
                                               NhaThauId = s.NhaThauId,
                                               //NgayHoaDon = s.YeuCauXuatKhoVatTuChiTiets.Any() && s.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri != null ? s.YeuCauXuatKhoVatTuChiTiets.FirstOrDefault().XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NhapKhoVatTu.NgayHoaDon : null,
                                               NCC = s.NhaThau != null ? s.NhaThau.Ten : "",
                                               DienGiai = s.LyDoXuatKho,
                                               ChietKhau = 0
                                           }).First();
                    xuatKhoDuocPhamKhacChiTietInVos = _yeuCauXuatKhoVatTuChiTietRepository.TableNoTracking
                       .Where(x => x.YeuCauXuatKhoVatTuId == phieuXuatKhoKhac.Id)
                       .Select(s => new XuatKhoDuocPhamKhacChiTietInVo()
                       {
                           Ma = s.VatTuBenhVien.Ma,
                           Ten = s.VatTuBenhVien.VatTus.Ten,
                           DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                           SoLo = s.Solo,
                           HanSuDungDisplay = s.HanSuDung.ApplyFormatDate(),
                           SoLuong = s.SoLuongXuat,
                           DonGia = s.DonGiaNhap,
                           VATCal = s.VAT,
                           GhiChu = ""
                       }).OrderBy(z => z.Ten).ToList();
                    decimal tongCong = 0;
                    tongCong = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.ThanhTien);
                    xuatKhoDuocPhamKhacInVo.VAT = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.VAT).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.GiaTriThanhToan = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongCong = tongCong.ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongTienBangChu = ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan));
                }
                if (phieuXuatKhoKhac.CoNCC != true)
                {
                    var STT = 1;
                    template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatThuocKhac"));
                    var gopThuocCungTenLoDvtHsdDonGias = xuatKhoDuocPhamKhacChiTietInVos.GroupBy(p => new { p.Ten, p.DVT, p.SoLo, p.HanSuDungDisplay, p.DonGia });
                    foreach (var item in gopThuocCungTenLoDvtHsdDonGias)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Key.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.SoLo
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.HanSuDungDisplay
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.Sum(c => c.SoLuong) != (int)item.Sum(c => c.SoLuong) ? item.Sum(c => c.SoLuong).ToString() : item.Sum(c => c.SoLuong).ApplyNumber())
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.Key.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + ((int)item.Sum(c => c.SoLuong) * item.Key.DonGia).ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                   + "</tr>";
                        STT++;
                    }
                    xuatKhoDuocPhamKhacInVo.ThuocVatTu = duocPhamHoacVatTus;
                    xuatKhoDuocPhamKhacInVo.CongKhoan = STT - 1;
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, xuatKhoDuocPhamKhacInVo);
                }
                else
                {
                    //lay SoPhieuNhap,NgayNhap, NgayHoaDon
                    if (xuatKhoDuocPhamKhacInVo.NhaThauId != null && !string.IsNullOrEmpty(xuatKhoDuocPhamKhacInVo.SoHoaDon))
                    {
                        var thongTinNhap = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(o => o.YeuCauNhapKhoVatTu.DuocKeToanDuyet == true && o.YeuCauNhapKhoVatTu.SoChungTu == xuatKhoDuocPhamKhacInVo.SoHoaDon && o.HopDongThauVatTu.NhaThauId == xuatKhoDuocPhamKhacInVo.NhaThauId)
                            .Select(o => new { o.Id, o.YeuCauNhapKhoVatTu.SoPhieu, o.YeuCauNhapKhoVatTu.NgayNhap, o.YeuCauNhapKhoVatTu.NgayHoaDon })
                            .OrderBy(o => o.Id).LastOrDefault();
                        if (thongTinNhap != null)
                        {
                            xuatKhoDuocPhamKhacInVo.SoPhieuNhap = thongTinNhap.SoPhieu;
                            xuatKhoDuocPhamKhacInVo.DTNgayNhap = thongTinNhap.NgayNhap;
                            xuatKhoDuocPhamKhacInVo.DTNgayHoaDon = thongTinNhap.NgayHoaDon;
                        }
                    }

                    template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatThuocKhacNCC"));
                    var STT = 1;
                    var gopThuocCungTenLoDvtHsdDonGias = xuatKhoDuocPhamKhacChiTietInVos.GroupBy(p => new { p.Ma, p.Ten, p.DVT, p.SoLo, p.HanSuDungDisplay, p.DonGia });
                    foreach (var item in gopThuocCungTenLoDvtHsdDonGias)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.Ma
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Key.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.SoLo
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.HanSuDungDisplay
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.Sum(c => c.SoLuong) != (int)item.Sum(c => c.SoLuong) ? item.Sum(c => c.SoLuong).ToString() : item.Sum(c => c.SoLuong).ApplyNumber())
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.Key.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + ((int)item.Sum(c => c.SoLuong) * item.Key.DonGia).ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "</tr>";
                        STT++;
                    }
                    xuatKhoDuocPhamKhacInVo.ThuocVatTu = duocPhamHoacVatTus;
                    xuatKhoDuocPhamKhacInVo.CongKhoan = STT - 1;
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, xuatKhoDuocPhamKhacInVo);
                }
            }
            return content;
        }

        public List<LookupItemVo> GetAllMayXetNghiemKhoKhac(DropDownListRequestModel queryInfo, DuocPhamBenhVienMayXetNghiemJson duocPhamBenhVienMayXetNghiemJson)
        {
            var getAllMayXetNghiems = _duocPhamBenhVienMayXetNghiemRepository.TableNoTracking
                .Where(z => z.DuocPhamBenhVienId == duocPhamBenhVienMayXetNghiemJson.DuocPhamBenhVienId && z.MayXetNghiem.HieuLuc)
                .Select(item => new LookupItemVo
                {
                    KeyId = item.MayXetNghiem.Id,
                    DisplayName = item.MayXetNghiem.Ten
                }).ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take);

            return getAllMayXetNghiems.ToList();
        }
    }
}
