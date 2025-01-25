using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial interface IYeuCauLinhKSNKService
    {
        #region Get data
        Task<List<LookupItemVo>> GetListNhanVienAsync(DropDownListRequestModel model);

        Task<GridDataSource> GetKSNKYeuCauLinhBuDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetKSNKYeuCauLinhBuTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoKSNKCanLinhBuDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoKSNKCanLinhBuTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetKSNKYeuCauLinhTrucTiepDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetKSNKYeuCauLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoKSNKCanLinhTrucTiepDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoKSNKCanLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo);
        #endregion
        #region Xử lý thêm/xóa/sửa lĩnh thường
        Task XuLyDuyetYeuCauLinhKSNKThuongAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhKSNK, long nguoiXuatId, long nguoiNhapId);
        Task<string> InPhieuDuyetLinhThuongKSNKAsync(InPhieuDuyetLinhKSNK inPhieu);

        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh bù
        Task XuLyDuyetYeuCauLinhKSNKBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhKSNK, long nguoiXuatId, long nguoiNhapId);
        Task<string> InPhieuDuyetLinhBuKSNKAsync(InPhieuDuyetLinhKSNK inPhieu);
        #endregion
    }
}
