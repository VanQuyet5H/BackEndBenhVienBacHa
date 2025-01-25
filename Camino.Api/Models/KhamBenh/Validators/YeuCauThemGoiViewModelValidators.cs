using System.Linq;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauThemGoi>))]
    public class YeuCauThemGoiViewModelValidators : AbstractValidator<YeuCauThemGoi>
    {
        public YeuCauThemGoiViewModelValidators(ILocalizationService localizationService)
        {
            RuleFor(x => x.GoiDichVuId)
                   .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.GoiDichVuId.Required"));
            //RuleFor(x => x.DichVuChiDinhTheoGois)
            //    .Must((model, s) => s.Any()).WithMessage(localizationService.GetResource("ycdvcls.DichVuChiDinhTheoGois.Required"));
        }
    }
}
