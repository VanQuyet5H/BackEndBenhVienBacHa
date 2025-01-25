using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial interface IYeuCauTiepNhanService
    {
        #region Danh Sách Kết Quả Cận Lâm Sàng

        Task<GridDataSource> GetCanLamSangDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetCanLamSangTotalPageForGridAsync(QueryInfo queryInfo);

        #endregion

        #region Danh sách kết quả mẫu
        Task<GridDataSource> GetDataForGridNoiDungMauAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNoiDungMauAsync(QueryInfo queryInfo);


        #endregion

        DanhSachCanLamSangVo GetThongTinCanLamSang(long id);
        DanhSachCanLamSangVo GetThongTinKLichSuCanLamSang(long id);
        Task<GridDataSource> GetLichSuCanLamSangDaCoKetQuaDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetLichSuCanLamSangCanLamSangDaCoKetQuaTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataChiTietLichSuCanLamSangForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalChiTietLichSuCanLamSangPageForGridAsync(QueryInfo queryInfo);
        KetQuaCLS GetCanLamSangIdByMaBNVaMaTT(TimKiemThongTinBenhNhan TimKiemThongTinBenhNhan);
        void DeleteFileKetQuaCanLamSang(List<FileKetQuaCanLamSang> fileKetQuaCanLamSangs);
        Task<List<YeuCauKyThuatCDHALookupItemVo>> GetListKyThuatDichVuKyThuatTheoTiepNhan(DropDownListRequestModel queryInfo, long yeuCauTiepNhanId);
        TrangThaiYeuCauKyThuat TrangThaiYeuCauDichVuKyThuat(long yeuCauDichVuKyThuatId);
        #region lookup
        Task<List<string>> GetListKyThuatDichVuKyThuatAsync(DropDownListRequestModel queryInfo);


        #endregion

        #region thêm/xóa/sửa kết quả
        void KiemTraLuuNoiDungKetQuaAsync(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, string kyThuat);


        #endregion

        #region In phiếu
        string XuLyInPhieuKetQuaAsync(PhieuInKetQuaInfoVo phieuInKetQuaInfoVo);
        string XuLyInPhieuKetQuaTheoYeuCauAsync(CauHinhHinhVo cauHinhIn);
        #endregion

        #region Cập nhật 27/12/2022
        YeuCauTiepNhan GetThongTinChungBenhNhan(long yeuCauTiepNhanId);
        #endregion
    }
}