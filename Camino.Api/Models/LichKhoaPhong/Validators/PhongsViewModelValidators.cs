using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LichKhoaPhong.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhongsViewModel>))]
    public class PhongsViewModelValidators : AbstractValidator<PhongsViewModel>
    {
        public PhongsViewModelValidators(ILocalizationService localizationService,
            IValidator<LichKhoaPhongViewModel> lichKhoaPhongViewValidator)
        {
            RuleForEach(x => x.Phong).SetValidator(lichKhoaPhongViewValidator);
        }
    }
}
