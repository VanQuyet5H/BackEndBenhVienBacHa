using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.BenefitInsurance;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Services.XacNhanBHYTs
{
    [ScopedDependency(ServiceType = typeof(IBhytXacNhanNoiTruService))]
    public class BhytXacNhanNoiTruService : YeuCauTiepNhanBaseService, IBhytXacNhanNoiTruService
    {
        public BhytXacNhanNoiTruService(IRepository<YeuCauTiepNhan> repository, IUserAgentHelper userAgentHelper, ILocalizationService localizationService, ICauHinhService cauHinhService, ITaiKhoanBenhNhanService taiKhoanBenhNhanService)
            : base(repository, userAgentHelper, cauHinhService, localizationService, taiKhoanBenhNhanService)
        { }

        public Task<BenefitInsuranceResultVo> XacNhanBhytNoiTruAsync(BenefitInsuranceVo duyetBaoHiemVo)
        {
            throw new System.NotImplementedException();
        }
    }
}
