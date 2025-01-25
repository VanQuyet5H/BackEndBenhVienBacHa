using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModel>))]

    public class PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModelValidator : AbstractValidator<PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModel>
    {
        public PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {

            RuleFor(x => x.ICDId)
              .NotEmpty().WithMessage(localizationService.GetResource("Common.ICDId.Required"))
              .MustAsync(async (request, input, id) =>
              {
                  var val = await dieuTriNoiTruService.IsTenICDKemTheoExists(input, request.Id, request.NoiTruPhieuDieuId);
                  return val;
              }).WithMessage(localizationService.GetResource("Common.ICDId.Exists"));
        }
    }
}
