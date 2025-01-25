using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhamDoan;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GoiKhamSucKhoeViewModel>))]

    public class KhamSucKhoeDoanViewModelValidator : AbstractValidator<GoiKhamSucKhoeViewModel>
    {
        public KhamSucKhoeDoanViewModelValidator(ILocalizationService localizationService, IKhamDoanService khamDoanService)
        {
            RuleFor(x => x.CongTyKhamSucKhoeId)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.CongTyKhamSucKhoeId.Required"));

            RuleFor(x => x.HopDongKhamSucKhoeId)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.HopDongKhamSucKhoeId.Required"));

            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.MaGoiKham.Required"))
                .MustAsync(async (viewModel, ma, id) =>
                {
                    return await khamDoanService.IsMaGoiKhamExists(ma, viewModel.Id, viewModel.HopDongKhamSucKhoeId);
                })
                .WithMessage(localizationService.GetResource("KhamDoan.MaGoiKham.Exists"));

            RuleFor(x => x.Ten)
                   .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.TenGoiKham.Required"))
                   .MustAsync(async (viewModel, ten, id) =>
                   {
                       return await khamDoanService.IsTenGoiKhamExists(ten, viewModel.Id, viewModel.HopDongKhamSucKhoeId);
                   })
                  .WithMessage(localizationService.GetResource("KhamDoan.TenGoiKham.Exists"));
        }
    }
}
