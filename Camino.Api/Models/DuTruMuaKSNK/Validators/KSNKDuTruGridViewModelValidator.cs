
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauMuaDuTruKiemSoatNhiemKhuan;
using FluentValidation;

namespace Camino.Api.Models.DuTruMuaKSNK.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KSNKDuTruGridViewModel>))]

    public class KSNKDuTruGridViewModelValidator : AbstractValidator<KSNKDuTruGridViewModel>
    {
        public KSNKDuTruGridViewModelValidator(ILocalizationService localizationService, IYeuCauMuaDuTruKiemSoatNhiemKhuanService ycMuaVatTuService)
        {
            RuleFor(x => x.VatTuId)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).When(p => !p.IsThemVatTu)
                .MustAsync(async (viewModel, soLuongTon, id) =>
                {
                    var kiemTraExists = await ycMuaVatTuService.CheckVatTuExists(viewModel.VatTuId, viewModel.LaVatTuBHYT, viewModel.YeuCauMuaVatTuChiTietValidators);
                    return kiemTraExists;
                })
              .WithMessage(localizationService.GetResource("YeuCauMuaDuTruVatTu.VatTu.Exists")).When(p => !p.IsThemVatTu);

            RuleFor(x => x.SoLuongDuTru)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.SoLuongDuTru.Required"));

            RuleFor(x => x.SoLuongDuKienSuDung)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.SoLuongDuKienSuDungTrongKy.Required"));

            RuleFor(x => x.NhomVatTuId)
             .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruVatTu.NhomVatTu.Required")).When(p => p.IsThemVatTu);

            RuleFor(x => x.Ten)
              .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).When(p => p.IsThemVatTu);
        }
    }
}
