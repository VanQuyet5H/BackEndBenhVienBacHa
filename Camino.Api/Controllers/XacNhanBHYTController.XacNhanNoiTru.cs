using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BenefitInsurance;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        [HttpPost("XnBhytNoiTruAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<BenefitInsuranceResultVo>> XnBhytNoiTruAsync([FromBody]BenefitInsuranceVo duyetBaoHiemVo)
        {
            if (duyetBaoHiemVo.BenefitInsurance.Any(x => x.MucHuong == null || x.MucHuong == 0))
            {
                var errors = new List<ValidationError>();

                for (int i = 0; i < duyetBaoHiemVo.BenefitInsurance.Count; i++)
                {
                    if (duyetBaoHiemVo.BenefitInsurance[i].MucHuong.GetValueOrDefault() == 0)
                    {
                        errors.Add(new ValidationError($"BenefitInsurance[{i}].TiLeBhyt", duyetBaoHiemVo.BenefitInsurance[i].MucHuong == null ? _localizationService.GetResource("BHYT.TiLeHuong.NotEmpty") : _localizationService.GetResource("BHYT.TiLeHuong.NotEqualZero")));
                    }
                }
                throw new ApiException(_localizationService.GetResource("BHYT.TiLeHuong.NotEqualZero"), 500, errors);
            }

            return Ok(await _bhytConfirmByDayService.XacNhanBHYTNoiTruAsync(duyetBaoHiemVo));
        }

        //Hàm hủy duyệt bảo hiểm y tế
        [HttpPost("HuyDuyetBaoHiemYteNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<BenefitInsuranceResultVo>> HuyDuyetBaoHiemYteNoiTru([FromBody]BenefitInsuranceVo dsBaoHiemVo)
        {
            return Ok(_bhytConfirmByDayService.HuyDuyetBaoHiemYteNoiTru(dsBaoHiemVo));
        }
    }
}
