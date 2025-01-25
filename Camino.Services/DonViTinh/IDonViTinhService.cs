using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DonViTinh;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.DonViTinh
{
    public interface IDonViTinhService : IMasterFileService<Core.Domain.Entities.DonViTinhs.DonViTinh>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo,bool isPrint);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<bool> IsTenExists(string ten, long id = 0);
        Task<bool> IsMaExists(string ma, long id = 0);

        #region hàm dùng chung

        Task<ICollection<DonViTinhTemplateVo>> GetDanhSachDonViTinhAsync(DropDownListRequestModel model);

        #endregion
    }
}
