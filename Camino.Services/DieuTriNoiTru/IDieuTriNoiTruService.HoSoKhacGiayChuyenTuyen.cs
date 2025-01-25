using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        NoiTruHoSoKhac GetThongTinHoSoKhacGiayChuyenTuyen(long yeuCauTiepNhanId);
        string GetChanDoan(long yeuCauTiepNhanId);
        Task<string> InGiayChuyenTuyen(long yeuCauTiepNhanId, bool isInFilePDF = true);
    }
}
