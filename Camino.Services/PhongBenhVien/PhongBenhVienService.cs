using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhongBenhVien;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhongBenhVien
{
    [ScopedDependency(ServiceType = typeof(IPhongBenhVienService))]
    [System.Runtime.InteropServices.Guid("ABCA4F36-14C8-4CD8-B002-FC6831E7AB6F")]
    public class PhongBenhVienService
        : MasterFileService<Core.Domain.Entities.PhongBenhViens.PhongBenhVien>
        , IPhongBenhVienService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> _hoatDongNhanVienrepository;
        public IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> _khoaPhongNhanVienRepository;
        public PhongBenhVienService
        (IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> repository,
            IUserAgentHelper userAgentHelper,
            IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> hoatDongNhanVienrepository,
            IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> khoaPhongNhanVienRepository,
            IRepository<User> userRepository)
            : base(repository)
        {
            _userAgentHelper = userAgentHelper;
            _userRepository = userRepository;
            _hoatDongNhanVienrepository = hoatDongNhanVienrepository;
            _khoaPhongNhanVienRepository = khoaPhongNhanVienRepository;
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
                var query = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong)
                    .Select(s => new PhongBenhVienGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        Ma = s.Ma,
                        IsDisabled = s.IsDisabled,
                        TenKhoaPhong = s.KhoaPhong.Ten,
                        KieuKham = s.KhoaPhong.CoKhamNgoaiTru
                    }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.Ma,
                g => g.TenKhoaPhong);

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

                var query = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong)
                    .Where(p => p.Ten.Contains(searchString) || p.Ma.Contains(searchString)
                    || p.KhoaPhong.Ten.Contains(searchString))
                    .Select(s => new PhongBenhVienGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        Ma = s.Ma,
                        IsDisabled = s.IsDisabled,
                        TenKhoaPhong = s.KhoaPhong.Ten,
                        KieuKham = s.KhoaPhong.CoKhamNgoaiTru
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ten,
                        g => g.Ma,
                        g => g.TenKhoaPhong);

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
                var result = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong)
                    .Select(s => new PhongBenhVienGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        Ma = s.Ma,
                        IsDisabled = s.IsDisabled,
                        TenKhoaPhong = s.KhoaPhong.Ten,
                        KieuKham = s.KhoaPhong.CoKhamNgoaiTru
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ten,
                        g => g.Ma,
                        g => g.TenKhoaPhong);

                var countTask = result.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong)
                    .Where(p => p.Ten.Contains(searchString) || p.Ma.Contains(searchString)
                    || p.KhoaPhong.Ten.Contains(searchString))
                    .Select(s => new PhongBenhVienGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        Ma = s.Ma,
                        IsDisabled = s.IsDisabled,
                        TenKhoaPhong = s.KhoaPhong.Ten,
                        KieuKham = s.KhoaPhong.CoKhamNgoaiTru
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ten,
                        g => g.Ma,
                        g => g.TenKhoaPhong);

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

        public async Task<List<LookupItemVo>> GetNamePhongBenhVienCreate(long id)
        {
            var list = await BaseRepository.TableNoTracking
                .Where(x => x.KhoaPhongId == id && x.IsDisabled != true)
               .Select(i => new LookupItemVo
               {
                   DisplayName = i.Ten,
                   KeyId = i.Id
               }).ToListAsync();
            return list;
        }
        public async Task<List<LookupItemVo>> GetNamePhongBenhVienDetail(long id, List<long> phongBenhVienIds)
        {
            var list = await BaseRepository.TableNoTracking
                .Where(x => x.KhoaPhongId == id && x.IsDisabled != true || phongBenhVienIds.Contains(x.Id))
               .Select(i => new LookupItemVo()
               {
                   DisplayName = i.Ten,
                   KeyId = i.Id
               }).ToListAsync();
            return list;
        }

        public async Task<List<long>> GetListPhongBenhVien(long id)
        {
            var list = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong).Where(p => p.IsDisabled != true);
            if (id != 0)
            {
                list = list.Where(x => x.KhoaPhongId == id);
            }
            var listId = await list.Select(i => i.Id).ToListAsync();
            return listId;
        }

        public async Task<List<Core.Domain.Entities.PhongBenhViens.PhongBenhVien>> GetListPhongBenhVienByKhoaPhongId(long id, DropDownListRequestModel model = null)
        {
            var modelEntity = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong).Where(p => p.IsDisabled != true);
            if (model != null)
            {
                modelEntity = modelEntity.Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? ""));
            }
            if (id != 0)
            {
                modelEntity = modelEntity.Where(x => x.KhoaPhongId == id);
            }
            var result = await modelEntity.Take(50).ToListAsync();
            return result;
        }

        public async Task<List<LookupItemVo>> GetListPhongBenhVienByCurrentUser()
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var hoatdongNhanVienPhongKhamLast = _hoatDongNhanVienrepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId).LastOrDefault();

            var lstKhoaPhong =
                await _userRepository.TableNoTracking
                    .Where(x => x.Id == currentUserId)
                    .Include(o => o.NhanVien).ThenInclude(n => n.KhoaPhongNhanViens)
                    .Select(x => x.NhanVien.KhoaPhongNhanViens)
                    .ToListAsync();
            if (lstKhoaPhong != null && lstKhoaPhong.Count > 0 && lstKhoaPhong[0].Count == 0)
            {
                return null;
            }
            var lstKhoaPhongId = lstKhoaPhong[0].Select(x => x.KhoaPhongId).ToList();

            if (hoatdongNhanVienPhongKhamLast != null)
            {
                var lstPhongDefault =
                    await BaseRepository.TableNoTracking
                        .Where(x => x.Id == hoatdongNhanVienPhongKhamLast.PhongBenhVienId)
                        .Select(item => new LookupItemVo()
                        {
                            DisplayName = item.Ma + " - " + item.Ten,
                            KeyId = item.Id
                        }).ToListAsync();

                var lstPhong =
                    await BaseRepository.TableNoTracking
                        .Where(x => lstKhoaPhongId.Contains(x.KhoaPhongId))
                        .Select(item => new LookupItemVo()
                        {
                            DisplayName = item.Ma + " - " + item.Ten,
                            KeyId = item.Id
                        }).ToListAsync();

                var phongs = lstPhong.Where(cc => hoatdongNhanVienPhongKhamLast != null && cc.KeyId != hoatdongNhanVienPhongKhamLast.PhongBenhVienId);
                var lstPhongs = lstPhongDefault.Concat(phongs).ToList();
                return lstPhongs;
            }
            else
            {
                var lstPhong =
                    await BaseRepository.TableNoTracking
                        .Where(x => lstKhoaPhongId.Contains(x.KhoaPhongId))
                        .Select(item => new LookupItemVo()
                        {
                            DisplayName = item.Ma + " - " + item.Ten,
                            KeyId = item.Id
                        }).ToListAsync();

                return lstPhong;
            }

        }

        public async Task<List<LookupItemTemplateVo>> GetListPhongBenhVienByKhoa(DropDownListRequestModel model)
        {
            var lstPhong =
                await BaseRepository.TableNoTracking
                    .Where(x => x.KhoaPhongId == model.Id)
                    .Select(item => new LookupItemTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        Ma = item.Ma
                    }).ToListAsync();
            return lstPhong;
        }

        public async Task<List<LookupItemVo>> GetListPhongBenhVienByKhoaSreach(DropDownListRequestModel model, long khoaPhongId)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var hinhThucKhamBenh = CommonHelper.GetIdFromRequestDropDownList(model);

            var KhoaNgoaiVienId = hinhThucKhamBenh == (long)HinhThucKhamBenh.NoiVien && GetKhoaPhongNgoaiVien() != null ? GetKhoaPhongNgoaiVien().KeyId : 0L;      
            var lstKhoaPhongs = await _userRepository.TableNoTracking
                                                 .Where(x => x.Id == currentUserId)
                                                 .Select(x => x.NhanVien.KhoaPhongNhanViens).ToListAsync();

            var lstKhoaPhongIds = lstKhoaPhongs[0].Select(x => x.KhoaPhongId).ToList();
            var phongBVCurrentUser = _hoatDongNhanVienrepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId).Select(c => c.PhongBenhVienId).LastOrDefault();

            if (khoaPhongId == 0)
            {
                var khoaChuaChonPhong = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId && cc.PhongBenhVienId == null).Select(cc => cc.KhoaPhongId);
                var phongBenhVienIds = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId).Select(cc => cc.PhongBenhVienId);
                var lstPhong = await BaseRepository.TableNoTracking
                             .Where(cc => cc.Id != phongBVCurrentUser && (khoaChuaChonPhong.Contains(cc.KhoaPhongId) || phongBenhVienIds.Contains(cc.Id)) && cc.KhoaPhongId != KhoaNgoaiVienId)
                             .ApplyLike(model.Query,
                                g => g.Ma,
                                g => g.Ten)
                             .Select(item => new LookupItemVo()
                             {
                                 DisplayName = item.Ma + " - " + item.Ten,
                                 KeyId = item.Id
                             }).ToListAsync();

                return lstPhong;
            }
            else
            {
                var phongBenhVienIds = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId && cc.KhoaPhongId == khoaPhongId && cc.KhoaPhongId != KhoaNgoaiVienId)
                                                                                  .Select(cc => cc.PhongBenhVienId)
                                                                                  .ToList();

                if (phongBenhVienIds != null && phongBenhVienIds.Where(cc => cc != null).Any())
                {
                    return await BaseRepository.TableNoTracking
                         .Where(cc => cc.Id != phongBVCurrentUser && (cc.KhoaPhongId == khoaPhongId && phongBenhVienIds.Contains(cc.Id)))
                         .ApplyLike(model.Query,
                            g => g.Ma,
                            g => g.Ten)
                     .Select(item => new LookupItemVo()
                     {
                         DisplayName = item.Ma + " - " + item.Ten,
                         KeyId = item.Id
                     }).ToListAsync();
                }
                else
                {
                    return await BaseRepository.TableNoTracking
                          .Where(cc => cc.Id != phongBVCurrentUser && cc.KhoaPhongId == khoaPhongId && cc.KhoaPhongId != KhoaNgoaiVienId)
                          .ApplyLike(model.Query,
                             g => g.Ma,
                             g => g.Ten)
                      .Select(item => new LookupItemVo()
                      {
                          DisplayName = item.Ma + " - " + item.Ten,
                          KeyId = item.Id
                      }).ToListAsync();
                }
            }

        }


        public async Task<List<LookupItemVo>> GetListPhongNgoaiVienByHopDongKhamSreach(DropDownListRequestModel model)
        {
            var hopDongKhamId = model.Id;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var lstKhoaPhongs = await _userRepository.TableNoTracking
                                                     .Where(x => x.Id == currentUserId)
                                                     .Select(x => x.NhanVien.KhoaPhongNhanViens).ToListAsync();
            var lstKhoaPhongIds = lstKhoaPhongs[0].Select(x => x.KhoaPhongId).ToList();
      
            var khoaPhongId = BaseRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamId).Select(c => c.KhoaPhongId).FirstOrDefault();
            if (khoaPhongId == 0)
            {
                var khoaChuaChonPhong = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId && cc.PhongBenhVienId == null).Select(cc => cc.KhoaPhongId);
                var phongBenhVienIds = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId).Select(cc => cc.PhongBenhVienId);

                var lstPhong = await BaseRepository.TableNoTracking
                             .Where(cc => (khoaChuaChonPhong.Contains(cc.KhoaPhongId) || phongBenhVienIds.Contains(cc.Id)))
                             .ApplyLike(model.Query,
                                g => g.Ma,
                                g => g.Ten)
                             .Select(item => new LookupItemVo()
                             {
                                 DisplayName = item.Ma + " - " + item.Ten,
                                 KeyId = item.Id
                             }).ToListAsync();

                return lstPhong;
            }
            else
            {

                var phongBenhVienIds = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId && cc.KhoaPhongId == khoaPhongId)
                                                                                  .Select(cc => cc.PhongBenhVienId)
                                                                                  .ToList();

                if (phongBenhVienIds != null && phongBenhVienIds.Where(cc => cc != null).Any())
                {
                    return await BaseRepository.TableNoTracking
                         .Where(cc => (cc.KhoaPhongId == khoaPhongId && phongBenhVienIds.Contains(cc.Id)) && cc.HopDongKhamSucKhoeId == hopDongKhamId)
                         .ApplyLike(model.Query,
                            g => g.Ma,
                            g => g.Ten)
                     .Select(item => new LookupItemVo()
                     {
                         DisplayName = item.Ma + " - " + item.Ten,
                         KeyId = item.Id
                     }).ToListAsync();
                }
                else
                {
                    return await BaseRepository.TableNoTracking
                          .Where(cc => cc.KhoaPhongId == khoaPhongId && cc.HopDongKhamSucKhoeId == hopDongKhamId)
                          .ApplyLike(model.Query,
                             g => g.Ma,
                             g => g.Ten)
                      .Select(item => new LookupItemVo()
                      {
                          DisplayName = item.Ma + " - " + item.Ten,
                          KeyId = item.Id
                      }).ToListAsync();
                }
            }

        }

        public LookupItemVo GetKhoaPhongNgoaiVien()
        {
            var lstPhong = BaseRepository.TableNoTracking.Where(p => p.KhoaPhong.Ma == "KKDNV" &&
                                                                          p.IsDisabled != true)
                                                               .Select(p => new LookupItemVo
                                                               {
                                                                   KeyId = p.KhoaPhong.Id,
                                                                   DisplayName = p.KhoaPhong.Ten
                                                               }).FirstOrDefault();

            return lstPhong;
        }

        public async Task<List<LookupItemVo>> GetPhongBenhViensByKhoaPhongId(DropDownListRequestModel model, long khoaPhongId)
        {
            var lstPhong = await BaseRepository.TableNoTracking.Where(p => p.KhoaPhongId == khoaPhongId &&
                                                                           p.IsDisabled != true)
                                                               .Select(p => new LookupItemVo
                                                               {
                                                                   KeyId = p.Id,
                                                                   DisplayName = p.Ten
                                                               })
                                                               .ApplyLike(model.Query, p => p.DisplayName)
                .Take(model.Take)
                                                               .ToListAsync();

            return lstPhong;
        }

        public async Task<LookupItemVo> GetKhoaByPhong(long phongId)
        {
            var listKhoaPhong = await BaseRepository.TableNoTracking.Where(cc => cc.Id == phongId)
                .Include(cc => cc.KhoaPhong)
                .Where(p => p.IsDisabled != true).FirstOrDefaultAsync();
            var query = new LookupItemVo
            {
                DisplayName = listKhoaPhong.KhoaPhong.Ma + " - " + listKhoaPhong.KhoaPhong.Ten,
                KeyId = listKhoaPhong.KhoaPhongId,
            };

            return query;
        }

        public async Task<LookupItemVo> GetTenKhoaByPhong(long phongId)
        {
            var phongBenhVien = await BaseRepository.TableNoTracking.Where(p => p.Id == phongId && p.IsDisabled != true)
                                                                    .Include(p => p.KhoaPhong)
                                                                    .FirstOrDefaultAsync();
            var query = new LookupItemVo
            {
                DisplayName = phongBenhVien.KhoaPhong.Ten,
                KeyId = phongBenhVien.KhoaPhongId,
            };

            return query;
        }

        public async Task<List<long>> GetPhongByListKhoa(List<long> khoaIds)
        {
            return await BaseRepository.TableNoTracking.Where(cc => khoaIds.Contains(cc.KhoaPhongId)).Select(cc => cc.Id).ToListAsync();
        }

        public async Task<List<LookupItemTemplateVo>> GetListPhongTatCa(DropDownListRequestModel model)
        {
            var lookups = await BaseRepository.TableNoTracking
                .ApplyLike(model.Query?.Trim(), x => x.Ten, x => x.Ma)
                .Where(x => x.IsDisabled != true)
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                .Take(model.Take)
                .Select(x => new LookupItemTemplateVo()
                {
                    KeyId = x.Id,
                    DisplayName = x.Ten,
                    Ten = x.Ten,
                    Ma = x.Ma
                }).ToListAsync();
            return lookups;
        }
    }
}
