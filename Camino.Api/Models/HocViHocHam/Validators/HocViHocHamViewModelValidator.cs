using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.HocViHocHam;
using Camino.Services.Localization;
using FluentValidation;


namespace Camino.Api.Models.HocViHocHam.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HocViHocHamViewModel>))]
    public class HocViHocHamViewModelValidator : AbstractValidator<HocViHocHamViewModel>
    {
        public HocViHocHamViewModelValidator(ILocalizationService iLocalizationService, IHocViHocHamService hocViHocHamService)
        {
            RuleFor(x => x.Ma)
               .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required"))
               .Length(0, 50).WithMessage(iLocalizationService.GetResource("Common.Ma.Range"))
                .Must((model, s) => !hocViHocHamService.CheckMaSoExits(model.Ma, model.Id))
                .WithMessage(iLocalizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(iLocalizationService.GetResource("Common.Ten.Range"))
                .Must((model, s) => !hocViHocHamService.CheckTenExits(model.Ten, model.Id))
                    .WithMessage(iLocalizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.MoTa)
                //.NotEmpty().WithMessage(iLocalizationService.GetResource("Common.MoTa.Required"))
                .Length(0, 2000).WithMessage(iLocalizationService.GetResource("Common.MoTa.Range")) ;

        }
    }
}
