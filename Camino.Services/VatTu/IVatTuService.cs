using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.VatTu;

namespace Camino.Services.VatTu
{
    public interface IVatTuService : IMasterFileService<Core.Domain.Entities.VatTus.VatTu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<NhomVatTuTreeViewVo>> GetListNhomVatTuAsync(DropDownListRequestModel model);
        Task<bool> CheckMaVatTuBenhVien(string maVatTuBenhVien);
    }
}
