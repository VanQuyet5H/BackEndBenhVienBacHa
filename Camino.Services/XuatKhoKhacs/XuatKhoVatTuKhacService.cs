using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
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
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using System.Globalization;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.Localization;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;

namespace Camino.Services.XuatKhoKhacs
{
    [ScopedDependency(ServiceType = typeof(IXuatKhoVatTuKhacService))]
    public class XuatKhoVatTuKhacService : MasterFileService<XuatKhoVatTu>, IXuatKhoVatTuKhacService
    {
        private readonly IRepository<Kho> _khoRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<XuatKhoVatTuChiTiet> _xuatKhoVatTuChiTietRepository;
        private readonly IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;
        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private readonly IRepository<NhapKhoVatTu> _nhapKhoVatTuRepository;
        private readonly IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<YeuCauXuatKhoVatTu> _yeuCauXuatKhoVatTuRepository;
        private readonly IRepository<YeuCauXuatKhoVatTuChiTiet> _yeuCauXuatKhoVatTuChiTietRepository;
        private readonly IRepository<YeuCauNhapKhoVatTuChiTiet> _yeuCauNhapKhoVatTuChiTietRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;
        private readonly IRepository<Core.Domain.Entities.NhaThaus.NhaThau> _nhaThauRepository;


        public XuatKhoVatTuKhacService(IRepository<XuatKhoVatTu> repository, IRepository<Kho> khoRepository
            , IRepository<XuatKhoVatTuChiTiet> xuatKhoVatTuChiTietRepository, IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository
            , IRepository<NhapKhoVatTu> nhapKhoVatTuRepository, IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository
            , IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository, IRepository<Template> templateRepository
            , ICauHinhService cauHinhService, IUserAgentHelper userAgentHelper
            , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
            , IRepository<YeuCauXuatKhoVatTu> yeuCauXuatKhoVatTuRepository
            , IRepository<YeuCauXuatKhoVatTuChiTiet> yeuCauXuatKhoVatTuChiTietRepository
            , ILocalizationService localizationService
            , IRepository<LocaleStringResource> localeStringResourceRepository
            , IRepository<YeuCauNhapKhoVatTuChiTiet> yeuCauNhapKhoVatTuChiTietRepository
            , IRepository<Core.Domain.Entities.NhaThaus.NhaThau> nhaThauRepository
            , IRepository<VatTuBenhVien> vatTuBenhVienRepository) : base(repository)
        {
            _khoRepository = khoRepository;
            _nhanVienRepository = nhanVienRepository;
            _xuatKhoVatTuChiTietRepository = xuatKhoVatTuChiTietRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _templateRepository = templateRepository;
            _cauHinhService = cauHinhService;
            _phongBenhVienRepository = phongBenhVienRepository;
            _userAgentHelper = userAgentHelper;
            _yeuCauXuatKhoVatTuRepository = yeuCauXuatKhoVatTuRepository;
            _yeuCauXuatKhoVatTuChiTietRepository = yeuCauXuatKhoVatTuChiTietRepository;
            _localizationService = localizationService;
            _localeStringResourceRepository = localeStringResourceRepository;
            _yeuCauNhapKhoVatTuChiTietRepository = yeuCauNhapKhoVatTuChiTietRepository;
            _nhaThauRepository = nhaThauRepository;
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
                 .Where(p => p.LoaiXuatKho == Enums.EnumLoaiXuatKho.XuatKhac && (p.KhoVatTuXuat.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoVatTuXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2))
                .Select(s => new XuatKhoVatTuKhacGridVo
                {
                    Id = s.Id,
                    KhoVatTuXuat = s.KhoVatTuXuat.Ten,
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    TenNguoiNhan = s.NguoiNhan.User.HoTen,
                    TenNguoiXuat = s.NguoiXuat.User.HoTen,
                    NgayXuat = s.NgayXuat,
                    DuocDuyet = true,
                    CoNCC = s.TraNCC
                }).Union(
                _yeuCauXuatKhoVatTuRepository
                .TableNoTracking
                 .Where(p => p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                .Select(z => new XuatKhoVatTuKhacGridVo
                {
                    Id = z.Id,
                    KhoVatTuXuat = z.KhoXuat.Ten,
                    LyDoXuatKho = z.LyDoXuatKho,
                    TenNguoiNhan = z.NguoiNhan.User.HoTen,
                    TenNguoiXuat = z.NguoiXuat.User.HoTen,
                    NgayXuat = z.NgayXuat,
                    DuocDuyet = null,
                    CoNCC = z.TraNCC
                }));

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<XuatKhoVatTuKhacGridVo>(queryInfo.AdditionalSearchString);

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
                         g => g.KhoVatTuXuat,
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
            BuildDefaultSortExpression(queryInfo);
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                   .Where(p => p.LoaiXuatKho == Enums.EnumLoaiXuatKho.XuatKhac && (p.KhoVatTuXuat.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoVatTuXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2))
                .Select(s => new XuatKhoVatTuKhacGridVo
                {
                    Id = s.Id,
                    KhoVatTuXuat = s.KhoVatTuXuat.Ten,
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    TenNguoiNhan = s.NguoiNhan.User.HoTen,
                    TenNguoiXuat = s.NguoiXuat.User.HoTen,
                    NgayXuat = s.NgayXuat,
                    DuocDuyet = true
                }).Union(
                _yeuCauXuatKhoVatTuRepository
                .TableNoTracking
                 .Where(p => p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                .Select(z => new XuatKhoVatTuKhacGridVo
                {
                    Id = z.Id,
                    KhoVatTuXuat = z.KhoXuat.Ten,
                    LyDoXuatKho = z.LyDoXuatKho,
                    TenNguoiNhan = z.NguoiNhan.User.HoTen,
                    TenNguoiXuat = z.NguoiXuat.User.HoTen,
                    NgayXuat = z.NgayXuat,
                    DuocDuyet = null
                }));

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<XuatKhoVatTuKhacGridVo>(queryInfo.AdditionalSearchString);

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
                         g => g.KhoVatTuXuat,
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
            var xuatKhoVatTuId = long.Parse(queryObj[0]);
            var tinhTrang = long.Parse(queryObj[1]);
            IQueryable<YeuCauXuatKhoVatTuGridVo> query;
            if (tinhTrang == 0)
            {
                query = _yeuCauXuatKhoVatTuChiTietRepository.TableNoTracking
               .Where(x => x.YeuCauXuatKhoVatTuId == xuatKhoVatTuId)
               .Select(s => new YeuCauXuatKhoVatTuGridVo()
               {
                   Id = s.Id,
                   Ten = s.VatTuBenhVien.VatTus.Ten,
                   DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                   TenNhom = s.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.VatTuBenhVien.Ma,
                   SoLo = s.Solo,
                   SoLuongXuat = s.SoLuongXuat,
                   HanSuDung = s.HanSuDung,
               });
            }
            else
            {
                query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
               .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == xuatKhoVatTuId)
               .Select(s => new YeuCauXuatKhoVatTuGridVo()
               {
                   Id = s.Id,
                   Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                   DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                   TenNhom = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                   SoLo = s.NhapKhoVatTuChiTiet.Solo,
                   HanSuDung = s.NhapKhoVatTuChiTiet.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,
                   SoPhieu = s.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
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
            var xuatKhoVatTuId = long.Parse(queryObj[0]);
            var tinhTrang = long.Parse(queryObj[1]);
            IQueryable<YeuCauXuatKhoVatTuGridVo> query;
            if (tinhTrang == 0)
            {
                query = _yeuCauXuatKhoVatTuChiTietRepository.TableNoTracking
               .Where(x => x.YeuCauXuatKhoVatTuId == xuatKhoVatTuId)
               .Select(s => new YeuCauXuatKhoVatTuGridVo()
               {
                   Id = s.Id,
                   Ten = s.VatTuBenhVien.VatTus.Ten,
                   DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                   TenNhom = s.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.VatTuBenhVien.Ma,
                   SoLo = s.Solo,
                   SoLuongXuat = s.SoLuongXuat,
                   HanSuDung = s.HanSuDung,
               });
            }
            else
            {
                query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
               .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == xuatKhoVatTuId)
               .Select(s => new YeuCauXuatKhoVatTuGridVo()
               {
                   Id = s.Id,
                   Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                   DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                   TenNhom = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                   SoLo = s.NhapKhoVatTuChiTiet.Solo,
                   HanSuDung = s.NhapKhoVatTuChiTiet.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,
                   SoPhieu = s.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
               });
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataForGridChildAsyncDaDuyet(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<YeuCauXuatKhoVatTuGridVo>(queryInfo.AdditionalSearchString);
            var query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
           .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == queryString.XuatKhoVatTuId)
           .Select(s => new YeuCauXuatKhoVatTuGridVo()
           {
               Id = s.Id,
               Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
               DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
               TenNhom = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
               Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
               SoLo = s.NhapKhoVatTuChiTiet.Solo,
               HanSuDung = s.NhapKhoVatTuChiTiet.HanSuDung,
               SoLuongXuat = s.SoLuongXuat,
               SoPhieu = s.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
           });
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.DVT,
                     g => g.Ten,
                     g => g.Ma,
                     g => g.SoLo,
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
            var queryString = JsonConvert.DeserializeObject<YeuCauXuatKhoVatTuGridVo>(queryInfo.AdditionalSearchString);
            var query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
           .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == queryString.XuatKhoVatTuId)
           .Select(s => new YeuCauXuatKhoVatTuGridVo()
           {
               Id = s.Id,
               Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
               DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
               TenNhom = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
               Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
               SoLo = s.NhapKhoVatTuChiTiet.Solo,
               HanSuDung = s.NhapKhoVatTuChiTiet.HanSuDung,
               SoLuongXuat = s.SoLuongXuat,
               SoPhieu = s.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
           });
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.DVT,
                     g => g.Ten,
                     g => g.Ma,
                     g => g.SoLo,
                     g => g.SoPhieu
               );
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataForGridAsyncVatTuDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            BuildDefaultSortExpression(queryInfo);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauXuatKhoVatTuChiTietVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(dpbv => dpbv.VatTus).ThenInclude(dpbv => dpbv.NhomVatTu)
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoVatTu.Kho.Id == info.KhoXuatId);

            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoVatTuChiTiets = nhapKhoVatTuChiTiets.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten,
                    g => g.VatTuBenhVien.Ma,
                    g => g.VatTuBenhVien.VatTus.DonViTinh,
                    g => g.Solo
               );
            }
            var yeuCauDieuChuyenVatTuChiTietTheoKhoXuats = new List<YeuCauXuatKhoVatTuGridVo>();

            if (info.NhaThauId != null && !string.IsNullOrEmpty(info.SoChungTu))
            {
                var yeuCauNhapKhoVatTuChiTiets = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking.Where(z => z.YeuCauNhapKhoVatTu.SoChungTu.Equals(info.SoChungTu)).ToList();
                var nhapKhoVTCTs = new List<NhapKhoVatTuChiTiet>();
                foreach (var item in yeuCauNhapKhoVatTuChiTiets)
                {
                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                    {
                        if (item.VatTuBenhVienId == nhapKhoVatTuChiTiet.VatTuBenhVienId
                         && item.LaVatTuBHYT == nhapKhoVatTuChiTiet.LaVatTuBHYT
                         && item.HopDongThauVatTuId == nhapKhoVatTuChiTiet.HopDongThauVatTuId
                         && item.Solo == nhapKhoVatTuChiTiet.Solo
                         && item.HanSuDung == nhapKhoVatTuChiTiet.HanSuDung
                         && item.TiLeTheoThapGia == nhapKhoVatTuChiTiet.TiLeTheoThapGia
                         && item.VAT == nhapKhoVatTuChiTiet.VAT
                         && item.MaVach == nhapKhoVatTuChiTiet.MaVach
                         && item.MaRef == nhapKhoVatTuChiTiet.MaRef)
                        {
                            nhapKhoVTCTs.Add(nhapKhoVatTuChiTiet);
                        }
                    }
                }

                var nhapKhoVatTuChiTietGroup = nhapKhoVTCTs.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh })
                                                .Select(g => new { nhapKhoVatTuChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoVatTuGridVo
                    {
                        Id = item.nhapKhoVatTuChiTiets.Id,
                        VatTuBenhVienId = item.nhapKhoVatTuChiTiets.VatTuBenhVienId,
                        Ten = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.Ten,
                        DVT = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.nhapKhoVatTuChiTiets.LaVatTuBHYT,
                        NhomVatTuId = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.nhapKhoVatTuChiTiets.VatTuBenhVien.Ma,
                        SoLo = item.nhapKhoVatTuChiTiets.Solo,
                        HanSuDung = item.nhapKhoVatTuChiTiets.HanSuDung,
                        DonGiaNhap = item.nhapKhoVatTuChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoVatTuChiTiets.NhapKhoVatTu.KhoId
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }
            else
            {
                var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh })
                                                .Select(g => new { nhapKhoVatTuChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoVatTuGridVo
                    {
                        Id = item.nhapKhoVatTuChiTiets.Id,
                        VatTuBenhVienId = item.nhapKhoVatTuChiTiets.VatTuBenhVienId,
                        Ten = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.Ten,
                        DVT = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.nhapKhoVatTuChiTiets.LaVatTuBHYT,
                        NhomVatTuId = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.nhapKhoVatTuChiTiets.VatTuBenhVien.Ma,
                        SoLo = item.nhapKhoVatTuChiTiets.Solo,
                        HanSuDung = item.nhapKhoVatTuChiTiets.HanSuDung,
                        DonGiaNhap = item.nhapKhoVatTuChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoVatTuChiTiets.NhapKhoVatTu.KhoId
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }

            var vatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == info.KhoXuatId
                         && x.SoLuongDaXuat < x.SoLuongNhap
                         //&& x.HanSuDung >= DateTime.Now
                         ).ToList();

            var result = yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Select(o =>
            {
                o.SoLuongTon = vatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                o.SoLuongXuat = vatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            });
            if (info.VatTuBenhViens.Any())
            {
                result = result.Where(x => !info.VatTuBenhViens.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung));
            }
            var query = result.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncVatTuDaChon(QueryInfo queryInfo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauXuatKhoVatTuChiTietVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(dpbv => dpbv.VatTus).ThenInclude(dpbv => dpbv.NhomVatTu)
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoVatTu.Kho.Id == info.KhoXuatId);

            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoVatTuChiTiets = nhapKhoVatTuChiTiets.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten,
                    g => g.VatTuBenhVien.Ma,
                    g => g.VatTuBenhVien.VatTus.DonViTinh,
                    g => g.Solo
               );
            }
            var yeuCauDieuChuyenVatTuChiTietTheoKhoXuats = new List<YeuCauXuatKhoVatTuGridVo>();

            if (info.NhaThauId != null && !string.IsNullOrEmpty(info.SoChungTu))
            {
                var yeuCauNhapKhoVatTuChiTiets = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking.Where(z => z.YeuCauNhapKhoVatTu.SoChungTu.Equals(info.SoChungTu)).ToList();
                var nhapKhoVTCTs = new List<NhapKhoVatTuChiTiet>();
                foreach (var item in yeuCauNhapKhoVatTuChiTiets)
                {
                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                    {
                        if (item.VatTuBenhVienId == nhapKhoVatTuChiTiet.VatTuBenhVienId
                         && item.LaVatTuBHYT == nhapKhoVatTuChiTiet.LaVatTuBHYT
                         && item.HopDongThauVatTuId == nhapKhoVatTuChiTiet.HopDongThauVatTuId
                         && item.Solo == nhapKhoVatTuChiTiet.Solo
                         && item.HanSuDung == nhapKhoVatTuChiTiet.HanSuDung
                         && item.TiLeTheoThapGia == nhapKhoVatTuChiTiet.TiLeTheoThapGia
                         && item.VAT == nhapKhoVatTuChiTiet.VAT
                         && item.MaVach == nhapKhoVatTuChiTiet.MaVach
                         && item.MaRef == nhapKhoVatTuChiTiet.MaRef)
                        {
                            nhapKhoVTCTs.Add(nhapKhoVatTuChiTiet);
                        }
                    }
                }

                var nhapKhoVatTuChiTietGroup = nhapKhoVTCTs.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh })
                                                .Select(g => new { nhapKhoVatTuChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoVatTuGridVo
                    {
                        Id = item.nhapKhoVatTuChiTiets.Id,
                        VatTuBenhVienId = item.nhapKhoVatTuChiTiets.VatTuBenhVienId,
                        Ten = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.Ten,
                        DVT = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.nhapKhoVatTuChiTiets.LaVatTuBHYT,
                        NhomVatTuId = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.nhapKhoVatTuChiTiets.VatTuBenhVien.Ma,
                        SoLo = item.nhapKhoVatTuChiTiets.Solo,
                        HanSuDung = item.nhapKhoVatTuChiTiets.HanSuDung,
                        DonGiaNhap = item.nhapKhoVatTuChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoVatTuChiTiets.NhapKhoVatTu.KhoId
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }
            else
            {
                var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh })
                                                .Select(g => new { nhapKhoVatTuChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoVatTuGridVo
                    {
                        Id = item.nhapKhoVatTuChiTiets.Id,
                        VatTuBenhVienId = item.nhapKhoVatTuChiTiets.VatTuBenhVienId,
                        Ten = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.Ten,
                        DVT = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.nhapKhoVatTuChiTiets.LaVatTuBHYT,
                        NhomVatTuId = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.nhapKhoVatTuChiTiets.VatTuBenhVien.Ma,
                        SoLo = item.nhapKhoVatTuChiTiets.Solo,
                        HanSuDung = item.nhapKhoVatTuChiTiets.HanSuDung,
                        DonGiaNhap = item.nhapKhoVatTuChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoVatTuChiTiets.NhapKhoVatTu.KhoId
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }

            if (info.VatTuBenhViens.Any())
            {
                yeuCauDieuChuyenVatTuChiTietTheoKhoXuats = yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Where(x => !info.VatTuBenhViens.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung)).ToList();
            }
            var query = yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.AsQueryable();
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<long?> GetNhapKhoVatTuIdBy(string soChungTu)
        {
            return await _nhapKhoVatTuRepository.TableNoTracking.Where(z => z.SoChungTu.Equals(soChungTu)).Select(z => z.Id).FirstOrDefaultAsync();
        }

        public async Task<List<LookupItemVo>> GetSoHoaDonTheoKhoVatTus(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.ParameterDependencies))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<SoCTTheoKhoDuocPhamJsonVo>(model.ParameterDependencies);
            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Include(cc => cc.NhapKhoVatTu)
                        .Where(z =>
                        //z.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap1 && 
                        z.NhapKhoVatTu.Kho.LoaiVatTu == true &&
                        z.HopDongThauVatTu.NhaThauId == info.NhaThauId
                        && !string.IsNullOrEmpty(z.NhapKhoVatTu.SoChungTu))
                        .GroupBy(x => new { x.NhapKhoVatTu.SoChungTu }).Select(g => new { NhapKhoVTChiTiets = g.FirstOrDefault() });
            var results = new List<LookupItemVo>();
            foreach (var item in query)
            {
                var result = new LookupItemVo
                {
                    KeyId = item.NhapKhoVTChiTiets.NhapKhoVatTuId,
                    DisplayName = item.NhapKhoVTChiTiets.NhapKhoVatTu.SoChungTu
                };
                results.Add(result);
            }
            results = results.Take(model.Take).ToList();
            if (!string.IsNullOrEmpty(model.Query))
            {
                results = results.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return results;
        }

        public async Task<List<YeuCauXuatKhoVatTuGridVo>> YeuCauXuatVatTuChiTiets(long yeuCauXuatKhoVatTuId)
        {
            var query = _yeuCauXuatKhoVatTuChiTietRepository.TableNoTracking.Where(z => z.YeuCauXuatKhoVatTuId == yeuCauXuatKhoVatTuId)
                .Select(s => new YeuCauXuatKhoVatTuGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ten = s.VatTuBenhVien.VatTus.Ten,
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    NhomVatTuId = s.VatTuBenhVien.VatTus.NhomVatTuId,
                    TenNhom = s.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = s.VatTuBenhVien.Ma,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    DonGiaNhap = s.DonGiaNhap,
                    KhoXuatId = s.YeuCauXuatKhoVatTu.KhoXuatId,
                    SoLuongXuat = s.SoLuongXuat,

                })
                .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new YeuCauXuatKhoVatTuGridVo
                {
                    Id = g.First().Id,
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    NhomVatTuId = g.First().NhomVatTuId,
                    TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    KhoXuatId = g.First().KhoXuatId,
                    SoLuongXuat = g.Sum(z => z.SoLuongXuat),
                });

            var yeuCauXuatPhamBuGridParentVos = query.ToList();

            var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => yeuCauXuatPhamBuGridParentVos.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT && x.NhapKhoVatTu.KhoId == z.KhoXuatId && x.HanSuDung == z.HanSuDung && x.Solo == z.SoLo) && x.SoLuongNhap > x.SoLuongDaXuat
            //&& x.HanSuDung >= DateTime.Now
            ).ToList();

            var result = yeuCauXuatPhamBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaVatTuBHYT));
            result = result.Select(o =>
            {
                o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            });
            return result.ToList();
        }

        public async Task XuLyThemYeuCauXuatKhoVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu, List<XuatKhoKhacVatTuChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet)
        {
            if (!laLuuDuyet) // chỉ lưu
            {
                if (yeuCauXuatVatTuChiTiets.Any())
                {
                    var vatTuBenhVienIds = yeuCauXuatVatTuChiTiets.Select(o => o.VatTuBenhVienId).ToList();
                    var soLos = yeuCauXuatVatTuChiTiets.Select(o => o.SoLo).ToList();

                    var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(o => o.NhapKhoVatTu.KhoId == yeuCauXuatKhoVatTu.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();

                    foreach (var chiTietVo in yeuCauXuatVatTuChiTiets)
                    {
                        var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                        .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (nhapKhoVatTuChiTietXuats.Count() == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat))
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        var nhapKhoVatTuChiTietXuat = nhapKhoVatTuChiTietXuats.First();
                        var yeuCauXuatKhoVatTuChiTietNew = new YeuCauXuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTietXuat.VatTuBenhVienId,
                            HopDongThauVatTuId = nhapKhoVatTuChiTietXuat.HopDongThauVatTuId,
                            LaVatTuBHYT = nhapKhoVatTuChiTietXuat.LaVatTuBHYT,
                            Solo = nhapKhoVatTuChiTietXuat.Solo,
                            HanSuDung = nhapKhoVatTuChiTietXuat.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoVatTuChiTietXuat.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoVatTuChiTietXuat.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTietXuat.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTietXuat.VAT,
                            MaVach = nhapKhoVatTuChiTietXuat.MaVach,
                            TiLeBHYTThanhToan = nhapKhoVatTuChiTietXuat.TiLeBHYTThanhToan,
                            MaRef = nhapKhoVatTuChiTietXuat.MaRef,
                            SoLuongXuat = chiTietVo.SoLuongXuat.Value
                        };
                        yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.Add(yeuCauXuatKhoVatTuChiTietNew);
                    }
                    _yeuCauXuatKhoVatTuRepository.Add(yeuCauXuatKhoVatTu);
                }
            }
            else
            {
                XuatKhoVatTu xuatKhoVatTu = null;
                if (yeuCauXuatVatTuChiTiets.Any())
                {
                    var vatTuBenhVienIds = yeuCauXuatVatTuChiTiets.Select(o => o.VatTuBenhVienId).ToList();
                    var soLos = yeuCauXuatVatTuChiTiets.Select(o => o.SoLo).ToList();

                    var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.Table
                        .Where(o => o.NhapKhoVatTu.KhoId == yeuCauXuatKhoVatTu.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();
                    //xuat kho
                    xuatKhoVatTu = new XuatKhoVatTu
                    {
                        LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatKhac,
                        KhoXuatId = yeuCauXuatKhoVatTu.KhoXuatId,
                        LyDoXuatKho = yeuCauXuatKhoVatTu.LyDoXuatKho,
                        NguoiXuatId = yeuCauXuatKhoVatTu.NguoiXuatId,
                        NguoiNhanId = yeuCauXuatKhoVatTu.NguoiNhanId,
                        LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                        NgayXuat = yeuCauXuatKhoVatTu.NgayXuat,
                        TraNCC = yeuCauXuatKhoVatTu.TraNCC,
                        NhaThauId = yeuCauXuatKhoVatTu.NhaThauId,
                        SoChungTu = yeuCauXuatKhoVatTu.SoChungTu
                    };


                    foreach (var chiTietVo in yeuCauXuatVatTuChiTiets)
                    {
                        var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                            .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat)
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        double soLuongCanXuat = chiTietVo.SoLuongXuat.Value;
                        while (!soLuongCanXuat.AlmostEqual(0))
                        {
                            // tinh so luong xuat
                            var nhapKhoVatTuChiTiet = nhapKhoVatTuChiTietXuats
                                .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                            var soLuongTon = (nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                            var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                            nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                            var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                            {
                                SoLuongXuat = soLuongXuat,
                                NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                                NgayXuat = yeuCauXuatKhoVatTu.NgayXuat
                            };
                            var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                            {
                                VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                                NgayXuat = yeuCauXuatKhoVatTu.NgayXuat
                            };
                            xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                            xuatKhoVatTu.XuatKhoVatTuChiTiets.Add(xuatKhoVatTuChiTiet);

                            soLuongCanXuat = (soLuongCanXuat - soLuongXuat).MathRoundNumber(2);
                        }
                    }
                    BaseRepository.Add(xuatKhoVatTu);
                }
            }
        }

        public async Task<CapNhatXuatKhoKhacResultVo> XuLyCapNhatYeuCauXuatKhoVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu, List<XuatKhoKhacVatTuChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet)
        {
            foreach (var chiTiet in yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets)
            {
                if (chiTiet.Id != 0)
                {
                    chiTiet.WillDelete = true;
                }
            }
            if (!laLuuDuyet) // chỉ lưu
            {
                if (yeuCauXuatVatTuChiTiets.Any())
                {
                    var vatTuBenhVienIds = yeuCauXuatVatTuChiTiets.Select(o => o.VatTuBenhVienId).ToList();
                    var soLos = yeuCauXuatVatTuChiTiets.Select(o => o.SoLo).ToList();

                    var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(o => o.NhapKhoVatTu.KhoId == yeuCauXuatKhoVatTu.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();

                    foreach (var chiTietVo in yeuCauXuatVatTuChiTiets)
                    {
                        var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                        .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (nhapKhoVatTuChiTietXuats.Count() == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat))
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        var nhapKhoVatTuChiTietXuat = nhapKhoVatTuChiTietXuats.First();
                        var yeuCauXuatKhoVatTuChiTietNew = new YeuCauXuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTietXuat.VatTuBenhVienId,
                            HopDongThauVatTuId = nhapKhoVatTuChiTietXuat.HopDongThauVatTuId,
                            LaVatTuBHYT = nhapKhoVatTuChiTietXuat.LaVatTuBHYT,
                            Solo = nhapKhoVatTuChiTietXuat.Solo,
                            HanSuDung = nhapKhoVatTuChiTietXuat.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoVatTuChiTietXuat.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoVatTuChiTietXuat.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTietXuat.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTietXuat.VAT,
                            MaVach = nhapKhoVatTuChiTietXuat.MaVach,
                            TiLeBHYTThanhToan = nhapKhoVatTuChiTietXuat.TiLeBHYTThanhToan,
                            MaRef = nhapKhoVatTuChiTietXuat.MaRef,
                            SoLuongXuat = chiTietVo.SoLuongXuat.Value
                        };
                        yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.Add(yeuCauXuatKhoVatTuChiTietNew);
                    }
                    _yeuCauXuatKhoVatTuRepository.Update(yeuCauXuatKhoVatTu);
                }
                return new CapNhatXuatKhoKhacResultVo { Id = yeuCauXuatKhoVatTu.Id, LastModified = yeuCauXuatKhoVatTu.LastModified };
            }
            else
            {
                if (yeuCauXuatVatTuChiTiets.Any())
                {
                    var vatTuBenhVienIds = yeuCauXuatVatTuChiTiets.Select(o => o.VatTuBenhVienId).ToList();
                    var soLos = yeuCauXuatVatTuChiTiets.Select(o => o.SoLo).ToList();

                    var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.Table
                        .Where(o => o.NhapKhoVatTu.KhoId == yeuCauXuatKhoVatTu.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();
                    //xuat kho
                    var xuatKhoVatTu = new XuatKhoVatTu
                    {
                        LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatKhac,
                        KhoXuatId = yeuCauXuatKhoVatTu.KhoXuatId,
                        LyDoXuatKho = yeuCauXuatKhoVatTu.LyDoXuatKho,
                        NguoiXuatId = yeuCauXuatKhoVatTu.NguoiXuatId,
                        NguoiNhanId = yeuCauXuatKhoVatTu.NguoiNhanId,
                        LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                        NgayXuat = yeuCauXuatKhoVatTu.NgayXuat,
                        TraNCC = yeuCauXuatKhoVatTu.TraNCC,
                        NhaThauId = yeuCauXuatKhoVatTu.NhaThauId,
                        SoChungTu = yeuCauXuatKhoVatTu.SoChungTu
                    };


                    foreach (var chiTietVo in yeuCauXuatVatTuChiTiets)
                    {
                        var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                            .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat)
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        double soLuongCanXuat = chiTietVo.SoLuongXuat.Value;
                        while (!soLuongCanXuat.AlmostEqual(0))
                        {
                            // tinh so luong xuat
                            var nhapKhoVatTuChiTiet = nhapKhoVatTuChiTietXuats
                                .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                            var soLuongTon = (nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                            var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                            nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                            var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                            {
                                SoLuongXuat = soLuongXuat,
                                NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                                NgayXuat = yeuCauXuatKhoVatTu.NgayXuat
                            };
                            var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                            {
                                VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                                NgayXuat = yeuCauXuatKhoVatTu.NgayXuat
                            };
                            xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                            xuatKhoVatTu.XuatKhoVatTuChiTiets.Add(xuatKhoVatTuChiTiet);

                            soLuongCanXuat = (soLuongCanXuat - soLuongXuat).MathRoundNumber(2);
                        }
                    }
                    yeuCauXuatKhoVatTu.WillDelete = true;
                    BaseRepository.Add(xuatKhoVatTu);
                    return new CapNhatXuatKhoKhacResultVo { Id = xuatKhoVatTu.Id, LastModified = xuatKhoVatTu.LastModified };
                }
                return null;
            }
        }

        public async Task XuLyThemHoacCapNhatVaDuyetYeuCauVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu, List<XuatKhoKhacVatTuChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet, bool isCreate)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            if (!laLuuDuyet) // chỉ lưu
            {
                if (!isCreate)
                {
                    foreach (var chiTiet in yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets)
                    {
                        if (chiTiet.Id != 0)
                        {
                            chiTiet.WillDelete = true;
                        }
                    }
                }
                var khoXuatIds = yeuCauXuatVatTuChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
                var nhapKhoVatTuChiTietAlls = _nhapKhoVatTuChiTietRepository.Table
                    .Include(vt => vt.NhapKhoVatTu)
                    .Where(o => khoXuatIds.Contains(o.NhapKhoVatTu.KhoId)
                    && o.SoLuongNhap > o.SoLuongDaXuat)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var yeuCauXuatVatTuChiTiet in yeuCauXuatVatTuChiTiets)
                {
                    var nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietAlls.Where(o => o.NhapKhoVatTu.KhoId == yeuCauXuatVatTuChiTiet.KhoXuatId
                      && o.LaVatTuBHYT == yeuCauXuatVatTuChiTiet.LaVatTuBHYT
                      && o.VatTuBenhVienId == yeuCauXuatVatTuChiTiet.VatTuBenhVienId
                      && o.HanSuDung == yeuCauXuatVatTuChiTiet.HanSuDung
                      && o.Solo == yeuCauXuatVatTuChiTiet.SoLo
                      && o.SoLuongNhap > o.SoLuongDaXuat)
                      .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                    var SLTon = nhapKhoVatTuChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                    if (SLTon < yeuCauXuatVatTuChiTiet.SoLuongXuat)
                    {
                        throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                    }

                    var soLuongXuat = yeuCauXuatVatTuChiTiet.SoLuongXuat.MathRoundNumber(2);// số lượng xuất

                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                    {
                        if (soLuongXuat == 0)
                        {
                            break;
                        }
                        var yeuCauXuatKhoVatTuChiTietNew = new YeuCauXuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                            HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                            LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT,
                            Solo = nhapKhoVatTuChiTiet.Solo,
                            HanSuDung = nhapKhoVatTuChiTiet.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTiet.VAT,
                            MaVach = nhapKhoVatTuChiTiet.MaVach,
                            TiLeBHYTThanhToan = nhapKhoVatTuChiTiet.TiLeBHYTThanhToan,
                            MaRef = nhapKhoVatTuChiTiet.MaRef,
                        };
                        var SLTonHienTai = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;
                        if (SLTonHienTai > soLuongXuat || SLTonHienTai.AlmostEqual(soLuongXuat.Value))
                        {
                            yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat = soLuongXuat.Value.MathRoundNumber(2);
                            soLuongXuat = 0;
                        }
                        else
                        {
                            yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat = SLTonHienTai.MathRoundNumber(2);
                            soLuongXuat = (soLuongXuat - SLTonHienTai).MathRoundNumber(2);
                        }
                        yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.Add(yeuCauXuatKhoVatTuChiTietNew);
                    }
                }
            }
            else
            {
                if (!isCreate)
                {
                    foreach (var chiTiet in yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets)
                    {
                        if (chiTiet.Id != 0)
                        {
                            chiTiet.WillDelete = true;
                        }
                    }
                }
                var tenNguoiNhan = _nhanVienRepository.TableNoTracking.Where(x => x.Id == yeuCauXuatKhoVatTu.NguoiNhanId).Select(z => z.User.HoTen).FirstOrDefault();
                var xuatKhoVatTu = new XuatKhoVatTu
                {
                    LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatKhac,
                    LyDoXuatKho = yeuCauXuatKhoVatTu.LyDoXuatKho,
                    NguoiNhanId = yeuCauXuatKhoVatTu.NguoiNhanId,
                    TenNguoiNhan = tenNguoiNhan,
                    NguoiXuatId = yeuCauXuatKhoVatTu.NguoiXuatId,
                    LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayXuat = DateTime.Now,
                    KhoXuatId = yeuCauXuatKhoVatTu.KhoXuatId,
                    TraNCC = yeuCauXuatKhoVatTu.TraNCC,
                    NhaThauId = yeuCauXuatKhoVatTu.NhaThauId,
                    SoChungTu = yeuCauXuatKhoVatTu.SoChungTu
                };
                var khoXuatIds = yeuCauXuatVatTuChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
                var nhapKhoVatTuChiTietAlls = _nhapKhoVatTuChiTietRepository.Table
                    .Include(vt => vt.NhapKhoVatTu)
                    .Where(o => khoXuatIds.Contains(o.NhapKhoVatTu.KhoId)
                    && o.SoLuongNhap > o.SoLuongDaXuat)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var yeuCauXuatVatTuChiTiet in yeuCauXuatVatTuChiTiets)
                {
                    var nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietAlls.Where(o => o.NhapKhoVatTu.KhoId == yeuCauXuatVatTuChiTiet.KhoXuatId
                     && o.LaVatTuBHYT == yeuCauXuatVatTuChiTiet.LaVatTuBHYT
                     && o.VatTuBenhVienId == yeuCauXuatVatTuChiTiet.VatTuBenhVienId
                     && o.HanSuDung == yeuCauXuatVatTuChiTiet.HanSuDung
                     && o.Solo == yeuCauXuatVatTuChiTiet.SoLo
                     && o.SoLuongNhap > o.SoLuongDaXuat)
                     .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                    var SLTon = nhapKhoVatTuChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                    if (SLTon < yeuCauXuatVatTuChiTiet.SoLuongXuat)
                    {
                        throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                    }

                    var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                    {
                        VatTuBenhVienId = yeuCauXuatVatTuChiTiet.VatTuBenhVienId,
                        XuatKhoVatTu = xuatKhoVatTu
                    };

                    var soLuongXuat = yeuCauXuatVatTuChiTiet.SoLuongXuat.MathRoundNumber(2);// số lượng xuất

                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                    {
                        if (soLuongXuat == 0)
                        {
                            break;
                        }
                        var yeuCauXuatKhoVatTuChiTietNew = new YeuCauXuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                            HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                            LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT,
                            Solo = nhapKhoVatTuChiTiet.Solo,
                            HanSuDung = nhapKhoVatTuChiTiet.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTiet.VAT,
                            MaVach = nhapKhoVatTuChiTiet.MaVach,
                            TiLeBHYTThanhToan = nhapKhoVatTuChiTiet.TiLeBHYTThanhToan,
                            MaRef = nhapKhoVatTuChiTiet.MaRef,
                        };
                        var SLTonHienTai = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;
                        if (SLTonHienTai > soLuongXuat || SLTonHienTai.AlmostEqual(soLuongXuat.Value))
                        {
                            nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongXuat.Value).MathRoundNumber(2);
                            yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat = soLuongXuat.Value.MathRoundNumber(2);
                            soLuongXuat = 0;
                        }
                        else
                        {
                            nhapKhoVatTuChiTiet.SoLuongDaXuat = nhapKhoVatTuChiTiet.SoLuongNhap.MathRoundNumber(2);
                            yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat = SLTonHienTai.MathRoundNumber(2);
                            soLuongXuat = (soLuongXuat - SLTonHienTai).MathRoundNumber(2);
                        }
                        var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                        {
                            SoLuongXuat = yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat,
                            //GhiChu = "Xuất khác.",
                            GhiChu = yeuCauXuatKhoVatTu.TraNCC == true ? "Trả nhà cung cấp" : "Xuất khác.",
                            XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet,
                            NgayXuat = DateTime.Now,
                            NhapKhoVatTuChiTietId = nhapKhoVatTuChiTiet.Id
                        };
                        yeuCauXuatKhoVatTuChiTietNew.XuatKhoVatTuChiTietViTri = xuatKhoVatTuChiTietViTri;
                        yeuCauXuatKhoVatTuChiTietNew.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet;
                        yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.Add(yeuCauXuatKhoVatTuChiTietNew);
                        await XuLySoLuongTonAsync(nhapKhoVatTuChiTiet);
                    }
                }
            }
            await _yeuCauXuatKhoVatTuRepository.UpdateAsync(yeuCauXuatKhoVatTu);
        }
        private async Task XuLySoLuongTonAsync(NhapKhoVatTuChiTiet nhapKhoVatTuChiTiet)
        {
            await _nhapKhoVatTuChiTietRepository.UpdateAsync(nhapKhoVatTuChiTiet);
        }
        public async Task<XuatKhoKhacVatTuRsVo> XuLyXoaYeuCauVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu)
        {
            foreach (var item in yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets)
            {
                item.WillDelete = true;
            }
            yeuCauXuatKhoVatTu.WillDelete = true;
            await _yeuCauXuatKhoVatTuRepository.UpdateAsync(yeuCauXuatKhoVatTu);
            var xuatKhoKhacDuocPhamRsVo = new XuatKhoKhacVatTuRsVo
            {
                Id = yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTuId.GetValueOrDefault(),
                LastModified = yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.LastModified,
            };
            return xuatKhoKhacDuocPhamRsVo;
        }
        public async Task CheckPhieuYeuCauXuatVTKhacDaDuyetHoacDaHuy(long yeuCauXuatKhoVatTuId)
        {
            var result = await _yeuCauXuatKhoVatTuRepository.TableNoTracking.Where(p => p.Id == yeuCauXuatKhoVatTuId).Select(p => p).FirstOrDefaultAsync();
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
        public async Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long yeuCauXuatKhoVatTuId)
        {
            var yeuCau = await _yeuCauXuatKhoVatTuRepository.TableNoTracking.Where(p => p.Id == yeuCauXuatKhoVatTuId).FirstOrDefaultAsync();
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
        public async Task<List<XuatKhoKhacLookupItem>> GetKhoTheoLoaiVatTu(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var khoId = CommonHelper.GetIdFromRequestDropDownList(model);
            var khos = await _khoRepository.TableNoTracking
                .Where(p =>
                        ((p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                         || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoVacXin
                        || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc)
                        && p.KhoaPhongId == phongBenhVien.KhoaPhongId
                        && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId))
                        && p.LoaiVatTu == true)
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
        public async Task<bool> CheckSoLuongTonVatTu(long vatTuBenhVienId, bool? laVatTuBHYT, long? khoXuatId, double? soLuongXuat, string soLo, DateTime? hanSuDung)
        {
            if (soLuongXuat == null)
            {
                return true;
            }
            var soLuongTonVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == vatTuBenhVienId && o.LaVatTuBHYT == laVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId && o.HanSuDung == hanSuDung && o.Solo == soLo && o.SoLuongNhap > o.SoLuongDaXuat
            //&& o.HanSuDung >= DateTime.Now
            ).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);

            soLuongTonVatTu = soLuongTonVatTu.MathRoundNumber(2);
            if (soLuongXuat > soLuongTonVatTu)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<double> GetSoLuongTonThucTe(YeuCauXuatKhoVatTuGridVo yeuCauXuatKhoVatTuGridVo)
        {
            return await _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(z => z.VatTuBenhVienId == yeuCauXuatKhoVatTuGridVo.VatTuBenhVienId && z.LaVatTuBHYT == yeuCauXuatKhoVatTuGridVo.LaVatTuBHYT && z.Solo == yeuCauXuatKhoVatTuGridVo.SoLo && z.HanSuDung == yeuCauXuatKhoVatTuGridVo.HanSuDung && z.NhapKhoVatTu.KhoId == yeuCauXuatKhoVatTuGridVo.KhoXuatId
            //&& z.HanSuDung >= DateTime.Now 
            && z.SoLuongNhap > z.SoLuongDaXuat).SumAsync(x => x.SoLuongNhap - x.SoLuongDaXuat);
        }
        public async Task<List<LookupItemVo>> GetKhoVatTu(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var khos = _khoRepository.TableNoTracking.Where(p =>
                    (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                    && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
                    && p.LoaiVatTu == true)
                    .ApplyLike(model.Query, p => p.Ten)
                    .Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                    }).Take(model.Take);
            return await khos.ToListAsync();
        }
    }
}
