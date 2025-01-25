using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task KiemTraThoiDiemXuatVienBenhAn(long yeuCauTiepNhanId);
        Task<List<string>> GetContentInPhieuThamKham(long yeuCauTiepNhanId, long? phieuDieuTriId = null, List<long> dienBienIds = null);
        bool IsCheckThongTinBenhAnDaKetThuc(long yeuCauTiepNhanId);
    }
}
