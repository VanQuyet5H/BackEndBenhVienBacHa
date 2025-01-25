using Camino.Core.Domain;
using Camino.Core.Domain.Entities.QuocGias;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuocGia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.QuocGias
{
    public interface IQuocGiaService : IMasterFileService<QuocGia>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        List<ChauLucVo> GetListChauLuc(DropDownListRequestModel model);
        Task<QuocGiaRelationships> GetQuocGiaRelationshipsById(long id);
        Task<IEnumerable<QuocGiaRelationships>> GetQuocGiaRelationshipsByIds(long[] ids);
        Task<bool> IsMaExists(string ma = null, long quocGiaId = 0);
        Task<bool> IsTenVietTatExists(string tenVietTat = null, long quocGiaId = 0);
        bool CheckChauLuc(Enums.EnumChauLuc param);
        string GetTenChauLuc(Enums.EnumChauLuc param);
        bool ContainNumber(string param);
    }
}
