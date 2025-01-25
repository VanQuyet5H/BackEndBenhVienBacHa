using Camino.Api.Models.NoiGioiThieu;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NoiGioiThieu;
using FluentValidation;

namespace Camino.Api.Models.NoiDungMauLoiDanBacSi.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DonViMauViewModel>))]

    public class DonViMauViewModelValidator : AbstractValidator<DonViMauViewModel>
    {
        public DonViMauViewModelValidator(ILocalizationService localizationService, INoiGioiThieuService noiGioiThieuService)
        {
            RuleFor(x => x.Ma)
                 .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required")).MustAsync(async (request, ten, id) =>
                 {
                     var checkExistsTen = await noiGioiThieuService.IsMaTenExists(request.Ma, request.Id, false);
                     return checkExistsTen;
                 })
                 .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, tenVT, id) =>
                {
                    var checkExistsTenVT = await noiGioiThieuService.IsMaTenExists(request.Ten, request.Id, true);
                    return checkExistsTenVT;
                })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));


        }
    }
}
