using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BangKiemAnToanNBPT;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        ThuocTienMeVaNhanVienGrid GetThuocTienMeVaNhanVienVaChiSoSinhTon(QueryInfo queryInfo);
        BangKiemAnToanNBPTGridVo GetThongTinBangKiemATNBPT(long yeuCauTiepNhan);
        Task<GridDataSource> GetDanhSachBangKiemAnToanNBPT(QueryInfo queryInfo);
        List<long> GetListChiSoSinhHieu(long yeuCauTiepNhan);
        BangKiemAnToanNBPTGridVo GetThongTinBangKiemAnToanNBPTViewDS(long noiTruHoSoKhacId);
        BangKiemAnToanNBPTGridVo GetDanhSachBangKiemAnToanNBPTSave(long yeuCauTiepNhanId);
        Task<string> BangKiemAnToanNBPT(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan);
        NhanVienNgayThucHien GetThongTinCreate(long idNguoiLogin, long yeuCauTiepNhanId);
        bool KiemTraThoaDieuKien(long yeuCauTiepNhanId, DateTime? ngay,int loai);
    }
}
