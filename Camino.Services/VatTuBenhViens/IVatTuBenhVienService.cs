using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.VatTuBenhViens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.VatTuBenhViens
{
   public interface IVatTuBenhVienService : IMasterFileService<VatTuBenhVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<VatTuYTeDropdownTemplateVo>> GetVatTuYTeBenhVienKhamBenh(DropDownListRequestModel model);
        Task<List<VatTuYTeDropdownTemplateVo>> GetVatTuYTeBenhVienKhamBenhUpdate(DropDownListRequestModel model, long id);
        List<LookupItemVo> GetLoaiSuDung(LookupQueryInfo queryInfo);

        #region //BVHD-3472
        Task<string> GetMaTaoMoiVatTuAsync(MaVatTuTaoMoiInfoVo model);
        Task<bool> KiemTraTrungVatTuBenhVienAsync(Core.Domain.Entities.VatTus.VatTu vatTu);
        Task<bool> KiemTraTrungMaVatTuBenhVienAsync(long vatTuBenhVienId, string maVatTu, List<string> maVatTuTemps = null);

        void XuLyCapNhatMaVatTuBenhVien();
        #endregion
    }
}
