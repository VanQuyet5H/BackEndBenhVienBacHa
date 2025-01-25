using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Linq;
using Camino.Core.Data;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.CauHinhHeSoTheoNoiGioiThieuHoaHong;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.DichVuBenhVienTongHops;
using Camino.Core.Domain;
using Camino.Services.CauHinh;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using static Camino.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore.Internal;
using System.IO;
using OfficeOpenXml;

namespace Camino.Services.CauHinhHeSoTheoNoiGioiThieuHoaHong
{
    [ScopedDependency(ServiceType = typeof(ICauHinhHeSoTheoNoiGioiThieuHoaHongService))]
    public class CauHinhHeSoTheoNoiGioiThieuHoaHongService : MasterFileService<Camino.Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu>, ICauHinhHeSoTheoNoiGioiThieuHoaHongService
    {
        IRepository<Camino.Core.Domain.Entities.NoiGioiThieu.NoiGioiThieuHopDong> _noiGioiThieuHopDongRepository;

        private IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> _dichVuGiuongBenhVienRepository;
        private IRepository<DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;
        private IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private IRepository<DichVuBenhVienTongHop> _dichVuBenhVienTongHopRepository;


        private readonly IRepository<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;

        private readonly IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVienGiaBenhVien> _dichVuGiuongBenhVienGiaBenhVienRepository;

        private readonly IRepository<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository;

        private readonly IRepository<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository;

        private readonly IRepository<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong> _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository;

        private readonly IRepository<NoiGioiThieuHopDongChiTietHeSoDuocPham> _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository;
        private readonly IRepository<NoiGioiThieuHopDongChiTietHeSoVatTu> _noiGioiThieuHopDongChiTietHeSoVatTuRepository;

        private readonly IRepository<NhomGiaDichVuKhamBenhBenhVien> _nhomGiaDichVuKhamBenhBenhVienRepository;

        private readonly IRepository<NhomGiaDichVuKyThuatBenhVien> _nhomGiaDichVuKyThuatBenhVienRepository;

        private readonly IRepository<NhomGiaDichVuGiuongBenhVien> _nhomGiaDichVuGiuongBenhVienRepository;


        private readonly IRepository<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository;

        private readonly IRepository<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository;

        private readonly IRepository<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong> _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository;

        private readonly IRepository<NoiGioiThieuHopDongChiTietHoaHongDuocPham> _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository;

        private readonly IRepository<NoiGioiThieuHopDongChiTietHoaHongVatTu> _noiGioiThieuHopDongChiTietHoaHongVatTuRepository;

        public CauHinhHeSoTheoNoiGioiThieuHoaHongService(IRepository<Camino.Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu> repository,
            IRepository<Camino.Core.Domain.Entities.NoiGioiThieu.NoiGioiThieuHopDong> noiGioiThieuHopDongRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> dichVuGiuongBenhVienRepository,
            IRepository<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<DichVuBenhVienTongHop> dichVuBenhVienTongHopRepository,
            ICauHinhService cauHinhService,
            IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository,
            IRepository<DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVienGiaBenhVien> dichVuGiuongBenhVienGiaBenhVienRepository,
            IRepository<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository,
            IRepository<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository,
            IRepository<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong> noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository,
            IRepository<NoiGioiThieuHopDongChiTietHeSoDuocPham> noiGioiThieuHopDongChiTietHeSoDuocPhamRepository,
            IRepository<NoiGioiThieuHopDongChiTietHeSoVatTu> noiGioiThieuHopDongChiTietHeSoVatTuRepository,
            IRepository<NhomGiaDichVuKhamBenhBenhVien> nhomGiaDichVuKhamBenhBenhVienRepository,
            IRepository<NhomGiaDichVuKyThuatBenhVien> nhomGiaDichVuKyThuatBenhVienRepository,
            IRepository<NhomGiaDichVuGiuongBenhVien> nhomGiaDichVuGiuongBenhVienRepository,
            IRepository<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository,
            IRepository<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository,
            IRepository<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong> noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository,
            IRepository<NoiGioiThieuHopDongChiTietHoaHongDuocPham> noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository,
             IRepository<NoiGioiThieuHopDongChiTietHoaHongVatTu> noiGioiThieuHopDongChiTietHoaHongVatTuRepository) : base(repository)
        {
            _noiGioiThieuHopDongRepository = noiGioiThieuHopDongRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _dichVuGiuongBenhVienRepository = dichVuGiuongBenhVienRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _dichVuBenhVienTongHopRepository = dichVuBenhVienTongHopRepository;
            _cauHinhService = cauHinhService;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
            _dichVuGiuongBenhVienGiaBenhVienRepository = dichVuGiuongBenhVienGiaBenhVienRepository;
            _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository = noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository;
            _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository = noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository;
            _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository = noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository;
            _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository = noiGioiThieuHopDongChiTietHeSoDuocPhamRepository;
            _noiGioiThieuHopDongChiTietHeSoVatTuRepository = noiGioiThieuHopDongChiTietHeSoVatTuRepository;
            _nhomGiaDichVuKhamBenhBenhVienRepository = nhomGiaDichVuKhamBenhBenhVienRepository;
            _nhomGiaDichVuKyThuatBenhVienRepository = nhomGiaDichVuKyThuatBenhVienRepository;
            _nhomGiaDichVuGiuongBenhVienRepository = nhomGiaDichVuGiuongBenhVienRepository;
            _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository = noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository;
            _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository = noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository;
            _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository = noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository;
            _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository = noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository;
            _noiGioiThieuHopDongChiTietHoaHongVatTuRepository = noiGioiThieuHopDongChiTietHoaHongVatTuRepository;
        }

        #region ds cấu hình hệ số theo nơi giới thiệu/ hoa hồng
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            //
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }


            var query = BaseRepository.TableNoTracking
                .Where(d=>d.IsDisabled == false)
                .Select(s => new CauHinhHeSoTheoNoiGioiThieuHoaHongGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                DonVi = s.DonVi,
                Sdt = s.SoDienThoaiDisplay,
                MoTa = s.MoTa
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.DonVi, g => g.Sdt, g => g.MoTa);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                  .Where(d => d.IsDisabled == false)
                  .Select(s => new CauHinhHeSoTheoNoiGioiThieuHoaHongGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                DonVi = s.DonVi,
                Sdt = s.SoDienThoaiDisplay,
                MoTa = s.MoTa
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.DonVi, g => g.Sdt, g => g.MoTa);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) && queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?.Replace("Format", "") ?? "";
            }
        }
        public async Task<List<LookupItemCauHinhHeSoTheoNoiGioiThieuHoaHongVo>> GetNoiGioiThieu(DropDownListRequestModel model)
        {

            var list = await BaseRepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = list.Select(item => new LookupItemCauHinhHeSoTheoNoiGioiThieuHoaHongVo()
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                DonVi = item.DonVi,
                MoTa = item.MoTa,
                SoDienThoai = item.SoDienThoaiDisplay
            }).ToList();

            if (model.Id != null && query.Any(d => d.KeyId == model.Id))
            {
                return query;
            }
            else if (model.Id != null && !query.Any(d => d.KeyId == model.Id))
            {
                var items = BaseRepository.TableNoTracking.Where(d => d.Id == model.Id)
                    .Select(item => new LookupItemCauHinhHeSoTheoNoiGioiThieuHoaHongVo()
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DonVi = item.DonVi,
                        MoTa = item.MoTa,
                        SoDienThoai = item.SoDienThoaiDisplay
                    }).ToList();
                var result = items.Concat(query).Take(model.Take).ToList();
                return result;
            }

            return query;
        }
        public async Task<List<LookupItemCauHinhHeSoTheoNoiGtHoaHongVo>> GetNoiGioiThieuHopDongAdd(DropDownListRequestModel model, long? id)
        {
            var lstNoiGioiThieuHopDongs = await _noiGioiThieuHopDongRepository.TableNoTracking
                            .Where(d => d.NoiGioiThieuId == id)
                            .ApplyLike(model.Query, g => g.Ten)
                            .Take(model.Take)
                            .ToListAsync();

            var lst = lstNoiGioiThieuHopDongs.Select(item => new LookupItemCauHinhHeSoTheoNoiGtHoaHongVo()
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                ThoiGian = item.NgayBatDau
            }).OrderByDescending(d => d.ThoiGian).ToList();

            return lst;
        }

        public async Task<List<LookupItemVo>> GetNoiGioiThieuHopDongAddHoHong(DropDownListRequestModel model, long? id)
        {
            var lstNoiGioiThieuHopDongs = await _noiGioiThieuHopDongRepository.TableNoTracking
                            .Where(d => d.NoiGioiThieuId == id &&
                                       !d.NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs.Any() &&
                                       !d.NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuats.Any() &&
                                       !d.NoiGioiThieuHopDongChiTietHoaHongDichVuGiuongs.Any() &&
                                       !d.NoiGioiThieuHopDongChiTietHoaHongDuocPhams.Any() &&
                                       !d.NoiGioiThieuHopDongChiTietHoaHongVatTus.Any())
                            .ApplyLike(model.Query, g => g.Ten)
                            .Take(model.Take)
                            .ToListAsync();

            var lst = lstNoiGioiThieuHopDongs.Select(item => new LookupItemVo()
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return lst;
        }
        public async Task<List<LookupItemVo>> GetNoiGioiThieuHopDong(DropDownListRequestModel model, long? id)
        {
            if (id == null)
            {
                var lst = await _noiGioiThieuHopDongRepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

                var query = lst.Select(item => new LookupItemVo()
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                }).ToList();


                return query;
            }
            else
            {
                var lstNoiGioiThieuHopDongs = await _noiGioiThieuHopDongRepository.TableNoTracking
                           .Where(d => d.NoiGioiThieuId == id)
                           .ApplyLike(model.Query, g => g.Ten)
                           .Take(model.Take)
                           .ToListAsync();

                var lst = lstNoiGioiThieuHopDongs.Select(item => new LookupItemVo()
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                }).ToList();

                return lst;
            }
        }
        #endregion

        #region ds nơi giới thiệu hợp đồng
        public async Task<GridDataSource> GetDataNoiGioiThieuHopDongForGridAsync(NoiGioiThieuHopDongQueryInfo queryInfo)
        {

            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            var query = _noiGioiThieuHopDongRepository.TableNoTracking
                .Where(d => d.NoiGioiThieuId == queryInfo.NoiGioiThieuId)
                .Select(s => new NoigioiThieuHopDongGridVo()
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    NgayBatDau = s.NgayBatDau,
                    NgayKetThuc = s.NgayKetThuc,
                }).ApplyLike(queryInfo.TimKiemSearch, g => g.Ten);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        #endregion

        #region  xử lý thêm
        public async Task<NoiGioiThieuHopDongVo> XuLyThemNoiGioiThieuHopDongAsync(NoiGioiThieuHopDong entity)
        {

            await _noiGioiThieuHopDongRepository.AddAsync(entity);


            return new NoiGioiThieuHopDongVo()
            {
                Id = entity.Id,
                Ten = entity.Ten,
                ThemhayCapNhat = true
            };
        }
        public async Task<NoiGioiThieuHopDongVo> XuLyCapNhatNoiGioiThieuHopDongAsync(NoiGioiThieuHopDong entity)
        {
            await _noiGioiThieuHopDongRepository.UpdateAsync(entity);

            return new NoiGioiThieuHopDongVo()
            {
                Id = entity.Id,
                Ten = entity.Ten,
                ThemhayCapNhat = false
            };
        }

        public async Task XuLyDeleteNoiGioiThieuHopDongAsync(NoiGioiThieuHopDong entity)
        {
            await _noiGioiThieuHopDongRepository.DeleteAsync(entity);
        }
        public async Task<NoiGioiThieuHopDong> GetByIdNoiGioiThieuHopDongAsync(long id)
        {
            var noiDungEntity = _noiGioiThieuHopDongRepository.TableNoTracking
                .Where(d => d.Id == id).First();

            return noiDungEntity;
        }
        #endregion
        #region lookup
        public async Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model)
        {
            return await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(x => x.HieuLuc
                                && x.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                    .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                    .Select(item => new DichVuAndThuocAndVTYTTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DichVu = item.Ten,
                        Ma = item.Ma
                    }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                    .Take(model.Take)
                    .ToListAsync();
        }
        public async Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetDichVuGiuong(DropDownListRequestModel model)
        {
            return await _dichVuGiuongBenhVienRepository.TableNoTracking
                     .Where(x => x.HieuLuc
                                 && x.DichVuGiuongBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                     .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                     .Select(item => new DichVuAndThuocAndVTYTTemplateVo
                     {
                         DisplayName = item.Ten,
                         KeyId = item.Id,
                         DichVu = item.Ten,
                         Ma = item.Ma
                     }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                     .Take(model.Take)
                     .ToListAsync();
        }
        public async Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetDichVuKham(DropDownListRequestModel model)
        {
            return await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                     .Where(x => x.HieuLuc
                                 && x.DichVuKhamBenhBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                     .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                     .Select(item => new DichVuAndThuocAndVTYTTemplateVo
                     {
                         DisplayName = item.Ten,
                         KeyId = item.Id,
                         DichVu = item.Ten,
                         Ma = item.Ma
                     }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                     .Take(model.Take)
                     .ToListAsync();
        }
        public async Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetDuocPham(DropDownListRequestModel model)
        {
             var result  = await _duocPhamBenhVienRepository.TableNoTracking
                   .Where(x => x.HieuLuc)
                   .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.DuocPham.Ten)
                   .Select(item => new DichVuAndThuocAndVTYTTemplateVo
                   {
                       DisplayName = item.DuocPham.Ten,
                       KeyId = item.Id,
                       DichVu = item.DuocPham.Ten,
                       Ma = item.MaDuocPhamBenhVien
                   }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                   .Take(model.Take)
                   .ToListAsync();
            return result;
        }
        public async Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetVatTu(DropDownListRequestModel model)
        {
            return await _vatTuBenhVienRepository.TableNoTracking
                     .Where(x => x.HieuLuc)
                     .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.VatTus.Ten)
                     .Select(item => new DichVuAndThuocAndVTYTTemplateVo
                     {
                         DisplayName = item.VatTus.Ten,
                         KeyId = item.Id,
                         DichVu = item.VatTus.Ten,
                         Ma = item.MaVatTuBenhVien
                     }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                     .Take(model.Take)
                     .ToListAsync();
        }

        public async Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuKham(DropDownListRequestModel model)
        {
            var dichVuKhamId = CommonHelper.GetIdFromRequestDropDownList(model);
            //var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKhamBenh.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var result = await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => o.NhomGiaDichVuKhamBenhBenhVienId == model.Id
                                || (o.DichVuKhamBenhBenhVienId == dichVuKhamId
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
            //var result = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
            //    .Select(item => new LookupItemVo
            //    {
            //        DisplayName = item.Ten,
            //        KeyId = item.Id,
            //    }).Distinct().ToListAsync();
            return result;
        }

        public async Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuKyThuat(DropDownListRequestModel model)
        {
            var dichKyThuatId = CommonHelper.GetIdFromRequestDropDownList(model);
            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var result = await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => o.NhomGiaDichVuKyThuatBenhVienId == model.Id
                                || (o.DichVuKyThuatBenhVienId == dichKyThuatId
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

        public async Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuGiuong(DropDownListRequestModel model)
        {
            var dichVuGiuongId = CommonHelper.GetIdFromRequestDropDownList(model);
            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuGiuong.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var result = await _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => o.NhomGiaDichVuGiuongBenhVienId == model.Id
                            || (o.DichVuGiuongBenhVienId == dichVuGiuongId
                                && o.TuNgay.Date <= DateTime.Now.Date
                                && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                )
                .OrderByDescending(x => x.NhomGiaDichVuGiuongBenhVienId == nhomGiaThuongId)
                .ThenBy(x => x.CreatedOn)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.NhomGiaDichVuGiuongBenhVien.Ten,
                    KeyId = item.NhomGiaDichVuGiuongBenhVienId,
                }).Distinct().ToListAsync();
            return result;
        }

        public async Task GetDonGia(ThongTinGiaNoiGioiThieuVo thongTinDichVu)
        {
            if (thongTinDichVu.NhomGiaId != null)
            {
                if (thongTinDichVu.DichVuKhamBenhBenhVienId != null)
                {
                    var thongTinGia = _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking.Include(d => d.NhomGiaDichVuKhamBenhBenhVien)
                        .FirstOrDefault(x => x.DichVuKhamBenhBenhVienId == thongTinDichVu.DichVuKhamBenhBenhVienId
                                    && x.NhomGiaDichVuKhamBenhBenhVienId == thongTinDichVu.NhomGiaId
                                    && x.TuNgay.Date <= DateTime.Now.Date
                                    && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    if (thongTinGia != null)
                    {
                        thongTinDichVu.DonGia = thongTinGia.Gia;
                        thongTinDichVu.TenNhomGia = thongTinGia.NhomGiaDichVuKhamBenhBenhVien?.Ten;
                    }
                }
                else if (thongTinDichVu.DichVuKyThuatBenhVienId != null)
                {
                    var thongTinGia = _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking.Include(d => d.NhomGiaDichVuKyThuatBenhVien)
                        .FirstOrDefault(x => x.DichVuKyThuatBenhVienId == thongTinDichVu.DichVuKyThuatBenhVienId
                                             && x.NhomGiaDichVuKyThuatBenhVienId == thongTinDichVu.NhomGiaId
                                             && x.TuNgay.Date <= DateTime.Now.Date
                                             && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    if (thongTinGia != null)
                    {
                        thongTinDichVu.DonGia = thongTinGia.Gia;
                        thongTinDichVu.TenNhomGia = thongTinGia.NhomGiaDichVuKyThuatBenhVien?.Ten;
                    }
                }
                else if (thongTinDichVu.DichVuGiuongBenhVienId != null)
                {
                    var thongTinGia = _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking.Include(d => d.NhomGiaDichVuGiuongBenhVien)
                        .FirstOrDefault(x => x.DichVuGiuongBenhVienId == thongTinDichVu.DichVuGiuongBenhVienId
                                             && x.NhomGiaDichVuGiuongBenhVienId == thongTinDichVu.NhomGiaId
                                             && x.TuNgay.Date <= DateTime.Now.Date
                                             && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    if (thongTinGia != null)
                    {
                        thongTinDichVu.DonGia = thongTinGia.Gia;
                        thongTinDichVu.TenNhomGia = thongTinGia.NhomGiaDichVuGiuongBenhVien?.Ten;
                    }
                }
            }
        }
        #endregion

        #region CRUD
        public async Task XuLyThemCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(CauHinhHeSoTheoThoiGianHoaHong vo)
        {

            if (vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs != null && vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Count() > 0)
            {

                #region add dv khám bệnh
                var dvKhamBenhs = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuKham == true).ToList();
                if (dvKhamBenhs.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh>();
                    // xử lý list Dv khám cần lưu
                    foreach (var itemDvKham in dvKhamBenhs)
                    {
                        var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh()
                        {
                            DichVuKhamBenhBenhVienId = itemDvKham.DichVuKhamBenhBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDvKham.DonGia.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan1 = itemDvKham.DonGiaNGTTuLan1.GetValueOrDefault(),
                            HeSoGioiThieuTuLan1 = itemDvKham.HeSoLan1.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan2 = itemDvKham.DonGiaNGTTuLan2,
                            HeSoGioiThieuTuLan2 = itemDvKham.HeSoLan2,
                            DonGiaGioiThieuTuLan3 = itemDvKham.DonGiaNGTTuLan3,
                            HeSoGioiThieuTuLan3 = itemDvKham.HeSoLan3,

                            NhomGiaDichVuKhamBenhBenhVienId = itemDvKham.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh.Add(viewModelDvKhamBenh);
                    }

                    await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                }
                #endregion


                #region add dv kỹ thuật
                var dvKyThuats = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuKyThuat == true).ToList();
                if (dvKyThuats.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHeSoDichVu = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat>();
                    // xử lý list Dv kỹ thuật cần lưu
                    foreach (var item in dvKyThuats)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat()
                        {
                            DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = item.DonGia.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan1 = item.DonGiaNGTTuLan1.GetValueOrDefault(),
                            HeSoGioiThieuTuLan1 = item.HeSoLan1.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan2 = item.DonGiaNGTTuLan2,
                            HeSoGioiThieuTuLan2 = item.HeSoLan2,
                            DonGiaGioiThieuTuLan3 = item.DonGiaNGTTuLan3,
                            HeSoGioiThieuTuLan3 = item.HeSoLan3,

                            NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHeSoDichVu.Add(viewModelDv);
                    }

                    await _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVu);
                }
                #endregion

                #region add dv giường
                var dvGiuongs = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuGiuong == true).ToList();
                if (dvGiuongs.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHeSoDichVu = new List<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong>();
                    // xử lý list Dv giường cần lưu
                    foreach (var itemDv in dvGiuongs)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoDichVuGiuong()
                        {
                            DichVuGiuongBenhVienId = itemDv.DichVuGiuongBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan1 = itemDv.DonGiaNGTTuLan1.GetValueOrDefault(),
                            HeSoGioiThieuTuLan1 = itemDv.HeSoLan1.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan2 = itemDv.DonGiaNGTTuLan2,
                            HeSoGioiThieuTuLan2 = itemDv.HeSoLan2,
                            DonGiaGioiThieuTuLan3 = itemDv.DonGiaNGTTuLan3,
                            HeSoGioiThieuTuLan3 = itemDv.HeSoLan3,

                            NhomGiaDichVuGiuongBenhVienId = itemDv.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHeSoDichVu.Add(viewModelDv);
                    }

                    await _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVu);
                }
                #endregion

                #region add  dp
                var dvDuocPhams = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDuocPham == true).ToList();
                if (dvDuocPhams.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHeSoDuocPham = new List<NoiGioiThieuHopDongChiTietHeSoDuocPham>();
                    // xử lý list DP cần lưu
                    foreach (var itemDv in dvDuocPhams)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoDuocPham()
                        {
                            DuocPhamBenhVienId = itemDv.DuocPhamBenhVienId.GetValueOrDefault(),
                            HeSo = itemDv.HeSo.GetValueOrDefault(),
                            LoaiGia = itemDv.NhomGiaThuocId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHeSoDuocPham.Add(viewModelDv);
                    }

                    await _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDuocPham);
                }
                #endregion

                #region add  vt
                var dvVatTus = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaVatTu == true).ToList();
                if (dvVatTus.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHeSoVatTu = new List<NoiGioiThieuHopDongChiTietHeSoVatTu>();
                    // xử lý list vt cần lưu
                    foreach (var itemDv in dvVatTus)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoVatTu()
                        {
                            VatTuBenhVienId = itemDv.VatTuBenhVienId.GetValueOrDefault(),
                            HeSo = itemDv.HeSo.GetValueOrDefault(),
                            LoaiGia = itemDv.NhomGiaVTYTId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHeSoVatTu.Add(viewModelDv);
                    }

                    await _noiGioiThieuHopDongChiTietHeSoVatTuRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoVatTu);
                }
                #endregion
            }



        }

        private List<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> GetByNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }

        private List<NoiGioiThieuHopDongChiTietHeSoDuocPham> GetByNoiGioiThieuHopDongChiTietHeSoDuocPhamIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }
        private List<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> GetByNoiGioiThieuHopDongChiTietHeSoDichVuKyThuatIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }
        private List<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong> GetByNoiGioiThieuHopDongChiTietHeSoDichVuGiuongIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }
        private List<NoiGioiThieuHopDongChiTietHeSoVatTu> GetByNoiGioiThieuHopDongChiTietHeSoVatTuIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHeSoVatTuRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }
        public async Task XuLyCapNhatCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(CauHinhHeSoTheoThoiGianHoaHong vo)
        {
            #region cập nhật data cũ trước
            if (vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs != null && vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Count() > 0)
            {
                // ds thong tin có id != 0
                if (vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.Id != 0).Count() > 0)
                {
                    #region dv khám
                    var infoDcTaoDichVuKhams = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.Id != 0 && d.LaDichVuKham == true).ToList();

                    if (infoDcTaoDichVuKhams.Any())
                    {


                        var dvKhams = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh>();

                        var listDichVuIds = infoDcTaoDichVuKhams.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhIdAsync(listDichVuIds);
                        // xử lý list Dv 
                        foreach (var itemDvKhamdb in dataItems)
                        {
                            foreach (var itemDvKham in infoDcTaoDichVuKhams)
                            {
                                if (itemDvKham.Id == itemDvKhamdb.Id)
                                {
                                    itemDvKhamdb.DichVuKhamBenhBenhVienId = itemDvKham.DichVuKhamBenhBenhVienId.GetValueOrDefault();
                                    itemDvKhamdb.DonGiaBenhVien = itemDvKham.DonGia.GetValueOrDefault();
                                    itemDvKhamdb.DonGiaGioiThieuTuLan1 = itemDvKham.DonGiaNGTTuLan1.GetValueOrDefault();
                                    itemDvKhamdb.HeSoGioiThieuTuLan1 = itemDvKham.HeSoLan1.GetValueOrDefault();
                                    itemDvKhamdb.DonGiaGioiThieuTuLan2 = itemDvKham.DonGiaNGTTuLan2;
                                    itemDvKhamdb.HeSoGioiThieuTuLan2 = itemDvKham.HeSoLan2;
                                    itemDvKhamdb.DonGiaGioiThieuTuLan3 = itemDvKham.DonGiaNGTTuLan3;
                                    itemDvKhamdb.HeSoGioiThieuTuLan3 = itemDvKham.HeSoLan3;

                                    itemDvKhamdb.NhomGiaDichVuKhamBenhBenhVienId = itemDvKham.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault();

                                    itemDvKhamdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                    dvKhams.Add(itemDvKhamdb);
                                }


                            }

                        }
                        await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.UpdateAsync(dvKhams);
                    }
                    #endregion

                    #region  dv kỹ thuật
                    var dvKyThuats = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuKyThuat == true && d.Id != 0).ToList();
                    if (dvKyThuats.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHeSoDichVu = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat>();
                        // xử lý list Dv kỹ thuật cần cap nhật

                        var listDichVuIds = dvKyThuats.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoDichVuKyThuatIdAsync(listDichVuIds);

                        foreach (var itemdb in dataItems)
                        {
                            foreach (var item in dvKyThuats)
                            {
                                if (item.Id == itemdb.Id)
                                {
                                    itemdb.DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId.GetValueOrDefault();
                                    itemdb.DonGiaBenhVien = item.DonGia.GetValueOrDefault();
                                    itemdb.DonGiaGioiThieuTuLan1 = item.DonGiaNGTTuLan1.GetValueOrDefault();
                                    itemdb.HeSoGioiThieuTuLan1 = item.HeSoLan1.GetValueOrDefault();
                                    itemdb.DonGiaGioiThieuTuLan2 = item.DonGiaNGTTuLan2;
                                    itemdb.HeSoGioiThieuTuLan2 = item.HeSoLan2;
                                    itemdb.DonGiaGioiThieuTuLan3 = item.DonGiaNGTTuLan3;
                                    itemdb.HeSoGioiThieuTuLan3 = item.HeSoLan3;

                                    itemdb.NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault();

                                    itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                    listNoiGioiThieuHopDongChiTietHeSoDichVu.Add(itemdb);
                                }

                            }


                        }

                        await _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.UpdateAsync(listNoiGioiThieuHopDongChiTietHeSoDichVu);
                    }
                    #endregion

                    #region  dv giường
                    var dvGiuongs = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuGiuong == true && d.Id != 0).ToList();
                    if (dvGiuongs.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHeSoDichVu = new List<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong>();
                        // xử lý list Dv giường cần lưu

                        var listDichVuIds = dvGiuongs.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoDichVuGiuongIdAsync(listDichVuIds);
                        foreach (var itemdb in dataItems)
                        {
                            foreach (var itemDv in dvGiuongs)
                            {
                                if (itemdb.Id == itemDv.Id)
                                {
                                    itemdb.DichVuGiuongBenhVienId = itemDv.DichVuGiuongBenhVienId.GetValueOrDefault();
                                    itemdb.DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault();
                                    itemdb.DonGiaGioiThieuTuLan1 = itemDv.DonGiaNGTTuLan1.GetValueOrDefault();
                                    itemdb.HeSoGioiThieuTuLan1 = itemDv.HeSoLan1.GetValueOrDefault();
                                    itemdb.DonGiaGioiThieuTuLan2 = itemDv.DonGiaNGTTuLan2;
                                    itemdb.HeSoGioiThieuTuLan2 = itemDv.HeSoLan2;
                                    itemdb.DonGiaGioiThieuTuLan3 = itemDv.DonGiaNGTTuLan3;
                                    itemdb.HeSoGioiThieuTuLan3 = itemDv.HeSoLan3;

                                    itemdb.NhomGiaDichVuGiuongBenhVienId = itemDv.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault();

                                    itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                    listNoiGioiThieuHopDongChiTietHeSoDichVu.Add(itemdb);
                                }
                            }

                        }

                        await _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.UpdateAsync(listNoiGioiThieuHopDongChiTietHeSoDichVu);
                    }
                    #endregion

                    #region   dp
                    var dvDuocPhams = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDuocPham == true && d.Id != 0).ToList();
                    if (dvDuocPhams.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHeSoDuocPham = new List<NoiGioiThieuHopDongChiTietHeSoDuocPham>();
                        // xử lý list DP cần lưu

                        var listDichVuIds = dvDuocPhams.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoDuocPhamIdAsync(listDichVuIds);
                        foreach (var itemdb in dataItems)
                        {
                            foreach (var itemDv in dvDuocPhams)
                            {
                                if (itemdb.Id == itemDv.Id)
                                {
                                    itemdb.DuocPhamBenhVienId = itemDv.DuocPhamBenhVienId.GetValueOrDefault();
                                    itemdb.HeSo = itemDv.HeSo.GetValueOrDefault();
                                    itemdb.LoaiGia = itemDv.NhomGiaThuocId.GetValueOrDefault();

                                    itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;
                                    listNoiGioiThieuHopDongChiTietHeSoDuocPham.Add(itemdb);
                                }
                            }
                        }

                        await _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.UpdateAsync(listNoiGioiThieuHopDongChiTietHeSoDuocPham);
                    }
                    #endregion

                    #region   vt
                    var dvVatTus = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaVatTu == true && d.Id != 0).ToList();
                    if (dvVatTus.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHeSoVatTu = new List<NoiGioiThieuHopDongChiTietHeSoVatTu>();
                        // xử lý list vt cần lưu

                        var listDichVuIds = dvVatTus.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoVatTuIdAsync(listDichVuIds);

                        foreach (var itemdb in dataItems)
                        {
                            foreach (var itemDv in dvVatTus)
                            {
                                if (itemdb.Id == itemDv.Id)
                                {
                                    itemdb.VatTuBenhVienId = itemDv.VatTuBenhVienId.GetValueOrDefault();
                                    itemdb.HeSo = itemDv.HeSo.GetValueOrDefault();
                                    itemdb.LoaiGia = itemDv.NhomGiaVTYTId.GetValueOrDefault();
                                    itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;
                                    listNoiGioiThieuHopDongChiTietHeSoVatTu.Add(itemdb);
                                }
                            }
                        }

                        await _noiGioiThieuHopDongChiTietHeSoVatTuRepository.UpdateAsync(listNoiGioiThieuHopDongChiTietHeSoVatTu);
                    }
                    #endregion
                }

                if (vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.Id == 0).Count() > 0)
                {
                    #region add dv khám bệnh
                    var dvKhamBenhs = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuKham == true && d.Id == 0).ToList();
                    if (dvKhamBenhs.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh>();
                        // xử lý list Dv khám cần lưu
                        foreach (var itemDvKham in dvKhamBenhs)
                        {
                            var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh()
                            {
                                DichVuKhamBenhBenhVienId = itemDvKham.DichVuKhamBenhBenhVienId.GetValueOrDefault(),
                                DonGiaBenhVien = itemDvKham.DonGia.GetValueOrDefault(),
                                DonGiaGioiThieuTuLan1 = itemDvKham.DonGiaNGTTuLan1.GetValueOrDefault(),
                                HeSoGioiThieuTuLan1 = itemDvKham.HeSoLan1.GetValueOrDefault(),
                                DonGiaGioiThieuTuLan2 = itemDvKham.DonGiaNGTTuLan2,
                                HeSoGioiThieuTuLan2 = itemDvKham.HeSoLan2,
                                DonGiaGioiThieuTuLan3 = itemDvKham.DonGiaNGTTuLan3,
                                HeSoGioiThieuTuLan3 = itemDvKham.HeSoLan3,

                                NhomGiaDichVuKhamBenhBenhVienId = itemDvKham.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId,
                            };
                            listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh.Add(viewModelDvKhamBenh);
                        }

                        await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                    }
                    #endregion

                    #region add dv kỹ thuật
                    var dvKyThuats = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuKyThuat == true && d.Id == 0).ToList();
                    if (dvKyThuats.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHeSoDichVu = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat>();
                        // xử lý list Dv kỹ thuật cần lưu
                        foreach (var item in dvKyThuats)
                        {
                            var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat()
                            {
                                DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId.GetValueOrDefault(),
                                DonGiaBenhVien = item.DonGia.GetValueOrDefault(),
                                DonGiaGioiThieuTuLan1 = item.DonGiaNGTTuLan1.GetValueOrDefault(),
                                HeSoGioiThieuTuLan1 = item.HeSoLan1.GetValueOrDefault(),
                                DonGiaGioiThieuTuLan2 = item.DonGiaNGTTuLan2,
                                HeSoGioiThieuTuLan2 = item.HeSoLan2,
                                DonGiaGioiThieuTuLan3 = item.DonGiaNGTTuLan3,
                                HeSoGioiThieuTuLan3 = item.HeSoLan3,

                                NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                            };
                            listNoiGioiThieuHopDongChiTietHeSoDichVu.Add(viewModelDv);
                        }

                        await _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVu);
                    }
                    #endregion

                    #region add dv giường
                    var dvGiuongs = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuGiuong == true && d.Id == 0).ToList();
                    if (dvGiuongs.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHeSoDichVu = new List<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong>();
                        // xử lý list Dv giường cần lưu
                        foreach (var itemDv in dvGiuongs)
                        {
                            var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoDichVuGiuong()
                            {
                                DichVuGiuongBenhVienId = itemDv.DichVuGiuongBenhVienId.GetValueOrDefault(),
                                DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault(),
                                DonGiaGioiThieuTuLan1 = itemDv.DonGiaNGTTuLan1.GetValueOrDefault(),
                                HeSoGioiThieuTuLan1 = itemDv.HeSoLan1.GetValueOrDefault(),
                                DonGiaGioiThieuTuLan2 = itemDv.DonGiaNGTTuLan2,
                                HeSoGioiThieuTuLan2 = itemDv.HeSoLan2,
                                DonGiaGioiThieuTuLan3 = itemDv.DonGiaNGTTuLan3,
                                HeSoGioiThieuTuLan3 = itemDv.HeSoLan3,

                                NhomGiaDichVuGiuongBenhVienId = itemDv.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                            };
                            listNoiGioiThieuHopDongChiTietHeSoDichVu.Add(viewModelDv);
                        }

                        await _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVu);
                    }
                    #endregion

                    #region add  dp
                    var dvDuocPhams = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDuocPham == true && d.Id == 0).ToList();
                    if (dvDuocPhams.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHeSoDuocPham = new List<NoiGioiThieuHopDongChiTietHeSoDuocPham>();
                        // xử lý list DP cần lưu
                        foreach (var itemDv in dvDuocPhams)
                        {
                            var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoDuocPham()
                            {
                                DuocPhamBenhVienId = itemDv.DuocPhamBenhVienId.GetValueOrDefault(),
                                HeSo = itemDv.HeSo.GetValueOrDefault(),
                                LoaiGia = itemDv.NhomGiaThuocId.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                            };
                            listNoiGioiThieuHopDongChiTietHeSoDuocPham.Add(viewModelDv);
                        }

                        await _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDuocPham);
                    }
                    #endregion

                    #region add  vt
                    var dvVatTus = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaVatTu == true && d.Id == 0).ToList();
                    if (dvVatTus.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHeSoVatTu = new List<NoiGioiThieuHopDongChiTietHeSoVatTu>();
                        // xử lý list vt cần lưu
                        foreach (var itemDv in dvVatTus)
                        {
                            var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoVatTu()
                            {
                                VatTuBenhVienId = itemDv.VatTuBenhVienId.GetValueOrDefault(),
                                HeSo = itemDv.HeSo.GetValueOrDefault(),
                                LoaiGia = itemDv.NhomGiaVTYTId.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                            };
                            listNoiGioiThieuHopDongChiTietHeSoVatTu.Add(viewModelDv);
                        }

                        await _noiGioiThieuHopDongChiTietHeSoVatTuRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoVatTu);
                    }
                    #endregion
                }

            }
            #endregion
        }

        public async Task XuLyDeleteCauHinhHeSoTheoNoiGioiThieuAsync(DeleteNoiGioiThieuHopDongVo vo)
        {
            if (vo.LaDichVuKham == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                    await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.DeleteAsync(entity);
                }
            }

            if (vo.LaDichVuKyThuat == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                    await _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaDichVuGiuong == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                    await _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaDuocPham == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                    await _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaVatTu == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoVatTuRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                    await _noiGioiThieuHopDongChiTietHeSoVatTuRepository.DeleteAsync(entity);
                }
            }
        }
        public async Task XuLyDeleteCauHinhHeSoTheoNoiGioiThieuHoaHongCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(DeleteNoiGioiThieuHopDongVo vo)
        {
            if (vo.LaDichVuKham == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.DeleteAsync(entity);
                }
            }

            if (vo.LaDichVuKyThuat == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaDichVuGiuong == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaDuocPham == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaVatTu == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHeSoVatTuRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHeSoVatTuRepository.DeleteAsync(entity);
                }
            }
        }
        public async Task<CauHinhHeSoTheoThoiGianHoaHong> XuLyGetDataCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(long id, long noiGioiThieuId) // id noi gioi thieu hop dong : 
        {
            var tenNoiGioiThieuHopDong = _noiGioiThieuHopDongRepository.TableNoTracking.Where(d => d.Id == id).Select(d => d.Ten).FirstOrDefault();
            var noiGioiThieuHopDong = BaseRepository.TableNoTracking
                .Where(d => d.Id == noiGioiThieuId)
                .Select(d => new CauHinhHeSoTheoThoiGianHoaHong
                {
                    Id = id,
                    ChonLoaiDichVuId = 1, // default
                    Ten = d.Ten,
                    NoiGioiThieuId = d.Id,
                    Donvi = d.DonVi,
                    MoTa = d.MoTa,
                    SoDienThoai = d.SoDienThoaiDisplay,
                    NoiGioiThieuHopDongId = id
                }).ToList();


            if (noiGioiThieuHopDong.Count() > 0)
            {


                var query = _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.TableNoTracking.Include(d => d.DichVuKhamBenhBenhVien).Where(d => d.NoiGioiThieuHopDongId == id)
                                                           .Select(d => new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo
                                                           {
                                                               Id = d.Id,
                                                               DichVuKhamBenhBenhVienId = d.DichVuKhamBenhBenhVienId,
                                                               DonGia = d.DonGiaBenhVien,
                                                               DonGiaNGTTuLan1 = d.DonGiaGioiThieuTuLan1,
                                                               HeSoLan1 = d.HeSoGioiThieuTuLan1,
                                                               DonGiaNGTTuLan2 = d.DonGiaGioiThieuTuLan2,
                                                               HeSoLan2 = d.HeSoGioiThieuTuLan2,
                                                               DonGiaNGTTuLan3 = d.DonGiaGioiThieuTuLan3,
                                                               HeSoLan3 = d.HeSoGioiThieuTuLan3,

                                                               NhomGiaDichVuKhamBenhBenhVienId = d.NhomGiaDichVuKhamBenhBenhVienId,
                                                               LaDichVuKham = true,
                                                               Nhom = "Dịch vụ khám".ToUpper(),
                                                               MaDichVu = d.DichVuKhamBenhBenhVien.Ma,
                                                               TenDichVu = d.DichVuKhamBenhBenhVien.Ten
                                                           }).Union(_noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.TableNoTracking.Include(d => d.DichVuKyThuatBenhVien).Where(d => d.NoiGioiThieuHopDongId == id)
                                                            .Select(d => new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo
                                                            {
                                                                Id = d.Id,
                                                                DichVuKyThuatBenhVienId = d.DichVuKyThuatBenhVienId,
                                                                DonGia = d.DonGiaBenhVien,
                                                                DonGiaNGTTuLan1 = d.DonGiaGioiThieuTuLan1,
                                                                HeSoLan1 = d.HeSoGioiThieuTuLan1,
                                                                DonGiaNGTTuLan2 = d.DonGiaGioiThieuTuLan2,
                                                                HeSoLan2 = d.HeSoGioiThieuTuLan2,
                                                                DonGiaNGTTuLan3 = d.DonGiaGioiThieuTuLan3,
                                                                HeSoLan3 = d.HeSoGioiThieuTuLan3,
                                                                LaDichVuKyThuat = true,

                                                                NhomGiaDichVuKyThuatBenhVienId = d.NhomGiaDichVuKyThuatBenhVienId,
                                                                Nhom = "Dịch vụ kỹ thuật".ToUpper(),
                                                                MaDichVu = d.DichVuKyThuatBenhVien.Ma,
                                                                TenDichVu = d.DichVuKyThuatBenhVien.Ten
                                                            })).Union(_noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == id)
                                                            .Select(d => new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo
                                                            {
                                                                Id = d.Id,
                                                                DichVuGiuongBenhVienId = d.DichVuGiuongBenhVienId,
                                                                DonGia = d.DonGiaBenhVien,
                                                                DonGiaNGTTuLan1 = d.DonGiaGioiThieuTuLan1,
                                                                HeSoLan1 = d.HeSoGioiThieuTuLan1,
                                                                DonGiaNGTTuLan2 = d.DonGiaGioiThieuTuLan2,
                                                                HeSoLan2 = d.HeSoGioiThieuTuLan2,
                                                                DonGiaNGTTuLan3 = d.DonGiaGioiThieuTuLan3,
                                                                HeSoLan3 = d.HeSoGioiThieuTuLan3,

                                                                NhomGiaDichVuGiuongBenhVienId = d.NhomGiaDichVuGiuongBenhVienId,
                                                                LaDichVuGiuong = true,
                                                                Nhom = "Dịch vụ giường".ToUpper(),
                                                                MaDichVu = d.DichVuGiuongBenhVien.Ma,
                                                                TenDichVu = d.DichVuGiuongBenhVien.Ten
                                                            })).Union(_noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == id)
                                                            .Select(d => new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo
                                                            {
                                                                Id = d.Id,
                                                                DuocPhamBenhVienId = d.DuocPhamBenhVienId,
                                                                NhomGiaThuocId = d.LoaiGia,
                                                                HeSo = d.HeSo,
                                                                LaDuocPham = true,
                                                                Nhom = "Dược phẩm".ToUpper(),
                                                            })).Union(_noiGioiThieuHopDongChiTietHeSoVatTuRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == id)
                                                             .Select(d => new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo
                                                             {
                                                                 Id = d.Id,
                                                                 VatTuBenhVienId = d.VatTuBenhVienId,
                                                                 NhomGiaVTYTId = d.LoaiGia,
                                                                 HeSo = d.HeSo,
                                                                 LaVatTu = true,
                                                                 Nhom = "VTYT".ToUpper(),
                                                             })).ToList();

                if (query.Where(d => d.LaDichVuKham == true).Count() > 0)
                {
                    var dichVuKhamIds = query.Select(d => d.DichVuKhamBenhBenhVienId).ToList();

                    var dichVuKhaVos = _dichVuKhamBenhBenhVienRepository.TableNoTracking.Where(d => dichVuKhamIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                            Ma = d.Ma
                        }).ToList();

                    var nhomGiaDichVuGiuongBenhVienIds = query.Select(d => d.NhomGiaDichVuKhamBenhBenhVienId).ToList();

                    var nhomGiaDichVuGiuongBenhVienVos = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking.Where(d => nhomGiaDichVuGiuongBenhVienIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaDichVuKham == true))
                    {
                        item.MaDichVu = dichVuKhaVos.Where(d => d.Id == item.DichVuKhamBenhBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = dichVuKhaVos.Where(d => d.Id == item.DichVuKhamBenhBenhVienId).Select(d => d.Ten).FirstOrDefault();

                        item.TenNhomGia = nhomGiaDichVuGiuongBenhVienVos.Where(d => d.Id == item.NhomGiaDichVuKhamBenhBenhVienId).Select(d => d.Ten).FirstOrDefault();
                    }


                }

                if (query.Where(d => d.LaDichVuKyThuat == true).Count() > 0)
                {
                    var dichVuKyThuatIds = query.Select(d => d.DichVuKyThuatBenhVienId).ToList();

                    var dichVuKyThuatVos = _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(d => dichVuKyThuatIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                            Ma = d.Ma
                        }).ToList();

                    var nhomGiaDichVuKyThuatBenhVienIds = query.Select(d => d.NhomGiaDichVuKyThuatBenhVienId).ToList();

                    var nhomGiaDichVuKyThuatBenhVienVos = _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking.Where(d => nhomGiaDichVuKyThuatBenhVienIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaDichVuKyThuat == true))
                    {
                        item.MaDichVu = dichVuKyThuatVos.Where(d => d.Id == item.DichVuKyThuatBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = dichVuKyThuatVos.Where(d => d.Id == item.DichVuKyThuatBenhVienId).Select(d => d.Ten).FirstOrDefault();

                        item.TenNhomGia = nhomGiaDichVuKyThuatBenhVienVos.Where(d => d.Id == item.NhomGiaDichVuKyThuatBenhVienId).Select(d => d.Ten).FirstOrDefault();
                    }


                }

                if (query.Where(d => d.LaDichVuGiuong == true).Count() > 0)
                {
                    var dichVuGiuongBenhVienIds = query.Select(d => d.DichVuGiuongBenhVienId).ToList();

                    var dichVuGiuongBenhVienVos = _dichVuGiuongBenhVienRepository.TableNoTracking.Where(d => dichVuGiuongBenhVienIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                            Ma = d.Ma
                        }).ToList();

                    var nhomGiaDichVuGiuongBenhVienIds = query.Select(d => d.NhomGiaDichVuGiuongBenhVienId).ToList();

                    var nhomGiaDichVuGiuongBenhVienVos = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking.Where(d => nhomGiaDichVuGiuongBenhVienIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaDichVuGiuong == true))
                    {
                        item.MaDichVu = dichVuGiuongBenhVienVos.Where(d => d.Id == item.DichVuGiuongBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = dichVuGiuongBenhVienVos.Where(d => d.Id == item.DichVuGiuongBenhVienId).Select(d => d.Ten).FirstOrDefault();

                        item.TenNhomGia = nhomGiaDichVuGiuongBenhVienVos.Where(d => d.Id == item.NhomGiaDichVuGiuongBenhVienId).Select(d => d.Ten).FirstOrDefault();
                    }


                }



                var datas = Enum.GetValues(typeof(LoaiGiaNoiGioiThieuHopDong)).Cast<Enum>();
                var models = datas.Select(o => new LookupItemVo
                {
                    DisplayName = o.GetDescription(),
                    KeyId = Convert.ToInt32(o)
                });



                if (query.Where(d => d.LaDuocPham == true).Count() > 0)
                {
                    var duocPhamIds = query.Select(d => d.DuocPhamBenhVienId).ToList();

                    var duocPhamVos = _duocPhamBenhVienRepository.TableNoTracking.Include(d => d.DuocPham).Where(d => duocPhamIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.DuocPham.Ten,
                            Ma = d.Ma
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaDuocPham == true))
                    {
                        item.MaDichVu = duocPhamVos.Where(d => d.Id == item.DuocPhamBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = duocPhamVos.Where(d => d.Id == item.DuocPhamBenhVienId).Select(d => d.Ten).FirstOrDefault();

                        item.TenNhomGia = models.Where(d => d.KeyId == (long)item.NhomGiaThuocId.GetValueOrDefault()).Select(d => d.DisplayName).FirstOrDefault();
                    }


                }

                if (query.Where(d => d.LaVatTu == true).Count() > 0)
                {
                    var vatTuIds = query.Select(d => d.VatTuBenhVienId).ToList();

                    var vatTuVos = _vatTuBenhVienRepository.TableNoTracking.Include(d => d.VatTus).Where(d => vatTuIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.VatTus.Ten,
                            Ma = d.Ma
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaVatTu == true))
                    {
                        item.MaDichVu = vatTuVos.Where(d => d.Id == item.VatTuBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = vatTuVos.Where(d => d.Id == item.VatTuBenhVienId).Select(d => d.Ten).FirstOrDefault();

                        item.TenNhomGia = models.Where(d => d.KeyId == (long)item.NhomGiaVTYTId.GetValueOrDefault()).Select(d => d.DisplayName).FirstOrDefault();
                    }


                }

                noiGioiThieuHopDong.First().ThongTinCauHinhHeSoTheoNoiGtHoaHongs = query;
                noiGioiThieuHopDong.First().TenNoiGioiThieuHopDong = tenNoiGioiThieuHopDong;
            }

            return noiGioiThieuHopDong.FirstOrDefault();
        }
        #endregion
        #region CRUD Cấu hình hoa hồng
        private List<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> GetByNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }

        private List<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> GetByNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuatIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }
        private List<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong> GetByNoiGioiThieuHopDongChiTietHoaHongDichVuGiuongIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }
        private List<NoiGioiThieuHopDongChiTietHoaHongDuocPham> GetByNoiGioiThieuHopDongChiTietHoaHongDuocPhamIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }
        private List<NoiGioiThieuHopDongChiTietHoaHongVatTu> GetByNoiGioiThieuHopDongChiTietHoaHongVatTuIdAsync(List<long> ids)
        {
            var query = _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.Table.Where(d => ids.Contains(d.Id)).ToList();
            return query.ToList();
        }
        public async Task XuLyThemCauHinhHoaHongAsync(CauHinhChiTietHoaHong vo)
        {
            if (vo.ThongTinCauHinhHoaHongs != null && vo.ThongTinCauHinhHoaHongs.Count() > 0)
            {

                #region add dv khám bệnh
                var dvKhamBenhs = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuKham == true).ToList();
                if (dvKhamBenhs.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh>();
                    // xử lý list Dv khám cần lưu
                    foreach (var itemDvKham in dvKhamBenhs)
                    {
                        var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh()
                        {
                            DichVuKhamBenhBenhVienId = itemDvKham.DichVuKhamBenhBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDvKham.DonGia.GetValueOrDefault(),
                            LoaiHoaHong = itemDvKham.ChonTienHayHoaHong,
                            SoTienHoaHong = itemDvKham.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemDvKham.DonGiaHoaHongHoacTien : null,
                            TiLeHoaHong = itemDvKham.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemDvKham.DonGiaHoaHongHoacTien : null,
                            ApDungTuLan = itemDvKham.ADDHHTuLan.GetValueOrDefault(),
                            ApDungDenLan = itemDvKham.ADDHHDenLan != null ? itemDvKham.ADDHHDenLan : null,

                            NhomGiaDichVuKhamBenhBenhVienId = itemDvKham.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh.Add(viewModelDvKhamBenh);
                    }

                    await _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh);
                }
                #endregion


                #region add dv kỹ thuật
                var dvKyThuats = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuKyThuat == true).ToList();
                if (dvKyThuats.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat>();
                    // xử lý list Dv kỹ thuật cần lưu
                    foreach (var item in dvKyThuats)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat()
                        {
                            DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = item.DonGia.GetValueOrDefault(),
                            LoaiHoaHong = item.ChonTienHayHoaHong,
                            SoTienHoaHong = item.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? item.DonGiaHoaHongHoacTien : null,
                            TiLeHoaHong = item.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? item.DonGiaHoaHongHoacTien : null,
                            ApDungTuLan = item.ADDHHTuLan.GetValueOrDefault(),
                            ApDungDenLan = item.ADDHHDenLan != null ? item.ADDHHDenLan : null,

                            NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat.Add(viewModelDv);
                    }

                    await _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat);
                }
                #endregion

                #region add dv giường
                var dvGiuongs = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuGiuong == true).ToList();
                if (dvGiuongs.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHoaHongDichVuGiuong = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong>();
                    // xử lý list Dv giường cần lưu
                    foreach (var itemDv in dvGiuongs)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong()
                        {
                            DichVuGiuongBenhVienId = itemDv.DichVuGiuongBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault(),
                            LoaiHoaHong = itemDv.ChonTienHayHoaHong,
                            SoTienHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemDv.DonGiaHoaHongHoacTien : null,
                            TiLeHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemDv.ADDHHDenLan : null,

                            NhomGiaDichVuGiuongBenhVienId = itemDv.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHoaHongDichVuGiuong.Add(viewModelDv);
                    }

                    await _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongDichVuGiuong);
                }
                #endregion

                #region add  dp
                var dvDuocPhams = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDuocPham == true).ToList();
                if (dvDuocPhams.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHoaHongDuocPham = new List<NoiGioiThieuHopDongChiTietHoaHongDuocPham>();
                    // xử lý list DP cần lưu
                    foreach (var itemDv in dvDuocPhams)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHoaHongDuocPham()
                        {
                            DuocPhamBenhVienId = itemDv.DuocPhamBenhVienId.GetValueOrDefault(),
                            TiLeHoaHong = itemDv.DonGiaHoaHongHoacTien.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHoaHongDuocPham.Add(viewModelDv);
                    }

                    await _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongDuocPham);
                }
                #endregion

                #region add  vt
                var dvVatTus = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaVatTu == true).ToList();
                if (dvVatTus.Any())
                {
                    var listNoiGioiThieuHopDongChiTietHoaHongVatTu = new List<NoiGioiThieuHopDongChiTietHoaHongVatTu>();
                    // xử lý list vt cần lưu
                    foreach (var itemDv in dvVatTus)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHoaHongVatTu()
                        {
                            VatTuBenhVienId = itemDv.VatTuBenhVienId.GetValueOrDefault(),
                            TiLeHoaHong = itemDv.DonGiaHoaHongHoacTien.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        listNoiGioiThieuHopDongChiTietHoaHongVatTu.Add(viewModelDv);
                    }

                    await _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongVatTu);
                }
                #endregion
            }
        }

        public async Task<CauHinhChiTietHoaHong> XuLyGetDataCauHinhHoaHongAsync(long id, long noiGioiThieuId) // id noi gioi thieu hop dong : 
        {
            var tenNoiGioiThieuHopDong = _noiGioiThieuHopDongRepository.TableNoTracking.Where(d => d.Id == id).Select(d => d.Ten).FirstOrDefault();
            var noiGioiThieuHopDong = BaseRepository.TableNoTracking
                .Where(d => d.Id == noiGioiThieuId)
                .Select(d => new CauHinhChiTietHoaHong
                {
                    Id = id,
                    ChonLoaiDichVuId = 1, // default
                    Ten = d.Ten,
                    NoiGioiThieuId = d.Id,
                    Donvi = d.DonVi,
                    MoTa = d.MoTa,
                    SoDienThoai = d.SoDienThoaiDisplay,
                    NoiGioiThieuHopDongId = id
                }).ToList();


            if (noiGioiThieuHopDong.Count() > 0)
            {


                var query = _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.TableNoTracking.Include(d => d.DichVuKhamBenhBenhVien).Where(d => d.NoiGioiThieuHopDongId == id)
                                                           .Select(d => new ThongTinCauHinhHoaHongGridVo
                                                           {
                                                               Id = d.Id,
                                                               DichVuKhamBenhBenhVienId = d.DichVuKhamBenhBenhVienId,
                                                               DonGia = d.DonGiaBenhVien,
                                                               ADDHHDenLan = d.ApDungDenLan,
                                                               ADDHHTuLan = d.ApDungTuLan,
                                                               ChonTienHayHoaHong = d.LoaiHoaHong,
                                                               DonGiaHoaHongHoacTien = d.LoaiHoaHong == LoaiHoaHong.SoTien ? d.SoTienHoaHong : d.TiLeHoaHong,


                                                               NhomGiaDichVuKhamBenhBenhVienId = d.NhomGiaDichVuKhamBenhBenhVienId,
                                                               LaDichVuKham = true,
                                                               Nhom = "Dịch vụ khám".ToUpper(),
                                                               MaDichVu = d.DichVuKhamBenhBenhVien.Ma,
                                                               TenDichVu = d.DichVuKhamBenhBenhVien.Ten
                                                           }).Union(_noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.TableNoTracking.Include(d => d.DichVuKyThuatBenhVien).Where(d => d.NoiGioiThieuHopDongId == id)
                                                            .Select(d => new ThongTinCauHinhHoaHongGridVo
                                                            {
                                                                Id = d.Id,
                                                                DichVuKyThuatBenhVienId = d.DichVuKyThuatBenhVienId,
                                                                DonGia = d.DonGiaBenhVien,
                                                                ADDHHDenLan = d.ApDungDenLan,
                                                                ADDHHTuLan = d.ApDungTuLan,
                                                                ChonTienHayHoaHong = d.LoaiHoaHong,
                                                                DonGiaHoaHongHoacTien = d.LoaiHoaHong == LoaiHoaHong.SoTien ? d.SoTienHoaHong : d.TiLeHoaHong,
                                                                LaDichVuKyThuat = true,

                                                                NhomGiaDichVuKyThuatBenhVienId = d.NhomGiaDichVuKyThuatBenhVienId,
                                                                Nhom = "Dịch vụ kỹ thuật".ToUpper(),
                                                                MaDichVu = d.DichVuKyThuatBenhVien.Ma,
                                                                TenDichVu = d.DichVuKyThuatBenhVien.Ten
                                                            })).Union(_noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == id)
                                                            .Select(d => new ThongTinCauHinhHoaHongGridVo
                                                            {
                                                                Id = d.Id,
                                                                DichVuGiuongBenhVienId = d.DichVuGiuongBenhVienId,
                                                                DonGia = d.DonGiaBenhVien,
                                                                ADDHHDenLan = d.ApDungDenLan,
                                                                ADDHHTuLan = d.ApDungTuLan,
                                                                ChonTienHayHoaHong = d.LoaiHoaHong,
                                                                DonGiaHoaHongHoacTien = d.LoaiHoaHong == LoaiHoaHong.SoTien ? d.SoTienHoaHong : d.TiLeHoaHong,

                                                                NhomGiaDichVuGiuongBenhVienId = d.NhomGiaDichVuGiuongBenhVienId,
                                                                LaDichVuGiuong = true,
                                                                Nhom = "Dịch vụ giường".ToUpper(),
                                                                MaDichVu = d.DichVuGiuongBenhVien.Ma,
                                                                TenDichVu = d.DichVuGiuongBenhVien.Ten
                                                            })).Union(_noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == id)
                                                            .Select(d => new ThongTinCauHinhHoaHongGridVo
                                                            {
                                                                Id = d.Id,
                                                                DuocPhamBenhVienId = d.DuocPhamBenhVienId,
                                                                DonGiaHoaHongHoacTien = d.TiLeHoaHong,
                                                                ChonTienHayHoaHong = LoaiHoaHong.TiLe,
                                                                LaDuocPham = true,
                                                                Nhom = "Dược phẩm".ToUpper(),
                                                                MaDichVu = d.DuocPhamBenhVien.Ma
                                                            })).Union(_noiGioiThieuHopDongChiTietHoaHongVatTuRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == id)
                                                             .Select(d => new ThongTinCauHinhHoaHongGridVo
                                                             {
                                                                 Id = d.Id,
                                                                 VatTuBenhVienId = d.VatTuBenhVienId,
                                                                 DonGiaHoaHongHoacTien = d.TiLeHoaHong,
                                                                 ChonTienHayHoaHong = LoaiHoaHong.TiLe,
                                                                 LaVatTu = true,
                                                                 Nhom = "VTYT".ToUpper(),
                                                                 MaDichVu = d.VatTuBenhVien.Ma,
                                                                 TenDichVu = d.VatTuBenhVien.VatTus.Ten
                                                             })).ToList();

                if (query.Where(d => d.LaDichVuKham == true).Count() > 0)
                {
                    var dichVuKhamIds = query.Select(d => d.DichVuKhamBenhBenhVienId).ToList();

                    var dichVuKhaVos = _dichVuKhamBenhBenhVienRepository.TableNoTracking.Where(d => dichVuKhamIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                            Ma = d.Ma
                        }).ToList();

                    var nhomGiaDichVuGiuongBenhVienIds = query.Select(d => d.NhomGiaDichVuKhamBenhBenhVienId).ToList();

                    var nhomGiaDichVuGiuongBenhVienVos = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking.Where(d => nhomGiaDichVuGiuongBenhVienIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaDichVuKham == true))
                    {
                        item.MaDichVu = dichVuKhaVos.Where(d => d.Id == item.DichVuKhamBenhBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = dichVuKhaVos.Where(d => d.Id == item.DichVuKhamBenhBenhVienId).Select(d => d.Ten).FirstOrDefault();

                        item.TenNhomGia = nhomGiaDichVuGiuongBenhVienVos.Where(d => d.Id == item.NhomGiaDichVuKhamBenhBenhVienId).Select(d => d.Ten).FirstOrDefault();
                    }


                }

                if (query.Where(d => d.LaDichVuKyThuat == true).Count() > 0)
                {
                    var dichVuKyThuatIds = query.Select(d => d.DichVuKyThuatBenhVienId).ToList();

                    var dichVuKyThuatVos = _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(d => dichVuKyThuatIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                            Ma = d.Ma
                        }).ToList();

                    var nhomGiaDichVuKyThuatBenhVienIds = query.Select(d => d.NhomGiaDichVuKyThuatBenhVienId).ToList();

                    var nhomGiaDichVuKyThuatBenhVienVos = _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking.Where(d => nhomGiaDichVuKyThuatBenhVienIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaDichVuKyThuat == true))
                    {
                        item.MaDichVu = dichVuKyThuatVos.Where(d => d.Id == item.DichVuKyThuatBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = dichVuKyThuatVos.Where(d => d.Id == item.DichVuKyThuatBenhVienId).Select(d => d.Ten).FirstOrDefault();

                        item.TenNhomGia = nhomGiaDichVuKyThuatBenhVienVos.Where(d => d.Id == item.NhomGiaDichVuKyThuatBenhVienId).Select(d => d.Ten).FirstOrDefault();
                    }


                }

                if (query.Where(d => d.LaDichVuGiuong == true).Count() > 0)
                {
                    var dichVuGiuongBenhVienIds = query.Select(d => d.DichVuGiuongBenhVienId).ToList();

                    var dichVuGiuongBenhVienVos = _dichVuGiuongBenhVienRepository.TableNoTracking.Where(d => dichVuGiuongBenhVienIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                            Ma = d.Ma
                        }).ToList();

                    var nhomGiaDichVuGiuongBenhVienIds = query.Select(d => d.NhomGiaDichVuGiuongBenhVienId).ToList();

                    var nhomGiaDichVuGiuongBenhVienVos = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking.Where(d => nhomGiaDichVuGiuongBenhVienIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaDichVuGiuong == true))
                    {
                        item.MaDichVu = dichVuGiuongBenhVienVos.Where(d => d.Id == item.DichVuGiuongBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = dichVuGiuongBenhVienVos.Where(d => d.Id == item.DichVuGiuongBenhVienId).Select(d => d.Ten).FirstOrDefault();

                        item.TenNhomGia = nhomGiaDichVuGiuongBenhVienVos.Where(d => d.Id == item.NhomGiaDichVuGiuongBenhVienId).Select(d => d.Ten).FirstOrDefault();
                    }


                }




                if (query.Where(d => d.LaDuocPham == true).Count() > 0)
                {
                    var duocPhamIds = query.Select(d => d.DuocPhamBenhVienId).ToList();

                    var duocPhamVos = _duocPhamBenhVienRepository.TableNoTracking.Include(d => d.DuocPham).Where(d => duocPhamIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.DuocPham.Ten,
                            Ma = d.Ma
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaDuocPham == true))
                    {
                        item.MaDichVu = duocPhamVos.Where(d => d.Id == item.DuocPhamBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = duocPhamVos.Where(d => d.Id == item.DuocPhamBenhVienId).Select(d => d.Ten).FirstOrDefault();
                    }


                }

                if (query.Where(d => d.LaVatTu == true).Count() > 0)
                {
                    var vatTuIds = query.Select(d => d.VatTuBenhVienId).ToList();

                    var vatTuVos = _vatTuBenhVienRepository.TableNoTracking.Include(d => d.VatTus).Where(d => vatTuIds.Contains(d.Id))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Ten = d.VatTus.Ten,
                            Ma = d.Ma
                        }).ToList();

                    foreach (var item in query.Where(d => d.LaVatTu == true))
                    {
                        item.MaDichVu = vatTuVos.Where(d => d.Id == item.VatTuBenhVienId).Select(d => d.Ma).FirstOrDefault();
                        item.TenDichVu = vatTuVos.Where(d => d.Id == item.VatTuBenhVienId).Select(d => d.Ten).FirstOrDefault();
                    }


                }


                noiGioiThieuHopDong.First().ThongTinCauHinhHoaHongs = query;
                noiGioiThieuHopDong.First().TenNoiGioiThieuHopDong = tenNoiGioiThieuHopDong;
            }

            return noiGioiThieuHopDong.FirstOrDefault();
        }

        public async Task XuLyCapNhatCauHinhHoaHongAsync(CauHinhChiTietHoaHong vo)
        {
            #region cập nhật data cũ trước
            if (vo.ThongTinCauHinhHoaHongs != null && vo.ThongTinCauHinhHoaHongs.Count() > 0)
            {
                // ds thong tin có id != 0
                if (vo.ThongTinCauHinhHoaHongs.Where(d => d.Id != 0).Count() > 0)
                {
                    #region dv khám
                    var infoDcTaoDichVuKhams = vo.ThongTinCauHinhHoaHongs.Where(d => d.Id != 0 && d.LaDichVuKham == true).ToList();

                    if (infoDcTaoDichVuKhams.Any())
                    {


                        var dvKhams = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh>();

                        var listDichVuIds = infoDcTaoDichVuKhams.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhIdAsync(listDichVuIds);
                        // xử lý list Dv 
                        foreach (var itemDvKhamdb in dataItems)
                        {
                            foreach (var itemDvKham in infoDcTaoDichVuKhams)
                            {
                                if (itemDvKham.Id == itemDvKhamdb.Id)
                                {
                                    itemDvKhamdb.DichVuKhamBenhBenhVienId = itemDvKham.DichVuKhamBenhBenhVienId.GetValueOrDefault();
                                    itemDvKhamdb.DonGiaBenhVien = itemDvKham.DonGia.GetValueOrDefault();
                                    itemDvKhamdb.LoaiHoaHong = itemDvKham.ChonTienHayHoaHong;
                                    itemDvKhamdb.SoTienHoaHong = itemDvKham.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemDvKham.DonGiaHoaHongHoacTien : null;
                                    itemDvKhamdb.TiLeHoaHong = itemDvKham.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemDvKham.DonGiaHoaHongHoacTien : null;
                                    itemDvKhamdb.ApDungTuLan = itemDvKham.ADDHHTuLan.GetValueOrDefault();
                                    itemDvKhamdb.ApDungDenLan = itemDvKham.ADDHHDenLan != null ? itemDvKham.ADDHHDenLan : null;

                                    itemDvKhamdb.NhomGiaDichVuKhamBenhBenhVienId = itemDvKham.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault();

                                    itemDvKhamdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                    dvKhams.Add(itemDvKhamdb);
                                }


                            }

                        }
                        await _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.UpdateAsync(dvKhams);
                    }
                    #endregion

                    #region  dv kỹ thuật
                    var dvKyThuats = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuKyThuat == true && d.Id != 0).ToList();
                    if (dvKyThuats.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat>();
                        // xử lý list Dv kỹ thuật cần cap nhật

                        var listDichVuIds = dvKyThuats.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuatIdAsync(listDichVuIds);

                        foreach (var itemdb in dataItems)
                        {
                            foreach (var item in dvKyThuats)
                            {
                                if (item.Id == itemdb.Id)
                                {
                                    itemdb.DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId.GetValueOrDefault();
                                    itemdb.DonGiaBenhVien = item.DonGia.GetValueOrDefault();
                                    itemdb.LoaiHoaHong = item.ChonTienHayHoaHong;
                                    itemdb.SoTienHoaHong = item.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? item.DonGiaHoaHongHoacTien : null;
                                    itemdb.TiLeHoaHong = item.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? item.DonGiaHoaHongHoacTien : null;
                                    itemdb.ApDungTuLan = item.ADDHHTuLan.GetValueOrDefault();
                                    itemdb.ApDungDenLan = item.ADDHHDenLan != null ? item.ADDHHDenLan : null;

                                    itemdb.NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault();

                                    itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                    listNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat.Add(itemdb);
                                }

                            }


                        }

                        await _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.UpdateAsync(listNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat);
                    }
                    #endregion

                    #region  dv giường
                    var dvGiuongs = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuGiuong == true && d.Id != 0).ToList();
                    if (dvGiuongs.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHoaHongDichVuGiuong = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong>();
                        // xử lý list Dv giường cần lưu

                        var listDichVuIds = dvGiuongs.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongDichVuGiuongIdAsync(listDichVuIds);
                        foreach (var itemdb in dataItems)
                        {
                            foreach (var itemDv in dvGiuongs)
                            {
                                if (itemdb.Id == itemDv.Id)
                                {
                                    itemdb.DichVuGiuongBenhVienId = itemDv.DichVuGiuongBenhVienId.GetValueOrDefault();
                                    itemdb.DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault();
                                    itemdb.LoaiHoaHong = itemDv.ChonTienHayHoaHong;
                                    itemdb.SoTienHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemDv.DonGiaHoaHongHoacTien : null;
                                    itemdb.TiLeHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemDv.DonGiaHoaHongHoacTien : null;
                                    itemdb.ApDungTuLan = itemDv.ADDHHTuLan.GetValueOrDefault();
                                    itemdb.ApDungDenLan = itemDv.ADDHHDenLan != null ? itemDv.ADDHHDenLan : null;

                                    itemdb.NhomGiaDichVuGiuongBenhVienId = itemDv.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault();

                                    itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                    listNoiGioiThieuHopDongChiTietHoaHongDichVuGiuong.Add(itemdb);
                                }
                            }

                        }

                        await _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.UpdateAsync(listNoiGioiThieuHopDongChiTietHoaHongDichVuGiuong);
                    }
                    #endregion

                    #region   dp
                    var dvDuocPhams = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDuocPham == true && d.Id != 0).ToList();
                    if (dvDuocPhams.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHoaHongDuocPham = new List<NoiGioiThieuHopDongChiTietHoaHongDuocPham>();
                        // xử lý list DP cần lưu

                        var listDichVuIds = dvDuocPhams.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongDuocPhamIdAsync(listDichVuIds);
                        foreach (var itemdb in dataItems)
                        {
                            foreach (var itemDv in dvDuocPhams)
                            {
                                if (itemdb.Id == itemDv.Id)
                                {
                                    itemdb.DuocPhamBenhVienId = itemDv.DuocPhamBenhVienId.GetValueOrDefault();
                                    itemdb.TiLeHoaHong = itemDv.DonGiaHoaHongHoacTien.GetValueOrDefault();

                                    itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;
                                    listNoiGioiThieuHopDongChiTietHoaHongDuocPham.Add(itemdb);
                                }
                            }
                        }

                        await _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.UpdateAsync(listNoiGioiThieuHopDongChiTietHoaHongDuocPham);
                    }
                    #endregion

                    #region   vt
                    var dvVatTus = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaVatTu == true && d.Id != 0).ToList();
                    if (dvVatTus.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHoaHongVatTu = new List<NoiGioiThieuHopDongChiTietHoaHongVatTu>();
                        // xử lý list vt cần lưu

                        var listDichVuIds = dvVatTus.Select(d => d.Id).ToList();
                        var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongVatTuIdAsync(listDichVuIds);

                        foreach (var itemdb in dataItems)
                        {
                            foreach (var itemDv in dvVatTus)
                            {
                                if (itemdb.Id == itemDv.Id)
                                {
                                    itemdb.VatTuBenhVienId = itemDv.VatTuBenhVienId.GetValueOrDefault();
                                    itemdb.TiLeHoaHong = itemDv.DonGiaHoaHongHoacTien.GetValueOrDefault();

                                    itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;
                                    listNoiGioiThieuHopDongChiTietHoaHongVatTu.Add(itemdb);
                                }
                            }
                        }

                        await _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.UpdateAsync(listNoiGioiThieuHopDongChiTietHoaHongVatTu);
                    }
                    #endregion
                }



                if (vo.ThongTinCauHinhHoaHongs.Where(d => d.Id == 0).Count() > 0)
                {
                    #region add dv khám bệnh
                    var dvKhamBenhs = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuKham == true).ToList();
                    if (dvKhamBenhs.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh>();
                        // xử lý list Dv khám cần lưu
                        foreach (var itemDvKham in dvKhamBenhs)
                        {
                            var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh()
                            {
                                DichVuKhamBenhBenhVienId = itemDvKham.DichVuKhamBenhBenhVienId.GetValueOrDefault(),
                                DonGiaBenhVien = itemDvKham.DonGia.GetValueOrDefault(),
                                LoaiHoaHong = itemDvKham.ChonTienHayHoaHong,
                                SoTienHoaHong = itemDvKham.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemDvKham.DonGiaHoaHongHoacTien : null,
                                TiLeHoaHong = itemDvKham.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemDvKham.DonGiaHoaHongHoacTien : null,
                                ApDungTuLan = itemDvKham.ADDHHTuLan.GetValueOrDefault(),
                                ApDungDenLan = itemDvKham.ADDHHDenLan != null ? itemDvKham.ADDHHDenLan : null,

                                NhomGiaDichVuKhamBenhBenhVienId = itemDvKham.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                            };
                            listNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh.Add(viewModelDvKhamBenh);
                        }

                        await _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh);
                    }
                    #endregion


                    #region add dv kỹ thuật
                    var dvKyThuats = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuKyThuat == true).ToList();
                    if (dvKyThuats.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat>();
                        // xử lý list Dv kỹ thuật cần lưu
                        foreach (var item in dvKyThuats)
                        {
                            var viewModelDv = new NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat()
                            {
                                DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId.GetValueOrDefault(),
                                DonGiaBenhVien = item.DonGia.GetValueOrDefault(),
                                LoaiHoaHong = item.ChonTienHayHoaHong,
                                SoTienHoaHong = item.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? item.DonGiaHoaHongHoacTien : null,
                                TiLeHoaHong = item.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? item.DonGiaHoaHongHoacTien : null,
                                ApDungTuLan = item.ADDHHTuLan.GetValueOrDefault(),
                                ApDungDenLan = item.ADDHHDenLan != null ? item.ADDHHDenLan : null,

                                NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                            };
                            listNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat.Add(viewModelDv);
                        }

                        await _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat);
                    }
                    #endregion

                    #region add dv giường
                    var dvGiuongs = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuGiuong == true).ToList();
                    if (dvGiuongs.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHoaHongDichVuGiuong = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong>();
                        // xử lý list Dv giường cần lưu
                        foreach (var itemDv in dvGiuongs)
                        {
                            var viewModelDv = new NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong()
                            {
                                DichVuGiuongBenhVienId = itemDv.DichVuGiuongBenhVienId.GetValueOrDefault(),
                                DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault(),
                                LoaiHoaHong = itemDv.ChonTienHayHoaHong,
                                SoTienHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemDv.DonGiaHoaHongHoacTien : null,
                                TiLeHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemDv.ADDHHDenLan : null,

                                NhomGiaDichVuGiuongBenhVienId = itemDv.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                            };
                            listNoiGioiThieuHopDongChiTietHoaHongDichVuGiuong.Add(viewModelDv);
                        }

                        await _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongDichVuGiuong);
                    }
                    #endregion

                    #region add  dp
                    var dvDuocPhams = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDuocPham == true).ToList();
                    if (dvDuocPhams.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHoaHongDuocPham = new List<NoiGioiThieuHopDongChiTietHoaHongDuocPham>();
                        // xử lý list DP cần lưu
                        foreach (var itemDv in dvDuocPhams)
                        {
                            var viewModelDv = new NoiGioiThieuHopDongChiTietHoaHongDuocPham()
                            {
                                DuocPhamBenhVienId = itemDv.DuocPhamBenhVienId.GetValueOrDefault(),
                                TiLeHoaHong = itemDv.DonGiaHoaHongHoacTien.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                            };
                            listNoiGioiThieuHopDongChiTietHoaHongDuocPham.Add(viewModelDv);
                        }

                        await _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongDuocPham);
                    }
                    #endregion

                    #region add  vt
                    var dvVatTus = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaVatTu == true).ToList();
                    if (dvVatTus.Any())
                    {
                        var listNoiGioiThieuHopDongChiTietHoaHongVatTu = new List<NoiGioiThieuHopDongChiTietHoaHongVatTu>();
                        // xử lý list vt cần lưu
                        foreach (var itemDv in dvVatTus)
                        {
                            var viewModelDv = new NoiGioiThieuHopDongChiTietHoaHongVatTu()
                            {
                                VatTuBenhVienId = itemDv.VatTuBenhVienId.GetValueOrDefault(),
                                TiLeHoaHong = itemDv.DonGiaHoaHongHoacTien.GetValueOrDefault(),

                                NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                            };
                            listNoiGioiThieuHopDongChiTietHoaHongVatTu.Add(viewModelDv);
                        }

                        await _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHoaHongVatTu);
                    }
                    #endregion
                }

            }
            #endregion
        }
        #endregion
        public async Task XuLyDeleteCauHinhHoaHongAsync(DeleteNoiGioiThieuHopDongVo vo)
        {
            if (vo.LaDichVuKham == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.DeleteAsync(entity);
                }
            }

            if (vo.LaDichVuKyThuat == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaDichVuGiuong == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaDuocPham == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaVatTu == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                     _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.DeleteAsync(entity);
                }
            }
        }

        public async Task XuLyDeleteCauHinhHoaHong(DeleteNoiGioiThieuHopDongVo vo)
        {
            if (vo.LaDichVuKham == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                   await _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.DeleteAsync(entity);
                }
            }

            if (vo.LaDichVuKyThuat == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                    await _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaDichVuGiuong == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                    await _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaDuocPham == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                    await _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.DeleteAsync(entity);
                }
            }
            if (vo.LaVatTu == true)
            {
                var entity = await _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.GetByIdAsync(vo.Id);
                if (entity != null)
                {
                    await _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.DeleteAsync(entity);
                }
            }
        }

        #region Import cấu hình nơi giới thiệu DV

        public async Task<List<ImportNoiGioiThieuDichVu>> ImportDSDichVus(Stream path, long noiGioiThieuHopDongId, long noiGioiThieuId)
        {
            var lstError = new List<ImportNoiGioiThieuDichVu>();


            using (ExcelPackage package = new ExcelPackage(path))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["DS Cấu Hình Nơi Giới Thiệu"];
                if (workSheet == null)
                {
                    throw new Exception("Thông tin file nhập không đúng");
                }
                int totalRows = workSheet.Dimension.Rows;// dòng có data

                var infoDichVuKhams = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Select(d => new {
                    Ma = d.Ma,
                    TenDV = d.Ten,
                    DichVuKhamBenhBenhVienId = d.Id
                }).ToList();

                var infoNhomGiaDichVuKhams = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ten = d.Ten,
                       NhomGiaDichVuKhamBenhBenhVienId = d.Id
                   }).ToList();

                var infoDichVuKyThuats = _dichVuKyThuatBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ma = d.Ma,
                       TenDV = d.Ten,
                       DichVuKyThuatBenhVienId = d.Id
                   }).ToList();

                var infoNhomGiaDichVuKyThuats = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ten = d.Ten,
                       NhomGiaDichVuKyThuatBenhVienId = d.Id
                   }).ToList();

                var infoDichVuGiuongs = _dichVuGiuongBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ma = d.Ma,
                       TenDV = d.Ten,
                       DichVuGiuongBenhVienId = d.Id
                   }).ToList();

                var infoNhomGiaDichVuGiuongs = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ten = d.Ten,
                       NhomGiaDichVuGiuongBenhVienId = d.Id
                   }).ToList();



                if (totalRows >= 3)// dòng 5 bắt đầu có data
                {
                    var danhSachs = new List<ImportNoiGioiThieuDichVu>();
                    var danhSachThoaManDieuKiens = new List<ImportNoiGioiThieuDichVu>();
                    for (int i = 5; i <= totalRows + 2; i++)
                    {
                        // ...Cells[i, 4] => i = Row, 4 = Column
                        var importNoiGioiThieuDichVu = new ImportNoiGioiThieuDichVu();
                        importNoiGioiThieuDichVu = GanThongTinDichVu(importNoiGioiThieuDichVu, workSheet, i);

                        if (importNoiGioiThieuDichVu.LaDichVu == "1")
                        {
                            if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1) || 
                              (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma) && 
                               !string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu)) ||
                              !string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu) ||
                               (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia) && IsKieuSo(importNoiGioiThieuDichVu.DonGia) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan1) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan2) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan2) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan3) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan3) == false)
                                )
                              
                            {
                                var value = false;
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Mã dịch vụ chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Tên dịch vụ chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Nhóm giá dịch vụ chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                
                                




                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá nơi giới thiệu từ lần 1 chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số lần 1 chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma) &&
                                    !string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu) &&
                                    (infoDichVuKhams.Where(d => d.TenDV.ToLower() == importNoiGioiThieuDichVu.TenDichVu.ToLower()
                                                                        && d.Ma.ToLower() == importNoiGioiThieuDichVu.Ma.ToLower())
                                                   .Select(d => d.DichVuKhamBenhBenhVienId).FirstOrDefault() == null ||
                                     infoDichVuKhams.Where(d => d.TenDV.ToLower() == importNoiGioiThieuDichVu.TenDichVu.ToLower()
                                                                        && d.Ma.ToLower() == importNoiGioiThieuDichVu.Ma.ToLower())
                                                   .Select(d => d.DichVuKhamBenhBenhVienId).FirstOrDefault() == 0))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Mã và tên dịch vụ khám không tồn tại";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);

                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu))
                                {
                                    var dv = infoNhomGiaDichVuKhams.Where(d => d.Ten.ToLower() == importNoiGioiThieuDichVu.NhomGiaDichVu.ToLower()).Select(d => d.NhomGiaDichVuKhamBenhBenhVienId).FirstOrDefault();
                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importNoiGioiThieuDichVu.IsError = true;

                                        errorMess = "Nhóm giá dịch vụ khám không tồn tại.";

                                        var error = new ImportNoiGioiThieuDichVu();
                                        error = GanThongTinDichVu(error, workSheet, i);
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importNoiGioiThieuDichVu);
                                    }
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia) && IsKieuSo(importNoiGioiThieuDichVu.DonGia) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá NGT từ lần 1 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan1) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số 1 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá NGT từ lần 2 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan2) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan2) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số 2 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá NGT từ lần 3 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan3) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan3) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số 3 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                               
                                // ////////
                                if(value == false)
                                {
                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    danhSachThoaManDieuKiens.Add(importNoiGioiThieuDichVu);
                                }
                            }
                            else
                            {
                                var error = new ImportNoiGioiThieuDichVu();
                                error = GanThongTinDichVu(error, workSheet, i);
                                danhSachThoaManDieuKiens.Add(importNoiGioiThieuDichVu);
                            }






                        }

                        if (importNoiGioiThieuDichVu.LaDichVu == "2")
                        {
                            if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1) ||
                               (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma) &&
                                !string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu)) ||
                               !string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu) ||
                               (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia) && IsKieuSo(importNoiGioiThieuDichVu.DonGia) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan1) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan2) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan2) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan3) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan3) == false)
                               )
                            {
                                var value = false;
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Mã dịch vụ chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Tên dịch vụ chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu) )
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Nhóm giá dịch vụ chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                 

                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá nơi giới thiệu từ lần 1 chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số lần 1 chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }


                                

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma) &&
                      !string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu) &&
                      (infoDichVuKyThuats.Where(d => d.TenDV.ToLower() == importNoiGioiThieuDichVu.TenDichVu.ToLower()
                                                          && d.Ma.ToLower() == importNoiGioiThieuDichVu.Ma.ToLower())
                                     .Select(d => d.DichVuKyThuatBenhVienId).FirstOrDefault() == null ||
                       infoDichVuKyThuats.Where(d => d.TenDV.ToLower() == importNoiGioiThieuDichVu.TenDichVu.ToLower()
                                                          && d.Ma.ToLower() == importNoiGioiThieuDichVu.Ma.ToLower())
                                     .Select(d => d.DichVuKyThuatBenhVienId).FirstOrDefault() == 0))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Mã và tên dịch vụ kỹ thuật không tồn tại";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);

                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu))
                                {
                                    var dv = infoNhomGiaDichVuKyThuats.Where(d => d.Ten.ToLower() == importNoiGioiThieuDichVu.NhomGiaDichVu.ToLower()).Select(d => d.NhomGiaDichVuKyThuatBenhVienId).FirstOrDefault();
                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importNoiGioiThieuDichVu.IsError = true;

                                        errorMess = "Nhóm giá dịch vụ kỹ thuật không tồn tại.";

                                        var error = new ImportNoiGioiThieuDichVu();
                                        error = GanThongTinDichVu(error, workSheet, i);
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importNoiGioiThieuDichVu);
                                    }
                                }


                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia) && IsKieuSo(importNoiGioiThieuDichVu.DonGia) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá NGT từ lần 1 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan1) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số 1 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá NGT từ lần 2 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan2) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan2) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số 2 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá NGT từ lần 3 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan3) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan3) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số 3 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                /////
                                ///
                                if(value == false)
                                {
                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    danhSachThoaManDieuKiens.Add(importNoiGioiThieuDichVu);
                                }
                            }
                            else
                            {
                                var error = new ImportNoiGioiThieuDichVu();
                                error = GanThongTinDichVu(error, workSheet, i);
                                danhSachThoaManDieuKiens.Add(importNoiGioiThieuDichVu);
                            }






                        }

                        if (importNoiGioiThieuDichVu.LaDichVu == "3")
                        {
                            if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) ||
                               string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1) ||
                               (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma) &&
                                !string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu)) ||
                               !string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu) ||
                               (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia) && IsKieuSo(importNoiGioiThieuDichVu.DonGia) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan1) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan2) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan2) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) == false) ||
                                (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan3) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan3) == false)
                               )
                              {
                                var value = false;
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Mã dịch vụ chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Tên dịch vụ chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Nhóm giá dịch vụ chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                          

                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá nơi giới thiệu từ lần 1 chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số lần 1 chưa nhập.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }


                               

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.Ma) &&
                              !string.IsNullOrEmpty(importNoiGioiThieuDichVu.TenDichVu) &&
                              (infoDichVuGiuongs.Where(d => d.TenDV.ToLower() == importNoiGioiThieuDichVu.TenDichVu.ToLower()
                                                                  && d.Ma.ToLower() == importNoiGioiThieuDichVu.Ma.ToLower())
                                             .Select(d => d.DichVuGiuongBenhVienId).FirstOrDefault() == null ||
                               infoDichVuGiuongs.Where(d => d.TenDV.ToLower() == importNoiGioiThieuDichVu.TenDichVu.ToLower()
                                                                  && d.Ma.ToLower() == importNoiGioiThieuDichVu.Ma.ToLower())
                                             .Select(d => d.DichVuGiuongBenhVienId).FirstOrDefault() == 0))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Mã và tên dịch vụ giường không tồn tại";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);

                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.NhomGiaDichVu))
                                {
                                    var dv = infoNhomGiaDichVuGiuongs.Where(d => d.Ten.ToLower() == importNoiGioiThieuDichVu.NhomGiaDichVu.ToLower()).Select(d => d.NhomGiaDichVuGiuongBenhVienId).FirstOrDefault();
                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importNoiGioiThieuDichVu.IsError = true;

                                        errorMess = "Nhóm giá dịch vụ giường không tồn tại.";

                                        var error = new ImportNoiGioiThieuDichVu();
                                        error = GanThongTinDichVu(error, workSheet, i);
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importNoiGioiThieuDichVu);
                                    }
                                }




                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGia) && IsKieuSo(importNoiGioiThieuDichVu.DonGia) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan1) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá NGT từ lần 1 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan1) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan1) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số 1 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan2) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá NGT từ lần 2 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan2) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan2) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số 2 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) && IsKieuSo(importNoiGioiThieuDichVu.DonGiaNGTTuLan3) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Đơn giá NGT từ lần 3 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                if (!string.IsNullOrEmpty(importNoiGioiThieuDichVu.HeSoLan3) && IsKieuSo(importNoiGioiThieuDichVu.HeSoLan3) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDichVu.IsError = true;

                                    errorMess = "Hệ số 3 nhập chưa đúng.";

                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDichVu);
                                }
                                ////
                                //
                                if(value == false)
                                {
                                    var error = new ImportNoiGioiThieuDichVu();
                                    error = GanThongTinDichVu(error, workSheet, i);
                                    danhSachThoaManDieuKiens.Add(importNoiGioiThieuDichVu);
                                }
                            }
                            else
                            {
                                var error = new ImportNoiGioiThieuDichVu();
                                error = GanThongTinDichVu(error, workSheet, i);
                                danhSachThoaManDieuKiens.Add(importNoiGioiThieuDichVu);
                            }






                        }
                    }

                    if (danhSachThoaManDieuKiens.Count() > 0 && lstError.Count() == 0)
                    {
                        XuLyTaoDichVu(noiGioiThieuHopDongId, noiGioiThieuId, danhSachThoaManDieuKiens);
                    }
                }
            }
            foreach (var item in lstError)
            {
                item.TenDV = item.TenDichVu;
            }
            return lstError;
        }
        private async Task XuLyTaoDichVu(long noiGioiThieuHopDongId, long noiGioiThieuId, List<ImportNoiGioiThieuDichVu> models)
        {
            CauHinhHeSoTheoThoiGianHoaHong vo = new CauHinhHeSoTheoThoiGianHoaHong();
            vo.NoiGioiThieuHopDongId = noiGioiThieuHopDongId;
            vo.ChonLoaiDichVuId = 0;
            vo.NoiGioiThieuId = noiGioiThieuId;

            var infoDichVuKhams = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Select(d => new {
                    Ma = d.Ma,
                    TenDV = d.Ten,
                    DichVuKhamBenhBenhVienId = d.Id
                }).ToList();

            var infoNhomGiaDichVuKhams = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ten = d.Ten,
                   NhomGiaDichVuKhamBenhBenhVienId = d.Id
               }).ToList();

            var infoDichVuKyThuats = _dichVuKyThuatBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ma = d.Ma,
                   TenDV = d.Ten,
                   DichVuKyThuatBenhVienId = d.Id
               }).ToList();

            var infoNhomGiaDichVuKyThuats = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ten = d.Ten,
                   NhomGiaDichVuKyThuatBenhVienId = d.Id
               }).ToList();

            var infoDichVuGiuongs = _dichVuGiuongBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ma = d.Ma,
                   TenDV = d.Ten,
                   DichVuGiuongBenhVienId = d.Id
               }).ToList();

            var infoNhomGiaDichVuGiuongs = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ten = d.Ten,
                   NhomGiaDichVuGiuongBenhVienId = d.Id
               }).ToList();


            foreach (var item in models)
            {
                if(item.LaDichVu == "1")
                {

                    var obj = new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo();


                    obj.DichVuKhamBenhBenhVienId = infoDichVuKhams.Where(d => d.TenDV.ToLower() == item.TenDichVu.ToLower()
                                                                        && d.Ma.ToLower() == item.Ma.ToLower()).Select(d => d.DichVuKhamBenhBenhVienId).FirstOrDefault();


                    obj.NhomGiaDichVuKhamBenhBenhVienId = infoNhomGiaDichVuKhams.Where(d => d.Ten.ToLower() == item.NhomGiaDichVu.ToLower()).Select(d => d.NhomGiaDichVuKhamBenhBenhVienId).FirstOrDefault();


                    obj.DonGiaNGTTuLan1 = Convert.ToDecimal(item.DonGiaNGTTuLan1);
                    obj.HeSoLan1 = Convert.ToDecimal(item.HeSoLan1);
                    obj.DonGia = Convert.ToDecimal(item.DonGia);


                    obj.LaDichVuKham = true;

                    if (!string.IsNullOrEmpty(item.DonGia))
                    {
                        obj.DonGia = Convert.ToDecimal(item.DonGia);
                    }

                    if (!string.IsNullOrEmpty(item.DonGiaNGTTuLan2))
                    {
                        obj.DonGiaNGTTuLan2 = Convert.ToDecimal(item.DonGiaNGTTuLan2);
                    }
                    else
                    {
                        obj.DonGiaNGTTuLan2 = null;
                    }
                    if (!string.IsNullOrEmpty(item.HeSoLan2))
                    {
                        obj.HeSoLan2 = Convert.ToDecimal(item.HeSoLan2);
                    }
                    else
                    {
                        obj.HeSoLan2 = null;
                    }

                    if (!string.IsNullOrEmpty(item.DonGiaNGTTuLan3))
                    {
                        obj.DonGiaNGTTuLan3 = Convert.ToDecimal(item.DonGiaNGTTuLan3);
                    }
                    else
                    {
                        obj.DonGiaNGTTuLan3 = null;
                    }
                    if (!string.IsNullOrEmpty(item.HeSoLan3))
                    {
                        obj.HeSoLan3 = Convert.ToDecimal(item.HeSoLan3);
                    }
                    else
                    {
                        obj.HeSoLan3 = null;
                    }
                  
                    vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Add(obj);
                }

                if (item.LaDichVu == "2")
                {

                    var obj = new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo();


                    obj.DichVuKyThuatBenhVienId = infoDichVuKyThuats.Where(d => d.TenDV.ToLower() == item.TenDichVu.ToLower()
                                                                        && d.Ma.ToLower() == item.Ma.ToLower()).Select(d => d.DichVuKyThuatBenhVienId).FirstOrDefault();


                    obj.NhomGiaDichVuKyThuatBenhVienId = infoNhomGiaDichVuKyThuats.Where(d => d.Ten.ToLower() == item.NhomGiaDichVu.ToLower()).Select(d => d.NhomGiaDichVuKyThuatBenhVienId).FirstOrDefault();


                    obj.DonGiaNGTTuLan1 = Convert.ToDecimal(item.DonGiaNGTTuLan1);
                    obj.HeSoLan1 = Convert.ToDecimal(item.HeSoLan1);
                    obj.DonGia = Convert.ToDecimal(item.DonGia);


                    obj.LaDichVuKyThuat = true;

                    if (!string.IsNullOrEmpty(item.DonGia))
                    {
                        obj.DonGia = Convert.ToDecimal(item.DonGia);
                    }

                    if (!string.IsNullOrEmpty(item.DonGiaNGTTuLan2))
                    {
                        obj.DonGiaNGTTuLan2 = Convert.ToDecimal(item.DonGiaNGTTuLan2);
                    }
                    else
                    {
                        obj.DonGiaNGTTuLan2 = null;
                    }
                    if (!string.IsNullOrEmpty(item.HeSoLan2))
                    {
                        obj.HeSoLan2 = Convert.ToDecimal(item.HeSoLan2);
                    }
                    else
                    {
                        obj.HeSoLan2 = null;
                    }

                    if (!string.IsNullOrEmpty(item.DonGiaNGTTuLan3))
                    {
                        obj.DonGiaNGTTuLan3 = Convert.ToDecimal(item.DonGiaNGTTuLan3);
                    }
                    else
                    {
                        obj.DonGiaNGTTuLan3 = null;
                    }
                    if (!string.IsNullOrEmpty(item.HeSoLan3))
                    {
                        obj.HeSoLan3 = Convert.ToDecimal(item.HeSoLan3);
                    }
                    else
                    {
                        obj.HeSoLan3 = null;
                    }
                    vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Add(obj);
                }

                if (item.LaDichVu == "3")
                {

                    var obj = new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo();


                    obj.DichVuGiuongBenhVienId = infoDichVuGiuongs.Where(d => d.TenDV.ToLower() == item.TenDichVu.ToLower()
                                                                        && d.Ma.ToLower() == item.Ma.ToLower()).Select(d => d.DichVuGiuongBenhVienId).FirstOrDefault();


                    obj.NhomGiaDichVuGiuongBenhVienId = infoNhomGiaDichVuGiuongs.Where(d => d.Ten.ToLower() == item.NhomGiaDichVu.ToLower()).Select(d => d.NhomGiaDichVuGiuongBenhVienId).FirstOrDefault();


                    obj.DonGiaNGTTuLan1 = Convert.ToDecimal(item.DonGiaNGTTuLan1);
                    obj.HeSoLan1 = Convert.ToDecimal(item.HeSoLan1);
                    obj.DonGia = Convert.ToDecimal(item.DonGia);


                    obj.LaDichVuGiuong = true;

                    if (!string.IsNullOrEmpty(item.DonGia))
                    {
                        obj.DonGia = Convert.ToDecimal(item.DonGia);
                    }

                    if (!string.IsNullOrEmpty(item.DonGiaNGTTuLan2))
                    {
                        obj.DonGiaNGTTuLan2 = Convert.ToDecimal(item.DonGiaNGTTuLan2);
                    }
                    else
                    {
                        obj.DonGiaNGTTuLan2 = null;
                    }
                    if (!string.IsNullOrEmpty(item.HeSoLan2))
                    {
                        obj.HeSoLan2 = Convert.ToDecimal(item.HeSoLan2);
                    }
                    else
                    {
                        obj.HeSoLan2 = null;
                    }

                    if (!string.IsNullOrEmpty(item.DonGiaNGTTuLan3))
                    {
                        obj.DonGiaNGTTuLan3 = Convert.ToDecimal(item.DonGiaNGTTuLan3);
                    }
                    else
                    {
                        obj.DonGiaNGTTuLan3 = null;
                    }
                    if (!string.IsNullOrEmpty(item.HeSoLan3))
                    {
                        obj.HeSoLan3 = Convert.ToDecimal(item.HeSoLan3);
                    }
                    else
                    {
                        obj.HeSoLan3 = null;
                    }

                    vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Add(obj);
                }
            }

            XuLyCapNhatImportCauHinhHeSoTheoNoiGioiThieuAsync(vo);
        }

        private ImportNoiGioiThieuDichVu GanThongTinDichVu(ImportNoiGioiThieuDichVu model, ExcelWorksheet workSheet, int i)
        {
            model.LaDichVu = workSheet.Cells[i, 2].Text;


            if (model.LaDichVu == "1")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :NhomGiaDichVu
                // 6 :DonGia
                // 7 :DonGiaNGTTuLan1
                // 8 :HeSoLan1
                // 9 :DonGiaNGTTuLan2
                // 10: HeSoLan2
                // 11: DonGiaNGTTuLan3
                // 12: HeSoLan3
                // 13: Ghi chu
                model.Ma = workSheet.Cells[i, 3].Text;
                model.TenDichVu = workSheet.Cells[i, 4].Text;
                model.NhomGiaDichVu = workSheet.Cells[i, 5].Text;

                model.DonGia = workSheet.Cells[i, 6].Text;

                model.DonGiaNGTTuLan1 = workSheet.Cells[i, 7].Text;

                model.HeSoLan1 = workSheet.Cells[i, 8].Text;

                model.DonGiaNGTTuLan2 = workSheet.Cells[i, 9].Text;

                model.HeSoLan2 = workSheet.Cells[i, 10].Text;

                model.DonGiaNGTTuLan3 = workSheet.Cells[i, 11].Text;

                model.HeSoLan3 = workSheet.Cells[i, 12].Text;

                model.GhiChu = workSheet.Cells[i, 13].Text;
            }

            if (model.LaDichVu == "2")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :NhomGiaDichVu
                // 6 :DonGia
                // 7 :DonGiaNGTTuLan1
                // 8 :HeSoLan1
                // 9 :DonGiaNGTTuLan2
                // 10: HeSoLan2
                // 11: DonGiaNGTTuLan3
                // 12: HeSoLan3
                // 13: Ghi chu
                model.Ma = workSheet.Cells[i, 3].Text;
                model.TenDichVu = workSheet.Cells[i, 4].Text;
                model.NhomGiaDichVu = workSheet.Cells[i, 5].Text;

                model.DonGia = workSheet.Cells[i, 6].Text;

                model.DonGiaNGTTuLan1 = workSheet.Cells[i, 7].Text;

                model.HeSoLan1 = workSheet.Cells[i, 8].Text;

                model.DonGiaNGTTuLan2 = workSheet.Cells[i, 9].Text;

                model.HeSoLan2 = workSheet.Cells[i, 10].Text;

                model.DonGiaNGTTuLan3 = workSheet.Cells[i, 11].Text;

                model.HeSoLan3 = workSheet.Cells[i, 12].Text;

                model.GhiChu = workSheet.Cells[i, 13].Text;
            }

            if (model.LaDichVu == "3")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :NhomGiaDichVu
                // 6 :DonGia
                // 7 :DonGiaNGTTuLan1
                // 8 :HeSoLan1
                // 9 :DonGiaNGTTuLan2
                // 10: HeSoLan2
                // 11: DonGiaNGTTuLan3
                // 12: HeSoLan3
                // 13: Ghi chu
                model.Ma = workSheet.Cells[i, 3].Text;
                model.TenDichVu = workSheet.Cells[i, 4].Text;
                model.NhomGiaDichVu = workSheet.Cells[i, 5].Text;

                model.DonGia = workSheet.Cells[i, 6].Text;

                model.DonGiaNGTTuLan1 = workSheet.Cells[i, 7].Text;

                model.HeSoLan1 = workSheet.Cells[i, 8].Text;

                model.DonGiaNGTTuLan2 = workSheet.Cells[i, 9].Text;

                model.HeSoLan2 = workSheet.Cells[i, 10].Text;

                model.DonGiaNGTTuLan3 = workSheet.Cells[i, 11].Text;

                model.HeSoLan3 = workSheet.Cells[i, 12].Text;

                model.GhiChu = workSheet.Cells[i, 13].Text;
            }

            return model;
        }

        public long GetNoiGioiThieuHopDongUuTiens(long id)
        {
            var lstNoiGioiThieuHopDongs = _noiGioiThieuHopDongRepository.TableNoTracking
                            .Where(d => d.NoiGioiThieuId == id)
                            .ToList();

            var lst = lstNoiGioiThieuHopDongs.Select(item => new LookupItemCauHinhHeSoTheoNoiGtHoaHongVo()
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                ThoiGian = item.NgayBatDau
            }).OrderByDescending(d => d.ThoiGian).ToList();

            return lst.Select(d => d.KeyId).FirstOrDefault(); ;
        }
        public string GetNoiGioiThieu(long id)
        {
            var lstNoiGioiThieuHopDongs = BaseRepository.TableNoTracking
                            .Where(d => d.Id == id).Select(d=>d.Ten)
                            .ToList();

            return lstNoiGioiThieuHopDongs.FirstOrDefault() ;
        }

        private async Task XuLyCapNhatImportCauHinhHeSoTheoNoiGioiThieuAsync(CauHinhHeSoTheoThoiGianHoaHong vo)
        {
            // nếu dịch vụ khám đã dc tạo rồi . thì cập nhật thông tin dịch vụ trong file excel theo file 
            // nếu dịch vụ khám chưa tạo thì tạo mới
            // nếu dịch vụ khám trong file không chứa dịch vụ khám dc tạo trước đó . xóa dịch vụ khám đó đi


            if (vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs != null && vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Count() > 0)
            {
                #region //----------------------------Dịch vụ khám ------------------------------------//
                var noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs = _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new
                    {
                        Id = d.Id,
                        DichVuKhamBenhBenhVienId = d.DichVuKhamBenhBenhVienId
                    }).ToList();

                var dichVuKhamIds = noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs.Select(d => d.DichVuKhamBenhBenhVienId).ToList();

                var dvKhamUpdates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuKham == true && dichVuKhamIds.Contains(d.DichVuKhamBenhBenhVienId.GetValueOrDefault())).ToList();
                var dvKhams = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh>();
                if (dvKhamUpdates.Count() > 0) // update
                {
                    var listDichVuIds = noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhIdAsync(listDichVuIds);
                    // xử lý list Dv 
                    foreach (var itemDvKhamdb in dataItems)
                    {
                        foreach (var itemUP in dvKhamUpdates)
                        {
                            if (itemUP.DichVuKhamBenhBenhVienId == itemDvKhamdb.DichVuKhamBenhBenhVienId)
                            {
                                itemDvKhamdb.DichVuKhamBenhBenhVienId = itemUP.DichVuKhamBenhBenhVienId.GetValueOrDefault();
                                itemDvKhamdb.DonGiaBenhVien = itemUP.DonGia.GetValueOrDefault();
                                itemDvKhamdb.DonGiaGioiThieuTuLan1 = itemUP.DonGiaNGTTuLan1.GetValueOrDefault();
                                itemDvKhamdb.HeSoGioiThieuTuLan1 = itemUP.HeSoLan1.GetValueOrDefault();
                                itemDvKhamdb.DonGiaGioiThieuTuLan2 = itemUP.DonGiaNGTTuLan2;
                                itemDvKhamdb.HeSoGioiThieuTuLan2 = itemUP.HeSoLan2;
                                itemDvKhamdb.DonGiaGioiThieuTuLan3 = itemUP.DonGiaNGTTuLan3;
                                itemDvKhamdb.HeSoGioiThieuTuLan3 = itemUP.HeSoLan3;

                                itemDvKhamdb.NhomGiaDichVuKhamBenhBenhVienId = itemUP.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault();

                                itemDvKhamdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                dvKhams.Add(itemDvKhamdb);
                            }


                        }

                    }
                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.UpdateAsync(dvKhams);
                }




                var dvKhamCreates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuKham == true && !dichVuKhamIds.Contains(d.DichVuKhamBenhBenhVienId.GetValueOrDefault())).ToList();

                if (dvKhamCreates.Any())
                {
                    // xử lý list Dv khám cần lưu
                    foreach (var itemDvKham in dvKhamCreates)
                    {
                        var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh()
                        {
                            DichVuKhamBenhBenhVienId = itemDvKham.DichVuKhamBenhBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDvKham.DonGia.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan1 = itemDvKham.DonGiaNGTTuLan1.GetValueOrDefault(),
                            HeSoGioiThieuTuLan1 = itemDvKham.HeSoLan1.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan2 = itemDvKham.DonGiaNGTTuLan2,
                            HeSoGioiThieuTuLan2 = itemDvKham.HeSoLan2,
                            DonGiaGioiThieuTuLan3 = itemDvKham.DonGiaNGTTuLan3,
                            HeSoGioiThieuTuLan3 = itemDvKham.HeSoLan3,

                            NhomGiaDichVuKhamBenhBenhVienId = itemDvKham.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId,
                        };
                        dvKhams.Add(viewModelDvKhamBenh);
                    }

                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                }



                var dvKhamCreateIds = dvKhamCreates.Where(d => d.DichVuKhamBenhBenhVienId != null).Select(d => d.DichVuKhamBenhBenhVienId).ToList();
                var dvKhamUpdateIds = dvKhamUpdates.Where(d => d.DichVuKhamBenhBenhVienId != null).Select(d => d.DichVuKhamBenhBenhVienId).ToList();

                var dvKhamThoaManDKIds = dvKhamCreateIds.Concat(dvKhamUpdateIds).Select(d => d.GetValueOrDefault());
                var dvKhamDeletes = noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs.Where(d => !dvKhamThoaManDKIds.Contains(d.DichVuKhamBenhBenhVienId)).Select(d => d.Id).ToList();

                var dataItemDeletes = dvKhamDeletes;
                var deleteDVKs = GetByNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhIdAsync(dataItemDeletes);
                foreach (var item in deleteDVKs)
                {
                    item.WillDelete = true;
                    dvKhams.Add(item);
                }

                #endregion//----------------------------END Dịch vụ khám --------------------------------//



                #region //----------------------------Dịch vụ kỹ thuật ------------------------------------//
                var noiGioiThieuHopDongChiTietHeSoDichVuKyThuats = _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new
                    {
                        Id = d.Id,
                        DichVuKyThuatBenhVienId = d.DichVuKyThuatBenhVienId
                    }).ToList();

                var dichVuKyThuatIds = noiGioiThieuHopDongChiTietHeSoDichVuKyThuats.Select(d => d.DichVuKyThuatBenhVienId).ToList();

                var dvKyThuatUpdates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuKyThuat == true && dichVuKyThuatIds.Contains(d.DichVuKyThuatBenhVienId.GetValueOrDefault())).ToList();
                var dvKyThuats = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat>();
                if (dvKyThuatUpdates.Count() > 0) // update
                {
                    var listDichVuIds = noiGioiThieuHopDongChiTietHeSoDichVuKyThuats.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoDichVuKyThuatIdAsync(listDichVuIds);
                    // xử lý list Dv 
                    foreach (var itemDvdb in dataItems)
                    {
                        foreach (var itemUP in dvKyThuatUpdates)
                        {
                            if (itemUP.DichVuKyThuatBenhVienId == itemDvdb.DichVuKyThuatBenhVienId)
                            {
                                itemDvdb.DichVuKyThuatBenhVienId = itemUP.DichVuKyThuatBenhVienId.GetValueOrDefault();
                                itemDvdb.DonGiaBenhVien = itemUP.DonGia.GetValueOrDefault();
                                itemDvdb.DonGiaGioiThieuTuLan1 = itemUP.DonGiaNGTTuLan1.GetValueOrDefault();
                                itemDvdb.HeSoGioiThieuTuLan1 = itemUP.HeSoLan1.GetValueOrDefault();
                                itemDvdb.DonGiaGioiThieuTuLan2 = itemUP.DonGiaNGTTuLan2;
                                itemDvdb.HeSoGioiThieuTuLan2 = itemUP.HeSoLan2;
                                itemDvdb.DonGiaGioiThieuTuLan3 = itemUP.DonGiaNGTTuLan3;
                                itemDvdb.HeSoGioiThieuTuLan3 = itemUP.HeSoLan3;

                                itemDvdb.NhomGiaDichVuKyThuatBenhVienId = itemUP.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault();

                                itemDvdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                dvKyThuats.Add(itemDvdb);
                            }


                        }

                    }
                }




                var dvKyThuatCreates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuKyThuat == true && !dichVuKyThuatIds.Contains(d.DichVuKyThuatBenhVienId.GetValueOrDefault())).ToList();

                if (dvKyThuatCreates.Any())
                {
                    // xử lý list Dv khám cần lưu
                    foreach (var itemDv in dvKyThuatCreates)
                    {
                        var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat()
                        {
                            DichVuKyThuatBenhVienId = itemDv.DichVuKyThuatBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan1 = itemDv.DonGiaNGTTuLan1.GetValueOrDefault(),
                            HeSoGioiThieuTuLan1 = itemDv.HeSoLan1.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan2 = itemDv.DonGiaNGTTuLan2,
                            HeSoGioiThieuTuLan2 = itemDv.HeSoLan2,
                            DonGiaGioiThieuTuLan3 = itemDv.DonGiaNGTTuLan3,
                            HeSoGioiThieuTuLan3 = itemDv.HeSoLan3,

                            NhomGiaDichVuKyThuatBenhVienId = itemDv.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId,
                        };
                        dvKyThuats.Add(viewModelDvKhamBenh);
                    }

                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                }



                var dvKyThuatCreateIds = dvKyThuatCreates.Where(d => d.DichVuKyThuatBenhVienId != null).Select(d => d.DichVuKyThuatBenhVienId).ToList();
                var dvKyThuatUpdateIds = dvKyThuatUpdates.Where(d => d.DichVuKyThuatBenhVienId != null).Select(d => d.DichVuKyThuatBenhVienId).ToList();

                var dvKyThuatThoaManDKIds = dvKyThuatCreateIds.Concat(dvKyThuatUpdateIds).Select(d => d.GetValueOrDefault());
                var dvKyThuatDeletes = noiGioiThieuHopDongChiTietHeSoDichVuKyThuats.Where(d => !dvKyThuatThoaManDKIds.Contains(d.DichVuKyThuatBenhVienId)).Select(d => d.Id).ToList();

                var dataItemKyThuatDeletes = dvKyThuatDeletes;
                var deleteDVKTs = GetByNoiGioiThieuHopDongChiTietHeSoDichVuKyThuatIdAsync(dataItemKyThuatDeletes);
                foreach (var item in deleteDVKTs)
                {
                    item.WillDelete = true;
                    dvKyThuats.Add(item);
                }

                #endregion//----------------------------END Dịch vụ kỹ thuật --------------------------------//



                #region //----------------------------Dịch vụ giường ------------------------------------//
                var noiGioiThieuHopDongChiTietHeSoDichVuGiuongs = _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new {
                        Id = d.Id,
                        DichVuGiuongBenhVienId = d.DichVuGiuongBenhVienId
                    }).ToList();

                var dichVuGiuongBenhIds = noiGioiThieuHopDongChiTietHeSoDichVuGiuongs.Select(d => d.DichVuGiuongBenhVienId).ToList();

                var dvGiuongtUpdates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuGiuong == true && dichVuGiuongBenhIds.Contains(d.DichVuGiuongBenhVienId.GetValueOrDefault())).ToList();
                var dvGiuongs = new List<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong>();
                if (dvGiuongtUpdates.Count() > 0) // update
                {
                    var listDichVuIds = noiGioiThieuHopDongChiTietHeSoDichVuGiuongs.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoDichVuGiuongIdAsync(listDichVuIds);
                    // xử lý list Dv 
                    foreach (var itemDvdb in dataItems)
                    {
                        foreach (var itemUP in dvGiuongtUpdates)
                        {
                            if (itemUP.DichVuGiuongBenhVienId == itemDvdb.DichVuGiuongBenhVienId)
                            {
                                itemDvdb.DichVuGiuongBenhVienId = itemUP.DichVuGiuongBenhVienId.GetValueOrDefault();
                                itemDvdb.DonGiaBenhVien = itemUP.DonGia.GetValueOrDefault();
                                itemDvdb.DonGiaGioiThieuTuLan1 = itemUP.DonGiaNGTTuLan1.GetValueOrDefault();
                                itemDvdb.HeSoGioiThieuTuLan1 = itemUP.HeSoLan1.GetValueOrDefault();
                                itemDvdb.DonGiaGioiThieuTuLan2 = itemUP.DonGiaNGTTuLan2;
                                itemDvdb.HeSoGioiThieuTuLan2 = itemUP.HeSoLan2;
                                itemDvdb.DonGiaGioiThieuTuLan3 = itemUP.DonGiaNGTTuLan3;
                                itemDvdb.HeSoGioiThieuTuLan3 = itemUP.HeSoLan3;

                                itemDvdb.NhomGiaDichVuGiuongBenhVienId = itemUP.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault();

                                itemDvdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                dvGiuongs.Add(itemDvdb);
                            }


                        }

                    }
                }




                var dvGiuongCreates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDichVuGiuong == true && !dichVuGiuongBenhIds.Contains(d.DichVuGiuongBenhVienId.GetValueOrDefault())).ToList();

                if (dvGiuongCreates.Any())
                {
                    // xử lý list Dv khám cần lưu
                    foreach (var itemDv in dvGiuongCreates)
                    {
                        var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHeSoDichVuGiuong()
                        {
                            DichVuGiuongBenhVienId = itemDv.DichVuGiuongBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan1 = itemDv.DonGiaNGTTuLan1.GetValueOrDefault(),
                            HeSoGioiThieuTuLan1 = itemDv.HeSoLan1.GetValueOrDefault(),
                            DonGiaGioiThieuTuLan2 = itemDv.DonGiaNGTTuLan2,
                            HeSoGioiThieuTuLan2 = itemDv.HeSoLan2,
                            DonGiaGioiThieuTuLan3 = itemDv.DonGiaNGTTuLan3,
                            HeSoGioiThieuTuLan3 = itemDv.HeSoLan3,

                            NhomGiaDichVuGiuongBenhVienId = itemDv.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId,
                        };
                        dvGiuongs.Add(viewModelDvKhamBenh);
                    }

                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                }



                var dvGiuongCreateIds = dvGiuongCreates.Where(d => d.DichVuGiuongBenhVienId != null).Select(d => d.DichVuGiuongBenhVienId).ToList();
                var dvGiuongUpdateIds = dvGiuongtUpdates.Where(d => d.DichVuGiuongBenhVienId != null).Select(d => d.DichVuGiuongBenhVienId).ToList();

                var dvGiuongThoaManDKIds = dvGiuongCreateIds.Concat(dvGiuongUpdateIds).Select(d => d.GetValueOrDefault());
                var dvGiuongDeletes = noiGioiThieuHopDongChiTietHeSoDichVuGiuongs.Where(d => !dvGiuongThoaManDKIds.Contains(d.DichVuGiuongBenhVienId)).Select(d => d.Id).ToList();

                var dataItemGiuongDeletes = dvGiuongDeletes;
                var deleteDVGs = GetByNoiGioiThieuHopDongChiTietHeSoDichVuGiuongIdAsync(dataItemGiuongDeletes);
                foreach (var item in deleteDVGs)
                {
                    item.WillDelete = true;
                    dvGiuongs.Add(item);
                }

                #endregion//----------------------------END Dịch vụ giường --------------------------------//
                if (dvKhams.Count() > 0)
                {
                    if (dvKhams.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = dvKhams.Where(d => d.Id == 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRange(dv);
                    }
                    if (dvKhams.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = dvKhams.Where(d => d.Id != 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.UpdateAsync(dvKhams);
                    }

                }
                if (dvKyThuats.Count() > 0)
                {
                    if (dvKyThuats.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = dvKyThuats.Where(d => d.Id == 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.AddRange(dv);
                    }
                    if (dvKyThuats.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = dvKyThuats.Where(d => d.Id != 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoDichVuKyThuatRepository.UpdateAsync(dv);
                    }
                }
                if (dvGiuongs.Count() > 0)
                {
                    if (dvGiuongs.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = dvGiuongs.Where(d => d.Id == 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.AddRange(dv);
                    }
                    if (dvGiuongs.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = dvGiuongs.Where(d => d.Id != 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoDichVuGiuongRepository.UpdateAsync(dv);
                    }
                }




            }
        }

        #endregion

        #region Import cấu hình nơi giới thiệu DV

        public async Task<List<ImportNoiGioiThieuDuocPhamVTYT>> ImportNoiGioiThieuDuocPhamVTYTs(Stream path, long noiGioiThieuHopDongId, long noiGioiThieuId)
        {
            var lstError = new List<ImportNoiGioiThieuDuocPhamVTYT>();


            using (ExcelPackage package = new ExcelPackage(path))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["DS Cấu Hình Nơi Giới Thiệu"];
                if (workSheet == null)
                {
                    throw new Exception("Thông tin file nhập không đúng");
                }
                int totalRows = workSheet.Dimension.Rows;// dòng có data

                var infoDuocPhams = _duocPhamBenhVienRepository.TableNoTracking.Include(d=>d.DuocPham)
                .Select(d => new {
                    Ma = d.Ma,
                    Ten = d.DuocPham.Ten,
                    DuocPhamBenhVienId = d.Id
                }).ToList();


                var infoVatTus = _vatTuBenhVienRepository.TableNoTracking.Include(d => d.VatTus)
                   .Select(d => new {
                       Ma = d.Ma,
                       Ten = d.VatTus.Ten,
                       VatTuBenhVienId = d.Id
                   }).ToList();

                var datas = Enum.GetValues(typeof(LoaiGiaNoiGioiThieuHopDong)).Cast<Enum>();
                var dsGias = datas.Select(o => new LookupItemVo
                {
                    DisplayName = o.GetDescription(),
                    KeyId = Convert.ToInt32(o)
                });

                if (totalRows >= 3)// dòng 5 bắt đầu có data
                {
                    var danhSachs = new List<ImportNoiGioiThieuDuocPhamVTYT>();
                    var danhSachThoaManDieuKiens = new List<ImportNoiGioiThieuDuocPhamVTYT>();
                    for (int i = 5; i <= totalRows + 2; i++)
                    {
                        // ...Cells[i, 4] => i = Row, 4 = Column
                        var importNoiGioiThieuDuocPhamVTYT = new ImportNoiGioiThieuDuocPhamVTYT();
                        importNoiGioiThieuDuocPhamVTYT = GanThongTinDpVTYT(importNoiGioiThieuDuocPhamVTYT, workSheet, i);

                        if (importNoiGioiThieuDuocPhamVTYT.LaDichVu == "1")
                        {
                            if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ma) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ten) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.NhomGia) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.HeSo) ||
                             
                              (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ma) &&
                               !string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ten)) ||
                              !string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.NhomGia) ||
                               (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.HeSo) && IsKieuSo(importNoiGioiThieuDuocPhamVTYT.HeSo) == false)
                              )

                            {
                                var value = false;
                                if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Mã dược phẩm chưa nhập.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ten))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Tên dược phẩm chưa nhập.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.NhomGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Nhóm giá dược phẩm chưa nhập.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }

                                if(!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.HeSo) && IsKieuSo(importNoiGioiThieuDuocPhamVTYT.HeSo) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Hệ số nhập không đúng.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }


                                if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.HeSo))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Hệ số chưa nhập.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }


                               

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ma) &&
                                     !string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ten) ||
                                     (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.NhomGia)))
                                {
                                    var dv = infoDuocPhams.Where(d => d.Ten.ToLower() == importNoiGioiThieuDuocPhamVTYT.Ten.ToLower()
                                                                        && d.Ma.ToLower() == importNoiGioiThieuDuocPhamVTYT.Ma.ToLower()).Select(d => d.DuocPhamBenhVienId).FirstOrDefault();
                                    var dvGia = dsGias.Where(d => d.DisplayName.ToLower() == importNoiGioiThieuDuocPhamVTYT.NhomGia).Select(d => d.KeyId).FirstOrDefault();
                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                        errorMess = "Mã và tên dược phẩm không tồn tại";

                                        var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                        error = GanThongTinDpVTYT(error, workSheet, i);
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                    }
                                    if (dvGia == null)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                        errorMess = "Nhóm giá dược phẩm không tồn tại.";

                                        var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                        error = GanThongTinDpVTYT(error, workSheet, i);
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                    }
                                    //else if (dvGia != null && dv != null)
                                    //{
                                    //    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    //    error = GanThongTinDpVTYT(error, workSheet, i);
                                    //    danhSachThoaManDieuKiens.Add(importNoiGioiThieuDuocPhamVTYT);
                                    //}

                                }


                                if (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.HeSo) && IsKieuSo(importNoiGioiThieuDuocPhamVTYT.HeSo) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Hệ số nhập không đúng.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }

                                ////
                                ///
                                if(value == false)
                                {
                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    danhSachThoaManDieuKiens.Add(importNoiGioiThieuDuocPhamVTYT);
                                }
                            }
                            else
                            {
                                var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                error = GanThongTinDpVTYT(error, workSheet, i);
                                danhSachThoaManDieuKiens.Add(importNoiGioiThieuDuocPhamVTYT);
                            }






                        }


                        if (importNoiGioiThieuDuocPhamVTYT.LaDichVu == "2")
                        {
                            if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ma) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ten) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.NhomGia) ||
                              string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.HeSo) ||

                              (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ma) &&
                               !string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ten)) ||
                              !string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.NhomGia) ||
                              (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.HeSo) && IsKieuSo(importNoiGioiThieuDuocPhamVTYT.HeSo) == false)
                              )

                            {
                                var value = false;
                                if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Mã vật tư chưa nhập.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ten))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Tên vật tư chưa nhập.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }
                                if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.NhomGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Nhóm giá vật tư chưa nhập.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }

                                if (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ma) && 
                                    !string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.Ten)  || 
                                    (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.NhomGia)))
                                {
                                    var dv = infoVatTus.Where(d => d.Ten.ToLower() == importNoiGioiThieuDuocPhamVTYT.Ten.ToLower()
                                                                        && d.Ma.ToLower() == importNoiGioiThieuDuocPhamVTYT.Ma.ToLower()).Select(d => d.VatTuBenhVienId).FirstOrDefault();
                                    var dvGia = dsGias.Where(d => d.DisplayName.ToLower() == importNoiGioiThieuDuocPhamVTYT.NhomGia).Select(d => d.KeyId).FirstOrDefault();
                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                        errorMess = "Mã và tên vật tư không tồn tại";

                                        var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                        error = GanThongTinDpVTYT(error, workSheet, i);
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                    }
                                    if (dvGia == null)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                        errorMess = "Nhóm giá vật tư không tồn tại.";

                                        var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                        error = GanThongTinDpVTYT(error, workSheet, i);
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                    }
                                }


                                if (!string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.HeSo) && IsKieuSo(importNoiGioiThieuDuocPhamVTYT.HeSo) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Hệ số nhập không đúng.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }




                                if (string.IsNullOrEmpty(importNoiGioiThieuDuocPhamVTYT.HeSo))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importNoiGioiThieuDuocPhamVTYT.IsError = true;

                                    errorMess = "Đơn giá chưa nhập.";

                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importNoiGioiThieuDuocPhamVTYT);
                                }

                                if(value == false)
                                {
                                    var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                    error = GanThongTinDpVTYT(error, workSheet, i);
                                    danhSachThoaManDieuKiens.Add(importNoiGioiThieuDuocPhamVTYT);
                                }
                            }
                            else
                            {
                                var error = new ImportNoiGioiThieuDuocPhamVTYT();
                                error = GanThongTinDpVTYT(error, workSheet, i);
                                danhSachThoaManDieuKiens.Add(importNoiGioiThieuDuocPhamVTYT);
                            }






                        }
                    }

                    if (danhSachThoaManDieuKiens.Count() > 0 && lstError.Count() == 0)
                    {
                        XuLyTaoDPVTYT(noiGioiThieuHopDongId, noiGioiThieuId, danhSachThoaManDieuKiens);
                    }
                }
            }

            foreach (var item in lstError)
            {
                item.TenDV = item.Ten;
            }
            return lstError;
        }
        private ImportNoiGioiThieuDuocPhamVTYT GanThongTinDpVTYT(ImportNoiGioiThieuDuocPhamVTYT model, ExcelWorksheet workSheet, int i)
        {
            model.LaDichVu = workSheet.Cells[i, 2].Text;


            if (model.LaDichVu == "1")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :NhomGiaDichVu
                // 6 :HeSo
                // 7: Ghi chu

                model.Ma = workSheet.Cells[i, 3].Text;
                model.Ten = workSheet.Cells[i, 4].Text;
                model.NhomGia = workSheet.Cells[i, 5].Text;

                model.HeSo = workSheet.Cells[i, 6].Text;

                model.GhiChu = workSheet.Cells[i, 7].Text;
            }

            if (model.LaDichVu == "2")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :NhomGiaDichVu
                // 6 :HeSo
                // 7: Ghi chu

                model.Ma = workSheet.Cells[i, 3].Text;
                model.Ten = workSheet.Cells[i, 4].Text;
                model.NhomGia = workSheet.Cells[i, 5].Text;

                model.HeSo = workSheet.Cells[i, 6].Text;

                model.GhiChu = workSheet.Cells[i, 7].Text;
            }



            return model;
        }

        private async Task XuLyTaoDPVTYT(long noiGioiThieuHopDongId, long noiGioiThieuId, List<ImportNoiGioiThieuDuocPhamVTYT> models)
        {
            CauHinhHeSoTheoThoiGianHoaHong vo = new CauHinhHeSoTheoThoiGianHoaHong();
            vo.NoiGioiThieuHopDongId = noiGioiThieuHopDongId;
            vo.ChonLoaiDichVuId = 0;
            vo.NoiGioiThieuId = noiGioiThieuId;

            var infoDuocPhams = _duocPhamBenhVienRepository.TableNoTracking.Include(d => d.DuocPham)
                 .Select(d => new {
                     Ma = d.Ma,
                     Ten = d.DuocPham.Ten,
                     DuocPhamBenhVienId = d.Id
                 }).ToList();


            var infoVatTus = _vatTuBenhVienRepository.TableNoTracking.Include(d => d.VatTus)
               .Select(d => new {
                   Ma = d.Ma,
                   Ten = d.VatTus.Ten,
                   VatTuBenhVienId = d.Id
               }).ToList();

            var datas = Enum.GetValues(typeof(LoaiGiaNoiGioiThieuHopDong)).Cast<Enum>();
            var dsGias = datas.Select(o => new LookupItemVo
            {
                DisplayName = o.GetDescription(),
                KeyId = Convert.ToInt32(o)
            });


            foreach (var item in models)
            {
                if (item.LaDichVu == "1")
                {

                    var obj = new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo();


                    obj.DuocPhamBenhVienId = infoDuocPhams.Where(d => d.Ten.ToLower().Trim() == item.Ten.ToLower().Trim()
                                                                        && d.Ma.ToLower().Trim() == item.Ma.ToLower().Trim()).Select(d => d.DuocPhamBenhVienId).FirstOrDefault();


                    obj.NhomGiaThuocId = dsGias.Where(d => d.DisplayName.ToLower().Trim() == item.NhomGia.ToLower().Trim()).Select(d => (LoaiGiaNoiGioiThieuHopDong)d.KeyId).FirstOrDefault();


                    
                    obj.HeSo = Convert.ToDecimal(item.HeSo);


                    obj.LaDuocPham = true;


                    vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Add(obj);
                }

                if (item.LaDichVu == "2")
                {

                    var obj = new ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo();


                    obj.VatTuBenhVienId = infoVatTus.Where(d => d.Ten.ToLower().Trim() == item.Ten.ToLower().Trim()
                                                                        && d.Ma.ToLower().Trim() == item.Ma.ToLower().Trim()).Select(d => d.VatTuBenhVienId).FirstOrDefault();


                    obj.NhomGiaVTYTId = dsGias.Where(d => d.DisplayName.ToLower().Trim() == item.NhomGia.ToLower().Trim()).Select(d => (LoaiGiaNoiGioiThieuHopDong)d.KeyId).FirstOrDefault();



                    obj.HeSo = Convert.ToDecimal(item.HeSo);


                    obj.LaVatTu = true;


                    vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Add(obj);
                }
            }

            XuLyCapNhatImportDPVTYAsync(vo);
        }


        private async Task XuLyCapNhatImportDPVTYAsync(CauHinhHeSoTheoThoiGianHoaHong vo)
        {
            // nếu DP/VT đã dc tạo rồi . thì cập nhật thông tin DP/VT trong file excel theo file 
            // nếu DP/VT chưa tạo thì tạo mới
            // nếu DP/VT trong file không chứa dịch vụ khám dc tạo trước đó . xóa DP/VT khám đó đi


            if (vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs != null && vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Count() > 0)
            {
                #region //----------------------------Dược phẩm  ------------------------------------//
                var noiGioiThieuHopDongChiTietHeSoDuocPhams = _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new
                    {
                        Id = d.Id,
                        DuocPhamBenhVienId = d.DuocPhamBenhVienId
                    }).ToList();

                var duocPhamIds = noiGioiThieuHopDongChiTietHeSoDuocPhams.Select(d => d.DuocPhamBenhVienId).ToList();

                var dsDuocPhamUpdates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDuocPham == true && duocPhamIds.Contains(d.DuocPhamBenhVienId.GetValueOrDefault())).ToList();
                var dsDuocPhams = new List<NoiGioiThieuHopDongChiTietHeSoDuocPham>();

                if (dsDuocPhamUpdates.Count() > 0) // update
                {
                    var listDuocPhamIds = noiGioiThieuHopDongChiTietHeSoDuocPhams.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoDuocPhamIdAsync(listDuocPhamIds);
                    // xử lý list Dv 
                    foreach (var itemdb in dataItems)
                    {
                        foreach (var itemUP in dsDuocPhamUpdates)
                        {
                            if (itemUP.DuocPhamBenhVienId == itemdb.DuocPhamBenhVienId)
                            {
                                itemdb.DuocPhamBenhVienId = itemUP.DuocPhamBenhVienId.GetValueOrDefault();
                                itemdb.HeSo = itemUP.HeSo.GetValueOrDefault();
                                itemdb.LoaiGia = itemUP.NhomGiaThuocId.GetValueOrDefault();

                                itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                dsDuocPhams.Add(itemdb);
                            }


                        }

                    }
                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.UpdateAsync(dvKhams);
                }




                var dvDuocPhamCreates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDuocPham == true && !duocPhamIds.Contains(d.DuocPhamBenhVienId.GetValueOrDefault())).ToList();

                if (dvDuocPhamCreates.Any())
                {
                    // xử lý list Dv khám cần lưu
                    foreach (var item in dvDuocPhamCreates)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoDuocPham()
                        {
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId.GetValueOrDefault(),
                            HeSo = item.HeSo.GetValueOrDefault(),
                            LoaiGia = item.NhomGiaThuocId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        dsDuocPhams.Add(viewModelDv);
                    }

                }



                var dvDuocPhamCreateIds = dvDuocPhamCreates.Where(d => d.DuocPhamBenhVienId != null).Select(d => d.DuocPhamBenhVienId).ToList();
                var dvDuocPhamUpdateIds = dsDuocPhamUpdates.Where(d => d.DuocPhamBenhVienId != null).Select(d => d.DuocPhamBenhVienId).ToList();

                var dvDuocPhamThoaManDKIds = dvDuocPhamCreateIds.Concat(dvDuocPhamUpdateIds).Select(d => d.GetValueOrDefault());
                var dvDuocPhamDeletes = noiGioiThieuHopDongChiTietHeSoDuocPhams.Where(d => !dvDuocPhamThoaManDKIds.Contains(d.DuocPhamBenhVienId)).Select(d => d.Id).ToList();

                var dataItemDeletes = dvDuocPhamDeletes;
                var deleteDuocPhams = GetByNoiGioiThieuHopDongChiTietHeSoDuocPhamIdAsync(dataItemDeletes);
                foreach (var item in deleteDuocPhams)
                {
                    item.WillDelete = true;
                    dsDuocPhams.Add(item);
                }

                #endregion //----------------------------END Dược phẩm --------------------------------//



                #region //----------------------------Vật tư  ------------------------------------//
                var noiGioiThieuHopDongChiTietHeSoVatTus = _noiGioiThieuHopDongChiTietHeSoVatTuRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new
                    {
                        Id = d.Id,
                        VatTuBenhVienId = d.VatTuBenhVienId
                    }).ToList();

                var vatTuIds = noiGioiThieuHopDongChiTietHeSoVatTus.Select(d => d.VatTuBenhVienId).ToList();

                var dsVatTuUpdates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaVatTu == true && vatTuIds.Contains(d.VatTuBenhVienId.GetValueOrDefault())).ToList();
                var dsVatTus = new List<NoiGioiThieuHopDongChiTietHeSoVatTu>();

                if (dsVatTuUpdates.Count() > 0) // update
                {
                    var listVatTuIds = noiGioiThieuHopDongChiTietHeSoVatTus.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHeSoVatTuIdAsync(listVatTuIds);
                    // xử lý list Dv 
                    foreach (var itemdb in dataItems)
                    {
                        foreach (var itemUP in dsVatTuUpdates)
                        {
                            if (itemUP.VatTuBenhVienId == itemdb.VatTuBenhVienId)
                            {
                                itemdb.VatTuBenhVienId = itemUP.VatTuBenhVienId.GetValueOrDefault();
                                itemdb.HeSo = itemUP.HeSo.GetValueOrDefault();
                                itemdb.LoaiGia = itemUP.NhomGiaVTYTId.GetValueOrDefault();

                                itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                dsVatTus.Add(itemdb);
                            }


                        }

                    }
                }




                var dvVatTuCreates = vo.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaVatTu == true && !vatTuIds.Contains(d.VatTuBenhVienId.GetValueOrDefault())).ToList();

                if (dvVatTuCreates.Any())
                {
                    // xử lý list Dv khám cần lưu
                    foreach (var item in dvVatTuCreates)
                    {
                        var viewModelDv = new NoiGioiThieuHopDongChiTietHeSoVatTu()
                        {
                            VatTuBenhVienId = item.VatTuBenhVienId.GetValueOrDefault(),
                            HeSo = item.HeSo.GetValueOrDefault(),
                            LoaiGia = item.NhomGiaVTYTId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId
                        };
                        dsVatTus.Add(viewModelDv);
                    }

                }



                var dvVatTuCreateIds = dvVatTuCreates.Where(d => d.VatTuBenhVienId != null).Select(d => d.VatTuBenhVienId).ToList();
                var dvVatTuUpdateIds = dsVatTuUpdates.Where(d => d.VatTuBenhVienId != null).Select(d => d.VatTuBenhVienId).ToList();

                var dvVatTuThoaManDKIds = dvVatTuCreateIds.Concat(dvVatTuUpdateIds).Select(d => d.GetValueOrDefault());
                var dvVatTuDeletes = noiGioiThieuHopDongChiTietHeSoVatTus.Where(d => !dvVatTuThoaManDKIds.Contains(d.VatTuBenhVienId)).Select(d => d.Id).ToList();

                var dataItemVatTuDeletes = dvVatTuDeletes;
                var deleteVatTus = GetByNoiGioiThieuHopDongChiTietHeSoVatTuIdAsync(dataItemVatTuDeletes);
                foreach (var item in deleteVatTus)
                {
                    item.WillDelete = true;
                    dsVatTus.Add(item);
                }

                #endregion//----------------------------END vật tư --------------------------------//




                if (dsDuocPhams.Count() > 0)
                {
                    if (dsDuocPhams.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = dsDuocPhams.Where(d => d.Id == 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.AddRange(dv);
                    }
                    if (dsDuocPhams.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = dsDuocPhams.Where(d => d.Id != 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoDuocPhamRepository.UpdateAsync(dv);
                    }

                }
                if (dsVatTus.Count() > 0)
                {
                    if (dsVatTus.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = dsVatTus.Where(d => d.Id == 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoVatTuRepository.AddRange(dv);
                    }
                    if (dsVatTus.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = dsVatTus.Where(d => d.Id != 0).ToList();
                        _noiGioiThieuHopDongChiTietHeSoVatTuRepository.UpdateAsync(dv);
                    }
                }
            }
        }
        #endregion



        #region Import cấu hình Hoa hồng DV

        public async Task<List<ImportHoaHongDichVu>> ImportHoaHongDichVus(Stream path, long noiGioiThieuHopDongId, long noiGioiThieuId)
        {
            var lstError = new List<ImportHoaHongDichVu>();


            using (ExcelPackage package = new ExcelPackage(path))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["DS Cấu Hình Hoa Hồng"];
                if (workSheet == null)
                {
                    throw new Exception("Thông tin file nhập không đúng");
                }
                int totalRows = workSheet.Dimension.Rows;// dòng có data

                var infoDichVuKhams = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Select(d => new {
                    Ma = d.Ma,
                    TenDV = d.Ten,
                    DichVuKhamBenhBenhVienId = d.Id
                }).ToList();

                var infoNhomGiaDichVuKhams = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ten = d.Ten,
                       NhomGiaDichVuKhamBenhBenhVienId = d.Id
                   }).ToList();

                var infoDichVuKyThuats = _dichVuKyThuatBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ma = d.Ma,
                       TenDV = d.Ten,
                       DichVuKyThuatBenhVienId = d.Id
                   }).ToList();

                var infoNhomGiaDichVuKyThuats = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ten = d.Ten,
                       NhomGiaDichVuKyThuatBenhVienId = d.Id
                   }).ToList();

                var infoDichVuGiuongs = _dichVuGiuongBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ma = d.Ma,
                       TenDV = d.Ten,
                       DichVuGiuongBenhVienId = d.Id
                   }).ToList();

                var infoNhomGiaDichVuGiuongs = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking
                   .Select(d => new {
                       Ten = d.Ten,
                       NhomGiaDichVuGiuongBenhVienId = d.Id
                   }).ToList();



                if (totalRows >= 3)// dòng 5 bắt đầu có data
                {
                    var danhSachs = new List<ImportHoaHongDichVu>();
                    var danhSachThoaManDieuKiens = new List<ImportHoaHongDichVu>();
                    for (int i = 5; i <= totalRows + 2; i++)
                    {
                        // ...Cells[i, 4] => i = Row, 4 = Column
                        var importHoaHongDichVu = new ImportHoaHongDichVu();
                        importHoaHongDichVu = GanThongTinHoaHongDichVu(importHoaHongDichVu, workSheet, i);

                        if (importHoaHongDichVu.LaDichVu == "1")
                        {
                            if (string.IsNullOrEmpty(importHoaHongDichVu.Ma) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.Ten) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.NhomGia) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.DonGia) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.LoaiHoaHong) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong) ||
                              (!string.IsNullOrEmpty(importHoaHongDichVu.Ma) &&
                               !string.IsNullOrEmpty(importHoaHongDichVu.Ten)) ||
                              !string.IsNullOrEmpty(importHoaHongDichVu.NhomGia) ||
                               (!string.IsNullOrEmpty(importHoaHongDichVu.DonGia) && IsKieuSo(importHoaHongDichVu.DonGia) == false) ||
                               (!string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong) &&
                               IsKieuSo(importHoaHongDichVu.DonGiaHoaHong) == false)
                              )

                            {
                                var value = false;
                                if (string.IsNullOrEmpty(importHoaHongDichVu.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Mã dịch vụ chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDichVu.Ten))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Tên dịch vụ chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDichVu.NhomGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Nhóm giá dịch vụ chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (!string.IsNullOrEmpty(importHoaHongDichVu.Ma) && 
                                    !string.IsNullOrEmpty(importHoaHongDichVu.Ten) ||
                                    !string.IsNullOrEmpty(importHoaHongDichVu.NhomGia))
                                {
                                    var dv = infoDichVuKhams.Where(d => d.TenDV.ToLower() == importHoaHongDichVu.Ten.ToLower()
                                                                        && d.Ma.ToLower() == importHoaHongDichVu.Ma.ToLower()).Select(d => d.DichVuKhamBenhBenhVienId).FirstOrDefault();

                                    var giaDV = infoNhomGiaDichVuKhams.Where(d => d.Ten.ToLower() == importHoaHongDichVu.NhomGia.ToLower()).Select(d => d.NhomGiaDichVuKhamBenhBenhVienId).FirstOrDefault();

                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importHoaHongDichVu.IsError = true;

                                        errorMess = "Mã và tên dịch vụ khám không tồn tại";

                                        var error = new ImportHoaHongDichVu();
                                        error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                        error.TenDV = error.Ten;
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importHoaHongDichVu);
                                    }
                                     if(giaDV == null || giaDV == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importHoaHongDichVu.IsError = true;

                                        errorMess = "Nhóm giá dịch vụ khám không tồn tại.";

                                        var error = new ImportHoaHongDichVu();
                                        error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importHoaHongDichVu);
                                    }
                                   
                                }



                                if (!string.IsNullOrEmpty(importHoaHongDichVu.DonGia) && IsKieuSo(importHoaHongDichVu.DonGia) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá nhập không đúng";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (!string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong) &&
                                     IsKieuSo(importHoaHongDichVu.DonGiaHoaHong) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá hoa hồng nhập không đúng";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }




                                if (string.IsNullOrEmpty(importHoaHongDichVu.DonGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDichVu.LoaiHoaHong))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Loại hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (string.IsNullOrEmpty(importHoaHongDichVu.ApDungHoaHongTuLan))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Áp dụng hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if(value == false)
                                {
                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    danhSachThoaManDieuKiens.Add(importHoaHongDichVu);
                                }
                            }
                            else
                            {
                                var error = new ImportHoaHongDichVu();
                                error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                error.TenDV = error.Ten;
                                danhSachThoaManDieuKiens.Add(importHoaHongDichVu);
                            }



                        }

                        if (importHoaHongDichVu.LaDichVu == "2")
                        {
                            if (string.IsNullOrEmpty(importHoaHongDichVu.Ma) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.Ten) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.NhomGia) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.DonGia) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.LoaiHoaHong) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong) ||
                             
                              (!string.IsNullOrEmpty(importHoaHongDichVu.Ma) &&
                               !string.IsNullOrEmpty(importHoaHongDichVu.Ten)) ||
                              !string.IsNullOrEmpty(importHoaHongDichVu.NhomGia) ||
                              (!string.IsNullOrEmpty(importHoaHongDichVu.DonGia) && IsKieuSo(importHoaHongDichVu.DonGia) == false) ||
                               (!string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong) &&
                               IsKieuSo(importHoaHongDichVu.DonGiaHoaHong) == false) 
                              )

                            {
                                var value = false;
                                if (string.IsNullOrEmpty(importHoaHongDichVu.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Mã dịch vụ chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDichVu.Ten))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Tên dịch vụ chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDichVu.NhomGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Nhóm giá dịch vụ chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (!string.IsNullOrEmpty(importHoaHongDichVu.Ma) &&
                                    !string.IsNullOrEmpty(importHoaHongDichVu.Ten) ||
                                    !string.IsNullOrEmpty(importHoaHongDichVu.NhomGia))
                                {
                                    var dv = infoDichVuKyThuats.Where(d => d.TenDV.ToLower() == importHoaHongDichVu.Ten.ToLower()
                                                                        && d.Ma.ToLower() == importHoaHongDichVu.Ma.ToLower()).Select(d => d.DichVuKyThuatBenhVienId).FirstOrDefault();

                                    var giaDV = infoNhomGiaDichVuKyThuats.Where(d => d.Ten.ToLower() == importHoaHongDichVu.NhomGia.ToLower()).Select(d => d.NhomGiaDichVuKyThuatBenhVienId).FirstOrDefault();

                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importHoaHongDichVu.IsError = true;

                                        errorMess = "Mã và tên dịch vụ kỹ thuật không tồn tại";

                                        var error = new ImportHoaHongDichVu();
                                        error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                        error.TenDV = error.Ten;
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importHoaHongDichVu);
                                    }
                                    if (giaDV == null || giaDV == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importHoaHongDichVu.IsError = true;

                                        errorMess = "Nhóm giá dịch vụ kỹ thuật không tồn tại.";

                                        var error = new ImportHoaHongDichVu();
                                        error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                        error.TenDV = error.Ten;
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importHoaHongDichVu);
                                    }
                                }




                                if (string.IsNullOrEmpty(importHoaHongDichVu.DonGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if(!string.IsNullOrEmpty(importHoaHongDichVu.DonGia) && IsKieuSo(importHoaHongDichVu.DonGia) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá nhập không đúng";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (!string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong) &&
                                     IsKieuSo(importHoaHongDichVu.DonGiaHoaHong) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá hoa hồng nhập không đúng";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }




                                if (string.IsNullOrEmpty(importHoaHongDichVu.LoaiHoaHong))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Loại hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (string.IsNullOrEmpty(importHoaHongDichVu.ApDungHoaHongTuLan))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Áp dụng hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }


                                if(value == false)
                                {
                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    danhSachThoaManDieuKiens.Add(importHoaHongDichVu);
                                }
                            }
                            else
                            {
                                var error = new ImportHoaHongDichVu();
                                error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                error.TenDV = error.Ten;
                                danhSachThoaManDieuKiens.Add(importHoaHongDichVu);
                            }



                        }

                        if(importHoaHongDichVu.LaDichVu == "3")
                        {
                            if (string.IsNullOrEmpty(importHoaHongDichVu.Ma) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.Ten) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.NhomGia) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.DonGia) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.LoaiHoaHong) ||
                              string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong) ||
                              (!string.IsNullOrEmpty(importHoaHongDichVu.Ma) &&
                               !string.IsNullOrEmpty(importHoaHongDichVu.Ten)) ||
                              !string.IsNullOrEmpty(importHoaHongDichVu.NhomGia) ||
                               (!string.IsNullOrEmpty(importHoaHongDichVu.DonGia) && IsKieuSo(importHoaHongDichVu.DonGia) == false) ||
                               (!string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong) &&
                               IsKieuSo(importHoaHongDichVu.DonGiaHoaHong) == false)

                              )

                            {
                                var value = false;
                                if (string.IsNullOrEmpty(importHoaHongDichVu.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Mã dịch vụ chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDichVu.Ten))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Tên dịch vụ chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDichVu.NhomGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Nhóm giá dịch vụ chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (!string.IsNullOrEmpty(importHoaHongDichVu.Ma) &&
                                    !string.IsNullOrEmpty(importHoaHongDichVu.Ten) ||
                                    !string.IsNullOrEmpty(importHoaHongDichVu.NhomGia))
                                {
                                    var dv = infoDichVuGiuongs.Where(d => d.TenDV.ToLower() == importHoaHongDichVu.Ten.ToLower()
                                                                        && d.Ma.ToLower() == importHoaHongDichVu.Ma.ToLower()).Select(d => d.DichVuGiuongBenhVienId).FirstOrDefault();

                                    var giaDV = infoNhomGiaDichVuGiuongs.Where(d => d.Ten.ToLower() == importHoaHongDichVu.NhomGia.ToLower()).Select(d => d.NhomGiaDichVuGiuongBenhVienId).FirstOrDefault();

                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importHoaHongDichVu.IsError = true;

                                        errorMess = "Mã và tên dịch vụ giường không tồn tại";

                                        var error = new ImportHoaHongDichVu();
                                        error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                        error.TenDV = error.Ten;
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importHoaHongDichVu);
                                    }
                                     if (giaDV == null || giaDV ==0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importHoaHongDichVu.IsError = true;

                                        errorMess = "Nhóm giá dịch vụ giường không tồn tại.";

                                        var error = new ImportHoaHongDichVu();
                                        error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                        error.TenDV = error.Ten;
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importHoaHongDichVu);
                                    }
                                }




                                if (string.IsNullOrEmpty(importHoaHongDichVu.DonGia))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDichVu.LoaiHoaHong))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Loại hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (string.IsNullOrEmpty(importHoaHongDichVu.ApDungHoaHongTuLan))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Áp dụng hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if (!string.IsNullOrEmpty(importHoaHongDichVu.DonGia) && IsKieuSo(importHoaHongDichVu.DonGia) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá nhập không đúng";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }
                                if (!string.IsNullOrEmpty(importHoaHongDichVu.DonGiaHoaHong) &&
                                     IsKieuSo(importHoaHongDichVu.DonGiaHoaHong) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDichVu.IsError = true;

                                    errorMess = "Đơn giá hoa hồng nhập không đúng";

                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDichVu);
                                }

                                if(value == false)
                                {
                                    var error = new ImportHoaHongDichVu();
                                    error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    danhSachThoaManDieuKiens.Add(importHoaHongDichVu);
                                }
                            }
                            else
                            {
                                var error = new ImportHoaHongDichVu();
                                error = GanThongTinHoaHongDichVu(error, workSheet, i);
                                error.TenDV = error.Ten;
                                danhSachThoaManDieuKiens.Add(importHoaHongDichVu);
                            }



                        }

                    }

                    if (danhSachThoaManDieuKiens.Count() > 0 && lstError.Count() == 0)
                    {
                        XuLyTaoHoaHongDichVu(noiGioiThieuHopDongId, noiGioiThieuId, danhSachThoaManDieuKiens);
                    }
                }
            }

            return lstError;
        }
        private async Task XuLyTaoHoaHongDichVu(long noiGioiThieuHopDongId, long noiGioiThieuId, List<ImportHoaHongDichVu> models)
        {
            CauHinhChiTietHoaHong vo = new CauHinhChiTietHoaHong();
            vo.NoiGioiThieuHopDongId = noiGioiThieuHopDongId;
            vo.ChonLoaiDichVuId = 0;
            vo.NoiGioiThieuId = noiGioiThieuId;

            var infoDichVuKhams = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Select(d => new {
                    Ma = d.Ma,
                    TenDV = d.Ten,
                    DichVuKhamBenhBenhVienId = d.Id
                }).ToList();

            var infoNhomGiaDichVuKhams = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ten = d.Ten,
                   NhomGiaDichVuKhamBenhBenhVienId = d.Id
               }).ToList();

            var infoDichVuKyThuats = _dichVuKyThuatBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ma = d.Ma,
                   TenDV = d.Ten,
                   DichVuKyThuatBenhVienId = d.Id
               }).ToList();

            var infoNhomGiaDichVuKyThuats = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ten = d.Ten,
                   NhomGiaDichVuKyThuatBenhVienId = d.Id
               }).ToList();

            var infoDichVuGiuongs = _dichVuGiuongBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ma = d.Ma,
                   TenDV = d.Ten,
                   DichVuGiuongBenhVienId = d.Id
               }).ToList();

            var infoNhomGiaDichVuGiuongs = _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking
               .Select(d => new {
                   Ten = d.Ten,
                   NhomGiaDichVuGiuongBenhVienId = d.Id
               }).ToList();


            foreach (var item in models)
            {
                if (item.LaDichVu == "1")
                {

                    var obj = new ThongTinCauHinhHoaHongGridVo();


                    obj.DichVuKhamBenhBenhVienId = infoDichVuKhams.Where(d => d.TenDV.ToLower() == item.Ten.ToLower()
                                                                        && d.Ma.ToLower() == item.Ma.ToLower()).Select(d => d.DichVuKhamBenhBenhVienId).FirstOrDefault();


                    obj.NhomGiaDichVuKhamBenhBenhVienId = infoNhomGiaDichVuKhams.Where(d => d.Ten.ToLower().Trim() == item.NhomGia.ToLower().Trim()).Select(d => d.NhomGiaDichVuKhamBenhBenhVienId).FirstOrDefault();


                  

                    if(!string.IsNullOrEmpty(item.LoaiHoaHong) )
                    {
                        if(item.LoaiHoaHong.ToLower().Trim() == "Số tiền".ToLower().Trim() )
                        {
                            obj.ChonTienHayHoaHong = LoaiHoaHong.SoTien;
                            obj.DonGiaHoaHongHoacTien = Convert.ToDecimal(item.DonGiaHoaHong);
                        }
                        if (item.LoaiHoaHong.ToLower().Trim() == "Tỉ lệ".ToLower().Trim())
                        {
                            obj.ChonTienHayHoaHong = LoaiHoaHong.TiLe;
                            obj.DonGiaHoaHongHoacTien = Convert.ToDecimal(item.DonGiaHoaHong);
                        }

                    }

                   

                    obj.ADDHHTuLan = Convert.ToInt32(item.ApDungHoaHongTuLan);

                    if(!string.IsNullOrEmpty(item.ApDungHoaHongDenLan))
                    {
                        obj.ADDHHDenLan = Convert.ToInt32(item.ApDungHoaHongDenLan);
                    }

                    obj.DonGia = Convert.ToDecimal(item.DonGia);

                    obj.LaDichVuKham = true;

                    vo.ThongTinCauHinhHoaHongs.Add(obj);
                }

                if (item.LaDichVu == "2")
                {

                    var obj = new ThongTinCauHinhHoaHongGridVo();


                    obj.DichVuKyThuatBenhVienId = infoDichVuKyThuats.Where(d => d.TenDV.ToLower() == item.Ten.ToLower()
                                                                        && d.Ma.ToLower() == item.Ma.ToLower()).Select(d => d.DichVuKyThuatBenhVienId).FirstOrDefault();


                    obj.NhomGiaDichVuKyThuatBenhVienId = infoNhomGiaDichVuKyThuats.Where(d => d.Ten.ToLower() == item.NhomGia.ToLower()).Select(d => d.NhomGiaDichVuKyThuatBenhVienId).FirstOrDefault();


                    
                    obj.DonGia = Convert.ToDecimal(item.DonGia);


                    obj.LaDichVuKyThuat = true;

                    if (!string.IsNullOrEmpty(item.LoaiHoaHong))
                    {
                        if (item.LoaiHoaHong.ToLower().Trim() == "Số tiền".ToLower().Trim())
                        {
                            obj.ChonTienHayHoaHong = LoaiHoaHong.SoTien;
                            obj.DonGiaHoaHongHoacTien = Convert.ToDecimal(item.DonGiaHoaHong);
                        }
                        if (item.LoaiHoaHong.ToLower().Trim() == "Tỉ lệ".ToLower().Trim())
                        {
                            obj.ChonTienHayHoaHong = LoaiHoaHong.TiLe;
                            obj.DonGiaHoaHongHoacTien = Convert.ToDecimal(item.DonGiaHoaHong);
                        }

                    }



                    obj.ADDHHTuLan = Convert.ToInt32(item.ApDungHoaHongTuLan);

                    if (!string.IsNullOrEmpty(item.ApDungHoaHongDenLan))
                    {
                        obj.ADDHHDenLan = Convert.ToInt32(item.ApDungHoaHongDenLan);
                    }


                    vo.ThongTinCauHinhHoaHongs.Add(obj);
                }

                if (item.LaDichVu == "3")
                {

                    var obj = new ThongTinCauHinhHoaHongGridVo();


                    obj.DichVuGiuongBenhVienId = infoDichVuGiuongs.Where(d => d.TenDV.ToLower() == item.Ten.ToLower()
                                                                        && d.Ma.ToLower() == item.Ma.ToLower()).Select(d => d.DichVuGiuongBenhVienId).FirstOrDefault();


                    obj.NhomGiaDichVuGiuongBenhVienId = infoNhomGiaDichVuGiuongs.Where(d => d.Ten.ToLower() == item.NhomGia.ToLower()).Select(d => d.NhomGiaDichVuGiuongBenhVienId).FirstOrDefault();


                   
                    obj.DonGia = Convert.ToDecimal(item.DonGia);


                    obj.LaDichVuGiuong = true;

                    if (!string.IsNullOrEmpty(item.LoaiHoaHong))
                    {
                        if (item.LoaiHoaHong.ToLower().Trim() == "Số tiền".ToLower().Trim())
                        {
                            obj.ChonTienHayHoaHong = LoaiHoaHong.SoTien;
                            obj.DonGiaHoaHongHoacTien = Convert.ToDecimal(item.DonGiaHoaHong);
                        }
                        if (item.LoaiHoaHong.ToLower().Trim() == "Tỉ lệ".ToLower().Trim())
                        {
                            obj.ChonTienHayHoaHong = LoaiHoaHong.TiLe;
                            obj.DonGiaHoaHongHoacTien = Convert.ToDecimal(item.DonGiaHoaHong);
                        }

                    }



                    obj.ADDHHTuLan = Convert.ToInt32(item.ApDungHoaHongTuLan);

                    if (!string.IsNullOrEmpty(item.ApDungHoaHongDenLan))
                    {
                        obj.ADDHHDenLan = Convert.ToInt32(item.ApDungHoaHongDenLan);
                    }

                    vo.ThongTinCauHinhHoaHongs.Add(obj);
                }
            }

            XuLyCapNhatImportCauHinhHoaHongAsync(vo);
        }

        private ImportHoaHongDichVu GanThongTinHoaHongDichVu(ImportHoaHongDichVu model, ExcelWorksheet workSheet, int i)
        {
            model.LaDichVu = workSheet.Cells[i, 2].Text;


            if (model.LaDichVu == "1")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :NhomGiaDichVu
                // 6 :DonGia
                // 7 :LoaiHoaHong
                // 8 :DonGiaHoaHong
                // 9: ApDungHoaHongTuLan
                // 10: ApDungHoaHongTuLan
                // 11: Ghi chu
                model.Ma = workSheet.Cells[i, 3].Text;
                model.Ten = workSheet.Cells[i, 4].Text;
                model.NhomGia = workSheet.Cells[i, 5].Text;

                model.DonGia = workSheet.Cells[i, 6].Text;

                model.LoaiHoaHong = workSheet.Cells[i, 7].Text;

                model.DonGiaHoaHong = workSheet.Cells[i, 8].Text;

                model.ApDungHoaHongTuLan = workSheet.Cells[i, 9].Text;

                model.ApDungHoaHongDenLan = workSheet.Cells[i, 10].Text;

                model.GhiChu = workSheet.Cells[i, 13].Text;
            }

            if (model.LaDichVu == "2")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :NhomGiaDichVu
                // 6 :DonGia
                // 7 :LoaiHoaHong
                // 8 :DonGiaHoaHong
                // 9: ApDungHoaHongTuLan
                // 10: ApDungHoaHongTuLan
                // 11: Ghi chu
                model.Ma = workSheet.Cells[i, 3].Text;
                model.Ten = workSheet.Cells[i, 4].Text;
                model.NhomGia = workSheet.Cells[i, 5].Text;

                model.DonGia = workSheet.Cells[i, 6].Text;

                model.LoaiHoaHong = workSheet.Cells[i, 7].Text;

                model.DonGiaHoaHong = workSheet.Cells[i, 8].Text;

                model.ApDungHoaHongTuLan = workSheet.Cells[i, 9].Text;

                model.ApDungHoaHongDenLan = workSheet.Cells[i, 10].Text;

                model.GhiChu = workSheet.Cells[i, 13].Text;
            }
            if (model.LaDichVu == "3")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :NhomGiaDichVu
                // 6 :DonGia
                // 7 :LoaiHoaHong
                // 8 :DonGiaHoaHong
                // 9: ApDungHoaHongTuLan
                // 10: ApDungHoaHongTuLan
                // 11: Ghi chu
                model.Ma = workSheet.Cells[i, 3].Text;
                model.Ten = workSheet.Cells[i, 4].Text;
                model.NhomGia = workSheet.Cells[i, 5].Text;

                model.DonGia = workSheet.Cells[i, 6].Text;

                model.LoaiHoaHong = workSheet.Cells[i, 7].Text;

                model.DonGiaHoaHong = workSheet.Cells[i, 8].Text;

                model.ApDungHoaHongTuLan = workSheet.Cells[i, 9].Text;

                model.ApDungHoaHongDenLan = workSheet.Cells[i, 10].Text;

                model.GhiChu = workSheet.Cells[i, 13].Text;
            }
            return model;
        }

        private async Task XuLyCapNhatImportCauHinhHoaHongAsync(CauHinhChiTietHoaHong vo)
        {
            // nếu dịch vụ khám đã dc tạo rồi . thì cập nhật thông tin dịch vụ trong file excel theo file 
            // nếu dịch vụ khám chưa tạo thì tạo mới
            // nếu dịch vụ khám trong file không chứa dịch vụ khám dc tạo trước đó . xóa dịch vụ khám đó đi


            if (vo.ThongTinCauHinhHoaHongs != null && vo.ThongTinCauHinhHoaHongs.Count() > 0)
            {
                #region//----------------------------Dịch vụ khám ------------------------------------//
                var noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs = _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new
                    {
                        Id = d.Id,
                        DichVuKhamBenhBenhVienId = d.DichVuKhamBenhBenhVienId
                    }).ToList();

                var dichVuKhamIds = noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs.Select(d => d.DichVuKhamBenhBenhVienId).ToList();

                var dvKhamUpdates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuKham == true && dichVuKhamIds.Contains(d.DichVuKhamBenhBenhVienId.GetValueOrDefault())).ToList();
                var dvKhams = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh>();
                if (dvKhamUpdates.Count() > 0) // update
                {
                    var listDichVuIds = noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhIdAsync(listDichVuIds);
                    // xử lý list Dv 
                    foreach (var itemDvKhamdb in dataItems)
                    {
                        foreach (var itemUP in dvKhamUpdates)
                        {
                            if (itemUP.DichVuKhamBenhBenhVienId == itemDvKhamdb.DichVuKhamBenhBenhVienId)
                            {
                                itemDvKhamdb.DichVuKhamBenhBenhVienId = itemUP.DichVuKhamBenhBenhVienId.GetValueOrDefault();
                                itemDvKhamdb.DonGiaBenhVien = itemUP.DonGia.GetValueOrDefault();
                                itemDvKhamdb.ApDungTuLan = itemUP.ADDHHTuLan.GetValueOrDefault();
                                if(itemUP.ADDHHDenLan != null)
                                {
                                    itemDvKhamdb.ApDungDenLan = itemUP.ADDHHDenLan.GetValueOrDefault();
                                }
                                itemDvKhamdb.LoaiHoaHong = itemUP.ChonTienHayHoaHong;


                                itemDvKhamdb.SoTienHoaHong = itemUP.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemUP.DonGiaHoaHongHoacTien : null;

                                itemDvKhamdb.TiLeHoaHong = itemUP.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemUP.DonGiaHoaHongHoacTien : null;

                                itemDvKhamdb.NhomGiaDichVuKhamBenhBenhVienId = itemUP.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault();

                                itemDvKhamdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                dvKhams.Add(itemDvKhamdb);
                            }


                        }

                    }
                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.UpdateAsync(dvKhams);
                }




                var dvKhamCreates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuKham == true && !dichVuKhamIds.Contains(d.DichVuKhamBenhBenhVienId.GetValueOrDefault())).ToList();

                if (dvKhamCreates.Any())
                {
                    // xử lý list Dv khám cần lưu
                    foreach (var itemDvKham in dvKhamCreates)
                    {
                        var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh()
                        {
                            DichVuKhamBenhBenhVienId = itemDvKham.DichVuKhamBenhBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDvKham.DonGia.GetValueOrDefault(),
                            NhomGiaDichVuKhamBenhBenhVienId = itemDvKham.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId,
                        };
                        viewModelDvKhamBenh.ApDungTuLan = itemDvKham.ADDHHTuLan.GetValueOrDefault();
                        if (itemDvKham.ADDHHDenLan != null)
                        {
                            viewModelDvKhamBenh.ApDungDenLan = itemDvKham.ADDHHDenLan.GetValueOrDefault();
                        }
                        viewModelDvKhamBenh.LoaiHoaHong = itemDvKham.ChonTienHayHoaHong;


                        viewModelDvKhamBenh.SoTienHoaHong = itemDvKham.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemDvKham.DonGiaHoaHongHoacTien : null;

                        viewModelDvKhamBenh.TiLeHoaHong = itemDvKham.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemDvKham.DonGiaHoaHongHoacTien : null;
                        
                        dvKhams.Add(viewModelDvKhamBenh);
                    }

                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                }



                var dvKhamCreateIds = dvKhamCreates.Where(d => d.DichVuKhamBenhBenhVienId != null).Select(d => d.DichVuKhamBenhBenhVienId).ToList();
                var dvKhamUpdateIds = dvKhamUpdates.Where(d => d.DichVuKhamBenhBenhVienId != null).Select(d => d.DichVuKhamBenhBenhVienId).ToList();

                var dvKhamThoaManDKIds = dvKhamCreateIds.Concat(dvKhamUpdateIds).Select(d => d.GetValueOrDefault());
                var dvKhamDeletes = noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs.Where(d => !dvKhamThoaManDKIds.Contains(d.DichVuKhamBenhBenhVienId)).Select(d => d.Id).ToList();

                var dataItemDeletes = dvKhamDeletes;

                var deleteDVKs = GetByNoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhIdAsync(dataItemDeletes);

                foreach (var item in deleteDVKs)
                {
                    item.WillDelete = true;
                    dvKhams.Add(item);
                }

                #endregion//----------------------------END Dịch vụ khám --------------------------------//



                #region //----------------------------Dịch vụ kỹ thuật ------------------------------------//
                var noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats = _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new
                    {
                        Id = d.Id,
                        DichVuKyThuatBenhVienId = d.DichVuKyThuatBenhVienId
                    }).ToList();

                var dichVuKyThuatIds = noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats.Select(d => d.DichVuKyThuatBenhVienId).ToList();

                var dvKyThuatUpdates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuKyThuat == true && dichVuKyThuatIds.Contains(d.DichVuKyThuatBenhVienId.GetValueOrDefault())).ToList();
                var dvKyThuats = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat>();
                if (dvKyThuatUpdates.Count() > 0) // update
                {
                    var listDichVuIds = noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuatIdAsync(listDichVuIds);
                    // xử lý list Dv 
                    foreach (var itemDvdb in dataItems)
                    {
                        foreach (var itemUP in dvKyThuatUpdates)
                        {
                            if (itemUP.DichVuKyThuatBenhVienId == itemDvdb.DichVuKyThuatBenhVienId)
                            {
                                itemDvdb.DichVuKyThuatBenhVienId = itemUP.DichVuKyThuatBenhVienId.GetValueOrDefault();
                                itemDvdb.DonGiaBenhVien = itemUP.DonGia.GetValueOrDefault();

                                itemDvdb.ApDungTuLan = itemUP.ADDHHTuLan.GetValueOrDefault();
                                if (itemUP.ADDHHDenLan != null)
                                {
                                    itemDvdb.ApDungDenLan = itemUP.ADDHHDenLan.GetValueOrDefault();
                                }
                                itemDvdb.LoaiHoaHong = itemUP.ChonTienHayHoaHong;


                                itemDvdb.SoTienHoaHong = itemUP.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemUP.DonGiaHoaHongHoacTien : null;

                                itemDvdb.TiLeHoaHong = itemUP.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemUP.DonGiaHoaHongHoacTien : null;

                                itemDvdb.NhomGiaDichVuKyThuatBenhVienId = itemUP.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault();

                                itemDvdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                dvKyThuats.Add(itemDvdb);
                            }


                        }

                    }
                }




                var dvKyThuatCreates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuKyThuat == true && !dichVuKyThuatIds.Contains(d.DichVuKyThuatBenhVienId.GetValueOrDefault())).ToList();

                if (dvKyThuatCreates.Any())
                {
                    foreach (var itemDv in dvKyThuatCreates)
                    {
                        var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat()
                        {
                            DichVuKyThuatBenhVienId = itemDv.DichVuKyThuatBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault(),
                            NhomGiaDichVuKyThuatBenhVienId = itemDv.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId,
                        };

                        viewModelDvKhamBenh.ApDungTuLan = itemDv.ADDHHTuLan.GetValueOrDefault();
                        if (itemDv.ADDHHDenLan != null)
                        {
                            viewModelDvKhamBenh.ApDungDenLan = itemDv.ADDHHDenLan.GetValueOrDefault();
                        }
                        viewModelDvKhamBenh.LoaiHoaHong = itemDv.ChonTienHayHoaHong;


                        viewModelDvKhamBenh.SoTienHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemDv.DonGiaHoaHongHoacTien : null;

                        viewModelDvKhamBenh.TiLeHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemDv.DonGiaHoaHongHoacTien : null;

                        dvKyThuats.Add(viewModelDvKhamBenh);
                    }

                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                }



                var dvKyThuatCreateIds = dvKyThuatCreates.Where(d => d.DichVuKyThuatBenhVienId != null).Select(d => d.DichVuKyThuatBenhVienId).ToList();
                var dvKyThuatUpdateIds = dvKyThuatUpdates.Where(d => d.DichVuKyThuatBenhVienId != null).Select(d => d.DichVuKyThuatBenhVienId).ToList();

                var dvKyThuatThoaManDKIds = dvKyThuatCreateIds.Concat(dvKyThuatUpdateIds).Select(d => d.GetValueOrDefault());
                var dvKyThuatDeletes = noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats.Where(d => !dvKyThuatThoaManDKIds.Contains(d.DichVuKyThuatBenhVienId)).Select(d => d.Id).ToList();

                var dataItemKyThuatDeletes = dvKyThuatDeletes;
                var deletes = GetByNoiGioiThieuHopDongChiTietHoaHongDichVuKyThuatIdAsync(dataItemKyThuatDeletes);
               
                foreach (var item in deletes)
                {
                    item.WillDelete = true;
                    dvKyThuats.Add(item);
                }

                #endregion//----------------------------END Dịch vụ kỹ thuật --------------------------------//



                #region //----------------------------Dịch vụ giường ------------------------------------//
                var noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs = _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new {
                        Id = d.Id,
                        DichVuGiuongBenhVienId = d.DichVuGiuongBenhVienId
                    }).ToList();

                var dichVuGiuongBenhIds = noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs.Select(d => d.DichVuGiuongBenhVienId).ToList();

                var dvGiuongtUpdates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuGiuong == true && dichVuGiuongBenhIds.Contains(d.DichVuGiuongBenhVienId.GetValueOrDefault())).ToList();
                var dvGiuongs = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong>();
                if (dvGiuongtUpdates.Count() > 0) // update
                {
                    var listDichVuIds = noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongDichVuGiuongIdAsync(listDichVuIds);
                    // xử lý list Dv 
                    foreach (var itemDvdb in dataItems)
                    {
                        foreach (var itemUP in dvGiuongtUpdates)
                        {
                            if (itemUP.DichVuGiuongBenhVienId == itemDvdb.DichVuGiuongBenhVienId)
                            {
                                itemDvdb.DichVuGiuongBenhVienId = itemUP.DichVuGiuongBenhVienId.GetValueOrDefault();
                                itemDvdb.DonGiaBenhVien = itemUP.DonGia.GetValueOrDefault();
                                

                                itemDvdb.NhomGiaDichVuGiuongBenhVienId = itemUP.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault();

                                itemDvdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                dvGiuongs.Add(itemDvdb);
                            }


                        }

                    }
                }




                var dvGiuongCreates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDichVuGiuong == true && !dichVuGiuongBenhIds.Contains(d.DichVuGiuongBenhVienId.GetValueOrDefault())).ToList();

                if (dvGiuongCreates.Any())
                {
                    // xử lý list Dv khám cần lưu
                    foreach (var itemDv in dvGiuongCreates)
                    {
                        var viewModelDvKhamBenh = new NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong()
                        {
                            DichVuGiuongBenhVienId = itemDv.DichVuGiuongBenhVienId.GetValueOrDefault(),
                            DonGiaBenhVien = itemDv.DonGia.GetValueOrDefault(),

                            NhomGiaDichVuGiuongBenhVienId = itemDv.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId,
                        };

                        viewModelDvKhamBenh.ApDungTuLan = itemDv.ADDHHTuLan.GetValueOrDefault();
                        if (itemDv.ADDHHDenLan != null)
                        {
                            viewModelDvKhamBenh.ApDungDenLan = itemDv.ADDHHDenLan.GetValueOrDefault();
                        }
                        viewModelDvKhamBenh.LoaiHoaHong = itemDv.ChonTienHayHoaHong;


                        viewModelDvKhamBenh.SoTienHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? itemDv.DonGiaHoaHongHoacTien : null;

                        viewModelDvKhamBenh.TiLeHoaHong = itemDv.ChonTienHayHoaHong == LoaiHoaHong.TiLe ? itemDv.DonGiaHoaHongHoacTien : null;


                        dvGiuongs.Add(viewModelDvKhamBenh);
                    }

                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                }



                var dvGiuongCreateIds = dvGiuongCreates.Where(d => d.DichVuGiuongBenhVienId != null).Select(d => d.DichVuGiuongBenhVienId).ToList();
                var dvGiuongUpdateIds = dvGiuongtUpdates.Where(d => d.DichVuGiuongBenhVienId != null).Select(d => d.DichVuGiuongBenhVienId).ToList();

                var dvGiuongThoaManDKIds = dvGiuongCreateIds.Concat(dvGiuongUpdateIds).Select(d => d.GetValueOrDefault());
                var dvGiuongDeletes = noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs.Where(d => !dvGiuongThoaManDKIds.Contains(d.DichVuGiuongBenhVienId)).Select(d => d.Id).ToList();

                var dataItemGiuongDeletes = dvGiuongDeletes;
                var deleteDVGiuongs = GetByNoiGioiThieuHopDongChiTietHoaHongDichVuGiuongIdAsync(dataItemGiuongDeletes);
                foreach (var item in deleteDVGiuongs)
                {
                    item.WillDelete = true;
                    dvGiuongs.Add(item);
                }

                #endregion //----------------------------END Dịch vụ kỹ gường --------------------------------//
                if (dvKhams.Count() > 0)
                {
                    if (dvKhams.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = dvKhams.Where(d => d.Id == 0).ToList();
                        _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.AddRange(dv);
                    }
                    if (dvKhams.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = dvKhams.Where(d => d.Id != 0).ToList();
                        _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhRepository.UpdateAsync(dvKhams);
                    }

                }
                if (dvKyThuats.Count() > 0)
                {
                    if (dvKyThuats.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = dvKyThuats.Where(d => d.Id == 0).ToList();
                        //var soLanAdd = dv.Count() / 100;
                        //var chiaDu = dv.Count() % 100;
                        //if (chiaDu > 0)
                        //{
                        //    soLanAdd = soLanAdd + 1;
                        //}
                        //var skip = 0;
                        //for (int i = 1; i <= soLanAdd; i++)
                        //{
                        //    var dvAdd = dv.Skip(skip).Take(100).ToList();
                        //    _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.AddRange(dv);
                        //    skip = skip + 100;
                        //}
                        _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.AddRange(dv);
                    }
                    if (dvKyThuats.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = dvKyThuats.Where(d => d.Id != 0).ToList();
                         _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuatRepository.UpdateAsync(dv);
                    }
                   
                }
                if (dvGiuongs.Count() > 0)
                {
                    if (dvGiuongs.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = dvGiuongs.Where(d => d.Id == 0).ToList();
                        _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.AddRange(dv);
                    }
                    if (dvGiuongs.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = dvGiuongs.Where(d => d.Id != 0).ToList();
                        _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongRepository.UpdateAsync(dv);
                    }
                }




            }
        }

        #endregion










        #region Import cấu hình Hoa hồng DP/VTYT

        public async Task<List<ImportHoaHongDPVTYT>> ImportHoaHongDPVTYTs(Stream path, long noiGioiThieuHopDongId, long noiGioiThieuId)
        {
            var lstError = new List<ImportHoaHongDPVTYT>();


            using (ExcelPackage package = new ExcelPackage(path))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["DS Cấu Hình Hoa Hồng"];
                if (workSheet == null)
                {
                    throw new Exception("Thông tin file nhập không đúng");
                }
                int totalRows = workSheet.Dimension.Rows;// dòng có data

                var infoDuocPhams = _duocPhamBenhVienRepository.TableNoTracking.Include(d => d.DuocPham)
             .Select(d => new
             {
                 Ma = d.Ma,
                 Ten = d.DuocPham.Ten,
                 DuocPhamBenhVienId = d.Id
             }).ToList();


                var infoVatTus = _vatTuBenhVienRepository.TableNoTracking.Include(d => d.VatTus)
                   .Select(d => new
                   {
                       Ma = d.Ma,
                       Ten = d.VatTus.Ten,
                       VatTuBenhVienId = d.Id
                   }).ToList();


                var datas = Enum.GetValues(typeof(LoaiGiaNoiGioiThieuHopDong)).Cast<Enum>();
                var models = datas.Select(o => new LookupItemVo
                {
                    DisplayName = o.GetDescription(),
                    KeyId = Convert.ToInt32(o)
                });

                if (totalRows >= 3)// dòng 5 bắt đầu có data
                {
                    var danhSachs = new List<ImportHoaHongDPVTYT>();
                    var danhSachThoaManDieuKiens = new List<ImportHoaHongDPVTYT>();
                    for (int i = 5; i <= totalRows + 2; i++)
                    {
                        // ...Cells[i, 4] => i = Row, 4 = Column
                        var importHoaHongDPVTYT = new ImportHoaHongDPVTYT();
                        importHoaHongDPVTYT = GanThongTinHoaHongDPVTYT(importHoaHongDPVTYT, workSheet, i);

                        if (importHoaHongDPVTYT.LaDichVu == "1")
                        {
                            if (string.IsNullOrEmpty(importHoaHongDPVTYT.Ma) ||
                              string.IsNullOrEmpty(importHoaHongDPVTYT.Ten) ||
                           
                              string.IsNullOrEmpty(importHoaHongDPVTYT.DonGiaHoaHong) ||
                              (!string.IsNullOrEmpty(importHoaHongDPVTYT.Ma) &&
                               !string.IsNullOrEmpty(importHoaHongDPVTYT.Ten)) ||
                              (!string.IsNullOrEmpty(importHoaHongDPVTYT.DonGiaHoaHong) && IsKieuSo(importHoaHongDPVTYT.DonGiaHoaHong) == false)
                              )

                            {
                                var value = false;
                                if (string.IsNullOrEmpty(importHoaHongDPVTYT.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDPVTYT.IsError = true;

                                    errorMess = "Mã dược phẩm chưa nhập.";

                                    var error = new ImportHoaHongDPVTYT();
                                    error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDPVTYT);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDPVTYT.Ten))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDPVTYT.IsError = true;

                                    errorMess = "Tên dược phẩm chưa nhập.";

                                    var error = new ImportHoaHongDPVTYT();
                                    error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDPVTYT);
                                }

                                if (!string.IsNullOrEmpty(importHoaHongDPVTYT.Ma) &&
                                    !string.IsNullOrEmpty(importHoaHongDPVTYT.Ten))
                                {
                                    var dv = infoDuocPhams.Where(d => d.Ten.ToLower() == importHoaHongDPVTYT.Ten.ToLower()
                                                                        && d.Ma.ToLower() == importHoaHongDPVTYT.Ma.ToLower()).Select(d => d.DuocPhamBenhVienId).FirstOrDefault();

                                  

                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importHoaHongDPVTYT.IsError = true;

                                        errorMess = "Mã và tên dược phẩm không tồn tại";

                                        var error = new ImportHoaHongDPVTYT();
                                        error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                        error.TenDV = error.Ten;
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importHoaHongDPVTYT);
                                    }
                                }




                                if (string.IsNullOrEmpty(importHoaHongDPVTYT.DonGiaHoaHong))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDPVTYT.IsError = true;

                                    errorMess = "Đơn giá hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDPVTYT();
                                    error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDPVTYT);
                                }
                                if (!string.IsNullOrEmpty(importHoaHongDPVTYT.DonGiaHoaHong) && IsKieuSo(importHoaHongDPVTYT.DonGiaHoaHong) == false)
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDPVTYT.IsError = true;

                                    errorMess = "Đơn giá hoa hồng nhập không đúng";

                                    var error = new ImportHoaHongDPVTYT();
                                    error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDPVTYT);
                                }
                            
                                if(value == false)
                                {
                                    var error = new ImportHoaHongDPVTYT();
                                    error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                    danhSachThoaManDieuKiens.Add(importHoaHongDPVTYT);
                                }
                            }
                            else
                            {
                                var error = new ImportHoaHongDPVTYT();
                                error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                danhSachThoaManDieuKiens.Add(importHoaHongDPVTYT);
                            }



                        }

                        if (importHoaHongDPVTYT.LaDichVu == "2")
                        {
                           
                            if (string.IsNullOrEmpty(importHoaHongDPVTYT.Ma) ||
                              string.IsNullOrEmpty(importHoaHongDPVTYT.Ten) ||

                              string.IsNullOrEmpty(importHoaHongDPVTYT.DonGiaHoaHong) ||
                              (!string.IsNullOrEmpty(importHoaHongDPVTYT.Ma) &&
                               !string.IsNullOrEmpty(importHoaHongDPVTYT.Ten))
                              )

                            {
                                var value = false;
                                if (string.IsNullOrEmpty(importHoaHongDPVTYT.Ma))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDPVTYT.IsError = true;

                                    errorMess = "Mã vật tư chưa nhập.";

                                    var error = new ImportHoaHongDPVTYT();
                                    error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDPVTYT);
                                }
                                if (string.IsNullOrEmpty(importHoaHongDPVTYT.Ten))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDPVTYT.IsError = true;

                                    errorMess = "Tên vật tư chưa nhập.";

                                    var error = new ImportHoaHongDPVTYT();
                                    error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDPVTYT);
                                }

                                if (!string.IsNullOrEmpty(importHoaHongDPVTYT.Ma) &&
                                    !string.IsNullOrEmpty(importHoaHongDPVTYT.Ten))
                                {
                                    var dv = infoVatTus.Where(d => d.Ten.ToLower() == importHoaHongDPVTYT.Ten.ToLower()
                                                                        && d.Ma.ToLower() == importHoaHongDPVTYT.Ma.ToLower()).Select(d => d.VatTuBenhVienId).FirstOrDefault();



                                    if (dv == null || dv == 0)
                                    {
                                        value = true;
                                        var errorMess = "";
                                        importHoaHongDPVTYT.IsError = true;

                                        errorMess = "Mã và tên vật tư không tồn tại";

                                        var error = new ImportHoaHongDPVTYT();
                                        error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                        error.TenDV = error.Ten;
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        danhSachs.Add(importHoaHongDPVTYT);
                                    }
                                }




                                if (string.IsNullOrEmpty(importHoaHongDPVTYT.DonGiaHoaHong))
                                {
                                    value = true;
                                    var errorMess = "";
                                    importHoaHongDPVTYT.IsError = true;

                                    errorMess = "Đơn giá hoa hồng chưa nhập.";

                                    var error = new ImportHoaHongDPVTYT();
                                    error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                    error.TenDV = error.Ten;
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    danhSachs.Add(importHoaHongDPVTYT);
                                }

                                if(value == false)
                                {
                                    var error = new ImportHoaHongDPVTYT();
                                    error = GanThongTinHoaHongDPVTYT(error, workSheet, i);
                                    danhSachThoaManDieuKiens.Add(importHoaHongDPVTYT);
                                }
                            }
                            else
                            {
                               
                            }



                        }

                    }

                    if (danhSachThoaManDieuKiens.Count() > 0 && lstError.Count() == 0)
                    {
                        XuLyTaoHoaHongDPVTYT(noiGioiThieuHopDongId, noiGioiThieuId, danhSachThoaManDieuKiens);
                    }
                }
            }

            return lstError;
        }
        private async Task XuLyTaoHoaHongDPVTYT(long noiGioiThieuHopDongId, long noiGioiThieuId, List<ImportHoaHongDPVTYT> models)
        {
            CauHinhChiTietHoaHong vo = new CauHinhChiTietHoaHong();
            vo.NoiGioiThieuHopDongId = noiGioiThieuHopDongId;
            vo.ChonLoaiDichVuId = 0;
            vo.NoiGioiThieuId = noiGioiThieuId;

            var infoDuocPhams = _duocPhamBenhVienRepository.TableNoTracking.Include(d => d.DuocPham)
         .Select(d => new
         {
             Ma = d.Ma,
             Ten = d.DuocPham.Ten,
             DuocPhamBenhVienId = d.Id
         }).ToList();


            var infoVatTus = _vatTuBenhVienRepository.TableNoTracking.Include(d => d.VatTus)
               .Select(d => new
               {
                   Ma = d.Ma,
                   Ten = d.VatTus.Ten,
                   VatTuBenhVienId = d.Id
               }).ToList();


            foreach (var item in models)
            {
                if (item.LaDichVu == "1")
                {

                    var obj = new ThongTinCauHinhHoaHongGridVo();


                    obj.DuocPhamBenhVienId = infoDuocPhams.Where(d => d.Ten.ToLower() == item.Ten.ToLower()
                                                                        && d.Ma.ToLower() == item.Ma.ToLower()).Select(d => d.DuocPhamBenhVienId).FirstOrDefault();
                  

                    obj.DonGiaHoaHongHoacTien = Convert.ToDecimal(item.DonGiaHoaHong);

                    obj.LaDuocPham = true;

                    vo.ThongTinCauHinhHoaHongs.Add(obj);
                }
                if (item.LaDichVu == "2")
                {

                    var obj = new ThongTinCauHinhHoaHongGridVo();


                    obj.VatTuBenhVienId = infoVatTus.Where(d => d.Ten.ToLower() == item.Ten.ToLower()
                                                                        && d.Ma.ToLower() == item.Ma.ToLower()).Select(d => d.VatTuBenhVienId).FirstOrDefault();


                    obj.DonGiaHoaHongHoacTien = Convert.ToDecimal(item.DonGiaHoaHong);

                    obj.LaVatTu = true;

                    vo.ThongTinCauHinhHoaHongs.Add(obj);
                }


            }

            XuLyCapNhatImportCauHinhHoaHongDPVTYTAsync(vo);
        }

        private ImportHoaHongDPVTYT GanThongTinHoaHongDPVTYT(ImportHoaHongDPVTYT model, ExcelWorksheet workSheet, int i)
        {
            model.LaDichVu = workSheet.Cells[i, 2].Text;


            if (model.LaDichVu == "1")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :DonGiaHoaHong
                // 6: Ghi chu
                model.Ma = workSheet.Cells[i, 3].Text;
                model.Ten = workSheet.Cells[i, 4].Text;

                model.DonGiaHoaHong = workSheet.Cells[i, 5].Text;

                model.GhiChu = workSheet.Cells[i, 6].Text;
            }
            if (model.LaDichVu == "2")
            {
                // 3 : Ma
                // 4 :TenDichVu
                // 5 :DonGiaHoaHong
                // 6: Ghi chu
                model.Ma = workSheet.Cells[i, 3].Text;
                model.Ten = workSheet.Cells[i, 4].Text;

                model.DonGiaHoaHong = workSheet.Cells[i, 5].Text;

                model.GhiChu = workSheet.Cells[i, 6].Text;
            }
            return model;
        }

        private async Task XuLyCapNhatImportCauHinhHoaHongDPVTYTAsync(CauHinhChiTietHoaHong vo)
        {
            // nếu dịch vụ khám đã dc tạo rồi . thì cập nhật thông tin dịch vụ trong file excel theo file 
            // nếu dịch vụ khám chưa tạo thì tạo mới
            // nếu dịch vụ khám trong file không chứa dịch vụ khám dc tạo trước đó . xóa dịch vụ khám đó đi


            if (vo.ThongTinCauHinhHoaHongs != null && vo.ThongTinCauHinhHoaHongs.Count() > 0)
            {
                //----------------------------Dược phẩm ------------------------------------//
                var noiGioiThieuHopDongChiTietHoaHongDuocPhams = _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new
                    {
                        Id = d.Id,
                        DuocPhamBenhVienId = d.DuocPhamBenhVienId
                    }).ToList();

                var duocPhamIds = noiGioiThieuHopDongChiTietHoaHongDuocPhams.Select(d => d.DuocPhamBenhVienId).ToList();

                var duocPhamUpdates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDuocPham == true && duocPhamIds.Contains(d.DuocPhamBenhVienId.GetValueOrDefault())).ToList();
                var duocPhams = new List<NoiGioiThieuHopDongChiTietHoaHongDuocPham>();
                if (duocPhamUpdates.Count() > 0) // update
                {
                    var listDichVuIds = noiGioiThieuHopDongChiTietHoaHongDuocPhams.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongDuocPhamIdAsync(listDichVuIds);
                    // xử lý list Dv 
                    foreach (var itemdb in dataItems)
                    {
                        foreach (var itemUP in duocPhamUpdates)
                        {
                            if (itemUP.DuocPhamBenhVienId == itemdb.DuocPhamBenhVienId)
                            {
                                itemdb.DuocPhamBenhVienId = itemUP.DuocPhamBenhVienId.GetValueOrDefault();
                                itemdb.TiLeHoaHong = itemUP.DonGiaHoaHongHoacTien.GetValueOrDefault();

                                itemdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                duocPhams.Add(itemdb);
                            }


                        }

                    }
                }




                var duocPhamCreates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaDuocPham == true && !duocPhamIds.Contains(d.DuocPhamBenhVienId.GetValueOrDefault())).ToList();

                if (duocPhamCreates.Any())
                {
                    // xử lý list DP cần lưu
                    foreach (var item in duocPhamCreates)
                    {
                        var viewModel = new NoiGioiThieuHopDongChiTietHoaHongDuocPham()
                        {
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId.GetValueOrDefault(),
                            TiLeHoaHong = item.DonGiaHoaHongHoacTien.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId,
                        };
                    

                        duocPhams.Add(viewModel);
                    }

                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                }



                var duocPhamCreateIds = duocPhamCreates.Where(d => d.DuocPhamBenhVienId != null).Select(d => d.DuocPhamBenhVienId).ToList();
                var duocPhamUpdateIds = duocPhamUpdates.Where(d => d.DuocPhamBenhVienId != null).Select(d => d.DuocPhamBenhVienId).ToList();

                var duocPhamThoaManDKIds = duocPhamCreateIds.Concat(duocPhamUpdateIds).Select(d => d.GetValueOrDefault());
                var duocPhamDeletes = noiGioiThieuHopDongChiTietHoaHongDuocPhams.Where(d => !duocPhamThoaManDKIds.Contains(d.DuocPhamBenhVienId)).Select(d => d.Id).ToList();

                var dataItemDeletes = duocPhamDeletes;
                var deleteDuocPhams = GetByNoiGioiThieuHopDongChiTietHoaHongDuocPhamIdAsync(dataItemDeletes);
                foreach (var item in deleteDuocPhams)
                {
                    item.WillDelete = true;
                    duocPhams.Add(item);
                }

                //----------------------------END Dược Phẩm --------------------------------//



                //----------------------------Vật tư ------------------------------------//
                var noiGioiThieuHopDongChiTietHoaHongVatTus = _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.TableNoTracking.Where(d => d.NoiGioiThieuHopDongId == vo.NoiGioiThieuHopDongId)
                    .Select(d => new
                    {
                        Id = d.Id,
                        VatTuBenhVienId = d.VatTuBenhVienId
                    }).ToList();

                var vatTuIds = noiGioiThieuHopDongChiTietHoaHongVatTus.Select(d => d.VatTuBenhVienId).ToList();

                var vatTuUpdates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaVatTu == true && vatTuIds.Contains(d.VatTuBenhVienId.GetValueOrDefault())).ToList();
                var vatTus = new List<NoiGioiThieuHopDongChiTietHoaHongVatTu>();
                if (vatTuUpdates.Count() > 0) // update
                {
                    var listDichVuIds = noiGioiThieuHopDongChiTietHoaHongVatTus.Select(d => d.Id).ToList();
                    var dataItems = GetByNoiGioiThieuHopDongChiTietHoaHongVatTuIdAsync(listDichVuIds);
                    // xử lý list Dv 
                    foreach (var itemDvdb in dataItems)
                    {
                        foreach (var itemUP in vatTuUpdates)
                        {
                            if (itemUP.VatTuBenhVienId == itemDvdb.VatTuBenhVienId)
                            {
                                itemDvdb.VatTuBenhVienId = itemUP.VatTuBenhVienId.GetValueOrDefault();
                                itemDvdb.TiLeHoaHong = itemUP.DonGiaHoaHongHoacTien.GetValueOrDefault();

                                itemDvdb.NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId;

                                vatTus.Add(itemDvdb);
                            }


                        }

                    }
                }




                var vatTuCreates = vo.ThongTinCauHinhHoaHongs.Where(d => d.LaVatTu == true && !vatTuIds.Contains(d.VatTuBenhVienId.GetValueOrDefault())).ToList();

                if (vatTuCreates.Any())
                {
                    // xử lý list Dv khám cần lưu
                    foreach (var itemDv in vatTuCreates)
                    {
                        var viewModel = new NoiGioiThieuHopDongChiTietHoaHongVatTu()
                        {
                            VatTuBenhVienId = itemDv.VatTuBenhVienId.GetValueOrDefault(),
                            TiLeHoaHong = itemDv.DonGiaHoaHongHoacTien.GetValueOrDefault(),

                            NoiGioiThieuHopDongId = vo.NoiGioiThieuHopDongId,
                        };

                        

                        vatTus.Add(viewModel);
                    }

                    //await _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhRepository.AddRangeAsync(listNoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh);
                }



                var vatTuCreateIds = vatTuCreates.Where(d => d.VatTuBenhVienId != null).Select(d => d.VatTuBenhVienId).ToList();
                var vatTuUpdateIds = vatTuUpdates.Where(d => d.VatTuBenhVienId != null).Select(d => d.VatTuBenhVienId).ToList();

                var vatTuThoaManDKIds = vatTuCreateIds.Concat(vatTuUpdateIds).Select(d => d.GetValueOrDefault());
                var vatTuDeletes = noiGioiThieuHopDongChiTietHoaHongVatTus.Where(d => !vatTuThoaManDKIds.Contains(d.VatTuBenhVienId)).Select(d => d.Id).ToList();

                var dataItemVatTuDeletes = vatTuDeletes;

                var deleteVatTus = GetByNoiGioiThieuHopDongChiTietHoaHongVatTuIdAsync(dataItemVatTuDeletes);
                foreach (var item in deleteVatTus)
                {
                    item.WillDelete = true;
                    vatTus.Add(item);
                }

                //----------------------------END Dịch vụ kỹ thuật --------------------------------//




                if (duocPhams.Count() > 0)
                {
                    if (duocPhams.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = duocPhams.Where(d => d.Id == 0).ToList();
                         _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.AddRange(dv);
                    }
                    if (duocPhams.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = duocPhams.Where(d => d.Id != 0).ToList();
                         _noiGioiThieuHopDongChiTietHoaHongDuocPhamRepository.UpdateAsync(dv);
                    }

                }
                if (vatTus.Count() > 0)
                {
                    if (vatTus.Where(d => d.Id == 0).Count() > 0)
                    {
                        var dv = vatTus.Where(d => d.Id == 0).ToList();
                         _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.AddRange(dv);
                    }
                    if (vatTus.Where(d => d.Id != 0).Count() > 0)
                    {
                        var dv = vatTus.Where(d => d.Id != 0).ToList();
                         _noiGioiThieuHopDongChiTietHoaHongVatTuRepository.UpdateAsync(dv);
                    }
                }
                




            }
        }

        #endregion
        private bool IsKieuSo(string sVal)
        {
            double test;
            return double.TryParse(sVal, out test);
        }
    }
}
