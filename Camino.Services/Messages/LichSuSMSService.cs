using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.Messages;
using Newtonsoft.Json;

namespace Camino.Services.Messages
{
    [ScopedDependency(ServiceType = typeof(ILichSuSMSService))]
    public class LichSuSMSService : MasterFileService<Core.Domain.Entities.Messages.LichSuSMS>, ILichSuSMSService
    {
        public LichSuSMSService(IRepository<Core.Domain.Entities.Messages.LichSuSMS> repository) : base(repository)
        {

        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Select(x => new LichSuSMSGrid()
            {
                GoiDen = x.GoiDen.ApplyFormatPhone(),
                NoiDung = x.NoiDung,
                TenTrangThai = x.TrangThai.GetDescription(),
                NgayGui = x.NgayGui.ApplyFormatDateTimeSACH(),
                TrangThai = x.TrangThai,
                Ngay = x.NgayGui

            });

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                query = query.ApplyLike(queryInfo.SearchTerms, g => g.GoiDen, g => g.NoiDung, g => g.TenTrangThai, g => g.NgayGui);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<LichSuSMSGrid>(queryInfo.AdditionalSearchString);
              
                if (queryString.GoiDen != null)
                {
                    query = query.Where(p => p.GoiDen != null && p.GoiDen.ToLower()
                                                 .Contains(queryString.GoiDen.ToLower().TrimEnd().TrimStart().ApplyFormatPhone()));
                }
                if (queryString.TrangThai != 0)
                {
                    query = query.Where(p => p.TrangThai == queryString.TrangThai);
                }
                if (queryString.NoiDung != null)
                {
                    query = query.Where(p => p.NoiDung != null && p.NoiDung.ToLower()
                                                 .Contains(queryString.NoiDung.ToLower().TrimEnd().TrimStart()));
                }
                if (queryString.TuNgay != null&& queryString.DenNgay != null)
                {
                    var a = Convert.ToDateTime(queryString.TuNgay);
                    var b = Convert.ToDateTime(queryString.DenNgay).AddDays(+1);
                    query = query.Where(p => ( p.Ngay>=a ));
                    query = query.Where(p => p.Ngay <= b);
                }
                if (queryString.TuNgay != null && queryString.DenNgay == null)
                {
                    var a = Convert.ToDateTime(queryString.TuNgay);
                    query = query.Where(p => (p.Ngay >= a));
                    query = query.Where(p => p.Ngay <= a.AddDays(+1));
                }
                if (queryString.TuNgay == null && queryString.DenNgay != null)
                {
                    var b = Convert.ToDateTime(queryString.DenNgay);
                    query = query.Where(p => (p.Ngay >= b));
                    query = query.Where(p => p.Ngay <= b.AddDays(+1));
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(x => new LichSuSMSGrid()
            {
                GoiDen = x.GoiDen.ApplyFormatPhone(),
                NoiDung = x.NoiDung,
                TenTrangThai = x.TrangThai.GetDescription(),
                NgayGui = x.NgayGui.ApplyFormatDateTime(),
                TrangThai = x.TrangThai,
                Ngay = x.NgayGui
            });
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                query = query.ApplyLike(queryInfo.SearchTerms, g => g.GoiDen, g => g.NoiDung, g => g.TenTrangThai, g => g.NgayGui);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<LichSuSMSGrid>(queryInfo.AdditionalSearchString);

                if (queryString.GoiDen != null)
                {
                    query = query.Where(p => p.GoiDen != null && p.GoiDen.ToLower()
                                                 .Contains(queryString.GoiDen.ToLower().TrimEnd().TrimStart().ApplyFormatPhone()));
                }
                if (queryString.TrangThai != 0)
                {
                    query = query.Where(p => p.TrangThai == queryString.TrangThai);
                }
                if (queryString.NoiDung != null)
                {
                    query = query.Where(p => p.NoiDung != null && p.NoiDung.ToLower()
                                                 .Contains(queryString.NoiDung.ToLower().TrimEnd().TrimStart()));
                }
                if (queryString.TuNgay != null && queryString.DenNgay != null)
                {
                    var a = Convert.ToDateTime(queryString.TuNgay);
                    var b = Convert.ToDateTime(queryString.DenNgay).AddDays(+1);
                    query = query.Where(p => (p.Ngay >= a));
                    query = query.Where(p => p.Ngay <= b);
                }
                if (queryString.TuNgay != null && queryString.DenNgay == null)
                {
                    var a = Convert.ToDateTime(queryString.TuNgay);
                    query = query.Where(p => (p.Ngay >= a));
                    query = query.Where(p => p.Ngay <= a.AddDays(+1));
                }
                if (queryString.TuNgay == null && queryString.DenNgay != null)
                {
                    var b = Convert.ToDateTime(queryString.DenNgay);
                    query = query.Where(p => (p.Ngay >= b));
                    query = query.Where(p => p.Ngay <= b.AddDays(+1));
                }
            }
            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public List<LookupItemVo> GetTrangThai()
        {
            var lstDocumentEnums = Enum.GetValues(typeof(Enums.TrangThaiLishSu)).Cast<Enum>();


            var query = lstDocumentEnums.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
               .ToList();
            return query;
        }
    }
}
