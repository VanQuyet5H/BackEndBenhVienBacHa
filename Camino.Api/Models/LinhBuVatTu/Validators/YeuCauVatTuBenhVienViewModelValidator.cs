using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
namespace Camino.Api.Models.LinhBuVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauVatTuBenhVienViewModel>))]

    public class YeuCauVatTuBenhVienViewModelValidator : AbstractValidator<YeuCauVatTuBenhVienViewModel>
    {
        public YeuCauVatTuBenhVienViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SLYeuCauLinhThucTe)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                 .Must((viewModel, soLuongTon, id) =>
                 {
                     if (viewModel.SLYeuCauLinhThucTe != null && viewModel.SLYeuCauLinhThucTe != 0 && viewModel.SLYeuCauLinhThucTe > viewModel.SLYeuCauLinhThucTeMax && viewModel.CheckBox)
                         return false;
                     return true;
                 })
                .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.OutRegulated"));
        }
    }
}
