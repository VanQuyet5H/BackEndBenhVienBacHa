using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.BenefitInsurance
{

    public class PhieuXacNhanBHYT
    {
        public long Id { get; set; }
        public string Html { get; set; }
        public string TenFile { get; set; }

        public string PageSize { get; set; } //A0,A1,A2,A3,A4,A5
        public string PageOrientation { get; set; } //Landscape,Portrait
    }

    public class BenefitInsuranceVo
    {
        public List<InsuranceConfirmVo> NonBenefitInsurance { get; set; } // list những bảo hiểm không cho hưởng

        public List<InsuranceConfirmVo> BenefitInsurance { get; set; } // list những bảo hiểm cho hưởng

        public long IdYeuCauTiepNhan { get; set; }
    }

    public class BenefitInsuranceResultVo
    {
        public bool IsError { get; set; }
        public int ErrorType { get; set; }//1= co dv nho hon 100 khi tong BHYT chi tra nho hon 15%, 2= co dv lon hon 80 khi tong BHYT chi tra lon hon 15%, 3=can reload lai data grid
        public string ErrorMessage { get; set; }
    }
}
