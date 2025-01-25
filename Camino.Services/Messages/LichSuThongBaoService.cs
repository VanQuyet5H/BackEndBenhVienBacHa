using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Messages;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Messages;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.Messages
{
    [ScopedDependency(ServiceType = typeof(ILichSuThongBaoService))]
    public class LichSuThongBaoService : MasterFileService<LichSuThongBao>, ILichSuThongBaoService
    {
        public LichSuThongBaoService(IRepository<LichSuThongBao> repository) : base(repository)
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

            var result = BaseRepository.TableNoTracking.Select(s => new LichSuThongBaoGripVo
            {
                Id = s.Id,
                GoiDen = s.GoiDen,
                NoiDung = s.NoiDung,
                TenTrangThai = s.TrangThai.GetDescription(),
                TrangThai = s.TrangThai,
                NgayGui = s.CreatedOn != null ? s.CreatedOn.GetValueOrDefault().ApplyFormatDateTimeSACH() : string.Empty,
                NgayGuiDate = s.CreatedOn
            });

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                result = result.ApplyLike(queryInfo.SearchTerms, g => g.GoiDen, g => g.NoiDung, g => g.NgayGui);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<LichSuThongBaoGripVo>(queryInfo.AdditionalSearchString);

                if (queryString.GoiDen != null)
                {
                    result = result.Where(p => p.GoiDen != null && p.GoiDen.ToLower()
                                                 .Contains(queryString.GoiDen.ToLower().TrimEnd().TrimStart()));
                }
                if (queryString.TrangThai != 0)
                {
                    result = result.Where(p => p.TrangThai == queryString.TrangThai);
                }
                if (queryString.NoiDung != null)
                {
                    result = result.Where(p => p.NoiDung != null && p.NoiDung.ToLower()
                                                 .Contains(queryString.NoiDung.ToLower().TrimEnd().TrimStart()));
                }

                if (queryString.NgayGuiTu == null || queryString.NgayGuiDen == null)
                {
                    var tuNgayTemp = queryString.NgayGuiTu ?? new DateTime().Date;
                    var denNgayTemp = queryString.NgayGuiDen ?? DateTime.Now.Date;

                    result = result.Where(p => p.NgayGuiDate.Value.Date >= tuNgayTemp && p.NgayGuiDate.Value.Date <= denNgayTemp);
                }
            }


            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : result.CountAsync();
            var queryTask = result.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var result = BaseRepository.TableNoTracking.Select(s => new LichSuThongBaoGripVo
            {
                Id = s.Id,
                GoiDen = s.GoiDen,
                NoiDung = s.NoiDung,
                TenTrangThai = s.TrangThai.GetDescription(),
                TrangThai = s.TrangThai,
                NgayGui = s.CreatedOn != null ? s.CreatedOn.GetValueOrDefault().ApplyFormatDateTime() : string.Empty,
                NgayGuiDate = s.CreatedOn
            });

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                result = result.ApplyLike(queryInfo.SearchTerms, g => g.GoiDen, g => g.NoiDung, g => g.NgayGui);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<LichSuThongBaoGripVo>(queryInfo.AdditionalSearchString);

                if (queryString.GoiDen != null)
                {
                    result = result.Where(p => p.GoiDen != null && p.GoiDen.ToLower()
                                                 .Contains(queryString.GoiDen.ToLower().TrimEnd().TrimStart()));
                }
                if (queryString.TrangThai != 0)
                {
                    result = result.Where(p => p.TrangThai == queryString.TrangThai);
                }
                if (queryString.NoiDung != null)
                {
                    result = result.Where(p => p.NoiDung != null && p.NoiDung.ToLower()
                                                 .Contains(queryString.NoiDung.ToLower().TrimEnd().TrimStart()));
                }

                if (queryString.NgayGuiTu == null || queryString.NgayGuiDen == null)
                {
                    var tuNgayTemp = queryString.NgayGuiTu ?? new DateTime().Date;
                    var denNgayTemp = queryString.NgayGuiDen ?? DateTime.Now.Date;

                    result = result.Where(p => p.NgayGuiDate.Value.Date >= tuNgayTemp && p.NgayGuiDate.Value.Date <= denNgayTemp);
                }
            }

            var countTask = result.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public List<LookupItemVo> GetTrangThai()
        {
            
            var list = Enum.GetValues(typeof(Enums.TrangThaiLishSu)).Cast<Enum>();
            var result = list.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return result;

        }

    }
}
