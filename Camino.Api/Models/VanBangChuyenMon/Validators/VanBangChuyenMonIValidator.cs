using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.VanBangChuyenMon;
using FluentValidation;

namespace Camino.Api.Models.VanBangChuyenMon.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<VanBangChuyenMonViewModel>))]
    public class VanBangChuyenMonIValidator : AbstractValidator<VanBangChuyenMonViewModel>
    {
        public VanBangChuyenMonIValidator(ILocalizationService iLocalizationService, IVanBangChuyenMonService vanBangChuyenMonService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required"))
                .MustAsync(async (model, input, s) => !await vanBangChuyenMonService.KiemTraMaTonTai(model.Id, input))
                .WithMessage(iLocalizationService.GetResource("Common.Ma.Exists"));
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(iLocalizationService.GetResource("Common.Ten.Range"))
                .Must((model, s) => !vanBangChuyenMonService.CheckTrinhDoChuyenMonExits(model.Ten, model.Id,true))
                .WithMessage(iLocalizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.TenVietTat)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.TenVietTat.Required"))
                 .Length(0, 250).WithMessage(iLocalizationService.GetResource("Common.TenVietTat.Range"))
                 .Must((model, s) => !vanBangChuyenMonService.CheckTrinhDoChuyenMonExits(model.TenVietTat, model.Id, false))
                    .WithMessage(iLocalizationService.GetResource("Common.TenVietTat.Exists")); ;


            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(iLocalizationService.GetResource("Common.MoTa.Range"));
           
        }
    }
}
