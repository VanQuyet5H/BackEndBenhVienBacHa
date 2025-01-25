using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Auth;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChuyenKhoaChuyenNganh;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.ExportImport.Help;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DichVuKyThuatBenhVien
{
    [ScopedDependency(ServiceType = typeof(IDichVuKyThuatBenhVienService))]
    public class DichVuKyThuatBenhVienService : MasterFileService<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien>, IDichVuKyThuatBenhVienService
    {
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> _dichVuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVienGiaBaoHiem> _dichVuKyThuatGiaBaoHiemRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatGiaBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.NhomGiaDichVuKyThuatBenhVien> _nhomGiaBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<DichVuKyThuatBenhVienNoiThucHien> _dichVuKyThuatBenhVienNoiThucHienRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private readonly IDuocPhamVaVatTuBenhVienService _duocPhamVaVatTuBenhVienService;

        IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepo;
        IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepo;

        private IRepository<DuocPham> _duocPhamRepository;
        private IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;
        private IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private IRepository<DuocPhamVaVatTuBenhVien> _duocPhamVaVatTuBenhVienRepository;
        private readonly ICauHinhService _cauHinhService;
        private IRepository<ChuyenKhoaChuyenNganh> _chuyenKhoaChuyenNganhRepository;
        private readonly ILocalizationService _localizationService;

        public DichVuKyThuatBenhVienService(IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> repository
            , IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> dichVuKyThuatRepository, IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVienGiaBaoHiem> dichVuKyThuatGiaBaoHiemRepository
            , IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository, IRepository<Core.Domain.Entities.DichVuKyThuats.NhomGiaDichVuKyThuatBenhVien> nhomGiaBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatGiaBenhVienRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
            IRepository<DichVuKyThuatBenhVienNoiThucHien> dichVuKyThuatBenhVienNoiThucHienRepository
            , IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            IDuocPhamVaVatTuBenhVienService duocPhamVaVatTuBenhVienService,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepo,
            IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepo,
            IRepository<DuocPham> duocPhamRepository,
            IRepository<Core.Domain.Entities.VatTus.VatTu> vatTuRepository,
            IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<DuocPhamVaVatTuBenhVien> duocPhamVaVatTuBenhVienRepository,
            IRepository<ChuyenKhoaChuyenNganh> chuyenKhoaChuyenNganhRepository,
            ICauHinhService cauHinhService,
            ILocalizationService localizationService) : base(repository)
        {
            _nhomGiaBenhVienRepository = nhomGiaBenhVienRepository;
            _dichVuKyThuatRepository = dichVuKyThuatRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _dichVuKyThuatGiaBaoHiemRepository = dichVuKyThuatGiaBaoHiemRepository;
            _dichVuKyThuatGiaBenhVienRepository = dichVuKyThuatGiaBenhVienRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _dichVuKyThuatBenhVienNoiThucHienRepository = dichVuKyThuatBenhVienNoiThucHienRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _duocPhamVaVatTuBenhVienService = duocPhamVaVatTuBenhVienService;
            _nhapKhoDuocPhamChiTietRepo = nhapKhoDuocPhamChiTietRepo;
            _nhapKhoVatTuChiTietRepo = nhapKhoVatTuChiTietRepo;
            _duocPhamRepository = duocPhamRepository;
            _vatTuRepository = vatTuRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _duocPhamVaVatTuBenhVienRepository = duocPhamVaVatTuBenhVienRepository;
            _cauHinhService = cauHinhService;
            _chuyenKhoaChuyenNganhRepository = chuyenKhoaChuyenNganhRepository;
            _localizationService = localizationService;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var dichVuKyThuatBenhVien = new JsonDichVuKyThuatBenhVien();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                dichVuKyThuatBenhVien = JsonConvert.DeserializeObject<JsonDichVuKyThuatBenhVien>(queryInfo.AdditionalSearchString);
            }

            var nhomDichVuBenhVienIds = new List<long>();
            if (dichVuKyThuatBenhVien.NhomDichVuBenhVienId != null)
            {
                var nhomDichVuKyThuatEnity = _nhomDichVuBenhVienRepository.TableNoTracking
                                                                          .Where(z => z.NhomDichVuBenhVienChaId == dichVuKyThuatBenhVien.NhomDichVuBenhVienId)
                                                                          .Select(xx => xx.Id).ToList();

                if (nhomDichVuKyThuatEnity != null && nhomDichVuKyThuatEnity.Any())
                    nhomDichVuBenhVienIds.AddRange(nhomDichVuKyThuatEnity);
                else
                    nhomDichVuBenhVienIds.Add(dichVuKyThuatBenhVien.NhomDichVuBenhVienId ?? 0);
            }

            var query = BaseRepository.TableNoTracking.Include(p => p.DichVuKyThuat)
                                                      .Where(z => (dichVuKyThuatBenhVien.HieuLuc == null && z.HieuLuc != null) || (dichVuKyThuatBenhVien.HieuLuc != null && z.HieuLuc == dichVuKyThuatBenhVien.HieuLuc))
                                                      .Where(z => (dichVuKyThuatBenhVien.AnhXa != true && z.DichVuKyThuatId == null) || (dichVuKyThuatBenhVien.AnhXa != false && z.DichVuKyThuatId != null))
                                                    .Select(s => new DichVuKyThuatBenhVienGridVo
                                                    {
                                                        Id = s.Id,
                                                        Ma = s.Ma,
                                                        Ma4350 = s.DichVuKyThuat == null ? string.Empty : s.DichVuKyThuat.Ma4350,
                                                        DichVuKyThuatId = s.DichVuKyThuatId.GetValueOrDefault(),
                                                        Ten = s.Ten,
                                                        TenNoiThucHien = "",
                                                        NgayBatDauHienThi = s.NgayBatDau.ApplyFormatDate(),
                                                        ThongTu = s.ThongTu,
                                                        NghiDinh = s.QuyetDinh,
                                                        NoiBanHanh = s.NoiBanHanh,
                                                        SoMayTT = s.SoMayThucHien,
                                                        SoMayCBCM = s.SoCanBoChuyenMon,
                                                        ThoiGianThucHien = s.ThoiGianThucHien == null ? 0 : Convert.ToInt64(s.ThoiGianThucHien),
                                                        SoCaCP = s.SoCaChophep == null ? 0 : Convert.ToInt64(s.SoCaChophep),
                                                        MoTa = s.MoTa,
                                                        HieuLucHienThi = s.HieuLuc == true ? "Có" : "Không",
                                                        TenNhomDichVuBenhVien = (!string.IsNullOrEmpty(s.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten) ? s.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten + " > " : "") + s.NhomDichVuBenhVien.Ten,
                                                        LoaiPhauThuatThuThuat = s.LoaiPhauThuatThuThuat,
                                                        LoaiMauXetNghiem = s.LoaiMauXetNghiem,
                                                        DichVuCoKetQuaLau = s.DichVuCoKetQuaLau,
                                                        TenKyThuat = s.TenKyThuat,

                                                        AnhXa = s.DichVuKyThuatId != null,
                                                        ChuyenKhoaChuyenNganh = s.ChuyenKhoaChuyenNganh.Ten,

                                                        DichVuKyThuatBenhVienGiaBenhViens = s.DichVuKyThuatVuBenhVienGiaBenhViens,
                                                        DichVuKyThuatBenhVienGiaBaoHiems = s.DichVuKyThuatBenhVienGiaBaoHiems,
                                                        NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,

                                                        KhoaPhongs = s.DichVuKyThuatBenhVienNoiThucHiens.Where(a => a.KhoaPhongId != null).Select(a => a.KhoaPhong),
                                                        PhongBenhViens = s.DichVuKyThuatBenhVienNoiThucHiens.Where(a => a.PhongBenhVienId != null).Select(a => a.PhongBenhVien),
                                                        KhoaPhongThucHiens = s.DichVuKyThuatBenhVienNoiThucHiens.Where(a => a.PhongBenhVienId != null).Select(a => a.PhongBenhVien.KhoaPhong),

                                                        KhoaPhongThucHienUuTiens = s.DichVuKyThuatBenhVienNoiThucHienUuTiens.Select(a => a.PhongBenhVien),
                                                        SoPhimXquang = s.SoPhimXquang
                                                    });

            if (dichVuKyThuatBenhVien.NhomDichVuBenhVienId != null && nhomDichVuBenhVienIds.Count > 0)
            {
                query = query.Where(z => nhomDichVuBenhVienIds.Contains(z.NhomDichVuBenhVienId));
            }

            if (!string.IsNullOrEmpty(dichVuKyThuatBenhVien.SearchString))
            {
                query = query.ApplyLike(dichVuKyThuatBenhVien.SearchString.Trim(), g => g.Ma, g => g.Ma4350, g => g.Ten, g => g.ThongTu, g => g.NghiDinh, g => g.NoiBanHanh);
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            await Task.WhenAll(countTask);

            var nhomGiaDichVuKyThuatBenhVienId = _nhomGiaBenhVienRepository.TableNoTracking.Where(c => c.Ten.ToUpper() == ("Thường").ToUpper()).Select(c => c.Id).FirstOrDefault();
            foreach (var item in queryTask)
            {
                item.GiaThuongBenhVien = string.Join(", ", item.DichVuKyThuatBenhVienGiaBenhViens.Where(c => c.TuNgay < DateTime.Now && c.NhomGiaDichVuKyThuatBenhVienId == nhomGiaDichVuKyThuatBenhVienId && (c.DenNgay == null || c.DenNgay >= DateTime.Now)).Select(c => c.Gia.ApplyFormatMoneyVND()).ToList());

                item.GiaBHYT = item.AnhXa ? string.Join(", ", item.DichVuKyThuatBenhVienGiaBaoHiems.Where(c => c.TuNgay < DateTime.Now && (c.DenNgay == null || c.DenNgay >= DateTime.Now)).Select(c => c.Gia.ApplyFormatMoneyVND()).ToList()) : string.Empty;
                item.TiLeBaoHiemThanhToan = item.AnhXa ? string.Join(", ", item.DichVuKyThuatBenhVienGiaBaoHiems.Where(c => c.TuNgay < DateTime.Now && (c.DenNgay == null || c.DenNgay >= DateTime.Now)).Select(c => c.TiLeBaoHiemThanhToan + "%").ToList()): string.Empty;

                item.TenNoiThucHien = string.Join("; ", item.KhoaPhongs.Select(a => a.Ma + " - " + a.Ten)) + string.Join("; ", item.PhongBenhViens.Select(a => a.Ma + " - " + a.Ten));
                item.Khoas = string.Join("; ", item.KhoaPhongThucHiens.Select(a => a.Ten).Distinct());
                item.TenNoiThucHienUuTien = string.Join("; ", item.KhoaPhongThucHienUuTiens.Select(a => a.Ma + " - " + a.Ten));
            }

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var dichVuKyThuatBenhVien = new JsonDichVuKyThuatBenhVien();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                dichVuKyThuatBenhVien = JsonConvert.DeserializeObject<JsonDichVuKyThuatBenhVien>(queryInfo.AdditionalSearchString);
            }

            var nhomDichVuBenhVienIds = new List<long>();
            if (dichVuKyThuatBenhVien.NhomDichVuBenhVienId != null)
            {
                var nhomDichVuKyThuatEnity = _nhomDichVuBenhVienRepository.TableNoTracking.Where(z => z.NhomDichVuBenhVienChaId == dichVuKyThuatBenhVien.NhomDichVuBenhVienId)
                        .Select(xx => xx.Id).ToList();
                if (nhomDichVuKyThuatEnity != null && nhomDichVuKyThuatEnity.Any())
                    nhomDichVuBenhVienIds.AddRange(nhomDichVuKyThuatEnity);
                else
                    nhomDichVuBenhVienIds.Add(dichVuKyThuatBenhVien.NhomDichVuBenhVienId ?? 0);
            }

            var query = BaseRepository.TableNoTracking.Include(p => p.DichVuKyThuat)
                                                      .Where(z => (dichVuKyThuatBenhVien.HieuLuc == null && z.HieuLuc != null) || (dichVuKyThuatBenhVien.HieuLuc != null && z.HieuLuc == dichVuKyThuatBenhVien.HieuLuc))
                                                      .Where(z => (dichVuKyThuatBenhVien.AnhXa != true && z.DichVuKyThuatId == null) || (dichVuKyThuatBenhVien.AnhXa != false && z.DichVuKyThuatId != null))
                                                    .Select(s => new DichVuKyThuatBenhVienGridVo
                                                    {
                                                        Id = s.Id,
                                                        Ma = s.Ma,
                                                        DichVuKyThuatId = s.DichVuKyThuatId.GetValueOrDefault(),
                                                        Ma4350 = s.DichVuKyThuat == null ? string.Empty : s.DichVuKyThuat.Ma4350,
                                                        Ten = s.Ten,
                                                        TenNoiThucHien = "",
                                                        NgayBatDauHienThi = s.NgayBatDau.ApplyFormatDate(),
                                                        ThongTu = s.ThongTu,
                                                        NghiDinh = s.QuyetDinh,
                                                        NoiBanHanh = s.NoiBanHanh,
                                                        SoMayTT = s.SoMayThucHien,
                                                        SoMayCBCM = s.SoCanBoChuyenMon,
                                                        ThoiGianThucHien = s.ThoiGianThucHien == null ? 0 : s.ThoiGianThucHien,
                                                        SoCaCP = s.SoCaChophep == null ? 0 : s.SoCaChophep,
                                                        MoTa = s.MoTa,
                                                        HieuLucHienThi = s.HieuLuc == true ? "Có" : "Không",
                                                        NhomDichVuBenhVienId = s.NhomDichVuBenhVienId
                                                    });
            if (dichVuKyThuatBenhVien.NhomDichVuBenhVienId != null && nhomDichVuBenhVienIds.Count > 0)
            {
                query = query.Where(z => nhomDichVuBenhVienIds.Contains(z.NhomDichVuBenhVienId));
            }

            if (!string.IsNullOrEmpty(dichVuKyThuatBenhVien.SearchString))
            {
                query = query.ApplyLike(dichVuKyThuatBenhVien.SearchString.Trim(), g => g.Ma, g => g.Ma4350, g => g.Ten, g => g.ThongTu, g => g.NghiDinh, g => g.NoiBanHanh);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var sortString = RemoveDisplaySort(queryInfo);

            var query = _dichVuKyThuatGiaBaoHiemRepository.TableNoTracking.Include(x => x.DichVuKyThuatBenhVien)
                .Where(x => x.DichVuKyThuatBenhVienId == long.Parse(queryInfo.SearchTerms))
                .Select(s => new DichVuKyThuatBenhVienChildrenGridVo()
                {
                    Id = s.Id,
                    Gia = s.Gia,
                    GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                    TiLeThanhToan = s.TiLeBaoHiemThanhToan,
                    TuNgay = s.TuNgay,
                    TuNgayDisplay = s.TuNgay.ApplyFormatDate(),
                    DenNgayDisplay = s.DenNgay == null ? null : Convert.ToDateTime(s.DenNgay).ApplyFormatDate()
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(sortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupItemVo>> GetNhomDichVu()
        {
            var lst = await _nhomGiaBenhVienRepository.TableNoTracking
                .ToListAsync();

            var query = lst.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id
            }).ToList();

            return query;
        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _dichVuKyThuatGiaBaoHiemRepository.TableNoTracking
                 .Where(x => x.DichVuKyThuatBenhVienId == long.Parse(queryInfo.SearchTerms))
                 .Select(s => new DichVuKyThuatBenhVienChildrenGridVo()
                 {
                     Id = s.Id,
                     Gia = s.Gia,
                     GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                     TiLeThanhToan = s.TiLeBaoHiemThanhToan,
                     TuNgay = s.TuNgay,
                     TuNgayDisplay = s.TuNgay.ApplyFormatDate(),
                     DenNgayDisplay = s.DenNgay == null ? null : Convert.ToDateTime(s.DenNgay).ApplyFormatDate()
                 });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildBenhVienAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var sortString = RemoveDisplaySort(queryInfo);

            var query = _dichVuKyThuatGiaBenhVienRepository.TableNoTracking.Include(x => x.DichVuKyThuatBenhVien).Include(x => x.NhomGiaDichVuKyThuatBenhVien)
                .Where(x => x.DichVuKyThuatBenhVienId == long.Parse(queryInfo.SearchTerms))
                .Select(s => new DichVuKyThuatBenhVienChildrenGiaBenhVienGridVo()
                {
                    Id = s.Id,
                    Gia = s.Gia,
                    GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                    LoaiGia = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                    TuNgay = s.TuNgay,
                    TuNgayDisplay = s.TuNgay.ApplyFormatDate(),
                    DenNgayDisplay = s.DenNgay == null ? null : Convert.ToDateTime(s.DenNgay).ApplyFormatDate()
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(sortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridChildBenhVienAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _dichVuKyThuatGiaBenhVienRepository.TableNoTracking.Include(x => x.DichVuKyThuatBenhVien).Include(x => x.NhomGiaDichVuKyThuatBenhVien)
               .Where(x => x.DichVuKyThuatBenhVienId == long.Parse(queryInfo.SearchTerms))
               .Select(s => new DichVuKyThuatBenhVienChildrenGiaBenhVienGridVo()
               {
                   Id = s.Id,
                   Gia = s.Gia,
                   GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                   LoaiGia = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                   TuNgay = s.TuNgay,
                   TuNgayDisplay = s.TuNgay.ApplyFormatDate(),
                   DenNgayDisplay = s.DenNgay == null ? null : Convert.ToDateTime(s.DenNgay).ApplyFormatDate()
               });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model)
        {
            return await _dichVuKyThuatRepository.TableNoTracking//.Where(o => BaseRepository.TableNoTracking.All(p => p.DichVuKyThuatId != o.Id))
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.TenChung)
                .Select(item => new DichVuKyThuatTemplateVo
                {
                    DisplayName = item.TenChung,//item.MaChung + " - " + item.TenChung,
                    KeyId = item.Id,
                    DichVu = item.TenChung,
                    Ma = item.MaChung,
                }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                .Take(model.Take)
                .ToListAsync();
        }
        public async Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuatBenhVien(DropDownListRequestModel model)
        {
            return await BaseRepository.TableNoTracking//.Where(o => BaseRepository.TableNoTracking.All(p => p.DichVuKyThuatId != o.Id))
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                .Select(item => new DichVuKyThuatTemplateVo
                {
                    DisplayName = item.Ten,//item.MaChung + " - " + item.TenChung,
                    KeyId = item.Id,
                    DichVu = item.Ten,
                    Ma = item.Ma,
                }).ApplyLike(model.Query, o => o.DisplayName)
                .Take(model.Take)
                .ToListAsync();
        }

        public async Task<List<DuocPhamVacxinTemplateVo>> GetListDuocPhamBenhVien(DropDownListRequestModel model)
        {
            return await _duocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.HieuLuc)
                .Select(item => new DuocPhamVacxinTemplateVo
                {
                    DisplayName = item.DuocPham.Ten,
                    KeyId = item.Id,
                    SoDangKy = item.DuocPham.SoDangKy,
                    MaDuocPhamBenhVien = item.MaDuocPhamBenhVien,
                    Ten = item.DuocPham.Ten,
                    MaHoatChat = item.DuocPham.MaHoatChat,
                    HoatChat = item.DuocPham.HoatChat,
                    NhaSanXuat = item.DuocPham.NhaSanXuat,
                    NuocSanXuat = item.DuocPham.NuocSanXuat,
                    QuyCach = item.DuocPham.QuyCach
                }).ApplyLike(model.Query, o => o.DisplayName)
                .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName)
                .Take(model.Take)
                .ToListAsync();
        }

        public async Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuatById(DropDownListRequestModel model, long id)
        {

            var lst = await _dichVuKyThuatRepository.TableNoTracking
                .Where(p => p.TenChung.Contains(model.Query ?? "") || p.MaChung.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();
            var entity = await _dichVuKyThuatRepository.TableNoTracking
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            var lstDichVuKhamBenhBenhVien = await BaseRepository.TableNoTracking.Distinct()
                .ToListAsync();

            for (int i = 0; i < lstDichVuKhamBenhBenhVien.Count; i++)
            {
                var check = lst.Where(x => x.Id == lstDichVuKhamBenhBenhVien[i].DichVuKyThuatId).FirstOrDefault();
                if (check != null)
                {
                    lst.RemoveAll(x => x.Id == lstDichVuKhamBenhBenhVien[i].DichVuKyThuatId);
                }
            }
            lst.Add(entity);
            var query = lst.Select(item => new DichVuKyThuatTemplateVo
            {
                DisplayName = item.MaChung + " - " + item.TenChung,
                KeyId = item.Id,
                DichVu = item.TenChung,
                Ma = item.MaChung,
            }).ToList();

            return query;
        }
        public async Task<List<KhoaKhamTemplateVo>> GetKhoaKham(DropDownListRequestModel model)
        {
            //var lstKhoaKham = await _khoaPhongRepository.TableNoTracking
            //    .Where(p => p.IsDisabled != true 
            //                && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")))
            //    .Take(model.Take)
            //    .ToListAsync();


            var lst = await _khoaPhongRepository.TableNoTracking
                .Where(p => p.IsDisabled != true)
                .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
                .Take(model.Take)
                .ToListAsync();

            var lstKhoaPhongDaChon = await _dichVuKyThuatBenhVienNoiThucHienRepository.TableNoTracking
                .Where(x => x.DichVuKyThuatBenhVienId == model.Id && x.KhoaPhongId != null
                                                                   && !lst.Select(a => a.Id.ToString()).Contains(x.KhoaPhongId.ToString())).Select(x => x.KhoaPhongId.ToString()).ToListAsync();
            if (lstKhoaPhongDaChon.Any())
            {
                var lstDaChon = await _khoaPhongRepository.TableNoTracking
                    .Where(p => lstKhoaPhongDaChon.Contains(p.Id.ToString()))
                    .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
                    .ToListAsync();

                lst.AddRange(lstDaChon);
            }

            var query = lst.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma,
            }).ToList();

            return query;
        }

        public async Task<List<NoiThucHienDichVuBenhVienVo>> GetPhongKhamTheoDichVuKyThuatBenhVien(DropDownListRequestModel model)
        {
            //var lst = await _phongBenhVienRepository.TableNoTracking
            //    .Where(p => p.IsDisabled != true)
            //    .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
            //    .Take(model.Take)
            //    .ToListAsync();

            //var lstPhongBenhVienDaChon = await _dichVuKyThuatBenhVienNoiThucHienRepository.TableNoTracking
            //    .Where(x => x.DichVuKyThuatBenhVienId == model.Id && x.PhongBenhVienId != null
            //                                                       && !lst.Select(a => a.Id.ToString()).Contains(x.PhongBenhVienId.ToString())).Select(x => x.PhongBenhVienId.ToString()).ToListAsync();
            //if (lstPhongBenhVienDaChon.Any())
            //{
            //    var lstDaChon = await _phongBenhVienRepository.TableNoTracking
            //        .Where(p => lstPhongBenhVienDaChon.Contains(p.Id.ToString()))
            //        .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
            //        .ToListAsync();

            //    lst.AddRange(lstDaChon);
            //}

            //var query = lst.Select(item => new KhoaKhamTemplateVo
            //{
            //    DisplayName = item.Ma + " - " + item.Ten,
            //    KeyId = item.Id,
            //    Ten = item.Ten,
            //    Ma = item.Ma,
            //}).ToList();

            //return query;

            if (string.IsNullOrEmpty(model.ParameterDependencies))
            {
                return new List<NoiThucHienDichVuBenhVienVo>();
            }

            var lstKhoaIdDaChon = JsonConvert.DeserializeObject<List<KhoaDaChonVo>>(model.ParameterDependencies);
            var lstKhoa = await _khoaPhongRepository.TableNoTracking
                .Include(x => x.PhongBenhViens)
                .Where(x => lstKhoaIdDaChon.Any(i => i.KhoaId == x.Id)
                            //&& (!lstKhoaDaLuu.Any() || lstKhoaDaLuu.Any(i => i == x.Id))
                            && (string.IsNullOrEmpty(model.Query) || !string.IsNullOrEmpty(model.Query) && ((x.Ma + " - " + x.Ten).ToLower().RemoveVietnameseDiacritics().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())
                            || x.PhongBenhViens.Any(y => (y.Ma + " - " + y.Ten).ToLower().RemoveVietnameseDiacritics().Contains(model.Query.RemoveVietnameseDiacritics().ToLower()))))) // thiếu điều kiện
                .ToArrayAsync();

            var lstPhongTheoKhoa = new List<NoiThucHienDichVuBenhVienVo>();
            var templateKeyId = "\"KhoaId\": {0}, \"PhongId\": {1}";
            foreach (var item in lstKhoa)
            {
                // get list phòng thuộc khoa hiện tại
                var lstItems = item.PhongBenhViens.Where(x => x.IsDisabled != true).Select(items =>
                    new NoiThucHienDichVuBenhVienVo()
                    {
                        DisplayName = items.Ten, //items.Ma + " - " + items.Ten,
                        KeyId = "{" + string.Format(templateKeyId, items.KhoaPhongId, items.Id) + "}",
                        Ma = items.Ma,
                        Ten = items.Ten,
                        KhoaId = items.KhoaPhongId
                    }).OrderBy(x => x.Ten).ThenBy(x => x.Ma).ToList();

                // thêm khoa vào list nơi thực hiện
                var khoa = new NoiThucHienDichVuBenhVienVo()
                {
                    DisplayName = item.Ten, //item.Ma + " - " + item.Ten,
                    KeyId = "{" + string.Format(templateKeyId, item.Id, "\"\"") + "}",
                    Ma = item.Ma,
                    Ten = item.Ten,
                    KhoaId = null,
                    Items = lstItems
                };
                lstPhongTheoKhoa.Add(khoa);


                // thêm phòng thuộc khoa vào list nơi thực hiện
                if (item.PhongBenhViens.Any())
                {
                    var lstPhong = item.PhongBenhViens.Where(x => x.IsDisabled != true);
                    if (!string.IsNullOrEmpty(model.Query))
                    {
                        if (!khoa.DisplayName.ToLower().RemoveVietnameseDiacritics().Contains(model.Query.RemoveVietnameseDiacritics().ToLower()))
                        {
                            lstPhong = lstPhong.Where(x =>
                                (x.Ma + " - " + x.Ten).ToLower().RemoveVietnameseDiacritics()
                                .Contains(model.Query.RemoveVietnameseDiacritics().ToLower()));
                        }
                    }
                    lstPhongTheoKhoa.AddRange(lstPhong.Where(x => x.IsDisabled != true).OrderBy(x => x.Ten).ThenBy(x => x.Ma).Select(items =>
                        new NoiThucHienDichVuBenhVienVo()
                        {
                            DisplayName = items.Ten, //items.Ma + " - " + items.Ten,
                            KeyId = "{" + string.Format(templateKeyId, items.KhoaPhongId, items.Id) + "}",
                            Ma = items.Ma,
                            Ten = items.Ten,
                            KhoaId = items.KhoaPhongId,
                            KhoaKeyId = "{" + string.Format(templateKeyId, items.KhoaPhongId, "\"\"") + "}",
                            Items = lstItems,
                            CountItems = lstItems.Count
                        }));
                }
            }
            return lstPhongTheoKhoa;
        }

        public async Task<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> GetAfterEntity(long id)
        {
            var lstEntity = await BaseRepository.Table.OrderByDescending(p => p.Id).ToListAsync();
            var isGetItem = false;
            foreach (var item in lstEntity)
            {
                if (isGetItem)
                {
                    return item;
                }
                if (item.Id == id)
                {
                    isGetItem = true;
                }
            }

            return new Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien();
        }

        public async Task UpdateLastEntity(long id, DateTime tuNgay, long? dichVuKyThuatId)
        {
            var lastEntity = await BaseRepository.Table.Where(p => p.Id != id && p.DichVuKyThuatId == dichVuKyThuatId).OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();
            if (lastEntity != null)
            {
                //lastEntity.DenNgay = tuNgay.AddDays(-1);
                await BaseRepository.UpdateAsync(lastEntity);
            }
        }
        public Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien CheckExistDichVuKyThuatBenhVien(long id)
        {
            var result = BaseRepository.TableNoTracking.Where(x => x.DichVuKyThuatId == id).FirstOrDefault();
            if (result != null)
            {
                return result;
            }
            return null;
        }
        public async Task AddAndUpdateLastEntity(DateTime tuNgayBaoHiem, ICollection<DichVuKyThuatBenhVienGiaBenhVien> giaBenhVienEntity)
        {
            var nhomDichvu = _nhomGiaBenhVienRepository.Table.ToList();
            var baohiemEntity = await _dichVuKyThuatGiaBaoHiemRepository.Table.OrderByDescending(p => p.Id)
                .ToListAsync();

            if (baohiemEntity.Count > 0)
            {
                var baohiem = baohiemEntity.FirstOrDefault();
                baohiem.DenNgay = tuNgayBaoHiem.Date.AddDays(-1);
                await _dichVuKyThuatGiaBaoHiemRepository.UpdateAsync(baohiem);
            }
            foreach (var item in nhomDichvu)
            {
                var giaTheoNhom = giaBenhVienEntity.Where(x => x.NhomGiaDichVuKyThuatBenhVienId.Equals(item.Id)).ToList();
                if (giaTheoNhom.Count > 0)
                {
                    for (int i = 1; i < giaTheoNhom.Count; i++)
                    {
                        giaTheoNhom[i - 1].DenNgay = Convert.ToDateTime(giaTheoNhom[i].TuNgay).Date.AddDays(-1);
                    }
                }

            }

        }
        public async Task<List<NhomGiaDichVuKyThuatBenhVien>> GetNhomGiaDichVuKyThuatBenhVien()
        {
            var lstEntity = await _nhomGiaBenhVienRepository.Table.ToListAsync();


            return lstEntity;
        }
        public async Task UpdateDayGiaBenhVienEntity(ICollection<DichVuKyThuatBenhVienGiaBenhVien> giaBenhVienEntity)
        {
            var nhomDichvu = _nhomGiaBenhVienRepository.Table.ToList();

            foreach (var item in nhomDichvu)
            {
                var giaTheoNhom = giaBenhVienEntity.Where(x => x.NhomGiaDichVuKyThuatBenhVienId.Equals(item.Id)).ToList();
                if (giaTheoNhom.Count > 0)
                {
                    for (int i = 1; i < giaTheoNhom.Count; i++)
                    {
                        giaTheoNhom[i - 1].DenNgay = Convert.ToDateTime(giaTheoNhom[i].TuNgay).Date.AddDays(-1);
                    }
                }
                var dichvuKhamBenhBenhVien = _dichVuKyThuatGiaBenhVienRepository.Table.Where(x => x.NhomGiaDichVuKyThuatBenhVienId == item.Id).OrderByDescending(x => x.Id).FirstOrDefault();

            }

        }
        public async Task<bool> IsTuNgayValid(DateTime? tuNgay, long? id, long? dichVuKyThuatId)
        {
            if (tuNgay == null || dichVuKyThuatId == null || dichVuKyThuatId == 0) return true;
            //update
            if (id != null && id != 0)
            {
                var lstEntity = await BaseRepository.TableNoTracking.Where(p => p.DichVuKyThuatId == dichVuKyThuatId).OrderByDescending(p => p.Id).ToListAsync();
                var isGetItem = false;
                foreach (var item in lstEntity)
                {
                    if (isGetItem)
                    {
                        var denNgayTemp = tuNgay.GetValueOrDefault().Date.AddDays(-1);
                        //if (item?.TuNgay.GetValueOrDefault().Date == denNgayTemp.Date ||
                        //    tuNgay.GetValueOrDefault().Date <= item?.TuNgay.GetValueOrDefault().Date
                        //    || (item?.DenNgay != null &&
                        //        tuNgay.GetValueOrDefault().Date <= item?.DenNgay.GetValueOrDefault().Date))
                        //{
                        //    return false;
                        //}
                        //else
                        //{
                        //    return true;
                        //}
                    }
                    if (item?.Id == id)
                    {
                        isGetItem = true;
                    }
                }
            }
            //create
            else
            {
                var item = await BaseRepository.TableNoTracking.Where(p => p.DichVuKyThuatId == dichVuKyThuatId).OrderByDescending(p => p.Id).FirstOrDefaultAsync();
                var denNgayTemp = tuNgay.GetValueOrDefault().Date.AddDays(-1);
                //if (item?.TuNgay.GetValueOrDefault().Date == denNgayTemp.Date ||
                //    tuNgay.GetValueOrDefault().Date <= item?.TuNgay.GetValueOrDefault().Date
                //    || (item?.DenNgay != null &&
                //        tuNgay.GetValueOrDefault().Date <= item?.DenNgay.GetValueOrDefault().Date))
                //{
                //    return false;
                //}
                //else
                //{
                //    return true;
                //}
            }

            //return true;
            throw new NotImplementedException();
        }


        private string RemoveDisplaySort(QueryInfo queryInfo)
        {
            var result = queryInfo.SortString;
            if (queryInfo.SortString.Contains("Display"))
            {
                result = queryInfo.SortString.Replace("Display", "");
            }
            return result;
        }

        public async Task<bool> IsExistsMaDichVuKyThuatBenhVien(long dichVuKyThuatBenhVienId, string ma)
        {
            if (dichVuKyThuatBenhVienId != 0 || string.IsNullOrEmpty(ma))
                return false;
            var dichVuKyThuatBenhVien =
                await BaseRepository.TableNoTracking.AnyAsync(x => x.Ma.Trim().ToLower() == ma.Trim().ToLower());
            return dichVuKyThuatBenhVien;
        }

        public async Task XuLyCapNhatNoiThucHienUuTienKhiCapNhatDichVuKyThuatAsync(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien dichVuKyThuatBenhVien)
        {
            if (dichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens.Any() &&
                dichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHienUuTiens.Any())
            {
                var lstPhongThucHienId = dichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens
                    .Where(x => x.PhongBenhVienId != null && !x.WillDelete).Select(x => x.PhongBenhVienId).ToList();
                var lstKhoaThucHienId = dichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens
                    .Where(x => x.KhoaPhongId != null && !x.WillDelete).Select(x => x.KhoaPhongId).ToList();

                var lstPhongThucHien = await _phongBenhVienRepository.TableNoTracking
                    .Where(x => lstPhongThucHienId.Any(a => a == x.Id) || lstKhoaThucHienId.Any(a => a == x.KhoaPhongId))
                    .Select(x => x.Id)
                    .Distinct()
                    .ToListAsync();
                foreach (var noiThucHienUuTien in dichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHienUuTiens.Where(x => x.WillDelete != true))
                {
                    if (lstPhongThucHien.All(x => x != noiThucHienUuTien.PhongBenhVienId))
                    {
                        noiThucHienUuTien.WillDelete = true;
                    }
                }
            }
        }

        public async Task<List<NoiThucHienUuTienDichVuBenhVienVo>> GetListNoiThucHienUuTienTheoNoiThucHienDangChonAsync(DropDownListRequestModel model)
        {
            var lstNoiThucHien = new List<NoiThucHienUuTienDichVuBenhVienVo>();
            if (!string.IsNullOrEmpty(model.ParameterDependencies) && !model.ParameterDependencies.Contains("undefined") && !model.ParameterDependencies.Contains("null"))
            {
                var firstIndex = model.ParameterDependencies.IndexOf(':');
                var lastIndex = model.ParameterDependencies.LastIndexOf('}');
                model.ParameterDependencies = model.ParameterDependencies.Remove(firstIndex, 1).Insert(firstIndex, ":[");
                model.ParameterDependencies = model.ParameterDependencies.Remove(lastIndex + 1, 1).Insert(lastIndex + 1, "]}");
                var lstNoiThucHienObj = JsonConvert.DeserializeObject<NoiThucHienDichVuBenhVienDichVuBenhVienVo>(model.ParameterDependencies).NoiThucHienIds;

                if (lstNoiThucHienObj.Any())
                {
                    var noiThucHienObj = lstNoiThucHienObj.First();

                    lstNoiThucHien = await _phongBenhVienRepository.TableNoTracking
                        .Where(x => x.IsDisabled != true
                                    && ((noiThucHienObj.PhongId != null && x.Id == noiThucHienObj.PhongId) ||
                                            (noiThucHienObj.PhongId == null && x.KhoaPhongId == noiThucHienObj.KhoaId)))
                        .OrderBy(x => x.Ten).ThenBy(x => x.Ma)
                        .Take(1)
                        .Union(_phongBenhVienRepository.TableNoTracking
                            .Where(x => x.IsDisabled != true
                                && lstNoiThucHienObj.Any(y => (y.PhongId == null && y.KhoaId == x.KhoaPhongId) || (y.PhongId != null && y.PhongId == x.Id)))
                            .OrderBy(x => x.Ten).ThenBy(x => x.Ma))
                        .ApplyLike(model.Query, x => x.Ma, x => x.Ten)
                        .Distinct()
                        .Select(item => new NoiThucHienUuTienDichVuBenhVienVo()
                        {
                            KeyId = item.Id,
                            DisplayName = item.Ten, //item.Ma + " - " + item.Ten,
                            Ma = item.Ma,
                            Ten = item.Ten,
                            TenKhoa = item.KhoaPhong.Ma + " - " + item.KhoaPhong.Ten
                        }).Take(model.Take).ToListAsync();
                }
            }
            return lstNoiThucHien;
        }

        public async Task<List<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien>> NhomDichVuBenhViens()
        {
            return await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();
        }

        public async Task XuLyChuyenNhomXetNghiem(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien dichVuKyThuatBenhVien, List<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhViens, long nhomDichVuBenhVienId, string ma, string ten)
        {
            LoaiDichVuKyThuat loaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(nhomDichVuBenhVienId, nhomDichVuBenhViens);
            if (dichVuKyThuatBenhVien.DichVuXetNghiemId != null)      // đang có dịch vụ xét nghiệm
            {
                if (loaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem) // chuyển sang nhóm không thuộc xét nghiệm
                {
                    dichVuKyThuatBenhVien.DichVuXetNghiemId = null;
                    if (dichVuKyThuatBenhVien.DichVuXetNghiem != null)
                    {
                        dichVuKyThuatBenhVien.DichVuXetNghiem.HieuLuc = false;
                        if (dichVuKyThuatBenhVien.DichVuXetNghiem.DichVuXetNghiems.Any()) // dịch vụ xét nghiệm cấp 2
                        {
                            foreach (var item in dichVuKyThuatBenhVien.DichVuXetNghiem.DichVuXetNghiems)
                            {
                                item.HieuLuc = false;
                            }
                            if (dichVuKyThuatBenhVien.DichVuXetNghiem.DichVuXetNghiems.SelectMany(z => z.DichVuXetNghiems).Any()) // dịch vụ xét nghiệm cấp 3
                            {
                                foreach (var item in dichVuKyThuatBenhVien.DichVuXetNghiem.DichVuXetNghiems.SelectMany(z => z.DichVuXetNghiems))
                                {
                                    item.HieuLuc = false;
                                }
                            }
                        }
                    }
                }
                else                                                // chuyển sang nhóm xét nghiệm khác
                {
                    dichVuKyThuatBenhVien.NhomDichVuBenhVienId = nhomDichVuBenhVienId;
                    if (dichVuKyThuatBenhVien.DichVuXetNghiem != null)
                    {
                        dichVuKyThuatBenhVien.DichVuXetNghiem.NhomDichVuBenhVienId = nhomDichVuBenhVienId;
                        if (dichVuKyThuatBenhVien.DichVuXetNghiem.DichVuXetNghiems.Any()) // dịch vụ xét nghiệm cấp 2
                        {
                            foreach (var item in dichVuKyThuatBenhVien.DichVuXetNghiem.DichVuXetNghiems)
                            {
                                item.NhomDichVuBenhVienId = nhomDichVuBenhVienId;
                            }
                            if (dichVuKyThuatBenhVien.DichVuXetNghiem.DichVuXetNghiems.SelectMany(z => z.DichVuXetNghiems).Any()) // dịch vụ xét nghiệm cấp 3
                            {
                                foreach (var item in dichVuKyThuatBenhVien.DichVuXetNghiem.DichVuXetNghiems.SelectMany(z => z.DichVuXetNghiems))
                                {
                                    item.NhomDichVuBenhVienId = nhomDichVuBenhVienId;
                                }
                            }
                        }
                    }
                }
            }
            else // đang là dịch vụ thường
            {
                if (loaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem) // chuyển sang nhóm xét nghiệm
                {
                    var dichVuXetNghiem = new Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem
                    {
                        NhomDichVuBenhVienId = nhomDichVuBenhVienId,
                        Ma = ma,
                        Ten = ten,
                        CapDichVu = 1,
                        HieuLuc = true
                    };
                    dichVuKyThuatBenhVien.DichVuXetNghiem = dichVuXetNghiem;
                }
            }
        }

        public List<LookupItemVo> GetDanhSachPhanLoaiPTTT(DropDownListRequestModel queryInfo)
        {
            //BVHD-3822 họ muốn LoaiPhauThuatThuThuat
            var lstEnumPhanLoaiPTTT = EnumHelper.GetListEnum<LoaiPhauThuatThuThuat>()
                                                .Select(item => new LookupItemVo
                                                {
                                                    KeyId = Convert.ToInt32(item),
                                                    DisplayName = item.GetDescription()
                                                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstEnumPhanLoaiPTTT = lstEnumPhanLoaiPTTT.Where(p => p.DisplayName.ToLower().RemoveVietnameseDiacritics().Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }

            return lstEnumPhanLoaiPTTT;
        }
        public async Task<bool> KiemTraNgay(DateTime? tuNgay, DateTime? denNgay)
        {
            if (tuNgay != null && denNgay != null)
            {
                if (denNgay < tuNgay)
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuDinhMucAsync(DropDownListRequestModel queryInfo)
        {
            //var duocPhamVaVatTus = await _duocPhamVaVatTuBenhVienService.GetDuocPhamVaVatTuTrongKhoTuTrucNhanVien(false, queryInfo.Query, 0, queryInfo.Take);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                var query = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>

                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .ApplyLike(queryInfo.Query, g => g.DuocPham.Ten)
                    .Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
                    {
                        DisplayName = s.DuocPham.Ten,
                        KeyId = s.Id,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh.Ten,
                        DuongDung = s.DuocPham.DuongDung.Ten,
                        SoLuongTon = Math.Round(s.NhapKhoDuocPhamChiTiets
                                                 .Where(nkct =>
                                                  nkct.HanSuDung >= DateTime.Now)
                                                 .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat), 1),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                    nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                   .Select(p => (p.HanSuDung != null ? p.HanSuDung.ApplyFormatDate() : "")).First(),
                        Loai = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien.GetDescription(),
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat

                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .ApplyLike(queryInfo.Query, g => g.VatTus.Ten)
                            .Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
                            {
                                DisplayName = s.VatTus.Ten,
                                KeyId = s.Id,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon = Math.Round(s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat), 1),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => (p.HanSuDung != null ? p.HanSuDung.ApplyFormatDate() : "")).First(),
                                Loai = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien.GetDescription(),
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            })).OrderBy(o => o.Ten).Take(queryInfo.Take);


                return await query.ToListAsync();
            }


            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ma));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ten));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.HoatChat));


            var duocPhamVaVatTuBenhViens = await _duocPhamVaVatTuBenhVienRepository
                .ApplyFulltext(queryInfo.Query, nameof(DuocPhamVaVatTuBenhVien), lstColumnNameSearch).Where(p => p.HieuLuc)
                .Select(s => new DuocPhamVaVatTuTrongKhoVo
                {
                    Id = s.DuocPhamBenhVienId ?? s.VatTuBenhVienId.Value,
                    LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu
                }).ToListAsync();

            var dctDuocPham = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);
            var dctVatTu = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);


            var queryFullText = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien && p.Id == o.Id))
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
                    {
                        DisplayName = s.DuocPham.Ten,
                        KeyId = s.Id,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh.Ten,
                        DuongDung = s.DuocPham.DuongDung.Ten,
                        SoLuongTon = Math.Round(s.NhapKhoDuocPhamChiTiets
                                                 .Where(nkct =>
                                                  nkct.HanSuDung >= DateTime.Now)
                                                 .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat), 1),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                    nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                   .Select(p => (p.HanSuDung != null ? p.HanSuDung.ApplyFormatDate() : "")).First(),
                        Loai = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien.GetDescription(),
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien && p.Id == o.Id))
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                             kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
                            {

                                DisplayName = s.VatTus.Ten,
                                KeyId = s.Id,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon = Math.Round(s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat), 1),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => (p.HanSuDung != null ? p.HanSuDung.ApplyFormatDate() : "")).First(),
                                Loai = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien.GetDescription(),
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            }))
                .OrderBy(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? (dctDuocPham.Any(a => a.Key == p.KeyId) ? dctDuocPham[p.KeyId] : duocPhamVaVatTuBenhViens.Count) : (dctVatTu.Any(a => a.Key == p.KeyId) ? dctVatTu[p.KeyId] : duocPhamVaVatTuBenhViens.Count))
                .Take(queryInfo.Take);


            return queryFullText.ToList();
        }

        public async Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuDinhMucDVKTAsync(DropDownListRequestModel queryInfo, long? duocPhamVTYTId)
        {
            //var duocPhamVaVatTus = await _duocPhamVaVatTuBenhVienService.GetDuocPhamVaVatTuTrongKhoTuTrucNhanVien(false, queryInfo.Query, 0, queryInfo.Take);
            if (duocPhamVTYTId != null && duocPhamVTYTId != 0)
            {
                queryInfo.Id = (long)duocPhamVTYTId;
            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                var query = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>

                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now)
                     && (queryInfo.Id == null || queryInfo.Id == 0 || (queryInfo.Id != 0 && o.Id == queryInfo.Id)))
                    .ApplyLike(queryInfo.Query, g => g.DuocPham.Ten)
                    .Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
                    {
                        DisplayName = s.DuocPham.Ten,
                        KeyId = s.Id,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh.Ten,
                        DuongDung = s.DuocPham.DuongDung.Ten,
                        SoLuongTon = Math.Round(s.NhapKhoDuocPhamChiTiets
                                                 .Where(nkct =>
                                                  nkct.HanSuDung >= DateTime.Now)
                                                 .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat), 1),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                    nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                   .Select(p => (p.HanSuDung != null ? p.HanSuDung.ApplyFormatDate() : "")).First(),
                        Loai = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien.GetDescription(),
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat,

                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now)
                            && (queryInfo.Id == null || queryInfo.Id == 0 || (queryInfo.Id != 0 && o.Id == queryInfo.Id)))
                            .ApplyLike(queryInfo.Query, g => g.VatTus.Ten)
                            .Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
                            {
                                DisplayName = s.VatTus.Ten,
                                KeyId = s.Id,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon = Math.Round(s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat), 1),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => (p.HanSuDung != null ? p.HanSuDung.ApplyFormatDate() : "")).First(),
                                Loai = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien.GetDescription(),
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            })).OrderBy(o => o.Ten).Take(queryInfo.Take);

                //if (queryInfo.Id != null && queryInfo.Id != 0)
                //{
                //    return await query.Where(d => d.KeyId == queryInfo.Id).ToListAsync();
                //}
                //else
                //{

                //}
                return await query.ToListAsync();

            }


            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ma));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ten));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.HoatChat));


            var duocPhamVaVatTuBenhViens = await _duocPhamVaVatTuBenhVienRepository
                .ApplyFulltext(queryInfo.Query, nameof(DuocPhamVaVatTuBenhVien), lstColumnNameSearch).Where(p => p.HieuLuc)
                .Select(s => new DuocPhamVaVatTuTrongKhoVo
                {
                    Id = s.DuocPhamBenhVienId ?? s.VatTuBenhVienId.Value,
                    LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu
                }).ToListAsync();

            var dctDuocPham = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);
            var dctVatTu = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);


            var queryFullText = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien && p.Id == o.Id))
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now)
                   && (queryInfo.Id == null || queryInfo.Id == 0 || (queryInfo.Id != 0 && o.Id == queryInfo.Id)))
                    .Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
                    {
                        DisplayName = s.DuocPham.Ten,
                        KeyId = s.Id,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh.Ten,
                        DuongDung = s.DuocPham.DuongDung.Ten,
                        SoLuongTon = Math.Round(s.NhapKhoDuocPhamChiTiets
                                                 .Where(nkct =>
                                                  nkct.HanSuDung >= DateTime.Now)
                                                 .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat), 1),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                    nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                   .Select(p => (p.HanSuDung != null ? p.HanSuDung.ApplyFormatDate() : "")).First(),
                        Loai = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien.GetDescription(),
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien && p.Id == o.Id))
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                             kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now)
                            && (queryInfo.Id == null || queryInfo.Id == 0 || (queryInfo.Id != 0 && o.Id == queryInfo.Id)))
                            .Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
                            {

                                DisplayName = s.VatTus.Ten,
                                KeyId = s.Id,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon = Math.Round(s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat), 1),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => (p.HanSuDung != null ? p.HanSuDung.ApplyFormatDate() : "")).First(),
                                Loai = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien.GetDescription(),
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            }))
                .OrderBy(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? (dctDuocPham.Any(a => a.Key == p.KeyId) ? dctDuocPham[p.KeyId] : duocPhamVaVatTuBenhViens.Count) : (dctVatTu.Any(a => a.Key == p.KeyId) ? dctVatTu[p.KeyId] : duocPhamVaVatTuBenhViens.Count))
                .Take(queryInfo.Take);

            return queryFullText.ToList();
        }
        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetThongTinDuocPham(long duocPhamId, long loaiDuocPhamHoacVatTu)
        {
            if (loaiDuocPhamHoacVatTu == 1)
            {
                var resultDuocPham = await _nhapKhoDuocPhamChiTietRepo.TableNoTracking
                .Where(kho =>
                    kho.DuocPhamBenhVienId == duocPhamId &&
                    //kho.LaDuocPhamBHYT == false &&
                    //kho.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                    kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now)
                .Select(x => new ThongTinDuocPhamQuayThuocVo()
                {
                    DuocPhamId = x.DuocPhamBenhViens.DuocPham.Id,
                    MaHoatChat = x.DuocPhamBenhViens.DuocPham.HoatChat,
                    TenDuocPham = x.DuocPhamBenhViens.DuocPham.Ten,
                    SoLuongTon = x.SoLuongNhap - x.SoLuongDaXuat,
                    NhapKhoDuocPhamChiTietId = x.Id,
                    TenHoatChat = x.DuocPhamBenhViens.DuocPham.HoatChat,
                    DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    //SoLuongToa = ,
                    SoLuongMua = 0,
                    DonGiaNhap = x.DonGiaNhap,
                    DonGia = x.DonGiaBan,
                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                    VAT = x.VAT,
                    Solo = x.Solo,
                    ViTri = x.KhoDuocPhamViTri.Ten,
                    HanSuDung = x.HanSuDung,
                    isNew = true,
                    LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                }).ToListAsync();
                return resultDuocPham;
            }
            else
            {
                var resultVatTu = await _nhapKhoVatTuChiTietRepo.TableNoTracking
                .Where(kho =>
                    kho.VatTuBenhVienId == duocPhamId &&
                    //kho.LaVatTuBHYT == false &&
                    //kho.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                    kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now)
               .Select(x => new ThongTinDuocPhamQuayThuocVo()
               {
                   DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                   TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                   SoLuongTon = x.SoLuongNhap - x.SoLuongDaXuat,
                   NhapKhoDuocPhamChiTietId = x.Id,
                   DonViTinh = x.VatTuBenhVien.VatTus.DonViTinh,
                   SoLuongToa = 0,
                   SoLuongMua = 0,
                   DonGiaNhap = x.DonGiaNhap,
                   DonGia = x.DonGiaBan,
                   TiLeTheoThapGia = x.TiLeTheoThapGia,
                   VAT = x.VAT,
                   Solo = x.Solo,
                   ViTri = x.KhoViTri.Ten,
                   HanSuDung = x.HanSuDung,
                   isNew = true,
                   LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien
               }).ToListAsync();
                return resultVatTu;
            }
            return null;
        }
        public async Task<bool> KiemTraSLTonDuocPhamVatTu(double slTon, long? duocPhamId, long? vatTuId)
        {
            bool kq = true;
            if (duocPhamId != null && duocPhamId != 0)
            {
                var ton = _nhapKhoDuocPhamChiTietRepo.TableNoTracking
                         .Where(d => d.DuocPhamBenhVienId == duocPhamId && d.HanSuDung >= DateTime.Now)
                         .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat);
                kq = slTon > ton ? false : true;
            }
            else
            {
                var ton = _nhapKhoVatTuChiTietRepo.TableNoTracking
                         .Where(d => d.VatTuBenhVienId == vatTuId && d.HanSuDung >= DateTime.Now)
                         .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat);
                kq = slTon > ton ? false : true;
            }
            return kq;
        }
        public async Task<bool> KiemTraIsNull(int? LaDuocPham, long? duocPhamId, long? vatTuId)
        {
            bool kq = true;
            if (duocPhamId == null && vatTuId == null)
            {
                kq = false;
                return kq;
            }
            return kq;
        }

        public async Task<List<LookupItemVo>> GetListChuyenKhoaChuyenNganh(DropDownListRequestModel model)
        {
            return await _chuyenKhoaChuyenNganhRepository.TableNoTracking
                .Where(x => x.HieuLuc)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                }).ApplyLike(model.Query, o => o.DisplayName)
                .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName)
                .Take(model.Take)
                .ToListAsync();
        }

        public virtual byte[] ExportDichVuKyThuatBenhVien(GridDataSource gridDataSource)
        {
            var datas = (ICollection<DichVuKyThuatBenhVienGridVo>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DichVuKyThuatBenhVienGridVo>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Dịch Vụ Kỹ Thuật Bệnh Viện");

                    // set row
                    worksheet.DefaultRowHeight = 16;
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 40;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 20;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 20;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 30;
                    worksheet.Column(21).Width = 30;
                    worksheet.Column(22).Width = 30;
                    worksheet.Column(23).Width = 30;
                    worksheet.Column(24).Width = 30;
                    worksheet.Column(25).Width = 20;
                    worksheet.Column(26).Width = 20;
                    worksheet.Column(27).Width = 20;
                    worksheet.Column(28).Width = 20;
                    worksheet.Column(29).Width = 20;
                    worksheet.Column(30).Width = 20;
                    worksheet.Column(31).Width = 20;
                    worksheet.Column(32).Width = 20;
                    worksheet.Column(33).Width = 20;
                    worksheet.Column(34).Width = 20;
                    worksheet.Column(35).Width = 20;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:AI1"])
                    {
                        range.Worksheet.Cells["A1:AI1"].Merge = true;
                        range.Worksheet.Cells["A1:AI1"].Value = "Dịch Vụ Kỹ Thuật Bệnh Viện";
                        range.Worksheet.Cells["A1:AI1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:AI1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:AI1"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A1:AI1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:AI1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2:AI3"])
                    {
                        range.Worksheet.Cells["A2:AI3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:AI3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:AI3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2:AI3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:AI3"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A2:AI3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A2:A3"].Merge = true;
                        range.Worksheet.Cells["A2:A3"].Value = "STT";
                        range.Worksheet.Cells["A2:A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:A3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B2:B3"].Merge = true;
                        range.Worksheet.Cells["B2:B3"].Value = "Ánh xạ";
                        range.Worksheet.Cells["B2:B3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B2:B3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C2:F2"].Merge = true;
                        range.Worksheet.Cells["C2:F2"].Value = "Thông tin dịch vụ kỹ thuật ánh xạ";
                        range.Worksheet.Cells["C2:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C2:F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C3:C3"].Merge = true;
                        range.Worksheet.Cells["C3:C3"].Value = "Mã DV";
                        range.Worksheet.Cells["C3:C3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C3:C3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D3:D3"].Merge = true;
                        range.Worksheet.Cells["D3:D3"].Value = "Tên DVKT Bệnh Viện";
                        range.Worksheet.Cells["D3:D3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D3:D3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E3:E3"].Merge = true;
                        range.Worksheet.Cells["E3:E3"].Value = "Mã 4350";
                        range.Worksheet.Cells["E3:E3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E3:E3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F3:F3"].Merge = true;
                        range.Worksheet.Cells["F3:F3"].Value = "Mô tả";
                        range.Worksheet.Cells["F3:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F3:F3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        //End Merge
                        range.Worksheet.Cells["G2:G3"].Merge = true;
                        range.Worksheet.Cells["G2:G3"].Value = "Mã DV";
                        range.Worksheet.Cells["G2:G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G2:G3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H2:H3"].Merge = true;
                        range.Worksheet.Cells["H2:H3"].Value = "Tên dịch vụ";
                        range.Worksheet.Cells["H2:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H2:H3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I2:I3"].Merge = true;
                        range.Worksheet.Cells["I2:I3"].Value = "Đơn giá BH";
                        range.Worksheet.Cells["I2:I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["I2:I3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J2:J3"].Merge = true;
                        range.Worksheet.Cells["J2:J3"].Value = "TLTT";
                        range.Worksheet.Cells["J2:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["J2:J3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K2:K3"].Merge = true;
                        range.Worksheet.Cells["K2:K3"].Value = "Ngày bắt đầu";
                        range.Worksheet.Cells["K2:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K2:K3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L2:L3"].Merge = true;
                        range.Worksheet.Cells["L2:L3"].Value = "Đơn giá thường";
                        range.Worksheet.Cells["L2:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["L2:L3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M2:M3"].Merge = true;
                        range.Worksheet.Cells["M2:M3"].Value = "Ngày bắt đầu";
                        range.Worksheet.Cells["M2:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["M2:M3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N2:N3"].Merge = true;
                        range.Worksheet.Cells["N2:N3"].Value = "Đơn giá Bảo Việt";
                        range.Worksheet.Cells["N2:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["N2:N3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O2:O3"].Merge = true;
                        range.Worksheet.Cells["O2:O3"].Value = "Ngày bắt đầu";
                        range.Worksheet.Cells["O2:O3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["O2:O3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P2:P3"].Merge = true;
                        range.Worksheet.Cells["P2:P3"].Value = "Đơn giá PVI";
                        range.Worksheet.Cells["P2:P3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["P2:P3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q2:Q3"].Merge = true;
                        range.Worksheet.Cells["Q2:Q3"].Value = "Ngày bắt đầu";
                        range.Worksheet.Cells["Q2:Q3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q2:Q3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        //custom
                        range.Worksheet.Cells["R2:R3"].Merge = true;
                        range.Worksheet.Cells["R2:R3"].Value = "STT thông tư";
                        range.Worksheet.Cells["R2:R3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["R2:R3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S2:S3"].Merge = true;
                        range.Worksheet.Cells["S2:S3"].Value = "Loại PTTT";
                        range.Worksheet.Cells["S2:S3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["S2:S3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T2:T3"].Merge = true;
                        range.Worksheet.Cells["T2:T3"].Value = "Hiệu lực";
                        range.Worksheet.Cells["T2:T3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["T2:T3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["U2:U3"].Merge = true;
                        range.Worksheet.Cells["U2:U3"].Value = "Khoa";
                        range.Worksheet.Cells["U2:U3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["U2:U3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["V2:V3"].Merge = true;
                        range.Worksheet.Cells["V2:V3"].Value = "Nơi thực hiện";
                        range.Worksheet.Cells["V2:V3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["V2:V3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["W2:W3"].Merge = true;
                        range.Worksheet.Cells["W2:W3"].Value = "Nơi thực hiện ưu tiên";
                        range.Worksheet.Cells["W2:W3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["W2:W3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["X2:X3"].Merge = true;
                        range.Worksheet.Cells["X2:X3"].Value = "Chuyên khoa chuyên ngành";
                        range.Worksheet.Cells["X2:X3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["X2:X3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Y2:Y3"].Merge = true;
                        range.Worksheet.Cells["Y2:Y3"].Value = "Quyết định";
                        range.Worksheet.Cells["Y2:Y3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Y2:Y3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Z2:Z3"].Merge = true;
                        range.Worksheet.Cells["Z2:Z3"].Value = "Nơi ban hành";
                        range.Worksheet.Cells["Z2:Z3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Z2:Z3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AA2:AA3"].Merge = true;
                        range.Worksheet.Cells["AA2:AA3"].Value = "Số máy thực hiện";
                        range.Worksheet.Cells["AA2:AA3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["AA2:AA3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AB2:AB3"].Merge = true;
                        range.Worksheet.Cells["AB2:AB3"].Value = "Số CBNV";
                        range.Worksheet.Cells["AB2:AB3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["AB2:AB3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AC2:AC3"].Merge = true;
                        range.Worksheet.Cells["AC2:AC3"].Value = "Số phim XQ";
                        range.Worksheet.Cells["AC2:AC3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["AC2:AC3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AD2:AD3"].Merge = true;
                        range.Worksheet.Cells["AD2:AD3"].Value = "Số ca CP";
                        range.Worksheet.Cells["AD2:AD3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["AD2:AD3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AE2:AE3"].Merge = true;
                        range.Worksheet.Cells["AE2:AE3"].Value = "Mô tả";
                        range.Worksheet.Cells["AE2:AE3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["AE2:AE3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AF2:AF3"].Merge = true;
                        range.Worksheet.Cells["AF2:AF3"].Value = "Kỹ thuật";
                        range.Worksheet.Cells["AF2:AF3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["AF2:AF3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AG2:AG3"].Merge = true;
                        range.Worksheet.Cells["AG2:AG3"].Value = "Loại mẫu xét nghiệm";
                        range.Worksheet.Cells["AG2:AG3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["AG2:AG3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AH2:AH3"].Merge = true;
                        range.Worksheet.Cells["AH2:AH3"].Value = "DV có KQ lâu";
                        range.Worksheet.Cells["AH2:AH3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["AH2:AH3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AI2:AI3"].Merge = true;
                        range.Worksheet.Cells["AI2:AI3"].Value = "DV không KQ";
                        range.Worksheet.Cells["AI2:AI3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["AI2:AI3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<DichVuKyThuatBenhVienGridVo>(requestProperties);

                    var stt = 1;
                    int index = 4;

                    if (datas.Any())
                    {
                        var nhomdichVuKhamBenh = _nhomGiaBenhVienRepository.TableNoTracking.ToList();

                        var nhomThuongId = nhomdichVuKhamBenh.Where(z => z.Ten.ToUpper() == ("Thường").ToUpper()).Select(c => c.Id).FirstOrDefault();
                        var nhomBaoVietId = nhomdichVuKhamBenh.Where(z => z.Ten.ToUpper() == ("Bảo việt").ToUpper()).Select(c => c.Id).FirstOrDefault();
                        var nhomDichVuId = nhomdichVuKhamBenh.Where(z => z.Ten.ToUpper() == ("Dịch vụ").ToUpper()).Select(c => c.Id).FirstOrDefault();

                        foreach (var item in datas)
                        {
                            var dichVuKyThuatBVGiaBHs = item.DichVuKyThuatBenhVienGiaBaoHiems.Where(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || o.DenNgay >= DateTime.Now)).ToList();
                            var dichVuKyThuatBVs = item.DichVuKyThuatBenhVienGiaBenhViens.Where(z => z.TuNgay.Date <= DateTime.Now.Date && (z.DenNgay == null || z.DenNgay >= DateTime.Now)).ToList();

                            var ngayTiLeBaoHiemThanhToan = string.Join("; ", dichVuKyThuatBVGiaBHs.Select(c => c.TuNgay.FormatNgayTimKiemTrenBaoCao()));
                            var tiLeBaoHiemThanhToan = string.Join("; ", dichVuKyThuatBVGiaBHs.Select(c => c.TiLeBaoHiemThanhToan + "%"));

                            var ngayNhomThuong = string.Join("; ", dichVuKyThuatBVs.Where(c => c.NhomGiaDichVuKyThuatBenhVienId == nhomThuongId && c.TuNgay != null).Select(c => c.TuNgay.FormatNgayTimKiemTrenBaoCao()));
                            var giaNhomThuong = string.Join("; ", dichVuKyThuatBVs.Where(c => c.NhomGiaDichVuKyThuatBenhVienId == nhomThuongId).Select(c => c.Gia.ApplyFormatMoneyVND()));

                            var ngayNhomBaoViet = string.Join("; ", dichVuKyThuatBVs.Where(c => c.NhomGiaDichVuKyThuatBenhVienId == nhomBaoVietId && c.TuNgay != null).Select(c => c.TuNgay.FormatNgayTimKiemTrenBaoCao()));
                            var giaNhomBaoViet = string.Join("; ", dichVuKyThuatBVs.Where(c => c.NhomGiaDichVuKyThuatBenhVienId == nhomBaoVietId).Select(c => c.Gia.ApplyFormatMoneyVND()));

                            var ngayNhomDichVu = string.Join("; ", dichVuKyThuatBVs.Where(c => c.NhomGiaDichVuKyThuatBenhVienId == nhomDichVuId && c.TuNgay != null).Select(c => c.TuNgay.FormatNgayTimKiemTrenBaoCao()));
                            var giaNhomDichVu = string.Join("; ", dichVuKyThuatBVs.Where(c => c.NhomGiaDichVuKyThuatBenhVienId == nhomDichVuId).Select(c => c.Gia.ApplyFormatMoneyVND()));

                            using (var range = worksheet.Cells["A" + index + ":AI" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":AI" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":AI" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["A" + index + ":AI" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":AI" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                //Ánh xạ
                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["B" + index].Value = item.AnhXa == true ? "x" : string.Empty;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.AnhXa == true ? item.Ma : string.Empty;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["D" + index].Value = item.AnhXa == true ? item.Ten : string.Empty;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.AnhXa == true ? item.Ma4350 : string.Empty;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.AnhXa == true ? item.MoTa : string.Empty;

                                // Mã dịch vụ
                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.Ma;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["H" + index].Value = item.Ten;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["I" + index].Value = item.GiaBHYT;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["J" + index].Value = tiLeBaoHiemThanhToan;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["K" + index].Value = ngayTiLeBaoHiemThanhToan;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["L" + index].Value = giaNhomThuong;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["M" + index].Value = ngayNhomThuong;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["N" + index].Value = giaNhomBaoViet;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["O" + index].Value = ngayNhomBaoViet;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["P" + index].Value = giaNhomDichVu;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["Q" + index].Value = ngayNhomDichVu;

                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.ThongTu;

                                worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.LoaiPhauThuatThuThuat;

                                worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["T" + index].Value = item.HieuLucHienThi;

                                worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["U" + index].Value = item.Khoas;

                                worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["V" + index].Value = item.TenNoiThucHien;

                                worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["W" + index].Value = item.TenNoiThucHienUuTien;

                                worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["X" + index].Value = item.ChuyenKhoaChuyenNganh;

                                worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Y" + index].Value = item.NghiDinh;

                                worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Z" + index].Value = item.NoiBanHanh;

                                worksheet.Cells["AA" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AA" + index].Value = item.SoMayTT;

                                worksheet.Cells["AB" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AB" + index].Value = item.SoMayCBCM;

                                worksheet.Cells["AC" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AC" + index].Value = item.SoPhimXquang;

                                worksheet.Cells["AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AD" + index].Value = item.SoCaCP;

                                worksheet.Cells["AE" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AE" + index].Value = item.MoTa;

                                worksheet.Cells["AF" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AF" + index].Value = item.TenKyThuat;

                                worksheet.Cells["AG" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AG" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["AG" + index].Value = item.LoaiMauXetNghiemText;

                                worksheet.Cells["AH" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AH" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["AH" + index].Value = item.DichVuCoKetQuaLau != null && item.DichVuCoKetQuaLau != false ? "x" : string.Empty;

                                worksheet.Cells["AI" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AI" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["AI" + index].Value = item.DichVuCoKetQuaLau != null && item.DichVuCoKetQuaLau != true ? "x" : string.Empty; ;

                                index++;
                                stt++;
                            }
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        public async Task<List<LookupItemVo>> GetNhomDichVuTheoDichVuKyThuat(DropDownListRequestModel queryInfo)
        {
            var dsKhamBenhBenhViens = new List<LookupItemVo>();
            var dichVuKyThuatBenhVien = JsonConvert.DeserializeObject<DichVuKyThuatBenhVienJSON>(queryInfo.ParameterDependencies.Replace("undefined", "null"));
            if (dichVuKyThuatBenhVien != null)
            {
                var lst = await _nhomGiaBenhVienRepository.TableNoTracking.Where(c =>
                         c.DichVuKyThuatVuBenhVienGiaBenhViens.Any(x => x.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVien.DichVuKyThuatBenhVienId &&
                         x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date))).ToListAsync();

                dsKhamBenhBenhViens.AddRange(lst.Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id
                }).ToList());
            }
            return dsKhamBenhBenhViens;
        }

        #region BVHD-3937
        public async Task<GiaDichVuBenhVieDataImportVo> XuLyKiemTraDataGiaDichVuBenhVienImportAsync(GiaDichVuBenhVienFileImportVo info)
        {
            var result = new GiaDichVuBenhVieDataImportVo();
            var datas = new List<ThongTinGiaDichVuTuFileExcelVo>();

            #region get data từ file excel
            using (ExcelPackage package = new ExcelPackage(info.Path))
            {
                ExcelWorksheet workSheeGiaDichVu = package.Workbook.Worksheets["Giá dịch vụ"];
                if (workSheeGiaDichVu == null)
                {
                    throw new Exception("Thông tin file không đúng");
                }

                int totalRowKham = workSheeGiaDichVu.Dimension.Rows; // count row có data
                if (totalRowKham >= 2) // row 1 là title, data bắt đầu từ row 2
                {
                    for (int i = 2; i <= totalRowKham; i++)
                    {
                        var dichVu = GanDataTheoRow(workSheeGiaDichVu, i);
                        if (dichVu == null)
                        {
                            break;
                        }
                        datas.Add(dichVu);
                    }
                }
            }
            #endregion

            #region Kiểm tra data
            if (datas.Any())
            {
                await KiemTraDataGiaDichVuBenhVienImportAsync(datas, result);
            }
            #endregion

            return result;
        }

        private ThongTinGiaDichVuTuFileExcelVo GanDataTheoRow(ExcelWorksheet workSheet, int index)
        {
            var dichVu = new ThongTinGiaDichVuTuFileExcelVo()
            {
                MaDichVuBenhVien = workSheet.Cells[index, 1].Text?.Trim(),
                TenDichVuBenhVien = workSheet.Cells[index, 2].Text?.Trim(),
                LoaiGia = workSheet.Cells[index, 3].Text?.Trim(),
                GiaBaoHiem = workSheet.Cells[index, 4].Text?.Trim(),
                TiLeBaoHiemThanhToan = workSheet.Cells[index, 5].Text?.Trim(),
                GiaBenhVien = workSheet.Cells[index, 6].Text?.Trim(),
                TuNgay = workSheet.Cells[index, 7].Text?.Trim(),
                DenNgay = workSheet.Cells[index, 8].Text?.Trim()
            };

            if (string.IsNullOrEmpty(dichVu.MaDichVuBenhVien)
                && string.IsNullOrEmpty(dichVu.TenDichVuBenhVien)
                && string.IsNullOrEmpty(dichVu.LoaiGia)
                && string.IsNullOrEmpty(dichVu.GiaBaoHiem)
                && string.IsNullOrEmpty(dichVu.TiLeBaoHiemThanhToan)
                && string.IsNullOrEmpty(dichVu.GiaBenhVien)
                && string.IsNullOrEmpty(dichVu.TuNgay)
                && string.IsNullOrEmpty(dichVu.DenNgay))
            {
                dichVu = null;
            }
            return dichVu;
        }

        public async Task KiemTraDataGiaDichVuBenhVienImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas, GiaDichVuBenhVieDataImportVo result)
        {
            if (datas.Any())
            {
                var lstLookupDichVu = datas
                    .Select(x => new LookupDichVuBenhVienVo()
                    {
                        MaDichVu = x.MaDichVuBenhVien?.Trim().ToLower(),
                        TenDichVu = x.TenDichVuBenhVien?.Trim().ToLower()
                    }).Distinct().ToList();

                var lstDichVuBenhVien = new List<ThongTinDichVuBenhVienVo>();
                if (lstLookupDichVu.Any())
                {
                    var lstDichVuKyThuatBenhVien = BaseRepository.TableNoTracking
                        .Select(x => new ThongTinDichVuBenhVienVo()
                        {
                            DichVuBenhVienId = x.Id,
                            MaDichVu = x.Ma,
                            TenDichVu = x.Ten
                        }).ToList();
                    lstDichVuKyThuatBenhVien = lstDichVuKyThuatBenhVien
                        .Where(x => lstLookupDichVu.Any(a => a.MaDichVu.Equals(x.MaDichVu.Trim().ToLower())
                                                              && a.TenDichVu.Equals(x.TenDichVu.Trim().ToLower())))
                        .ToList();
                    if (lstDichVuKyThuatBenhVien.Any())
                    {
                        var lstDichVuId = lstDichVuKyThuatBenhVien.Select(x => x.DichVuBenhVienId).Distinct().ToList();
                        var lstGiaBenhVien = _dichVuKyThuatGiaBenhVienRepository.TableNoTracking
                            .Where(x => lstDichVuId.Contains(x.DichVuKyThuatBenhVienId))
                            .Select(a => new ThongTinGiaBenhVienVo()
                            {
                                DichVuBenhVienId = a.DichVuKyThuatBenhVienId,
                                NhomGiaId = a.NhomGiaDichVuKyThuatBenhVienId,
                                TenLoaiGia = a.NhomGiaDichVuKyThuatBenhVien.Ten,
                                Gia = a.Gia,
                                TuNgay = a.TuNgay,
                                DenNgay = a.DenNgay
                            })
                            .ToList();
                        var lstGiaBaoHiem = _dichVuKyThuatGiaBaoHiemRepository.TableNoTracking
                            .Where(x => lstDichVuId.Contains(x.DichVuKyThuatBenhVienId))
                            .Select(a => new ThongTinGiaBaoHiemVo()
                            {
                                DichVuBenhVienId = a.DichVuKyThuatBenhVienId,
                                Gia = a.Gia,
                                TiLeBaoHiemThanhToan = a.TiLeBaoHiemThanhToan,
                                TuNgay = a.TuNgay,
                                DenNgay = a.DenNgay
                            })
                            .ToList();

                        foreach (var dichVuBenhVien in lstDichVuKyThuatBenhVien)
                        {
                            var lstGiaTheoBenhVien = lstGiaBenhVien.Where(x => x.DichVuBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            var lstGiaTheoBaoHiem = lstGiaBaoHiem.Where(x => x.DichVuBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();


                            dichVuBenhVien.ThongTinGiaBenhViens = lstGiaTheoBenhVien;
                            dichVuBenhVien.ThongTinGiaBaoHiems = lstGiaTheoBaoHiem;
                        }
                        lstDichVuBenhVien = lstDichVuBenhVien.Concat(lstDichVuKyThuatBenhVien).ToList();
                    }
                }

                var lstNhomGiaBenhVien = _nhomGiaBenhVienRepository.TableNoTracking
                    .Select(x => new LookupItemVo()
                    {
                        KeyId = x.Id,
                        DisplayName = x.Ten
                    }).ToList();

                #region Message
                var maDichVuBenhVienRequired = _localizationService.GetResource("ImportGiaDichVu.MaDichVuBenhVien.Required"); //"Mã dịch vụ bệnh viện yêu cầu nhập"
                var tenDichVuBenhVienRequired = _localizationService.GetResource("ImportGiaDichVu.TenDichVuBenhVien.Required"); //"Tên dịch vụ bệnh viện yêu cầu nhập"
                var dichVuBenhVienNotExists = _localizationService.GetResource("ImportGiaDichVu.TenDichVuBenhVien.NotExists"); //"Dịch vụ bệnh viện không tồn tại"

                var loaiGiaRequired = _localizationService.GetResource("ImportGiaDichVu.LoaiGia.Required"); // "Loại giá yêu cầu nhập"
                var loaiGiaNotExists = _localizationService.GetResource("ImportGiaDichVu.LoaiGia.NotExists"); //"Loại giá không tồn tại"
                var loaiGiaRequiredGiaBenhVien = _localizationService.GetResource("ImportGiaDichVu.LoaiGia.RequiredGiaBenhVien"); //"Loại giá chỉ nhập đối với giá bệnh viện"

                var giaBaoHiemRequired = _localizationService.GetResource("ImportGiaDichVu.GiaBaoHiem.Required"); // "Giá bảo hiểm yêu cầu nhập"
                var giaBaoHiemOrBenhVienRequired = _localizationService.GetResource("ImportGiaDichVu.GiaBaoHiemOrBenhVien.Required"); //"Yêu cầu nhập giá bảo hiểm hoặc bệnh viện"
                var giaBaoHiemFormat = _localizationService.GetResource("ImportGiaDichVu.GiaBaoHiem.Format"); // "Giá bảo hiểm nhập sai định dạng"

                var tiLeBaoHiemRequired = _localizationService.GetResource("ImportGiaDichVu.TiLeBaoHiem.Required"); // "Tỉ lệ bảo hiểm thanh toán yêu cầu nhập"
                var tiLeBaoHiemOnlyForBaoHiem = _localizationService.GetResource("ImportGiaDichVu.TiLeBaoHiem.OnlyForBaoHiem"); //"Tỉ lệ bảo hiểm thanh toán chỉ nhập với giá bảo hiểm"
                var tiLeBaoHiemFormat = _localizationService.GetResource("ImportGiaDichVu.TiLeBaoHiem.Format"); // "Tỉ lệ bảo hiểm thanh toán nhập sai định dạng"

                var giaBenhVienRequired = _localizationService.GetResource("ImportGiaDichVu.GiaBenhVien.Required"); // "Giá bệnh viện yêu cầu nhập"
                var giaBenhVienFormat = _localizationService.GetResource("ImportGiaDichVu.GiaBenhVien.Format"); // "Giá bệnh viện nhập sai định dạng"

                var tuNgayRequired = _localizationService.GetResource("ImportGiaDichVu.TuNgay.Required"); // "Từ ngày yêu cầu nhập"
                var tuNgayFormat = _localizationService.GetResource("ImportGiaDichVu.TuNgay.Format"); // "Từ ngày nhập sai định dạng"
                var tuNgayLessThanDenNgay = _localizationService.GetResource("ImportGiaDichVu.TuNgay.LessThanDenNgay"); // "Từ ngày phải nhỏ hơn đến ngày"
                var tuNgayInvalid = _localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"); // "Từ ngày không hợp lệ"
                var tuNgayDuplicate = _localizationService.GetResource("ImportGiaDichVu.TuNgay.Duplicate"); // "Từ ngày bị trùng"

                var denNgayFormat = _localizationService.GetResource("ImportGiaDichVu.DenNgay.Format"); // "Đến ngày nhập sai định dạng"
                var denNgayInvalid = _localizationService.GetResource("ImportGiaDichVu.DenNgay.Invalid"); // "Đến ngày không hợp lệ"
                #endregion

                foreach (var dichVu in datas)
                {
                    dichVu.ValidationErrors = new List<ValidationErrorGiaDichVuVo>();

                    #region Gán lại thông tin giá cho data dịch vụ từ file excel

                    var thongTinDichVu = lstDichVuBenhVien.FirstOrDefault(x =>
                        x.MaDichVu.Trim().ToLower().Equals(dichVu.MaDichVuBenhVien.ToLower())
                        && x.TenDichVu.Trim().ToLower().Equals(dichVu.TenDichVuBenhVien.ToLower()));
                    if (thongTinDichVu != null)
                    {
                        dichVu.DichVuBenhVienId = thongTinDichVu.DichVuBenhVienId;

                        if (!string.IsNullOrEmpty(dichVu.LoaiGia))
                        {
                            var loaiGia = lstNhomGiaBenhVien.FirstOrDefault(x =>
                                x.DisplayName.Trim().ToLower().Equals(dichVu.LoaiGia.ToLower()));
                            if (loaiGia != null)
                            {
                                dichVu.LoaiGiaId = loaiGia.KeyId;
                            }

                            //Cập nhật 13/06/2022: Cập nhật theo yêu cầu tạo mới loại giá nếu chưa có
                            else
                            {
                                var newNhomGia = new NhomGiaDichVuKyThuatBenhVien()
                                {
                                    Ten = dichVu.LoaiGia.Trim()
                                };
                                _nhomGiaBenhVienRepository.Add(newNhomGia);
                                dichVu.LoaiGiaId = newNhomGia.Id;
                                lstNhomGiaBenhVien.Add(new LookupItemVo()
                                {
                                    KeyId = newNhomGia.Id,
                                    DisplayName = newNhomGia.Ten
                                });
                            }
                        }
                    }

                    #endregion
                }
                foreach (var dichVu in datas)
                {
                    #region Kiểm tra yêu cầu nhập và format dữ liệu

                    #region Mã dịch vụ bệnh viện
                    if (string.IsNullOrEmpty(dichVu.MaDichVuBenhVien))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                        {
                            Field = nameof(dichVu.MaDichVuBenhVien),
                            Message = maDichVuBenhVienRequired
                        });
                    }
                    #endregion

                    #region Tên dịch vụ bệnh viện
                    if (string.IsNullOrEmpty(dichVu.TenDichVuBenhVien))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                        {
                            Field = nameof(dichVu.TenDichVuBenhVien),
                            Message = tenDichVuBenhVienRequired
                        });
                    }
                    else
                    {
                        if (dichVu.DichVuBenhVienId == null)
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.TenDichVuBenhVien),
                                Message = dichVuBenhVienNotExists
                            });
                        }
                    }
                    #endregion

                    #region Loại giá
                    if (string.IsNullOrEmpty(dichVu.LoaiGia))
                    {
                        if (!string.IsNullOrEmpty(dichVu.GiaBenhVien))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.LoaiGia),
                                Message = loaiGiaRequired
                            });
                        }
                    }
                    else
                    {
                        if (!lstNhomGiaBenhVien.Any(x => x.DisplayName.ToLower().Equals(dichVu.LoaiGia.ToLower())))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.LoaiGia),
                                Message = loaiGiaNotExists
                            });
                        }
                        else if (!string.IsNullOrEmpty(dichVu.GiaBaoHiem))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.LoaiGia),
                                Message = loaiGiaRequiredGiaBenhVien
                            });
                        }
                    }
                    #endregion

                    #region Giá bảo hiểm
                    if (string.IsNullOrEmpty(dichVu.GiaBaoHiem))
                    {
                        if (!string.IsNullOrEmpty(dichVu.TiLeBaoHiemThanhToan))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.GiaBaoHiem),
                                Message = giaBaoHiemRequired
                            });
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(dichVu.LoaiGia) && string.IsNullOrEmpty(dichVu.GiaBenhVien))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                {
                                    Field = nameof(dichVu.GiaBaoHiem),
                                    Message = giaBaoHiemOrBenhVienRequired
                                });
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dichVu.LoaiGia) || !string.IsNullOrEmpty(dichVu.GiaBenhVien))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.GiaBaoHiem),
                                Message = giaBaoHiemOrBenhVienRequired
                            });
                        }
                        else
                        {
                            var isNumeric = decimal.TryParse(dichVu.GiaBaoHiem, out decimal giaBaoHiem);
                            if (isNumeric)
                            {
                                dichVu.GiaBaoHiemValue = giaBaoHiem;
                            }
                            else
                            {
                                if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.GiaBaoHiem)))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                    {
                                        Field = nameof(dichVu.GiaBaoHiem),
                                        Message = giaBaoHiemFormat
                                    });
                                }
                            }
                        }
                    }
                    #endregion

                    #region Tỉ lệ bảo hiểm thanh toán
                    if (string.IsNullOrEmpty(dichVu.TiLeBaoHiemThanhToan))
                    {
                        if (!string.IsNullOrEmpty(dichVu.GiaBaoHiem))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.TiLeBaoHiemThanhToan),
                                Message = tiLeBaoHiemRequired
                            });
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dichVu.LoaiGia) || !string.IsNullOrEmpty(dichVu.GiaBenhVien))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.TiLeBaoHiemThanhToan),
                                Message = tiLeBaoHiemOnlyForBaoHiem
                            });
                        }
                        else
                        {
                            var isNumeric = int.TryParse(dichVu.TiLeBaoHiemThanhToan, out int tlbhtt);
                            if (isNumeric)
                            {
                                dichVu.TiLeBaoHiemThanhToanValue = tlbhtt;
                            }
                            else
                            {
                                if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TiLeBaoHiemThanhToan)))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                    {
                                        Field = nameof(dichVu.TiLeBaoHiemThanhToan),
                                        Message = tiLeBaoHiemFormat
                                    });
                                }
                            }
                        }
                    }
                    #endregion

                    #region Giá bệnh viện
                    if (string.IsNullOrEmpty(dichVu.GiaBenhVien))
                    {
                        if (!string.IsNullOrEmpty(dichVu.LoaiGia))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.GiaBenhVien),
                                Message = giaBenhVienRequired
                            });
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dichVu.LoaiGia))
                        {
                            var isNumeric = decimal.TryParse(dichVu.GiaBenhVien, out decimal giaBenhVien);
                            if (isNumeric)
                            {
                                dichVu.GiaBenhVienValue = giaBenhVien;
                            }
                            else
                            {
                                if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.GiaBenhVien)))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                    {
                                        Field = nameof(dichVu.GiaBenhVien),
                                        Message = giaBenhVienFormat
                                    });
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(dichVu.GiaBaoHiem) || !string.IsNullOrEmpty(dichVu.TiLeBaoHiemThanhToan))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.GiaBenhVien),
                                Message = giaBaoHiemOrBenhVienRequired
                            });
                        }
                    }
                    #endregion

                    #region Từ ngày
                    if (string.IsNullOrEmpty(dichVu.TuNgay))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                        {
                            Field = nameof(dichVu.TuNgay),
                            Message = tuNgayRequired
                        });
                    }
                    else
                    {
                        //var isDate = DateTime.TryParse(dichVu.TuNgay, out DateTime tuNgay);
                        DateTime tuNgay;
                        var strNgay = KiemTraFormatNgay(dichVu.TuNgay);

                        var isDate = DateTime.TryParseExact(strNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tuNgay);
                        if (isDate)
                        {
                            dichVu.TuNgayValue = tuNgay;
                        }
                        else
                        {
                            if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                {
                                    Field = nameof(dichVu.TuNgay),
                                    Message = tuNgayFormat
                                });
                            }
                        }
                    }
                    #endregion

                    #region Đến ngày
                    if (!string.IsNullOrEmpty(dichVu.DenNgay))
                    {
                        //var isDate = DateTime.TryParse(dichVu.DenNgay, out DateTime denNgay);
                        DateTime denNgay;
                        var strNgay = KiemTraFormatNgay(dichVu.DenNgay);

                        var isDate = DateTime.TryParseExact(strNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out denNgay);
                        if (isDate)
                        {
                            dichVu.DenNgayValue = denNgay;
                        }
                        else
                        {
                            if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.DenNgay)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                {
                                    Field = nameof(dichVu.DenNgay),
                                    Message = denNgayFormat
                                });
                            }
                        }
                    }
                    #endregion

                    #region Từ ngày đến ngày
                    if (dichVu.TuNgayValue != null && dichVu.DenNgayValue != null && dichVu.TuNgayValue >= dichVu.DenNgayValue)
                    {
                        if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.TuNgay),
                                Message = tuNgayLessThanDenNgay
                            });
                        }
                    }
                    #endregion

                    #region Kiểm tra thời gian hiệu lực theo từng dịch vụ
                    var dichVuBenhVien = lstDichVuBenhVien.FirstOrDefault(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId);
                    if (dichVuBenhVien != null)
                    {
                        if (dichVu.LoaiGiaId != null)
                        {
                            var giaBenhVienTheoDichVuHienTai = dichVuBenhVien.ThongTinGiaBenhViens.Where(x => x.NhomGiaId == dichVu.LoaiGiaId).OrderByDescending(x => x.TuNgay).FirstOrDefault();
                            if (giaBenhVienTheoDichVuHienTai != null)
                            {
                                // nếu từ ngày excel = từ ngày hiện tại => gán đến ngày theo file excel
                                // nếu từ ngày excel != từ ngày hiện tại => gán đến ngày = từ ngày file excel -1
                                //if (dichVu.TuNgayValue <= giaBenhVienTheoDichVuHienTai.TuNgay.AddDays(1))

                                var tuNgayHopLe = (dichVu.TuNgayValue == giaBenhVienTheoDichVuHienTai.TuNgay)
                                                  || (dichVu.TuNgayValue != giaBenhVienTheoDichVuHienTai.TuNgay
                                                      && dichVu.TuNgayValue > giaBenhVienTheoDichVuHienTai.TuNgay.AddDays(1));
                                if (!tuNgayHopLe)
                                {
                                    if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                                    {
                                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                        {
                                            Field = nameof(dichVu.TuNgay),
                                            Message = tuNgayInvalid
                                        });
                                    }
                                }

                                if (dichVu.DenNgayValue != null && dichVu.DenNgayValue <= giaBenhVienTheoDichVuHienTai.TuNgay.AddDays(1))
                                {
                                    if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.DenNgay)))
                                    {
                                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                        {
                                            Field = nameof(dichVu.DenNgay),
                                            Message = denNgayInvalid
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            var giaBaoHiemTheoDichVuHienTai = dichVuBenhVien.ThongTinGiaBaoHiems.OrderByDescending(x => x.TuNgay).FirstOrDefault();
                            if (giaBaoHiemTheoDichVuHienTai != null)
                            {
                                // nếu từ ngày excel = từ ngày hiện tại => gán đến ngày theo file excel
                                // nếu từ ngày excel != từ ngày hiện tại => gán đến ngày = từ ngày file excel -1
                                //if (dichVu.TuNgayValue <= giaBaoHiemTheoDichVuHienTai.TuNgay.AddDays(1))

                                var tuNgayHopLe = (dichVu.TuNgayValue == giaBaoHiemTheoDichVuHienTai.TuNgay)
                                                  || (dichVu.TuNgayValue != giaBaoHiemTheoDichVuHienTai.TuNgay
                                                      && dichVu.TuNgayValue > giaBaoHiemTheoDichVuHienTai.TuNgay.AddDays(1));
                                if (!tuNgayHopLe)
                                {
                                    if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                                    {
                                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                        {
                                            Field = nameof(dichVu.TuNgay),
                                            Message = tuNgayInvalid
                                        });
                                    }
                                }

                                if (dichVu.DenNgayValue != null && dichVu.DenNgayValue <= giaBaoHiemTheoDichVuHienTai.TuNgay.AddDays(1))
                                {
                                    if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.DenNgay)))
                                    {
                                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                        {
                                            Field = nameof(dichVu.DenNgay),
                                            Message = denNgayInvalid
                                        });
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion
                }

                #region Kiểm tra thời gian hiệu lực theo từng dịch vụ import
                foreach (var dichVu in datas)
                {
                    var dichVuBenhVien =
                        lstDichVuBenhVien.FirstOrDefault(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId);
                    if (dichVuBenhVien != null)
                    {
                        if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                        {
                            bool laGiaBenhVien = (dichVu.LoaiGiaId != null || dichVu.GiaBenhVienValue != null)
                                                 && dichVu.GiaBaoHiemValue == null
                                                 && dichVu.TiLeBaoHiemThanhToanValue == null;
                            bool laGiaBaoHiem = (dichVu.GiaBaoHiemValue != null || dichVu.TiLeBaoHiemThanhToanValue != null)
                                                && dichVu.LoaiGiaId == null
                                                && dichVu.GiaBenhVienValue == null;

                            var lstGiaBaoHiemImport = datas.Where(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId
                                                                       && x.TuNgayValue != null
                                                                       && x.LoaiGiaId == null
                                                                       && x.GiaBenhVienValue == null
                                                                       && (x.GiaBaoHiemValue != null
                                                                           || x.TiLeBaoHiemThanhToanValue != null))
                                .ToList();

                            var lstGiaBenhVienImport = datas.Where(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId
                                                                       && x.TuNgayValue != null
                                                                       && x.LoaiGiaId == dichVu.LoaiGiaId
                                                                       && x.GiaBaoHiemValue == null
                                                                       && x.TiLeBaoHiemThanhToanValue == null
                                                                       && (x.LoaiGiaId != null || x.GiaBenhVienValue != null))
                                .ToList();

                            if ((laGiaBaoHiem && lstGiaBaoHiemImport.GroupBy(x => new { x.TuNgayValue }).Any(x => x.Count() > 1))
                                || (laGiaBenhVien && lstGiaBenhVienImport.GroupBy(x => new { x.TuNgayValue }).Any(x => x.Count() > 1)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                {
                                    Field = nameof(dichVu.TuNgay),
                                    Message = tuNgayDuplicate
                                });
                            }
                            else
                            {
                                // giữa 2 dòng import giá của cùng 1 dịch vụ, thì từ ngày sau phải cách từ ngày trước ít nhất 1 ngày
                                var tuNgayBenhVienTruocs = lstGiaBenhVienImport.Select(x => x.TuNgayValue.Value)
                                    .Where(x => x <= dichVu.TuNgayValue.Value)
                                    .OrderByDescending(x => x).Skip(1).ToList();
                                var tuNgayBaoHiemTruocs = lstGiaBaoHiemImport.Select(x => x.TuNgayValue.Value)
                                    .Where(x => x <= dichVu.TuNgayValue.Value)
                                    .OrderByDescending(x => x).Skip(1).ToList();

                                if ((laGiaBenhVien && tuNgayBenhVienTruocs.Any() && tuNgayBenhVienTruocs.First().AddDays(1) >= dichVu.TuNgayValue.Value)
                                    || (laGiaBaoHiem && tuNgayBaoHiemTruocs.Any() && tuNgayBaoHiemTruocs.First().AddDays(1) >= dichVu.TuNgayValue.Value))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                    {
                                        Field = nameof(dichVu.TuNgay),
                                        Message = tuNgayInvalid
                                    });
                                }
                            }
                        }
                    }
                }
                #endregion

                result.DuLieuDungs = datas.Where(x => !x.ValidationErrors.Any()).OrderBy(x => x.MaDichVuBenhVien).ToList();
                result.DuLieuSais = datas.Where(x => x.ValidationErrors.Any()).OrderBy(x => x.MaDichVuBenhVien).ToList();
            }
        }

        private bool KiemTraDaCoValidationErrors(List<ValidationErrorGiaDichVuVo> validationErrors, string filed)
        {
            return validationErrors.Any(a => a.Field.Equals(filed));
        }

        private string KiemTraFormatNgay(string ngayKiemTra)
        {
            var ngaySauKiemTra = ngayKiemTra;
            var arrTuNgay = ngayKiemTra.Split("/");
            if (arrTuNgay.Length == 3)
            {
                var ngay = arrTuNgay[0];
                var thang = arrTuNgay[1];
                if (ngay.Length == 1)
                {
                    ngay = $"0{ngay}";
                }
                if (thang.Length == 1)
                {
                    thang = $"0{thang}";
                }

                ngaySauKiemTra = $"{ngay}/{thang}/{arrTuNgay[2]}";
            }

            return ngaySauKiemTra;
        }

        public async Task XuLyLuuGiaDichVuImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas)
        {
            var lstDichVuBenhVienId = datas.Where(x => x.DichVuBenhVienId != null)
                .Select(x => x.DichVuBenhVienId.Value).Distinct().ToList();
            if (lstDichVuBenhVienId.Any())
            {
                var lstDichVuBenhVien = BaseRepository.Table
                    .Include(x => x.DichVuKyThuatBenhVienGiaBaoHiems)
                    .Include(x => x.DichVuKyThuatVuBenhVienGiaBenhViens)
                    .Where(x => lstDichVuBenhVienId.Contains(x.Id))
                    .ToList();
                foreach (var dichVuBenhVien in lstDichVuBenhVien)
                {
                    var lstGiaBaoHiemImport = datas.Where(x => x.DichVuBenhVienId == dichVuBenhVien.Id
                                                               && x.GiaBaoHiemValue != null
                                                               && x.TiLeBaoHiemThanhToanValue != null)
                        .OrderBy(x => x.TuNgayValue).ToList();
                    var lstGiaBenhVienImport = datas.Where(x => x.DichVuBenhVienId == dichVuBenhVien.Id
                                                                && x.LoaiGiaId != null
                                                                && x.GiaBenhVienValue != null)
                        .OrderBy(x => x.TuNgayValue).ToList();

                    #region Xử lý giá Bảo hiểm
                    var giaBaoHiemCuoiCungHienTai = dichVuBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.OrderByDescending(x => x.TuNgay).FirstOrDefault();
                    if (lstGiaBaoHiemImport.Any())
                    {
                        var tuNgayHopLe = giaBaoHiemCuoiCungHienTai == null
                                          || lstGiaBaoHiemImport.Any(x => x.TuNgayValue == giaBaoHiemCuoiCungHienTai?.TuNgay)
                                          || lstGiaBaoHiemImport.Any(x => x.TuNgayValue != giaBaoHiemCuoiCungHienTai?.TuNgay
                                                                          && x.TuNgayValue > giaBaoHiemCuoiCungHienTai?.TuNgay.AddDays(1));

                        //if (giaBaoHiemCuoiCungHienTai != null && lstGiaBaoHiemImport.Any(x => x.TuNgayValue <= giaBaoHiemCuoiCungHienTai.TuNgay.AddDays(1)))
                        if (!tuNgayHopLe)
                        {
                            throw new Exception(_localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"));
                        }

                        //xử lý gán giá trị từ ngày đến ngày
                        foreach (var item in lstGiaBaoHiemImport.Select((value, index) => new { index, value }))
                        {
                            // đối với dòng giá import đầu tiên, thì so sánh với từ ngày giá trong DB
                            // đối với các dòng giá từ thứ 2 trở đi, thì so sánh với từ ngày của giá trước nó
                            if (item.index == 0)
                            {
                                if (giaBaoHiemCuoiCungHienTai != null)
                                {
                                    if (item.value.TiLeBaoHiemThanhToanValue == giaBaoHiemCuoiCungHienTai.TiLeBaoHiemThanhToan
                                        && item.value.GiaBaoHiemValue == giaBaoHiemCuoiCungHienTai.Gia)
                                    {
                                        giaBaoHiemCuoiCungHienTai.DenNgay = item.value.DenNgayValue;
                                        item.value.LaCapNhatDenNgayTruocDo = true;
                                    }
                                    else
                                    {
                                        //giaBaoHiemCuoiCungHienTai.DenNgay = item.value.TuNgayValue.Value.AddDays(-1);
                                        if (item.value.TuNgayValue == giaBaoHiemCuoiCungHienTai.TuNgay)
                                        {
                                            giaBaoHiemCuoiCungHienTai.DenNgay = DateTime.Now.Date.AddDays(-1);
                                            item.value.TuNgayValue = DateTime.Now.Date;
                                            if (item.value.DenNgayValue != null && item.value.TuNgayValue.Value.AddDays(1) >= item.value.DenNgayValue)
                                            {
                                                throw new Exception(_localizationService.GetResource("ImportGiaDichVu.DenNgay.Invalid"));
                                            }
                                        }
                                        else
                                        {
                                            giaBaoHiemCuoiCungHienTai.DenNgay = item.value.TuNgayValue.Value.AddDays(-1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (item.value.TuNgayValue <= lstGiaBaoHiemImport[item.index - 1].TuNgayValue.Value.AddDays(1))
                                {
                                    throw new Exception(_localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"));
                                }
                                else
                                {
                                    lstGiaBaoHiemImport[item.index - 1].DenNgayValue = item.value.TuNgayValue.Value.AddDays(-1);
                                    if (giaBaoHiemCuoiCungHienTai != null && lstGiaBaoHiemImport[item.index - 1].LaCapNhatDenNgayTruocDo)
                                    {
                                        giaBaoHiemCuoiCungHienTai.DenNgay = lstGiaBaoHiemImport[item.index - 1].DenNgayValue;
                                    }
                                }
                            }
                        }

                        foreach (var gia in lstGiaBaoHiemImport.Where(x => !x.LaCapNhatDenNgayTruocDo))
                        {
                            dichVuBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.Add(new DichVuKyThuatBenhVienGiaBaoHiem()
                            {
                                Gia = gia.GiaBaoHiemValue.Value,
                                TiLeBaoHiemThanhToan = gia.TiLeBaoHiemThanhToanValue.Value,
                                TuNgay = gia.TuNgayValue.Value,
                                DenNgay = gia.DenNgayValue
                            });
                        }
                    }

                    #endregion

                    #region Xử lý giá Bệnh viện
                    if (lstGiaBenhVienImport.Any())
                    {
                        //xử lý gán giá trị từ ngày đến ngày
                        var lstLoaiGiaId = lstGiaBenhVienImport.Select(x => x.LoaiGiaId.Value).Distinct().ToList();
                        foreach (var loaiGiaId in lstLoaiGiaId)
                        {
                            var lstGiaBenhVienImportTheoLoaiGia = lstGiaBenhVienImport.Where(x => x.LoaiGiaId == loaiGiaId).ToList();
                            foreach (var item in lstGiaBenhVienImportTheoLoaiGia.Select((value, index) => new { index, value }))
                            {
                                var giaBenhVienCuoiCungHienTaiTheoLoaiGia = dichVuBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens
                                    .Where(x => x.NhomGiaDichVuKyThuatBenhVienId == item.value.LoaiGiaId)
                                    .OrderByDescending(x => x.TuNgay).FirstOrDefault();

                                var tuNgayHopLe = giaBenhVienCuoiCungHienTaiTheoLoaiGia == null
                                                  || (item.value.TuNgayValue == giaBenhVienCuoiCungHienTaiTheoLoaiGia?.TuNgay)
                                                  || (item.value.TuNgayValue != giaBenhVienCuoiCungHienTaiTheoLoaiGia?.TuNgay
                                                      && item.value.TuNgayValue > giaBenhVienCuoiCungHienTaiTheoLoaiGia?.TuNgay.AddDays(1));

                                //if (giaBenhVienCuoiCungHienTaiTheoLoaiGia != null && item.value.TuNgayValue <= giaBenhVienCuoiCungHienTaiTheoLoaiGia.TuNgay.AddDays(1))
                                if (!tuNgayHopLe)
                                {
                                    throw new Exception(_localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"));
                                }

                                // đối với dòng giá import đầu tiên, thì so sánh với từ ngày giá trong DB
                                // đối với các dòng giá từ thứ 2 trở đi, thì so sánh với từ ngày của giá trước nó
                                if (item.index == 0)
                                {
                                    if (giaBenhVienCuoiCungHienTaiTheoLoaiGia != null)
                                    {
                                        if (item.value.GiaBenhVienValue == giaBenhVienCuoiCungHienTaiTheoLoaiGia.Gia)
                                        {
                                            giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = item.value.DenNgayValue;
                                            item.value.LaCapNhatDenNgayTruocDo = true;
                                        }
                                        else
                                        {
                                            //giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = item.value.TuNgayValue.Value.AddDays(-1);
                                            if (item.value.TuNgayValue == giaBenhVienCuoiCungHienTaiTheoLoaiGia.TuNgay)
                                            {
                                                giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = DateTime.Now.Date.AddDays(-1);
                                                item.value.TuNgayValue = DateTime.Now.Date;
                                                if (item.value.DenNgayValue != null && item.value.TuNgayValue.Value.AddDays(1) >= item.value.DenNgayValue)
                                                {
                                                    throw new Exception(_localizationService.GetResource("ImportGiaDichVu.DenNgay.Invalid"));
                                                }
                                            }
                                            else
                                            {
                                                giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = item.value.TuNgayValue.Value.AddDays(-1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (item.value.TuNgayValue <= lstGiaBenhVienImportTheoLoaiGia[item.index - 1].TuNgayValue.Value.AddDays(1))
                                    {
                                        throw new Exception(_localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"));
                                    }
                                    else
                                    {
                                        lstGiaBenhVienImportTheoLoaiGia[item.index - 1].DenNgayValue = item.value.TuNgayValue.Value.AddDays(-1);
                                        if (giaBenhVienCuoiCungHienTaiTheoLoaiGia != null && lstGiaBenhVienImportTheoLoaiGia[item.index - 1].LaCapNhatDenNgayTruocDo)
                                        {
                                            giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = lstGiaBenhVienImportTheoLoaiGia[item.index - 1].DenNgayValue;
                                        }
                                    }
                                }
                            }
                        }

                        foreach (var gia in lstGiaBenhVienImport.Where(x => !x.LaCapNhatDenNgayTruocDo))
                        {
                            dichVuBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.Add(new DichVuKyThuatBenhVienGiaBenhVien()
                            {
                                NhomGiaDichVuKyThuatBenhVienId = gia.LoaiGiaId.Value,
                                Gia = gia.GiaBenhVienValue.Value,
                                TuNgay = gia.TuNgayValue.Value,
                                DenNgay = gia.DenNgayValue
                            });
                        }
                    }
                    #endregion
                }

                BaseRepository.Context.SaveChanges();
            }
        }
        #endregion
        #region BVHD-3961
        public async Task<List<KhoaKhamTemplateVo>> GetListKhoaPhongAll(DropDownListRequestModel queryInfo)
        {
            var listKhoaPhong = await _khoaPhongRepository.TableNoTracking
                .Where(p => p.IsDisabled != true)
                .ApplyLike(queryInfo.Query, g => g.Ma, g => g.Ten)
                .Take(queryInfo.Take)
                .ToListAsync();

            var query = listKhoaPhong.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();
            query.Add(new KhoaKhamTemplateVo
            {
                DisplayName = "Nhà thuốc",
                KeyId = -1,
                Ten = "Nhà thuốc"
            });
            query.Insert(0, new KhoaKhamTemplateVo
            {
                DisplayName = "Toàn viện",
                KeyId = 0,
                Ten = "Toàn viện"
            });

            return query;
        }
        #endregion
    }
}