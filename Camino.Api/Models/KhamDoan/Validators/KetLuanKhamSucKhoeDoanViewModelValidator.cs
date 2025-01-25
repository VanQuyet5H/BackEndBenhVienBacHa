using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhamDoan;
using Camino.Services.Localization;
using FluentValidation;


namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KetLuanKhamSucKhoeDoanViewModel>))]

    public class KetLuanKhamSucKhoeDoanViewModelValidator : AbstractValidator<KetLuanKhamSucKhoeDoanViewModel>
    {
        public KetLuanKhamSucKhoeDoanViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PhanLoaiSucKhoeId)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.PhanLoaiSucKhoeId.Required"))
                .When(x => x.LaGetKetQuaMau != true && x.CoHienThiPhanLoai);
        }
    }
}
