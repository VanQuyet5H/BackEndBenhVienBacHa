using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhVatTu;
using FluentValidation;

namespace Camino.Api.Models.LinhBuVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhBuVatTuChiTietViewModel>))]
    public class LinhBuVatTuChiTietViewModelValidator : AbstractValidator<LinhBuVatTuChiTietViewModel>
    {
        public LinhBuVatTuChiTietViewModelValidator(ILocalizationService localizationService, IYeuCauLinhVatTuService ycLinhBuVatTuService)
        {
            //RuleFor(x => x.SoLuong)
            //    .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
            //    .MustAsync(async (viewModel, soLuongTon, id) =>
            //    {
            //        var kiemTraSLTon = await ycLinhBuVatTuService.CheckSoLuongTonVatTu(viewModel.VatTuBenhVienId, viewModel.SoLuong);
            //        return kiemTraSLTon;
            //    })
            //    .WithMessage(localizationService.GetResource("YeuCauLinhVatTu.SLYeuCau.NotValid"));
        }
    }
}
