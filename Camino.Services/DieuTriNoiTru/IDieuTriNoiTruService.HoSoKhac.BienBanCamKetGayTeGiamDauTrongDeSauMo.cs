using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        NoiTruHoSoKhac GetThongTinBienBanCamKetGayTeGiamDauTrongDeSauMo(long yeuCauTiepNhanId);
        Task<string> InBienBanCamKetGayTeGiamDauTrongDeSauMo(long yeuCauTiepNhanId, bool isInFilePDF = true);
    }
}
