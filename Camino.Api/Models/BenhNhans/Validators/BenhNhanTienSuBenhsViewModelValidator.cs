using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BenhNhans;
using Camino.Services.Localization;
using FluentValidation;
namespace Camino.Api.Models.BenhNhans.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BenhNhanTienSuBenhsViewModel>))]

    public class BenhNhanTienSuBenhsViewModelValidator : AbstractValidator<BenhNhanTienSuBenhsViewModel>
    {
        public BenhNhanTienSuBenhsViewModelValidator(ILocalizationService localizationService, IBenhNhanService benhNhanService)
        {
            RuleFor(x => x.TenBenh)
              .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.TenBenh.Required"))
              .MustAsync(async (viewModel, ten, id) =>
              {
                  var kiemTraExists = await benhNhanService.CheckTienSuBenhExists(viewModel.LoaiTienSuBenh, viewModel.TenBenh, viewModel.TenTienSuBenhs);
                  return kiemTraExists;
              })
              .WithMessage(localizationService.GetResource("BenhNhan.TenBenh.Exists"));

            RuleFor(x => x.LoaiTienSuBenh)
            .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.LoaiTienSuBenh.Required"));
        }
    }
}
