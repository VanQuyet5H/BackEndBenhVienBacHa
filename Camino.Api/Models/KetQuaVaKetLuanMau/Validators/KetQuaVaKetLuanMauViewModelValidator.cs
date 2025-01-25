using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KetQuaVaKetLuanMau;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KetQuaVaKetLuanMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KetQuaVaKetLuanMauViewModel>))]
    public class KetQuaVaKetLuanMauViewModelValidator : AbstractValidator<KetQuaVaKetLuanMauViewModel>
    {
        public KetQuaVaKetLuanMauViewModelValidator(ILocalizationService localizationService, IKetQuaVaKetLuanMauService _ketQuaVaKetLuanMauService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.Ma.Required"))
                .MustAsync(async (model, input, d) => 
                    !await _ketQuaVaKetLuanMauService.KiemTraTrungMa(model.Id, input)).WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.Ma.IsExists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.Ten.Required"));

            RuleFor(x => x.KetQuaMau)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.KetQuaMau.Required"));
            RuleFor(x => x.KetLuanMau)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.KetLuanMau.Required"));
        }
    }
}
