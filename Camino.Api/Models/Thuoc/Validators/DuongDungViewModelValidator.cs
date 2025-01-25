using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.Thuocs;

namespace Camino.Api.Models.Thuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuongDungViewModel>))]
    public class DuongDungViewModelValidator : AbstractValidator<DuongDungViewModel>
    {
        public DuongDungViewModelValidator(ILocalizationService localizationService, IDuongDungService duongDungService)
        {

            RuleFor(x => x.Ten)
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) =>
                {
                    var checkExistsTen = await duongDungService.IsTenExists(ten, request.Id);
                    return checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.Ma)
                .Length(0, 50).WithMessage(localizationService.GetResource("Common.Ma.Range"))
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required")).MustAsync(async (request, ma, id) =>
                {
                    var checkExistsMa = await duongDungService.IsMaExists(ma, request.Id);
                    return checkExistsMa;
                })
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));
        }
    }
}
