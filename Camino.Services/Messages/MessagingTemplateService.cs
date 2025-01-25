using System;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Camino.Core.Helpers;
using Camino.Core.Data;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.Entities.Messages;
using Camino.Core.Domain.ValueObject.Template;


namespace Camino.Services.Messages
{
    [ScopedDependency(ServiceType = typeof(IMessagingTemplateService))]
    public class MessagingTemplateService : MasterFileService<MessagingTemplate>, IMessagingTemplateService
    {
        public MessagingTemplateService(IRepository<MessagingTemplate> repository) : base(repository)
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

            var query = BaseRepository.TableNoTracking.Select(s => new MesagingTemplateGridVo
                      {
                          Id = s.Id,
                          PhienBan = (int)s.MessagePriority,
                          LoaiTemplate = s.MessagingType,
                          Ten = s.Name,
                          TieuDe = s.Title,
                          DateUpdate = s.LastTime.GetValueOrDefault(),
                          LastTime = s.LastTime,
                          LoaiTemplateText = s.MessagingType.GetDescription(),
                          DateUpdateText = s.LastTime.GetValueOrDefault().ToString("MM/dd/yyyy"),
                          NoiDung = s.Body,
                          HoatDong = s.IsDisabled,
                          Description = s.Description
                      });
             query = query.ApplyLike(queryInfo.SearchTerms, g => g.Description);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking.Select(s => new MesagingTemplateGridVo
            {
                Id = s.Id,
                PhienBan = (int)s.MessagePriority,
                LoaiTemplate = s.MessagingType,
                Ten = s.Name,
                TieuDe = s.Title,
                DateUpdate = s.LastTime.GetValueOrDefault(),
                LastTime = s.LastTime,
                LoaiTemplateText = s.MessagingType.GetDescription(),
                DateUpdateText = s.LastTime.GetValueOrDefault().ToString("MM/dd/yyyy"),
                NoiDung = s.Body,
                HoatDong = s.IsDisabled,
                Description = s.Description
            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Description);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
    }
}
