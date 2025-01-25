using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ICDs;
using Camino.Core.Domain.ValueObject.NhomDichVuBenhVien;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.NhomDichVuBenhVien
{
    [ScopedDependency(ServiceType = typeof(INhomDichVuBenhVienService))]
    public class NhomDichVuBenhVienService : MasterFileService<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien>, INhomDichVuBenhVienService
    {
        public NhomDichVuBenhVienService(
            IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> repository) : base(repository)
        { }

        //public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        //{
        //    if(exportExcel)
        //    {
        //        queryInfo.Skip = 0;
        //        queryInfo.Take = int.MaxValue;
        //    }

        //    var searchString = queryInfo.SearchTerms.Trim();

        //    var query = BaseRepository.TableNoTracking
        //        .Where(p => p.Ten.Contains(searchString) || p.Ma.Contains(searchString)
        //        )
        //        .Select(s => new NhomDichVuBenhVienGridVo
        //        {
        //            Id = s.Id,
        //            NhomDichVuBenhVienCha = s.NhomDichVuBenhVienCha != null ? s.NhomDichVuBenhVienCha.Ten : null,
        //            Ten = s.Ten,
        //            Ma = s.Ma,
        //            MoTa = s.MoTa,
        //            IsDefault=s.IsDefault
        //        }).ApplyLike(queryInfo.SearchTerms,
        //            g => g.Ma,
        //            g => g.Ten,
        //            g => g.MoTa);

        //    var countTask = queryInfo.LazyLoadPage == true ?
        //        Task.FromResult(0) :
        //        query.CountAsync();
        //    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
        //        .Take(queryInfo.Take).ToArrayAsync();

        //    await Task.WhenAll(countTask, queryTask);

        //    return new GridDataSource
        //    {
        //        Data = queryTask.Result,
        //        TotalRowCount = countTask.Result
        //    };
        //}

        //public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        //{
        //    var searchString = queryInfo.SearchTerms.Trim();

        //    var query = BaseRepository.TableNoTracking
        //        .Where(p => p.Ten.Contains(searchString) || p.Ma.Contains(searchString)
        //        )
        //        .Select(s => new NhomDichVuBenhVienGridVo
        //        {
        //            Id = s.Id,
        //            Ten = s.Ten,
        //            Ma = s.Ma,
        //            MoTa = s.MoTa,
        //            IsDefault = s.IsDefault
        //        }).ApplyLike(queryInfo.SearchTerms,
        //            g => g.Ma,
        //            g => g.Ten,
        //            g => g.MoTa);

        //    var countTask = query.CountAsync();

        //    await Task.WhenAll(countTask);

        //    return new GridDataSource
        //    {
        //        TotalRowCount = countTask.Result
        //    };
        //}

        public async Task<List<NhomDichVuBenhVienGridCombobox>> GetDichVuKhamBenh(DropDownListRequestModel model)
        {
            var nhomDichVuBenhVienId = long.Parse(model.ParameterDependencies);
            var nhomDichVuBenhViens = await BaseRepository.TableNoTracking
                .Where(p => p.Id != nhomDichVuBenhVienId)
                .Select(item => new NhomDichVuBenhVienGridCombobox
                {
                    DisplayName = item.Ten,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    KeyId = item.Id
                })
                .Union(
                    BaseRepository.TableNoTracking
                    .Where(p => p.Id == nhomDichVuBenhVienId)
                     .Select(item => new NhomDichVuBenhVienGridCombobox
                     {
                         DisplayName = item.Ten,
                         Ma = item.Ma,
                         Ten = item.Ten,
                         KeyId = item.Id
                     })
                )
                .ApplyLike(model.Query, s => s.Ma, s => s.Ten)
                .Take(model.Take)
                .ToListAsync();
            return nhomDichVuBenhViens;
        }

        public async Task<bool> CheckChiDinhVong(long id, long? nhomDichVuChaId)
        {
            if (nhomDichVuChaId == null)
            {
                return true;
            }

            long? nhomChaId;

            do
            {
                nhomChaId = await BaseRepository.TableNoTracking
                    .Where(p => p.Id == nhomDichVuChaId.GetValueOrDefault())
                    .Select(p => p.NhomDichVuBenhVienChaId)
                    .FirstOrDefaultAsync();

                if (nhomChaId == id)
                {
                    return false;
                }

                nhomDichVuChaId = nhomChaId.GetValueOrDefault();
            } while (nhomChaId != null);

            return true;
        }

        public List<NhomDichVuBenhVienTreeViewGridVo> NhomDichVuBenhVienTreeViewChas(QueryInfo queryInfo)
        {
            var list = new List<NhomDichVuBenhVienTreeViewGridVo>();

            var searchString = string.Empty;
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                searchString = queryInfo.SearchTerms.RemoveVietnameseDiacritics().ToLower().Trim();
            }
            var nhomQuery = BaseRepository.TableNoTracking
                .Where(p => p.NhomDichVuBenhVienChaId == null
                            && (queryInfo.SearchTerms == null
                            || p.Ten.ToLower().Contains(searchString)
                            || p.NhomDichVuBenhViens.Any(o => o.Ten.RemoveVietnameseDiacritics().ToLower().Contains(searchString)
                                                            || o.Ma.RemoveVietnameseDiacritics().ToLower().Contains(searchString)))
                            )
                .OrderBy(p => p.Id)
                .Select(s => new NhomDichVuBenhVienTreeViewGridVo
                {
                    Id = s.Id,
                    IdCap = (s.Id + ";nhomCha;" + 1).ToString(),
                    Ma = s.Ma,
                    Ten = s.Ten,
                    CapNhomDichVuBenhVien = 1,
                    NhomDichVuBenhVienChaId = s.Id,
                    HasChildren = s.NhomDichVuBenhViens.Any(),
                    IsDefault = s.IsDefault
                });
            if (nhomQuery.Any())
            {
                list.AddRange(nhomQuery.ToList());
            }
            return list;
        }

        public List<NhomDichVuBenhVienTreeViewGridVo> NhomDichVuBenhVienTreeViewCons(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new List<NhomDichVuBenhVienTreeViewGridVo>();
            }
            var queryString = JsonConvert.DeserializeObject<NhomDichVuBenhVienTreeViewGridVo>(queryInfo.AdditionalSearchString);

            var query = BaseRepository.TableNoTracking
                .Where(p => p.NhomDichVuBenhVienChaId == queryString.NhomDichVuBenhVienChaId
                            )
                .OrderBy(p => p.Id)
                .Select(s => new NhomDichVuBenhVienTreeViewGridVo
                {
                    Id = s.Id,
                    IdCap = (s.Id + ";nhomCon;" + 2).ToString(),
                    Ma = s.Ma,
                    Ten = s.Ten,
                    NhomDichVuBenhVienChaId = s.NhomDichVuBenhVienChaId,
                    CapNhomDichVuBenhVien = 2,
                    HasChildren = s.NhomDichVuBenhViens.Any(),
                    IsDefault = s.IsDefault
                });
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                queryString.SearchString = queryString.SearchString.RemoveVietnameseDiacritics().ToLower().Trim();
                query = query.Where(p => p.Ten.RemoveVietnameseDiacritics().ToLower().Contains(queryString.SearchString) || p.Ma.RemoveVietnameseDiacritics().ToLower().Contains(queryString.SearchString));
            }
            return query.ToList();
        }

    }
}
