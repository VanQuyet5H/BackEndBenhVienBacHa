using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.NhapKhoMau
{
    public interface INhapKhoMauService : IMasterFileService<Core.Domain.Entities.MauVaChePhams.NhapKhoMau>
    {
        #region Grid
        Task<GridDataSource> GetDataForGridNhapKhoMauAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNhapKhoMauAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridNhapKhoMauChiTietAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNhapKhoMauChiTietAsync(QueryInfo queryInfo);
        #endregion

        #region lookup
        Task<List<LookupItemYeuCauTruyenMauVo>> GetListYeuCauTruyenMauAsync(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListNhomMauAsync(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListYeuToRhAsync(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListLoaiXetNghiemMauNhapThemAsync(DropDownListRequestModel model);

        #endregion

        #region Kiểm tra data
        Task<bool> KiemTraTrungMaTuiMauAsync(string maTuiMau, long nhapKhoMauChiTietId, List<string> maTuiMauDangNhaps);
        Task<bool> KiemTraTrungYeuCauTruyenMauAsync(long? yeuCauTruyenMauId, long nhapKhoMauChiTietId, List<long> yeuCauTruyenMauIdDangChons);

        Task<bool> KiemTraCapNhatGiaNhapKhoMauChiTietAsync(decimal? giaDV, decimal? giaBH, long nhapKhoMauChiTietId);
        #endregion

        #region get data
        Task<Core.Domain.Entities.MauVaChePhams.NhapKhoMau> GetPhieuNhapKhoMauAsync(long id);


        #endregion

        #region In phiếu
        Task<string> XuLyInPhieuTruyenMauAsync(long phieuTruyenMauId);


        #endregion

        #region Duyệt
        void XuLyXuatKhoMau(Core.Domain.Entities.MauVaChePhams.NhapKhoMau phieuNhapKhoMau);


        #endregion

        Task XuLyTaoPhieuNhapKhoMauAsync(Core.Domain.Entities.MauVaChePhams.NhapKhoMau nhapKhoMau);
        Task XuLyXoaPhieuNhapKhoMauAsync(long id);
        Task XuLyCapNhatPhieuNhapKhoMauAsync(Core.Domain.Entities.MauVaChePhams.NhapKhoMau nhapKhoMau);
    }
}
