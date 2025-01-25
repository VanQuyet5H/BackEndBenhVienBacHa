using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.LoaiPhongBenh.LoaiPhongBenhNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LoaiPhongBenh.LoaiPhongBenhNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LoaiPhongBenhNoiTruViewModel>))]
    public class LoaiPhongBenhNoiTruViewModelValidator
        : AbstractValidator<LoaiPhongBenhNoiTruViewModel>
    {
        public LoaiPhongBenhNoiTruViewModelValidator(
                ILocalizationService localizationService,
                ILoaiPhongBenhNoiTruService loaiPhongBenhNoiTruService
            )
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService
                .GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService
                .GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await loaiPhongBenhNoiTruService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService
                .GetResource("Common.MoTa.Range"));
        }
    }
}
