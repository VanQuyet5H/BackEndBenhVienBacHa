using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial interface IThuNganNoiTruService
    {
        #region Bảng kê chưa quyết toán

        byte[] XuatBangKeCoBHYTChuaQuyetToan(long yeuCauTiepNhanId);
        byte[] XuatBangKeChuaQuyetToan(long yeuCauTiepNhanId);
        byte[] XuatBangKeChuaQuyetToanTrongGoiDv(long yeuCauTiepNhanId);

        #endregion

        #region Bảng kê chờ thu

        byte[] XuatBangKeNoiTruChoThu(ThuPhiKhamChuaBenhNoiTruVo model);
        byte[] XuatBangKeNoiTruChoThuTrongGoi(QuyetToanDichVuTrongGoiVo model);

        #endregion

        #region Bảng kê đã thu

        byte[] XuatBangKeCoBHYT(long taiKhoanThuId);

        byte[] XuatBangKe(long taiKhoanThuId);

        byte[] XuatBangKeTrongGoiDv(long taiKhoanThuId);

        #endregion
    }
}