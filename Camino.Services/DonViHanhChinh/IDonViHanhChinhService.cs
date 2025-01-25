using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DonViHanhChinh;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DonViHanhChinh
{
    public interface IDonViHanhChinhService : IMasterFileService<Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh>
    {
        Task<GridDataSource> GetDataForGridAsync(DonViHanhChinhQueryInfo queryInfo, bool exportExcel);
        Task<GridDataSource> GetTotalPageForGridAsync(DonViHanhChinhQueryInfo queryInfo);
        Task<List<LookupItemVo>> GetTinhThanhLookup(DonViHanhChinhLookupQueryInfo model);
        Task<List<LookupItemVo>> GetQuanHuyenLookup(DonViHanhChinhLookupQueryInfo model, long tinhThanhId);
        Task<List<LookupItemVo>> GetPhuongXaLookup(DonViHanhChinhLookupQueryInfo model, long quanHuyenId, long? tinhThanhId = null);
        Task<List<LookupItemVo>> GetKhomApLookup(DonViHanhChinhLookupQueryInfo model, long khomApId, long? quanHuyenId, long? tinhThanhId = null);

        Task<List<LookupItemTemplateVo>> GetListDonViHanhChinh(DropDownListRequestModel model);
        bool CheckExistMaCapTinhThanhPho(string ma, long donViId = 0);
        bool CheckExistMaCapQuanHuyen(long trucThuocDonViHanhChinhId, string ma, long donViId = 0);
        bool CheckExistMaCapPhuongXa(long trucThuocDonViHanhChinhId, string ma, long donViId = 0);
        List<DonViHanhChinhExcel> GetDataExportExecl(DonViHanhChinhQueryInfo queryInfo);
    }
}
