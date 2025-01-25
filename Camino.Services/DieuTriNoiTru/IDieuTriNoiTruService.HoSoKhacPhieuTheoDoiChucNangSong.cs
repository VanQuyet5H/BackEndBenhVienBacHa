using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        NoiTruHoSoKhac GetThongTinHoSoKhacPhieuTheoDoiChucNangSong(long yeuCauTiepNhanId);
        Task<string> InPhieuTheoDoiChucNangSong(long yeuCauTiepNhanId, bool isInFilePDF = true);
    }
}
