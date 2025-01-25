using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.GachNos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.GachNo
{
    public interface IGachNoService : IMasterFileService<Core.Domain.Entities.GachNos.GachNo>
    {
        #region grid
        Task<GridDataSource> GetDataGachNoForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageGachNoForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataBenhNhanForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageBenhNhanForGridAsync(QueryInfo queryInfo);

        #endregion

        #region get list
        Task<List<LookupItemCauHinhVo>> GetListLoaiThuChiAsync(DropDownListRequestModel model);
        Task<List<LookupLoaiTienTeItemVo>> GetListLoaiTienTeAsync(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListDoiTuongAsync(DropDownListRequestModel model);
        Task<List<LookupCongTyBHTNItemVo>> GetListMaCongTyBaoHiemTuNhanAsync(DropDownListRequestModel model);
        Task<List<LookupBenhNhanItemVo>> GetListMaBenhNhanAsync(DropDownListRequestModel model);
        Task<List<LookupItemCauHinhVo>> GetListTaiKhoanAsync(DropDownListRequestModel model);
        Task<List<LookupItemCauHinhVo>> GetListSoTaiKhoanNganHangAsync(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListLoaiChungTuAsync(DropDownListRequestModel model);
        Task<List<LookupItemTemplateVo>> GetListBaoHiemTuNhanAsync(DropDownListRequestModel model);
        #endregion

        #region xử lý thêm/xóa/sửa
        Task<bool> KiemTraQuyenXacNhanNhapLieuAsync();
        Task XuLyCapNhatCongNo(Core.Domain.Entities.GachNos.GachNo gachNo);
        Task<List<GachNoHistoryVo>> GetLichSuGachNoAsync(long id);
        #endregion

        #region Báo cáo công nợ
        Task<BaoCaoGachNoCongTyBhtnGridDatasourceVo> GetBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo);

        Task<BaoCaoChiTietGachNoCongTyBhtnGrid> GetChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo);
        #endregion
    }
}
