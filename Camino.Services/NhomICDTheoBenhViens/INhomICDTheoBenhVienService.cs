using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.NhomICDTheoBenhViens
{
    public interface INhomICDTheoBenhVienService : IMasterFileService<NhomICDTheoBenhVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetMaICD(DropDownListRequestModel model, bool coHienThiMa = false);
        Task<List<LookupItemTemplateVo>> GetChuong(DropDownListRequestModel model);
        List<LookupItemTextVo> GetMaTuTaoICD(DropDownListRequestModel model, JsonMaICD maICDs);

        Task<bool> IsTenVietTatTiengVietExists(string tenTiengVietVietTat = null, long nhomICDTheoBenhVienId = 0);
        Task<bool> IsTenVietTatTiengAnhExists(string tenTiengAnhtVietTat = null, long nhomICDTheoBenhVienId = 0);
        Task<bool> IsMaBenhExists(string maBenh = null, long nhomICDTheoBenhVienId = 0);
        Task<bool> IsSTTExists(string stt = null, long nhomICDTheoBenhVienId = 0);
    }
}
