using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        NoiTruHoSoKhac GetThongTinHoSoKhacBangTheoDoiGayMeHoiSuc(long yeuCauTiepNhanId, long? hoSoKhacId);
        Task<GridDataSource> GetDanhSachBangTheoDoiGayMeHoiSuc(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPagesDanhSachBangTheoDoiGayMeHoiSuc(QueryInfo queryInfo);
        Task<string> InBangTheoDoiGayMeHoiSuc(long yeuCauTiepNhanId, long hoSoKhacId, bool isInFilePDF = true);
    }
}
