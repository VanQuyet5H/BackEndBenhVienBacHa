
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauMuaDuTruVatTu;
using FluentValidation;

namespace Camino.Api.Models.DuTruMuaVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<VatTuDuTruGridViewModel>))]

    public class VatTuDuTruGridViewModelValidator : AbstractValidator<VatTuDuTruGridViewModel>
    {
        public VatTuDuTruGridViewModelValidator(ILocalizationService localizationService, IYeuCauMuaDuTruVatTuService ycMuaVatTuService)
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
           
            //RuleFor(x => x.DVT)
            //  .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.DonViTinh.Required")).When(p => p.IsThemVatTu);
        }
    }
}
