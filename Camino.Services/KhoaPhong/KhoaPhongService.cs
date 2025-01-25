using System;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoaPhong;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Microsoft.EntityFrameworkCore.Internal;
using Camino.Core.Domain.Entities.Users;
using Camino.Services.Helpers;
using Camino.Services.PhongBenhVien;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.KhoaPhong
{
    [ScopedDependency(ServiceType = typeof(IKhoaPhongService))]
    public class KhoaPhongService
        : MasterFileService<Core.Domain.Entities.KhoaPhongs.KhoaPhong>
        , IKhoaPhongService
    {
        private IRepository<Core.Domain.Entities.BenhVien.Khoas.Khoa> _repositoryKhoa;
        private readonly IRepository<User> _userRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IPhongBenhVienService _phongBenhVienService;
        public KhoaPhongService
        (IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> repository,
            IUserAgentHelper userAgentHelper,
            IRepository<User> userRepository,
            IRepository<Core.Domain.Entities.BenhVien.Khoas.Khoa> repositoryKhoa,
            IPhongBenhVienService phongBenhVienService)
            : base(repository)
        {
            _repositoryKhoa = repositoryKhoa;
            _userAgentHelper = userAgentHelper;
            _userRepository = userRepository;
            _phongBenhVienService = phongBenhVienService;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {

                var query = BaseRepository.TableNoTracking.Include(p => p.KhoaPhongChuyenKhoas).ThenInclude(r => r.Khoa)
                    .Select(s => new KhoaPhongGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        Ma = s.Ma,
                        LoaiKhoaPhong = s.LoaiKhoaPhong,
                        CoKhamNgoaiTru = s.CoKhamNgoaiTru,
                        CoKhamNoiTru = s.CoKhamNoiTru,
                        MoTa = s.MoTa,
                        SoTienThuTamUng = s.SoTienThuTamUng ?? 0,
                        IsDisabled = s.IsDisabled,
                        TenKhoa = s.KhoaPhongChuyenKhoas.Select(i => i.Khoa.Ten).ToArray().Join(","),
                        TenLoaiKhoaPhongDisplayName = s.LoaiKhoaPhong.GetDescription(),
                        //KieuKhamDisplay = s.CoKhamNgoaiTru == true ? "Ngoại Trú" : "Nội Trú"
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.MoTa);


                var countTask = queryInfo.LazyLoadPage == true ?
                    Task.FromResult(0) :
                    query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);

                return new GridDataSource
                {
                    Data = queryTask.Result,
                    TotalRowCount = countTask.Result
                };
            }
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking
                    .Where(p => p.Ten.Contains(searchString) || p.Ma.Contains(searchString)
                    )
                    .Select(s => new KhoaPhongGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        Ma = s.Ma,
                        LoaiKhoaPhong = s.LoaiKhoaPhong,
                        CoKhamNgoaiTru = s.CoKhamNgoaiTru,
                        CoKhamNoiTru = s.CoKhamNoiTru,
                        MoTa = s.MoTa,
                        IsDisabled = s.IsDisabled,
                        SoTienThuTamUng = s.SoTienThuTamUng ?? 0,
                        TenLoaiKhoaPhongDisplayName = s.LoaiKhoaPhong.GetDescription(),
                        //KieuKhamDisplay = s.CoKhamNgoaiTru == true ? "Ngoại Trú" : "Nội Trú"
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.MoTa);

                var countTask = queryInfo.LazyLoadPage == true ?
                    Task.FromResult(0) :
                    query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);

                return new GridDataSource
                {
                    Data = queryTask.Result,
                    TotalRowCount = countTask.Result
                };
            }
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var result = BaseRepository.TableNoTracking
                    .Select(s => new KhoaPhongGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        Ma = s.Ma,
                        LoaiKhoaPhong = s.LoaiKhoaPhong,
                        CoKhamNgoaiTru = s.CoKhamNgoaiTru,
                        CoKhamNoiTru = s.CoKhamNoiTru,
                        MoTa = s.MoTa,
                        IsDisabled = s.IsDisabled,
                        SoTienThuTamUng = s.SoTienThuTamUng ?? 0,
                        TenLoaiKhoaPhongDisplayName = s.LoaiKhoaPhong.GetDescription(),
                        //KieuKhamDisplay = s.CoKhamNgoaiTru == true ? "Ngoại Trú" : "Nội Trú"
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,

                        g => g.MoTa);

                var countTask = result.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking
                    .Where(p => p.Ten.Contains(searchString) || p.Ma.Contains(searchString))
                    .Select(s => new KhoaPhongGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        Ma = s.Ma,
                        LoaiKhoaPhong = s.LoaiKhoaPhong,
                        CoKhamNgoaiTru = s.CoKhamNgoaiTru,
                        CoKhamNoiTru = s.CoKhamNoiTru,
                        MoTa = s.MoTa,
                        SoTienThuTamUng = s.SoTienThuTamUng ?? 0,
                        IsDisabled = s.IsDisabled,
                        TenLoaiKhoaPhongDisplayName = s.LoaiKhoaPhong.GetDescription(),
                        //KieuKhamDisplay = s.CoKhamNgoaiTru == true ? "Ngoại Trú" : "Nội Trú"
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.MoTa);

                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource
                {
                    TotalRowCount = countTask.Result
                };
            }
        }

        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) &&
                queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?
                    .Replace("Format", "");
            }
        }

        public async Task<bool> IsTenExists(string ten = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id);
            }

            return result;
        }

        public async Task<bool> IsMaExists(string ma = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) && p.Id != id);
            }

            return result;
        }

        public async Task<List<KhoaKhamTemplateVo>> GetListKhoaPhong(DropDownListRequestModel model)
        {
            var listKhoaPhong = await BaseRepository.TableNoTracking
                .Where(p => p.IsDisabled != true && p.CoKhamNgoaiTru != null)
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoaPhong.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return query;
        }
        public async Task<List<KhoaKhamTemplateVo>> GetListKhoaPhongThuocNoiTruAll(DropDownListRequestModel queryInfo)
        {
            var listKhoaPhong = await BaseRepository.TableNoTracking
                .Where(p => p.IsDisabled != true && p.CoKhamNoiTru != null)
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

            return query;
        }

        public async Task<List<KhoaKhamTemplateVo>> GetListKhoaPhongAll(DropDownListRequestModel queryInfo)
        {
            var listKhoaPhong = await BaseRepository.TableNoTracking
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

            return query;
        }

        public List<LookupItemVo> GetListLoaiKhoaPhong(LookupQueryInfo queryInfo)
        {
            var listEnumLoaiKhoaPhong = EnumHelper.GetListEnum<Enums.EnumLoaiKhoaPhong>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumLoaiKhoaPhong = listEnumLoaiKhoaPhong.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                      .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumLoaiKhoaPhong;
        }

        public List<LookupItemVo> GetListKieuKham(LookupQueryInfo queryInfo)
        {
            var listKieuKham = EnumHelper.GetListEnum<Enums.EnumKieuKham>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listKieuKham = listKieuKham.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                              .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listKieuKham;
        }

        public async Task<List<KhoaKhamTemplateVo>> GetListKhoa(DropDownListRequestModel model)
        {
            var hinhThucKhamBenh = CommonHelper.GetIdFromRequestDropDownList(model);
            if (hinhThucKhamBenh == (long)HinhThucKhamBenh.KhamDoanNgoaiVien)
            {
                var currentUserId = _userAgentHelper.GetCurrentUserId();
                var listKhoaPhong = await BaseRepository.TableNoTracking.Include(cc => cc.KhoaPhongNhanViens)
                                                        .Where(p => p.IsDisabled != true && p.Ma == "KKDNV")
                                                        .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                                                        .ToListAsync();
                var query = listKhoaPhong.Where(cc => cc.KhoaPhongNhanViens.Any(g => g.NhanVienId == currentUserId)).Select(item => new KhoaKhamTemplateVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    Ten = item.Ten,
                    Ma = item.Ma
                }).Take(model.Take).ToList();

                return query;
            }
            else
            {
                var currentUserId = _userAgentHelper.GetCurrentUserId();
                var listKhoaPhong = await BaseRepository.TableNoTracking.Include(cc => cc.KhoaPhongNhanViens)
                                                        .Where(p => p.IsDisabled != true && p.Ma != "KKDNV")
                                                        .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                                                        .ToListAsync();

                var query = listKhoaPhong.Where(cc => cc.KhoaPhongNhanViens.Any(g => g.NhanVienId == currentUserId)).Select(item => new KhoaKhamTemplateVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    Ten = item.Ten,
                    Ma = item.Ma
                }).Take(model.Take).ToList();

                return query;
            }

        }

        // to do nam ho
        public async Task<List<LookupItemVo>> GetListKhoaPhongThuNgan(DropDownListRequestModel model)
        {
            int phongTaiChinhKeToan = (int)Enums.EnumKhoaPhong.PTCKT;

            var listKhoaPhong = await BaseRepository.TableNoTracking
                .Where(p => p.Id == phongTaiChinhKeToan)
                .Include(cc => cc.PhongBenhViens)
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoaPhong.SelectMany(cc => cc.PhongBenhViens)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ma + " - " + item.Ten,
                    KeyId = item.Id
                }).ToList();

            return query;
        }
        //public async Task<List<LookupItemVo>> GetListKhoaPhongThuNgan(DropDownListRequestModel model)
        //{
        //    var listKhoaPhong = await BaseRepository.TableNoTracking
        //        .Where(p => p.Ten.Contains(model.Query ?? ""))
        //        .Take(model.Take)
        //        .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
        //        .ToListAsync();

        //    var query = listKhoaPhong
        //        .Select(item => new LookupItemVo
        //        {
        //            DisplayName = item.Ten,
        //            KeyId = item.Id
        //        }).ToList();

        //    return query;
        //}

    }
}
