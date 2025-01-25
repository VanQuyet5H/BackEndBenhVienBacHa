using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.HDPP;
using FluentValidation;

namespace Camino.Api.Models.KetQuaVaKetLuanMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HDPPViewModel>))]
    public class HDPPViewModelValidator : AbstractValidator<HDPPViewModel>
    {
        public HDPPViewModelValidator(ILocalizationService localizationService,
            IHDPPService _hdppService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("HDPP.Ma.Required"))
                .MustAsync(async (model, input, d) =>
                    !await _hdppService.KiemTraTenTrung(model.Id, input)).WithMessage(localizationService.GetResource("HDPP.Ma.IsExists"));

        }
    }
}
