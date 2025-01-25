using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhDuocPham;
using FluentValidation;

namespace Camino.Api.Models.LinhBuDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauDuocPhamBenhVienViewModel>))]
    public class YeuCauDuocPhamBenhVienViewModelValidator : AbstractValidator<YeuCauDuocPhamBenhVienViewModel>
    {
        public YeuCauDuocPhamBenhVienViewModelValidator(ILocalizationService localizationService, IYeuCauLinhDuocPhamService ycLinhBuDuocPhamService)
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
