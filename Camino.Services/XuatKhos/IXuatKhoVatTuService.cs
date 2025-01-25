using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.XuatKhos
{
    public interface IXuatKhoVatTuService : IMasterFileService<XuatKhoVatTu>
    {
        Task<GridDataSource> GetAllVatTuData(QueryInfo queryInfo);
        Task<GridDataSource> GetAllVatTuTotal(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? XuatKhoVatTuId = null, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<bool> IsValidateUpdateOrRemove(long id);

        Task DeleteXuatKho(XuatKhoVatTu entity);

        Task<XuatKhoVatTuChiTiet> GetVatTu(ThemVatTu model);

        Task<NhapKhoVatTuChiTiet> GetNhapKhoVatTuChiTietById(long id);

        Task<List<VatTuXuatGridVo>> GetVatTuOnGroup(Enums.LoaiSuDung? groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon);

        Task<List<LookupItemVo>> GetKhoVatTu(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetKhoVatTuNhap(DropDownListRequestModel model);

        Task<bool> IsKhoExists(long id);

        Task SaveChange();

        Task<double> GetSoLuongTon(long duocPhamId, bool isDatChatLuong, long khoNhapId);

        Task<bool> IsKhoLeOrNhaThuoc(long id);

        Task<string> InPhieuXuat(long id, string hostingName);
        Task<long> XuatKhoVatTu(ThongTinXuatKhoVatTuVo thongTinXuatKhoVatTuVo);
    }
}
