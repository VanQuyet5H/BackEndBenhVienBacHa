using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.GoiDichVu;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.GoiDichVus
{
    [ScopedDependency(ServiceType = typeof(IGoiDvService))]
    public class GoiDvService : MasterFileService<GoiDichVu>, IGoiDvService
    {
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> _dichVuGiuongBenhVienRepository;
        private readonly IRepository<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private readonly IRepository<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;
        private readonly IRepository<DichVuGiuongBenhVienGiaBenhVien> _dichVuGiuongBenhVienGiaBenhVienRepository;
        private readonly IRepository<GoiDichVu> _goiDichVuRepository;
        private readonly IRepository<NhomGiaDichVuKyThuatBenhVien> _nhomGiaDichVuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.NhomGiaDichVuGiuongBenhVien> _nhomGiaDichVuGiuongBenhVienRepository;
        private readonly ICauHinhService _cauHinhService;

        public GoiDvService(
            IRepository<GoiDichVu> repository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> dichVuGiuongBenhVienRepository,
            IRepository<DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository,
            IRepository<DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository,
            IRepository<DichVuGiuongBenhVienGiaBenhVien> dichVuGiuongBenhVienGiaBenhVienRepository,
            IRepository<GoiDichVu> goiDichVuRepository,
            ICauHinhService cauHinhService,
            IRepository<NhomGiaDichVuKyThuatBenhVien> nhomGiaDichVuKyThuatBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.NhomGiaDichVuGiuongBenhVien> nhomGiaDichVuGiuongBenhVienRepository
        ) : base(repository)
        {
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _dichVuGiuongBenhVienRepository = dichVuGiuongBenhVienRepository;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
            _dichVuGiuongBenhVienGiaBenhVienRepository = dichVuGiuongBenhVienGiaBenhVienRepository;
            _goiDichVuRepository = goiDichVuRepository;
            _nhomGiaDichVuKyThuatBenhVienRepository = nhomGiaDichVuKyThuatBenhVienRepository;
            _nhomGiaDichVuGiuongBenhVienRepository = nhomGiaDichVuGiuongBenhVienRepository;
            _cauHinhService = cauHinhService;
        }

        public async Task<List<DichVuKyThuatChoBenhVienTemplateVo>> GetListDichVuKyThuat(DropDownListRequestModel model)
        {
            var listDichVuKyThuats = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Include(p => p.DichVuKyThuat)
                .Where(p => (p.Ten.Contains(model.Query ?? "") ||
                             p.Ma.Contains(model.Query ?? "")) && p.HieuLuc)
                .Take(model.Take)
                .ToListAsync();
            var query = listDichVuKyThuats.Select(item => new DichVuKyThuatChoBenhVienTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ma = item.Ma,
                Ten = item.Ten
            }).ToList();

            return query;
        }

        public async Task<List<DichVuGiuongTemplateVo>> GetListDichVuGiuong(DropDownListRequestModel model)
        {
            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add("Ten");
            lstColumnNameSearch.Add("Ma");

            var listDuocPhams = await _dichVuGiuongBenhVienRepository
                .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien), lstColumnNameSearch)
                .Include(p => p.DichVuGiuong)
                .Where(p => p.HieuLuc)
                .Take(model.Take)
                .ToListAsync();

            var query = listDuocPhams.Select(item => new DichVuGiuongTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();
            return query;
        }

        public async Task<long> GetChiPhiHienTaiDichVuKhamBenh(long dichVuKhamBenhBenhVienId, long nhomGiaDichVuKhamBenhBenhVienId)
        {
            var now = DateTime.Now;

            var chiPhiHienTai = Convert.ToInt64(_dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(p => p.NhomGiaDichVuKhamBenhBenhVienId == nhomGiaDichVuKhamBenhBenhVienId &&
                            p.DichVuKhamBenhBenhVienId == dichVuKhamBenhBenhVienId &&
                            p.TuNgay.Date <= now.Date && (p.DenNgay.Value.Date >= now.Date || p.DenNgay == null))
                .Select(p => p.Gia).LastOrDefault());

            return await Task.FromResult(chiPhiHienTai);
        }

        public async Task<long> GetChiPhiHienTaiDichVuKyThuat(long dichVuKyThuatBenhVienId,
            long nhomGiaDichVuKyThuatBenhVienId)
        {
            var now = DateTime.Now;

            var chiPhiHienTai = Convert.ToInt64(_dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(p => p.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId &&
                            p.NhomGiaDichVuKyThuatBenhVienId == nhomGiaDichVuKyThuatBenhVienId &&
                            p.TuNgay.Date <= now.Date && (p.DenNgay.Value.Date >= now.Date || p.DenNgay == null))
                .Select(p => p.Gia).LastOrDefault());

            return await Task.FromResult(chiPhiHienTai);
        }

        public async Task<long> GetChiPhiChoDichVuGiuong(long dichVuGiuongBenhVienId, long nhomGiaId)
        {
            var now = DateTime.Now;

            var chiPhiHienTai = Convert.ToInt64(_dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(p => p.DichVuGiuongBenhVienId == dichVuGiuongBenhVienId &&
                            p.NhomGiaDichVuGiuongBenhVienId == nhomGiaId &&
                            p.TuNgay.Date <= now.Date && (p.DenNgay.Value.Date >= now.Date || p.DenNgay == null))
                .Select(p => p.Gia).LastOrDefault());

            return await Task.FromResult(chiPhiHienTai);
        }

        public GridDataSource GetDataForGridChiTietAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var count = 1;
            var goiDichVuKhamBenh = new List<GoiDichVuKhongCoChietKhauGridVo>();
            GoiDichVu goiDichVu;

            if (queryInfo.ModifiedBy == 69)
            {
                goiDichVu = _goiDichVuRepository.TableNoTracking
                                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenh)
                                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

                                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuong)
                                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)

                                    //.Include(p => p.GoiDichVuChiTietVatTus).ThenInclude(p => p.VatTuBenhVien).ThenInclude(p => p.VatTus)

                                    //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham)
                                    //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.NhapKhoDuocPhamChiTiets)
                                    .Where(p => p.Id == Convert.ToInt64(queryInfo.AdditionalSearchString)).FirstOrDefault();
            }
            else
            {
                goiDichVu = _goiDichVuRepository.TableNoTracking
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenh)
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuong)
                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)

                    //.Include(p => p.GoiDichVuChiTietVatTus).ThenInclude(p => p.VatTuBenhVien).ThenInclude(p => p.VatTus)

                    //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham)
                    //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.NhapKhoDuocPhamChiTiets)
                    .Where(p => p.Id == Convert.ToInt64(queryInfo.SearchTerms)).FirstOrDefault();
            }
            goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietDichVuKhamBenhs.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            {
                Id = count++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                Ma = p.DichVuKhamBenhBenhVien?.Ma,
                TenDichVu = p.DichVuKhamBenhBenhVien?.Ten,
                LoaiGia = p.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                SoLuong = 1,
                DonGia = p.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens
                                  .Where(x => x.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVien?.Id && x.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVien?.Id && x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                           || x.DenNgay == null))
                                  .LastOrDefault() != null ?
                    p.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens
                        .Where(x => x.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVien?.Id && x.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVien?.Id && x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                                                                                                                                                           || x.DenNgay == null))
                        .LastOrDefault()
                ?.Gia : 0,

                ThanhTien = p.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens
                                .Where(x => x.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVien?.Id && x.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVien?.Id && x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                                                                                                                                                                   || x.DenNgay == null))
                                .LastOrDefault() != null ?
                    p.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens
                        .Where(x => x.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVien?.Id && x.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVien?.Id && x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                                                                                                                                                           || x.DenNgay == null))
                        .LastOrDefault()
                        ?.Gia : 0,
                IdDichVuKhac = p.DichVuKhamBenhBenhVien?.Id
            }));

            goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietDichVuKyThuats.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            {
                Id = count++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                Ma = p.DichVuKyThuatBenhVien.Ma,
                TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                LoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                SoLuong = p.SoLan,
                DonGia = p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens?
                                 .Where(x => x.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId && x.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId
                                                                                                    && x.TuNgay <= DateTime.Now && (x.DenNgay > DateTime.Now
                                          || x.DenNgay == null) && x.TuNgay < DateTime.Now)
                                 .LastOrDefault() != null ? p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens?
                    .Where(x => x.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId && x.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId
                                                                                       && x.TuNgay <= DateTime.Now && (x.DenNgay > DateTime.Now
                                         || x.DenNgay == null) && x.TuNgay < DateTime.Now)
                    .LastOrDefault()
                    ?.Gia : 0,

                ThanhTien = p.SoLan * (p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens?
                                           .Where(x => x.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId && x.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId
                                                                                                              && x.TuNgay <= DateTime.Now && (x.DenNgay > DateTime.Now
                                                                                                                                              || x.DenNgay == null) && x.TuNgay < DateTime.Now)
                                           .LastOrDefault() != null ? p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens?
                                .Where(x => x.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId && x.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId
                                                                                                   && x.TuNgay <= DateTime.Now && (x.DenNgay > DateTime.Now
                                                                                                                                   || x.DenNgay == null) && x.TuNgay < DateTime.Now)
                                .LastOrDefault()
                                ?.Gia : 0),
                IdDichVuKhac = p.DichVuKyThuatBenhVien?.Id
            }));
            goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietDichVuGiuongs.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            {
                Id = count++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                Ma = p.DichVuGiuongBenhVien.Ma,
                TenDichVu = p.DichVuGiuongBenhVien.Ten,
                LoaiGia = p.NhomGiaDichVuGiuongBenhVien?.Ten,
                DonGia = p.NhomGiaDichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens?
                    .Where(x => x.DichVuGiuongBenhVienId == p.DichVuGiuongBenhVienId && x.NhomGiaDichVuGiuongBenhVienId == p.NhomGiaDichVuGiuongBenhVienId &&
                                x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                || x.DenNgay == null)).LastOrDefault() != null ?
                    p.NhomGiaDichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens?
                        .Where(x => x.DichVuGiuongBenhVienId == p.DichVuGiuongBenhVienId && x.NhomGiaDichVuGiuongBenhVienId == p.NhomGiaDichVuGiuongBenhVienId &&
                                    x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                 || x.DenNgay == null)).LastOrDefault()?.Gia : 0,
                SoLuong = 1,

                ThanhTien = p.NhomGiaDichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens?
                                .Where(x => x.DichVuGiuongBenhVienId == p.DichVuGiuongBenhVienId && x.NhomGiaDichVuGiuongBenhVienId == p.NhomGiaDichVuGiuongBenhVienId &&
                                            x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                         || x.DenNgay == null)).LastOrDefault() != null ?
                    p.NhomGiaDichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens?
                        .Where(x => x.DichVuGiuongBenhVienId == p.DichVuGiuongBenhVienId && x.NhomGiaDichVuGiuongBenhVienId == p.NhomGiaDichVuGiuongBenhVienId &&
                                    x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                 || x.DenNgay == null)).LastOrDefault()?.Gia : 0,
                IdDichVuKhac = p.DichVuGiuongBenhVien?.Id
            }));

            //goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietVatTus.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            //{
            //    Id = count++,
            //    IdDatabase = p.Id,
            //    Nhom = Enums.EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
            //    Ma = p.VatTuBenhVien.Ma,
            //    TenDichVu = p.VatTuBenhVien.VatTus?.Ten,
            //    SoLuong = 1,
            //    DonGia = 0,
            //    ThanhTien = 0,
            //    IdDichVuKhac = p.VatTuBenhVien?.Id
            //}));

            //goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietDuocPhams.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            //{
            //    Id = count++,
            //    IdDatabase = p.Id,
            //    Nhom = Enums.EnumNhomGoiDichVu.DuocPham.GetDescription(),
            //    TenDichVu = p.DuocPhamBenhVien.DuocPham?.Ten,
            //    SoLuong = Convert.ToInt32(p.SoLuong),
            //    //TODO update entity kho on 9/9/2020
            //    //DonGia = p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets?
            //    //    .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId).LastOrDefault() != null ? p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets?
            //    //    .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId).LastOrDefault()
            //    //    ?.DonGiaBan : 0,
            //    //ThanhTien = Convert.ToInt32(p.SoLuong) * (p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets?
            //    //                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId).LastOrDefault() != null ? p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets?
            //    //    .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId).LastOrDefault()
            //    //    ?.DonGiaBan : 0),
            //    HoatChat = p.DuocPhamBenhVien.DuocPham?.HoatChat,
            //    NhaSX = p.DuocPhamBenhVien.DuocPham?.NhaSanXuat,
            //    IdDichVuKhac = p.DuocPhamBenhVien?.Id
            //}));
            var queryIqueryable = goiDichVuKhamBenh.AsQueryable();
            var queryTask = queryIqueryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = 0 };
        }

        public GridDataSource GetTotalPageForGridChiTietAsync(QueryInfo queryInfo)
        {
            var count = 1;
            var goiDichVuKhamBenh = new List<GoiDichVuKhongCoChietKhauGridVo>();
            GoiDichVu goiDichVu;

            if (queryInfo.ModifiedBy == 69)
            {
                goiDichVu = _goiDichVuRepository.TableNoTracking
                                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenh)
                                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

                                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuong)
                                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)

                                    //.Include(p => p.GoiDichVuChiTietVatTus).ThenInclude(p => p.VatTuBenhVien).ThenInclude(p => p.VatTus)

                                    //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham)
                                    //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.NhapKhoDuocPhamChiTiets)
                                    .Where(p => p.Id == Convert.ToInt64(queryInfo.AdditionalSearchString)).FirstOrDefault();
            }
            else
            {
                goiDichVu = _goiDichVuRepository.TableNoTracking
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenh)
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuong)
                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)

                    //.Include(p => p.GoiDichVuChiTietVatTus).ThenInclude(p => p.VatTuBenhVien).ThenInclude(p => p.VatTus)

                    //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham)
                    //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.NhapKhoDuocPhamChiTiets)
                    .Where(p => p.Id == Convert.ToInt64(queryInfo.SearchTerms)).FirstOrDefault();
            }
            goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietDichVuKhamBenhs.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            {
                Id = count++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                Ma = p.DichVuKhamBenhBenhVien?.Ma,
                TenDichVu = p.DichVuKhamBenhBenhVien?.Ten,
                LoaiGia = p.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                SoLuong = 1,
                DonGia = p.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens
                                  .Where(x => x.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVien?.Id && x.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVien?.Id && x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                           || x.DenNgay == null))
                                  .LastOrDefault() != null ?
                    p.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens
                        .Where(x => x.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVien?.Id && x.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVien?.Id && x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                                                                                                                                                           || x.DenNgay == null))
                        .LastOrDefault()
                ?.Gia : 0,

                ThanhTien = p.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens
                                .Where(x => x.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVien?.Id && x.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVien?.Id && x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                                                                                                                                                                   || x.DenNgay == null))
                                .LastOrDefault() != null ?
                    p.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens
                        .Where(x => x.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVien?.Id && x.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVien?.Id && x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                                                                                                                                                           || x.DenNgay == null))
                        .LastOrDefault()
                        ?.Gia : 0,
                IdDichVuKhac = p.DichVuKhamBenhBenhVien?.Id
            }));

            goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietDichVuKyThuats.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            {
                Id = count++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                Ma = p.DichVuKyThuatBenhVien.Ma,
                TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                LoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                SoLuong = p.SoLan,
                DonGia = p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens?
                                 .Where(x => x.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId && x.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId
                                                                                                    && x.TuNgay <= DateTime.Now && (x.DenNgay > DateTime.Now
                                          || x.DenNgay == null) && x.TuNgay < DateTime.Now)
                                 .LastOrDefault() != null ? p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens?
                    .Where(x => x.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId && x.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId
                                                                                       && x.TuNgay <= DateTime.Now && (x.DenNgay > DateTime.Now
                                         || x.DenNgay == null) && x.TuNgay < DateTime.Now)
                    .LastOrDefault()
                    ?.Gia : 0,

                ThanhTien = p.SoLan * (p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens?
                                           .Where(x => x.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId && x.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId
                                                                                                              && x.TuNgay <= DateTime.Now && (x.DenNgay > DateTime.Now
                                                                                                                                              || x.DenNgay == null) && x.TuNgay < DateTime.Now)
                                           .LastOrDefault() != null ? p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens?
                                .Where(x => x.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId && x.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId
                                                                                                   && x.TuNgay <= DateTime.Now && (x.DenNgay > DateTime.Now
                                                                                                                                   || x.DenNgay == null) && x.TuNgay < DateTime.Now)
                                .LastOrDefault()
                                ?.Gia : 0),
                IdDichVuKhac = p.DichVuKyThuatBenhVien?.Id
            }));
            goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietDichVuGiuongs.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            {
                Id = count++,
                IdDatabase = p.Id,
                Nhom = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                Ma = p.DichVuGiuongBenhVien.Ma,
                TenDichVu = p.DichVuGiuongBenhVien.Ten,
                LoaiGia = p.NhomGiaDichVuGiuongBenhVien?.Ten,
                DonGia = p.NhomGiaDichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens?
                    .Where(x => x.DichVuGiuongBenhVienId == p.DichVuGiuongBenhVienId && x.NhomGiaDichVuGiuongBenhVienId == p.NhomGiaDichVuGiuongBenhVienId &&
                                x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                || x.DenNgay == null)).LastOrDefault() != null ?
                    p.NhomGiaDichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens?
                        .Where(x => x.DichVuGiuongBenhVienId == p.DichVuGiuongBenhVienId && x.NhomGiaDichVuGiuongBenhVienId == p.NhomGiaDichVuGiuongBenhVienId &&
                                    x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                 || x.DenNgay == null)).LastOrDefault()?.Gia : 0,
                SoLuong = 1,

                ThanhTien = p.NhomGiaDichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens?
                                .Where(x => x.DichVuGiuongBenhVienId == p.DichVuGiuongBenhVienId && x.NhomGiaDichVuGiuongBenhVienId == p.NhomGiaDichVuGiuongBenhVienId &&
                                            x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                         || x.DenNgay == null)).LastOrDefault() != null ?
                    p.NhomGiaDichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens?
                        .Where(x => x.DichVuGiuongBenhVienId == p.DichVuGiuongBenhVienId && x.NhomGiaDichVuGiuongBenhVienId == p.NhomGiaDichVuGiuongBenhVienId &&
                                    x.TuNgay <= DateTime.Now && (x.DenNgay >= DateTime.Now
                                                                 || x.DenNgay == null)).LastOrDefault()?.Gia : 0,
                IdDichVuKhac = p.DichVuGiuongBenhVien?.Id
            }));

            //goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietVatTus.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            //{
            //    Id = count++,
            //    IdDatabase = p.Id,
            //    Nhom = Enums.EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
            //    Ma = p.VatTuBenhVien.Ma,
            //    TenDichVu = p.VatTuBenhVien.VatTus?.Ten,
            //    SoLuong = 1,
            //    DonGia = 0,
            //    ThanhTien = 0,
            //    IdDichVuKhac = p.VatTuBenhVien?.Id
            //}));

            //goiDichVuKhamBenh.AddRange(goiDichVu?.GoiDichVuChiTietDuocPhams.Select(p => new GoiDichVuKhongCoChietKhauGridVo
            //{
            //    Id = count++,
            //    IdDatabase = p.Id,
            //    Nhom = Enums.EnumNhomGoiDichVu.DuocPham.GetDescription(),
            //    TenDichVu = p.DuocPhamBenhVien.DuocPham?.Ten,
            //    SoLuong = Convert.ToInt32(p.SoLuong),
            //    //TODO update entity kho on 9/9/2020
            //    //DonGia = p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets?
            //    //    .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId).LastOrDefault() != null ? p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets?
            //    //    .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId).LastOrDefault()
            //    //    ?.DonGiaBan : 0,
            //    //ThanhTien = Convert.ToInt32(p.SoLuong) * (p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets?
            //    //                .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId).LastOrDefault() != null ? p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets?
            //    //    .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId).LastOrDefault()
            //    //    ?.DonGiaBan : 0),
            //    HoatChat = p.DuocPhamBenhVien.DuocPham?.HoatChat,
            //    NhaSX = p.DuocPhamBenhVien.DuocPham?.NhaSanXuat,
            //    IdDichVuKhac = p.DuocPhamBenhVien?.Id
            //}));
            var queryIqueryable = goiDichVuKhamBenh.AsQueryable();
            var countTask = queryIqueryable.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<List<LookupItemVo>> LoaiGiaNhomGiaDichVuKyThuatBenhVien(long? dichVuKyThuatId)
        {
            //var result = _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking
            //    .Select(item => new LookupItemVo
            //    {
            //        DisplayName = item.Ten,
            //        KeyId = item.Id,
            //    }).Distinct().ToListAsync();
            //await Task.WhenAll(result);

            //return result.Result;
            if (dichVuKyThuatId != null)
            {
                //BVHD-3894
                var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
                long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

                var result = _dichVuKyThuatBenhVienGiaBenhVienRepository
                    .TableNoTracking.Where(s => s.DichVuKyThuatBenhVienId == dichVuKyThuatId)
                    .Where(p => p.TuNgay.Date <= DateTime.Now.Date && (p.DenNgay == null || DateTime.Now.Date <= p.DenNgay.Value.Date))
                    .OrderByDescending(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId).ThenBy(x => x.Id)
               .Select(item => new LookupItemVo
               {
                   DisplayName = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                   KeyId = item.NhomGiaDichVuKyThuatBenhVien.Id,
               }).Distinct().ToListAsync();
                await Task.WhenAll(result);

                return result.Result;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<LookupItemVo>> LoaiGiaNhomGiaGiuongBenhVien(long? dichVuGiuongBenhVienId)
        {
            if (dichVuGiuongBenhVienId != null)
            {
                //BVHD-3894
                var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuGiuong.NhomGiaThuong");
                long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

                var result = _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking
                    .Where(s => s.DichVuGiuongBenhVienId == dichVuGiuongBenhVienId 
                                && s.TuNgay.Date <= DateTime.Now.Date 
                                && (s.DenNgay == null || DateTime.Now.Date <= s.DenNgay.Value.Date))
                    .OrderByDescending(x => x.NhomGiaDichVuGiuongBenhVienId == nhomGiaThuongId).ThenBy(x => x.Id)
               .Select(item => new LookupItemVo
               {
                   DisplayName = item.NhomGiaDichVuGiuongBenhVien.Ten,
                   KeyId = item.NhomGiaDichVuGiuongBenhVien.Id,
               }).Distinct().ToListAsync();
                await Task.WhenAll(result);

                return result.Result;
            }
            else
            {
                return null;
            }
            //var result = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking.Where(s=>s.DichVuGiuongBenhVienGiaBenhViens.Any(p=>p.Gia != 0))
            //    .Select(item => new LookupItemVo
            //    {
            //        DisplayName = item.Ten,
            //        KeyId = item.Id,
            //    }).Distinct().ToListAsync();
            //await Task.WhenAll(result);

            //return result.Result;
        }
        public async Task<List<LookupItemVo>> GetLoaiGiaDichVuKhamBenh(long? idDichVuKhamBenhId)
        {
            if (idDichVuKhamBenhId != null)
            {
                //BVHD-3894
                var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKhamBenh.NhomGiaThuong");
                long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

                var result = _dichVuKhamBenhBenhVienGiaBenhVienRepository
                    .TableNoTracking.Where(p => p.TuNgay.Date <= DateTime.Now.Date && (p.DenNgay == null || DateTime.Now.Date <= p.DenNgay.Value.Date))
                    .Where(s => s.DichVuKhamBenhBenhVienId == idDichVuKhamBenhId)
                    .OrderByDescending(x => x.NhomGiaDichVuKhamBenhBenhVienId == nhomGiaThuongId).ThenBy(x => x.Id)
               .Select(item => new LookupItemVo
               {
                   DisplayName = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                   KeyId = item.NhomGiaDichVuKhamBenhBenhVien.Id,
               }).Distinct().ToListAsync();
                await Task.WhenAll(result);

                return result.Result;
            }
            else
            {
                return null;
            }
            //var result = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
            //    .Select(item => new LookupItemVo
            //    {
            //        DisplayName = item.Ten,
            //        KeyId = item.Id,
            //    }).Distinct().ToListAsync();
            //await Task.WhenAll(result);
            //return result.Result;
        }


        public async Task<List<LookupItemVo>> LoaiGiaNhomGiaDichVuKyThuatBenhVienGrid(long? dichVuKyThuatId)
        {
            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var result = await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => (o.DichVuKyThuatBenhVienId == (long)dichVuKyThuatId
                                    && o.TuNgay.Date <= DateTime.Now.Date
                                    && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                                )
                .OrderByDescending(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId)
                .ThenBy(x => x.CreatedOn)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                    KeyId = item.NhomGiaDichVuKyThuatBenhVienId,
                }).Distinct().ToListAsync();
            return result;

        }
        public async Task<List<LookupItemVo>> LoaiGiaNhomGiaGiuongBenhVienGrid(long? dichVuGiuongBenhVienId)
        {
            if (dichVuGiuongBenhVienId != null)
            {
                var result = _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking.Where(s => s.DichVuGiuongBenhVienId == dichVuGiuongBenhVienId)
               .Select(item => new LookupItemVo
               {
                   DisplayName = item.NhomGiaDichVuGiuongBenhVien.Ten,
                   KeyId = item.NhomGiaDichVuGiuongBenhVien.Id,
               }).Distinct().ToListAsync();
                await Task.WhenAll(result);

                return result.Result;
            }
            else
            {
                return null;
            }
            //var result = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking.Where(s=>s.DichVuGiuongBenhVienGiaBenhViens.Any(p=>p.Gia != 0))
            //    .Select(item => new LookupItemVo
            //    {
            //        DisplayName = item.Ten,
            //        KeyId = item.Id,
            //    }).Distinct().ToListAsync();
            //await Task.WhenAll(result);

            //return result.Result;
        }
        public async Task<List<LookupItemVo>> GetLoaiGiaDichVuKhamBenhGrid(long? idDichVuKhamBenhId)
        {

            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKhamBenh.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var result = await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => (o.DichVuKhamBenhBenhVienId == (long)idDichVuKhamBenhId
                                    && o.TuNgay.Date <= DateTime.Now.Date
                                    && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                                )
                .OrderByDescending(x => x.NhomGiaDichVuKhamBenhBenhVienId == nhomGiaThuongId)
                .ThenBy(x => x.CreatedOn)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    KeyId = item.NhomGiaDichVuKhamBenhBenhVienId,
                }).Distinct().ToListAsync();
            return result;
        }
    }
}

