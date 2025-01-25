using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.ValueObject.ICDs;
using Camino.Core.Data;

namespace Camino.Services.ICDs
{
    [ScopedDependency(ServiceType = typeof(IChuanDoanService))]
    public class ChuanDoanService : MasterFileService<ChuanDoan>, IChuanDoanService
    {
        IRepository<ChuanDoanLienKetICD> _chuanDoanLienKetICDrepository;
        IRepository<ICD> _icDrepository;
        IRepository<DanhMucChuanDoan> _danhMucChuanDoanLienKetICDrepository;
        IRepository<ChuanDoan> _chuanDoanrepository;
        public ChuanDoanService(IRepository<ChuanDoan> repository, IRepository<ChuanDoan> chuanDoanrepository, IRepository<ICD> icDrepository, IRepository<ChuanDoanLienKetICD> chuanDoanLienKetIcDrepository, IRepository<DanhMucChuanDoan> danhMucChuanDoanLienKetIcDrepository) : base(repository)
        {
            _chuanDoanLienKetICDrepository = chuanDoanLienKetIcDrepository;
            _danhMucChuanDoanLienKetICDrepository = danhMucChuanDoanLienKetIcDrepository;
            _icDrepository = icDrepository;
            _chuanDoanrepository = chuanDoanrepository;
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            var query = BaseRepository.TableNoTracking
                .Include(x => x.DanhMucChuanDoan)
                .Select(s => new ChuanDoanGridVo
                {

                    Id = s.Id,
                    Ma = s.Ma,
                    TenTiengAnh = s.TenTiengAnh,
                    TenTiengViet = s.TenTiengViet
                });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.TenTiengAnh, g => g.TenTiengViet);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Include(x => x.DanhMucChuanDoan)
                .Select(s => new ChuanDoanGridVo
                {

                    Id = s.Id,
                    Ma = s.Ma,
                    TenTiengAnh = s.TenTiengAnh,
                    TenTiengViet = s.TenTiengViet
                });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.TenTiengAnh, g => g.TenTiengViet);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _chuanDoanLienKetICDrepository.TableNoTracking.Include(x => x.ICD)
                .Where(x => x.ChuanDoanId == long.Parse(queryInfo.SearchTerms))
                .Select(s => new ChuanDoanLienKetGridVo()
                {
                    Id = s.Id,
                    Ma = s.ICD.Ma,
                    TenTiengViet = s.ICD.TenTiengViet,
                    TenTiengAnh = s.ICD.TenTiengAnh,
                    ICDId = s.ICDId,
                    ChuanDoanId = s.ChuanDoanId
                });
            query = query.GroupBy(g => new
            {
                g.ICDId
            }).Select(g => g.First());
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _chuanDoanLienKetICDrepository.TableNoTracking.Include(x => x.ICD)
               .Where(x => x.ChuanDoanId == long.Parse(queryInfo.SearchTerms))
               .Select(s => new ChuanDoanLienKetGridVo()
               {
                   Id = s.Id,
                   Ma = s.ICD.Ma,
                   TenTiengViet = s.ICD.TenTiengViet,
                   TenTiengAnh = s.ICD.TenTiengAnh,
                   ICDId = s.ICDId
               });
            query = query.GroupBy(g => new
            {
                g.ICDId
            }).Select(g => g.First());
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<List<DanhMucGridCombobox>> GetDichVuKhamBenh(DropDownListRequestModel model)
        {
            var lst = await _danhMucChuanDoanLienKetICDrepository.TableNoTracking.Include(x => x.ChuanDoans)
                .Where(p => p.TenTiengViet.Contains(model.Query ?? "") || p.TenTiengAnh.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = lst.Select(item => new DanhMucGridCombobox
            {
                TenTiengViet = item.TenTiengViet,
                TenTiengAnh = item.TenTiengAnh,
                KeyId = item.Id
            }).ToList();

            return query;
        }
        public string GetTenDanhMuc(long id)
        {
            var lst = _danhMucChuanDoanLienKetICDrepository.TableNoTracking.Include(x => x.ChuanDoans).Where(x => x.Id == id)
                .First().TenTiengViet;


            return lst;
        }
        public async Task<List<DanhMucGridCombobox>> GetListChuanDoanIcd(DropDownListRequestModel model)
        {
            var listKhoa = await _icDrepository.TableNoTracking
                 .ApplyLike(model.Query, g => g.Ma, g => g.TenTiengViet)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoa.Select(item => new DanhMucGridCombobox
            {
                KeyId = item.Id,
                TenTiengViet = item.TenTiengViet,
                Ma = item.Ma
            }).ToList();

            return query;
        }
        public ChuanDoan GetChuanDoanLast()
        {
            var chuan = _chuanDoanrepository.TableNoTracking.Include(x => x.ChuanDoanLienKetICDs).Last();
            return chuan;
        }
        public List<DanhMucChuanDoan> GetDanhMucChuanDoan(long id)
        {
            var chuan = _danhMucChuanDoanLienKetICDrepository.TableNoTracking.Where(x => x.Id.Equals(id)).ToList();
            return chuan;
        }

        public async Task<List<LookupItemTextVo>> GetListChuanDoanTheoMaBenh(DropDownListRequestModel model)
        {
            var listKhoa = await _icDrepository.TableNoTracking
                 .ApplyLike(model.Query, g => g.Ma, g => g.TenTiengViet)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoa.Select(item => new LookupItemTextVo
            {
                KeyId = item.Ma,
                DisplayName = item.Ma + "-" + item.TenTiengViet
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemTextVo>> GetListChuanDoanTheoTenBenh(DropDownListRequestModel model)
        {
            var listKhoa = await _icDrepository.TableNoTracking
                 .ApplyLike(model.Query, g => g.Ma, g => g.TenTiengViet)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoa.Select(item => new LookupItemTextVo
            {
                KeyId = item.TenTiengViet,
                DisplayName = item.Ma + "-" + item.TenTiengViet
            }).ToList();

            return query;
        }

        public async Task<bool> IsTenViExists(string tenVi = null, long id = 0)
        {
            var result = false;
            if (tenVi != null)
            {
                if (id == 0)
                {
                    result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengViet.ToLower().TrimEnd().TrimStart().Equals(tenVi.ToLower().TrimEnd().TrimStart()));
                }
                else
                {
                    result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengViet.ToLower().TrimEnd().TrimStart().Equals(tenVi.ToLower().TrimEnd().TrimStart()) && p.Id != id);
                }
            }

            if (result)
                return false;
            return true;
        }
        public async Task<bool> IsTenEngExists(string tenEng = null, long id = 0)
        {
            var result = false;
            if (tenEng != null)
            {
                if (id == 0)
                {
                    result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengAnh.ToLower().TrimEnd().TrimStart().Equals(tenEng.ToLower().TrimEnd().TrimStart()));
                }
                else
                {
                    result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengAnh.ToLower().TrimEnd().TrimStart().Equals(tenEng.ToLower().TrimEnd().TrimStart()) && p.Id != id);
                }
            }

            if (result)
                return false;
            return true;
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
            if (result)
                return false;
            return true;
        }
    }
}
