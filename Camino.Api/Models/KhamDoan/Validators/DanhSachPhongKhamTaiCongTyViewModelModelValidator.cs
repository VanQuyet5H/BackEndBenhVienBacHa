using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DanhSachPhongKhamTaiCongTyViewModel>))]
    public class DanhSachPhongKhamTaiCongTyViewModelModelValidator : AbstractValidator<DanhSachPhongKhamTaiCongTyViewModel>
    {
        public DanhSachPhongKhamTaiCongTyViewModelModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Ma).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.MaPhongCongTy.Required"));
            RuleFor(p => p.Ten).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.TenPhongCongTy.Required"));
            RuleFor(p => p.Tang).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.TangCongTy.Required"));          
        }
    }
}
