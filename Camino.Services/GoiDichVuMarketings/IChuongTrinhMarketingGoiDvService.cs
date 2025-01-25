using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.GoiDichVu;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuaTang;

namespace Camino.Services.GoiDichVuMarketings
{
    public interface IChuongTrinhMarketingGoiDvService : IMasterFileService<ChuongTrinhGoiDichVu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetDataForGridYeuCauGoiDichVuAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridLoaiGoiMarketingAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridLoaiGoiMarketingAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridYeuCauGoiDichVuAsync(QueryInfo queryInfo);

        Task<List<GoiDichVuTemplateVo>> GetGoiDv(DropDownListRequestModel model);

        Task<List<QuaTangTemplateVo>> GetListQuaTang(DropDownListRequestModel model);

        Task<bool> IsTenExists(string ten = null, long id = 0);

        Task<bool> IsMaExists(string ma = null, long id = 0);
        
        Task<List<LookupItemVo>> GetLoaiGoiDichVus(DropDownListRequestModel queryInfo);

        Task<bool> IsMaTenExists(string maHoacTen = null, long Id = 0, bool flag = false);
    }
}
