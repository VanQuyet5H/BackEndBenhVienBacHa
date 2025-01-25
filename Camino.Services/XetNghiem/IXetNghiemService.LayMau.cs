using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.XetNghiem
{
    public partial interface IXetNghiemService
    {
        #region grid
        Task<GridDataSource> GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer3(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer4(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer3(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer4(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridNhomCanLayMauXetNghiemAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNhomCanLayMauXetNghiemAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDichVuCanLayMauXetNghiemAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridDichVuCanLayMauXetNghiemAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDichVuCanLayMauXetNghiemAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridDichVuCanLayMauXetNghiemAsyncVer2(QueryInfo queryInfo);
        #endregion

        #region Get data

        Task<ThongTinYeuCauTiepNhanLayMauVo> GetDanhThongTinYeuCauTiepNhanLayMauAsync(long yeuCauTiepNhanId);
        Task<List<LookupItemTemplateVo>> GetListBarcodeTheoYeuCauTiepNhanAsync(DropDownListRequestModel model);
        Task<LookupItemVo> KiemTraBarcodeDangChonAsync(string strQuery);
        LookupItemVo TaoBarcodeAsync(int? barcodeNumber);

        Task<List<LichSuTuChoiMauVo>> GetLichSuTuChoiMauAsync(long yeuCauTiepNhanId);
        Task<string> InBarcodeLayMauXetNghiemAsync(LayMauXetNghiemInBarcodeVo model);
        #endregion

        #region Xử lý tương tác data

        Task XuLyLayMauXetNghiemAsync(LayMauXetNghiemXacNhanLayMauVo layMauXetNghiemVo);
        Task XuLyLayLaiMauXetNghiemAsync(LayMauXetNghiemXacNhanLayMauVo layMauXetNghiemVo);
        Task XuLyBenhNhanNhanKetQuaAsync(long yeuCauTiepNhanId);
        Task<long> XuLyGuiMauXetNghiemAsync(GuiMauXetNghiemVo phieuGuiMauXetNghiem);
        Task<string> InPhieuGuiMauXetNghiemAsync(InPhieuDGuimauXetNghiemVo inPhieuVo);

        #endregion

        #region Xử lý lấy, gửi và nhận mẫu 1 lần
        Task<long> XuLyGuiVaNhanMauXetNghiemAsync(LayMauXetNghiemXacNhanLayMauVo layMauXetNghiemVo);
        Task XuLyHuyMauXetNghiemAsync(LayMauXetNghiemXacNhanLayMauVo layMauXetNghiemVo);
        #endregion

        #region Cập nhật lấy mẫu
        Task XuLyCapBarcodeChoDichhVuDangChonAsync(CapBarcodeTheoDichVuVo capBarcodeTheoDichVuVo);
        Task XuLyXacNhanNhanMauXetNghiemAsync(XacNhanNhanMauChoDichVuVo xacNhanNhanMauVo); //long phienXetNghiemChiTietId
        Task XuLyXacNhanHuyCapBarcodeTheoDichVuAsync(XacNhanNhanMauChoDichVuVo xacNhanNhanMauVo);
        Task CapNhatGridItemThoiGianNhanMauAsync(CapNhatGridItemChoDichVuDaCapCodeVo capNhatNgayNhanMauVo);
        #endregion
    }
}
