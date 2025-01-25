using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.ChucVu;

namespace Camino.Api.Models.ChucVu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChucVuViewModel>))]

    public class ChucVuViewModelValidator : AbstractValidator<ChucVuViewModel>
    {
        public ChucVuViewModelValidator(ILocalizationService localizationService, IChucVuService chucVuService)
        {

            RuleFor(x => x.Ten)
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) =>
                {
                    var checkExistsTen = await chucVuService.IsTenExists(ten, request.Id);
                    return checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.TenVietTat)
                .Length(0, 50).WithMessage(localizationService.GetResource("Common.TenVietTat.Range"))
                .NotEmpty().WithMessage(localizationService.GetResource("Common.TenVietTat.Required")).MustAsync(async (request, tenVT, id) =>
                {
                    var checkExistsTenVT = await chucVuService.IsTenVietTatExists(tenVT, request.Id);
                    return checkExistsTenVT;
                })
                .WithMessage(localizationService.GetResource("Common.TenVietTat.Exists"));
            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));
        }
    }
}
