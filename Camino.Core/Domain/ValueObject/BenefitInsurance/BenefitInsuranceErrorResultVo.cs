using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.BenefitInsurance
{
    public class BenefitInsuranceErrorResultVo
    {
        public bool IsError => SaiMucHuongMessage != null || CatchingErrors.Count != 0;

        public string SaiMucHuongMessage { get; set; }

        public List<CatchingErrorsVo> CatchingErrors { get; set; }
    }

    public class CatchingErrorsVo
    {
        public string Field { get; set; }

        public string Message { get; set; }
    }
}
