using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuChamSocSoSinhViewModel>))]

    public class PhieuChamSocSoSinhViewModelValidator : AbstractValidator<PhieuChamSocSoSinhViewModel>
    {
        public PhieuChamSocSoSinhViewModelValidator(ILocalizationService localizationService, IValidator<ThongTinHoSoPhieuChamSocSoSinhViewModel> thongTinValidator)
        {
            RuleForEach(x => x.ThongTinHoSoPhieuChamSocSoSinhs).SetValidator(thongTinValidator);
        }
    }
}
