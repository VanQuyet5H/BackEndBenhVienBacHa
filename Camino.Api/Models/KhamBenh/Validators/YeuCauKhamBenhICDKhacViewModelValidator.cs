using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauKhamBenh;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauKhamBenhICDKhacViewModel>))]
    public class YeuCauKhamBenhICDKhacViewModelValidator : AbstractValidator<YeuCauKhamBenhICDKhacViewModel>
    {
        public YeuCauKhamBenhICDKhacViewModelValidator(ILocalizationService localizationService, IYeuCauKhamBenhService yeuCauKhamBenhService)
        {
            //RuleFor(x => x.ICDId)
            //       .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenhICDKhac.ICDId.Required"));
            RuleFor(x => x.ICDId)
              .NotEmpty().WithMessage(localizationService.GetResource("Common.ICDId.Required"))
              .MustAsync(async (request, input, id) =>
              {
                  var val = await yeuCauKhamBenhService.IsTenICDKhacExists(input, request.Id, request.YeuCauKhamBenhId);

                  return val;
              }).WithMessage(localizationService.GetResource("Common.ICDId.Exists"));
        }
    }
}
