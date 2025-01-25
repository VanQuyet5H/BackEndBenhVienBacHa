using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Messages;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.Messages
{
    [ScopedDependency(ServiceType = typeof(ILichSuEmailService))]
    public class LichSuEmailService : MasterFileService<Core.Domain.Entities.Messages.LichSuEmail>, ILichSuEmailService
    {
        public LichSuEmailService(IRepository<Core.Domain.Entities.Messages.LichSuEmail> repository) : base(repository)
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

            var query = BaseRepository.TableNoTracking.Select(s => new LichSuEmailGrid()
            {
                Id = s.Id,
                GoiDen = s.GoiDen,
                TieuDe = s.TieuDe,
                NoiDung = s.NoiDung,
                TapTinDinhKem = s.TapTinDinhKem,
                urlTapTinDinhKem = s.TapTinDinhKem,
                TenTrangThai = s.TrangThai.GetDescription(),
                NgayGuiFormat = s.NgayGui.ApplyFormatDateTimeSACH(),
                NgayGui = s.NgayGui,
                TrangThai = s.TrangThai
            });

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                query = query.ApplyLike(queryInfo.SearchTerms, g => g.GoiDen, g => g.TieuDe, g => g.NoiDung, g => g.TapTinDinhKem);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<LichSuEmailGrid>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.GoiDen))
                {
                    query = query.Where(p => p.GoiDen.ToLower()
                                                 .Contains(queryString.GoiDen.ToLower().TrimEnd().TrimStart()));
                }
                if (queryString.TrangThai != 0)
                {
                    query = query.Where(p => p.TrangThai == queryString.TrangThai);
                }
                if (!string.IsNullOrEmpty(queryString.TieuDe))
                {
                    query = query.Where(p => p.TieuDe.ToLower()
                                                 .Contains(queryString.TieuDe.ToLower().TrimEnd().TrimStart()));
                }
                if (!string.IsNullOrEmpty(queryString.TuNgay) || !string.IsNullOrEmpty(queryString.DenNgay))
                {
                    var tuNgayTemp = string.IsNullOrEmpty(queryString.TuNgay)
                        ? new DateTime().Date
                        : Convert.ToDateTime(queryString.TuNgay);
                    var denNgayTemp = string.IsNullOrEmpty(queryString.DenNgay)
                        ? DateTime.Now.Date
                        : Convert.ToDateTime(queryString.DenNgay);

                    query = query.Where(p => p.NgayGui.Value.Date >= tuNgayTemp && p.NgayGui.Value.Date <= denNgayTemp);
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
            var query = BaseRepository.TableNoTracking.Select(s => new LichSuEmailGrid()
            {
                Id = s.Id,
                GoiDen = s.GoiDen,
                TieuDe = s.TieuDe,
                NoiDung = s.NoiDung,
                TapTinDinhKem = s.TapTinDinhKem,
                urlTapTinDinhKem = s.TapTinDinhKem,
                TenTrangThai = s.TrangThai.GetDescription(),
                NgayGuiFormat = s.NgayGui.ApplyFormatDateTime(),
                NgayGui = s.NgayGui,
                TrangThai = s.TrangThai
            });

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                query = query.ApplyLike(queryInfo.SearchTerms, g => g.GoiDen, g => g.TieuDe, g => g.NoiDung, g => g.TapTinDinhKem);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<LichSuEmailGrid>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.GoiDen))
                {
                    query = query.Where(p => p.GoiDen.ToLower()
                                                 .Contains(queryString.GoiDen.ToLower().TrimEnd().TrimStart()));
                }
                if (queryString.TrangThai != 0)
                {
                    query = query.Where(p => p.TrangThai == queryString.TrangThai);
                }
                if (!string.IsNullOrEmpty(queryString.TieuDe))
                {
                    query = query.Where(p => p.TieuDe.ToLower()
                                                 .Contains(queryString.TieuDe.ToLower().TrimEnd().TrimStart()));
                }
                if (!string.IsNullOrEmpty(queryString.TuNgay) || !string.IsNullOrEmpty(queryString.DenNgay))
                {
                    var tuNgayTemp = string.IsNullOrEmpty(queryString.TuNgay)
                        ? new DateTime().Date
                        : Convert.ToDateTime(queryString.TuNgay);
                    var denNgayTemp = string.IsNullOrEmpty(queryString.DenNgay)
                        ? DateTime.Now.Date
                        : Convert.ToDateTime(queryString.DenNgay);

                    query = query.Where(p => p.NgayGui.Value.Date >= tuNgayTemp && p.NgayGui.Value.Date <= denNgayTemp);
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
    }
}
