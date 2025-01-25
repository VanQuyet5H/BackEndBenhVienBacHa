using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        NoiTruHoSoKhac GetThongTinHoSoKhacGiayTuNguyenTrietSan(long yeuCauTiepNhanId);
        Task<string> InGiayTuNguyenTrietSan(long yeuCauTiepNhanId, string hostingName, bool isInFilePDF = true);
    }
}