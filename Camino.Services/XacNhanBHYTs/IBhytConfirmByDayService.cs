using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenefitInsurance;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Services.XacNhanBHYTs
{
    public interface IBhytConfirmByDayService : IYeuCauTiepNhanBaseService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForDaXacNhanAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForDaXacNhanAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForBothBhyt(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForBothBhyt(QueryInfo queryInfo);
        Task<DanhSachChoGridVo[]> GetXacNhanBhytByMaBnVaMaTt(TimKiemThongTinBenhNhan timKiemThongTinBenhNhan);

        Task<BenefitInsuranceResultVo> ConfirmBenefitInsuranceAsync(BenefitInsuranceVo duyetBaoHiemVo);
        Task<BenefitInsuranceResultVo> XacNhanBHYTNoiTruAsync(BenefitInsuranceVo duyetBaoHiemVo);
        Task DuyetBaoHiemAsync(long yeuCauTiepNhanId);
        BenefitInsuranceResultVo HuyDuyetBaoHiemYte(BenefitInsuranceVo duyetBaoHiemVo);
        BenefitInsuranceResultVo HuyDuyetBaoHiemYteNoiTru(BenefitInsuranceVo duyetBaoHiemVo);
    }
}
