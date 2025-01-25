using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HopDongSucKhoeDiaDiemViewModel>))]
    public class DiaDiemKhamViewModelValidator : AbstractValidator<HopDongSucKhoeDiaDiemViewModel>
    {
        public DiaDiemKhamViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DiaDiem).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.DiaDiemKham.Required"));
            RuleFor(p => p.CongViecId).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.CongViecKham.Required"));
            RuleFor(p => p.Ngay).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.DiaDiemNgayKham.Required"));          
        }
    }
}
