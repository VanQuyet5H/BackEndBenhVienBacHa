using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;

namespace Camino.Services.XetNghiem
{
    public partial interface IXetNghiemService
    {
        #region grid
        Task<GridDataSource> GetDataForGridChuaCapCodeAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChuaCapCodeAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChuaCapCodeAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChuaCapCodeAsyncVer2(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDaCapCodeAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridDaCapCodeAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDaCapCodeAsyncVer2(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsync(QueryInfo queryInfo);
        #endregion

        #region get data
        Task<List<LookupItemTemplateVo>> GetListHopDongKhamSucKhoeHieuLucAsync(DropDownListRequestModel model);
        Task<ThongTinBenhNhanXetNghiemVo> GetThongTinBenhNhanXetNghiemAsync(BenhNhanXetNghiemQueryVo query);
        Task<List<PhienXetNghiem>> GetChiTietPhienXetNghiemsAsync(List<long> phienXetNghiemIds);
        #endregion

        #region Xử lý data

        Task KiemTraDichVuCanHuyCapCodeAsync(List<long> yeuCauDichVuKyThuatIds);
        List<ThongTinSoLuongInThemTheoTaiKhoanVo> GetSoLuongInThemTheoUser();
        Task XuLyCapNhatSoLuongInThemTheoUserAsync(int? soLuongInThem);
        Task KiemTraDichVuCanHuyNhanMauAsync(List<long> yeuCauDichVuKyThuatIds);
        Task XuLyXacNhanHuyNhanMauTheoDichVuAsync(XacNhanNhanMauChoDichVuVo xacNhanNhanMauVo);
        #endregion

        Task<List<BenhNhanChuaCapBarcode>> ImportNguoiBenhChuaCapBarcodeXetNghiem(Stream path);
        List<string> InBarcodeCuaBenhNhan(InBarcodeDaCapCodeBenhNhan inBarcodeDaCapCodeBenhNhan);

    }
}
