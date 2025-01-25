using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Domain.ValueObject.XuatKhos;

namespace Camino.Services.YeuCauHoanTra.KSNK
{
    public partial interface IYeuCauHoanTraKSNKService
    {
        Task<GridDataSource> GetAllDpVtKsnkData(QueryInfo queryInfo);
        //Task<GridDataSource> GetAllVatTuData(QueryInfo queryInfo);
        //Task<GridDataSource> GetAllVatTuTotal(QueryInfo queryInfo);
        Task<List<XuatKhoKhacLookupItem>> GetKhoDPvaVTKSNK(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetKhoVatTuHoanTra(DropDownListRequestModel model);
        Task<List<KSNKHoanTraGridVo>> GetVatTuOnGroup(Enums.LoaiSuDung groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon);
        Task<YeuCauTraVatTuChiTiet> GetVatTu(ThemKSNKHoanTra model);
        Task UpdateGiaChoNhapKhoChiTiet(double soLuongXuat, long id);
        Task UpdateSlXuatNhapKhoChiTiet(double soLuongXuat, long id);
        Task<bool> CheckValidSlTon(List<ThemKSNKHoanTra> lstModelThemVatTuHoanTra, long id);
    }
}
