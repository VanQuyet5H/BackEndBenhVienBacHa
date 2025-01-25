using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task<GridDataSource> GetDataLichSuTiepNhanKhamSucKhoeForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalPageLichSuTiepNhanKhamSucKhoeForGridAsync(QueryInfo queryInfo);
        #region In phiếu
        Task<string> XuLyInKhamSucKhoeAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo);
        Task<string> XuLyInPhieuDangKyKhamSucKhoeAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo);
        string GetTemplatePhieuDangKyKham(int loaiPhieu);
        Task<string> XuLyInKetQuaKhamSucKhoeAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo);

        Task<string> XuLyInKetQuaKhamSucKhoeKetLuanAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo);
        #endregion
        #region update 772021
        Task<List<DanhSachPhanLoaiCacBenhTatGrid>> GetGridPhanLoaiVaCacBenhtatDenghi(long hopDongKhamSucKhoeId);
        List<DanhSachPhanLoaiCacBenhTatGrid> GetGridPhanLoaiVaCacBenhtatDenghiByHopDong(long hopDongKhamSucKhoeId);
        List<LookupItemVo> GetPhanLoaiSucKhoeKetLuan(DropDownListRequestModel model,long? phanLoaiId);
        #endregion
        #region chức năng copy dịch vụ xét nghiệm có data của tất cả yêu cầu tiếp nhận thuộc hợp đồng khám 
        void UpdateAllKetQuaKSKDoanCuaHopDongNhanVienBatDauKham(long hopDongKhamSucKhoeId);
        #endregion

        #region //BVHD-3929
        Task<List<string>> XuLyInNhieuPhieuDangKyKhamSucKhoeAsync(PhieuInDangKyKSKVo phieuInDangKyKSKVo);


        #endregion
    }
}
