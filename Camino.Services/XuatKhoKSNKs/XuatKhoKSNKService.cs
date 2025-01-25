using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhoKSNK;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Services.CauHinh;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Services.Helpers;
using System.Globalization;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.XuatKhoKSNKs
{
    [ScopedDependency(ServiceType = typeof(IXuatKhoKSNKService))]
    public class XuatKhoKSNKService : MasterFileService<XuatKhoVatTu>, IXuatKhoKSNKService
    {
        private readonly IRepository<Kho> _khoRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;
        private readonly IRepository<Core.Domain.Entities.XuatKhos.XuatKhoDuocPham> _xuatKhoDuocPhamRepository;
        private readonly IRepository<XuatKhoVatTuChiTiet> _xuatKhoVatTuChiTietRepository;
        private readonly IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;
        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private readonly IRepository<NhapKhoVatTu> _nhapKhoVatTuRepository;
        private readonly IRepository<NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly ICauHinhService _cauHinhService;
        private new readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        private readonly IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhom;


        public XuatKhoKSNKService(IRepository<XuatKhoVatTu> repository, IRepository<Kho> khoRepository
            , IRepository<XuatKhoVatTu> xuatKhoVatTuRepository, IRepository<Core.Domain.Entities.XuatKhos.XuatKhoDuocPham> xuatKhoDuocPhamRepository
            , IRepository<XuatKhoVatTuChiTiet> xuatKhoVatTuChiTietRepository, IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository
            , IRepository<NhapKhoVatTu> nhapKhoVatTuRepository, IRepository<NhapKhoDuocPham> nhapKhoDuocPhamRepository, IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository
            , IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository
            , IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository, IRepository<Template> templateRepository
            , ICauHinhService cauHinhService, IUserAgentHelper userAgentHelper
            , IRepository<VatTuBenhVien> vatTuBenhVienRepository
            , IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository
            , ILocalizationService localizationService
            , IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository
            , IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhom
            , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository) : base(repository)
        {
            _khoRepository = khoRepository;
            _nhanVienRepository = nhanVienRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _xuatKhoVatTuChiTietRepository = xuatKhoVatTuChiTietRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _templateRepository = templateRepository;
            _cauHinhService = cauHinhService;
            _phongBenhVienRepository = phongBenhVienRepository;
            _userAgentHelper = userAgentHelper;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _localizationService = localizationService;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
            _duocPhamBenhVienPhanNhom = duocPhamBenhVienPhanNhom;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var queryObject = new XuatKhoVatTuSearch();

            string searchString = null;
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                searchString = queryInfo.SearchTerms.Replace("\t", "").Trim();
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<XuatKhoVatTuSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    searchString = queryObject.SearchString.Replace("\t", "").Trim();
                }
            }

            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                .Where(p => p.LoaiXuatKho != Enums.EnumLoaiXuatKho.XuatKhac && p.KhoVatTuXuat.LaKhoKSNK == true && (p.KhoVatTuXuat.KhoaPhongId == phongBenhVien.KhoaPhongId))
                .Select(s => new XuatKhoKSNKGridVo
                {
                    Id = s.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                    KhoXuat = s.KhoVatTuXuat != null ? s.KhoVatTuXuat.Ten : "",
                    KhoNhap = s.KhoVatTuNhap != null ? s.KhoVatTuNhap.Ten : "",
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NguoiNhan = s.LoaiNguoiNhan == Enums.LoaiNguoiGiaoNhan.TrongHeThong ? (s.NguoiNhan != null ? (s.NguoiNhan.User != null ? s.NguoiNhan.User.HoTen : s.TenNguoiNhan) : s.TenNguoiNhan) : s.TenNguoiNhan,
                    NguoiXuat = s.NguoiXuat != null ? (s.NguoiXuat.User != null ? s.NguoiXuat.User.HoTen : "") : "",
                    NgayXuat = s.NgayXuat,
                });
            query = query.Union(_xuatKhoDuocPhamRepository.TableNoTracking
                .Where(p => p.LoaiXuatKho != Enums.XuatKhoDuocPham.XuatKhac && p.KhoDuocPhamXuat.LaKhoKSNK == true && (p.KhoDuocPhamXuat.KhoaPhongId == phongBenhVien.KhoaPhongId))
                .Select(s => new XuatKhoKSNKGridVo
                {
                    Id = s.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    KhoXuat = s.KhoDuocPhamXuat != null ? s.KhoDuocPhamXuat.Ten : "",
                    KhoNhap = s.KhoDuocPhamNhap != null ? s.KhoDuocPhamNhap.Ten : "",
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NguoiNhan = s.LoaiNguoiNhan == Enums.LoaiNguoiGiaoNhan.TrongHeThong ? (s.NguoiNhan != null ? (s.NguoiNhan.User != null ? s.NguoiNhan.User.HoTen : s.TenNguoiNhan) : s.TenNguoiNhan) : s.TenNguoiNhan,
                    NguoiXuat = s.NguoiXuat != null ? (s.NguoiXuat.User != null ? s.NguoiXuat.User.HoTen : "") : "",
                    NgayXuat = s.NgayXuat,
                }));

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.ApplyLike(searchString, g => g.KhoXuat, g => g.KhoNhap, g => g.SoPhieu, g => g.NguoiNhan, g => g.NguoiXuat, g => g.LyDoXuatKho);
            }
            if (queryObject != null)
            {
                if (queryObject.RangeXuat != null &&
                               (!string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || !string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay)))
                {                    
                    queryObject.RangeXuat.TuNgay.TryParseExactCustom(out var tuNgay);
                    queryObject.RangeXuat.DenNgay.TryParseExactCustom(out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay) || p.NgayXuat <= denNgay));
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);


            var queryObject = new XuatKhoVatTuSearch();

            string searchString = null;
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                searchString = queryInfo.SearchTerms.Replace("\t", "").Trim();
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<XuatKhoVatTuSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    searchString = queryObject.SearchString.Replace("\t", "").Trim();
                }
            }

            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                .Where(p => p.LoaiXuatKho != Enums.EnumLoaiXuatKho.XuatKhac && p.KhoVatTuXuat.LaKhoKSNK == true && (p.KhoVatTuXuat.KhoaPhongId == phongBenhVien.KhoaPhongId))
                .Select(s => new XuatKhoKSNKGridVo
                {
                    Id = s.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                    KhoXuat = s.KhoVatTuXuat != null ? s.KhoVatTuXuat.Ten : "",
                    KhoNhap = s.KhoVatTuNhap != null ? s.KhoVatTuNhap.Ten : "",
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NguoiNhan = s.LoaiNguoiNhan == Enums.LoaiNguoiGiaoNhan.TrongHeThong ? (s.NguoiNhan != null ? (s.NguoiNhan.User != null ? s.NguoiNhan.User.HoTen : s.TenNguoiNhan) : s.TenNguoiNhan) : s.TenNguoiNhan,
                    NguoiXuat = s.NguoiXuat != null ? (s.NguoiXuat.User != null ? s.NguoiXuat.User.HoTen : "") : "",
                    NgayXuat = s.NgayXuat,
                });
            query = query.Union(_xuatKhoDuocPhamRepository.TableNoTracking
                .Where(p => p.LoaiXuatKho != Enums.XuatKhoDuocPham.XuatKhac && p.KhoDuocPhamXuat.LaKhoKSNK == true && (p.KhoDuocPhamXuat.KhoaPhongId == phongBenhVien.KhoaPhongId))
                .Select(s => new XuatKhoKSNKGridVo
                {
                    Id = s.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    KhoXuat = s.KhoDuocPhamXuat != null ? s.KhoDuocPhamXuat.Ten : "",
                    KhoNhap = s.KhoDuocPhamNhap != null ? s.KhoDuocPhamNhap.Ten : "",
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NguoiNhan = s.LoaiNguoiNhan == Enums.LoaiNguoiGiaoNhan.TrongHeThong ? (s.NguoiNhan != null ? (s.NguoiNhan.User != null ? s.NguoiNhan.User.HoTen : s.TenNguoiNhan) : s.TenNguoiNhan) : s.TenNguoiNhan,
                    NguoiXuat = s.NguoiXuat != null ? (s.NguoiXuat.User != null ? s.NguoiXuat.User.HoTen : "") : "",
                    NgayXuat = s.NgayXuat,
                }));
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.ApplyLike(searchString, g => g.KhoXuat, g => g.KhoNhap, g => g.SoPhieu, g => g.NguoiNhan, g => g.NguoiXuat, g => g.LyDoXuatKho);
            }
            if (queryObject != null)
            {
                if (queryObject.RangeXuat != null &&
                               (!string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || !string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay)))
                {
                    //DateTime.TryParseExact(queryObject.RangeXuat.TuNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    //DateTime.TryParseExact(queryObject.RangeXuat.DenNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    queryObject.RangeXuat.TuNgay.TryParseExactCustom(out var tuNgay);
                    queryObject.RangeXuat.DenNgay.TryParseExactCustom(out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay) || p.NgayXuat <= denNgay));
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridVatTuChildAsync(QueryInfo queryInfo, long? XuatKhoVatTuId = null, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "VatTu", Dir = "asc" } };
            }

            long par = 0;
            if (XuatKhoVatTuId != null && XuatKhoVatTuId != 0)
            {
                par = XuatKhoVatTuId ?? 0;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }


            var query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == par)
                .Select(s => new XuatKhoKSNKChildrenGridVo()
                {
                    Id = s.Id,
                    VatTu = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    LoaiSuDung = s.XuatKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() ?? "CHƯA PHÂN NHÓM",
                    DVT = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    Loai = s.NhapKhoVatTuChiTiet.LaVatTuBHYT ? "BHYT" : "Không BHYT",
                    SoLuongXuat = s.SoLuongXuat.ApplyNumber(),
                    SoPhieu = s.NhapKhoVatTuChiTiet.NhapKhoVatTu.SoPhieu,
                });



            var queryString = queryInfo.AdditionalSearchString;
            //if (!string.IsNullOrEmpty(queryString))
            //{
            //    query = query.ApplyLike(queryString, g => g.VatTu, g => g.DVT, g => g.Loai);
            //}


            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);   
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridVatTuChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == long.Parse(queryInfo.SearchTerms))
                .Select(s => new XuatKhoKSNKChildrenGridVo()
                {
                    Id = s.Id,
                    VatTu = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    LoaiSuDung = s.XuatKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() ?? "CHƯA PHÂN NHÓM",
                    DVT = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    Loai = s.NhapKhoVatTuChiTiet.LaVatTuBHYT ? "BHYT" : "Không BHYT",
                    SoLuongXuat = s.SoLuongXuat.ApplyFormatMoneyToDouble(false),
                    SoPhieu = s.NhapKhoVatTuChiTiet.NhapKhoVatTu.SoPhieu,
                });



            var queryString = queryInfo.AdditionalSearchString;

            if (!string.IsNullOrEmpty(queryString))
            {
                query = query.ApplyLike(queryString, g => g.VatTu, g => g.DVT, g => g.Loai);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridDuocPhamChildAsync(QueryInfo queryInfo, long? XuatKhoDuocPhamId, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "DuocPham", Dir = "asc" } };
            }

            long par = 0;
            if (XuatKhoDuocPhamId != null && XuatKhoDuocPhamId != 0)
            {
                par = XuatKhoDuocPhamId ?? 0;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }

            var query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == par && x.SoLuongXuat > 0)
                .Select(s => new XuatKhoDuocPhamChildrenGridVo()
                {
                    Id = s.Id,
                    DuocPham = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.Ten,
                    LoaiSuDung = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    DVT = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    Loai = s.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT ? "BHYT" : "Không BHYT",
                    SoLuongXuat = s.SoLuongXuat.ApplyNumber(),
                    SoPhieu = s.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.SoPhieu,
                });



            var queryString = queryInfo.AdditionalSearchString;

            //if (!string.IsNullOrEmpty(queryString))
            //{
            //    query = query.ApplyLike(queryString, g => g.DuocPham, g => g.DVT, g => g.Loai);
            //}

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridDuocPhamChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == long.Parse(queryInfo.SearchTerms) && x.SoLuongXuat > 0)
                .Select(s => new XuatKhoDuocPhamChildrenGridVo()
                {
                    Id = s.Id,
                    DuocPham = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.Ten,
                    LoaiSuDung = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    DVT = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    Loai = s.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT ? "BHYT" : "Không BHYT",
                    SoLuongXuat = s.SoLuongXuat.ApplyFormatMoneyToDouble(false),
                });
            var queryString = queryInfo.AdditionalSearchString;

            if (!string.IsNullOrEmpty(queryString))
            {
                query = query.ApplyLike(queryString, g => g.DuocPham, g => g.DVT, g => g.Loai);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
                             
        public async Task<bool> IsKhoExists(long id)
        {
            var kho = await _khoRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return kho != null;
        }

        public async Task<bool> IsValidateUpdateOrRemove(long id)
        {
            var result = true;
            var nhapKhoEntity = await _nhapKhoVatTuRepository.TableNoTracking
                                                .Include(p => p.NhapKhoVatTuChiTiets)
                                                .FirstOrDefaultAsync(p => p.XuatKhoVatTuId == id);
            if (nhapKhoEntity != null && nhapKhoEntity.NhapKhoVatTuChiTiets != null)
            {
                if (nhapKhoEntity.NhapKhoVatTuChiTiets.Any(p => p.SoLuongDaXuat != 0))
                {
                    result = false;
                }
            }
            return result;
        }

        public async Task DeleteXuatKho(XuatKhoVatTu entity)
        {
            _nhapKhoVatTuChiTietRepository.AutoCommitEnabled = false;
            BaseRepository.AutoCommitEnabled = false;

            foreach (var item in entity.XuatKhoVatTuChiTiets)
            {
                foreach (var viTri in item.XuatKhoVatTuChiTietViTris)
                {
                    var nhapKhoChiTietRevert = await _nhapKhoVatTuChiTietRepository.Table.FirstOrDefaultAsync(p =>
                            p.Id == viTri.NhapKhoVatTuChiTietId);
                    if (nhapKhoChiTietRevert != null)
                    {
                        nhapKhoChiTietRevert.SoLuongDaXuat = nhapKhoChiTietRevert.SoLuongDaXuat - viTri.SoLuongXuat;
                        await _nhapKhoVatTuChiTietRepository.UpdateAsync(nhapKhoChiTietRevert);
                    }
                }
            }

            await BaseRepository.DeleteAsync(entity);
            await BaseRepository.Context.SaveChangesAsync();
        }

        public async Task<XuatKhoVatTuChiTiet> GetVatTu(ThemKSNK model)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var result = new XuatKhoVatTuChiTiet();
            result.VatTuBenhVienId = model.VatTuBenhVienId ?? 0;
            var vatTuBenhVien = await _vatTuBenhVienRepository.TableNoTracking
                    .Include(p => p.NhapKhoVatTuChiTiets)
                    .ThenInclude(p => p.NhapKhoVatTu)
                .FirstOrDefaultAsync(p => p.Id == model.VatTuBenhVienId);
            var soLuongXuat = model.SoLuongXuat;
            long tempId = 0;
            var loaiSuDung = model.LoaiSuDung;
            foreach (var item in vatTuBenhVien.NhapKhoVatTuChiTiets.Where(p => p.LaVatTuBHYT == model.LaVatTuBHYT
            && p.VatTuBenhVien.LoaiSuDung == loaiSuDung
            && p.NhapKhoVatTu.KhoId == (model.KhoId ?? tempId)).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung))
            {
                //var isDatChatLuong = model.ChatLuong == 1;
                //if (item.DatChatLuong != isDatChatLuong) continue;
                if (soLuongXuat == null || soLuongXuat == 0) break;
                var soLuongCon = item.SoLuongNhap - item.SoLuongDaXuat;
                if (soLuongXuat < soLuongCon)
                {
                    var chiTietViTri = new XuatKhoVatTuChiTietViTri
                    {
                        SoLuongXuat = soLuongXuat ?? 0,
                        NhapKhoVatTuChiTietId = item.Id,
                    };
                    result.XuatKhoVatTuChiTietViTris.Add(chiTietViTri);
                    soLuongXuat = 0;
                    break;
                }
                else if (soLuongCon == 0)
                {
                    continue;
                }
                else if (soLuongCon < 0)
                {
                    continue;
                }
                else
                {
                    soLuongXuat = soLuongXuat - soLuongCon;
                    var chiTietViTri = new XuatKhoVatTuChiTietViTri
                    {
                        SoLuongXuat = item.SoLuongNhap - item.SoLuongDaXuat,
                        NhapKhoVatTuChiTietId = item.Id,
                    };
                    result.XuatKhoVatTuChiTietViTris.Add(chiTietViTri);
                }
            }

            if (soLuongXuat != 0)
            {
                return null;
            }

            return result;
        }

        public async Task<NhapKhoVatTuChiTiet> GetNhapKhoVatTuChiTietById(long id)
        {
            var result = await _nhapKhoVatTuChiTietRepository.TableNoTracking
               .Include(p => p.KhoViTri)
               .FirstOrDefaultAsync(p => p.Id == id);
            return result;
        }

        public async Task<GridDataSource> GetAllDpVtKsnkData(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;
            long khoNhapId = 0;
            var lstDaChon = new List<DaSuaSoLuongXuat>();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
                lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|")[2]);
                if (queryInfo.AdditionalSearchString.Split("|").Length > 3)
                {
                    long.TryParse(queryInfo.AdditionalSearchString.Split("|")[3], out khoNhapId);
                }
            }

            var allDataNhapDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT
            }).GroupBy(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT,
                }, o => o,
                (k, v) => new DpVtKsnkXuatGridVo
                {
                    DuocPhamVatTuId = k.DuocPhamBenhVienId,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    DonGia = k.DonGiaTonKho,
                    SoLo = k.Solo,
                    HanSuDung = k.HanSuDung,
                    LaDpVtBHYT = k.LaDuocPhamBHYT
                }).Where(o=>o.SoLuongTon >= 0.01).ToList();

            var allDataNhapVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.VatTuBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaVatTuBHYT
                }).GroupBy(o => new
                    {
                        o.VatTuBenhVienId,
                        o.DonGiaTonKho,
                        o.Solo,
                        o.HanSuDung,
                        o.LaVatTuBHYT,
                    }, o => o,
                    (k, v) => new DpVtKsnkXuatGridVo
                    {
                        DuocPhamVatTuId = k.VatTuBenhVienId,
                        LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                        SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                        DonGia = k.DonGiaTonKho,
                        SoLo = k.Solo,
                        HanSuDung = k.HanSuDung,
                        LaDpVtBHYT = k.LaVatTuBHYT
                    }).Where(o => o.SoLuongTon >= 0.01).ToList();

            var duocPhamBenhVienIds = allDataNhapDuocPham.Select(o => o.DuocPhamVatTuId).Distinct().ToList();
            var vatTuBenhVienIds = allDataNhapVatTu.Select(o => o.DuocPhamVatTuId).Distinct().ToList();

            var thongTinDuocPhams = _duocPhamBenhVienRepository.TableNoTracking
                .Where(o => duocPhamBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.DuocPham.Ten,
                    Nhom = (o.DuocPhamBenhVienPhanNhom != null ? o.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM"),
                    DVT = o.DuocPham.DonViTinh.Ten
                }).ToList();
            var thongTinVatTus = _vatTuBenhVienRepository.TableNoTracking
                .Where(o => vatTuBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.VatTus.Ten,
                    NhomVatTuId = o.VatTus.NhomVatTuId,
                    Nhom = o.VatTus.NhomVatTu.Ten,
                    DVT = o.VatTus.DonViTinh
                }).ToList();

            var khos = _khoRepository.TableNoTracking.ToList();            
            var khoXuat = khos.FirstOrDefault(o => o.Id == khoXuatId);
            var khoNhap = khos.FirstOrDefault(o => o.Id == khoNhapId);
            var xuatVTHanhChinh = khoXuat != null && khoNhap != null && khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh && khoNhap.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe;

            List<string> lstId = new List<string>();
            if (!string.IsNullOrEmpty(lstIdString))
            {
                lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
            }
            var allData = new List<DpVtKsnkXuatGridVo>();
            foreach (var dpVtKsnkXuatGridVo in allDataNhapDuocPham)
            {
                var thongTinDuocPham = thongTinDuocPhams.First(o => o.Id == dpVtKsnkXuatGridVo.DuocPhamVatTuId);
                dpVtKsnkXuatGridVo.MaVatTu = thongTinDuocPham.Ma;
                dpVtKsnkXuatGridVo.TenVatTu = thongTinDuocPham.Ten;
                dpVtKsnkXuatGridVo.TenNhom = thongTinDuocPham.Nhom;
                dpVtKsnkXuatGridVo.DVT = thongTinDuocPham.DVT;
                if (string.IsNullOrEmpty(queryInfo.SearchTerms) ||
                    (dpVtKsnkXuatGridVo.TenVatTu != null && dpVtKsnkXuatGridVo.TenVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.MaVatTu != null && dpVtKsnkXuatGridVo.MaVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.SoLo != null && dpVtKsnkXuatGridVo.SoLo.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())))
                {
                    if(!lstId.Contains(dpVtKsnkXuatGridVo.Id))
                        allData.Add(dpVtKsnkXuatGridVo);
                }
            }

            foreach (var dpVtKsnkXuatGridVo in allDataNhapVatTu)
            {
                var thongTinVatTu = thongTinVatTus.First(o => o.Id == dpVtKsnkXuatGridVo.DuocPhamVatTuId);
                dpVtKsnkXuatGridVo.MaVatTu = thongTinVatTu.Ma;
                dpVtKsnkXuatGridVo.TenVatTu = thongTinVatTu.Ten;
                dpVtKsnkXuatGridVo.TenNhom = thongTinVatTu.Nhom;
                dpVtKsnkXuatGridVo.DVT = thongTinVatTu.DVT;
                if (string.IsNullOrEmpty(queryInfo.SearchTerms) ||
                    (dpVtKsnkXuatGridVo.TenVatTu != null && dpVtKsnkXuatGridVo.TenVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.MaVatTu != null && dpVtKsnkXuatGridVo.MaVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.SoLo != null && dpVtKsnkXuatGridVo.SoLo.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())))
                {
                    if (!lstId.Contains(dpVtKsnkXuatGridVo.Id) && (xuatVTHanhChinh == false || thongTinVatTu.NhomVatTuId == (long)EnumNhomVatTu.NhomHanhChinh))
                        allData.Add(dpVtKsnkXuatGridVo);
                }
            }

            var dataReturn = allData.OrderBy(o=>o.MaVatTu).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            var stt = 1;
            foreach (var item in dataReturn)
            {
                item.SoLuongXuat = item.SoLuongTon;
                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = dataReturn, TotalRowCount = allData.Count };
        }

        //public async Task<GridDataSource> GetAllVatTuData(QueryInfo queryInfo)
        //{
        //    BuildDefaultSortExpression(queryInfo);

        //    var lstIdString = string.Empty;
        //    long khoXuatId = 0;
        //    var lstDaChon = new List<DaSuaSoLuongXuat>();

        //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {
        //        lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
        //        long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
        //        lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|")[2]);

        //    }
        //    var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
        //    var lstNhom = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.SoLuongNhap - p.SoLuongDaXuat > 0
        //            && p.NhapKhoVatTu.Kho.Id == khoXuatId
        //            )
        //        .Select(p => new
        //        {
        //            name = p.VatTuBenhVien.LoaiSuDung.GetDescription(),
        //            key = p.VatTuBenhVien.LoaiSuDung
        //        }).Distinct().OrderBy(p => p.name).ToList();

        //    var query = _vatTuBenhVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new VatTuXuatGridVo { }).AsQueryable();

        //    foreach (var nhom in lstNhom)
        //    {
        //        var queryCoBHYT = _vatTuBenhVienRepository.TableNoTracking
        //            .Include(p => p.NhapKhoVatTuChiTiets).ThenInclude(p => p.KhoViTri).ThenInclude(p => p.KhoDuocPham)
        //            .Where(p => p.NhapKhoVatTuChiTiets.Any(x => x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
        //                            && x.HanSuDung >= DateTime.Now
        //                            && x.NhapKhoVatTu.Kho.Id == khoXuatId
        //                            && x.VatTuBenhVien.LoaiSuDung == nhom.key))
        //            .Select(s => new VatTuXuatGridVo
        //            {
        //                Id = s.Id + "," + nhom.key + "," + "true",
        //                TenVatTu = s.VatTus.Ten,
        //                DVT = s.VatTus.DonViTinh,
        //                LaVatTuBHYT = true,

        //                LoaiSuDung = nhom.key,
        //                LoaiSuDungDisplay = nhom.name + "",

        //                MaVatTu = s.Ma,

        //                SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == true
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.Solo).FirstOrDefault(),

        //                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == true
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.HanSuDung).FirstOrDefault(),

        //                DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == true
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.DonGiaNhap).FirstOrDefault(),

        //            });
        //        queryCoBHYT = queryCoBHYT.ApplyLike(queryInfo.SearchTerms, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);

        //        var queryKhongBHYT = _vatTuBenhVienRepository.TableNoTracking
        //            .Include(p => p.NhapKhoVatTuChiTiets).ThenInclude(p => p.KhoViTri).ThenInclude(p => p.KhoDuocPham)
        //            .Where(p => p.NhapKhoVatTuChiTiets.Any(x => !x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
        //                            && x.HanSuDung >= DateTime.Now
        //                            && x.NhapKhoVatTu.Kho.Id == khoXuatId
        //                            && x.VatTuBenhVien.LoaiSuDung == nhom.key))
        //            .Select(s => new VatTuXuatGridVo
        //            {
        //                Id = s.Id + "," + nhom.key + "," + "false",
        //                TenVatTu = s.VatTus.Ten,
        //                DVT = s.VatTus.DonViTinh,
        //                LaVatTuBHYT = false,

        //                LoaiSuDung = nhom.key,
        //                LoaiSuDungDisplay = nhom.name + "",

        //                MaVatTu = s.Ma,
        //                SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == false
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.Solo).FirstOrDefault(),

        //                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == false
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.HanSuDung).FirstOrDefault(),

        //                DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == false
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.DonGiaNhap).FirstOrDefault(),
        //            });
        //        queryKhongBHYT = queryKhongBHYT.ApplyLike(queryInfo.SearchTerms, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);

        //        query = query.Concat(queryCoBHYT).Concat(queryKhongBHYT);
        //    }

        //    if (!string.IsNullOrEmpty(lstIdString))
        //    {
        //        var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
        //        query = query.Where(p => !lstId.Contains(p.Id));
        //    }

        //    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
        //    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
        //        .Take(queryInfo.Take).ToArrayAsync();
        //    await Task.WhenAll(countTask, queryTask);

        //    var stt = 1;
        //    foreach (var item in queryTask.Result)
        //    {
        //        //
        //        var id = long.Parse(item.Id.Split(",")[0]);
        //        var nhapKho = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.LaVatTuBHYT == item.LaVatTuBHYT && x.VatTuBenhVienId == id
        //        //&& x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 
        //        && x.NhapKhoVatTu.Kho.Id == khoXuatId
        //        && x.VatTuBenhVien.LoaiSuDung == item.LoaiSuDung);

        //        item.SoLuongTon = nhapKho.Sum(o => o.SoLuongNhap) - nhapKho.Sum(o => o.SoLuongDaXuat);
        //        if (lstDaChon.Any(p => p.Id == item.Id))
        //        {
        //            item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
        //        }
        //        else
        //        {
        //            item.SoLuongXuat = item.SoLuongTon;
        //        }
        //        //
        //        item.STT = stt;
        //        stt++;
        //    }

        //    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        //}

        //public async Task<GridDataSource> GetAllVatTuTotal(QueryInfo queryInfo)
        //{
        //    BuildDefaultSortExpression(queryInfo);

        //    var lstIdString = string.Empty;
        //    long khoXuatId = 0;

        //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {
        //        lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
        //        long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
        //    }

        //    //if (khoXuatId == 0)
        //    //{
        //    //    return new GridDataSource { Data = null, TotalRowCount = 0 };
        //    //}

        //    var lstNhom = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.SoLuongNhap - p.SoLuongDaXuat > 0
        //            //&& p.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1
        //            && p.NhapKhoVatTu.Kho.Id == khoXuatId
        //            )
        //        .Select(p => new
        //        {
        //            name = p.VatTuBenhVien.LoaiSuDung.GetDescription(),
        //            key = p.VatTuBenhVien.LoaiSuDung
        //        }).Distinct().OrderBy(p => p.name).ToList();

        //    var query = _vatTuBenhVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new VatTuXuatGridVo { }).AsQueryable();
        //    var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

        //    foreach (var nhom in lstNhom)
        //    {
        //        var queryCoBHYT = _vatTuBenhVienRepository.TableNoTracking
        //            .Include(p => p.NhapKhoVatTuChiTiets).ThenInclude(p => p.KhoViTri).ThenInclude(p => p.KhoDuocPham)
        //            .Where(p => p.NhapKhoVatTuChiTiets.Any(x => x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
        //                            && x.HanSuDung >= DateTime.Now
        //                            && x.NhapKhoVatTu.Kho.Id == khoXuatId
        //                            && x.VatTuBenhVien.LoaiSuDung == nhom.key))
        //            .Select(s => new VatTuXuatGridVo
        //            {
        //                Id = s.Id + "," + nhom.key + "," + "true",
        //                TenVatTu = s.VatTus.Ten,
        //                DVT = s.VatTus.DonViTinh,
        //                LaVatTuBHYT = true,
        //                LoaiSuDung = nhom.key,
        //                LoaiSuDungDisplay = nhom.name + "",
        //                MaVatTu = s.Ma,
        //                SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == true
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.Solo).FirstOrDefault(),

        //                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == true
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.HanSuDung).FirstOrDefault(),

        //                DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == true
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.DonGiaNhap).FirstOrDefault(),

        //            });
        //        queryCoBHYT = queryCoBHYT.ApplyLike(queryInfo.SearchTerms, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);

        //        var queryKhongBHYT = _vatTuBenhVienRepository.TableNoTracking
        //            .Include(p => p.NhapKhoVatTuChiTiets).ThenInclude(p => p.KhoViTri).ThenInclude(p => p.KhoDuocPham)
        //            .Where(p => p.NhapKhoVatTuChiTiets.Any(x => !x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
        //                            && x.HanSuDung >= DateTime.Now
        //                            && x.NhapKhoVatTu.Kho.Id == khoXuatId
        //                            && x.VatTuBenhVien.LoaiSuDung == nhom.key))
        //            .Select(s => new VatTuXuatGridVo
        //            {
        //                Id = s.Id + "," + nhom.key + "," + "false",
        //                TenVatTu = s.VatTus.Ten,
        //                DVT = s.VatTus.DonViTinh,
        //                LaVatTuBHYT = false,

        //                LoaiSuDung = nhom.key,
        //                LoaiSuDungDisplay = nhom.name + "",

        //                MaVatTu = s.Ma,
        //                SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == false
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.Solo).FirstOrDefault(),

        //                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == false
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.HanSuDung).FirstOrDefault(),

        //                DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == false
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.DonGiaNhap).FirstOrDefault(),
        //            });
        //        queryKhongBHYT = queryKhongBHYT.ApplyLike(queryInfo.SearchTerms, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);

        //        query = query.Concat(queryCoBHYT).Concat(queryKhongBHYT);
        //    }

        //    if (!string.IsNullOrEmpty(lstIdString))
        //    {
        //        var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
        //        query = query.Where(p => !lstId.Contains(p.Id));
        //    }

        //    var countTask = query.CountAsync();
        //    await Task.WhenAll(countTask);
        //    return new GridDataSource { TotalRowCount = countTask.Result };
        //}

        public async Task<List<DpVtKsnkXuatGridVo>> GetDpVtOnGroup(string tenNhom, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon)
        {
            var allDataNhapDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT
                }).GroupBy(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT,
                }, o => o,
                (k, v) => new DpVtKsnkXuatGridVo
                {
                    DuocPhamVatTuId = k.DuocPhamBenhVienId,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    DonGia = k.DonGiaTonKho,
                    SoLo = k.Solo,
                    HanSuDung = k.HanSuDung,
                    LaDpVtBHYT = k.LaDuocPhamBHYT
                }).Where(o => o.SoLuongTon >= 0.01).ToList();

            var allDataNhapVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.VatTuBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaVatTuBHYT
                }).GroupBy(o => new
                {
                    o.VatTuBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaVatTuBHYT,
                }, o => o,
                    (k, v) => new DpVtKsnkXuatGridVo
                    {
                        DuocPhamVatTuId = k.VatTuBenhVienId,
                        LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                        SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                        DonGia = k.DonGiaTonKho,
                        SoLo = k.Solo,
                        HanSuDung = k.HanSuDung,
                        LaDpVtBHYT = k.LaVatTuBHYT
                    }).Where(o => o.SoLuongTon >= 0.01).ToList();

            var duocPhamBenhVienIds = allDataNhapDuocPham.Select(o => o.DuocPhamVatTuId).Distinct().ToList();
            var vatTuBenhVienIds = allDataNhapVatTu.Select(o => o.DuocPhamVatTuId).Distinct().ToList();

            var thongTinDuocPhams = _duocPhamBenhVienRepository.TableNoTracking
                .Where(o => duocPhamBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.DuocPham.Ten,
                    Nhom = (o.DuocPhamBenhVienPhanNhom != null ? o.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM"),
                    DVT = o.DuocPham.DonViTinh.Ten
                }).ToList();
            var thongTinVatTus = _vatTuBenhVienRepository.TableNoTracking
                .Where(o => vatTuBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.VatTus.Ten,
                    Nhom = o.VatTus.NhomVatTu.Ten,
                    DVT = o.VatTus.DonViTinh
                }).ToList();
            
            var dataReturn = new List<DpVtKsnkXuatGridVo>();
            foreach (var dpVtKsnkXuatGridVo in allDataNhapDuocPham)
            {
                var thongTinDuocPham = thongTinDuocPhams.First(o => o.Id == dpVtKsnkXuatGridVo.DuocPhamVatTuId);
                dpVtKsnkXuatGridVo.MaVatTu = thongTinDuocPham.Ma;
                dpVtKsnkXuatGridVo.TenVatTu = thongTinDuocPham.Ten;
                dpVtKsnkXuatGridVo.TenNhom = thongTinDuocPham.Nhom;
                dpVtKsnkXuatGridVo.DVT = thongTinDuocPham.DVT;
                if (string.IsNullOrEmpty(searchString) ||
                    (dpVtKsnkXuatGridVo.TenVatTu != null && dpVtKsnkXuatGridVo.TenVatTu.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.MaVatTu != null && dpVtKsnkXuatGridVo.MaVatTu.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.SoLo != null && dpVtKsnkXuatGridVo.SoLo.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())))
                {
                    //if (!lstId.Contains(dpVtKsnkXuatGridVo.Id))
                    if (dpVtKsnkXuatGridVo.TenNhom == tenNhom)
                        dataReturn.Add(dpVtKsnkXuatGridVo);
                }
            }

            foreach (var dpVtKsnkXuatGridVo in allDataNhapVatTu)
            {
                var thongTinVatTu = thongTinVatTus.First(o => o.Id == dpVtKsnkXuatGridVo.DuocPhamVatTuId);
                dpVtKsnkXuatGridVo.MaVatTu = thongTinVatTu.Ma;
                dpVtKsnkXuatGridVo.TenVatTu = thongTinVatTu.Ten;
                dpVtKsnkXuatGridVo.TenNhom = thongTinVatTu.Nhom;
                dpVtKsnkXuatGridVo.DVT = thongTinVatTu.DVT;
                if (string.IsNullOrEmpty(searchString) ||
                    (dpVtKsnkXuatGridVo.TenVatTu != null && dpVtKsnkXuatGridVo.TenVatTu.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.MaVatTu != null && dpVtKsnkXuatGridVo.MaVatTu.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.SoLo != null && dpVtKsnkXuatGridVo.SoLo.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())))
                {
                    //if (!lstId.Contains(dpVtKsnkXuatGridVo.Id))
                    if (dpVtKsnkXuatGridVo.TenNhom == tenNhom)
                        dataReturn.Add(dpVtKsnkXuatGridVo);
                }
            }

            foreach (var item in dataReturn)
            {
                item.SoLuongXuat = item.SoLuongTon;
                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
            }
            return dataReturn;
        }
        public async Task<List<VatTuXuatGridVo>> GetVatTuOnGroup(Enums.LoaiSuDung? groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            var queryCoBHYT = _vatTuBenhVienRepository.TableNoTracking
                    .Where(p => p.NhapKhoVatTuChiTiets.Any(x => x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
                                    && x.HanSuDung >= DateTime.Now
                                    && x.NhapKhoVatTu.Kho.Id == khoXuatId
                                    && x.VatTuBenhVien.LoaiSuDung == groupId))
                    .Select(s => new VatTuXuatGridVo
                    {
                        Id = s.Id + "," + groupId + "," + "true",
                        TenVatTu = s.VatTus.Ten,
                        DVT = s.VatTus.DonViTinh,
                        LaVatTuBHYT = true,

                        LoaiSuDung = groupId,
                        LoaiSuDungDisplay = groupId.GetDescription(),

                        MaVatTu = s.Ma,
                        SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == true
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         && nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.Solo).FirstOrDefault(),

                        HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == true
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         && nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.HanSuDung).FirstOrDefault(),

                        DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == true
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         && nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.DonGiaNhap).FirstOrDefault(),
                    });
            //gap qua nen cheat
            if (searchString != "undefined")
            {
                queryCoBHYT = queryCoBHYT.ApplyLike(searchString, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);
            }

            var queryKhongBHYT = _vatTuBenhVienRepository.TableNoTracking
                .Where(p => p.NhapKhoVatTuChiTiets.Any(x => !x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
                                && x.HanSuDung >= DateTime.Now
                                && x.NhapKhoVatTu.Kho.Id == khoXuatId
                                && x.VatTuBenhVien.LoaiSuDung == groupId))
                .Select(s => new VatTuXuatGridVo
                {
                    Id = s.Id + "," + groupId + "," + "false",
                    TenVatTu = s.VatTus.Ten,
                    DVT = s.VatTus.DonViTinh,
                    LaVatTuBHYT = false,

                    LoaiSuDung = groupId,
                    LoaiSuDungDisplay = groupId.GetDescription(),

                    MaVatTu = s.Ma,
                    SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == false
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         && nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.Solo).FirstOrDefault(),

                    HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                     && nkct.LaVatTuBHYT == false
                                                                     && nkct.VatTuBenhVienId == s.Id
                                                                     && nkct.HanSuDung >= DateTime.Now
                                                                     && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.HanSuDung).FirstOrDefault(),

                    DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                     && nkct.LaVatTuBHYT == false
                                                                     && nkct.VatTuBenhVienId == s.Id
                                                                     && nkct.HanSuDung >= DateTime.Now
                                                                     && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.DonGiaNhap).FirstOrDefault(),
                });
            //gap qua nen cheat
            if (searchString != "undefined")
            {
                queryKhongBHYT = queryKhongBHYT.ApplyLike(searchString, g => g.TenVatTu, g => g.DVT);
            }

            var query = queryCoBHYT.Concat(queryKhongBHYT).ToList();

            foreach (var item in query)
            {
                //
                var vatTuId = long.Parse(item.Id.Split(",")[0]);
                //
                var nhapKho = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.LaVatTuBHYT == item.LaVatTuBHYT && x.VatTuBenhVienId == vatTuId
                && x.NhapKhoVatTu.Kho.Id == khoXuatId && x.VatTuBenhVien.LoaiSuDung == groupId);

                item.SoLuongTon = nhapKho.Sum(o => o.SoLuongNhap) - nhapKho.Sum(o => o.SoLuongDaXuat);

                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
                else
                {
                    item.SoLuongXuat = item.SoLuongTon;
                }
            }

            return query;
        }

        public async Task<List<LookupItemVo>> GetKhoKSNK(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            //danh sách kho KSNK cũ dc phép xuất đi
            //7	Kho Vật Tư y tế
            //251 Kho BP KSNK
            //168 VTYT TDCN
            //169 VTYT CĐHA
            //175 VTYT tủ Khoa GMHS
            //247 VTYT tủ Khoa Khám Bệnh
            //174 VTYT tủ Khoa Ngoại
            //172 VTYT tủ Khoa Nhi
            //163 VTYT tủ Khoa Nội
            //233 VTYT Tủ khoa sản
            //236 Kho VTYT Thẩm mỹ
            //170 VTYT tủ khoa Xét Nghiệm
            var khoKSNKCuIds = new List<long>() { 7, 251, 168, 169, 175, 247, 174, 172, 163, 233, 236, 170 };
            var khos = _khoRepository.TableNoTracking.Where(p => (p.LaKhoKSNK == true || khoKSNKCuIds.Contains(p.Id)) && p.KhoaPhongId == phongBenhVien.KhoaPhongId && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId))
                .ApplyLike(model.Query, p => p.Ten)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                })
                .Take(model.Take);
            return await khos.ToListAsync();
        }

        private long GetKhoKSNKXuatTuKhoKSNKCu(long khoKSNKCuId)
        {
            //7   Kho Vật Tư y tế             300 Kho Hành Chính
            //251 Kho BP KSNK                 301 Kho KSNK
            //168 VTYT TDCN                   310 KSNK tủ khoa CĐHA-TDCN
            //169 VTYT CĐHA                  310 KSNK tủ khoa CĐHA-TDCN
            //175 VTYT tủ Khoa GMHS           307 KSNK tủ khoa GMHS
            //247 VTYT tủ Khoa Khám Bệnh      308 KSNK tủ khoa Khám bệnh
            //174 VTYT tủ Khoa Ngoại          303 KSNK tủ khoa Ngoại
            //172 VTYT tủ Khoa Nhi            304 KSNK tủ khoa Nhi
            //163 VTYT tủ Khoa Nội            302 KSNK tủ khoa Nội
            //233 VTYT Tủ khoa sản            305 KSNK tủ khoa Sản
            //236 Kho VTYT Thẩm mỹ            306 KSNK tủ khoa Thẩm mỹ
            //170 VTYT tủ khoa Xét Nghiệm     309 KSNK tủ khoa Xét nghiệm
            switch (khoKSNKCuId)
            {
                case 7:
                    return 300;
                case 251:
                    return 301;
                case 168:
                    return 310;
                case 169:
                    return 310;
                case 175:
                    return 307;
                case 247:
                    return 308;
                case 174:
                    return 303;
                case 172:
                    return 304;
                case 163:
                    return 302;
                case 233:
                    return 305;
                case 236:
                    return 306;
                case 170:
                    return 309;
            }
            return 0;
        } 

        public async Task<XuatKhoKsnkResultVo> XuatKhoKsnk(ThongTinXuatKhoKsnkVo thongTinXuatKhoKsnkVo)
        {
            foreach(var xuatKhoKsnkChiTiet in thongTinXuatKhoKsnkVo.ThongTinXuatKhoKsnkChiTietVos)
            {
                var thongTinChiTiet = xuatKhoKsnkChiTiet.Id.Split(',');
                xuatKhoKsnkChiTiet.DuocPhamVatTuId = long.Parse(thongTinChiTiet[0]);
                xuatKhoKsnkChiTiet.LoaiDuocPhamVatTu = int.Parse(thongTinChiTiet[1]) == (int)Enums.LoaiDuocPhamVatTu.LoaiVatTu ? Enums.LoaiDuocPhamVatTu.LoaiVatTu : Enums.LoaiDuocPhamVatTu.LoaiDuocPham;
                xuatKhoKsnkChiTiet.LaDpVtBHYT = thongTinChiTiet[2] == "1";
                xuatKhoKsnkChiTiet.HanSuDung = DateTime.ParseExact(thongTinChiTiet[3], "yyyyMMdd", null);
                xuatKhoKsnkChiTiet.SoLo = thongTinChiTiet[4];
            }
            Core.Domain.Entities.XuatKhos.XuatKhoDuocPham xuatKhoDuocPham = null;
            XuatKhoVatTu xuatKhoVatTu = null;
            var xuatKhoDuocPhamChiTietVos = thongTinXuatKhoKsnkVo.ThongTinXuatKhoKsnkChiTietVos.Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham);
            if (xuatKhoDuocPhamChiTietVos.Any())
            {
                _nhapKhoDuocPhamRepository.AutoCommitEnabled = false;
                var duocPhamBenhVienIds = xuatKhoDuocPhamChiTietVos.Select(o => o.DuocPhamVatTuId).ToList();
                var soLos = xuatKhoDuocPhamChiTietVos.Select(o => o.SoLo).ToList();
                
                var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.Table
                    .Where(o => o.NhapKhoDuocPhams.KhoId == thongTinXuatKhoKsnkVo.KhoXuatId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                    .ToList();
                //xuat kho
                xuatKhoDuocPham = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham
                {
                    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                    KhoXuatId = thongTinXuatKhoKsnkVo.KhoXuatId,
                    KhoNhapId = thongTinXuatKhoKsnkVo.KhoNhapId,
                    LyDoXuatKho = thongTinXuatKhoKsnkVo.LyDoXuatKho,
                    NguoiXuatId = thongTinXuatKhoKsnkVo.NguoiXuatId,
                    LoaiNguoiNhan = thongTinXuatKhoKsnkVo.LoaiNguoiNhan,
                    TenNguoiNhan = thongTinXuatKhoKsnkVo.TenNguoiNhan,
                    NguoiNhanId = thongTinXuatKhoKsnkVo.NguoiNhanId,
                    NgayXuat = thongTinXuatKhoKsnkVo.NgayXuat
                };
                foreach (var chiTietVo in xuatKhoDuocPhamChiTietVos)
                {
                    var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                        .Where(o => o.DuocPhamBenhVienId == chiTietVo.DuocPhamVatTuId && o.LaDuocPhamBHYT == chiTietVo.LaDpVtBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Date);
                    var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat) && slTon < chiTietVo.SoLuongXuat)
                    {
                        throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                    }
                    double soLuongCanXuat = chiTietVo.SoLuongXuat;
                    while (!soLuongCanXuat.Equals(0))
                    {
                        // tinh so luong xuat
                        var nhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTietXuats
                            .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                        var soLuongTon = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                        var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongXuat;
                        var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                        {
                            SoLuongXuat = soLuongXuat,
                            NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                            NgayXuat = thongTinXuatKhoKsnkVo.NgayXuat
                        };
                        var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                            NgayXuat = thongTinXuatKhoKsnkVo.NgayXuat
                        };
                        xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                        xuatKhoDuocPham.XuatKhoDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);

                        soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                    }
                }
                //nhap kho
                var nhapKho = new NhapKhoDuocPham();
                nhapKho.XuatKhoDuocPham = xuatKhoDuocPham;
                nhapKho.KhoId = thongTinXuatKhoKsnkVo.KhoNhapId;
                nhapKho.NguoiGiaoId = thongTinXuatKhoKsnkVo.NguoiXuatId;
                nhapKho.NguoiNhapId = thongTinXuatKhoKsnkVo.NguoiNhanId ?? _userAgentHelper.GetCurrentUserId();
                nhapKho.NgayNhap = thongTinXuatKhoKsnkVo.NgayXuat;
                nhapKho.LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong;
                foreach (var item in xuatKhoDuocPham.XuatKhoDuocPhamChiTiets)
                {
                    foreach (var viTri in item.XuatKhoDuocPhamChiTietViTris)
                    {
                        var nhapKhoChiTiet = new NhapKhoDuocPhamChiTiet();
                        nhapKhoChiTiet.HopDongThauDuocPhamId = viTri.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId;
                        nhapKhoChiTiet.Solo = viTri.NhapKhoDuocPhamChiTiet.Solo;
                        nhapKhoChiTiet.LaDuocPhamBHYT = viTri.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT;
                        nhapKhoChiTiet.HanSuDung = viTri.NhapKhoDuocPhamChiTiet.HanSuDung;
                        nhapKhoChiTiet.SoLuongNhap = viTri.SoLuongXuat;
                        nhapKhoChiTiet.DonGiaNhap = viTri.NhapKhoDuocPhamChiTiet.DonGiaNhap;
                        nhapKhoChiTiet.VAT = viTri.NhapKhoDuocPhamChiTiet.VAT;
                        nhapKhoChiTiet.TiLeBHYTThanhToan = viTri.NhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan;
                        nhapKhoChiTiet.MaVach = viTri.NhapKhoDuocPhamChiTiet.MaVach;
                        nhapKhoChiTiet.MaRef = viTri.NhapKhoDuocPhamChiTiet.MaRef;
                        nhapKhoChiTiet.DuocPhamBenhVienId = viTri.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId;
                        nhapKhoChiTiet.SoLuongDaXuat = 0;
                        nhapKhoChiTiet.NgayNhap = thongTinXuatKhoKsnkVo.NgayXuat;
                        nhapKhoChiTiet.NgayNhapVaoBenhVien = viTri.NhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien;
                        nhapKhoChiTiet.TiLeTheoThapGia = viTri.NhapKhoDuocPhamChiTiet.TiLeTheoThapGia;
                        nhapKhoChiTiet.PhuongPhapTinhGiaTriTonKho = viTri.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho;
                        nhapKho.NhapKhoDuocPhamChiTiets.Add(nhapKhoChiTiet);
                    }
                }
                _nhapKhoDuocPhamRepository.Add(nhapKho);
            }

            var xuatKhoVatTuChiTietVos = thongTinXuatKhoKsnkVo.ThongTinXuatKhoKsnkChiTietVos.Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiVatTu);
            if (xuatKhoVatTuChiTietVos.Any())
            {
                _nhapKhoVatTuRepository.AutoCommitEnabled = false;
                var vatTuBenhVienIds = xuatKhoVatTuChiTietVos.Select(o => o.DuocPhamVatTuId).ToList();
                var soLos = xuatKhoVatTuChiTietVos.Select(o => o.SoLo).ToList();

                var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.Table
                    .Where(o => o.NhapKhoVatTu.KhoId == thongTinXuatKhoKsnkVo.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                    .ToList();
                //xuat kho
                xuatKhoVatTu = new XuatKhoVatTu
                {
                    LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatQuaKhoKhac,
                    KhoXuatId = thongTinXuatKhoKsnkVo.KhoXuatId,
                    KhoNhapId = thongTinXuatKhoKsnkVo.KhoNhapId,
                    LyDoXuatKho = thongTinXuatKhoKsnkVo.LyDoXuatKho,
                    NguoiXuatId = thongTinXuatKhoKsnkVo.NguoiXuatId,
                    LoaiNguoiNhan = thongTinXuatKhoKsnkVo.LoaiNguoiNhan,
                    TenNguoiNhan = thongTinXuatKhoKsnkVo.TenNguoiNhan,
                    NguoiNhanId = thongTinXuatKhoKsnkVo.NguoiNhanId,
                    NgayXuat = thongTinXuatKhoKsnkVo.NgayXuat
                };
                foreach (var chiTietVo in xuatKhoVatTuChiTietVos)
                {
                    var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                        .Where(o => o.VatTuBenhVienId == chiTietVo.DuocPhamVatTuId && o.LaVatTuBHYT == chiTietVo.LaDpVtBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Date);
                    var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat) && slTon < chiTietVo.SoLuongXuat)
                    {
                        throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                    }
                    double soLuongCanXuat = chiTietVo.SoLuongXuat;
                    while (!soLuongCanXuat.Equals(0))
                    {
                        // tinh so luong xuat
                        var nhapKhoVatTuChiTiet = nhapKhoVatTuChiTietXuats
                            .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                        var soLuongTon = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;
                        var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                        nhapKhoVatTuChiTiet.SoLuongDaXuat += soLuongXuat;
                        var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                        {
                            SoLuongXuat = soLuongXuat,
                            NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                            NgayXuat = thongTinXuatKhoKsnkVo.NgayXuat
                        };
                        var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                            NgayXuat = thongTinXuatKhoKsnkVo.NgayXuat
                        };
                        xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                        xuatKhoVatTu.XuatKhoVatTuChiTiets.Add(xuatKhoVatTuChiTiet);

                        soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                    }
                }
                //nhap kho
                var nhapKho = new NhapKhoVatTu();
                nhapKho.XuatKhoVatTu = xuatKhoVatTu;
                nhapKho.KhoId = thongTinXuatKhoKsnkVo.KhoNhapId;
                nhapKho.NguoiGiaoId = thongTinXuatKhoKsnkVo.NguoiXuatId;
                nhapKho.NguoiNhapId = thongTinXuatKhoKsnkVo.NguoiNhanId ?? _userAgentHelper.GetCurrentUserId();
                nhapKho.NgayNhap = thongTinXuatKhoKsnkVo.NgayXuat;
                nhapKho.LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong;
                foreach (var item in xuatKhoVatTu.XuatKhoVatTuChiTiets)
                {
                    foreach (var viTri in item.XuatKhoVatTuChiTietViTris)
                    {
                        var nhapKhoChiTiet = new NhapKhoVatTuChiTiet();
                        nhapKhoChiTiet.HopDongThauVatTuId = viTri.NhapKhoVatTuChiTiet.HopDongThauVatTuId;
                        nhapKhoChiTiet.Solo = viTri.NhapKhoVatTuChiTiet.Solo;
                        nhapKhoChiTiet.LaVatTuBHYT = viTri.NhapKhoVatTuChiTiet.LaVatTuBHYT;
                        nhapKhoChiTiet.HanSuDung = viTri.NhapKhoVatTuChiTiet.HanSuDung;
                        nhapKhoChiTiet.SoLuongNhap = viTri.SoLuongXuat;
                        nhapKhoChiTiet.DonGiaNhap = viTri.NhapKhoVatTuChiTiet.DonGiaNhap;
                        nhapKhoChiTiet.VAT = viTri.NhapKhoVatTuChiTiet.VAT;
                        nhapKhoChiTiet.TiLeBHYTThanhToan = viTri.NhapKhoVatTuChiTiet.TiLeBHYTThanhToan;
                        nhapKhoChiTiet.MaVach = viTri.NhapKhoVatTuChiTiet.MaVach;
                        nhapKhoChiTiet.MaRef = viTri.NhapKhoVatTuChiTiet.MaRef;
                        nhapKhoChiTiet.VatTuBenhVienId = viTri.NhapKhoVatTuChiTiet.VatTuBenhVienId;
                        nhapKhoChiTiet.SoLuongDaXuat = 0;
                        nhapKhoChiTiet.NgayNhap = thongTinXuatKhoKsnkVo.NgayXuat;
                        nhapKhoChiTiet.NgayNhapVaoBenhVien = viTri.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien;
                        nhapKhoChiTiet.TiLeTheoThapGia = viTri.NhapKhoVatTuChiTiet.TiLeTheoThapGia;
                        nhapKhoChiTiet.PhuongPhapTinhGiaTriTonKho = viTri.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho;
                        nhapKho.NhapKhoVatTuChiTiets.Add(nhapKhoChiTiet);
                    }
                }
                _nhapKhoVatTuRepository.Add(nhapKho);
            }
            BaseRepository.Context.SaveChanges();
            return new XuatKhoKsnkResultVo { XuatKhoDuocPhamId = xuatKhoDuocPham?.Id, XuatKhoVatTuId = xuatKhoVatTu?.Id };
        }

        public async Task SaveChange()
        {
            await BaseRepository.Context.SaveChangesAsync();
        }

        public async Task<double> GetSoLuongTon(long vatTuId, bool isDatChatLuong, long khoNhapId)
        {
            var vatTu = await _vatTuBenhVienRepository.TableNoTracking
                .Include(p => p.NhapKhoVatTuChiTiets)
                .ThenInclude(p => p.NhapKhoVatTu)
                .FirstOrDefaultAsync(p => p.Id == vatTuId);
            double total = 0;
            foreach (var item in vatTu.NhapKhoVatTuChiTiets.Where(p => p.NhapKhoVatTu.KhoId == khoNhapId))
            {
                total = total + item.SoLuongNhap - item.SoLuongDaXuat;
            }
            return total;
        }

        public async Task<List<LookupItemVo>> GetKhoKSNKNhap(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var khoXuatId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (khoXuatId == 0) return new List<LookupItemVo>();
            var khoXuat = _khoRepository.TableNoTracking.First(p => p.Id == khoXuatId);
            var khoKSNKId = GetKhoKSNKXuatTuKhoKSNKCu(khoXuat.Id);

            var lst = _khoRepository.TableNoTracking.Where(p => ((p.Id == khoKSNKId) || (khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh && p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoKSNK) || ((khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh || khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoKSNK || khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe) && p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe))
                       && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId) && p.LaKhoKSNK == true)
                            .ApplyLike(model.Query, p => p.Ten)
                            .Take(model.Take).Select(item => new LookupItemVo
                            {
                                DisplayName = item.Ten,
                                KeyId = item.Id
                            });
            return await lst.ToListAsync(); ;
        }

        public async Task<bool> IsKhoLeOrNhaThuoc(long id)
        {
            var kho = await _khoRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return (kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2);
        }

        public async Task<string> InPhieuXuatVatTu(long id, string hostingName)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatKhoVatTu"));

            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == id)
                .Select(item => new ThongTinInXuatKhoVatTuVo()
                {
                    TenNguoiNhanHang = item.NguoiNhan.User.HoTen,
                    BoPhan = item.KhoVatTuNhap.PhongBenhVien != null
                            ? item.KhoVatTuNhap.PhongBenhVien.Ma + " - " + item.KhoVatTuNhap.PhongBenhVien.Ten
                            : (item.KhoVatTuNhap.KhoaPhong != null ? item.KhoVatTuNhap.KhoaPhong.Ma + " - " + item.KhoVatTuNhap.KhoaPhong.Ten : ""),
                    LyDoXuatKho = item.LyDoXuatKho,
                    XuatTaiKho = item.KhoVatTuXuat.Ten,
                    DiaDiem = "",
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU XUẤT</th>" +
                         "</p>";

            var query = await _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == id)
                .Select(s => new ThongTinInXuatKhoVatTuChiTietVo
                {
                    DuocPhamBenhVienId = s.XuatKhoVatTuChiTiet.VatTuBenhVienId,
                    Ma = s.XuatKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    TenThuoc = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    //DVT = s.DuocPhamBenhVien.DuocPham.DonViTinhThamKhao ?? (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                    DVT = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    SLYeuCau = s.SoLuongXuat.ApplyNumber(),
                    SLThucXuat = s.SoLuongXuat.ApplyNumber(),
                    SLYC = s.SoLuongXuat,
                    SLTX = s.SoLuongXuat,
                    //LaDuocPhamBHYT = s.LaDuocPhamBHYT
                    TenNhom = s.XuatKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() ?? "CHƯA PHÂN NHÓM"
                }).OrderBy(z => z.TenThuoc)
                .ToListAsync();

            data.SoLuongThucXuatTong = query.Sum(p => p.SLTX).ApplyNumber();
            data.SoLuongYeuCauTong = query.Sum(p => p.SLYC).ApplyNumber();

            var totalTenNhom = query.Select(p => p.TenNhom).Distinct().ToList();

            var info = string.Empty;

            var STT = 1;
            foreach (var tenNhom in totalTenNhom)
            {
                var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + tenNhom.ToUpper()
                                        + "</b></tr>";
                info += headerNhom;
                var queryNhom = query.Where(p => p.TenNhom == tenNhom).ToList();
                foreach (var item in queryNhom)
                {
                    info = info
                                           + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                           + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                           + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucXuat
                                           + "</tr>";
                    STT++;
                }
            }

            data.Header = hearder;
            data.DanhSachThuoc = info;
            ;

            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public async Task<string> InPhieuXuatDuocPham(long id, string hostingName)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatKhoDuocPham"));

            var data = await _xuatKhoDuocPhamRepository.TableNoTracking
                .Where(x => x.Id == id)
                .Select(item => new ThongTinInXuatKhoDuocPhamVo()
                {
                    TenNguoiNhanHang = item.NguoiNhan.User.HoTen,
                    BoPhan = item.KhoDuocPhamNhap.PhongBenhVien != null
                            ? item.KhoDuocPhamNhap.PhongBenhVien.Ma + " - " + item.KhoDuocPhamNhap.PhongBenhVien.Ten
                            : (item.KhoDuocPhamNhap.KhoaPhong != null ? item.KhoDuocPhamNhap.KhoaPhong.Ma + " - " + item.KhoDuocPhamNhap.KhoaPhong.Ten : ""),
                    LyDoXuatKho = item.LyDoXuatKho,
                    XuatTaiKho = item.KhoDuocPhamXuat.Ten,
                    DiaDiem = "",
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU XUẤT</th>" +
                         "</p>";

            var query = await _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == id && x.SoLuongXuat > 0)
                .Select(s => new ThongTinInXuatKhoDuocPhamChiTietVo
                {
                    DuocPhamBenhVienId = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.Ma,
                    TenThuoc = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.Ten,
                    //DVT = s.DuocPhamBenhVien.DuocPham.DonViTinhThamKhao ?? (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                    DVT = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    SLYeuCau = s.SoLuongXuat.ApplyNumber(),
                    SLThucXuat = s.SoLuongXuat.ApplyNumber(),
                    SLYC = s.SoLuongXuat,
                    SLTX = s.SoLuongXuat,
                    //LaDuocPhamBHYT = s.LaDuocPhamBHYT
                    TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    DuocPhamBenhVienPhanNhomId = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId
                }).OrderBy(z => z.TenThuoc)
                .ToListAsync();
            var duocPhamBenhVienPhanNhoms = await _duocPhamBenhVienPhanNhom.TableNoTracking.ToListAsync();

            foreach (var item in query)
            {
                item.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(item.DuocPhamBenhVienPhanNhomId.Value, duocPhamBenhVienPhanNhoms);
                item.TenNhom = _duocPhamBenhVienPhanNhom.TableNoTracking.Where(p => p.Id == item.DuocPhamBenhVienPhanNhomChaId).Select(z => z.Ten).FirstOrDefault() ?? "CHƯA PHÂN NHÓM";
            }

            data.SoLuongThucXuatTong = query.Sum(p => p.SLTX).ApplyNumber();
            data.SoLuongYeuCauTong = query.Sum(p => p.SLYC).ApplyNumber();

            var totalTenNhom = query.Select(p => p.TenNhom).Distinct().ToList();

            var info = string.Empty;

            var STT = 1;
            foreach (var tenNhom in totalTenNhom)
            {
                var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + tenNhom.ToUpper()
                                        + "</b></tr>";
                info += headerNhom;
                var queryNhom = query.Where(p => p.TenNhom == tenNhom).ToList();
                foreach (var item in queryNhom)
                {
                    info = info
                                           + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                           + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                           + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucXuat
                                           + "</tr>";
                    STT++;
                }
            }

            data.Header = hearder;
            data.DanhSachThuoc = info;
            ;

            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

    }
}
