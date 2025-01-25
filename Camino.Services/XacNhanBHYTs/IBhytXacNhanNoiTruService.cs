using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BenefitInsurance;

namespace Camino.Services.XacNhanBHYTs
{
    public interface IBhytXacNhanNoiTruService
    {
        Task<BenefitInsuranceResultVo> XacNhanBhytNoiTruAsync(BenefitInsuranceVo duyetBaoHiemVo);
    }
}
