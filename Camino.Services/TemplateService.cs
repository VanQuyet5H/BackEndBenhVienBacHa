using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.Template;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;

namespace Camino.Services
{
    [ScopedDependency(ServiceType = typeof(ITemplateService))]
    public class TemplateService : MasterFileService<Template>, ITemplateService
    {
        public TemplateService(IRepository<Template> repository) : base(repository)
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

            var query = BaseRepository.TableNoTracking.Select(s => new TemplateGridVo
            {
                Id = s.Id,
                PhienBan = (int)s.Version,
                LoaiTemplate = s.TemplateType,
                Ten = s.Name,
                TieuDe = s.Title,
                DateUpdate = s.LastTime.GetValueOrDefault(),
                LastTime = s.LastTime,
                DateUpdateText = s.LastTime.GetValueOrDefault().ApplyFormatDate(),
                NoiDung = s.Body,
                HoatDong = s.IsDisabled,
                Description = s.Description
            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Description);
            query = query.Select(s => new TemplateGridVo
            {
                Id = s.Id,
                PhienBan = s.PhienBan,
                LoaiTemplate = s.LoaiTemplate,
                Ten = s.Ten,
                TieuDe = s.TieuDe,
                DateUpdate = s.LastTime.GetValueOrDefault(),
                LastTime = s.LastTime,
                DateUpdateText = s.LastTime.GetValueOrDefault().ApplyFormatDate(),
                NoiDung = s.NoiDung,
                HoatDong = s.HoatDong,
                Description = s.Description
            });

            var additional = queryInfo.AdditionalSearchString;
            int templateType = 0;
            if (additional != null) 
                templateType = Int32.Parse(additional);
            if (templateType != 0)
            {
                query = query.Where(s => (int)s.LoaiTemplate == templateType);
            }

            var allTemplate = query.GroupBy(o => new { Ten = o.Ten, NgonNgu = o.NgonNgu })
                  .Select(p => p.OrderByDescending(q => q.DateUpdate).FirstOrDefault());

            //var allTemplate = BaseRepository.Table.AsNoTracking().GroupBy(o => new { Ten = o.Name, NgonNgu = o.Language })
            //      .Select(p => p.OrderByDescending(q => q.Version).FirstOrDefault());
           

            var temp = from entity in allTemplate select new { entity };
            var queryResult = temp.Select(s => new TemplateGridVo
            {
                Id = s.entity.Id,
                PhienBan = s.entity.PhienBan,
                LoaiTemplate = s.entity.LoaiTemplate,
                Ten = s.entity.Ten,
                TieuDe = s.entity.TieuDe,
                DateUpdate = s.entity.LastTime.GetValueOrDefault(),
                LoaiTemplateText = s.entity.LoaiTemplateText,
                DateUpdateText = s.entity.LastTime.GetValueOrDefault().ApplyFormatDate(),
                NoiDung = s.entity.NoiDung,
                HoatDong = s.entity.HoatDong,
                Description = s.entity.Description,
                LastTime = s.entity.LastTime
            });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = queryResult.OrderBy((queryInfo.SortString == "Description asc"? "DateUpdate desc" :  queryInfo.SortString)).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data =  queryTask.Result, TotalRowCount = countTask.Result };
        }


        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking.Select(s => new TemplateGridVo
            {
                Id = s.Id,
                PhienBan = (int)s.Version,
                LoaiTemplate = s.TemplateType,
                Ten = s.Name,
                TieuDe = s.Title,
                DateUpdate = s.LastTime.GetValueOrDefault(),
                LastTime = s.LastTime,
                DateUpdateText = s.LastTime.GetValueOrDefault().ApplyFormatDate(),
                NoiDung = s.Body,
                HoatDong = s.IsDisabled,
                Description = s.Description
            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Description);
            //BuildDefaultSortExpression(queryInfo);
            ////var additional = queryInfo.AdditionalSearchString;
            ////int templateType = 0;
            ////if (additional != null)
            ////    templateType = Int32.Parse(additional);


            //var allTemplate = BaseRepository.Table.AsNoTracking().GroupBy(o => new { Ten = o.Name, NgonNgu = o.Language })
            //      .Select(p => p.OrderByDescending(q => q.Version).FirstOrDefault());
            //allTemplate = allTemplate.ApplyLike(queryInfo.SearchTerms, g => g.Description);
            ////if (templateType != 0)
            ////{
            ////    allTemplate = allTemplate.Where(s => (int)s.TemplateType == templateType);
            ////}
            //var temp = from entity in allTemplate select new { entity };
            //var query = temp.Select(s => new TemplateGridVo
            //{
            //    Id = s.entity.Id,
            //    PhienBan = s.entity.Version,
            //    LoaiTemplate = s.entity.TemplateType,
            //    Ten = s.entity.Name,
            //    TieuDe = s.entity.Title,
            //    DateUpdate = s.entity.LastTime.GetValueOrDefault(),
            //    LoaiTemplateText = s.entity.TemplateType.GetDescription(),
            //    DateUpdateText = s.entity.LastTime.GetValueOrDefault().ToString("MM/dd/yyyy"),
            //    NoiDung = s.entity.Body,
            //    HoatDong = s.entity.IsDisabled,
            //    Description = s.entity.Description,
            //    LastTime = s.entity.LastTime
            //});
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.Description);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<int> GetLastVersionOfTemplateAsync(string tieude, int ngonngu)
        {
            var template = await BaseRepository.Table
                .OrderByDescending(k => k.Version)
                .FirstOrDefaultAsync(o => o.Name == tieude && o.Language == ngonngu);
            var phienban = template.Version;
            return phienban;
        }

        public void  AddTemplate(Template template)
        {
            var temps =  BaseRepository.Table.AsNoTracking().Where(o => o.Name == template.Name && o.Language == template.Language).OrderByDescending(o=> o.Version).ToList();
            var temp = temps.First();
            if(temp!= null)
            {
                template.Version = temp.Version + 1;
                template.Id = 0;
                var entity = new Template
                {
                    TemplateType = template.TemplateType,
                    Language = template.Language,
                    Body = template.Body,
                    Version = template.Version,
                    Name = template.Name,
                    Title = template.Title,

                };
                BaseRepository.Add(entity);
            }
           
        }

        public Template GetByName(string ten, Enums.LanguageType ngonngu, Enums.TemplateType loaiTemplate)
        {
            return BaseRepository.Table.AsNoTracking().OrderByDescending(k => k.Version).FirstOrDefault(o => o.Name == ten && o.Language == (int)ngonngu && o.TemplateType== loaiTemplate);
        }

    }
}
