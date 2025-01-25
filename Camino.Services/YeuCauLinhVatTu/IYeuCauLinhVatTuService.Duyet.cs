using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial interface IYeuCauLinhVatTuService
    {
        #region Get data
        Task<List<LookupItemVo>> GetListNhanVienAsync(DropDownListRequestModel model);

        Task<GridDataSource> GetVatTuYeuCauLinhBuDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetVatTuYeuCauLinhBuTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoVatTuCanLinhBuDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoVatTuCanLinhBuTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetVatTuYeuCauLinhTrucTiepDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetVatTuYeuCauLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoVatTuCanLinhTrucTiepDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoVatTuCanLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo);
        #endregion
        #region Xử lý thêm/xóa/sửa lĩnh thường
        Task XuLyDuyetYeuCauLinhVatTuThuongAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhVatTu, long nguoiXuatId, long nguoiNhapId);
        Task<string> InPhieuDuyetLinhThuongVatTuAsync(InPhieuDuyetLinhVatTu inPhieu);

        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh bù
        Task XuLyDuyetYeuCauLinhVatTuBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhVatTu, long nguoiXuatId, long nguoiNhapId);
        Task<string> InPhieuDuyetLinhBuVatTuAsync(InPhieuDuyetLinhVatTu inPhieu);
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh trực tiếp
        Task XuLyDuyetYeuCauLinhVatTuTrucTiepAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhVatTu, long nguoiXuatId, long nguoiNhapId);
        Task<string> InPhieuDuyetLinhTrucTiepVatTuAsync(InPhieuDuyetLinhVatTu inPhieu);
        #endregion
    }
}
