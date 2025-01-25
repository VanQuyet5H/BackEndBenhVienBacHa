using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        NoiTruHoSoKhac GetThongTinHoSoKhacBieuDoChuyenDa(long yeuCauTiepNhanId);
        Task<string> InBieuDoChuyenDa(long yeuCauTiepNhanId, bool isInFilePDF = true);
    }
}
