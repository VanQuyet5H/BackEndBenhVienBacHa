using Camino.Api.Models.KhamTheoDoiBoPhanKhac;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.PhauThuatThuThuat;
using FluentValidation;

namespace Camino.Api.Models.KhamTheoDoi.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamTheoDoiViewModel>))]
    public class KhamTheoDoiViewModelValidator : AbstractValidator<KhamTheoDoiViewModel>
    {
        public KhamTheoDoiViewModelValidator(ILocalizationService localizationService, IKhamTheoDoiService khamTheoDoiService, IValidator<KhamTheoDoiBoPhanKhacViewModel> khamTheoDoiBoPhanKhacValidator)
        {
            //this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.KhamToanThan)
                .MaximumLength(4000).WithMessage(localizationService.GetResource("PTTT.TheoDoi.KhamToanThan.Range"));

            RuleForEach(p => p.KhamTheoDoiBoPhanKhacs)
                .SetValidator(khamTheoDoiBoPhanKhacValidator);
        }
    }
}