using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial interface IYeuCauTiepNhanService
    {
        byte[] XuatBangKeNgoaiTruCoBHYTExcel(long yeuCauTiepNhanId, bool xemTruoc = false);
        byte[] XuatBangKeNgoaiTruExcel(long yeuCauTiepNhanId, bool xemTruoc = false);
        byte[] XuatBangKeNgoaiTruTrongGoiDv(long yeuCauTiepNhanId, bool xemTruoc = false);

        byte[] XuatBangKeNgoaiTruChoThuExcel(ThuPhiKhamChuaBenhVo thuPhiKhamChuaBenhVo);
        byte[] XuatBangKeNgoaiTruTrongGoiChoThuExcel(QuyetToanDichVuTrongGoiVo thuPhiKhamChuaBenhVo);
    }
}