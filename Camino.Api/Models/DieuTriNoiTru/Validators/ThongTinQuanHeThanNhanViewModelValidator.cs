using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThongTinQuanHeThanNhanVo>))]

    public class ThongTinQuanHeThanNhanViewModelValidator : AbstractValidator<ThongTinQuanHeThanNhanVo>
    {
        public ThongTinQuanHeThanNhanViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.HoTen)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));
        }
    }
}
