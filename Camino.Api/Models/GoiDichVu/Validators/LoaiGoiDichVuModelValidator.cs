using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.GoiDichVuMarketings;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.GoiDichVu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LoaiGoiDichVuModel>))]

    public class LoaiGoiDichVuModelValidator : AbstractValidator<LoaiGoiDichVuModel>
    {
        public LoaiGoiDichVuModelValidator(ILocalizationService localizationService, IChuongTrinhMarketingGoiDvService chuongTrinhMarketingGoiDvService)
        {
            RuleFor(x => x.Ma)
                 .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required")).MustAsync(async (request, ten, id) =>
                 {
                     var checkExistsTen = await chuongTrinhMarketingGoiDvService.IsMaTenExists(request.Ma, request.Id, false);
                     return checkExistsTen;
                 })
                 .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, tenVT, id) =>
                {
                    var checkExistsTenVT = await chuongTrinhMarketingGoiDvService.IsMaTenExists(request.Ten, request.Id, true);
                    return checkExistsTenVT;
                })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));


        }
    }
}
