using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.DoiTuongUuDais;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DoiTuongUuDais
{
    [ScopedDependency(ServiceType = typeof(IDoiTuongUuDaiService))]
    public class DoiTuongUuDaiService : MasterFileService<DoiTuongUuDai>, IDoiTuongUuDaiService
    {
        private readonly IRepository<DoiTuongUuDaiDichVuKyThuatBenhVien> _doiTuongUuDaiDichVuKyThuatRepository;
        private readonly IRepository<DoiTuongUuDaiDichVuKhamBenhBenhVien> _doiTuongUuDaiDichVuKhamBenhRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> _dichvuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichvuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVien> _dichvuKhamBenhBenhVienRepository;
        public DoiTuongUuDaiService(IRepository<DoiTuongUuDai> repository, IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichvuKyThuatBenhVienRepository, IRepository<DoiTuongUuDaiDichVuKhamBenhBenhVien> doiTuongUuDaiDichVuKhamBenhRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> dichvuKyThuatRepository, IRepository<DoiTuongUuDaiDichVuKyThuatBenhVien> doiTuongUuDaiDichVuKyThuatRepository,
            IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVien> dichvuKhamBenhBenhVienRepository) : base(repository)
        {
            _dichvuKhamBenhBenhVienRepository = dichvuKhamBenhBenhVienRepository;
            _doiTuongUuDaiDichVuKhamBenhRepository = doiTuongUuDaiDichVuKhamBenhRepository;
            _dichvuKyThuatBenhVienRepository = dichvuKyThuatBenhVienRepository;
            _dichvuKyThuatRepository = dichvuKyThuatRepository;
            _doiTuongUuDaiDichVuKyThuatRepository = doiTuongUuDaiDichVuKyThuatRepository;
        }
        public async Task<List<LookupItemVo>> GetDoiTuong()
        {
            var lst = await BaseRepository.TableNoTracking.Where(x => x.IsDisabled == false)
                .ToListAsync();

            var query = lst.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id
            }).ToList();

            return query;
        }
        public async Task AddDoiTuongEntity(DoiTuongUuDaiDichVuKyThuatBenhVien entity)
        {
            await _doiTuongUuDaiDichVuKyThuatRepository.AddAsync(entity);
        }
        public async Task<bool> CheckDichVuKhamBenhActive(long id)
        {
            var check = await _dichvuKhamBenhBenhVienRepository.TableNoTracking.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (check != null)
            {
                if (check.HieuLuc == true)
                {
                    return true;
                }

            }
            return false;
        }
        public async Task<bool> CheckDichVuKhamBenhExit(long doiTuongUuDaiId , long dichVuKhamBenhBenhVienId)
        {
            var check = await _doiTuongUuDaiDichVuKhamBenhRepository.TableNoTracking.Where(x => x.DoiTuongUuDaiId == doiTuongUuDaiId && x.DichVuKhamBenhBenhVienId == dichVuKhamBenhBenhVienId).FirstOrDefaultAsync();
            if (check != null)
            {
               return true;
            }
            return false;
        }
        public async Task<bool> CheckDichVuKyThuatExit(long doiTuongUuDaiId, long dichVuKyThuatBenhVienId)
        {
            var check = await _doiTuongUuDaiDichVuKyThuatRepository.TableNoTracking.Where(x => x.DoiTuongUuDaiId == doiTuongUuDaiId && x.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId).FirstOrDefaultAsync();
            if (check != null)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> CheckDichVuKyThuatActive(long id)
        {
            var check = await _dichvuKyThuatBenhVienRepository.TableNoTracking.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (check != null)
            {
                if (check.HieuLuc == true)
                {
                    return true;
                }

            }
            return false;
        }
        public async Task AddDoiTuongUuDaiKhamBenhEntity(DoiTuongUuDaiDichVuKhamBenhBenhVien entity)
        {
            await _doiTuongUuDaiDichVuKhamBenhRepository.AddAsync(entity);
        }
        public async Task<bool> CheckDichVuKyThuatExist(long id)
        {
            var check = await _dichvuKyThuatBenhVienRepository.TableNoTracking.Where(x => x.Id == id).ToListAsync();
            if (check.Count > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> CheckDichVuKhamBenhExist(long id)
        {
            var check = await _dichvuKhamBenhBenhVienRepository.TableNoTracking.Where(x => x.Id == id).ToListAsync();
            if (check.Count > 0)
            {
                return true;
            }
            return false;
        }
        public async Task AddDoiTuongEntity(long idCu, long idMoi, int TiLeUuDai, long doiTuong, long doiTuongOld)
        {
            var lastEntity = await _doiTuongUuDaiDichVuKyThuatRepository.Table.Where(p => p.DichVuKyThuatBenhVienId == idCu && p.DoiTuongUuDaiId == doiTuongOld).Include(x => x.DichVuKyThuatBenhVien).Include(x => x.DoiTuongUuDai)
               .ToListAsync();

            for (int i = 0; i < lastEntity.Count; i++)
            {
                lastEntity[i].DichVuKyThuatBenhVienId = idMoi;
                lastEntity[i].DoiTuongUuDaiId = doiTuong;
                lastEntity[i].TiLeUuDai = TiLeUuDai;
            }

            await _doiTuongUuDaiDichVuKyThuatRepository.UpdateAsync(lastEntity);
        }
        public async Task DeleteToAdd(long id)
        {
            var lastEntity = await _doiTuongUuDaiDichVuKyThuatRepository.Table.Where(p => p.DichVuKyThuatBenhVienId == id).Include(x => x.DichVuKyThuatBenhVien).Include(x => x.DoiTuongUuDai)
               .ToListAsync();
            await _doiTuongUuDaiDichVuKyThuatRepository.DeleteAsync(lastEntity);
        }
        public async Task DeleteToAddDoiTuongUuDaiKhamBenh(long id)
        {
            var lastEntity = await _doiTuongUuDaiDichVuKhamBenhRepository.Table.Where(p => p.DichVuKhamBenhBenhVienId == id).Include(x => x.DichVuKhamBenhBenhVien).Include(x => x.DoiTuongUuDai)
               .ToListAsync();
            await _doiTuongUuDaiDichVuKhamBenhRepository.DeleteAsync(lastEntity);
        }
        public async Task<List<DoiTuongUuDaiTemplateVo>> GetDichVuKyThuatById(DropDownListRequestModel model, long id)
        {

            var lst = await _dichvuKyThuatBenhVienRepository.TableNoTracking.Include(x => x.DichVuKyThuat)
                  .Where(p => (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")
                                                                               || (p.DichVuKyThuat != null && p.DichVuKyThuat.Ma4350.Contains(model.Query ?? "")))
                              && p.HieuLuc)
                  .Take(model.Take)
                  .ToListAsync();
            var entity = _doiTuongUuDaiDichVuKyThuatRepository.TableNoTracking
                .Select(x => x.DichVuKyThuatBenhVienId).Distinct().ToList();
            var entityAdd = _dichvuKyThuatBenhVienRepository.TableNoTracking.Where(x => x.Id == id).Include(x => x.DichVuKyThuat)
                .Where(p => (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")
                                                                             || (p.DichVuKyThuat != null && p.DichVuKyThuat.Ma4350.Contains(model.Query ?? "")))
                            ).ToList();
            for (int i = 0; i < entity.Count; i++)
            {
                lst.RemoveAll(x => x.Id == entity[i]);
            }
            lst.AddRange(entityAdd);
            var query = lst.Select(item => new DoiTuongUuDaiTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                DichVu = item.Ten,
                Ma4350 = item.DichVuKyThuat?.Ma4350,
                //TenKhoa = item.Khoa != null ? item.Khoa.Ten : null,
                Ma = item.Ma,
            }).ToList();

            return query;
        }
        public async Task<List<DoiTuongUuDaiTemplateVo>> GetDichVuKhamBenhById(DropDownListRequestModel model, long id)
        {

            var lst = await _dichvuKhamBenhBenhVienRepository.TableNoTracking.Include(x => x.DichVuKhamBenh)//.Include(x => x.KhoaPhong)
                  .Where(p => (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")
                                                                                || (p.DichVuKhamBenh != null && p.DichVuKhamBenh.MaTT37.Contains(model.Query ?? "")))
                        //|| (p.KhoaPhong != null && p.KhoaPhong.Ten.Contains(model.Query ?? "")))
                        && p.HieuLuc)
                  .Take(model.Take)
                  .ToListAsync();
            var entity = _doiTuongUuDaiDichVuKhamBenhRepository.TableNoTracking
                .Select(x => x.DichVuKhamBenhBenhVienId).Distinct().ToList();
            var entityAdd = _dichvuKhamBenhBenhVienRepository.TableNoTracking.Where(x => x.Id == id).Include(x => x.DichVuKhamBenh)//.Include(x => x.KhoaPhong)
                .Where(p => (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")
                                                                                || (p.DichVuKhamBenh != null && p.DichVuKhamBenh.MaTT37.Contains(model.Query ?? ""))))
                //|| (p.KhoaPhong != null && p.KhoaPhong.Ten.Contains(model.Query ?? ""))))
                .ToList();
            for (int i = 0; i < entity.Count; i++)
            {
                lst.RemoveAll(x => x.Id == entity[i]);
            }
            lst.AddRange(entityAdd);
            var query = lst.Select(item => new DoiTuongUuDaiTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                DichVu = item.Ten,
                Ma4350 = item.DichVuKhamBenh?.MaTT37,
                //TenKhoa = item.KhoaPhong != null ? item.KhoaPhong.Ten : null,
                Ma = item.Ma,
            }).ToList();

            return query;
        }
        public async Task<List<DoiTuongUuDaiTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model)
        {
            var modelQuery = model.Query ?? "";

            if (string.IsNullOrEmpty(modelQuery) || !modelQuery.Contains(" "))
            {
                var lstR = _dichvuKyThuatBenhVienRepository.TableNoTracking
                    .Include(x => x.DichVuKyThuat)
                    .Where(p => (p.Ten.Contains(modelQuery) || p.Ma.Contains(modelQuery)
                                 || (p.DichVuKyThuat != null && p.DichVuKyThuat.Ma4350.Contains(modelQuery))));
                var queryR = await lstR.Select(item => new DoiTuongUuDaiTemplateVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    DichVu = item.Ten,
                    Ma4350 = item.DichVuKyThuat != null ? item.DichVuKyThuat.Ma4350 : "",
                    Ma = item.Ma,
                })
                    .Take(model.Take)
                    .ToListAsync();

                return queryR;
            }

            var lstColumnNameSearch = new List<string> { "Ten", "Ma" };

            var lstId = _dichvuKyThuatBenhVienRepository
                .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien), lstColumnNameSearch)
                .Select(p => p.Id).ToList();

            var dct = lstId.Select((p, i) => new
            {
                key = p,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);


            var lst = _dichvuKyThuatBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuKyThuat)
                .Where(p => p.HieuLuc)
                .Where(p => lstId.Any(x => x == p.Id) || (p.DichVuKyThuat != null && p.DichVuKyThuat.Ma4350.Contains(modelQuery)));
            var query = await lst.Select(item => new DoiTuongUuDaiTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                DichVu = item.Ten,
                Ma4350 = item.DichVuKyThuat != null ? item.DichVuKyThuat.Ma4350 : "",
                Ma = item.Ma,
            })
            .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
            .Take(model.Take)
            .ToListAsync();

            return query;
        }
        public async Task<List<DoiTuongUuDaiTemplateVo>> GetDichVuKhamBenh(DropDownListRequestModel model)
        {
            var modelQuery = model.Query ?? "";

            if (string.IsNullOrEmpty(modelQuery) || !modelQuery.Contains(" "))
            {
                var lstR = _dichvuKhamBenhBenhVienRepository.TableNoTracking
                    .Include(x => x.DichVuKhamBenh)
                    .Where(p => (p.Ten.Contains(modelQuery) || p.Ma.Contains(modelQuery)
                                 || (p.DichVuKhamBenh != null && p.DichVuKhamBenh.MaTT37.Contains(modelQuery))));
                var queryR = await lstR.Select(item => new DoiTuongUuDaiTemplateVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    DichVu = item.Ten,
                    Ma4350 = item.DichVuKhamBenh != null ? item.DichVuKhamBenh.MaTT37 : "",
                    Ma = item.Ma,
                })
                    .Take(model.Take)
                    .ToListAsync();

                return queryR;
            }

            var lstColumnNameSearch = new List<string> { "Ten", "Ma" };

            var lstId = _dichvuKhamBenhBenhVienRepository
                .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVien), lstColumnNameSearch)
                .Select(p => p.Id).ToList();

            var dct = lstId.Select((p, i) => new
            {
                key = p,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);


            var lst = _dichvuKhamBenhBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuKhamBenh)
                .Where(p => p.HieuLuc)
                .Where(p => lstId.Any(x => x == p.Id) || (p.DichVuKhamBenh != null && p.DichVuKhamBenh.MaTT37.Contains(modelQuery)));
            var query = await lst.Select(item => new DoiTuongUuDaiTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                DichVu = item.Ten,
                Ma4350 = item.DichVuKhamBenh != null ? item.DichVuKhamBenh.MaTT37 : "",
                Ma = item.Ma,
            })
            .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
            .Take(model.Take)
            .ToListAsync();

            return query;
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _doiTuongUuDaiDichVuKyThuatRepository.TableNoTracking
                .Include(p => p.DoiTuongUuDai)
                .Include(x => x.DichVuKyThuatBenhVien).ThenInclude(x => x.DichVuKyThuat)
                //.Include(x => x.DichVuKyThuatBenhVien).ThenInclude(x=>x.Khoa)
                .Select(s => new DoiTuongUuDaiGridVo
                {
                    Id = s.Id,
                    Ma = s.DichVuKyThuatBenhVien.Ma,
                    Ma4350 = s.DichVuKyThuatBenhVien.DichVuKyThuat == null ? string.Empty : s.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350,
                    DichVuKyThuatId = s.DichVuKyThuatBenhVienId,
                    Ten = s.DichVuKyThuatBenhVien.Ten,

                }).Distinct();
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ma4350, g => g.Ten);

            query = query.GroupBy(g => new
            {
                g.DichVuKyThuatId
            })
               .Select(g => g.First());
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _doiTuongUuDaiDichVuKyThuatRepository.TableNoTracking
                 .Include(p => p.DoiTuongUuDai)
                 .Include(x => x.DichVuKyThuatBenhVien).ThenInclude(x => x.DichVuKyThuat)
                 //.Include(x => x.DichVuKyThuatBenhVien).ThenInclude(x => x.Khoa)
                 .Select(s => new DoiTuongUuDaiGridVo
                 {
                     Id = s.Id,
                     Ma = s.DichVuKyThuatBenhVien.Ma,
                     Ma4350 = s.DichVuKyThuatBenhVien.DichVuKyThuat == null ? string.Empty : s.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350,
                     DichVuKyThuatId = s.DichVuKyThuatBenhVien.Id,
                     Ten = s.DichVuKyThuatBenhVien.Ten,
                     //TenKhoa = s.DichVuKyThuatBenhVien.Khoa.Ten,

                 });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ma4350, g => g.Ten);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long parentId = 0)
        {
            BuildDefaultSortExpression(queryInfo);
            //var sortString = RemoveDisplaySort(queryInfo);
            var currentParentId = parentId == 0 ? long.Parse(queryInfo.SearchTerms) : parentId;

            var query = _doiTuongUuDaiDichVuKyThuatRepository.TableNoTracking.Include(x => x.DichVuKyThuatBenhVien).ThenInclude(x => x.DichVuKyThuat).Include(x => x.DoiTuongUuDai)
                .Where(x => x.DichVuKyThuatBenhVienId == currentParentId)
                .Select(s => new DoiTuongUuDaiChildGridVo()
                {
                    Id = s.Id,
                    DoiTuong = s.DoiTuongUuDai.Ten,
                    TiLeUuDai = s.TiLeUuDai + "%"
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo, long parentId = 0)
        {
            BuildDefaultSortExpression(queryInfo);

            var currentParentId = parentId == 0 ? long.Parse(queryInfo.SearchTerms) : parentId;

            var query = _doiTuongUuDaiDichVuKyThuatRepository.TableNoTracking.Include(x => x.DichVuKyThuatBenhVien).ThenInclude(x => x.DichVuKyThuat).Include(x => x.DoiTuongUuDai)
                .Where(x => x.Id == currentParentId)
                .Select(s => new DoiTuongUuDaiChildGridVo()
                {
                    Id = s.Id,
                    DoiTuong = s.DoiTuongUuDai.Ten,
                    TiLeUuDai = s.TiLeUuDai + "%"
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task DeleteDoiTuongDichVuKyThuat(long id)
        {
            var lastEntity = await _doiTuongUuDaiDichVuKyThuatRepository.Table.Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (lastEntity != null)
            {
                var entity = await _doiTuongUuDaiDichVuKyThuatRepository.Table.Where(p => p.DichVuKyThuatBenhVienId == lastEntity.DichVuKyThuatBenhVienId).ToListAsync();
                if (entity.Count > 0)
                {
                    foreach (var item in entity)
                    {
                        await _doiTuongUuDaiDichVuKyThuatRepository.DeleteAsync(item);
                    }
                }

                //lastEntity.DenNgay = tuNgay.AddDays(-1);

            }
        }
        public async Task DeleteDoiTuongDichVuKhamBenh(long id)
        {
            var lastEntity = await _doiTuongUuDaiDichVuKhamBenhRepository.Table.Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (lastEntity != null)
            {
                var entity = await _doiTuongUuDaiDichVuKhamBenhRepository.Table.Where(p => p.DichVuKhamBenhBenhVienId == lastEntity.DichVuKhamBenhBenhVienId).ToListAsync();
                if (entity.Count > 0)
                {
                    foreach (var item in entity)
                    {
                        await _doiTuongUuDaiDichVuKhamBenhRepository.DeleteAsync(item);
                    }
                }

            }
        }
        public async Task<List<DoiTuongUuDaiDichVuKyThuatBenhVien>> GetData(long id)
        {
            var lastEntity = await _doiTuongUuDaiDichVuKyThuatRepository.Table.Where(p => p.Id == id).Include(x => x.DichVuKyThuatBenhVien).Include(x => x.DoiTuongUuDai)
                .FirstOrDefaultAsync();

            if (lastEntity != null)
            {
                var entity = await _doiTuongUuDaiDichVuKyThuatRepository.Table.Where(p => p.DichVuKyThuatBenhVienId == lastEntity.DichVuKyThuatBenhVienId).Include(x => x.DoiTuongUuDai)
                    .Include(x => x.DichVuKyThuatBenhVien).ToListAsync();
                return entity;

                //lastEntity.DenNgay = tuNgay.AddDays(-1);

            }
            return null;
        }
        public async Task<List<DoiTuongUuDaiDichVuKhamBenhBenhVien>> GetDataDoiTuongUuDaiKhamBenh(long id)
        {
            var lastEntity = await _doiTuongUuDaiDichVuKhamBenhRepository.Table.Where(p => p.Id == id).Include(x => x.DichVuKhamBenhBenhVien).Include(x => x.DoiTuongUuDai)
                .FirstOrDefaultAsync();

            if (lastEntity != null)
            {
                var entity = await _doiTuongUuDaiDichVuKhamBenhRepository.Table.Where(p => p.DichVuKhamBenhBenhVienId == lastEntity.DichVuKhamBenhBenhVienId).Include(x => x.DoiTuongUuDai)
                    .Include(x => x.DichVuKhamBenhBenhVien).ToListAsync();
                return entity;

                //lastEntity.DenNgay = tuNgay.AddDays(-1);

            }
            return null;
        }
        public async Task<string> GetNameDichVuKyThuat(long id)
        {
            var lastEntity = await _dichvuKyThuatBenhVienRepository.Table.Where(p => p.Id == id).Include(x => x.DichVuKyThuat)//.Include(x => x.Khoa)
                .FirstOrDefaultAsync();

            if (lastEntity != null)
            {
                var entity = lastEntity.Ten;
                return entity;

                //lastEntity.DenNgay = tuNgay.AddDays(-1);

            }
            return null;
        }
        public async Task<string> GetNameDichVuKhamBenh(long id)
        {
            var lastEntity = await _dichvuKhamBenhBenhVienRepository.Table.Where(p => p.Id == id).Include(x => x.DichVuKhamBenh)//.Include(x => x.KhoaPhong)
                .FirstOrDefaultAsync();

            if (lastEntity != null)
            {
                var entity = lastEntity.Ten;
                return entity;

                //lastEntity.DenNgay = tuNgay.AddDays(-1);

            }
            return null;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<GridDataSource> GetDataForGridBenhVienAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _doiTuongUuDaiDichVuKhamBenhRepository.TableNoTracking
                .Include(p => p.DoiTuongUuDai)
                .Include(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.DichVuKhamBenh)
                //.Include(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.KhoaPhong)
                .Select(s => new DoiTuongUuDaiGridVo
                {
                    Id = s.Id,
                    Ma = s.DichVuKhamBenhBenhVien.Ma,
                    Ma4350 = s.DichVuKhamBenhBenhVien.DichVuKhamBenh == null ? string.Empty : s.DichVuKhamBenhBenhVien.DichVuKhamBenh.MaTT37,
                    DichVuKyThuatId = s.DichVuKhamBenhBenhVienId,
                    Ten = s.DichVuKhamBenhBenhVien.Ten,
                    //TenKhoa = s.DichVuKhamBenhBenhVien.KhoaPhong.Ten,

                }).Distinct();
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ma4350, g => g.Ten);

            query = query.GroupBy(g => new
            {
                g.DichVuKyThuatId
            })
               .Select(g => g.First());
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridBenhVienAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _doiTuongUuDaiDichVuKhamBenhRepository.TableNoTracking
               .Include(p => p.DoiTuongUuDai)
               .Include(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.DichVuKhamBenh)
               //.Include(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.KhoaPhong)
               .Select(s => new DoiTuongUuDaiGridVo
               {
                   Id = s.Id,
                   Ma = s.DichVuKhamBenhBenhVien.Ma,
                   Ma4350 = s.DichVuKhamBenhBenhVien.DichVuKhamBenh == null ? string.Empty : s.DichVuKhamBenhBenhVien.DichVuKhamBenh.MaTT37,
                   DichVuKyThuatId = s.DichVuKhamBenhBenhVienId,
                   Ten = s.DichVuKhamBenhBenhVien.Ten,
                   //TenKhoa = s.DichVuKhamBenhBenhVien.KhoaPhong.Ten,

               }).Distinct();
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ma4350, g => g.Ten);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataForGridBenhVienChildAsync(QueryInfo queryInfo, long parentId = 0)
        {
            BuildDefaultSortExpression(queryInfo);
            //var sortString = RemoveDisplaySort(queryInfo);

            var currentParentId = parentId == 0 ? long.Parse(queryInfo.SearchTerms) : parentId;

            var query = _doiTuongUuDaiDichVuKhamBenhRepository.TableNoTracking.Include(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.DichVuKhamBenh).Include(x => x.DoiTuongUuDai)
                .Where(x => x.DichVuKhamBenhBenhVienId == currentParentId)
                .Select(s => new DoiTuongUuDaiChildGridVo()
                {
                    Id = s.Id,
                    DoiTuong = s.DoiTuongUuDai.Ten,
                    TiLeUuDai = s.TiLeUuDai + "%"
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridBenhVienChildAsync(QueryInfo queryInfo, long parentId = 0)
        {
            BuildDefaultSortExpression(queryInfo);

            var currentParentId = parentId == 0 ? long.Parse(queryInfo.SearchTerms) : parentId;

            var query = _doiTuongUuDaiDichVuKhamBenhRepository.TableNoTracking.Include(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.DichVuKhamBenh).Include(x => x.DoiTuongUuDai)
                 .Where(x => x.DichVuKhamBenhBenhVienId == currentParentId)
                 .Select(s => new DoiTuongUuDaiChildGridVo()
                 {
                     Id = s.Id,
                     DoiTuong = s.DoiTuongUuDai.Ten,
                     TiLeUuDai = s.TiLeUuDai + "%"
                 });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

    }
}
