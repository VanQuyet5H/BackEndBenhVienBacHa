using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhamDoan;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GoiKhamSucKhoeChungViewModel>))]

    public class KhamSucKhoeDoanChungViewModelValidator : AbstractValidator<GoiKhamSucKhoeChungViewModel>
    {
        public KhamSucKhoeDoanChungViewModelValidator(ILocalizationService localizationService,
                                                      IKhamDoanService khamDoanService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.MaGoiKham.Required"))
                .MustAsync(async (viewModel, ma, id) =>
                {
                    return await khamDoanService.IsMaGoiKhamExistsChung(ma, viewModel.Id);
                })
                .WithMessage(localizationService.GetResource("KhamDoan.MaGoiKham.Exists"));

            RuleFor(x => x.Ten)
                   .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.TenGoiKham.Required"))
                   .MustAsync(async (viewModel, ten, id) =>
                   {
                       return await khamDoanService.IsTenGoiKhamExistsChung(ten, viewModel.Id);
                   })
                  .WithMessage(localizationService.GetResource("KhamDoan.TenGoiKham.Exists"));
        }
    }
}
