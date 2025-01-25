using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ICDs;

namespace Camino.Services.ICDs
{
    public interface IChuanDoanService : IMasterFileService<ChuanDoan>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        Task<List<DanhMucGridCombobox>> GetDichVuKhamBenh(DropDownListRequestModel model);
        Task<List<DanhMucGridCombobox>> GetListChuanDoanIcd(DropDownListRequestModel model);
        ChuanDoan GetChuanDoanLast();
        Task<bool> IsTenViExists(string tenVi = null, long id = 0);
        Task<bool> IsTenEngExists(string tenEng = null, long id = 0);
        Task<bool> IsMaExists(string ma = null, long id = 0);
        string GetTenDanhMuc(long id);
        List<DanhMucChuanDoan> GetDanhMucChuanDoan(long id);

        Task<List<LookupItemTextVo>> GetListChuanDoanTheoMaBenh(DropDownListRequestModel model);
        Task<List<LookupItemTextVo>> GetListChuanDoanTheoTenBenh(DropDownListRequestModel model);
    }
}
