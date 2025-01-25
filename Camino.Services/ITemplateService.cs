using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services
{
    public interface ITemplateService : IMasterFileService<Template>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<int> GetLastVersionOfTemplateAsync(string tieude, int ngonngu);
        void AddTemplate(Template template);
        Template GetByName(string tieude, Enums.LanguageType ngonngu, Enums.TemplateType loaiTemplate);

    }
}
