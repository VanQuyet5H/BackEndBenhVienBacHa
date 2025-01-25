using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.VatTu;

namespace Camino.Services.HopDongThauVatTuService
{
    public interface IHopDongThauVatTuService
        : IMasterFileService<HopDongThauVatTu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? hopDongThauId = null, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<List<VatTuTemplateVo>> GetListVatTu(DropDownListRequestModel model);

        Task<bool> GetHieuLucVatTu(long id);

        bool KiemTraConVatTu(long? idVatTu);

        Task<bool> CheckExist(long id, long nhaThauId, string soHopDong, string soQuyetDinh);

        Task<bool> CheckVatTuBenhVienExist(long vatTuId);

        Task<List<VatTuTemplateVo>> GetVatTus(DropDownListRequestModel queryInfo);
    }
}
