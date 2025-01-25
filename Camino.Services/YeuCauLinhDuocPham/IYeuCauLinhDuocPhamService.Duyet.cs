using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial interface IYeuCauLinhDuocPhamService
    {
        #region Get data
        Task<List<LookupItemVo>> GetListNhanVienAsync(DropDownListRequestModel model);

        Task<GridDataSource> GetDuocPhamYeuCauLinhBuDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDuocPhamYeuCauLinhBuTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoDuocPhamCanLinhBuDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoDuocPhamCanLinhBuTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDuocPhamYeuCauLinhTrucTiepDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDuocPhamYeuCauLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoDuocPhamCanLinhTrucTiepDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoDuocPhamCanLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo);
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh thường
        Task XuLyDuyetYeuCauLinhDuocPhamThuongAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhDuocPham, List<DuocPhamCanXuatVo> duocPhamCanXuatVos, long nguoiXuatId, long nguoiNhapId);
        Task<string> InPhieuDuyetLinhThuongDuocPhamAsync(InPhieuDuyetLinhDuocPham inPhieu);

        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh bù
        Task XuLyDuyetYeuCauLinhDuocPhamBuAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhDuocPham, long nguoiXuatId, long nguoiNhapId);
        Task<string> InPhieuDuyetLinhBuDuocPhamAsync(InPhieuDuyetLinhDuocPham inPhieu);
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh trực tiếp
        Task XuLyDuyetYeuCauLinhDuocPhamTrucTiepAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhDuocPham, long nguoiXuatId, long nguoiNhapId);
        Task<string> InPhieuDuyetLinhTrucTiepDuocPhamAsync(InPhieuDuyetLinhDuocPham inPhieu);
        #endregion
    }
}
