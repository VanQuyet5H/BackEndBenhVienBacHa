using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;

namespace Camino.Services.YeuCauHoanTra.VatTu
{
    public partial interface IYeuCauHoanTraVatTuService
    {
        Task<GridDataSource> GetAllVatTuData(QueryInfo queryInfo);
        Task<GridDataSource> GetAllVatTuTotal(QueryInfo queryInfo);

        Task<List<LookupItemVo>> GetKhoVatTu(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetKhoVatTuHoanTra(DropDownListRequestModel model);

        Task<List<VatTuHoanTraGridVo>> GetVatTuOnGroup(Enums.LoaiSuDung groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon);

        Task<YeuCauTraVatTuChiTiet> GetVatTu(ThemVatTuHoanTra model);

        Task UpdateGiaChoNhapKhoChiTiet(double soLuongXuat, long id);

        Task UpdateSlXuatNhapKhoChiTiet(double soLuongXuat, long id);

        Task<bool> CheckValidSlTon(List<ThemVatTuHoanTra> lstModelThemVatTuHoanTra, long id);
    }
}
