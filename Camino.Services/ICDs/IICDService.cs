using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ICDs;

namespace Camino.Services.ICDs
{
    public interface IICDService : IMasterFileService<ICD>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<QuanLyICDTemplateVo>> GetTenLoaiICD(DropDownListRequestModel queryInfo);
        Task<List<KhoaQuanLyICDTemplateVo>> GetTenKhoa(DropDownListRequestModel queryInfo);
        Task<bool> CheckLoaiICDExist(long? id);
        Task<bool> CheckKhoaExist(long? id);
        Task<bool> IsMaExist(string ma = null, long Id = 0);
        Task<bool> IsTenExist(string ten = null, long Id = 0);

        Task<string> GetMaICD(long id);
    }
}
