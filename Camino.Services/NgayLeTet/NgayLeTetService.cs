using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.NgheNghiep;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Camino.Core.Data;
using Camino.Data;
using Camino.Core.Domain.ValueObject.NgayLeTet;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Camino.Services.NgayLeTet
{
    [ScopedDependency(ServiceType = typeof(INgayLeTetService))]
    public class NgayLeTetService : MasterFileService<Core.Domain.Entities.CauHinhs.NgayLeTet>, INgayLeTetService
    {
        public NgayLeTetService(IRepository<Core.Domain.Entities.CauHinhs.NgayLeTet> repository) : base(repository)
        {

        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new NgayLeTetSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NgayLeTetSearch>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking.Select(item => new NgayLeTetGridVo
            {
                Id = item.Id,
                Ten = item.Ten,
                Ngay = item.Ngay,
                Thang = item.Thang,
                Nam = item.Nam,
                LeHangNam = item.LeHangNam,
                GhiChu = item.GhiChu
            });

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.Ten))
                {
                    query = query.ApplyLike(queryObject.Ten, p => p.Ten);
                }

                if (!string.IsNullOrEmpty(queryObject.Nam.ToString()))
                {
                    query = query.ApplyLike(queryObject.Nam.ToString(), p => p.Nam.ToString());
                }
            }

            var queryTask = query.OrderBy(queryInfo.SortString)
                             .Skip(queryInfo.Skip)
                             .Take(queryInfo.Take)
                             .ToArrayAsync();

            await Task.WhenAll(queryTask);

            return new GridDataSource { Data = queryTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryObject = new NgayLeTetSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NgayLeTetSearch>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking.Select(item => new NgayLeTetGridVo { Ten = item.Ten, Nam = item.Nam });
            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.Ten))
                {
                    query = query.ApplyLike(queryObject.Ten, p => p.Ten);
                }

                if (!string.IsNullOrEmpty(queryObject.Nam.ToString()))
                {
                    query = query.ApplyLike(queryObject.Nam.ToString(), p => p.Nam.ToString());
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public List<LookupItemTextVo> GetNamSreachs(DropDownListRequestModel model, NamSearch nam)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listIcd = NamSreachs().Where(p => p.DisplayName.Contains(model.Query ?? "") || p.KeyId.Contains(model.Query ?? ""))
                                            .Take(model.Take)
                                            .ToList();

                if (!string.IsNullOrEmpty(nam.Nam))
                {
                    var listMaICD = nam.Nam.Split(";");
                    var modelResult = NamSreachs().Where(p => listMaICD.Select(c => c).Contains(p.KeyId));
                    listIcd.AddRange(modelResult);
                }

                var query = listIcd.Select(item => new LookupItemTextVo
                {
                    DisplayName = item.DisplayName,
                    KeyId = item.KeyId
                }).ToList();

                return query;
            }
            else
            {

                var listIcd = NamSreachs().Take(model.Take).ToList();

                if (!string.IsNullOrEmpty(nam.Nam))
                {
                    var listMaICD = nam.Nam.Split(";");
                    var modelResult = NamSreachs().Where(p => listMaICD.Select(c => c).Contains(p.KeyId));
                    listIcd.AddRange(modelResult);
                }

                var query = listIcd.Select(item => new LookupItemTextVo
                {
                    DisplayName = item.DisplayName,
                    KeyId = item.KeyId
                }).ToList();

                return query;
            }
        }

        public List<LookupItemTextVo> NamSreachs()
        {
            var nams = new List<LookupItemTextVo>();

            int nam = 2020;
            int counts = 50;
            for (int i = 0; i < counts; i++)
            {
                var icd = new LookupItemTextVo();

                icd.KeyId = (nam + i).ToString();
                icd.DisplayName = (nam + i).ToString();
                nams.Add(icd);
            }
            return nams;
        }
    }
}
