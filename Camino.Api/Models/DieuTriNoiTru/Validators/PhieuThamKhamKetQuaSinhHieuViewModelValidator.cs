using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuThamKhamKetQuaSinhHieuViewModel>))]
    public class PhieuThamKhamKetQuaSinhHieuViewModelValidator : AbstractValidator<PhieuThamKhamKetQuaSinhHieuViewModel>
    {
        public PhieuThamKhamKetQuaSinhHieuViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ThoiDiemThucHien)
            .Must((model, s) => 
            {
                if (model.ThoiDiemThucHien == null)
                {
                    return false;
                }
                else
                {
                    if (model.ThoiDiemThucHien < model.ThoiDiemNhapVien || model.ThoiDiemThucHien > DateTime.Now)
                    {
                        return false;
                    }
                }
                return true;
            }).WithMessage(localizationService.GetResource("KetQuaSinhHieus.ThoiDiemThucHien.MoreThanNow"));
        }
    }
}
